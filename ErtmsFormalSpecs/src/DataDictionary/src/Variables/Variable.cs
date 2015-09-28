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
using DataDictionary.Types;
using DataDictionary.Values;
using Utils;
using Collection = DataDictionary.Types.Collection;
using NameSpace = DataDictionary.Types.NameSpace;
using Structure = DataDictionary.Types.Structure;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Variables
{
    public class Variable : Generated.Variable, IVariable, IDefaultValueElement,
        IGraphicalDisplay
    {
        /// <summary>
        ///     Indicates that the DeclaredElement dictionary is currently being built
        /// </summary>
        private bool _buildingDeclaredElements;

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
        }

        /// <summary>
        ///     The elements declared by this variable
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements
        {
            get
            {
                Dictionary<string, List<INamable>> retVal = new Dictionary<string, List<INamable>>();

                if (!_buildingDeclaredElements)
                {
                    try
                    {
                        _buildingDeclaredElements = true;

                        StructureValue structureValue = Value as StructureValue;
                        if (structureValue != null)
                        {
                            if (structureValue.DeclaredElements == null)
                            {
                                structureValue.InitDeclaredElements();
                            }
                            retVal = structureValue.DeclaredElements;
                        }
                    }
                    finally
                    {
                        _buildingDeclaredElements = false;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates if this Procedure contains implemented sub-elements
        /// </summary>
        public override bool ImplementationPartiallyCompleted
        {
            get
            {
                if (getImplemented())
                {
                    return true;
                }

                foreach (Variable subVariable in allSubVariables())
                {
                    if (subVariable.ImplementationPartiallyCompleted)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            if (!_buildingDeclaredElements)
            {
                try
                {
                    _buildingDeclaredElements = true;

                    StructureValue structureValue = Value as StructureValue;
                    if (structureValue != null)
                    {
                        structureValue.Find(name, retVal);
                    }

                    // Dereference of an empty value holds the empty value
                    EmptyValue emptyValue = Value as EmptyValue;
                    if (emptyValue != null)
                    {
                        retVal.Add(emptyValue);
                    }
                }
                finally
                {
                    _buildingDeclaredElements = false;
                }
            }
        }

        /// <summary>
        ///     The enclosing name space
        /// </summary>
        public NameSpace NameSpace
        {
            get { return EnclosingNameSpaceFinder.find(this); }
        }

        /// <summary>
        ///     Provides the mode of the variable
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return getVariableMode(); }
            set { setVariableMode(value); }
        }

        /// <summary>
        ///     The default value
        /// </summary>
        public string Default
        {
            get
            {
                string retVal = getDefaultValue();

                if (string.IsNullOrEmpty(retVal) && Type != null)
                {
                    retVal = Type.getDefault();
                }

                if (string.IsNullOrEmpty(retVal))
                {
                    Structure structure = Type as Structure;
                    if (structure != null)
                    {
                        retVal = structure.FullName + "{}";
                    }
                    else
                    {
                        Collection collection = Type as Collection;
                        if (collection != null)
                        {
                            retVal = "[]";
                        }
                        else
                        {
                            retVal = "0";
                        }
                    }
                }

                return retVal;
            }

            set { setDefaultValue(value); }
        }

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
            set
            {
                Default = value;
                _expression = null;
            }
        }

        /// <summary>
        ///     Provides the expression tree associated to this action's expression
        /// </summary>
        private Expression _expression;

        public Expression Expression
        {
            get
            {
                if (_expression == null)
                {
                    _expression = EFSSystem.Parser.Expression(this, ExpressionText);
                }

                return _expression;
            }
            set { _expression = value; }
        }

        public InterpreterTreeNode Tree
        {
            get { return Expression; }
        }

        /// <summary>
        ///     Clears the expression tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
            Expression = null;
        }

        /// <summary>
        ///     Creates the tree according to the expression text
        /// </summary>
        public InterpreterTreeNode Compile()
        {
            // Side effect, builds the statement if it is not already built
            return Tree;
        }

        /// <summary>
        ///     Indicates that the expression is valid for this IExpressionable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool checkValidExpression(string expression)
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            bool retVal;

            Expression tree = EFSSystem.Parser.Expression(this, expression, null, false, null, true);
            retVal = tree != null;

            return retVal;
        }

        public override ArrayList EnclosingCollection
        {
            get { return NameSpace.Variables; }
        }

        /// <summary>
        ///     Provides the type name of the variable
        /// </summary>
        public string TypeName
        {
            get { return getTypeName(); }
            set
            {
                setTypeName(value);
                _value = null;
            }
        }

        /// <summary>
        ///     The type associated to this variable
        /// </summary>
        public Type Type
        {
            get
            {
                return EFSSystem.FindType(NameSpace, getTypeName());
            }
            set
            {
                if (value != null)
                {
                    setTypeName(value.FullName);
                }
                else
                {
                    setTypeName(null);
                }
            }
        }

        /// <summary>
        ///     The enclosed variables
        /// </summary>
        public Dictionary<string, IVariable> SubVariables
        {
            get
            {
                Dictionary<string, IVariable> retVal = new Dictionary<string, IVariable>();

                StructureValue structureValue = Value as StructureValue;
                if (structureValue != null)
                {
                    retVal = structureValue.SubVariables;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the variable's default value
        /// </summary>
        public IValue DefaultValue
        {
            get
            {
                IValue retVal = null;

                if (Type != null)
                {
                    if (Utils.Util.isEmpty(getDefaultValue()))
                    {
                        // The variable does not define a default value, get the one from the type
                        retVal = Type.DefaultValue;
                    }
                    else
                    {
                        // The variable defines a default value, try to interpret it
                        if (Expression != null)
                        {
                            retVal = Expression.GetExpressionValue(new InterpretationContext(Type), null);
                            if (retVal != null && !Type.Match(retVal.Type))
                            {
                                AddError("Default value type (" + retVal.Type.Name + ")does not match variable type (" +
                                         Type.Name + ")");
                                retVal = null;
                            }
                        }
                    }
                }
                else
                {
                    AddError("Cannot find type of variable (" + getTypeName() + ")");
                }

                if (retVal == null)
                {
                    AddError("Cannot create default value");
                }
                else
                {
                    retVal = retVal.RightSide(this, false, true);
                }

                return retVal;
            }
        }

        public override void AddElementLog(ElementLog log)
        {
            if (Enclosing is NameSpace)
            {
                base.AddElementLog(log);
            }
            else
            {
                IEnclosed current = Enclosing as IEnclosed;
                while (current != null)
                {
                    ModelElement element = current as ModelElement;
                    if (element != null)
                    {
                        element.AddElementLog(log);
                        current = null;
                    }
                    else
                    {
                        current = current.Enclosing as IEnclosed;
                    }
                }
            }
        }

        /// <summary>
        ///     The variable's value
        /// </summary>
        private IValue _value;

        public IValue Value
        {
            get
            {
                if (_value == null)
                {
                    _value = DefaultValue;
                }
                return _value;
            }
            set { _value = value; }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            if (string.IsNullOrEmpty(Comment))
            {
                if (Type != null)
                {
                    explanation.Comment(Type);
                }
            }
            else
            {
                explanation.Comment(this);
            }

            explanation.Write(Name);
            explanation.Write(" : ");
            explanation.Write(TypeName);
            if (Value != null)
            {
                explanation.Write(" = ");
                explanation.Write(Value.LiteralName);
            }
            explanation.WriteLine();
        }

        public override string ToString()
        {
            string retVal = "Variable " + Name + " : " + TypeName;
            if (Value != null)
            {
                retVal += " = " + Value;
            }
            else
            {
                retVal += " is null";
            }

            return retVal;
        }

        /// <summary>
        ///     The X position
        /// </summary>
        public int X
        {
            get { return getX(); }
            set { setX(value); }
        }

        /// <summary>
        ///     The Y position
        /// </summary>
        public int Y
        {
            get { return getY(); }
            set { setY(value); }
        }

        /// <summary>
        ///     The width
        /// </summary>
        public int Width
        {
            get { return getWidth(); }
            set { setWidth(value); }
        }

        /// <summary>
        ///     The height
        /// </summary>
        public int Height
        {
            get { return getHeight(); }
            set { setHeight(value); }
        }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public string GraphicalName
        {
            get { return Name; }
        }

        /// <summary>
        ///     Indicates whether the namespace is hidden
        /// </summary>
        public bool Hidden
        {
            get { return getHidden(); }
            set { setHidden(value); }
        }

        /// <summary>
        ///     Indicates that the element is pinned
        /// </summary>
        public bool Pinned
        {
            get { return getPinned(); }
            set { setPinned(value); }
        }

        public override void HandleChange()
        {
            base.HandleChange();

            Structure structure = Type as Structure;
            if (structure != null)
            {
                structure.HandleChange();
            }

            StructureValue enclosingStructureValue = Enclosing as StructureValue;
            if (enclosingStructureValue != null)
            {
                IVariable enclosingVariable = enclosingStructureValue.Enclosing as IVariable;
                if (enclosingVariable != null)
                {
                    enclosingVariable.HandleChange();
                }
            }
        }

        /// <summary>
        ///     Creates a copy of the variable in the designated dictionary. The namespace structure is copied over.
        ///     The new variable is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public Variable CreateVariableUpdate(Dictionary dictionary)
        {
            Variable retVal = (Variable) Duplicate();
            retVal.setUpdates(Guid);
            retVal.ClearAllRequirements();

            String[] names = FullName.Split('.');
            names = names.Take(names.Count() - 1).ToArray();
            NameSpace nameSpace = dictionary.GetNameSpaceUpdate(names, Dictionary);
            nameSpace.appendVariables(retVal);

            return retVal;
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string result = base.CreateStatusMessage();

            result += "Variable " + Name;

            return result;
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static Variable CreateDefault(ICollection enclosingCollection)
        {
            Variable retVal = (Variable) acceptor.getFactory().createVariable();

            Util.DontNotify(() =>
            {
                retVal.Name = "Variable" + GetElementNumber(enclosingCollection);
                retVal.Type = EfsSystem.Instance.BoolType;
            });

            return retVal;
        }
    }
}