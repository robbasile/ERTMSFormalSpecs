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
using DataDictionary.Generated;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Enum = DataDictionary.Types.Enum;
using NameSpace = DataDictionary.Types.NameSpace;
using Range = DataDictionary.Types.Range;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Constants
{
    public class EnumValue : Generated.EnumValue, IValue, IGraphicalDisplay
    {
        /// <summary>
        ///     The corresponding type
        /// </summary>
        public Type Type
        {
            get
            {
                Type retVal = null;

                if (Enum != null)
                {
                    retVal = Enum;
                }
                else if (Range != null)
                {
                    retVal = Range;
                }

                return retVal;
            }
            set { }
        }

        public string LiteralName
        {
            get
            {
                string retVal = "";

                if (Enum != null)
                {
                    retVal = Enum.Name + "." + Name;
                }
                else if (Range != null)
                {
                    retVal = Range.Name + "." + Name;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     The enclosing enumeration type
        /// </summary>
        public Enum Enum
        {
            get { return Enclosing as Enum; }
        }

        /// <summary>
        ///     The enclosing range
        /// </summary>
        public Range Range
        {
            get { return Enclosing as Range; }
        }

        public IValue Value
        {
            get
            {
                IValue retVal = this;

                if (Range != null)
                {
                    retVal = Range.getValue(getValue());
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the enclosing collection, for deletion purposes
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;

                if (Enum != null)
                {
                    retVal = Enum.Values;
                }
                else if (Range != null)
                {
                    retVal = Range.SpecialValues;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Creates a valid right side IValue, according to the target variable (left side)
        /// </summary>
        /// <param name="variable">The target variable</param>
        /// <param name="duplicate">Indicates that a duplication of the variable should be performed</param>
        /// <param name="setEnclosing">Indicates that the new value enclosing element should be set</param>
        /// <returns></returns>
        public virtual IValue RightSide(IVariable variable, bool duplicate, bool setEnclosing)
        {
            return this;
        }

        /// <summary>
        ///     The namespace related to the typed element
        /// </summary>
        public NameSpace NameSpace
        {
            get { return null; }
        }

        /// <summary>
        ///     Provides the type name of the element
        /// </summary>
        public string TypeName
        {
            get { return Type.FullName; }
            set { }
        }

        /// <summary>
        ///     Provides the mode of the typed element
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return acceptor.VariableModeEnumType.aInternal; }
        }

        /// <summary>
        ///     Provides the default value of the typed element
        /// </summary>
        public string Default
        {
            get { return FullName; }
            set { }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Comment(this);
            explanation.Write(Name);
            if (!String.IsNullOrEmpty(getValue()))
            {
                explanation.WriteLine(" : " + getValue());
            }
            else
            {
                explanation.WriteLine();
            }
        }

        /// <summary>
        ///     Converts a structure value to its corresponding structure expression.
        ///     null entries correspond to the default value
        /// </summary>
        /// <returns></returns>
        public string ToExpressionWithDefault()
        {
            return FullName;
        }


        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static EnumValue CreateDefault(ICollection enclosingCollection)
        {
            EnumValue retVal = (EnumValue) acceptor.getFactory().createEnumValue();

            Util.DontNotify(() =>
            {
                retVal.Name = "Value" + GetElementNumber(enclosingCollection);
                retVal.setValue("");
            });

            return retVal;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string GraphicalName { get { return Name; } }
        public bool Hidden { get; set; }
        public bool Pinned { get; set; }
    }
}