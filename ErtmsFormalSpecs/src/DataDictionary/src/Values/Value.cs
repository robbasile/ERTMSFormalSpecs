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
    public interface IValue : INamable, ITypedElement, TextualExplain
    {
        /// <summary>
        ///     Provides the EFS system in which this value is created
        /// </summary>
        EFSSystem EFSSystem { get; }

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

    public abstract class Value : IValue, IEnclosed
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
        ///     Provides the EFS system in which this value is created
        /// </summary>
        public EFSSystem EFSSystem
        {
            get
            {
                if (Type != null)
                {
                    return Type.EFSSystem;
                }
                return null;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        public Value(Type type)
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
                this.Enclosing = variable;
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
            get { return this.FullName; }
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
        /// <param name="text"></param>
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
        ///     Nothing to do
        /// </summary>
        public void ClearMessages()
        {
        }

        /// <summary>
        ///     Indicates that at least one message of type levelEnum is attached to the element
        /// </summary>
        /// <param name="levelEnum"></param>
        /// <returns></returns>
        public bool HasMessage(ElementLog.LevelEnum levelEnum)
        {
            return false;
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
        /// <param name="copy"></param>
        public void AddModelElement(IModelElement element)
        {
        }

        /// <summary>
        ///     Provides an RTF explanation of the value
        /// </summary>
        /// <returns></returns>
        public string getExplain()
        {
            return LiteralName;
        }

        /// <summary>
        ///     Provides an explanation of the enumeration
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel)
        {
            return TextualExplainUtilities.Pad("{" + LiteralName + "}", indentLevel);
        }

        /// <summary>
        ///     The explanation of the element
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public string getExplain(bool subElements)
        {
            return getExplain(0);
        }

        /// <summary>
        ///     Indicates if the element holds messages, or is part of a path to a message
        /// </summary>
        public MessagePathInfoEnum MessagePathInfo
        {
            get { return MessagePathInfoEnum.Nothing; }
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

    public abstract class BaseValue<CorrespondingType, StorageType> : Value
        where CorrespondingType : Type
    {
        /// <summary>
        ///     The actual value of this value
        /// </summary>
        private StorageType val;

        public StorageType Val
        {
            get { return val; }
            set { val = value; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="val"></param>
        public BaseValue(CorrespondingType type, StorageType val)
            : base(type)
        {
            Val = val;
        }
    }
}