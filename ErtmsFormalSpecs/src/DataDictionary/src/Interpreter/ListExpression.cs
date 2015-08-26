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
using DataDictionary.Generated;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Values;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    public class ListExpression : Expression
    {
        /// <summary>
        ///     The values in the list
        /// </summary>
        public List<Expression> ListElements { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        /// <param name="root"></param>
        /// <param name="log"></param>
        public ListExpression(ModelElement root, ModelElement log, List<Expression> elements, int start, int end)
            : base(root, log, start, end)
        {
            SetListElements(elements);
        }

        /// <summary>
        ///     Sets the list elements for the list expression
        /// </summary>
        /// <param name="elements"></param>
        public void SetListElements(List<Expression> elements)
        {
            if (elements != null)
            {
                ListElements = elements;

                foreach (Expression expr in ListElements)
                {
                    SetEnclosed(expr);
                }
            }
        }

        /// <summary>
        ///     The type of the collection
        /// </summary>
        private Collection ExpressionType { get; set; }

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
                Type elementType = null;

                if (ListElements != null)
                {
                    foreach (Expression expr in ListElements)
                    {
                        expr.SemanticAnalysis(instance, expectation);
                        StaticUsage.AddUsages(expr.StaticUsage, null);

                        Type current = expr.GetExpressionType();
                        if (elementType == null)
                        {
                            elementType = current;
                        }
                        else
                        {
                            if (!current.Match(elementType))
                            {
                                AddError("Cannot mix types " + current + " and " + elementType + "in collection");
                            }
                        }
                    }
                }

                if (elementType != null)
                {
                    ExpressionType = (Collection) acceptor.getFactory().createCollection();
                    ExpressionType.Type = elementType;
                    ExpressionType.Name = "ListOf_" + elementType.FullName;
                    ExpressionType.Enclosing = Root.EFSSystem;

                    StaticUsage.AddUsage(elementType, Root, Usage.ModeEnum.Type);
                }
                else
                {
                    ExpressionType = new GenericCollection(EFSSystem.INSTANCE);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Checks the expression
        /// </summary>
        public override void CheckExpression()
        {
            foreach (Expression expr in ListElements)
            {
                expr.CheckExpression();
            }
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            return ExpressionType;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        protected internal override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            ListValue retVal = new ListValue(ExpressionType, new List<IValue>());

            foreach (Expression expr in ListElements)
            {
                IValue val = expr.GetValue(context, explain);
                if (val != null)
                {
                    retVal.Val.Add(val);
                }
                else
                {
                    AddError("Cannot evaluate " + expr);
                }
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
            foreach (Expression expr in ListElements)
            {
                expr.Fill(retVal, filter);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            if (ListElements.Count > 0)
            {
                explanation.Write("[");
                explanation.Indent(2,
                    () => explanation.ExplainList(ListElements, explainSubElements, ", ",
                        element => element.GetExplain(explanation, explainSubElements)));
                explanation.Write("]");
            }
            else
            {
                explanation.Write("[]");
            }
        }
    }
}