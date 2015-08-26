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
using System.Text;
using DataDictionary.Types;
using DataDictionary.Variables;
using Utils;

namespace DataDictionary.Values
{
    /// <summary>
    ///     An empty value to fill the empty gaps in the collections
    /// </summary>
    public class EmptyValue : Value, ISubDeclarator
    {
        public override string Name
        {
            get { return "EMPTY"; }
            set { }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public EmptyValue(EfsSystem efsSystem)
            : base(efsSystem.AnyType)
        {
            InitDeclaredElements();
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();
        }

        /// <summary>
        ///     The elements declared by this declarator
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; set; }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            // Dereference of an empty value holds the empty value (not a null pointer exception-like thing)
            retVal.Add(this);
        }
    }

    public class ListValue : BaseValue<Collection, List<IValue>>
    {
        public override string Name
        {
            get
            {
                string retVal = "[";

                bool first = true;
                foreach (IValue value in Val)
                {
                    if (value != null)
                    {
                        if (!first)
                        {
                            retVal = retVal + ", ";
                        }
                        retVal = retVal + value.Name;
                        first = false;
                    }
                }
                retVal = retVal + "]";

                return retVal;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public ListValue(Collection type, List<IValue> val)
            : base(type, val)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="other"></param>
        public ListValue(ListValue other)
            : base(other.CollectionType, new List<IValue>())
        {
            foreach (IValue value in other.Val)
            {
                // TODO : This will cause an issue when going back in the time line. 
                // All values should be copied instead
                Val.Add(value);
            }
        }

        /// <summary>
        ///     The collection type associated to this list value
        /// </summary>
        public Collection CollectionType
        {
            get { return Type as Collection; }
        }

        /// <summary>
        ///     Creates a valid right side IValue, according to the target variable (left side)
        /// </summary>
        /// <param name="variable">The target variable</param>
        /// <param name="duplicate">Indicates that a duplication of the variable should be performed</param>
        /// <param name="setEnclosing">Indicates that the new value enclosing element should be set</param>
        /// <returns></returns>
        public override IValue RightSide(IVariable variable, bool duplicate, bool setEnclosing)
        {
            ListValue retVal = this;

            //  Complete the list with empty values
            Collection collectionType = variable.Type as Collection;
            if (collectionType != null)
            {
                EmptyValue emptyValue = EfsSystem.Instance.EmptyValue;
                while (retVal.Val.Count < collectionType.getMaxSize())
                {
                    retVal.Val.Add(emptyValue);
                }
            }

            if (setEnclosing)
            {
                retVal.Enclosing = variable;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the number of non empty elements in the list value
        /// </summary>
        public int ElementCount
        {
            get
            {
                int retVal = 0;

                foreach (IValue value in Val)
                {
                    if (!(value is EmptyValue))
                    {
                        retVal += 1;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Converts a structure value to its corresponding structure expression.
        ///     null entries correspond to the default value
        /// </summary>
        /// <returns></returns>
        public override string ToExpressionWithDefault()
        {
            StringBuilder retVal = new StringBuilder();

            retVal.Append("[");

            bool first = true;
            foreach (IValue value in Val)
            {
                if (value != null)
                {
                    if (!first)
                    {
                        retVal.Append(", ");
                    }
                    retVal.Append(value.ToExpressionWithDefault());
                    first = false;
                }
            }
            retVal.Append("]");

            return retVal.ToString();
        }
    }
}