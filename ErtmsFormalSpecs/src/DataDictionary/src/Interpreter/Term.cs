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
using DataDictionary.Variables;
using Utils;

namespace DataDictionary.Interpreter
{
    public class Term : InterpreterTreeNode, IReference, ITextualExplain
    {
        /// <summary>
        ///     The designator of this term
        /// </summary>
        public Designator Designator { get; private set; }

        /// <summary>
        ///     The literal value of this designator
        /// </summary>
        public Expression LiteralValue { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this model is built</param>
        /// <param name="designator"></parparam>
        ///     <param name="start">The start character for this expression in the original string</param>
        ///     <param name="end">The end character for this expression in the original string</param>
        public Term(ModelElement root, ModelElement log, Designator designator, int start, int end)
            : base(root, log, start, end)
        {
            Designator = designator;
            Designator.Enclosing = this;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this model is built</param>
        /// <param name="literal"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public Term(ModelElement root, ModelElement log, Expression literal, int start, int end)
            : base(root, log, start, end)
        {
            LiteralValue = literal;
        }

        /// <summary>
        ///     Provides the possible references for this term (only available during semantic analysis)
        /// </summary>
        /// <param name="instance">the instance on which this element should be found.</param>
        /// <param name="expectation">the expectation on the element found</param>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public ReturnValue getReferences(INamable instance, BaseFilter expectation, bool last)
        {
            ReturnValue retVal = null;

            if (Designator != null)
            {
                retVal = Designator.getReferences(instance, expectation, last);
            }
            else if (LiteralValue != null)
            {
                retVal = LiteralValue.getReferences(instance, expectation, last);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the possible references types for this expression (used in semantic analysis)
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public ReturnValue getReferenceTypes(INamable instance, BaseFilter expectation, bool last)
        {
            ReturnValue retVal = null;

            if (Designator != null)
            {
                retVal = new ReturnValue();

                foreach (ReturnValueElement element in Designator.getReferences(instance, expectation, last).Values)
                {
                    if (element.Value is Type)
                    {
                        bool asType = true;
                        retVal.Add(element.Value, null, asType);
                    }
                }
            }
            else if (LiteralValue != null)
            {
                retVal = LiteralValue.getReferenceTypes(instance, expectation, true);
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the semantic analysis of the term
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">
        ///     Indicates the kind of element we are looking for</paraparam>
        ///     <param name="lastElement">Indicates that this element is the last one in a dereference chain</param>
        ///     <returns>True if semantic analysis should be continued</returns>
        public void SemanticAnalysis(INamable instance, BaseFilter expectation, bool lastElement)
        {
            if (Designator != null)
            {
                Designator.SemanticAnalysis(instance, expectation, lastElement);
                StaticUsage = Designator.StaticUsage;
            }
            else if (LiteralValue != null)
            {
                LiteralValue.SemanticAnalysis(instance, expectation);
                StaticUsage = LiteralValue.StaticUsage;
            }
        }

        /// <summary>
        ///     The model element referenced by this term.
        /// </summary>
        public INamable Ref
        {
            get
            {
                INamable retVal = null;

                if (Designator != null)
                {
                    retVal = Designator.Ref;
                }
                else if (LiteralValue != null)
                {
                    retVal = LiteralValue.Ref;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public Type GetExpressionType()
        {
            Type retVal = null;

            if (Designator != null)
            {
                retVal = Designator.GetDesignatorType();
            }
            else if (LiteralValue != null)
            {
                retVal = LiteralValue.GetExpressionType();
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the variable referenced by this expression, if any
        /// </summary>
        /// <param name="context">The context on which the variable must be found</param>
        /// <returns></returns>
        public IVariable GetVariable(InterpretationContext context)
        {
            IVariable retVal = null;

            if (Designator != null)
            {
                retVal = Designator.GetVariable(context);
            }
            else if (LiteralValue != null)
            {
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = null;

            if (Designator != null)
            {
                retVal = Designator.GetValue(context);
            }
            else if (LiteralValue != null)
            {
                retVal = LiteralValue.GetValue(context, explain);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the element called by this term, if any
        /// </summary>
        /// <param name="context">The context on which the variable must be found</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public ICallable getCalled(InterpretationContext context, ExplanationPart explain)
        {
            ICallable retVal = null;

            if (Designator != null)
            {
                retVal = Designator.getCalled(context);
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public void fill(List<INamable> retVal, BaseFilter filter)
        {
            if (Designator != null)
            {
                Designator.fill(retVal, filter);
            }
            else if (LiteralValue != null)
            {
                LiteralValue.fill(retVal, filter);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            if (Designator != null)
            {
                Designator.GetExplain(explanation);
            }
            else if (LiteralValue != null)
            {
                LiteralValue.GetExplain(explanation);
            }
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public void checkExpression()
        {
            if (Designator != null)
            {
                Designator.checkExpression();
            }
            else if (LiteralValue != null)
            {
                LiteralValue.checkExpression();
            }
        }
    }
}