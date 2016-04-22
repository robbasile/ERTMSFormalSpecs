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
using Range = DataDictionary.Types.Range;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;
using Structure = DataDictionary.Types.Structure;

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
        /// The model elements for which the cache should be cleaned
        /// </summary>
        public CacheImpact CacheImpact { get; set; }

        /// <summary>
        /// The time stored in the model
        /// </summary>
        private IVariable TimeInModel { get; set; }

        /// <summary>
        ///     The current time
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        ///     The last time when activation has been performed
        /// </summary>
        public double LastActivationTime { get; set; }

        /// <summary>
        ///     Indicates that clients should wait
        /// </summary>
        public bool PleaseWait { get; set; }

        /// <summary>
        /// Indicates that no variables are accessed twice during the same cycle
        /// </summary>
        public bool CheckForCompatibleChanges { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subSequence"></param>
        /// <param name="explain"></param>
        /// <param name="ensureCompilation">Indicates that the runner should make sure that the system is compiled</param>
        /// <param name="checkForCompatibleChanges">Indicates that the runner should check that no variables are accessed twice during the same cycle</param>
        public Runner(SubSequence subSequence, bool explain, bool ensureCompilation, bool checkForCompatibleChanges = false)
        {
            EventTimeLine = new EventTimeLine(this);
            SubSequence = subSequence;
            CompletedSubStep = new HashSet<SubStep>();
            EfsSystem.Instance.Runner = this;
            Explain = explain;
            CheckForCompatibleChanges = checkForCompatibleChanges;

            if (ensureCompilation)
            {
                // Compile everything
                EfsSystem.Instance.Compiler.Compile_Synchronous(EfsSystem.Instance.ShouldRebuild);
                EfsSystem.Instance.ShouldRebuild = false;
            }

            Setup();
            PleaseWait = true;

            Expression expression = new Parser().Expression(subSequence.Dictionary, "Kernel.DateAndTime.CurrentTime");
            TimeInModel = expression.GetVariable(new InterpretationContext());
            Range range = TimeInModel.Type as Range;
            if (range == null || range.getPrecision() != acceptor.PrecisionEnum.aDoublePrecision)
            {
                TimeInModel = null;
            }
        }

        /// <summary>
        ///     A simple runner
        /// </summary>
        public Runner(bool explain, int storeEventCount = 0)
        {
            EventTimeLine = new EventTimeLine(this);
            SubSequence = null;
            CompletedSubStep = new HashSet<SubStep>();
            EventTimeLine.MaxNumberOfEvents = storeEventCount;
            EfsSystem.Instance.Runner = this;
            Explain = explain;

            // Compile everything
            EfsSystem.Instance.Compiler.Compile_Synchronous(EfsSystem.Instance.ShouldRebuild);
            EfsSystem.Instance.ShouldRebuild = false;

            Setup();
            TimeInModel = null;
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
                    // ReSharper disable once UnusedVariable
                    IValue value = expression.GetExpressionValue(new InterpretationContext(SubSequence.Frame), null);
                }

                PleaseWait = false;
            });
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

            NextCycle();
        }

        /// <summary>
        /// Updates the system to setup next cycle
        /// </summary>
        private void NextCycle()
        {
            if (TimeInModel != null)
            {
                // Use the time in the model for checking deadlines
                Range range = TimeInModel.Type as Range;
                if (range != null)
                {
                    Time = range.getValueAsDouble(TimeInModel.Value);
                }
            }
            else
            {
                // Increase time if it is not taken into account in the model
                Time += 0.1;
            }
        }

        /// <summary>
        ///     Executes a single rule priority (shared version of the method)
        /// </summary>
        /// <param name="priority"></param>
        private void InnerExecuteOnePriority(acceptor.RulePriority priority)
        {
            Util.DontNotify(() =>
            {
                CacheImpact = new CacheImpact();
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
                ClearCaches();
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
                NextCycle();
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
                if (variable.Type != null && variable.Type.ApplicableRule(priority))
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
                    EventTimeLine.AddModelEvent(ruleFired, true);
                    ExplanationPart changesExplanation = ExplanationPart.CreateSubExplanation(activation.Explanation,
                        "Changes");

                    // Registers all model updates due to this rule triggering
                    foreach (Action action in activation.RuleCondition.Actions)
                    {
                        if (action.Statement != null)
                        {
                            VariableUpdate variableUpdate = new VariableUpdate(action, activation.Instance, priority);
                            variableUpdate.ComputeChanges(false, this);
                            EventTimeLine.AddModelEvent(variableUpdate, false);
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
                                            if (change.CheckForCompatibility() || otherChange.CheckForCompatibility())
                                            {
                                                string action1 = ((INamable)action.Enclosing).FullName + " : " +
                                                                 variableUpdate.Action.FullName;
                                                string action2 = ((INamable)otherAction.Enclosing).FullName + " : " +
                                                                 traceBack[otherChange].Action.FullName;
                                                variableUpdate.Action.AddError(
                                                    "Simultaneous change of the variable " + variable.FullName + " with different values. Conflit between\n" +
                                                    action1 + "\n and \n" + action2);
                                                action.AddError("Conflicting change");
                                                otherAction.AddError("Conflicting change");
                                            }
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
                            State leavingState = (State) change.PreviousValue;
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

                                HandleLeaveState(priority, newUpdates, change.Variable, leavingState, enteringState);
                                HandleEnterState(priority, newUpdates, change.Variable, leavingState, enteringState);
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
                        // the priority is not specified for the rule evaluation since
                        // the rules of the enter states have to be executed regardless the priority
                        rule.Evaluate(this, null, variable, newActivations, explanation);
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
                        // the priority is not specified for the rule evaluation since
                        // the rules of the leave states have to be executed regardless the priority
                        rule.Evaluate(this, null, variable, newActivations, explanation);
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
        /// Clears the cache of all impacted elements
        /// </summary>
        public void ClearCaches()
        {
            CacheImpact.ClearCaches();
            CacheImpact = null;
        }

        /// <summary>
        ///     Setups the sub-step by applying its actions and adding its expects in the expect list
        /// </summary>
        /// <returns>True if the substep was not already seetup</returns>
        public bool SetupSubStep(SubStep subStep)
        {
            bool retVal = false;

            if (subStep != null)
            {
                if (!EventTimeLine.ContainsSubStep(subStep))
                {
                    Util.DontNotify(() =>
                    {
                        LogInstance = subStep;
                        CacheImpact = new CacheImpact();
                        EventTimeLine.AddModelEvent(new SubStepActivated(subStep, CurrentPriority), true);
                        ClearCaches();
                    });

                    retVal = true;
                }
            }

            return retVal;
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
                            EventTimeLine.AddModelEvent(new FailedExpectation(expect, CurrentPriority, null), true);
                            break;

                        case acceptor.ExpectationKind.aContinuous:
                            // Continuous expectation who raised its deadline
                            EventTimeLine.AddModelEvent(new ExpectationReached(expect, CurrentPriority, null), true);
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
                                            new ExpectationReached(expect, priority, explanation), true);
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
                                                new ExpectationReached(expect, priority, explanation), true);
                                        }
                                        else
                                        {
                                            if (!getBoolValue(expectation, expectation.Expression, explanation))
                                            {
                                                // A continuous expectation who reached a case where it is not satisfied
                                                EventTimeLine.AddModelEvent(
                                                    new FailedExpectation(expect, priority, explanation), true);
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
                                                new FailedExpectation(expect, priority, explanation), true);
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
        /// The list of SubSteps for which expectations have been reached
        /// </summary>
        private HashSet<SubStep> CompletedSubStep { get; set; }

        /// <summary>
        ///     Provides the current test step
        /// </summary>
        /// <returns></returns>
        public SubStep CurrentSubStep()
        {
            if (SubSequence != null)
            {
                foreach (TestCase testCase in SubSequence.TestCases)
                {
                    foreach (Step step in testCase.Steps)
                    {
                        foreach (SubStep subStep in step.SubSteps)
                        {
                            if (!CompletedSubStep.Contains(subStep))
                            {
                                return subStep;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Steps to the next sub-step (either in the current test case, or in the next test case)
        /// </summary>
        private void NextSubStep()
        {
            SubStep current = CurrentSubStep();
            if (current != null)
            {
                CompletedSubStep.Add(current);
            }
        }

        /// <summary>
        ///     Provides the next test case
        /// </summary>
        /// <returns></returns>
        public TestCase CurrentTestCase()
        {
            TestCase retVal = null;

            SubStep subStep = CurrentSubStep();
            if (subStep != null)
            {
                retVal = subStep.Step.TestCase;
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the current test step
        /// </summary>
        /// <returns></returns>
        public Step CurrentStep()
        {
            Step retVal = null;

            var subStep = CurrentSubStep();
            if (subStep != null)
            {
                retVal = subStep.Step;
            }

            return retVal;
        }

        /// <summary>
        ///     Runs the test case until the step provided is encountered
        ///     This does not execute the corresponding step.
        /// </summary>
        /// <param name="target"></param>
        public void RunUntilStep(Step target)
        {
            SubStep currentSubStep = CurrentSubStep();
            while (currentSubStep != null && currentSubStep.Step != target)
            {
                StepOnce();
                currentSubStep = CurrentSubStep();
            }
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
                StepOnce();
            }
        }

        /// <summary>
        /// Performs a single step
        /// </summary>
        public void StepOnce()
        {
            SubStep subStep = null;

            if (ActiveBlockingExpectations().Count == 0)
            {
                // When no blocking expectation, one can execute the next substep
                subStep = CurrentSubStep();
                if (EventTimeLine.ContainsSubStep(subStep))
                {
                    NextSubStep();
                    subStep = CurrentSubStep();
                }

                SetupSubStep(subStep);
            }

            if (subStep == null)
            {
                Cycle();
            }
            else if (!subStep.getSkipEngine())
            {
                Cycle();
            }
            else
            {
                CheckExpectationsState(acceptor.RulePriority.aCleanUp);
                NextCycle();
            }
        }

        /// <summary>
        ///     Steps one step backward in this run
        /// </summary>
        public void StepBack()
        {
            CacheImpact = new CacheImpact();
            EventTimeLine.StepBack();
            SynchronizeCompletedSubStepWithTimeLine();
            ClearCaches();
        }

        /// <summary>
        /// Synchronizes the set of completed substep according to the time line
        /// </summary>
        private void SynchronizeCompletedSubStepWithTimeLine()
        {
            List<SubStep> toRemove = new List<SubStep>();
            foreach (SubStep subStep in CompletedSubStep)
            {
                if (!EventTimeLine.ContainsSubStep(subStep))
                {
                    toRemove.Add(subStep);
                }
            }

            foreach (SubStep subStep in toRemove)
            {
                CompletedSubStep.Remove(subStep);
            }
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
                            EventTimeLine.AddModelEvent(modelInterpretationFailure, true);
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