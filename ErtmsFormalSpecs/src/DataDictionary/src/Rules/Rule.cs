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
using System.Linq;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Tests.Runner;
using DataDictionary.Variables;
using Utils;
using NameSpace = DataDictionary.Types.NameSpace;
using Paragraph = DataDictionary.Specification.Paragraph;
using Procedure = DataDictionary.Functions.Procedure;
using StateMachine = DataDictionary.Types.StateMachine;
using Structure = DataDictionary.Types.Structure;
using Visitor = DataDictionary.Generated.Visitor;

namespace DataDictionary.Rules
{
    public class Rule : Generated.Rule, IGraphicalDisplay
    {
        /// <summary>
        ///     Provides the execution time for this rule, in milliseconds
        /// </summary>
        public long ExecutionTimeInMilli { get; set; }

        /// <summary>
        ///     Provides the number of times this rule has been executed
        /// </summary>
        public int ExecutionCount { get; set; }

        /// <summary>
        ///     Indicates if this Rule contains implemented sub-elements
        /// </summary>
        public override bool ImplementationPartiallyCompleted
        {
            get
            {
                if (getImplemented())
                {
                    return true;
                }

                foreach (RuleCondition ruleCondition in RuleConditions)
                {
                    if (ruleCondition.ImplementationPartiallyCompleted)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        ///     Provides the namespace associated to the rule
        /// </summary>
        public NameSpace NameSpace
        {
            get { return EnclosingNameSpaceFinder.find(this); }
        }

        /// <summary>
        ///     The preconditions for this rule
        /// </summary>
        public ArrayList RuleConditions
        {
            get
            {
                if (allConditions() == null)
                {
                    setAllConditions(new ArrayList());
                }
                return allConditions();
            }
        }

        /// <summary>
        ///     Provides the rule condition which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RuleCondition FindRuleCondition(string name)
        {
            return (RuleCondition) NamableUtils.FindByName(name, RuleConditions);
        }

        /// <summary>
        ///     The traces to the specifications
        /// </summary>
        public ArrayList Traces
        {
            get
            {
                if (allRequirements() == null)
                {
                    setAllRequirements(new ArrayList());
                }
                return allRequirements();
            }
        }

        /// <summary>
        ///     The enclosing rule, if any
        /// </summary>
        public Rule EnclosingRule
        {
            get { return Enclosing as Rule; }
        }

        /// <summary>
        ///     The enclosing rule condition, if any
        /// </summary>
        public RuleCondition EnclosingRuleCondition
        {
            get { return Enclosing as RuleCondition; }
        }

        /// <summary>
        ///     The enclosing procedure, if any
        /// </summary>
        public Procedure EnclosingProcedure
        {
            get { return Enclosing as Procedure; }
        }

        /// <summary>
        ///     The enclosing structure (if any)
        /// </summary>
        public Structure EnclosingStructure
        {
            get { return EnclosingFinder<Structure>.find(this); }
        }

        /// <summary>
        ///     The enclosing state machine (if any)
        /// </summary>
        public StateMachine EnclosingStateMachine
        {
            get { return EnclosingFinder<StateMachine>.find(this); }
        }

        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal;

                if (EnclosingRuleCondition != null)
                {
                    retVal = EnclosingRuleCondition.SubRules;
                }
                else if (EnclosingProcedure != null)
                {
                    retVal = EnclosingProcedure.Rules;
                }
                else if (EnclosingStateMachine != null)
                {
                    retVal = EnclosingStateMachine.Rules;
                }
                else if (EnclosingStructure != null)
                {
                    retVal = EnclosingStructure.Rules;
                }
                else
                {
                    retVal = NameSpace.Rules;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates if the implementation of this Rule is completed
        /// </summary>
        public override bool ImplementationCompleted
        {
            get
            {
                bool retVal = getImplemented();
                foreach (RuleCondition ruleCondition in RuleConditions)
                {
                    foreach (Rule rule in ruleCondition.SubRules)
                    {
                        retVal = retVal && rule.ImplementationCompleted;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Comment(this);

            bool first = true;
            bool condition = false;
            foreach (RuleCondition ruleCondition in RuleConditions)
            {
                if (!first)
                {
                    explanation.WriteLine("ELSE");
                }
                ruleCondition.GetExplain(explanation, explainSubElements);
                first = false;
                condition = condition || ruleCondition.PreConditions.Count > 0;
            }

            if (condition)
            {
                explanation.WriteLine("END IF");
            }
        }

        /// <summary>
        ///     Provides the requirements for enclosing rules
        /// </summary>
        public List<ReqRef> EnclosingRequirements
        {
            get
            {
                List<ReqRef> retVal = new List<ReqRef>();

                Rule enclosing = EnclosingRule;
                while (enclosing != null)
                {
                    foreach (ReqRef req in enclosing.Traces)
                    {
                        retVal.Add(req);
                    }
                    enclosing = enclosing.EnclosingRule;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the list of applicable paragraphs
        /// </summary>
        public List<Paragraph> ApplicableParagraphs
        {
            get
            {
                List<Paragraph> retVal;

                if (EnclosingRequirements.Count == 0)
                {
                    retVal = new List<Paragraph>();
                    foreach (Specification.Specification specification in Dictionary.Specifications)
                    {
                        retVal.AddRange(specification.AllParagraphs);
                    }
                }
                else
                {
                    retVal = new List<Paragraph>();

                    foreach (ReqRef req in EnclosingRequirements)
                    {
                        foreach (Specification.Specification specification in Dictionary.Specifications)
                        {
                            specification.SubParagraphs(req.Name, retVal);
                        }
                    }
                }
                return retVal;
            }
        }

        /// <summary>
        ///     Provides the activation priority list for this rule
        /// </summary>
        private HashSet<acceptor.RulePriority> _activationPriorities;

        public HashSet<acceptor.RulePriority> ActivationPriorities
        {
            get
            {
                if (_activationPriorities == null)
                {
                    _activationPriorities = new HashSet<acceptor.RulePriority>();
                    _activationPriorities.Add(getPriority());
                    foreach (RuleCondition condition in RuleConditions)
                    {
                        foreach (Rule subRule in condition.SubRules)
                        {
                            _activationPriorities.UnionWith(subRule.ActivationPriorities);
                        }
                    }
                }
                return _activationPriorities;
            }
            set { _activationPriorities = value; }
        }

        /// <summary>
        ///     Evaluates the rule and its sub rules
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="priority">the priority level : a rule can be activated only if its priority level == priority</param>
        /// <param name="instance">The instance on which the rule must be evaluated</param>
        /// <param name="activations">the rule conditions to be activated</param>
        /// <param name="explanation">The explanation part to be filled</param>
        /// <returns>the number of actions that were activated during this evaluation</returns>
        public bool Evaluate(Runner runner, acceptor.RulePriority? priority, IModelElement instance,
            HashSet<Runner.Activation> activations, ExplanationPart explanation)
        {
            bool retVal = false;

            if (UpdatedBy.Count == 0 && !IsRemoved && (priority == null || ActivationPriorities.Contains((acceptor.RulePriority) priority)))
            {
                long start = Environment.TickCount;

                foreach (RuleCondition ruleCondition in RuleConditions)
                {
                    retVal = ruleCondition.Evaluate(runner, priority, instance, activations, explanation);
                    if (retVal)
                    {
                        break;
                    }
                }

                // Guard evaluation execution time
                long stop = Environment.TickCount;
                long span = (stop - start);
                ExecutionTimeInMilli += span;
            }

            return retVal;
        }

        /// <summary>
        ///     Finds all usages of a TypedElement
        /// </summary>
        private class UsageVisitor : Visitor
        {
            /// <summary>
            ///     The usages
            /// </summary>
            public HashSet<RuleCondition> Usages { get; private set; }

            /// <summary>
            ///     The element looked for
            /// </summary>
            public IVariable Target { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="target"></param>
            public UsageVisitor(IVariable target)
            {
                Target = target;
                Usages = new HashSet<RuleCondition>();
            }

            /// <summary>
            ///     Take care of all conditions
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Generated.RuleCondition obj, bool visitSubNodes)
            {
                RuleCondition ruleCondition = (RuleCondition) obj;

                if (ruleCondition.Uses(Target))
                {
                    Usages.Add(ruleCondition);
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Provides the set of rules which uses this variable
        /// </summary>
        /// <param name="node">the element to find in rules</param>
        /// <returns>the list of rules which use the element provided</returns>
        public static HashSet<RuleCondition> RulesUsingThisElement(IVariable node)
        {
            UsageVisitor visitor = new UsageVisitor(node);

            EfsSystem efsSystem = EnclosingFinder<EfsSystem>.find(node);
            if (efsSystem != null)
            {
                foreach (Dictionary dictionary in efsSystem.Dictionaries)
                {
                    visitor.visit(dictionary);
                }
            }

            return visitor.Usages;
        }

        /// <summary>
        ///     Provides all the paragraphs associated to this rule
        /// </summary>
        /// <param name="paragraphs">The list of paragraphs to be filled</param>
        /// <returns></returns>
        public override void findRelatedParagraphsRecursively(List<Paragraph> paragraphs)
        {
            base.findRelatedParagraphsRecursively(paragraphs);

            // Perform the call recursively
            foreach (RuleCondition ruleCondition in RuleConditions)
            {
                foreach (Rule subRule in ruleCondition.SubRules)
                {
                    subRule.findRelatedParagraphsRecursively(paragraphs);
                }
            }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                RuleCondition item = element as RuleCondition;
                if (item != null)
                {
                    appendConditions(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        ///     Indicates that this rule has been defined in a procedure
        /// </summary>
        /// <returns></returns>
        public bool BelongsToAProcedure()
        {
            Procedure procedure = EnclosingFinder<Procedure>.find(this);

            return procedure != null;
        }

        /// <summary>
        ///     Duplicates this model element
        /// </summary>
        /// <returns></returns>
        public Rule duplicate()
        {
            Rule retVal = (Rule) acceptor.getFactory().createRule();
            retVal.Name = Name;
            foreach (RuleCondition ruleCondition in RuleConditions)
            {
                RuleCondition newRuleCondition = ruleCondition.duplicate();
                retVal.appendConditions(newRuleCondition);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a copy of the rule in the designated dictionary. The namespace structure is copied over.
        ///     The new rule is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public Rule CreateRuleUpdate(Dictionary dictionary)
        {
            Rule retVal = (Rule) Duplicate();
            retVal.SetUpdateInformation(this);
            retVal.ClearAllRequirements();

            String[] names = FullName.Split('.');
            names = names.Take(names.Count() - 1).ToArray();

            if (Enclosing is NameSpace)
            {
                NameSpace nameSpace = dictionary.GetNameSpaceUpdate(names, Dictionary);
                nameSpace.appendRules(retVal);
            }
            else
            {
                String[] nameSpaceRef = names.Take(names.Count() - 1).ToArray();

                if (EnclosingStateMachine != null)
                {
                    StateMachine stateMachine = EnclosingStateMachine.CreateSubStateMachineUpdate(dictionary);
                    stateMachine.appendRules(retVal);
                }
                else if (EnclosingStructure != null)
                {
                    NameSpace nameSpace = dictionary.GetNameSpaceUpdate(nameSpaceRef, Dictionary);
                    Structure structure = nameSpace.GetStructureUpdate(names.Last(), (NameSpace) nameSpace.Updates);
                    structure.appendRules(retVal);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Deletes all requirements linked to this rule and all sub-elements
        /// </summary>
        public override void ClearAllRequirements()
        {
            base.ClearAllRequirements();

            foreach (RuleCondition condition in RuleConditions)
            {
                condition.ClearAllRequirements();
                condition.Requirements.Clear();
                foreach (Rule subRule in condition.SubRules)
                {
                    subRule.ClearAllRequirements();
                }
            }
        }

        /// <summary>
        ///     Sets the update information for this rule (this rule updates source)
        /// </summary>
        /// <param name="source"></param>
        public override void SetUpdateInformation(ModelElement source)
        {
            base.SetUpdateInformation(source);
            Rule sourceRule = (Rule) source;

            foreach (RuleCondition ruleCondition in RuleConditions)
            {
                RuleCondition baseRuleCondition = sourceRule.FindRuleCondition(ruleCondition.Name);
                if (baseRuleCondition != null)
                {
                    ruleCondition.SetUpdateInformation(baseRuleCondition);
                }
            }
        }

        /// <summary>
        ///     Ensures that all update information for this rule has been removed
        /// </summary>
        public override void RecoverUpdateInformation()
        {
            base.RecoverUpdateInformation();

            foreach (RuleCondition ruleCondition in RuleConditions)
            {
                ruleCondition.RecoverUpdateInformation();
            }
        }

        /// <summary>
        ///     The X position
        /// </summary>
        public int X
        {
            get { return getX(); }
            set { setX(value); }
        }

        /// <summary>
        ///     The Y position
        /// </summary>
        public int Y
        {
            get { return getY(); }
            set { setY(value); }
        }

        /// <summary>
        ///     The width
        /// </summary>
        public int Width
        {
            get { return getWidth(); }
            set { setWidth(value); }
        }

        /// <summary>
        ///     The height
        /// </summary>
        public int Height
        {
            get { return getHeight(); }
            set { setHeight(value); }
        }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public string GraphicalName
        {
            get { return Name; }
        }

        /// <summary>
        ///     Indicates whether the namespace is hidden
        /// </summary>
        public bool Hidden
        {
            get { return getHidden(); }
            set { setHidden(value); }
        }

        /// <summary>
        ///     Indicates that the element is pinned
        /// </summary>
        public bool Pinned
        {
            get { return getPinned(); }
            set { setPinned(value); }
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string result = base.CreateStatusMessage();

            result += "Rule " + Name + " with " + RuleConditions.Count + " rule conditions";

            return result;
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static Rule CreateDefault(ICollection enclosingCollection)
        {
            Rule retVal = (Rule) acceptor.getFactory().createRule();

            Util.DontNotify(() =>
            {
                retVal.Name = "Rule" + GetElementNumber(enclosingCollection);

                RuleCondition condition = (RuleCondition) acceptor.getFactory().createRuleCondition();
                condition.Name = "<Condition1>";
                retVal.appendConditions(condition);
            });

            return retVal;
        }
    }
}