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
using Utils;
using Collection = DataDictionary.Types.Collection;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.Interpreter.ListOperators
{
    public abstract class ListOperatorExpression : Expression, ISubDeclarator
    {
        /// <summary>
        ///     List operators
        /// </summary>
        public static string[] LIST_OPERATORS =
        {
            ThereIsExpression.OPERATOR,
            ForAllExpression.OPERATOR,
            FirstExpression.OPERATOR,
            LastExpression.OPERATOR,
            CountExpression.OPERATOR,
            ReduceExpression.OPERATOR,
            SumExpression.OPERATOR,
            MapExpression.OPERATOR
        };

        /// <summary>
        ///     The expression which evaluates to a list
        /// </summary>
        public Expression ListExpression { get; private set; }

        /// <summary>
        ///     The iterator variable
        /// </summary>
        public Variable IteratorVariable { get; private set; }

        /// <summary>
        ///     The name of the iterator variable
        /// </summary>
        public string IteratorName { get; private set; }

        /// <summary>
        ///     The iterator variable during the previous iteration
        /// </summary>
        public Variable PreviousIteratorVariable { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="listExpression"></param>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        /// <param name="iteratorVariableName">The name of the iterator variable</param>
        public ListOperatorExpression(ModelElement root, ModelElement log, Expression listExpression,
            string iteratorVariableName, int start, int end)
            : base(root, log, start, end)
        {
            ListExpression = listExpression;
            ListExpression.Enclosing = this;

            IteratorVariable = (Variable) acceptor.getFactory().createVariable();
            IteratorVariable.Enclosing = this;
            IteratorVariable.Name = iteratorVariableName;

            PreviousIteratorVariable = (Variable) acceptor.getFactory().createVariable();
            PreviousIteratorVariable.Enclosing = this;
            PreviousIteratorVariable.Name = "prevX";

            InitDeclaredElements();
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            ISubDeclaratorUtils.AppendNamable(this, IteratorVariable);
            ISubDeclaratorUtils.AppendNamable(this, PreviousIteratorVariable);
        }

        /// <summary>
        ///     The elements declared by this declarator
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; private set; }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                // ListExpression
                ListExpression.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                StaticUsage.AddUsages(ListExpression.StaticUsage, Usage.ModeEnum.Read);

                Collection collectionType = ListExpression.GetExpressionType() as Collection;
                if (collectionType != null)
                {
                    StaticUsage.AddUsage(collectionType, Root, Usage.ModeEnum.Type);
                    IteratorVariable.Type = collectionType.Type;
                    PreviousIteratorVariable.Type = collectionType.Type;
                }
                else
                {
                    AddError("Cannot determine collection type on list expression " + ToString());
                }
            }

            return retVal;
        }

        protected bool ElementFound = false;
        protected bool MatchingElementFound = false;

        /// <summary>
        ///     Prepares the iteration on the context provided
        /// </summary>
        /// <param name="context"></param>
        /// <returns>the token required to EndIteration</returns>
        protected virtual int PrepareIteration(InterpretationContext context)
        {
            int retVal = context.LocalScope.PushContext();
            context.LocalScope.setVariable(IteratorVariable);
            context.LocalScope.setVariable(PreviousIteratorVariable);

            PreviousIteratorVariable.Value = EFSSystem.EmptyValue;
            IteratorVariable.Value = EFSSystem.EmptyValue;

            ElementFound = false;
            MatchingElementFound = false;

            return retVal;
        }

        /// <summary>
        ///     Prepares the next iteration
        /// </summary>
        protected virtual void NextIteration()
        {
            PreviousIteratorVariable.Value = IteratorVariable.Value;
        }

        /// <summary>
        ///     Ends the iteration
        /// </summary>
        /// <param name="context"></param>
        protected virtual void EndIteration(InterpretationContext context, ExplanationPart explain, int token)
        {
            if (!ElementFound)
            {
                ExplanationPart.CreateSubExplanation(explain, "Empty collection");
            }
            else if (!MatchingElementFound)
            {
                ExplanationPart.CreateSubExplanation(explain, "No matching element found");
            }

            context.LocalScope.PopContext(token);
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void fill(List<INamable> retVal, BaseFilter filter)
        {
            ListExpression.fill(retVal, filter);
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public override void checkExpression()
        {
            base.checkExpression();

            if (ListExpression != null)
            {
                ListExpression.checkExpression();

                Type listExpressionType = ListExpression.GetExpressionType();
                if (!(listExpressionType is Collection))
                {
                    AddError("List expression " + ListExpression.ToString() + " should hold a collection");
                }
            }
            else
            {
                AddError("List expression should be provided");
            }
        }
    }
}