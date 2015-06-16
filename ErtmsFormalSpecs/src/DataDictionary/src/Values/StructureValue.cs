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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Variables;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.Values
{
    public class StructureValue : BaseValue<Structure, Dictionary<string, INamable>>, ISubDeclarator
    {
        /// <summary>
        ///     Provides the type as a structure
        /// </summary>
        public Structure Structure
        {
            get { return Type as Structure; }
        }

        private static int depth = 0;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="setDefaultValue">Indicates that default values should be set</param>
        public StructureValue(Structure structure, bool setDefaultValue = true)
            : base(structure, new Dictionary<string, INamable>())
        {
            Enclosing = structure;

            depth += 1;
            if (depth > 100)
            {
                throw new Exception("Possible structure recursion found");
            }
            try
            {
                Util.DontNotify(() =>
                {
                    foreach (StructureElement element in Structure.Elements)
                    {
                        Variable variable = (Variable) acceptor.getFactory().createVariable();
                        variable.Enclosing = this;
                        if (element.Type != null)
                        {
                            variable.Type = element.Type;
                        }
                        variable.Name = element.Name;
                        variable.Mode = element.Mode;
                        variable.Default = element.Default;

                        if (setDefaultValue)
                        {
                            variable.Value = variable.DefaultValue;
                        }
                        else
                        {
                            if (variable.Type is Collection)
                            {
                                variable.Value = new ListValue(variable.Type as Collection, new List<IValue>());
                            }
                            else
                            {
                                variable.Value = new DefaultValue(variable);
                            }
                        }
                        set(variable);
                    }
                });
            }
            finally
            {
                depth -= 1;
                DeclaredElements = null;
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
                Util.DontNotify(() =>
                {
                    HashSet<string> members = new HashSet<string>();
                    foreach (KeyValuePair<Designator, Expression> pair in structureExpression.Associations)
                    {
                        IValue val = pair.Value.GetValue(new InterpretationContext(context), explain);
                        if (val != null)
                        {
                            Variable var = (Variable) acceptor.getFactory().createVariable();
                            var.Name = pair.Key.Image;
                            var.Value = val;
                            var.Enclosing = this;
                            set(var);
                            members.Add(var.Name);
                        }
                        else
                        {
                            structureExpression.AddError("Cannot evaluate value for " + pair.Value);
                        }
                    }

                    foreach (StructureElement element in Structure.Elements)
                    {
                        if (!members.Contains(element.Name))
                        {
                            Variable variable = (Variable) acceptor.getFactory().createVariable();
                            variable.Enclosing = this;
                            if (element.Type != null)
                            {
                                variable.Type = element.Type;
                            }
                            variable.Name = element.Name;
                            variable.Mode = element.Mode;
                            variable.Default = element.Default;
                            variable.Value = variable.DefaultValue;
                            set(variable);
                        }
                    }
                });
            }
            finally
            {
                depth -= 1;
                DeclaredElements = null;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="structure"></param>
        public StructureValue(StructureValue other)
            : base(other.Structure, new Dictionary<string, INamable>())
        {
            Enclosing = other.Structure;

            foreach (KeyValuePair<string, INamable> pair in other.Val)
            {
                Variable variable = pair.Value as Variable;
                if (variable != null)
                {
                    Variable var2 = (Variable) acceptor.getFactory().createVariable();
                    var2.Type = variable.Type;
                    var2.Name = variable.Name;
                    var2.Mode = variable.Mode;
                    var2.Default = variable.Default;
                    var2.Enclosing = this;
                    if (variable.Value != null)
                    {
                        var2.Value = variable.Value.RightSide(var2, true, true);
                    }
                    else
                    {
                        var2.Value = null;
                    }
                    set(var2);
                }
            }

            DeclaredElements = null;
        }

        /// <summary>
        ///     Sets the value of a given association
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        public void set(IVariable variable)
        {
            StructureElement element = Structure.FindStructureElement(variable.Name);

            if (element != null)
            {
                variable.Type = element.Type;
                Val[variable.Name] = variable;
                variable.Enclosing = this;
            }
        }

        /// <summary>
        ///     Gets the value associated to a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IVariable getVariable(string name)
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
            DeclaredElements = new Dictionary<string, List<INamable>>();

            foreach (INamable namable in Val.Values)
            {
                ISubDeclaratorUtils.AppendNamable(this, namable);
            }
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
            ISubDeclaratorUtils.Find(this, name, retVal);
        }

        public override string Name
        {
            get
            {
                string retVal = Type.FullName + "\n{\n";

                bool first = true;
                foreach (object tmp in Val.Values)
                {
                    if (!first)
                    {
                        retVal += ", \n";
                    }
                    Variable variable = tmp as Variable;
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
        private Dictionary<string, IVariable> subVariables;

        public Dictionary<string, IVariable> SubVariables
        {
            get
            {
                if (subVariables == null)
                {
                    subVariables = new Dictionary<string, IVariable>();

                    foreach (KeyValuePair<string, INamable> kp in Val)
                    {
                        IVariable var = kp.Value as IVariable;

                        if (var != null)
                        {
                            subVariables.Add(kp.Key, var);
                        }
                    }
                }

                return subVariables;
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
            foreach (object tmp in Val.Values)
            {
                Variable variable = tmp as Variable;
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