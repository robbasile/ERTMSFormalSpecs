// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpecs software and documentation
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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Rules;
using DataDictionary.Tests.Runner.Events;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Action = DataDictionary.Rules.Action;
using Collection = DataDictionary.Types.Collection;
using NameSpace = DataDictionary.Types.NameSpace;
using Rule = DataDictionary.Generated.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;
using Structure = DataDictionary.Types.Structure;
using Variable = DataDictionary.Generated.Variable;
using Visitor = DataDictionary.Generated.Visitor;

namespace DataDictionary.Tests.Runner
{
    public class Runner
    {
        /// <summary>
        ///     The event time line for this runner
        /// </summary>
        public EventTimeLine EventTimeLine { get; private set; }

        /// <summary>
        ///     Indicates whether an explanation should be provided to all actions
        /// </summary>
        public bool Explain { get; set; }

        /// <summary>
        ///     The test case for which this runner has been created
        /// </summary>
        public SubSequence SubSequence { get; private set; }

        /// <summary>
        ///     The step between two activations
        /// </summary>
        private double _step = 0.1;

        public double Step
        {
            get { return _step; }
            set { _step = value; }
        }

        /// <summary>
        ///     The current time
        /// </summary>
        public double Time
        {
            get { return EventTimeLine.CurrentTime; }
            set { EventTimeLine.CurrentTime = value; }
        }

        /// <summary>
        ///     The last time when activation has been performed
        /// </summary>
        public double LastActivationTime { get; set; }

