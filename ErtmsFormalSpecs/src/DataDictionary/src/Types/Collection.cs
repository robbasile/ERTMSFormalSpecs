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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;

namespace DataDictionary.Types
{
    public class Collection : Generated.Collection, ITypedElement
    {
        public override string ExpressionText
        {
            get
            {
                string retVal = Default;
                if (retVal == null)
                {
                    retVal = "";
                }
                return retVal;
            }
            set { Default = value; }
        }

        /// <summary>
        ///     Provides the mode of the typed element
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return acceptor.VariableModeEnumType.defaultVariableModeEnumType; }
        }

        /// <summary>
        ///     Provides the type name of the element
        /// </summary>
        public string TypeName
        {
            get { return getTypeName(); }
            set { setTypeName(value); }
        }

        /// <summary>
        ///     The type associated to this structure element
        /// </summary>
        private Type type;

        public virtual Type Type
        {
            get
            {
                if (type == null)
                {
                    type = EFSSystem.FindType(NameSpace, getTypeName());
                }
                return type;
            }
            set
            {
                if (value != null)
                {
                    setTypeName(value.getName());
                }
                else
                {
                    setTypeName(null);
                }
                type = value;
            }
        }

        public override ArrayList EnclosingCollection
        {
            get { return NameSpace.Collections; }
        }

        /// <summary>
        ///     Compares two lists for equality
        /// </summary>
        /// <param name="first"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool CompareForEquality(IValue first, IValue other)
        {
            bool retVal = false;

            ListValue list1 = first as ListValue;
            ListValue list2 = other as ListValue;
            if (list1 != null && list2 != null)
            {
                if (list1.ElementCount == list2.ElementCount)
                {
                    retVal = true;
                    foreach (IValue val1 in list1.Val)
                    {
                        if (!(val1 is EmptyValue))
                        {
                            bool found = false;

                            foreach (IValue val2 in list2.Val)
                            {
                                if (val1.Type.CompareForEquality(val1, val2))
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                retVal = false;
                                break;
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        public override bool Contains(IValue first, IValue other)
        {
            bool retVal = false;

            ListValue listValue = first as ListValue;
            if (listValue != null)
            {
                foreach (IValue value in listValue.Val)
                {
                    StateMachine stateMachine = value.Type as StateMachine;
                    if (stateMachine != null)
                    {
                        if (stateMachine.Contains(value, other))
                        {
                            retVal = true;
                            break;
                        }
                    }
                    else
                    {
                        if (value.Type.CompareForEquality(value, other))
                        {
                            retVal = true;
                            break;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates if the type is abstract
        /// </summary>
        public override bool IsAbstract
        {
            get
            {
                bool result = false;
                Collection collection = Type as Collection;
                if (collection != null)
                {
                    result = collection.IsAbstract;
                }
                else
                {
                    Structure structure = Type as Structure;
                    if (structure != null)
                    {
                        result = structure.IsAbstract;
                    }
                }
                return result;
            }
        }

        /// <summary>
        ///     Parses the image and provides the corresponding value
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override IValue getValue(string image)
        {
            IValue retVal = null;

            Expression expression = new Parser().Expression(this, image);
            if (expression != null)
            {
                retVal = expression.GetExpressionValue(new InterpretationContext(this), null);
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that this collection matches the other collections
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            if (!retVal && otherType is Collection)
            {
                Collection otherCollection = (Collection) otherType;

                if (Type != null)
                {
                    if (otherCollection.Type != null)
                    {
                        retVal = Type.Match(otherCollection.Type);
                    }
                    else
                    {
                        // null type for a collection means "any type"
                        retVal = true;
                    }
                }
                else
                {
                    // null type for a collection means "any type"
                    retVal = true;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            base.GetExplain(explanation, explainSubElements);
            explanation.Write("COLLECTION ");
            explanation.Write(Name);
            explanation.Write(" OF ");
            explanation.WriteLine(getTypeName());
        }

        /// <summary>
        ///     Creates a copy of the collection in the designated dictionary. The namespace structure is copied over.
        ///     The new collection is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public Collection CreateCollectionUpdate(Dictionary dictionary)
        {
            Collection retVal = (Collection) Duplicate();
            retVal.setUpdates(Guid);
            retVal.ClearAllRequirements();

            String[] names = FullName.Split('.');
            names = names.Take(names.Count() - 1).ToArray();
            NameSpace nameSpace = dictionary.GetNameSpaceUpdate(names, Dictionary);
            nameSpace.appendCollections(retVal);

            return retVal;
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string result = base.CreateStatusMessage();

            result += "Collection selected";

            return result;
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static Collection CreateDefault(ICollection enclosingCollection)
        {
            Collection retVal = (Collection) acceptor.getFactory().createCollection();

            Util.DontNotify(() => { retVal.Name = "Collection" + GetElementNumber(enclosingCollection); });

            return retVal;
        }

        public override IValue PerformArithmericOperation(InterpretationContext context, IValue left, BinaryExpression.Operator Operation, IValue right)
        {
            ListValue retVal = null;

            switch (Operation)
            {
                case BinaryExpression.Operator.Add:
                    Collection leftType = left.Type as Collection;
                    Collection rightType = right.Type as Collection;
                    Collection returnType;

                    if (leftType != null && rightType != null)
                    {
                        if (leftType is GenericCollection)
                        {
                            if (rightType is GenericCollection)
                            {
                                returnType = new GenericCollection(EFSSystem);
                            }
                            else
                            {
                                returnType = rightType;
                            }
                        }
                        else
                        {
                            if (leftType == rightType)
                            {
                                returnType = leftType;
                            }
                            else if (leftType.Type == rightType.Type)
                            {
                                returnType = leftType;
                            }
                            else
                            {
                                throw new Exception("Cannot determine the collection type for expression");
                            }
                        }

                        ListValue leftValue = left as ListValue;
                        ListValue rightValue = right as ListValue;
                        if (leftValue != null && rightValue != null)
                        {
                            retVal = new ListValue(returnType, new List<IValue>());
                            foreach (IValue val in leftValue.Val)
                            {
                                if (!(val is EmptyValue))
                                {
                                    retVal.Val.Add(val.RightSide(null, true, true));                                    
                                }
                            }
                            foreach (IValue val in rightValue.Val)
                            {
                                if (!(val is EmptyValue))
                                {
                                    retVal.Val.Add(val.RightSide(null, true, true));
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Cannot add a collection to a single element");
                        }
                    }
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Converts a value in this type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public override IValue convert(IValue value)
        {
            ListValue retVal = value as ListValue;

            if (retVal != null)
            {
                if (retVal.ElementCount <= getMaxSize())
                {
                    foreach (IValue val in retVal.Val)
                    {
                        if (!Type.Match(val.Type))
                        {
                            throw new Exception("Invalid element type in collection " + val.LiteralName);
                        }
                    }
                }
                else
                {
                    throw new Exception("Too many elements in collection");
                }
            }

            return retVal;
        }
    }

    /// <summary>
    ///     A generic collection
    /// </summary>
    public class GenericCollection : Collection
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public GenericCollection(EfsSystem efsSystem)
        {
            Enclosing = efsSystem;
        }

        /// <summary>
        ///     The type of the elements in this collection
        /// </summary>
        public override Type Type
        {
            get { return EFSSystem.AnyType; }
        }
    }
}