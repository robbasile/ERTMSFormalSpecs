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
using DataDictionary.Values;
using Utils;
using NameSpace = DataDictionary.Types.NameSpace;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Variables
{
    public class Actual : IVariable
    {
        /// <summary>
        /// The parameter for which this actual is created
        /// </summary>
        public Parameter Parameter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameter"></param>
        public Actual(Parameter parameter)
        {
            Parameter = parameter;
            DeclaredElements = null;
        }

        public object Enclosing {
            get { return Parameter.Enclosing; }
            set { }
        }

        public string Name
        {
            get { return Parameter.Name; } 
            set { }
        }

        public string FullName
        {
            get { return Name; }
        }

        public int CompareTo(IModelElement other)
        {
            return Name.CompareTo(other.Name);
        }

        public ArrayList SubElements
        {
            get { return null; }
        }

        public ArrayList EnclosingCollection
        {
            get { return null; }
        }

        public void Delete()
        {
        }

        public string ExpressionText {
            get { return null; }
            set { }
        }

        public List<ElementLog> Messages
        {
            get { return null; }
        }

        public void ClearMessages(bool precise)
        {
        }

        public MessageInfoEnum MessagePathInfo
        {
            get { return MessageInfoEnum.NoMessage; }              
        }

        public bool IsRemoved
        {
            get { return false; }            
        }

        public void AddModelElement(IModelElement element)
        {
        }

        public bool IsParent(IModelElement element)
        {
            return Parameter.IsParent(element);
        }

        public void ClearCache()
        {
            // Nothing to do
        }

        public NameSpace NameSpace
        {
            get { return Parameter.NameSpace; }
        }

        public string TypeName
        {
            get { return Parameter.TypeName; }
            set { }
        }

        public Type Type
        {
            get { return Parameter.Type; }
            set { }
        }

        public acceptor.VariableModeEnumType Mode
        {
            get { return Parameter.Mode; }
        }

        public string Default
        {
            get { return Parameter.Default; }
            set { }
        }

        public Dictionary<string, List<INamable>> DeclaredElements
        {
            get; 
            private set;
        }

        public void InitDeclaredElements()
        {
        }

        public void Find(string name, List<INamable> retVal)
        {
            StructureValue structureValue = Value as StructureValue;
            if (structureValue != null)
            {
                structureValue.Find(name, retVal);
            }
        }

        public IValue Value { get; set; }

        public IValue DefaultValue
        {
            get { return null; }
        }

        public void HandleChange(CacheImpact cacheImpact)
        {
            // Nothing to do
        }

        /// <summary>
        /// Indicates that the model is expanded
        /// </summary>
        public ExpandableEnum Expanded { get; set; }
    }
}