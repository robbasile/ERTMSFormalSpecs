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
    public class DerefExpression : Expression
    {
        /// <summary>
        ///     Desig elements of this designator
        /// </summary>
        public List<Expression> Arguments { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="log"></param>
        /// <param name="arguments"></param>
        /// <param name="parsingData">Additional information about the parsing process</param>
        public DerefExpression(ModelElement root, ModelElement log, List<Expression> arguments, ParsingData parsingData)
            : base(root, log, parsingData)
        {
            if (arguments != null)
            {
                Arguments = arguments;

                foreach (Expression expr in Arguments)
                {
                    SetEnclosed(expr);
                }
            }
        }

        /// <summary>
        ///     Provides the ICallable referenced by this
        /// </summary>
        public ICallable Called
        {
            get
            {
                ICallable retVal = Ref as ICallable;

                if (retVal == null)
                {
                    Type type = GetExpressionType();
                    if (type != null)
                    {
                        retVal = type.CastFunction;
                    }
                }

                return retVal;
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
                Ref = null;

                ReturnValue tmp = GetReferences(instance, expectation, false);

                if (tmp.IsUnique)
                {
                    // Unique element has been found. Reference it and perform the semantic analysis 
                    // on all dereferenced expression, now that the context is known for each expression
                    Ref = tmp.Values[0].Value;
                    StaticUsage.AddUsage(Ref, Root, null);

                    References referenceFilter;
                    ReturnValueElement current = tmp.Values[0];
                    for (int i = Arguments.Count - 1; i > 0; i--)
                    {
                        referenceFilter = new References(current.Value);
                        current = current.PreviousElement;
                        Arguments[i].SemanticAnalysis(current.Value, referenceFilter);
                        StaticUsage.AddUsages(Arguments[i].StaticUsage, null);
                        StaticUsage.AddUsage(Arguments[i].Ref, Root, null);
                    }
                    referenceFilter = new References(current.Value);
                    Arguments[0].SemanticAnalysis(null, referenceFilter);
                    StaticUsage.AddUsages(Arguments[0].StaticUsage, null);
                    StaticUsage.AddUsage(Arguments[0].Ref, Root, null);
                }
                else if (tmp.IsAmbiguous)
                {
                    // Several possible interpretations for this deref expression, not allowed
                    AddError("Expression " + ToString() + " may have several interpretations " + tmp +
                             ", please disambiguate");
                }
                else
                {
                    // No possible interpretation for this deref expression, not allowed
                    AddError("Expression " + ToString() + " has no interpretation");
                }
            }

            return retVal;
        }


        /// <summary>
        ///     Provides the possible references for this expression (only available during semantic analysis)
        /// </summary>
        /// <param name="instance">the instance on which this element should be found.</param>
        /// <param name="expectation">the expectation on the element found</param>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public override ReturnValue GetReferences(INamable instance, BaseFilter expectation, bool last)
        {
            ReturnValue retVal = Arguments[0].GetReferences(instance, AllMatches.INSTANCE, false);

            if (retVal.IsEmpty)
            {
                retVal = Arguments[0].GetReferenceTypes(instance, AllMatches.INSTANCE, false);
            }

            // When variables & parameters are found, only consider the first one
            // which is the one that is closer in the tree
            {
                ReturnValue tmp2 = retVal;
                retVal = new ReturnValue();

                ReturnValueElement variable = null;
                foreach (ReturnValueElement elem in tmp2.Values)
                {
                    if (elem.Value is Parameter || elem.Value is IVariable)
                    {
                        if (variable == null)
                        {
                            variable = elem;
                            retVal.Values.Add(elem);
                        }
                    }
                    else
                    {
                        retVal.Values.Add(elem);
                    }
                }
            }

            if (retVal.IsUnique)
            {
                Arguments[0].Ref = retVal.Values[0].Value;
            }

            if (!retVal.IsEmpty)
            {
                for (int i = 1; i < Arguments.Count; i++)
                {
                    ReturnValue tmp2 = retVal;
                    retVal = new ReturnValue(Arguments[i]);

                    foreach (ReturnValueElement elem in tmp2.Values)
                    {
                        bool removed = false;
                        ModelElement model = elem.Value as ModelElement;
                        if (model != null)
                        {
                            removed = model.IsRemoved;
                        }

                        if (!removed)
                        {
                            retVal.Merge(elem,
                                Arguments[i].GetReferences(elem.Value, AllMatches.INSTANCE, i == (Arguments.Count - 1)));
                        }
                    }

                    if (retVal.IsEmpty)
                    {
                        AddError("Cannot find " + Arguments[i] + " in " + Arguments[i - 1]);
                    }
                }
            }
            else
            {
                AddError("Cannot evaluate " + Arguments[0]);
            }

            retVal.Filter(expectation);

            return retVal;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            Type retVal = Ref as Type;

            if (retVal == null)
            {
                ITypedElement typedElement = Ref as ITypedElement;
                if (typedElement != null)
                {
                    retVal = typedElement.Type;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the variable referenced by this expression, if any
        /// </summary>
        /// <param name="context">The context on which the variable must be found</param>
        /// <returns></returns>
        public override IVariable GetVariable(InterpretationContext context)
        {
            INamable current = null;

            InterpretationContext ctxt = new InterpretationContext(context);
            foreach (Expression expression in Arguments)
            {
                if (current != null)
                {
                    // Current can be null on several loop iterations when the referenced element
                    // does not references a variable (for instance, when it references a namespace)
                    ctxt.Instance = current;
                }
                current = expression.GetVariable(ctxt);
                if (current == null)
                {
                    current = expression.GetValue(ctxt, null);
                }
            }

            return current as IVariable;
        }

        /// <summary>
        /// Indicates whether this expression references a valid expression component
        /// (a variable or an enum)
        /// </summary>
        /// <returns></returns>
        public bool IsValidExpressionComponent()
        {
            bool retVal = true;
            int count = Arguments.Count;
            for (int i = count - 1; i > 0; i--)
            {
                INamable aNamable = Arguments[i].Ref;
                if (aNamable is Variable ||
                    aNamable is Function ||
                    aNamable is Types.Enum ||
                    aNamable is Constants.EnumValue ||
                    aNamable is Constants.State)
                {
                    break;
                }
                if ((aNamable is NameSpace) ||
                    (aNamable is Structure))
                {
                    retVal = false;
                }
            }
            return retVal;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        protected internal override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            INamable retVal = Ref as IValue;

            if (retVal == null)
            {
                IVariable variable = Ref as IVariable;
                if (variable != null)
                {
                    retVal = variable.Value;
                }
            }

            if (retVal == null)
            {
                InterpretationContext ctxt = new InterpretationContext(context);
                foreach (Expression expression in Arguments)
                {
                    if (retVal != null)
                    {
                        ctxt.Instance = retVal;
                    }
                    retVal = expression.GetValue(ctxt, explain);

                    if (retVal == EfsSystem.Instance.EmptyValue)
                    {
                        break;
                    }
                }
            }

            if (retVal == null)
            {
                AddError(ToString() + " does not refer to a value");
            }

            return retVal as IValue;
        }

        /// <summary>
        ///     Provides the value of the prefix of the expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="elementCount">The number of elements to consider</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public INamable GetPrefixValue(InterpretationContext context, int elementCount, ExplanationPart explain)
        {
            INamable retVal = null;

            InterpretationContext ctxt = new InterpretationContext(context);
            for (int i = 0; i < elementCount; i++)
            {
                if (retVal != null)
                {
                    ctxt.Instance = retVal;
                }
                retVal = Arguments[i].GetValue(ctxt, explain);
                if (retVal == null)
                {
                    retVal = Arguments[i].Ref;
                }

                if (retVal == EfsSystem.Instance.EmptyValue)
                {
                    break;
                }
            }

            if (retVal == null)
            {
                AddError(ToString() + " prefix does not refer to a value");
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the callable that is called by this expression
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override ICallable GetCalled(InterpretationContext context, ExplanationPart explain)
        {
            ICallable retVal = Called;

            if (retVal == null)
            {
                AddError("Cannot evaluate call to " + ToString());
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
            if (filter.AcceptableChoice(Ref))
            {
                retVal.Add(Ref);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            explanation.ExplainList(Arguments, true, ".", expression => expression.GetExplain(explanation));
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void CheckExpression()
        {
            foreach (Expression subExpression in Arguments)
            {
                subExpression.CheckExpression();
            }

            base.CheckExpression();
        }

        /// <summary>
        ///     Creates the graph associated to this expression, when the given parameter ranges over the X axis
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameter">The parameters of *the enclosing function* for which the graph should be created</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph CreateGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = Graph.createGraph(GetValue(context, explain), parameter, explain);

            if (retVal == null)
            {
                throw new Exception("Cannot create graph for " + ToString());
            }

            return retVal;
        }


        /// <summary>
        ///     Provides the surface of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the surface</param>
        /// <param name="xParam">The X axis of this surface</param>
        /// <param name="yParam">The Y axis of this surface</param>
        /// <param name="explain"></param>
        /// <returns>The surface which corresponds to this expression</returns>
        public override Surface CreateSurface(InterpretationContext context, Parameter xParam, Parameter yParam,
            ExplanationPart explain)
        {
            Surface retVal = Surface.createSurface(GetValue(context, explain), xParam, yParam);

            if (retVal == null)
            {
                throw new Exception("Cannot create surface for " + ToString());
            }
            retVal.XParameter = xParam;
            retVal.YParameter = yParam;

            return retVal;
        }
    }
}