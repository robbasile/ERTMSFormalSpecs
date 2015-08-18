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

using System.Collections;
using System.Collections.Generic;
using DataDictionary.Generated;
using DataDictionary.Types;
using DataDictionary.Variables;
using Utils;
using NameSpace = DataDictionary.Types.NameSpace;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Values
{
    public interface IValue : ITypedElement, ITextualExplain
    {
        /// <summary>
        ///     The complete name to access the value
        /// </summary>
        string LiteralName { get; }

        /// <summary>
        ///     Creates a valid right side IValue, according to the target variable (left side)
        /// </summary>
        /// <param name="variable">The target variable</param>
        /// <param name="duplicate">Indicates that a duplication of the variable should be performed</param>
        /// <param name="setEnclosing">Indicates that the new value enclosing element should be set</param>
        /// <returns></returns>
        IValue RightSide(IVariable variable, bool duplicate, bool setEnclosing);

        /// <summary>
        ///     Converts a structure value to its corresponding structure expression.
        ///     null entries correspond to the default value
        /// </summary>
        /// <returns></returns>
        string ToExpressionWithDefault();
    }

    public abstract class Value : IValue
    {
        public virtual string Name
        {
            get { return "<unnamed value>"; }
            set { }
        }

        public virtual string FullName
        {
            get { return Name; }
        }

        public virtual string LiteralName
        {
            get { return Name; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        protected Value(Type type)
        {
            Type = type;
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
            if (setEnclosing)
            {
                Enclosing = variable;
            }

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
        ///     The type of the element
        /// </summary>
        public Type Type { get; set; }

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
        ///     The enclosing model element
        /// </summary>
        public object Enclosing { get; set; }

        /// <summary>
        ///     Compares two values
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IModelElement other)
        {
            if (this == other)
            {
                return 0;
            }
            return -1;
        }

        /// <summary>
        ///     Nothing to do
        /// </summary>
        public void Delete()
        {
        }

        /// <summary>
        ///     The enclosing collection
        /// </summary>
        public ArrayList EnclosingCollection
        {
            get { return null; }
        }

        /// <summary>
        ///     The expression text data of this model element
        /// </summary>
        public string ExpressionText
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///     The messages logged on the model element
        /// </summary>
        public List<ElementLog> Messages
        {
            get { return null; }
        }

        /// <summary>
        ///     Clears the messages associated to this model element
        /// </summary>
        /// <param name="precise">
        ///     Indicates that the MessagePathInfo should be recomputed precisely
        ///     according to the sub elements and should update the enclosing elements
        /// </param>
        public void ClearMessages(bool precise)
        {
        }

        /// <summary>
        ///     Indicates if the element holds messages, or is part of a path to a message
        /// </summary>
        public MessageInfoEnum MessagePathInfo
        {
            get { return MessageInfoEnum.NoMessage; }
        }

        /// <summary>
        ///     Indicates that the element is removed
        /// </summary>
        public bool IsRemoved
        {
            get { return false; }
        }

        /// <summary>
        ///     The sub elements of this model element
        /// </summary>
        public ArrayList SubElements
        {
            get { return null; }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public void AddModelElement(IModelElement element)
        {
        }

        /// <summary>
        ///     Indicates whether this is a parent of the element.
        ///     It also returns true then parent==element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsParent(IModelElement element)
        {
            return element == this;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.WriteLine(LiteralName);
        }

        /// <summary>
        ///     The enclosing value, if exists
        /// </summary>
        public Value EnclosingValue
        {
            get { return EnclosingFinder<Value>.find(this); }
        }

        /// <summary>
        ///     Converts a structure value to its corresponding structure expression.
        ///     null entries correspond to the default value
        /// </summary>
        /// <returns></returns>
        public virtual string ToExpressionWithDefault()
        {
            return FullName;
        }
    }

    public abstract class BaseValue<TCorrespondingType, TStorageType> : Value
        where TCorrespondingType : Type
    {
        /// <summary>
        ///     The actual value of this value
        /// </summary>
        public TStorageType Val { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        protected BaseValue(TCorrespondingType type, TStorageType val)
            : base(type)
        {
            Val = val;
        }
    }
}