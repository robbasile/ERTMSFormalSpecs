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
using DataDictionary.Rules;
using DataDictionary.Tests.Runner;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.Interpreter.Statement
{
    public class ApplyStatement : Statement, ISubDeclarator
    {
        /// <summary>
        ///     The procedure to call
        /// </summary>
        public Statement AppliedStatement { get; private set; }

        /// <summary>
        ///     The list on which the procedure should be called
        /// </summary>
        public Expression ListExpression { get; private set; }

        /// <summary>
        ///     The list on which the procedure should be called
        /// </summary>
        public Expression ConditionExpression { get; private set; }

        /// <summary>
        ///     The iterator variable
        /// </summary>
        public Variable IteratorVariable { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this element is built</param>
        /// <param name="log"></param>
        /// <param name="appliedStatement">The statement to be applied when the condition is satisfied</param>
        /// <param name="listExpression">The list to work on</param>
        /// <param name="conditionExpression">The condition to apply on the list elements</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public ApplyStatement(ModelElement root, ModelElement log, Statement appliedStatement, Expression listExpression,
            Expression conditionExpression, int start, int end)
            : base(root, log, start, end)
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            AppliedStatement = appliedStatement;
            AppliedStatement.Enclosing = this;

            ListExpression = listExpression;
            ListExpression.Enclosing = this;

            ConditionExpression = conditionExpression;
            if (ConditionExpression != null)
            {
                ConditionExpression.Enclosing = this;
            }

            IteratorVariable = (Variable) acceptor.getFactory().createVariable();
            IteratorVariable.Enclosing = this;
            IteratorVariable.Name = "X";
            InitDeclaredElements();
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            ISubDeclaratorUtils.AppendNamable(this, IteratorVariable);
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
        ///     Performs the semantic analysis of the statement
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance = null)
        {
            bool retVal = base.SemanticAnalysis(instance);

            if (retVal)
            {
                // ListExpression
                ListExpression.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                StaticUsage.AddUsages(ListExpression.StaticUsage, Usage.ModeEnum.ReadAndWrite);

                Collection collectionType = ListExpression.GetExpressionType() as Collection;
                if (collectionType != null)
                {
                    IteratorVariable.Type = collectionType.Type;
                }

                // ConditionExpression
                if (ConditionExpression != null)
                {
                    ConditionExpression.SemanticAnalysis(instance);
                    StaticUsage.AddUsages(ConditionExpression.StaticUsage, Usage.ModeEnum.Read);
                }

                AppliedStatement.SemanticAnalysis(instance);
                StaticUsage.AddUsages(AppliedStatement.StaticUsage, Usage.ModeEnum.Call);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the statement which modifies the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns>null if no statement modifies the element</returns>
        public override VariableUpdateStatement Modifies(ITypedElement variable)
        {
            VariableUpdateStatement retVal = AppliedStatement.Modifies(variable);

            return retVal;
        }

        /// <summary>
        ///     Provides the list of update statements induced by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void UpdateStatements(List<VariableUpdateStatement> retVal)
        {
            AppliedStatement.UpdateStatements(retVal);
        }

        /// <summary>
        ///     Provides the list of elements read by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void ReadElements(List<ITypedElement> retVal)
        {
            AppliedStatement.ReadElements(retVal);
        }

        /// <summary>
        ///     Checks the statement for semantical errors
        /// </summary>
        public override void CheckStatement()
        {
            if (ListExpression != null)
            {
                ListExpression.CheckExpression();
                Collection listExpressionType = ListExpression.GetExpressionType() as Collection;
                if (listExpressionType == null)
                {
                    Root.AddError("Target does not references a list");
                }
            }
            else
            {
                Root.AddError("List should be specified");
            }

            if (ConditionExpression != null)
            {
                ConditionExpression.CheckExpression();
            }

            if (AppliedStatement != null)
            {
                AppliedStatement.CheckStatement();
            }
            else
            {
                Root.AddError("Procedure should be specified in the APPLY statement");
            }
        }

        /// <summary>
        ///     Indicates whether the condition is satisfied with the value provided
        ///     Hyp : the value of the iterator variable has been assigned before
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public bool ConditionSatisfied(InterpretationContext context, ExplanationPart explain)
        {
            bool retVal = true;

            if (ConditionExpression != null)
            {
                BoolValue b = ConditionExpression.GetExpressionValue(context, explain) as BoolValue;
                ExplanationPart.CreateSubExplanation(explain, ConditionExpression, b); 
                
                if (b == null)
                {
                    retVal = false;
                }
                else
                {
                    retVal = b.Val;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the changes performed by this statement
        /// </summary>
        /// <param name="context">The context on which the changes should be computed</param>
        /// <param name="changes">The list to fill with the changes</param>
        /// <param name="explanation">The explanatino to fill, if any</param>
        /// <param name="apply">Indicates that the changes should be applied immediately</param>
        /// <param name="runner"></param>
        public override void GetChanges(InterpretationContext context, ChangeList changes, ExplanationPart explanation,
            bool apply, Runner runner)
        {
            // Explain what happens in this statement
            explanation = ExplanationPart.CreateSubExplanation(explanation, this);

            IVariable variable = ListExpression.GetVariable(context);
            if (variable != null)
            {
                if (variable.Value != EFSSystem.EmptyValue)
                {
                    // HacK : ensure that the value is a correct rigth side
                    // and keep the result of the right side operation
                    ListValue listValue = variable.Value.RightSide(variable, false, false) as ListValue;
                    variable.Value = listValue;

                    ExplanationPart.CreateSubExplanation(explanation, "Input data = ", listValue);
                    if (listValue != null)
                    {
                        int token = context.LocalScope.PushContext();
                        context.LocalScope.setVariable(IteratorVariable);
                        bool elementFound = false;
                        bool matchingElementFound = false;
                        foreach (IValue value in listValue.Val)
                        {
                            if (value != EFSSystem.EmptyValue)
                            {
                                elementFound = true;
                                IteratorVariable.Value = value;
                                if (ConditionSatisfied(context, explanation))
                                {
                                    matchingElementFound = true;
                                    AppliedStatement.GetChanges(context, changes, explanation, apply, runner);
                                }
                            }
                        }

                        if (!elementFound)
                        {
                            ExplanationPart.CreateSubExplanation(explanation, "Empty collection");
                        }
                        else if (!matchingElementFound)
                        {
                            ExplanationPart.CreateSubExplanation(explanation, "No matching element found");
                        }

                        context.LocalScope.PopContext(token);
                    }
                    else
                    {
                        Root.AddError("List expression does not evaluate to a list value");
                    }
                }
            }
            else
            {
                Root.AddError("Cannot find variable for " + ListExpression);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            explanation.Write("APPLY ");
            AppliedStatement.GetExplain(explanation);
            explanation.Write(" ON ");
            ListExpression.GetExplain(explanation);
            if (ConditionExpression != null)
            {
                explanation.Write(" | ");
                ConditionExpression.GetExplain(explanation);
            }
        }

        /// <summary>
        ///     Provides a real short description of this statement
        /// </summary>
        /// <returns></returns>
        public override string ShortShortDescription()
        {
            return ListExpression.ToString();
        }

        /// <summary>
        ///     Provides the main model elemnt affected by this statement
        /// </summary>
        /// <returns></returns>
        public override ModelElement AffectedElement()
        {
            return ListExpression.Ref as ModelElement;
        }
    }
}