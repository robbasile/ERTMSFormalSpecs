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
using System.Collections.Generic;
using System.Text;
using DataDictionary.Interpreter;
using DataDictionary.Variables;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;

namespace DataDictionary.Values
{
    public class StructureValue : BaseValue<Structure, Dictionary<string, INamable>>, ISubDeclarator
    {
        /// <summary>
        ///     Provides the type as a structure
        /// </summary>
        public Structure Structure
        {
            get
            {
                Structure retVal = (Structure) base.Type;

                retVal = retVal.UnifiedStructure;

                return retVal;
            }
        }

        /// <summary>
        /// Gets the unified structure as type
        /// </summary>
        public override Types.Type Type
        {
            get { return Structure; }
            set { base.Type = value; }
        }

        /// <summary>
        /// Avoid infinite loop while building structure values (in case of recursive definition)
        /// </summary>
        private static int _depth;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="setDefaultValue">Indicates that default values should be set</param>
        public StructureValue(Structure structure, bool setDefaultValue = true)
            : base(structure, new Dictionary<string, INamable>())
        {
            Enclosing = structure;

            _depth += 1;
            if (_depth > 100)
            {
                throw new Exception("Possible structure recursion found");
            }
            try
            {
                foreach (StructureElement element in Structure.Elements)
                {
                    Field field = CreateField(element, structure);

                    if (setDefaultValue)
                    {
                        field.Value = field.DefaultValue;
                    }
                    else
                    {
                        if (field.Type is Collection)
                        {
                            field.Value = new ListValue(field.Type as Collection, new List<IValue>());
                        }
                        else
                        {
                            field.Value = new DefaultValue(field);
                        }
                    }
                }
            }
            finally
            {
                _depth -= 1;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="structureExpression"></param>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        public StructureValue(StructExpression structureExpression, InterpretationContext context,
            ExplanationPart explain)
            : base(structureExpression.GetExpressionType() as Structure, new Dictionary<string, INamable>())
        {
            Enclosing = Structure;

            try
            {
                HashSet<string> members = new HashSet<string>();
                foreach (KeyValuePair<Designator, Expression> pair in structureExpression.Associations)
                {
                    StructureElement structureElement = Structure.FindStructureElement(pair.Key.Image);
                    if (structureElement != null)
                    {
                        IValue val = pair.Value.GetExpressionValue(new InterpretationContext(context), explain);
                        if (val != null)
                        {
                            Field field = CreateField(structureElement, structureExpression.RootLog);
                            field.Value = val;
                            members.Add(field.Name);
                        }
                        else
                        {
                            structureExpression.AddError("Cannot evaluate value for " + pair.Value);
                        }
                    }
                    else
                    {
                        structureExpression.AddError("Cannot find structure element " + pair.Key.Image);
                    }
                }

                foreach (StructureElement element in Structure.Elements)
                {
                    if (!members.Contains(element.Name))
                    {
                        Field field = CreateField(element, structureExpression.RootLog);
                        field.Value = element.DefaultValue;
                    }
                }
            }
            finally
            {
                _depth -= 1;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="other"></param>
        public StructureValue(StructureValue other)
            : base(other.Structure, new Dictionary<string, INamable>())
        {
            Enclosing = other.Structure;

            foreach (KeyValuePair<string, INamable> pair in other.Val)
            {
                Field field = pair.Value as Field;
                if (field != null)
                {
                    Field field2 = CreateField(field.StructureElement, Structure); 
                    if (field.Value != null)
                    {
                        field2.Value = field.Value.RightSide(field2, true, true);
                    }
                    else
                    {
                        field2.Value = null;
                    }
                }
            }
        }

        /// <summary>
        ///     Sets the value of a given association
        /// </summary>
        /// <param name="structureElement"></param>
        /// <param name="log">The element on which errors should be logged</param>
        /// <returns>the newly created field</returns>
        public Field CreateField(StructureElement structureElement, ModelElement log)
        {
            Field retVal = null;

            if (structureElement != null)
            {
                retVal = new Field(this, structureElement);
                Val[retVal.Name] = retVal;
            }
            else
            {
                log.AddError("Cannot find structure element");
            }

            return retVal;
        }

        /// <summary>
        ///     Sets the value of a given association
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <param name="log">The element on which errors should be logged</param>
        /// <returns>the newly created field</returns>
        public Field CreateField(object enclosing, string name, ModelElement log)
        {
            Field retVal = null;

            StructureElement structureElement = Structure.FindStructureElement(name);
            if (structureElement != null)
            {
                retVal = CreateField(structureElement, log);
            }
            else
            {
                log.AddError("Cannot find structure element " + name);
            }

            return retVal;
        }

        /// <summary>
        ///     Gets the value associated to a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IVariable GetVariable(string name)
        {
            IVariable retVal = null;

            if (Val.ContainsKey(name))
            {
                retVal = Val[name] as IVariable;
            }

            return retVal;
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
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
            INamable namable;

            if (Val.TryGetValue(name, out namable))
            {
                retVal.Add(namable);                
            }
        }

        public override string Name
        {
            get
            {
                string retVal = Type.FullName + "\n{\n";

                bool first = true;
                foreach (INamable tmp in Val.Values)
                {
                    if (!first)
                    {
                        retVal += ", \n";
                    }
                    IVariable variable = tmp as IVariable;
                    if (variable != null && variable.Value != null)
                    {
                        retVal += "    " + variable.Name + " => " + variable.Value.FullName;
                    }

                    first = false;
                }
                retVal += "\n}";

                return retVal;
            }
            set { }
        }

        /// <summary>
        ///     The sub variables of this structure
        /// </summary>
        private Dictionary<string, IVariable> _subVariables;

        public Dictionary<string, IVariable> SubVariables
        {
            get
            {
                if (_subVariables == null)
                {
                    _subVariables = new Dictionary<string, IVariable>();

                    foreach (KeyValuePair<string, INamable> kp in Val)
                    {
                        IVariable var = kp.Value as IVariable;

                        if (var != null)
                        {
                            _subVariables.Add(kp.Key, var);
                        }
                    }
                }

                return _subVariables;
            }
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
            StructureValue retVal = this;

            if (duplicate)
            {
                retVal = new StructureValue(retVal);
            }

            if (setEnclosing)
            {
                retVal.Enclosing = variable;
            }

            return retVal;
        }

        /// <summary>
        ///     Converts a structure value to its corresponding structure expression.
        ///     null entries correspond to the default value
        /// </summary>
        /// <returns></returns>
        public override string ToExpressionWithDefault()
        {
            StringBuilder retVal = new StringBuilder();

            retVal.Append(Type.FullName + "{");
            bool first = true;
            foreach (INamable tmp in Val.Values)
            {
                IVariable variable = tmp as IVariable;
                if (variable != null && !(variable.Value is DefaultValue))
                {
                    if (!first)
                    {
                        retVal.Append(",");
                    }
                    retVal.Append("\n  " + variable.Name + " => " + variable.Value.ToExpressionWithDefault());
                    first = false;
                }
            }
            retVal.Append("\n}");

            return retVal.ToString();
        }
    }
}