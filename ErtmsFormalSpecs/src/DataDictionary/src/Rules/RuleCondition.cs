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
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Structure = DataDictionary.Types.Structure;

namespace DataDictionary.Rules
{
    public class RuleCondition : Generated.RuleCondition, ITextualExplain
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public RuleCondition()
        {
        }

        /// <summary>
        ///     Indicates if this RuleCondition contains implemented sub-elements
        /// </summary>
        public override bool ImplementationPartiallyCompleted
        {
            get
            {
                if (getImplemented())
                {
                    return true;
                }

                foreach (Rule rule in SubRules)
                {
                    if (rule.ImplementationPartiallyCompleted)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        ///     Provides the preconditions associated to this rule condition
        /// </summary>
        public ArrayList PreConditions
        {
            get
            {
                if (allPreConditions() == null)
                {
                    setAllPreConditions(new ArrayList());
                }
                return allPreConditions();
            }
            set { setAllPreConditions(value); }
        }

        /// <summary>
        ///     Provides the precondition which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PreCondition FindPreCondition(string name)
        {
            return (PreCondition) NamableUtils.FindByName(name, PreConditions);
        }

        /// <summary>
        ///     Provides the set of preconditions (both local and from the eclosing rules)
        /// </summary>
        public List<PreCondition> AllPreConditions
        {
            get
            {
                List<PreCondition> retVal = new List<PreCondition>();

                RuleCondition current = this;
                while (current != null)
                {
                    foreach (PreCondition preCondition in current.PreConditions)
                    {
                        retVal.Add(preCondition);
                    }

                    // TODO : Also add the negation of the preceding rule conditions
                    current = current.EnclosingRule.EnclosingRuleCondition;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the actions associated to this rule condition
        /// </summary>
        public ArrayList Actions
        {
            get
            {
                if (allActions() == null)
                {
                    setAllActions(new ArrayList());
                }
                return allActions();
            }
            set { setAllActions(value); }
        }

        /// <summary>
        ///     Provides the action which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Action FindAction(string name)
        {
            return (Action) NamableUtils.FindByName(name, Actions);
        }

        /// <summary>
        ///     Provides the sub rules associated to this rule condition
        /// </summary>
        public ArrayList SubRules
        {
            get
            {
                if (allSubRules() == null)
                {
                    setAllSubRules(new ArrayList());
                }
                return allSubRules();
            }
            set { setAllSubRules(value); }
        }

        /// <summary>
        ///     Provides the sub rule which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Rule FindRule(string name)
        {
            return (Rule) NamableUtils.FindByName(name, SubRules);
        }

        /// <summary>
        ///     Provides the enclosing rule
        /// </summary>
        public Rule EnclosingRule
        {
            get { return getFather() as Rule; }
        }

        /// <summary>
        ///     Provides the enclosing structure
        /// </summary>
        public Structure EnclosingStructure
        {
            get { return EnclosingFinder<Structure>.find(this); }
        }

        /// <summary>
        ///     Provides the enclosing collection
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return EnclosingRule.RuleConditions; }
        }

        /// <summary>
        ///     Indicates whether this rule uses the typed element
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool Uses(IVariable variable)
        {
            return Modifies(variable) != null || Reads(variable);
        }

        /// <summary>
        ///     Provides the statement which modifies the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns>null if no statement modifies the element</returns>
        public VariableUpdateStatement Modifies(ITypedElement variable)
        {
            VariableUpdateStatement retVal = null;

            foreach (Action action in Actions)
            {
                retVal = action.Modifies(variable);
                if (retVal != null)
                {
                    return retVal;
                }
            }

            return retVal;
        }


        /// <summary>
        ///     Indicates whether this rule reads the content of this variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool Reads(ITypedElement variable)
        {
            foreach (PreCondition precondition in PreConditions)
            {
                if (precondition.Reads(variable))
                {
                    return true;
                }
            }

            foreach (Action action in Actions)
            {
                if (action.Reads(variable))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///     Evaluates the rule and its sub rules
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="priority">the priority level : a rule can be activated only if its priority level == priority</param>
        /// <param name="instance">The instance on which the rule must be evaluated</param>
        /// <param name="ruleConditions">the rule conditions to be activated</param>
        /// <param name="explanation">The explanation part to be filled</param>
        /// <param name="runner"></param>
        /// <returns>the number of actions that were activated during this evaluation</returns>
        public bool Evaluate(Runner runner, acceptor.RulePriority priority, IModelElement instance,
            HashSet<Runner.Activation> activations, ExplanationPart explanation)
        {
            bool retVal = false;

            ExplanationPart conditionExplanation = ExplanationPart.CreateSubExplanation(explanation, this);
            InterpretationContext context = new InterpretationContext(instance);
            retVal = EvaluatePreConditions(context, conditionExplanation, runner);

            if (retVal)
            {
                if (conditionExplanation != null)
                {
                    conditionExplanation.Message = "Condition " + Name + " satisfied";
                }

                foreach (Rule subRule in SubRules)
                {
                    subRule.Evaluate(runner, priority, instance, activations, conditionExplanation);
                }

                if (EnclosingRule.getPriority() == priority)
                {
                    activations.Add(new Runner.Activation(this, instance, conditionExplanation));
                }
            }
            else
            {
                if (conditionExplanation != null)
                {
                    conditionExplanation.Message = "Condition " + Name + " not satisfied";
                }
            }


            return retVal;
        }

        /// <summary>
        ///     Provides the actual value for the preconditions
        /// </summary>
        /// <param name="context">The context on which the precondition must be evaluated</param>
        /// <param name="explanation">The explanation part to fill, if any</param>
        /// <param name="log">indicates that this should be logged</param>
        /// <param name="runner"></param>
        /// <returns></returns>
        public bool EvaluatePreConditions(InterpretationContext context, ExplanationPart explanation, Runner runner)
        {
            bool retVal = true;

            foreach (PreCondition preCondition in PreConditions)
            {
                try
                {
                    ExplanationPart subExplanation = ExplanationPart.CreateNamedSubExplanation(explanation,
                        "PreCondition ", preCondition);
                    BoolValue value = preCondition.Expression.GetValue(context, subExplanation) as BoolValue;
                    ExplanationPart.SetNamable(subExplanation, value);
                    if (value != null)
                    {
                        retVal = retVal && value.Val;
                    }
                    else
                    {
                        retVal = false;
                        // TODO : Handle Error
                    }

                    if (!retVal)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    preCondition.Expression.AddErrorAndExplain(e.Message, explanation);
                    retVal = false;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            int indent = 0;

            if (PreConditions.Count > 0)
            {
                indent = 2;
                explanation.Write("IF ");
                if (PreConditions.Count > 1)
                {
                    // Prepare the space for the following ANDs
                    explanation.Write("   ");
                }

                bool first = true;
                foreach (PreCondition preCondition in PreConditions)
                {
                    if (!first)
                    {
                        explanation.WriteLine();
                        explanation.Write("   AND ");
                    }
                    preCondition.GetExplain(explanation, explainSubElements);
                    first = false;
                }
                explanation.WriteLine();
                explanation.WriteLine("THEN");
            }
            else
            {
                explanation.WriteLine();
            }

            explanation.Indent(indent, () =>
            {
                if (Name.CompareTo(EnclosingRule.Name) != 0)
                {
                    explanation.Comment(Name);
                }

                foreach (Action action in Actions)
                {
                    explanation.Write();
                    action.GetExplain(explanation, explainSubElements);
                    explanation.WriteLine();
                }

                if (explainSubElements)
                {
                    foreach (Rule subRule in SubRules)
                    {
                        subRule.GetExplain(explanation, explainSubElements);
                    }
                }
            });
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                PreCondition item = element as PreCondition;
                if (item != null)
                {
                    appendPreConditions(item);
                }
            }
            {
                Action item = element as Action;
                if (item != null)
                {
                    appendActions(item);
                }
            }
            {
                Rule item = element as Rule;
                if (item != null)
                {
                    appendSubRules(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        ///     Duplicates this model element
        /// </summary>
        /// <returns></returns>
        public RuleCondition duplicate()
        {
            RuleCondition retVal = (RuleCondition) acceptor.getFactory().createRuleCondition();
            retVal.Name = Name;
            foreach (PreCondition preCondition in PreConditions)
            {
                PreCondition newPreCondition = preCondition.duplicate();
                retVal.appendPreConditions(newPreCondition);
            }
            foreach (Action action in Actions)
            {
                Action newAction = action.duplicate();
                retVal.appendActions(newAction);
            }

            return retVal;
        }

        /// <summary>
        ///     Sets the update information for this rule condition (this rule condition updates source)
        /// </summary>
        /// <param name="source"></param>
        public override void SetUpdateInformation(ModelElement source)
        {
            base.SetUpdateInformation(source);
            RuleCondition sourceRuleCondition = (RuleCondition) source;

            foreach (Action action in Actions)
            {
                Action baseAction = sourceRuleCondition.FindAction(action.Name);
                if (baseAction != null)
                {
                    action.SetUpdateInformation(baseAction);
                }
            }

            foreach (PreCondition preCondition in PreConditions)
            {
                PreCondition basePreCondition = sourceRuleCondition.FindPreCondition(preCondition.Name);
                if (basePreCondition != null)
                {
                    preCondition.SetUpdateInformation(basePreCondition);
                }
            }

            foreach (Rule rule in SubRules)
            {
                Rule baseRule = sourceRuleCondition.FindRule(rule.Name);
                if (baseRule != null)
                {
                    rule.SetUpdateInformation(baseRule);
                }
            }
        }

        /// <summary>
        ///     Ensures that all update information has been deleted
        /// </summary>
        public override void RecoverUpdateInformation()
        {
            base.RecoverUpdateInformation();

            foreach (Action action in Actions)
            {
                action.RecoverUpdateInformation();
            }

            foreach (PreCondition precondition in PreConditions)
            {
                precondition.RecoverUpdateInformation();
            }

            foreach (Rule rule in SubRules)
            {
                rule.RecoverUpdateInformation();
            }
        }

        /// <summary>
        ///     Creates the status message 
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string result = base.CreateStatusMessage();

            result += "Rule condition" + Name + " with " + PreConditions.Count + " preconditions and " + Actions.Count + " actions";

            return result;
        }

    }
}