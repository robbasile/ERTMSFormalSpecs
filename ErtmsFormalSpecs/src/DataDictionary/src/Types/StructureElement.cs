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
using DataDictionary.Interpreter.Filter;
using DataDictionary.Values;
using Utils;

namespace DataDictionary.Types
{
    public class StructureElement : Generated.StructureElement, ITypedElement, ISubDeclarator, ITextualExplain,
        IDefaultValueElement
    {
        public NameSpace NameSpace
        {
            get { return EnclosingNameSpaceFinder.find(this); }
        }

        /// <summary>
        ///     Provides the mode of the structure element
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return getMode(); }

            set { setMode(value); }
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
        }

        /// <summary>
        ///     Provides all the values that can be stored in this structure
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements
        {
            get
            {
                Dictionary<string, List<INamable>> retVal = new Dictionary<string, List<INamable>>();

                if (Type is Structure)
                {
                    Structure structure = Type as Structure;

                    if (structure.DeclaredElements == null)
                    {
                        structure.InitDeclaredElements();
                    }
                    retVal = structure.DeclaredElements;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            if (Type is Structure)
            {
                Structure structure = Type as Structure;
                structure.Find(name, retVal);
            }
        }

        /// <summary>
        ///     Provides the type name of the structure element
        /// </summary>
        public string TypeName
        {
            get
            {
                return getTypeName();
            }
            set
            {
                Type = null;
                setTypeName(value);

                // Ensure types and typename are synchronized
                __type = Type;
            }
        }

        /// <summary>
        ///     The cached type
        /// </summary>
        private Type __type;

        /// <summary>
        ///     Indicates that the type is being computed (no reentrance)
        /// </summary>
        private bool ComputingType { get; set; }

        /// <summary>
        ///     The type associated to this structure element
        /// </summary>
        public Type Type
        {
            get
            {
                if (__type == null && !ComputingType)
                {
                    ComputingType = true;

                    Expression typeExpression = EFSSystem.Parser.Expression(this, getTypeName(), IsType.INSTANCE, true,
                        null, true);
                    if (typeExpression != null)
                    {
                        __type = typeExpression.Ref as Type;
                    }

                    ComputingType = false;
                }

                return __type;
            }
            set
            {
                __type = value;
                if (value != null)
                {
                    setTypeName(value.getName());
                }
                else
                {
                    setTypeName(null);
                }
            }
        }

        /// <summary>
        ///     The enclosing structure
        /// </summary>
        public Structure Structure
        {
            get { return (Structure) getFather(); }
        }

        /// <summary>
        ///     The enclosing collection
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return Structure.Elements; }
        }

        /// <summary>
        ///     The default value
        /// </summary>
        public string Default
        {
            get { return getDefault(); }
            set { setDefault(value); }
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
                __expression = null;
            }
        }

        /// <summary>
        ///     Provides the expression tree associated to this action's expression
        /// </summary>
        private Expression __expression;

        public Expression Expression
        {
            get
            {
                if (__expression == null)
                {
                    __expression = EFSSystem.Parser.Expression(this, ExpressionText);
                }

                return __expression;
            }
            set { __expression = value; }
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
            bool retVal = false;

            Expression tree = EFSSystem.Parser.Expression(this, expression, null, false, null, true);
            retVal = tree != null;

            return retVal;
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
                    if (Utils.Util.isEmpty(Default))
                    {
                        retVal = Type.DefaultValue;
                    }
                    else
                    {
                        retVal = Type.getValue(Default);

                        if (retVal == null)
                        {
                            if (Expression != null)
                            {
                                retVal = Expression.GetExpressionValue(new InterpretationContext(this), null);
                                if (retVal != null && !Type.Match(retVal.Type))
                                {
                                    AddError("Default value type (" + retVal.Type.Name +
                                             ")does not match variable type (" + Type.Name + ")");
                                    retVal = null;
                                }
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

                return retVal;
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Comment(this);

            string typeName = TypeName;
            if (Type != null)
            {
                typeName = Type.FullName;
            }

            explanation.Write("FIELD ");
            explanation.Write(Name);
            explanation.Write(" : ");
            explanation.WriteLine(typeName);
        }

        /// <summary>
        ///     Handles a change of the model element by invalidating the cache of all element in CacheDependancy
        /// </summary>
        public override void HandleChange()
        {
            base.HandleChange();
            Structure.HandleChange();
        }

        /// <summary>
        ///     Creates a copy of the structure element in the designated dictionary. The namespace structure is copied over.
        ///     The new structure element is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public StructureElement CreateStructureElementUpdate(Dictionary dictionary)
        {
            StructureElement retVal = new StructureElement();
            retVal.Name = Name;
            retVal.TypeName = TypeName;
            retVal.Comment = Comment;
            retVal.setUpdates(Guid);

            String[] names = FullName.Split('.');
            names = names.Take(names.Count() - 1).ToArray();
            String[] nameSpaceRef = names.Take(names.Count() - 1).ToArray();

            NameSpace nameSpace = dictionary.GetNameSpaceUpdate(nameSpaceRef, Dictionary);
            Structure structure = nameSpace.GetStructureUpdate(names.Last(), (NameSpace) nameSpace.Updates);
            structure.appendElements(retVal);

            return retVal;
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static StructureElement CreateDefault(ICollection enclosingCollection)
        {
            StructureElement retVal = (StructureElement) acceptor.getFactory().createStructureElement();

            Util.DontNotify(() =>
            {
                retVal.Name = "Element" + GetElementNumber(enclosingCollection);
                retVal.Mode = acceptor.VariableModeEnumType.aInternal;
            });

            return retVal;
        }
    }
}