﻿// ------------------------------------------------------------------------------
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
using DataDictionary.Values;
using Utils;
using NameSpace = DataDictionary.Types.NameSpace;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Variables
{
    /// <summary>
    /// A variable created by the system, for instance, a structure field, or a temporary variable for expressions
    /// </summary>
    public class SyntheticVariable : IVariable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public SyntheticVariable(object enclosing, string name, Type type)
        {
            Enclosing = enclosing;
            Name = name;
            Type = type;
        }

        public object Enclosing { get; set; }

        /// <summary>
        /// The name of the field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The fullname
        /// </summary>
        public string FullName
        {
            get
            {
                string retVal = Name;

                IEnclosed current = Enclosing as IEnclosed;
                while (current != null)
                {
                    INamable namable = current as INamable;
                    if (namable != null && !(namable is IValue))
                    {
                        retVal = namable.Name + "." + retVal;                        
                    }
                    current = current.Enclosing as IEnclosed;
                    if (current is Dictionary)
                    {
                        break;
                    }
                }

                return retVal;
            }
        }

        public int CompareTo(IModelElement other)
        {
            int retVal = -1;

            if (other != null)
            {
                retVal = Name.CompareTo(other.Name);
            }

            return retVal;
        }

        public ArrayList SubElements { get; set; }

        public ArrayList EnclosingCollection
        {
            get { return null; }
        }

        public void Delete()
        {
        }

        public string ExpressionText { get; set; }

        public List<ElementLog> Messages { get; private set; }

        public void ClearMessages(bool precise)
        {
        }

        public MessageInfoEnum MessagePathInfo { get; private set; }

        /// <summary>
        /// Indicates that an update removes this element
        /// </summary>
        public bool IsRemoved
        {
            get { return false; }
        }

        public void AddModelElement(IModelElement element)
        {
            // Nothing to do
        }

        public bool IsParent(IModelElement element)
        {
            return false;
        }

        public void ClearCache()
        {
            // Nothing to do
        }

        /// <summary>
        /// The structure element for which this field is built
        /// </summary>
        public Type Type{ get; set; }

        /// <summary>
        /// The field type name
        /// </summary>
        public string TypeName
        {
            get { return Type.FullName; } 
            set { }             
        }

        /// <summary>
        /// The enclosing structure element namespace
        /// </summary>
        public NameSpace NameSpace
        {
            get { return Type.NameSpace; }
        }

        /// <summary>
        /// The field mode, from its structure element
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return acceptor.VariableModeEnumType.aInternal; }
        }

        /// <summary>
        /// The default value string
        /// </summary>
        public string Default
        {
            get { return Type.Default; }
            set { }
        }

        /// <summary>
        /// The sub fields of this field
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements {
            get
            {
                Dictionary<string, List<INamable>> retVal = null;

                StructureValue structureValue = Value as StructureValue;
                if (structureValue != null)
                {
                    retVal = structureValue.DeclaredElements;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Reset the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
        }

        /// <summary>
        /// Finds in sub elements according to name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            StructureValue structureValue = Value as StructureValue;
            if (structureValue != null)
            {
                structureValue.Find(name, retVal);                
            }
        }

        /// <summary>
        /// The value stored in the field
        /// </summary>
        private IValue _value;

        /// <summary>
        /// The field's value
        /// </summary>
        public IValue Value
        {
            get { return _value; }
            set { 
                _value = value;
                if (_value is StructureValue)
                {
                    _value.Enclosing = this;
                }
            }
        }

        /// <summary>
        /// The default value 
        /// </summary>
        public IValue DefaultValue
        {
            get { return Type.DefaultValue; }
        }

        /// <summary>
        /// Actions to be taken when the variable's value changes
        /// </summary>
        /// <param name="cacheImpact"></param>
        public void HandleChange(CacheImpact cacheImpact)
        {
            Type.HandleChange(cacheImpact);
        }

        /// <summary>
        /// Indicates that the model is expanded
        /// </summary>
        public ExpandableEnum Expanded { get; set; }

    }
}
