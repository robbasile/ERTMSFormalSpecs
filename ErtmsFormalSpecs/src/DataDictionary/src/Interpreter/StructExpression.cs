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

using System.Collections.Generic;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Values;
using Utils;

namespace DataDictionary.Interpreter
{
    public class StructExpression : Expression
    {
        /// <summary>
        ///     The structure instanciated by this structure expression
        /// </summary>
        public Expression Structure { get; private set; }

        /// <summary>
        ///     The associations name is equivalent to Expression that is used to initialize this structure
        /// </summary>
        public Dictionary<Designator, Expression> Associations { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="log"></param>
        /// <param name="structure"></param>
        /// <param name="associations"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public StructExpression(ModelElement root, ModelElement log, Expression structure,
            Dictionary<Designator, Expression> associations, int start, int end)
            : base(root, log, start, end)
        {
            Structure = structure;
            SetAssociation(associations);
        }

        /// <summary>
        ///     Setup the variable => value association
        /// </summary>
        /// <param name="associations"></param>
        public void SetAssociation(Dictionary<Designator, Expression> associations)
        {
            if (associations != null)
            {
                Associations = associations;
                foreach (Expression expr in Associations.Values)
                {
                    SetEnclosed(expr);
                }
            }
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">Indicates the kind of element we are looking for</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                // Structure
                if (Structure != null)
                {
                    Structure.SemanticAnalysis(instance, IsStructure.INSTANCE);
                    StaticUsage.AddUsages(Structure.StaticUsage, Usage.ModeEnum.Type);

                    // Structure field Association
                    if (Associations != null)
                    {
                        Structure structureType = Structure.Ref as Structure;
                        foreach (KeyValuePair<Designator, Expression> pair in Associations)
                        {
                            if (structureType != null)
                            {
                                pair.Key.Ref = structureType.FindStructureElement(pair.Key.Image);
                                StaticUsage.AddUsage(pair.Key.Ref, Root, Usage.ModeEnum.Parameter);
                            }

                            pair.Value.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                            StaticUsage.AddUsages(pair.Value.StaticUsage, Usage.ModeEnum.Read);
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            return Structure.GetExpressionType();
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        protected internal override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            StructureValue retVal = null;

            Structure structureType = Structure.GetExpressionType() as Structure;
            if (structureType != null)
            {
                retVal = new StructureValue(this, context, explain);
            }
            else
            {
                AddError("Cannot determine structure type for " + ToString());
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void Fill(List<INamable> retVal, BaseFilter filter)
        {
            foreach (Expression expression in Associations.Values)
            {
                expression.Fill(retVal, filter);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            Structure.GetExplain(explanation);
            explanation.Write("{");
            explanation.Indent(2, () => explanation.ExplainList(Associations, explainSubElements, ", ", element =>
            {
                explanation.WriteLine();
                element.Key.GetExplain(explanation);
                explanation.Write(" => ");
                element.Value.GetExplain(explanation);
            }));
            explanation.WriteLine();
            explanation.Write("}");
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void CheckExpression()
        {
            Structure structureType = Structure.GetExpressionType() as Structure;
            if (structureType != null)
            {
                if (structureType.IsAbstract)
                {
                    AddError("Instantiation of abstract types is forbidden");
                }
                foreach (KeyValuePair<Designator, Expression> pair in Associations)
                {
                    Designator name = pair.Key;
                    Expression expression = pair.Value;

                    List<INamable> targets = new List<INamable>();
                    structureType.Find(name.Image, targets);
                    if (targets.Count > 0)
                    {
                        expression.CheckExpression();
                        Type type = expression.GetExpressionType();
                        if (type != null)
                        {
                            if (type.IsAbstract)
                            {
                                AddError("Instantiation of abstract types is forbidden");
                            }
                            else
                            {
                                foreach (INamable namable in targets)
                                {
                                    ITypedElement element = namable as ITypedElement;
                                    if (element != null && element.Type != null)
                                    {
                                        if (!element.Type.Match(type))
                                        {
                                            AddError("Expression " + expression + " type (" + type.FullName +
                                                     ") does not match the target element " + element.Name + " type (" +
                                                     element.Type.FullName + ")");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            AddError("Expression " + expression + " type cannot be found");
                        }
                    }
                    else
                    {
                        Root.AddError("Cannot find " + name + " in structure " + Structure);
                    }
                }
            }
            else
            {
                AddError("Cannot find structure type " + Structure);
            }
        }
    }
}