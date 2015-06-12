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
using System.Collections;
using System.Collections.Generic;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Tests.Runner;
using DataDictionary.Types;
using Utils;
using NameSpace = DataDictionary.Types.NameSpace;
using Step = DataDictionary.Tests.Step;
using Structure = DataDictionary.Types.Structure;
using SubStep = DataDictionary.Tests.SubStep;
using Translation = DataDictionary.Tests.Translations.Translation;

namespace DataDictionary.Rules
{
    public class Action : Generated.Action, IExpressionable, TextualExplain, ICommentable
    {
        public override string Name
        {
            get { return ExpressionText; }
            set { }
        }

        public override string ExpressionText
        {
            get
            {
                string retVal = getExpression();
                if (retVal == null)
                {
                    retVal = "";
                }
                return retVal;
            }
            set
            {
                setExpression(value);
                statement = null;
            }
        }

        /// <summary>
        ///     Provides the expression tree associated to this action's expression
        /// </summary>
        private Statement statement;

        public Statement Statement
        {
            get
            {
                if (statement == null)
                {
                    statement = EFSSystem.ParseStatement(this, ExpressionText);
                }
                return statement;
            }
            set { statement = value; }
        }

        public InterpreterTreeNode Tree
        {
            get { return Statement; }
        }

        /// <summary>
        ///     Clears the statement tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
            Statement = null;
        }

        /// <summary>
        ///     Creates the tree according to the statement text
        /// </summary>
        public InterpreterTreeNode Compile()
        {
            // Side effect, builds the statement if it is not already built
            return Tree;
        }

        /// <summary>
        ///     Indicates that the expression is valid for this IExpressionable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool checkValidExpression(string expression)
        {
            bool retVal = false;

            Statement tree = EFSSystem.Parser.Statement(this, expression, true);
            retVal = tree != null;

            return retVal;
        }

        /// <summary>
        ///     The enclosing Rule, if any
        /// </summary>
        public Rule Rule
        {
            get { return Enclosing as Rule; }
        }

        /// <summary>
        ///     The enclosing RuleCondition, if any
        /// </summary>
        public RuleCondition RuleCondition
        {
            get { return Enclosing as RuleCondition; }
        }

        /// <summary>
        ///     Finds the enclosing structure
        /// </summary>
        public Structure EnclosingStructure
        {
            get { return EnclosingFinder<Structure>.find(this); }
        }

        /// <summary>
        ///     Finds the enclosing namespace
        /// </summary>
        public NameSpace NameSpace
        {
            get { return EnclosingNameSpaceFinder.find(this); }
        }

        /// <summary>
        ///     The enclosing sub-step, if any
        /// </summary>
        public SubStep SubStep
        {
            get { return Enclosing as SubStep; }
        }

        /// <summary>
        ///     The enclosing step, if any
        /// </summary>
        public Step Step
        {
            get
            {
                Step retVal = null;

                if (SubStep != null)
                {
                    retVal = SubStep.Step;
                }

                return retVal; 
            }
        }

        /// <summary>
        ///     The enclosing translation, if any
        /// </summary>
        public Translation Translation
        {
            get
            {
                Translation result = null;
                if (SubStep != null)
                {
                    result = SubStep.Translation;
                }
                return result;
            }
        }

        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;

                if (RuleCondition != null)
                {
                    retVal = RuleCondition.Actions;
                }
                else if (SubStep != null)
                {
                    retVal = SubStep.Actions;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the list of update statements induced by this action
        /// </summary>
        public List<VariableUpdateStatement> UpdateStatements
        {
            get
            {
                List<VariableUpdateStatement> retVal = new List<VariableUpdateStatement>();

                if (Statement != null)
                {
                    Statement.UpdateStatements(retVal);
                }
                else
                {
                    AddError("Cannot parse statement");
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the statement which modifies the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns>null if no statement modifies the element</returns>
        public VariableUpdateStatement Modifies(ITypedElement variable)
        {
            if (Statement != null)
            {
                return Statement.Modifies(variable);
            }

            return null;
        }

        /// <summary>
        ///     Indicates whether this action reads the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool Reads(ITypedElement variable)
        {
            if (Statement != null)
            {
                return Statement.Reads(variable);
            }

            return false;
        }

        /// <summary>
        ///     Creates a list of changes to be applied on the system
        /// </summary>
        /// <param name="context">The context on which the changes should be computed</param>
        /// <param name="changes">The list of changes to be updated</param>
        /// <param name="explanation">The explanatino to fill, if any</param>
        /// <param name="apply">Indicates that the changes should be applied immediately</param>
        /// <param name="runner"></param>
        /// <returns>The list to fill with the changes</param>
        public virtual void GetChanges(InterpretationContext context, ChangeList changes, ExplanationPart explanation,
            bool apply, Runner runner)
        {
            long start = Environment.TickCount;

            try
            {
                if (Statement != null)
                {
                    Statement.GetChanges(context, changes, explanation, apply, runner);
                }
                else
                {
                    AddError("Invalid actions statement");
                }
            }
            catch (Exception e)
            {
                AddException(e);
            }

            long stop = Environment.TickCount;
            long span = (stop - start);

            if (RuleCondition != null && RuleCondition.EnclosingRule != null)
            {
                // Rule execution execution time (as opposed to guard evaluation)
                RuleCondition.EnclosingRule.ExecutionTimeInMilli += span;
                RuleCondition.EnclosingRule.ExecutionCount += 1;
            }
        }

        /// <summary>
        ///     Explains the pre Condition
        /// </summary>
        /// <returns></returns>
        public override string getExplain()
        {
            string retVal = ExpressionText;

            return retVal;
        }

        /// <summary>
        ///     Indicates the name of the updated variable, if any
        /// </summary>
        /// <returns></returns>
        public string UpdatedVariable()
        {
            string retVal = null;

            VariableUpdateStatement update = Statement as VariableUpdateStatement;
            if (update != null)
            {
                retVal = update.VariableIdentification.ToString();
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
        }

        /// <summary>
        ///     Duplicates this model element
        /// </summary>
        /// <returns></returns>
        public Action duplicate()
        {
            Action retVal = (Action) acceptor.getFactory().createAction();
            retVal.Name = Name;
            retVal.ExpressionText = ExpressionText;

            return retVal;
        }

        /// <summary>
        ///     Provides an explanation of the step's behaviour
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel, bool explainSubElements)
        {
            string retVal = TextualExplainUtilities.Comment(this, indentLevel);

            retVal += TextualExplainUtilities.Pad(ExpressionText, indentLevel);

            return retVal;
        }

        /// <summary>
        ///     Provides an explanation of the step's behaviour
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public string getExplain(bool explainSubElements)
        {
            string retVal = "";

            retVal = getExplain(0, explainSubElements);

            return retVal;
        }

        /// <summary>
        ///     The comment related to this element
        /// </summary>
        public string Comment
        {
            get { return getComment(); }
            set { setComment(value); }
        }
    }
}