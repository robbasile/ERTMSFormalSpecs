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
    public class RemoveStatement : Statement, ISubDeclarator
    {
        /// <summary>
        ///     The condition which should be true on the element to be removed
        /// </summary>
        public Expression Condition { get; private set; }

        /// <summary>
        ///     The list on which the value should be removed
        /// </summary>
        public Expression ListExpression { get; private set; }

        /// <summary>
        ///     Indicates which element should be removed
        /// </summary>
        public enum PositionEnum
        {
            First,
            Last,
            All
        };

        /// <summary>
        ///     The remove position
        /// </summary>
        public PositionEnum Position { get; private set; }

        /// <summary>
        ///     The iterator variable
        /// </summary>
        public Variable IteratorVariable { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this element is built</param>
        /// <param name="log"></param>
        /// <param name="condition">The corresponding function call designator</param>
        /// <param name="position">The position in which the element should be removed</param>
        /// <param name="listExpression">The expressions used to compute the parameters</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public RemoveStatement(ModelElement root, ModelElement log, Expression condition, PositionEnum position,
            Expression listExpression, int start, int end)
            : base(root, log, start, end)
        {
            Condition = condition;
            if (condition != null)
            {
                condition.Enclosing = this;
            }

            Position = position;

            ListExpression = listExpression;
            ListExpression.Enclosing = this;

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
            DeclaredElements = new Dictionary<string, List<INamable>>();

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
                ListExpression.SemanticAnalysis(instance);
                StaticUsage.AddUsages(ListExpression.StaticUsage, Usage.ModeEnum.ReadAndWrite);

                Collection collectionType = ListExpression.GetExpressionType() as Collection;
                if (collectionType != null)
                {
                    IteratorVariable.Type = collectionType.Type;
                }

                // Condition
                if (Condition != null)
                {
                    Condition.SemanticAnalysis(instance);
                    StaticUsage.AddUsages(Condition.StaticUsage, Usage.ModeEnum.Read);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the list of variable read by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void ReadElements(List<ITypedElement> retVal)
        {
            retVal.AddRange(ListExpression.GetVariables());

            if (Condition != null)
            {
                retVal.AddRange(Condition.GetVariables());
            }
        }

        /// <summary>
        ///     Provides the statement which modifies the element
        /// </summary>
        /// <param name="element"></param>
        /// <returns>null if no statement modifies the element</returns>
        public override VariableUpdateStatement Modifies(ITypedElement element)
        {
            return null;
        }

        /// <summary>
        ///     Provides the list of update statements induced by this statement
        /// </summary>
        /// <param name="retVal">the list to fill</param>
        public override void UpdateStatements(List<VariableUpdateStatement> retVal)
        {
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

            if (Condition != null)
            {
                BoolValue b = Condition.GetExpressionValue(context, explain) as BoolValue;
                ExplanationPart.CreateSubExplanation(explain, Condition, b);

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
        ///     Checks the statement for semantical errors
        /// </summary>
        public override void CheckStatement()
        {
            if (ListExpression != null)
            {
                if (ListExpression.Ref is Parameter)
                {
                    Root.AddError("Cannot change the list value which is a parameter (" + ListExpression + ")");
                }

                Collection targetListType = ListExpression.GetExpressionType() as Collection;
                if (targetListType != null)
                {
                    if (Condition != null)
                    {
                        Condition.CheckExpression();
                        BoolType conditionType = Condition.GetExpressionType() as BoolType;
                        if (conditionType == null)
                        {
                            Root.AddError("Condition does not evaluates to boolean");
                        }
                    }
                }
                else
                {
                    Root.AddError("Cannot determine type of " + ListExpression);
                }
            }
            else
            {
                Root.AddError("List should be specified");
            }
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
                // HacK : ensure that the value is a correct rigth side
                // and keep the result of the right side operation
                ListValue listValue = variable.Value.RightSide(variable, false, false) as ListValue;
                variable.Value = listValue;
                if (listValue != null)
                {
                    // Provide the state of the list before removing elements from it
                    ExplanationPart.CreateSubExplanation(explanation, "Input data = ", listValue);

                    ListValue newListValue = new ListValue(listValue.CollectionType, new List<IValue>());

                    int token = context.LocalScope.PushContext();
                    context.LocalScope.SetVariable(IteratorVariable);

                    int index = 0;
                    if (Position == PositionEnum.Last)
                    {
                        index = listValue.Val.Count - 1;
                    }

                    // Remove the element while required to do so
                    while (index >= 0 && index < listValue.Val.Count)
                    {
                        IValue value = listValue.Val[index];
                        index = NextIndex(index);

                        if (value == EFSSystem.INSTANCE.EmptyValue)
                        {
                            InsertInResult(newListValue, value);
                        }
                        else
                        {
                            IteratorVariable.Value = value;
                            if (ConditionSatisfied(context, explanation))
                            {
                                if (Position != PositionEnum.All)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                InsertInResult(newListValue, value);
                            }
                        }
                    }

                    // Complete the list
                    while (index >= 0 && index < listValue.Val.Count)
                    {
                        IValue value = listValue.Val[index];

                        InsertInResult(newListValue, value);
                        index = NextIndex(index);
                    }

                    // Fill the gap
                    while (newListValue.Val.Count < listValue.Val.Count)
                    {
                        newListValue.Val.Add(EFSSystem.INSTANCE.EmptyValue);
                    }

                    Change change = new Change(variable, variable.Value, newListValue);
                    changes.Add(change, apply, runner);
                    ExplanationPart.CreateSubExplanation(explanation, Root, change);

                    context.LocalScope.PopContext(token);
                }
            }
        }

        /// <summary>
        ///     Provides the next index of a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int NextIndex(int index)
        {
            if (Position == PositionEnum.Last)
            {
                index = index - 1;
            }
            else
            {
                index = index + 1;
            }
            return index;
        }

        /// <summary>
        ///     Inserts a value in the result set
        /// </summary>
        /// <param name="newListValue"></param>
        /// <param name="value"></param>
        private void InsertInResult(ListValue newListValue, IValue value)
        {
            if (Position == PositionEnum.Last)
            {
                newListValue.Val.Insert(0, value);
            }
            else
            {
                newListValue.Val.Add(value);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            explanation.Write("REMOVE ");

            switch (Position)
            {
                case PositionEnum.First:
                    explanation.Write("FIRST ");
                    break;

                case PositionEnum.Last:
                    explanation.Write("LAST ");
                    break;

                case PositionEnum.All:
                    explanation.Write("ALL ");
                    break;
            }

            if (Condition != null)
            {
                Condition.GetExplain(explanation);
            }

            explanation.Write(" IN ");
            ListExpression.GetExplain(explanation);
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