// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Variables;
using Utils;

namespace DataDictionary.Interpreter
{
    /// <summary>
    ///     The possible return values for InnerGetValue
    /// </summary>
    public class ReturnValue
    {
        /// <summary>
        ///     The interpreter tree node on which these values are linked
        /// </summary>
        public InterpreterTreeNode Node { get; private set; }

        /// <summary>
        ///     The values of this return value
        /// </summary>
        public List<ReturnValueElement> Values { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ReturnValue(InterpreterTreeNode node)
        {
            Node = node;
            Values = new List<ReturnValueElement>();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ReturnValue()
        {
            Node = null;
            Values = new List<ReturnValueElement>();
        }

        /// <summary>
        ///     Indicates if there is more than one value in the result set
        /// </summary>
        public bool IsAmbiguous
        {
            get { return Values.Count > 1; }
        }

        /// <summary>
        ///     Indicates if there is only one value in the result set
        /// </summary>
        public bool IsUnique
        {
            get { return Values.Count == 1; }
        }

        /// <summary>
        ///     Indicates if there is no more value in the result set
        /// </summary>
        public bool IsEmpty
        {
            get { return Values.Count == 0; }
        }

        /// <summary>
        ///     Adds a new value in the set of return values
        /// </summary>
        /// <param name="value">The value to add</param>
        /// <param name="previous">The previous element in the chain</param>
        /// <param name="asType"></param>
        public void Add(INamable value, ReturnValueElement previous = null, bool asType = false)
        {
            if (value != null)
            {
                ReturnValueElement element = new ReturnValueElement(value, previous, asType);
                foreach (ReturnValueElement elem in Values)
                {
                    if (elem.CompareTo(element) == 0)
                    {
                        element = null;
                        break;
                    }
                }

                if (element != null)
                {
                    Values.Add(element);
                }
            }
        }

        /// <summary>
        ///     Merges the other return value with this one
        /// </summary>
        /// <param name="previous">The previous ReturnValueElement which lead to this ReturnValueElement</param>
        /// <param name="other">The other return value to merge with</param>
        public void Merge(ReturnValueElement previous, ReturnValue other)
        {
            foreach (ReturnValueElement elem in other.Values)
            {
                Add(elem.Value, previous, elem.AsType);
            }
        }

        public override string ToString()
        {
            string retVal = "";

            if (Values.Count > 0)
            {
                foreach (ReturnValueElement elem in Values)
                {
                    if (!string.IsNullOrEmpty(retVal))
                    {
                        retVal = retVal + ", ";
                    }

                    retVal = retVal + elem.Value.FullName + "(" + elem.Value.GetType() + ")";
                }
            }
            else
            {
                retVal = "<nothing>";
            }

            return retVal;
        }

        /// <summary>
        ///     Filters out value according to predicate
        /// </summary>
        /// <param name="accept"></param>
        public void Filter(BaseFilter accept)
        {
            ApplyUpdates();
            DiscardRemoved();

            // Only keep the most specific elements.
            string mostSpecific = null;
            foreach (ReturnValueElement element in Values)
            {
                if (accept.AcceptableChoice(element.Value))
                {
                    if (mostSpecific == null)
                    {
                        mostSpecific = element.Value.FullName;
                    }
                    else
                    {
                        if (mostSpecific.Length < element.Value.FullName.Length)
                        {
                            mostSpecific = element.Value.FullName;
                        }
                    }
                }
            }

            // if the filtering is about variables, ensure that there is at least one variable in the element chain
            if (accept is IsVariable)
            {
                List<ReturnValueElement> tmp = new List<ReturnValueElement>();
                foreach (ReturnValueElement element in Values)
                {
                    bool variableFound = false;
                    bool onlyStructureElement = true;
                    ReturnValueElement current = element;
                    while (!variableFound && current != null)
                    {
                        variableFound = IsStrictVariableOrValue.INSTANCE.AcceptableChoice(current.Value) ||
                                        current.AsType;
                        onlyStructureElement = onlyStructureElement && current.Value is StructureElement;
                        current = current.PreviousElement;
                    }

                    if (variableFound)
                    {
                        tmp.Add(element);
                    }
                    else if (onlyStructureElement)
                    {
                        tmp.Add(element);
                    }
                }

                // HaCK : If tmp is empty, this indicates that the filter above is too restrictive. 
                // Keep the original set
                if (tmp.Count > 0)
                {
                    Values = tmp;
                }
            }

            // Build a new list with the filtered out elements
            bool variable = false;
            {
                List<ReturnValueElement> tmp = new List<ReturnValueElement>();
                foreach (ReturnValueElement element in Values)
                {
                    if (accept.AcceptableChoice(element.Value) &&
                        (mostSpecific == null || mostSpecific.Equals(element.Value.FullName)))
                    {
                        tmp.Add(element);
                        variable = variable || element.Value is IVariable;
                    }
                }
                Values = tmp;
            }

            // HaCK : If both Variable and StructureElement are found, only keep the variable
            if (variable)
            {
                List<ReturnValueElement> tmp = new List<ReturnValueElement>();
                foreach (ReturnValueElement element in Values)
                {
                    if (!(element.Value is StructureElement) && !(element.Value is Type))
                    {
                        tmp.Add(element);
                    }
                }

                Values = tmp;
            }
        }

        /// <summary>
        ///     Removes the elements that have been marked as removed from the model
        /// </summary>
        public void DiscardRemoved()
        {
            Values.RemoveAll(ValueIsRemoved);
        }

        /// <summary>
        ///     Indicates that the referenced value has been removed
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool ValueIsRemoved(ReturnValueElement element)
        {
            bool retVal = false;

            ModelElement model = element.Value as ModelElement;
            if (model != null)
            {
                retVal = model.IsRemoved;
            }

            return retVal;
        }

        /// <summary>
        ///     Applies data dictionary updates by merging structures and removing other updated model elements
        /// </summary>
        public void ApplyUpdates()
        {
            // We collect all the values that have been redefined to filter them out of the list
            HashSet<ModelElement> redefined = new HashSet<ModelElement>();
            foreach (ReturnValueElement element in Values)
            {
                ModelElement modelElement = element.Value as ModelElement;
                if (modelElement != null && modelElement.Updates != null)
                {
                    redefined.Add(modelElement.Updates);
                }
            }

            List<ReturnValueElement> tmp = new List<ReturnValueElement>();
            foreach (ReturnValueElement element in Values)
            {
                ModelElement modelElement = element.Value as ModelElement;
                if (modelElement != null)
                {
                    if (!redefined.Contains(modelElement))
                    {
                        // If the element is a structure, the uppdate must be combined with its base
                        Structure elementAsStruct = element.Value as Structure;
                        if (elementAsStruct != null)
                        {
                            element.Value = elementAsStruct.UnifiedStructure;
                        }

                        StateMachine elementAsSm = element.Value as StateMachine;
                        if (elementAsSm != null)
                        {
                            element.Value = elementAsSm.UnifiedStateMachine;
                        }

                        tmp.Add(element);
                    }
                }
                else
                {
                    tmp.Add(element);
                }
            }

            RemoveDuplicates(tmp);
            Values = tmp;
        }

        /// <summary>
        ///     According to updates, several path may lead to the same model element.
        ///     Since that model element is uniquely idenfied, keep only one instance
        /// </summary>
        /// <param name="list">The list of elements from which we remove duplicates</param>
        /// <returns>The list of individual ReturnValueElements derived from List</returns>
        private void RemoveDuplicates(List<ReturnValueElement> list)
        {
            List<ReturnValueElement> tmp = new List<ReturnValueElement>();
            if (list.Count > 0)
            {
                foreach (ReturnValueElement element in list)
                {
                    bool isPresent = false;
                    foreach (ReturnValueElement elem in tmp)
                    {
                        if (elem.CompareTo(element) == 0)
                        {
                            isPresent = true;
                        }
                    }

                    if (!isPresent)
                    {
                        tmp.Add(element);
                    }
                }
            }
            list = tmp;
        }

        /// <summary>
        ///     The empty return value
        /// </summary>
        public static ReturnValue Empty = new ReturnValue();
    }
}