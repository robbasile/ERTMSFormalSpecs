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
using DataDictionary.Constants;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Interpreter.ListOperators;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;

namespace DataDictionary.Interpreter
{
    public class Designator : InterpreterTreeNode, IReference, ITextualExplain
    {
        /// <summary>
        ///     Provides the designator image
        /// </summary>
        public string Image { get; private set; }

        /// <summary>
        ///     Predefined designators
        /// </summary>
        public const string THIS = "THIS";

        public const string ENCLOSING = "ENCLOSING";

        /// <summary>
        ///     Indicates that the designator is predefined
        /// </summary>
        /// <returns></returns>
        public bool IsPredefined()
        {
            return Image == THIS || Image == ENCLOSING;
        }

        /// <summary>
        ///     Indicates whether this designator references
        ///     - an element from the stack
        ///     - an element from the model
        ///     - an element from the current instance
        ///     - reference to THIS
        ///     - reference to the enclosing structure
        /// </summary>
        public enum LocationEnum
        {
            NotDefined,
            Stack,
            Model,
            Instance,
            This,
            Enclosing
        };

        /// <summary>
        ///     The location referenced by this designator
        /// </summary>
        public LocationEnum Location
        {
            get
            {
                LocationEnum retVal = LocationEnum.NotDefined;

                if (Ref != null)
                {
                    if (Image == THIS)
                    {
                        retVal = LocationEnum.This;
                    }
                    else if (Image == ENCLOSING)
                    {
                        retVal = LocationEnum.Enclosing;
                    }
                    else if (Ref is Parameter)
                    {
                        retVal = LocationEnum.Stack;
                    }
                    else if (Ref is IVariable)
                    {
                        INamable current = INamableUtils.getEnclosing(Ref);
                        while (current != null && retVal == LocationEnum.NotDefined)
                        {
                            if ((current is ListOperatorExpression) ||
                                (current is Statement.Statement) ||
                                (current is StabilizeExpression) ||
                                (current is LetExpression))
                            {
                                ISubDeclarator subDeclarator = current as ISubDeclarator;
                                if (ISubDeclaratorUtils.ContainsValue(subDeclarator, Ref))
                                {
                                    retVal = LocationEnum.Stack;
                                }
                            }
                            else if (current is Structure)
                            {
                                retVal = LocationEnum.Instance;
                            }
                            else if (current is NameSpace)
                            {
                                retVal = LocationEnum.Model;
                            }

                            current = INamableUtils.getEnclosing(current);
                        }
                    }
                    else if (Ref is StructureElement)
                    {
                        retVal = LocationEnum.Instance;
                    }
                    else
                    {
                        retVal = LocationEnum.Model;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="enclosing">the enclosing tree node</param>
        /// <param name="image">The designator image</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public Designator(ModelElement root, ModelElement log, string image, int start, int end)
            : base(root, log, start, end)
        {
            Image = image;
        }

        /// <summary>
        ///     Provides the possible references for this designator (only available during semantic analysis)
        /// </summary>
        /// <param name="instance">the instance on which this element should be found.</param>
        /// <param name="expectation">the expectation on the element found</param>
        /// <param name="lastElement">Indicates that this element is the last one in a dereference chain</param>
        /// <returns></returns>
        public ReturnValue getReferences(INamable instance, BaseFilter expectation, bool lastElement)
        {
            ReturnValue retVal = new ReturnValue(this);

            if (instance == null)
            {
                // Special handling for THIS or ENCLOSING
                if (Image == THIS || Image == ENCLOSING)
                {
                    INamable currentElem = Root;
                    while (currentElem != null)
                    {
                        Type type = currentElem as Type;
                        if (type != null)
                        {
                            StateMachine stateMachine = type as StateMachine;
                            while (stateMachine != null)
                            {
                                type = stateMachine;
                                stateMachine = stateMachine.EnclosingStateMachine;
                            }


                            // Enclosing does not references state machines. 
                            if (!(Image == ENCLOSING && type is StateMachine))
                            {
                                retVal.Add(type);
                                return retVal;
                            }
                        }
                        currentElem = enclosing(currentElem);
                    }

                    return retVal;
                }

                // No enclosing instance. Try to first name of a . separated list of names
                //  . First in the enclosing expression
                InterpreterTreeNode current = this;
                while (current != null)
                {
                    ISubDeclarator subDeclarator = current as ISubDeclarator;
                    if (FillBySubdeclarator(subDeclarator, expectation, false, retVal) > 0)
                    {
                        // If this is the last element in the dereference chain, stop at first match
                        if (lastElement)
                        {
                            return retVal;
                        }
                        current = null;
                    }
                    else
                    {
                        current = current.Enclosing;
                    }
                }

                // . In the predefined elements
                addReference(EFSSystem.GetPredefinedItem(Image), expectation, false, retVal);
                if (lastElement && !retVal.IsEmpty)
                {
                    return retVal;
                }

                // . In the enclosing items, except the enclosing dictionary since dictionaries are handled in a later step
                INamable currentNamable = Root;
                while (currentNamable != null)
                {
                    ISubDeclarator subDeclarator = currentNamable as ISubDeclarator;
                    if (subDeclarator != null && !(subDeclarator is Dictionary))
                    {
                        if (FillBySubdeclarator(subDeclarator, expectation, false, retVal) > 0 && lastElement)
                        {
                            return retVal;
                        }
                    }

                    currentNamable = enclosingSubDeclarator(currentNamable);
                }

                // . In the dictionaries declared in the system
                foreach (Dictionary dictionary in EFSSystem.Dictionaries)
                {
                    if (FillBySubdeclarator(dictionary, expectation, false, retVal) > 0 && lastElement)
                    {
                        return retVal;
                    }

                    NameSpace defaultNameSpace = dictionary.findNameSpace("Default");
                    if (defaultNameSpace != null)
                    {
                        if (FillBySubdeclarator(defaultNameSpace, expectation, false, retVal) > 0 && lastElement)
                        {
                            return retVal;
                        }
                    }
                }
            }
            else
            {
                // The instance is provided, hence, this is not the first designator in the . separated list of designators
                bool asType = false;
                if (instance is ITypedElement && !(instance is State))
                {
                    // If the instance is a typed element, dereference it to its corresponding type
                    ITypedElement element = instance as ITypedElement;
                    if (element.Type != EFSSystem.NoType)
                    {
                        instance = element.Type;
                        asType = true;
                    }
                }

                // Find the element in all enclosing sub declarators of the instance
                while (instance != null)
                {
                    ISubDeclarator subDeclarator = instance as ISubDeclarator;
                    if (FillBySubdeclarator(subDeclarator, expectation, asType, retVal) > 0)
                    {
                        instance = null;
                    }
                    else
                    {
                        if (instance is Dictionary)
                        {
                            instance = enclosingSubDeclarator(instance);
                        }
                        else
                        {
                            instance = null;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a reference which satisfies the provided expectation in the result set
        /// </summary>
        /// <param name="namable"></param>
        /// <param name="expectation"></param>
        /// <param name="asType">Indicates that we had to move from instance to type to perform the deferencing</param>
        /// <param name="resultSet"></param>
        private int addReference(INamable namable, BaseFilter expectation, bool asType, ReturnValue resultSet)
        {
            int retVal = 0;

            if (namable != null)
            {
                if (expectation.AcceptableChoice(namable))
                {
                    if (asType)
                    {
                        if (!(namable is IValue) && !(namable is Type))
                        {
                            resultSet.Add(namable);
                            retVal += 1;
                        }
                        else if (namable is State)
                        {
                            // TODO : Refactor model to avoid this
                            resultSet.Add(namable);
                            retVal += 1;
                        }
                    }
                    else
                    {
                        resultSet.Add(namable);
                        retVal += 1;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the retVal result set according to the subDeclarator class provided as parameter
        /// </summary>
        /// <param name="subDeclarator">The subdeclarator used to get the image</param>
        /// <param name="expectation">The expectatino of the desired element</param>
        /// <param name="location">The location of the element found</param>
        /// <param name="asType">Indicates that we had to go from the values to the types to perform dereferencing</param>
        /// <param name="values">The return value to update</param>
        /// <return>the number of elements added</return>
        private int FillBySubdeclarator(ISubDeclarator subDeclarator, BaseFilter expectation, bool asType,
            ReturnValue values)
        {
            int retVal = 0;

            if (subDeclarator != null)
            {
                // Go to the beginning of the update chain 
                ISubDeclarator currentDeclarator = subDeclarator;
                ModelElement modelElement = subDeclarator as ModelElement;
                while (modelElement != null)
                {
                    if (modelElement.Updates != null)
                    {
                        currentDeclarator = modelElement.Updates as ISubDeclarator;
                    }
                    modelElement = modelElement.Updates;
                }

                while (currentDeclarator != null)
                {
                    // Adds the elements of the current declarator
                    List<INamable> tmp = new List<INamable>();
                    currentDeclarator.Find(Image, tmp);
                    foreach (INamable namable in tmp)
                    {
                        retVal += addReference(namable, expectation, asType, values);
                    }

                    // Follow the update chain
                    modelElement = currentDeclarator as ModelElement;
                    if (modelElement != null && modelElement.UpdatedBy.Count == 1)
                    {
                        currentDeclarator = modelElement.UpdatedBy[0] as ISubDeclarator;
                    }
                    else
                    {
                        currentDeclarator = null;
                    }
                }
            }

            values.ApplyUpdates();

            return retVal;
        }

        /// <summary>
        ///     The model element referenced by this designator.
        ///     This value can be null. In that case, reference should be done by dereferencing each argument of the Deref
        ///     expression
        /// </summary>
        public INamable Ref { get; set; }

        /// <summary>
        ///     Performs the semantic analysis of the term
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <para name="expectation">
        ///     Indicates the kind of element we are looking for</paraparam>
        ///     <param name="lastElement">Indicates that this element is the last one in a dereference chain</param>
        ///     <returns>True if semantic analysis should be continued</returns>
        public void SemanticAnalysis(INamable instance, BaseFilter expectation, bool lastElement)
        {
            ReturnValue tmp = getReferences(instance, expectation, lastElement);
            if (Image != THIS && Image != ENCLOSING)
            {
                tmp.filter(expectation);
            }
            if (tmp.IsUnique)
            {
                Ref = tmp.Values[0].Value;
            }

            if (StaticUsage == null)
            {
                StaticUsage = new Usages();
                StaticUsage.AddUsage(Ref, Root, null);
            }
        }

        /// <summary>
        ///     Provides the element referenced by this designator, given the enclosing element
        /// </summary>
        /// <param name="enclosing"></param>
        /// <returns></returns>
        public INamable getReference(InterpretationContext context)
        {
            INamable retVal = null;

            switch (Location)
            {
                case LocationEnum.Stack:
                    retVal = context.LocalScope.getVariable(Image);

                    if (retVal == null)
                    {
                        AddError(Image + " not found on the stack");
                    }
                    break;

                case LocationEnum.Instance:
                    INamable instance = context.Instance;

                    while (instance != null)
                    {
                        ISubDeclarator subDeclarator = instance as ISubDeclarator;
                        if (subDeclarator != null)
                        {
                            INamable tmp = getReferenceBySubDeclarator(subDeclarator);
                            if (tmp != null)
                            {
                                if (retVal == null)
                                {
                                    retVal = tmp;
                                    instance = null;
                                }
                            }
                        }

                        instance = enclosingSubDeclarator(instance);
                    }

                    if (retVal == null)
                    {
                        AddError(Image + " not found in the current instance " + context.Instance.Name);
                    }
                    break;

                case LocationEnum.This:
                    retVal = context.Instance;
                    break;

                case LocationEnum.Enclosing:
                    ITypedElement typedElement = context.Instance as ITypedElement;
                    while (typedElement != null && !(typedElement.Type is Structure))
                    {
                        IEnclosed enclosed = typedElement as IEnclosed;
                        if (enclosed != null)
                        {
                            typedElement = enclosed.Enclosing as ITypedElement;
                        }
                        else
                        {
                            typedElement = null;
                        }
                    }
                    retVal = typedElement;
                    break;


                case LocationEnum.Model:
                    retVal = Ref;

                    if (retVal == null)
                    {
                        AddError(Image + " not found in the enclosing model");
                    }
                    break;

                case LocationEnum.NotDefined:
                    AddError("Semantic analysis not performed on " + ToString());
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the enclosing element
        /// </summary>
        /// <param name="retVal"></param>
        /// <returns></returns>
        private INamable enclosing(INamable retVal)
        {
            IEnclosed enclosed = retVal as IEnclosed;

            if (enclosed != null)
            {
                retVal = enclosed.Enclosing as INamable;
            }
            else
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the enclosing sub declarator
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private INamable enclosingSubDeclarator(INamable instance)
        {
            INamable retVal = instance;

            do
            {
                retVal = enclosing(retVal);
            } while (retVal != null && !(retVal is ISubDeclarator));

            return retVal;
        }

        /// <summary>
        ///     Provides the reference for this subdeclarator
        /// </summary>
        /// <param name="subDeclarator"></param>
        /// <returns></returns>
        private INamable getReferenceBySubDeclarator(ISubDeclarator subDeclarator)
        {
            INamable retVal = null;

            List<INamable> tmp = new List<INamable>();
            subDeclarator.Find(Image, tmp);
            if (tmp.Count > 0)
            {
                // Remove duplicates
                List<INamable> tmp2 = new List<INamable>();
                foreach (INamable namable in tmp)
                {
                    bool found = false;
                    foreach (INamable other in tmp2)
                    {
                        if (namable == other)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        tmp2.Add(namable);

                        if (EFSSystem.INSTANCE.CheckParentRelationship && !(namable is EmptyValue))
                        {
                            // Consistency check. 
                            // Empty value should not be considered because we can dereference 'Empty'
                            IVariable subDeclVar = subDeclarator as Variable;
                            object enclosed = ((IEnclosed) namable).Enclosing;
                            if (subDeclVar != null)
                            {
                                if (enclosed != subDeclVar.Value)
                                {
                                    AddError(
                                        "Consistency check failed : enclosed element's father relationship is inconsistent");
                                }
                            }
                            else
                            {
                                if (enclosed != subDeclarator)
                                {
                                    // There is still an exception : when the element is declared in the default namespace
                                    if (subDeclarator != EFSSystem.INSTANCE ||
                                        enclosed != EFSSystem.INSTANCE.FindByFullName("Default"))
                                    {
                                        AddError(
                                            "Consistency check failed : enclosed element's father relationship is inconsistent");
                                    }
                                }
                            }
                        }
                    }
                }

                // Provide the result, if it is unique
                if (tmp2.Count == 1)
                {
                    retVal = tmp2[0];
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type designated by this designator
        /// </summary>
        /// <returns></returns>
        public Type GetDesignatorType()
        {
            Type retVal = null;

            if (Ref is ITypedElement)
            {
                retVal = (Ref as ITypedElement).Type;
            }
            else
            {
                retVal = Ref as Type;
            }

            if (retVal == null)
            {
                AddError("Cannot determine typed element referenced by " + ToString());
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public void fill(List<INamable> retVal, BaseFilter filter)
        {
            if (filter.AcceptableChoice(Ref))
            {
                retVal.Add(Ref);
            }
        }

        public IVariable GetVariable(InterpretationContext context)
        {
            IVariable retVal = null;

            INamable reference = getReference(context);
            retVal = reference as IVariable;

            return retVal;
        }

        public IValue GetValue(InterpretationContext context)
        {
            IValue retVal = null;

            INamable reference = getReference(context);

            // Deref the reference, if required
            if (reference is IVariable)
            {
                retVal = (reference as IVariable).Value;
            }
            else
            {
                retVal = reference as IValue;
            }

            return retVal;
        }

        public ICallable getCalled(InterpretationContext context)
        {
            ICallable retVal = getReference(context) as ICallable;

            if (retVal == null)
            {
                Type type = GetDesignatorType();
                if (type != null)
                {
                    retVal = type.CastFunction;
                }
            }

            return retVal;
        }

        public void checkExpression()
        {
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            explanation.Write(Image);
        }
    }
}