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
using DataDictionary.Functions;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
     * The grammar is following:                                     *
     * Expression0      -> Expression1 Expression0Cont               *
     * Expression0Cont  -> OR Expression1 Expression0Cont            *
     * Expression0Cont  -> Epsilon                                   *
     * Expression1      -> Expression2 Expression1Cont               *
     * Expression1Cont  -> AND Expression2 Expression1Cont           *
     * Expression1Cont  -> Epsilon                                   *
     * Expression2      -> Expression3 Expression2Cont               *
     * Expression2Cont  -> {+, -} Expression3 Expression2Cont        *
     * Expression2Cont  -> Epsilon                                   *
     * Expression3      -> Expression4 Expression3Cont               *
     * Expression3Cont  -> {*, /} Expression4 Expression3Cont        *
     * Expression3Cont  -> Epsilon                                   *
     * Expression4      -> Expression5 Expression4Cont               *
     * Expression4Cont  -> {^} Expression5 Expression4Cont           *
     * Expression4Cont  -> Epsilon                                   *
     * Expression5      -> Term {+, -}                               *
     * Term             -> Literal                                   *
     * Term             -> Desig                                     *
     * Term             -> Desig (arg1, ...)                         *
     * Term             -> (Expression0)                             *
     *                                                               *
     * =>                                                            *
     * Expression_i     -> Expression_i+1 Expression_iCont           *
     * Expression_iCont -> {op_i+1} Expression_i+1 Expression_iCont  *
     * Expression_iCont -> Epsilon                                   *
     * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

    public abstract class Expression : InterpreterTreeNode, IReference
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root for which this expression should be evaluated</param>
        /// <param name="log"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        protected Expression(ModelElement root, ModelElement log, int start, int end)
            : base(root, log, start, end)
        {
        }

        /// <summary>
        ///     Indicates whether the semantic analysis has been performed for this expression
        /// </summary>
        protected bool SemanticAnalysisDone { get; private set; }

        /// <summary>
        ///     Provides the possible references for this expression (only available during semantic analysis)
        /// </summary>
        /// <param name="instance">the instance on which this element should be found.</param>
        /// <param name="expectation">the expectation on the element found</param>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public virtual ReturnValue GetReferences(INamable instance, BaseFilter expectation, bool last)
        {
            return ReturnValue.Empty;
        }

        /// <summary>
        ///     Provides the possible references types for this expression (used in semantic analysis)
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">Indicates the kind of element we are looking for</param>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public virtual ReturnValue GetReferenceTypes(INamable instance, BaseFilter expectation, bool last)
        {
            ReturnValue retVal = new ReturnValue(this);

            SemanticAnalysis(instance, AllMatches.INSTANCE);
            const bool asType = true;
            retVal.Add(GetExpressionType(), null, asType);

            return retVal;
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">Indicates the kind of element we are looking for</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public virtual bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = !SemanticAnalysisDone;

            if (retVal)
            {
                StaticUsage = new Usages();
                SemanticAnalysisDone = true;
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public bool SemanticAnalysis(INamable instance = null)
        {
            return SemanticAnalysis(instance, AllMatches.INSTANCE);
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public bool SemanticAnalysis(BaseFilter expectation)
        {
            return SemanticAnalysis(null, expectation);
        }

        /// <summary>
        ///     Provides the INamable which is referenced by this expression, if any
        /// </summary>
        public virtual INamable Ref
        {
            get { return null; }
            // ReSharper disable once ValueParameterNotUsed
            protected set { }
        }

        /// <summary>
        ///     Provides the ICallable that is statically defined
        /// </summary>
        public virtual ICallable GetStaticCallable()
        {
            ICallable retVal = Ref as ICallable;

            if (retVal == null)
            {
                Type type = Ref as Type;
                if (type != null)
                {
                    retVal = type.CastFunction;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public abstract Type GetExpressionType();

        /// <summary>
        ///     Provides all the steps used to get the value of the expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ExplanationPart Explain(InterpretationContext context = null)
        {
            ExplanationPart retVal = new ExplanationPart(Root, this);

            if (context == null)
            {
                context = new InterpretationContext();
            }
            try
            {
                GetValue(context, retVal);
            }
            catch (Exception)
            {
            }

            return retVal;
        }

        /// <summary>
        ///     Adds an error message to the root element and explains it
        /// </summary>
        /// <param name="message"></param>
        /// <param name="explain"></param>
        public override void AddErrorAndExplain(string message, ExplanationPart explain)
        {
            if (RootLog != null)
            {
                ExplanationPart.CreateSubExplanation(explain, message);
                RootLog.AddErrorAndExplain(message, explain);
            }
        }

        /// <summary>
        ///     Provides the variable referenced by this expression, if any
        /// </summary>
        /// <param name="context">The context on which the variable must be found</param>
        /// <returns></returns>
        public virtual IVariable GetVariable(InterpretationContext context)
        {
            return null;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        protected internal abstract IValue GetValue(InterpretationContext context, ExplanationPart explain);

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public virtual IValue GetExpressionValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = null;

            try
            {
                Util.DontNotify(() =>
                {
                    retVal = GetValue(context, explain);                    
                });
            }
            catch (Exception e)
            {
                ModelElement modelElement = context.Instance as ModelElement;
                if (modelElement != null)
                {
                    modelElement.AddErrorAndExplain(e.Message, explain);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the callable that is called by this expression
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public virtual ICallable GetCalled(InterpretationContext context, ExplanationPart explain)
        {
            return null;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public abstract void Fill(List<INamable> retVal, BaseFilter filter);

        /// <summary>
        ///     Provides the right sides used by this expression
        /// </summary>
        public List<ITypedElement> GetRightSides()
        {
            List<ITypedElement> retVal = new List<ITypedElement>();

            List<INamable> tmp = new List<INamable>();
            Fill(tmp, IsRightSide.INSTANCE);

            foreach (INamable namable in tmp)
            {
                ITypedElement element = namable as ITypedElement;
                if (element != null)
                {
                    retVal.Add(element);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the variables used by this expression
        /// </summary>
        public List<IVariable> GetVariables()
        {
            List<IVariable> retVal = new List<IVariable>();

            List<INamable> tmp = new List<INamable>();
            Fill(tmp, IsVariable.INSTANCE);

            foreach (INamable namable in tmp)
            {
                IVariable variable = namable as IVariable;
                if (variable != null)
                {
                    retVal.Add(variable);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the list of literals found in the expression
        /// </summary>
        public List<IValue> GetLiterals()
        {
            List<IValue> retVal = new List<IValue>();

            List<INamable> tmp = new List<INamable>();
            Fill(tmp, IsValue.INSTANCE);

            foreach (INamable namable in tmp)
            {
                IValue value = namable as IValue;
                if (value != null)
                {
                    retVal.Add(value);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public virtual void CheckExpression()
        {
        }

        /// <summary>
        ///     Creates the graph associated to this expression, when the given parameter ranges over the X axis
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameter">The parameters of *the enclosing function* for which the graph should be created</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public virtual Graph CreateGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            return null;
        }

        /// <summary>
        ///     Provides the surface of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the surface</param>
        /// <param name="xParam">The X axis of this surface</param>
        /// <param name="yParam">The Y axis of this surface</param>
        /// <param name="explain"></param>
        /// <returns>The surface which corresponds to this expression</returns>
        public virtual Surface CreateSurface(InterpretationContext context, Parameter xParam, Parameter yParam,
            ExplanationPart explain)
        {
            return null;
        }
    }
}