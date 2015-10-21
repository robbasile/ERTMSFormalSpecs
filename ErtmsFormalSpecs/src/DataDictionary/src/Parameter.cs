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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Variables;
using Utils;
using Function = DataDictionary.Functions.Function;
using NameSpace = DataDictionary.Types.NameSpace;
using Procedure = DataDictionary.Functions.Procedure;
using Type = DataDictionary.Types.Type;

namespace DataDictionary
{
    public class Parameter : Generated.Parameter, ITypedElement, IGraphicalDisplay
    {
        /// <summary>
        ///     Parameter namespace
        /// </summary>
        public NameSpace NameSpace
        {
            get { return EnclosingNameSpaceFinder.find(this); }
        }

        /// <summary>
        ///     Parameter type name
        /// </summary>
        public string TypeName
        {
            get { return getTypeName(); }
            set
            {
                _type = null;
                setTypeName(value);
            }
        }

        /// <summary>
        ///     Parameter type
        /// </summary>
        private Type _type;

        public Type Type
        {
            get
            {
                if (_type == null)
                {
                    Expression typeExpression = new Parser().Expression(this, getTypeName(),
                        IsType.INSTANCE, true, null, true);

                    if (typeExpression != null)
                    {
                        _type = typeExpression.Ref as Type;
                    }
                }
                return _type;
            }
            set
            {
                TypeName = value.Name;
                _type = value;
            }
        }

        /// <summary>
        ///     The enclosing collection of the parameter
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;

                if (Enclosing is Function)
                {
                    retVal = EnclosingFinder<Function>.find(this).FormalParameters;
                }
                else if (Enclosing is Procedure)
                {
                    retVal = EnclosingFinder<Procedure>.find(this).FormalParameters;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Creates an actual parameter based on this formal parameter and the value assigned to that parameter
        /// </summary>
        /// <returns></returns>
        public Actual CreateActual()
        {
            return new Actual(this);
        }

        /// <summary>
        ///     Provides the mode of the parameter
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return acceptor.VariableModeEnumType.aInternal; }
            set { }
        }

        /// <summary>
        ///     The default value
        /// </summary>
        public string Default
        {
            get { return Type.Default; }
            set { }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public override void AddModelElement(IModelElement element)
        {
        }

        public override string ToString()
        {
            string retVal = "Parameter " + Name;

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Write(Name);
            explanation.Write(" : ");
            explanation.Write(TypeName);
        }

        /// <summary>
        ///     Provides the name of this model element when accessing it from the other model element (provided as parameter)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override string ReferenceName(ModelElement user)
        {
            string retVal = Name;

            return retVal;
        }


        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static Parameter CreateDefault(ICollection enclosingCollection)
        {
            Parameter retVal = (Parameter) acceptor.getFactory().createParameter();

            Util.DontNotify(() => { retVal.Name = "Parameter" + GetElementNumber(enclosingCollection); });

            return retVal;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string GraphicalName
        {
            get { return Name; }
        }

        public bool Hidden
        {
            get { return false; }
            set { }
        }

        public bool Pinned
        {
            get { return false; }
            set { }
        }
    }
}