        /// <summary>
        ///     Indicates that clients should wait
        /// </summary>
        public bool PleaseWait { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="subSequence"></param>
        /// <param name="explain"></param>
        /// <param name="ensureCompilation">Indicates that the runner should make sure that the system is compiled</param>
        public Runner(SubSequence subSequence, bool explain, bool ensureCompilation)
        {
            EventTimeLine = new EventTimeLine();
            SubSequence = subSequence;
            EfsSystem.Instance.Runner = this;
            Explain = explain;

            if (ensureCompilation)
            {
                // Compile everything
                EfsSystem.Instance.Compiler.Compile_Synchronous(EfsSystem.Instance.ShouldRebuild);
                EfsSystem.Instance.ShouldRebuild = false;
            }

            Setup();
            PleaseWait = true;
        }

        /// <summary>
        ///     A simple runner
        /// </summary>
        public Runner(bool explain, int step = 100, int storeEventCount = 0)
        {
            EventTimeLine = new EventTimeLine();
            SubSequence = null;
            Step = step;
            EventTimeLine.MaxNumberOfEvents = storeEventCount;
            EfsSystem.Instance.Runner = this;
            Explain = explain;

            // Compile everything
            EfsSystem.Instance.Compiler.Compile_Synchronous(EfsSystem.Instance.ShouldRebuild);
            EfsSystem.Instance.ShouldRebuild = false;

            Setup();
        }

        /// <summary>
        ///     Sets up all variables before any execution on the system
        /// </summary>
        private class Setuper : Visitor
        {
            /// <summary>
            ///     Sets the default values to each variable
            /// </summary>
            /// <param name="variable">The variable to set</param>
            /// <param name="subNodes">Indicates whether sub nodes should be considered</param>
            public override void visit(Variable variable, bool subNodes)
            {
                Variables.Variable var = (Variables.Variable) variable;

                var.Value = var.DefaultValue;

                base.visit(variable, subNodes);
            }

            /// <summary>
            ///     Indicates which rules are not active
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Rule obj, bool visitSubNodes)
            {
                Rules.Rule rule = obj as Rules.Rule;
                if (rule != null)
                {
                    rule.ActivationPriorities = null;
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Clear the cache of all functions
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Function obj, bool visitSubNodes)
            {
                Functions.Function function = obj as Functions.Function;

                if (function != null)
                {
                    function.ClearCache();
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Initializes the execution time for functions and rules
        /// </summary>
        private class ExecutionTimeInitializer : Visitor
        {
            public override void visit(Function obj, bool visitSubNodes)
            {
                Functions.Function function = obj as Functions.Function;

                if (function != null)
                {
                    function.ExecutionCount = 0;
                    function.ExecutionTimeInMilli = 0L;
                }

                base.visit(obj, visitSubNodes);
            }

            public override void visit(Rule obj, bool visitSubNodes)
            {
                Rules.Rule rule = obj as Rules.Rule;

                if (rule != null)
                {
                    rule.ExecutionCount = 0;
                    rule.ExecutionTimeInMilli = 0L;
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Sets up the runner before performing a test case
        /// </summary>
        public void Setup()
        {
            Util.DontNotify(() =>
            {
                // Clears all caches
                FinderRepository.INSTANCE.ClearCache();
                EfsSystem.Instance.ClearFunctionCache();

                // Setup the execution environment
                Setuper setuper = new Setuper();
                ExecutionTimeInitializer executionTimeInitializer = new ExecutionTimeInitializer();
                foreach (Dictionary dictionary in EfsSystem.Instance.Dictionaries)
                {
                    setuper.visit(dictionary);
                    executionTimeInitializer.visit(dictionary);
                }

                // Setup the step
                if (SubSequence != null)
                {
                    Expression expression = SubSequence.Frame.CycleDuration;
                    IValue value = expression.GetExpressionValue(new InterpretationContext(SubSequence.Frame), null);
                    Step = Functions.Function.GetDoubleValue(value);
                }

                PleaseWait = false;
            });
        }

        public class Activation
        {
            /// <summary>
            ///     The action to activate
            /// </summary>
            public RuleCondition RuleCondition { get; private set; }

            /// <summary>
            ///     The instance on which the action is applied
            /// </summary>
            public IModelElement Instance { get; private set; }

            /// <summary>
            ///     The explanation why this activation has been performed
            /// </summary>
            public ExplanationPart Explanation { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="ruleCondition">The rule condition which leads to this activation</param>
            /// <param name="instance">The instance on which this rule condition's preconditions are evaluated to true</param>
            /// <param name="explanation"></param>
            public Activation(RuleCondition ruleCondition, IModelElement instance, ExplanationPart explanation)
            {
                RuleCondition = ruleCondition;
                Instance = instance;
                Explanation = explanation;
            }

            /// <summary>
            ///     Indicates that two Activations are the same when they share the action and,
            ///     if specified, the instance on which they are applied
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                bool retVal = false;

                Activation other = obj as Activation;
                if (other != null)
                {
                    retVal = RuleCondition.Equals(other.RuleCondition);
                    if (retVal && Instance != null)
                    {
                        if (other.Instance != null)
                        {
                            retVal = Instance.Equals(other.Instance);
                        }
                        else
                        {
                            retVal = false;
                        }
                    }
                }
                return retVal;
            }

            /// <summary>
            ///     The hash code, according to Equal operator.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                int retVal = RuleCondition.GetHashCode();

                if (Instance != null)
                {
                    retVal = retVal + Instance.GetHashCode();
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the order in which rules should be activated
        /// </summary>
        public static acceptor.RulePriority[] PrioritiesOrder =
        {
            acceptor.RulePriority.aVerification,
            acceptor.RulePriority.aUpdateINTERNAL,
            acceptor.RulePriority.aProcessing,
            acceptor.RulePriority.aUpdateOUT,
            acceptor.RulePriority.aCleanUp
        };

        /// <summary>
        ///     The current priority
        /// </summary>
        public acceptor.RulePriority? CurrentPriority { get; private set; }

        /// <summary>
        ///     Activates the rules in the dictionary until stabilisation
        /// </summary>
        public void Cycle()
        {
            CurrentPriority = null;

            LastActivationTime = Time;

            Utils.ModelElement.Errors = new Dictionary<Utils.ModelElement, List<ElementLog>>();

            foreach (acceptor.RulePriority priority in PrioritiesOrder)
            {
                InnerExecuteOnePriority(priority);
            }
            CurrentPriority = null;

            RegisterErrors(Utils.ModelElement.Errors);

            EventTimeLine.GarbageCollect();

            EventTimeLine.CurrentTime += Step;
        }

        /// <summary>
        ///     Executes a single rule priority (shared version of the method)
        /// </summary>
        /// <param name="priority"></param>
        private void InnerExecuteOnePriority(acceptor.RulePriority priority)
        {
            Util.DontNotify(() =>
            {

                CurrentPriority = priority;

                // Activates the processing engine
                HashSet<Activation> activations = new HashSet<Activation>();
                foreach (Dictionary dictionary in EfsSystem.Instance.Dictionaries)
                {
                    foreach (NameSpace nameSpace in dictionary.NameSpaces)
                    {
                        SetupNameSpaceActivations(priority, activations, nameSpace);
                    }
                }

                List<VariableUpdate> updates = new List<VariableUpdate>();
                EvaluateActivations(activations, priority, ref updates);
                ApplyUpdates(updates);
                CheckExpectationsState(priority);
            });
        }

        /// <summary>
        ///     Executes the interpretation machine for one priority
        /// </summary>
        /// <param name="priority"></param>
        public void ExecuteOnePriority(acceptor.RulePriority priority)
        {
            try
            {
                ControllersManager.NamableController.DesactivateNotification();
                LastActivationTime = Time;

                Utils.ModelElement.Errors = new Dictionary<Utils.ModelElement, List<ElementLog>>();

                // Executes a single rule priority
                InnerExecuteOnePriority(priority);

                EventTimeLine.GarbageCollect();
            }
            finally
            {
                ControllersManager.NamableController.ActivateNotification();
            }

            if (priority == acceptor.RulePriority.aCleanUp)
            {
                EventTimeLine.CurrentTime += Step;
            }
        }

        /// <summary>
        ///     Determines the set of rules in a specific namespace to be applied.
        /// </summary>
        /// <param name="priority">The priority for which this activation is requested</param>
        /// <param name="activations">The set of activations to be filled</param>
        /// <param name="nameSpace">The namespace to consider</param>
        /// <returns></returns>
        protected void SetupNameSpaceActivations(acceptor.RulePriority priority, HashSet<Activation> activations,
            NameSpace nameSpace)
        {
            // Finds all activations in sub namespaces
            foreach (NameSpace subNameSpace in nameSpace.NameSpaces)
            {
                SetupNameSpaceActivations(priority, activations, subNameSpace);
            }

            foreach (Rules.Rule rule in nameSpace.Rules)
            {
                // We only apply rules that have not been updated
                ExplanationPart explanation = new ExplanationPart(rule, "Rule evaluation");
                rule.Evaluate(this, priority, rule, activations, explanation);
            }

            foreach (IVariable variable in nameSpace.Variables)
            {
                EvaluateVariable(priority, activations, variable,
                    new ExplanationPart(variable as ModelElement, "Evaluating variable"));
            }
        }

        /// <summary>
        ///     Evaluates the rules associated to a single variable
        /// </summary>
        /// <param name="priority">The priority in which this variable is evaluated</param>
        /// <param name="activations">The activation list result of this evaluation</param>
        /// <param name="variable">The variable to evaluate</param>
        /// <param name="explanation">The explanation part to be filled</param>
        private void EvaluateVariable(acceptor.RulePriority priority, HashSet<Activation> activations,
            IVariable variable, ExplanationPart explanation)
        {
            if (variable != null && variable.Value != EfsSystem.Instance.EmptyValue)
            {
                if (variable.Type is Structure)
                {
                    Structure structure = variable.Type as Structure;
                    foreach (Rules.Rule rule in structure.Rules)
                    {
                        rule.Evaluate(this, priority, variable, activations, explanation);
                    }

                    StructureValue value = variable.Value as StructureValue;
                    if (value != null)
                    {
                        foreach (IVariable subVariable in value.SubVariables.Values)
                        {
                            EvaluateVariable(priority, activations, subVariable, explanation);
                        }
                    }
                }
                else if (variable.Type is StateMachine)
                {
                    EvaluateStateMachine(activations, priority, variable, explanation);
                }
                else if (variable.Type is Collection)
                {
                    Collection collectionType = variable.Type as Collection;
                    if (variable.Value != EfsSystem.Instance.EmptyValue)
                    {
                        ListValue val = variable.Value as ListValue;

                        if (val != null)
                        {
                            foreach (IValue subVal in val.Val)
                            {
                                Variables.Variable tmp = new Variables.Variable
                                {
                                    Name = "list_entry",
                                    Type = collectionType.Type,
                                    Value = subVal
                                };

                                EvaluateVariable(priority, activations, tmp, explanation);
                            }
                        }
                        else
                        {
                            ModelElement element = variable as ModelElement;
                            if (element != null)
                            {
                                element.AddError("Variable " + variable.Name + " does not hold a collection but " +
                                                 variable.Value);
                            }
                            else
                            {
                                throw new Exception("Variable " + variable.Name + " does not hold a collection but " +
                                                    variable.Value);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Try to find a rule, in this state machine, or in a sub state machine
        ///     which
        /// </summary>
        /// <param name="activations"></param>
        /// <param name="priority">The priority when this evaluation occurs</param>
        /// <param name="currentStateVariable">The variable which holds the current state of the procedure</param>
        /// <param name="explanation">The explanation part to be filled</param>
        private void EvaluateStateMachine(HashSet<Activation> activations, acceptor.RulePriority priority,
            IVariable currentStateVariable, ExplanationPart explanation)
        {
            if (currentStateVariable != null)
            {
                State currentState = currentStateVariable.Value as State;
                if (currentState != null)
                {
                    StateMachine currentStateMachine = currentState.StateMachine;
                    while (currentStateMachine != null)
                    {
                        foreach (Rules.Rule rule in currentStateMachine.Rules)
                        {
                            rule.Evaluate(this, priority, currentStateVariable, activations, explanation);
                        }
                        currentStateMachine = currentStateMachine.EnclosingStateMachine;
                    }
                }
            }
        }

        /// <summary>
        ///     Indicates that the changes performed should be checked for compatibility
        /// </summary>
        private bool CheckForCompatibleChanges = false;

        /// <summary>
        ///     Applies the selected actions and update the system state
        /// </summary>
        /// <param name="activations"></param>
        /// <param name="updates"></param>
        /// <param name="priority"></param>
        public void EvaluateActivations(HashSet<Activation> activations, acceptor.RulePriority priority,
            ref List<VariableUpdate> updates)
        {
            Dictionary<IVariable, Change> changes = new Dictionary<IVariable, Change>();
            Dictionary<Change, VariableUpdate> traceBack = new Dictionary<Change, VariableUpdate>();

            foreach (Activation activation in activations)
            {
                if (activation.RuleCondition.Actions.Count > 0)
                {
                    // Register the fact that a rule has been triggered
                    RuleFired ruleFired = new RuleFired(activation, priority);
                    EventTimeLine.AddModelEvent(ruleFired, this, true);
                    ExplanationPart changesExplanation = ExplanationPart.CreateSubExplanation(activation.Explanation,
                        "Changes");

                    // Registers all model updates due to this rule triggering
                    foreach (Action action in activation.RuleCondition.Actions)
                    {
                        if (action.Statement != null)
                        {
                            VariableUpdate variableUpdate = new VariableUpdate(action, activation.Instance, priority);
                            variableUpdate.ComputeChanges(false, this);
                            EventTimeLine.AddModelEvent(variableUpdate, this, false);
                            ruleFired.AddVariableUpdate(variableUpdate);
                            if (changesExplanation != null)
                            {
                                changesExplanation.SubExplanations.Add(variableUpdate.Explanation);
                            }
                            updates.Add(variableUpdate);

                            if (CheckForCompatibleChanges)
                            {
                                ChangeList actionChanges = variableUpdate.Changes;
                                if (variableUpdate.Action.Statement is ProcedureCallStatement)
                                {
                                    Dictionary<IVariable, Change> procedureChanges = new Dictionary<IVariable, Change>();

                                    foreach (Change change in variableUpdate.Changes.Changes)
                                    {
                                        procedureChanges[change.Variable] = change;
                                    }

                                    actionChanges = new ChangeList();
                                    foreach (Change change in procedureChanges.Values)
                                    {
                                        actionChanges.Add(change, false, this);
                                    }
                                }

                                foreach (Change change in actionChanges.Changes)
                                {
                                    IVariable variable = change.Variable;
                                    if (changes.ContainsKey(change.Variable))
                                    {
                                        Change otherChange = changes[change.Variable];
                                        Action otherAction = traceBack[otherChange].Action;
                                        if (!variable.Type.CompareForEquality(otherChange.NewValue, change.NewValue))
                                        {
                                            string action1 = ((INamable) action.Enclosing).FullName + " : " +
                                                             variableUpdate.Action.FullName;
                                            string action2 = ((INamable) otherAction.Enclosing).FullName + " : " +
                                                             traceBack[otherChange].Action.FullName;
                                            variableUpdate.Action.AddError(
                                                "Simultaneous change of the same variable with different values. Conflit between " +
                                                action1 + " and " + action2);
                                        }
                                    }
                                    else
                                    {
                                        changes.Add(change.Variable, change);
                                        traceBack.Add(change, variableUpdate);
                                    }
                                }
                            }
                        }
                        else
                        {
                            action.AddError("Cannot parse action statement");
                        }
                    }
                }
            }

            // Handles the leave & enter state rules
            List<VariableUpdate> updatesToProcess = updates;
            updates = new List<VariableUpdate>();

            // Avoid considering twice the same transition
            List<Tuple<State, State>> transitions = new List<Tuple<State, State>>();

            while (updatesToProcess.Count > 0)
            {
                List<VariableUpdate> newUpdates = new List<VariableUpdate>();

                foreach (VariableUpdate update in updatesToProcess)
                {
                    updates.Add(update);

                    foreach (Change change in update.Changes.Changes)
                    {
                        if (change.Variable.Type is StateMachine)
                        {
                            State leavingState = (State) change.Variable.Value;
                            State enteringState = (State) change.NewValue;

                            bool transitionFound = false;
                            foreach (Tuple<State, State> transition in transitions)
                            {
                                if ((transition.Item1 == leavingState) && (transition.Item2 == enteringState))
                                {
                                    transitionFound = true;
                                    break;
                                }
                            }

                            if (! transitionFound)
                            {
                                Tuple<State, State> transition = new Tuple<State, State>(leavingState, enteringState);
                                transitions.Add(transition);

                                HandleLeaveState(priority, newUpdates, change.Variable, (State) change.Variable.Value,
                                    (State) change.NewValue);
                                HandleEnterState(priority, newUpdates, change.Variable, (State) change.Variable.Value,
                                    (State) change.NewValue);
                            }
                        }
                    }
                }

                updatesToProcess = newUpdates;
            }
        }

        private readonly HashSet<State> _processedStates = new HashSet<State>();

        /// <summary>
        ///     Add actions when entering a state
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="updates"></param>
        /// <param name="variable"></param>
        /// <param name="leaveState"></param>
        /// <param name="enterState"></param>
        private void HandleEnterState(acceptor.RulePriority priority, List<VariableUpdate> updates, IVariable variable,
            State leaveState, State enterState)
        {
            if (!_processedStates.Contains(enterState))
            {
                _processedStates.Add(enterState);

                if (!enterState.getStateMachine().Contains(enterState, leaveState))
                {
                    if (enterState.getEnterAction() != null)
                    {
                        Rules.Rule rule = (Rules.Rule) enterState.getEnterAction();
                        ExplanationPart explanation = new ExplanationPart(rule, "Rule evaluation");
                        HashSet<Activation> newActivations = new HashSet<Activation>();
                        List<VariableUpdate> newUpdates = new List<VariableUpdate>();
                        rule.Evaluate(this, priority, variable, newActivations, explanation);
                        EvaluateActivations(newActivations, priority, ref newUpdates);
                        updates.AddRange(newUpdates);
                    }

                    if (enterState.EnclosingState != null)
                    {
                        HandleEnterState(priority, updates, variable, leaveState, enterState.EnclosingState);
                    }
                }

                _processedStates.Remove(enterState);
            }
        }

        /// <summary>
        ///     Add actions when leaving a state
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="updates"></param>
        /// <param name="variable"></param>
        /// <param name="leaveState"></param>
        /// <param name="enterState"></param>
        private void HandleLeaveState(acceptor.RulePriority priority, List<VariableUpdate> updates, IVariable variable,
            State leaveState, State enterState)
        {
            if (!_processedStates.Contains(leaveState))
            {
                _processedStates.Add(leaveState);

                if (!leaveState.getStateMachine().Contains(leaveState, enterState))
                {
                    if (leaveState.getLeaveAction() != null)
                    {
                        Rules.Rule rule = (Rules.Rule) leaveState.getLeaveAction();
                        ExplanationPart explanation = new ExplanationPart(rule, "Rule evaluation");
                        HashSet<Activation> newActivations = new HashSet<Activation>();
                        List<VariableUpdate> newUpdates = new List<VariableUpdate>();
                        rule.Evaluate(this, priority, variable, newActivations, explanation);
                        EvaluateActivations(newActivations, priority, ref newUpdates);
                        updates.AddRange(newUpdates);
                    }

                    if (leaveState.EnclosingState != null)
                    {
                        HandleLeaveState(priority, updates, variable, leaveState.EnclosingState, enterState);
                    }
                }

                _processedStates.Remove(leaveState);
            }
        }

        /// <summary>
        ///     Applies the updates on the system
        /// </summary>
        /// <param name="updates"></param>
        private void ApplyUpdates(IEnumerable<VariableUpdate> updates)
        {
            foreach (VariableUpdate update in updates)
            {
                update.Apply(this);
            }
        }

        /// <summary>
        ///     Setups the sub-step by applying its actions and adding its expects in the expect list
        /// </summary>
        public void SetupSubStep(SubStep subStep)
        {
            Util.DontNotify(() =>
            {
                LogInstance = subStep;

                // No setup can occur when some expectations are still active
                if (!EventTimeLine.ContainsSubStep(subStep))
                {
                    EventTimeLine.AddModelEvent(new SubStepActivated(subStep, CurrentPriority), this, true);
                }
            });
        }

        /// <summary>
        ///     Provides the still active expectations
        /// </summary>
        /// <returns></returns>
        public HashSet<Expect> ActiveExpectations()
        {
            return new HashSet<Expect>(EventTimeLine.ActiveExpectations);
        }

        /// <summary>
        ///     Provides the still active and blocking expectations
        /// </summary>
        /// <returns></returns>
        public HashSet<Expect> ActiveBlockingExpectations()
        {
            return EventTimeLine.ActiveBlockingExpectations();
        }

        /// <summary>
        ///     Provides the failed expectations
        /// </summary>
        /// <returns></returns>
        public HashSet<ModelEvent> FailedExpectations()
        {
            return EventTimeLine.FailedExpectations();
        }

        /// <summary>
        ///     Updates the expectation state according to the variables' value
        /// </summary>
        /// <param name="priority">The priority for which this check is performed</param>
        private void CheckExpectationsState(acceptor.RulePriority priority)
        {
            // Update the state of the expectation according to system's state
            foreach (Expect expect in ActiveExpectations())
            {
                Expectation expectation = expect.Expectation;

                // Determine if the deadline is reached
                if (expect.TimeOut < EventTimeLine.CurrentTime)
                {
                    switch (expect.Expectation.getKind())
                    {
                        case acceptor.ExpectationKind.aInstantaneous:
                        case acceptor.ExpectationKind.defaultExpectationKind:
                            // Instantaneous expectation who raised its deadling
                            EventTimeLine.AddModelEvent(new FailedExpectation(expect, CurrentPriority, null), this, true);
                            break;

                        case acceptor.ExpectationKind.aContinuous:
                            // Continuous expectation who raised its deadline
                            EventTimeLine.AddModelEvent(new ExpectationReached(expect, CurrentPriority, null), this,
                                true);
                            break;
                    }
                }
                else
                {
                    ExplanationPart explanation = new ExplanationPart(expectation,
                        "Expectation " + expectation.Expression);
                    try
                    {
                        if (expectation.getCyclePhase() == acceptor.RulePriority.defaultRulePriority ||
                            expectation.getCyclePhase() == priority)
                        {
                            switch (expectation.getKind())
                            {
                                case acceptor.ExpectationKind.aInstantaneous:
                                case acceptor.ExpectationKind.defaultExpectationKind:
                                    if (getBoolValue(expectation, expectation.Expression, explanation))
                                    {
                                        // An instantaneous expectation who reached its satisfactory condition
                                        EventTimeLine.AddModelEvent(
                                            new ExpectationReached(expect, priority, explanation), this, true);
                                    }
                                    else
                                    {
                                        expectation.Explain = explanation;
                                    }
                                    break;

                                case acceptor.ExpectationKind.aContinuous:
                                    if (expectation.getCondition() != null)
                                    {
                                        if (!getBoolValue(expectation, expectation.ConditionTree, explanation))
                                        {
                                            // An continuous expectation who reached its satisfactory condition
                                            EventTimeLine.AddModelEvent(
                                                new ExpectationReached(expect, priority, explanation), this, true);
                                        }
                                        else
                                        {
                                            if (!getBoolValue(expectation, expectation.Expression, explanation))
                                            {
                                                // A continuous expectation who reached a case where it is not satisfied
                                                EventTimeLine.AddModelEvent(
                                                    new FailedExpectation(expect, priority, explanation), this, true);
                                            }
                                            else
                                            {
                                                expectation.Explain = explanation;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!getBoolValue(expectation, expectation.Expression, explanation))
                                        {
                                            // A continuous expectation who reached a case where it is not satisfied
                                            EventTimeLine.AddModelEvent(
                                                new FailedExpectation(expect, priority, explanation), this, true);
                                        }
                                        else
                                        {
                                            expectation.Explain = explanation;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        expectation.AddErrorAndExplain(e.Message, explanation);
                    }
                }
            }
        }

        /// <summary>
        ///     Provides the value of the expression provided
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="expression"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        private bool getBoolValue(ModelElement instance, Expression expression, ExplanationPart explain)
        {
            bool retVal;

            InterpretationContext context = new InterpretationContext(instance);
            BoolValue val = expression.GetExpressionValue(context, explain) as BoolValue;

            if (val != null)
            {
                retVal = val.Val;
            }
            else
            {
                throw new Exception("Cannot evaluate value of " + expression);
            }

            return retVal;
        }

        /// <summary>
        ///     Runs until all expectations are reached or failed
        /// </summary>
        public void RunForBlockingExpectations(bool performCycle)
        {
            if (performCycle)
            {
                Cycle();
            }

            while (ActiveBlockingExpectations().Count > 0)
            {
                Cycle();
            }
        }

        /// <summary>
        ///     Runs until all expectations are reached or failed
        /// </summary>
        public void RunForExpectations(bool performCycle)
        {
            if (performCycle)
            {
                Cycle();
            }

            while (ActiveBlockingExpectations().Count > 0)
            {
                Cycle();
            }
        }

        /// <summary>
        ///     Indicates that no test has been run yet
        /// </summary>
        private const int TestNotRun = -1;

        /// <summary>
        ///     Indicates that the current test case & current step & current sub-step must be rebuilt from the time line
        /// </summary>
        private const int RebuildCurrentSubStep = -2;

        /// <summary>
        ///     Indicates that the current test case & current step & current sub-step must be rebuilt from the time line
        /// </summary>
        private const int NoMoreStep = -3;

        /// <summary>
        ///     The index of the last activated sub-step in the current test case
        /// </summary>
        private int _currentSubStepIndex = TestNotRun;

        /// <summary>
        ///     The index of the last activated step in the current test case
        /// </summary>
        private int _currentStepIndex = TestNotRun;

        /// <summary>
        ///     The index of the test case in which the last activated step belongs
        /// </summary>
        private int _currentTestCaseIndex = TestNotRun;

        /// <summary>
        ///     Provides the next test case
        /// </summary>
        /// <returns></returns>
        public TestCase CurrentTestCase()
        {
            TestCase retVal = null;

            if (SubSequence != null && _currentTestCaseIndex != NoMoreStep)
            {
                if (_currentTestCaseIndex >= 0 && _currentTestCaseIndex < SubSequence.TestCases.Count)
                {
                    retVal = (TestCase) SubSequence.TestCases[_currentTestCaseIndex];
                }
            }

            return retVal;
        }

        /// <summary>
        ///     steps to the next test case
        /// </summary>
        private void NextTestCase()
        {
            if (_currentTestCaseIndex != NoMoreStep)
            {
                if (_currentTestCaseIndex == RebuildCurrentSubStep)
                {
                    _currentStepIndex = RebuildCurrentSubStep;
                    NextStep();
                }
                else
                {
                    _currentTestCaseIndex += 1;
                    TestCase testCase = CurrentTestCase();
                    while (testCase != null && testCase.Steps.Count == 0 &&
                           _currentTestCaseIndex < SubSequence.TestCases.Count)
                    {
                        _currentTestCaseIndex += 1;
                        testCase = CurrentTestCase();
                    }

                    if (testCase == null)
                    {
                        _currentTestCaseIndex = NoMoreStep;
                        _currentStepIndex = NoMoreStep;
                    }
                }
            }
        }

        /// <summary>
        ///     Provides the current test step
        /// </summary>
        /// <returns></returns>
        public Step CurrentStep()
        {
            Step retVal = null;

            if (_currentStepIndex != NoMoreStep)
            {
                TestCase testCase = CurrentTestCase();
                if (testCase != null)
                {
                    if (_currentStepIndex >= 0 && _currentStepIndex < testCase.Steps.Count)
                    {
                        retVal = (Step) testCase.Steps[_currentStepIndex];
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Steps to the next step (either in the current test case, or in the next test case)
        /// </summary>
        private void NextStep()
        {
            if (_currentStepIndex != NoMoreStep)
            {
                Step step = CurrentStep();

                do
                {
                    if (_currentStepIndex != RebuildCurrentSubStep)
                    {
                        _currentStepIndex += 1;
                        TestCase testCase = CurrentTestCase();
                        if (testCase == null)
                        {
                            NextTestCase();
                            testCase = CurrentTestCase();
                        }

                        if (testCase != null && _currentStepIndex >= testCase.Steps.Count)
                        {
                            NextTestCase();
                            testCase = CurrentTestCase();
                            if (testCase != null)
                            {
                                _currentStepIndex = 0;
                            }
                            else
                            {
                                _currentTestCaseIndex = NoMoreStep;
                                _currentStepIndex = NoMoreStep;
                            }
                        }
                        step = CurrentStep();
                    }
                } while (step != null && step.IsEmpty());
            }
        }

        /// <summary>
        ///     Provides the current test step
        /// </summary>
        /// <returns></returns>
        public SubStep CurrentSubStep()
        {
            SubStep retVal = null;

            if (_currentSubStepIndex != NoMoreStep)
            {
                if (_currentSubStepIndex == RebuildCurrentSubStep)
                {
                    _currentTestCaseIndex = -1;
                    _currentStepIndex = -1;
                    _currentSubStepIndex = -1;
                    int previousTestCaseIndex = _currentTestCaseIndex;
                    int previousStepIndex = _currentStepIndex;
                    int previousSubStepIndex = _currentSubStepIndex;

                    NextSubStep();
                    retVal = CurrentSubStep();
                    while (retVal != null && EventTimeLine.SubStepActivationCache.ContainsKey(retVal))
                    {
                        previousTestCaseIndex = _currentTestCaseIndex;
                        previousStepIndex = _currentStepIndex;
                        previousSubStepIndex = _currentSubStepIndex;

                        NextSubStep();
                        retVal = CurrentSubStep();
                    }

                    _currentTestCaseIndex = previousTestCaseIndex;
                    _currentStepIndex = previousStepIndex;
                    _currentSubStepIndex = previousSubStepIndex;
                }

                Step step = CurrentStep();
                if (step != null)
                {
                    if (_currentSubStepIndex >= 0 && _currentSubStepIndex < step.SubSteps.Count)
                    {
                        retVal = (SubStep) step.SubSteps[_currentSubStepIndex];
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Steps to the next sub-step (either in the current test case, or in the next test case)
        /// </summary>
        private void NextSubStep()
        {
            if (_currentSubStepIndex != NoMoreStep)
            {
                SubStep subStep = CurrentSubStep();
                Step step;

                do
                {
                    _currentSubStepIndex++;
                    step = CurrentStep();
                    if (step == null)
                    {
                        NextStep();
                        step = CurrentStep();
                    }

                    if (step != null && _currentSubStepIndex >= step.SubSteps.Count)
                    {
                        NextStep();
                        step = CurrentStep();
                        if (step != null)
                        {
                            _currentSubStepIndex = 0;
                        }
                        else
                        {
                            _currentTestCaseIndex = NoMoreStep;
                            _currentStepIndex = NoMoreStep;
                        }
                    }
                    subStep = CurrentSubStep();
                } while (step != null && (step.IsEmpty() || subStep.IsEmpty()));
            }
        }

        /// <summary>
        ///     Runs the test case until the step provided is encountered
        ///     This does not execute the corresponding step.
        /// </summary>
        /// <param name="target"></param>
        public void RunUntilStep(Step target)
        {
            Util.DontNotify(() =>
            {
                _currentStepIndex = NoMoreStep;
                _currentTestCaseIndex = NoMoreStep;

                if (target != null)
                {
                    RunForBlockingExpectations(false);
                }
                else
                {
                    RunForExpectations(false);
                }

                // Run all following steps until the target step is encountered
                foreach (TestCase testCase in SubSequence.TestCases)
                {
                    foreach (Step step in testCase.Steps)
                    {
                        if (step == target)
                        {
                            _currentStepIndex = RebuildCurrentSubStep;
                            _currentTestCaseIndex = RebuildCurrentSubStep;
                            break;
                        }

                        if (!EventTimeLine.ContainsStep(step))
                        {
                            foreach (SubStep subStep in step.SubSteps)
                            {
                                SetupSubStep(subStep);
                                if (!subStep.getSkipEngine())
                                {
                                    if (target != null)
                                    {
                                        RunForBlockingExpectations(true);
                                    }
                                    else
                                    {
                                        RunForExpectations(true);
                                    }
                                }
                                else
                                {
                                    foreach (acceptor.RulePriority priority in PrioritiesOrder)
                                    {
                                        CheckExpectationsState(priority);
                                    }
                                }
                            }

                            while (EventTimeLine.ActiveBlockingExpectations().Count > 0)
                            {
                                Cycle();
                            }
                        }
                    }

                    if (_currentTestCaseIndex == RebuildCurrentSubStep)
                    {
                        break;
                    }
                }
            });
        }

        /// <summary>
        ///     Runs the test case until the step provided is encountered
        ///     This does not execute the corresponding step.
        /// </summary>
        /// <param name="targetTime"></param>
        public void RunUntilTime(double targetTime)
        {
            while (EventTimeLine.CurrentTime < targetTime)
            {
                SubStep subStep = null;
                if (ActiveBlockingExpectations().Count == 0)
                {
                    NextSubStep();
                    subStep = CurrentSubStep();
                    if (subStep != null)
                    {
                        SetupSubStep(subStep);
                    }
                }

                if (subStep == null || !subStep.getSkipEngine())
                {
                    Cycle();
                }
                else
                {
                    if (subStep.getSkipEngine())
                    {
                        CheckExpectationsState(acceptor.RulePriority.aCleanUp);
                    }
                    EventTimeLine.CurrentTime += Step;
                }
            }
        }

        /// <summary>
        ///     Steps one step backward in this run
        /// </summary>
        public void StepBack()
        {
            EventTimeLine.StepBack(_step);
            _currentSubStepIndex = RebuildCurrentSubStep;
            _currentStepIndex = RebuildCurrentSubStep;
            _currentTestCaseIndex = RebuildCurrentSubStep;
        }

        /// <summary>
        ///     Indicates whether a rule condition has been activated at a given time
        /// </summary>
        /// <param name="ruleCondition">The rule condition that should be activated</param>
        /// <param name="time">the time when the rule condition should be activated</param>
        /// <param name="variable">The variable impacted by this rule condition, if any</param>
        /// <returns>true if the corresponding rule condition has been activated at the time provided</returns>
        public bool RuleActivatedAtTime(RuleCondition ruleCondition, double time, IVariable variable)
        {
            return EventTimeLine.RuleActivatedAtTime(ruleCondition, time, variable);
        }

        /// <summary>
        ///     Provides the log instance, an object on which logging should be performed
        /// </summary>
        public ModelElement LogInstance { get; set; }

        /// <summary>
        ///     Terminates the execution of a run
        /// </summary>
        public void EndExecution()
        {
            ExecutionTimeInitializer initializer = new ExecutionTimeInitializer();
            foreach (Dictionary dictionary in EfsSystem.Instance.Dictionaries)
            {
                initializer.visit(dictionary);
            }
        }

        /// <summary>
        ///     Registers the errors raised during evaluation and create ModelInterpretationFailure for each one of them
        /// </summary>
        /// <param name="errors"></param>
        private void RegisterErrors(Dictionary<Utils.ModelElement, List<ElementLog>> errors)
        {
            foreach (KeyValuePair<Utils.ModelElement, List<ElementLog>> pair in errors)
            {
                foreach (ElementLog log in pair.Value)
                {
                    switch (log.Level)
                    {
                        case ElementLog.LevelEnum.Error:
                            ModelInterpretationFailure modelInterpretationFailure = new ModelInterpretationFailure(log,
                                pair.Key, null);
                            ModelElement modelElement = pair.Key as ModelElement;
                            if (modelElement != null)
                            {
                                modelInterpretationFailure.Explanation = modelElement.Explain;
                            }
                            EventTimeLine.AddModelEvent(modelInterpretationFailure, this, true);
                            break;

                        case ElementLog.LevelEnum.Warning:
                            break;
                        case ElementLog.LevelEnum.Info:
                            break;
                    }
                }
            }
        }
    }
}