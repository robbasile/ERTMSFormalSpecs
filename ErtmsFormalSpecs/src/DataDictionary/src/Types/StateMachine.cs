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
using DataDictionary.Interpreter.Statement;
using DataDictionary.Rules;
using DataDictionary.Values;
using Utils;
using Action = DataDictionary.Rules.Action;
using Function = DataDictionary.Functions.Function;
using PreCondition = DataDictionary.Rules.PreCondition;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Generated.RuleCondition;
using State = DataDictionary.Constants.State;
using Visitor = DataDictionary.Generated.Visitor;

namespace DataDictionary.Types
{
    public class StateMachine : Generated.StateMachine, IEnumerateValues, ISubDeclarator, IFinder
    {
        public override string FullName
        {
            get
            {
                string retVal = "";

                StateMachine current = this;
                while (current.EnclosingStateMachine != null)
                {
                    if (string.IsNullOrEmpty(retVal))
                    {
                        retVal = current.EnclosingState.Name;
                    }
                    else
                    {
                        retVal = current.EnclosingState.Name + "." + retVal;
                    }
                    current = current.EnclosingStateMachine;
                }

                // Current.EnclosingStateMachine is null
                if (string.IsNullOrEmpty(retVal))
                {
                    retVal = ((INamable) current.Enclosing).FullName + "." + current.Name;
                }
                else
                {
                    retVal = ((INamable) current.Enclosing).FullName + "." + current.Name + "." + retVal;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates if this StateMachine contains implemented sub-elements
        /// </summary>
        public override bool ImplementationPartiallyCompleted
        {
            get
            {
                if (getImplemented())
                {
                    return true;
                }

                foreach (Rule rule in Rules)
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
        ///     Constructor
        /// </summary>
        public StateMachine()
        {
            FinderRepository.INSTANCE.Register(this);
        }

        /// <summary>
        ///     The states
        /// </summary>
        public ArrayList States
        {
            get
            {
                if (allStates() == null)
                {
                    setAllStates(new ArrayList());
                }
                return allStates();
            }
        }

        /// <summary>
        ///     Gets the state which name corresponds to the image provided
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override IValue getValue(string image)
        {
            IValue retVal = null;

            foreach (State state in States)
            {
                if (state.Name.CompareTo(image) == 0)
                {
                    retVal = state;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The rules
        /// </summary>
        public ArrayList Rules
        {
            get
            {
                if (allRules() == null)
                {
                    setAllRules(new ArrayList());
                }
                return allRules();
            }
        }

        /// <summary>
        ///     Provides the rule which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Rule FindRule(string name)
        {
            return (Rule) INamableUtils.findByName(name, Rules);
        }

        /// <summary>
        ///     Provides the values whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        public IValue findValue(string[] names, int index)
        {
            State retVal = null;

            if (index < names.Length)
            {
                retVal = (State) INamableUtils.findByName(names[index], States);
                ;

                if (retVal != null && index < names.Length - 1)
                {
                    StateMachine stateMachine = retVal.StateMachine;
                    if (stateMachine != null)
                    {
                        retVal = (State) stateMachine.findValue(names, index + 1);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the state which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public State FindState(string name)
        {
            State retVal = (State) findValue(name.Split('.'), 0);

            if (retVal == null)
            {
                Log.Error("Cannot find state " + name);
            }

            return retVal;
        }

        public State EnclosingState
        {
            get { return Enclosing as State; }
        }

        public NameSpace EnclosingNameSpace
        {
            get { return Enclosing as NameSpace; }
        }

        public override void Delete()
        {
            if (EnclosingState != null)
            {
                EnclosingState.StateMachine = null;
            }
            else if (EnclosingNameSpace != null)
            {
                EnclosingNameSpace.StateMachines.Remove(this);
            }
            else
            {
                Structure structure = Enclosing as Structure;
                if (structure != null)
                {
                    structure.StateMachines.Remove(this);
                }
            }
        }

        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = base.EnclosingCollection;

                if (EnclosingNameSpace != null)
                {
                    retVal = EnclosingNameSpace.Structures;
                }
                else
                {
                    Structure structure = Enclosing as Structure;
                    if (structure != null)
                    {
                        retVal = structure.StateMachines;
                    }
                }

                return retVal;
            }
        }

        public void Constants(string scope, Dictionary<string, object> retVal)
        {
            foreach (State state in this.States)
            {
                state.Constants(scope, retVal);
            }
        }

        public void ClearCache()
        {
            cachedValues = null;
            DeclaredElements = null;
        }

        /// <summary>
        ///     Provides the set of states available in this state machine
        /// </summary>
        public List<IValue> cachedValues;

        public List<IValue> AllValues
        {
            get
            {
                if (cachedValues == null)
                {
                    cachedValues = new List<IValue>();

                    foreach (State state in StateFinder.INSTANCE.find(this))
                    {
                        cachedValues.Add(state);
                    }
                }

                return cachedValues;
            }
        }

        public override bool Contains(IValue first, IValue other)
        {
            bool retVal = false;

            State state1 = first as State;
            State state2 = other as State;
            if (state1 != null && state2 != null)
            {
                if (state1.Type == state2.Type)
                {
                    State current = state2;
                    while (current != null & retVal == false)
                    {
                        retVal = (current == state1);
                        current = current.EnclosingState;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            foreach (State state in States)
            {
                ISubDeclaratorUtils.AppendNamable(this, state);
            }
        }

        /// <summary>
        ///     Provides all the states that can be stored in this state machine
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; set; }

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
        ///     Provides the states used in an expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static List<State> GetStates(Expression expression)
        {
            List<State> retval = new List<State>();

            if (expression != null)
            {
                foreach (IValue value in expression.GetLiterals())
                {
                    State state = value as State;
                    if (state != null)
                    {
                        retval.Add(state);
                    }
                }

                Call call = expression as Call;
                if (call != null)
                {
                    Function function = call.Called.getStaticCallable() as Function;
                    if (function != null)
                    {
                        foreach (IValue value in function.GetLiterals())
                        {
                            State state = value as State;
                            if (state != null)
                            {
                                retval.Add(state);
                            }
                        }
                    }
                }
            }

            return retval;
        }

        /// <summary>
        ///     This class is used to find all transitions in the model
        /// </summary>
        private class TransitionFinder : Visitor
        {
            /// <summary>
            ///     The transitions currently found
            /// </summary>
            private List<Transition> transitions = new List<Transition>();

            public List<Transition> Transitions
            {
                get { return transitions; }
            }

            /// <summary>
            ///     The state machine for which this transition creator has been created
            /// </summary>
            private StateMachine stateMachine;

            public StateMachine StateMachine
            {
                get { return stateMachine; }
                private set { stateMachine = value; }
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="stateMachine"></param>
            public TransitionFinder(StateMachine stateMachine)
            {
                FinderRepository.INSTANCE.ClearCache();
                Transitions.Clear();
                StateMachine = stateMachine;
                State initialState = StateMachine.DefaultValue as State;
                if (initialState != null)
                {
                    Transitions.Add(new Transition(null, null, null, initialState));
                }
            }

            /// <summary>
            ///     Check if this rule corresponds to a transition for this state machine
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(RuleCondition obj, bool visitSubNodes)
            {
                Rules.RuleCondition ruleCondition = (Rules.RuleCondition) obj;

                foreach (Action action in ruleCondition.Actions)
                {
                    try
                    {
                        foreach (VariableUpdateStatement update in action.UpdateStatements)
                        {
                            Type targetType = update.TargetType;
                            if (targetType is StateMachine)
                            {
                                Expression expressionTree = update.Expression;
                                if (expressionTree != null)
                                {
                                    // HaCK: This is a bit rough, but should be sufficient for now...
                                    foreach (State stt1 in GetStates(expressionTree))
                                    {
                                        // TargetState is the target state either in this state machine or in a sub state machine
                                        State targetState = StateMachine.StateInThisStateMachine(stt1);

                                        int transitionCount = Transitions.Count;
                                        bool filteredOut = false;

                                        // Finds the enclosing state of this action to determine the source state of this transition
                                        State enclosingState = EnclosingFinder<State>.find(action);
                                        if (enclosingState != null)
                                        {
                                            filteredOut = filteredOut ||
                                                          AddTransition(update, stt1, null, enclosingState);
                                        }

                                        if (!filteredOut)
                                        {
                                            foreach (PreCondition preCondition in ruleCondition.AllPreConditions)
                                            {
                                                // A transition from one state to another has been found
                                                foreach (State stt2 in GetStates(preCondition.Expression))
                                                {
                                                    filteredOut = filteredOut ||
                                                                  AddTransition(update, stt1, preCondition, stt2);
                                                }
                                            }
                                        }

                                        if (Transitions.Count == transitionCount)
                                        {
                                            if (targetState == stt1 && targetState.EnclosingStateMachine == StateMachine)
                                            {
                                                if (!filteredOut)
                                                {
                                                    Action enclosingAction = update.Root as Action;

                                                    if (enclosingAction != null)
                                                    {
                                                        // No precondition could be found => one can reach this state at anytime
                                                        if (
                                                            !findMatchingTransition(enclosingAction.RuleCondition, null,
                                                                targetState))
                                                        {
                                                            Transitions.Add(new Transition(null, null, update,
                                                                targetState));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    action.AddError("Cannot parse expression");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }

                base.visit(obj, visitSubNodes);
            }

            /// <summary>
            ///     Adds a transition in the transitions sets
            /// </summary>
            /// <param name="update">The update state which provides the target of the transition</param>
            /// <param name="target">The target state, as determined by the update statement</param>
            /// <param name="filteredOut"></param>
            /// <param name="preCondition">the precondition (if any) which is used to determine the initial state</param>
            /// <param name="initial">The initial state</param>
            /// <returns>
            ///     true if the transition has been filtered out. A transition can be filtered out if the target state is equal to the
            ///     initial state or the initial state is null
            /// </returns>
            private bool AddTransition(VariableUpdateStatement update, State target, PreCondition preCondition,
                State initial)
            {
                bool retVal = false;

                if (SameParentStateMachine(initial, target))
                {
                    State initialState = StateMachine.StateInThisStateMachine(initial);
                    // TargetState is the target state either in this state machine or in a sub state machine
                    State targetState = StateMachine.StateInThisStateMachine(target);

                    // Determine the rule condition (if any) related to this state machine
                    Rules.RuleCondition condition = null;
                    if (update != null)
                    {
                        Action action = update.Root as Action;
                        condition = action.RuleCondition;
                    }

                    if (targetState != null || initialState != null)
                    {
                        // This transition is about this state machine.
                        if (initialState != targetState && initialState != null)
                        {
                            // Check that the transition is not yet present
                            // This case can occur when the same RuleCondition references two different states 
                            // in a substate machine. Only draws the transition once.
                            if (!findMatchingTransition(condition, initialState, targetState))
                            {
                                Transitions.Add(new Transition(preCondition, initialState, update, targetState));
                            }
                        }
                        else
                        {
                            if (initialState == initial)
                            {
                                retVal = true;
                            }
                        }
                    }
                }

                return retVal;
            }

            /// <summary>
            ///     Indicates that the two states have the same outermost state machine
            /// </summary>
            /// <param name="state1"></param>
            /// <param name="state2"></param>
            /// <returns></returns>
            private bool SameParentStateMachine(State state1, State state2)
            {
                return GetParentStateMachine(state1) == GetParentStateMachine(state2);
            }

            /// <summary>
            ///     Provides the outermost state machine enclosing the state provided
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            private StateMachine GetParentStateMachine(State state)
            {
                StateMachine retVal = state.EnclosingStateMachine;

                while (retVal.EnclosingStateMachine != null)
                {
                    retVal = retVal.EnclosingStateMachine;
                }

                return retVal;
            }

            /// <summary>
            ///     Finds a transition which matches the initial state, target state and rule condition in the existing transitions
            /// </summary>
            /// <param name="condition"></param>
            /// <param name="initialState"></param>
            /// <param name="targetState"></param>
            /// <returns></returns>
            private bool findMatchingTransition(Rules.RuleCondition condition, State initialState, State targetState)
            {
                bool retVal = false;

                foreach (Transition t in Transitions)
                {
                    if (t.RuleCondition == condition && t.Source == initialState && t.Target == targetState)
                    {
                        retVal = true;
                        break;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the transitions associated to this state machine, based on the underlying rules
        /// </summary>
        public List<Transition> Transitions
        {
            get
            {
                TransitionFinder finder = new TransitionFinder(this);
                foreach (Dictionary dictionary in EFSSystem.Dictionaries)
                {
                    finder.visit(dictionary);
                }
                return finder.Transitions;
            }
        }

        /// <summary>
        ///     Indicates that the state machine contains (either directly or indirectly) the state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        internal State StateInThisStateMachine(State state)
        {
            State retVal = null;

            foreach (State other in States)
            {
                if (other == state)
                {
                    retVal = state;
                    break;
                }

                retVal = other.StateMachine.StateInThisStateMachine(state);
                if (retVal != null)
                {
                    retVal = other;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the enclosing state machine, if any
        /// </summary>
        public StateMachine EnclosingStateMachine
        {
            get
            {
                StateMachine retVal = null;

                if (EnclosingState != null)
                {
                    retVal = EnclosingState.EnclosingStateMachine;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates that the other type can be placed in variables of this type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            StateMachine current = otherType as StateMachine;
            while (current != null && !retVal)
            {
                retVal = this == current;
                current = current.EnclosingStateMachine;
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                State item = element as State;
                if (item != null)
                {
                    appendStates(item);
                }
            }
            {
                Rule item = element as Rule;
                if (item != null)
                {
                    appendRules(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        ///     Instanciates this state machine for the instanciation of a StructureProcedure into a Procedure
        /// </summary>
        /// <returns></returns>
        public StateMachine instanciate()
        {
            StateMachine retVal = (StateMachine) acceptor.getFactory().createStateMachine();
            retVal.Name = Name;
            retVal.setFather(getFather());
            retVal.Default = Default;
            foreach (State state in States)
            {
                State newState = state.duplicate();
                retVal.appendStates(newState);
            }
            foreach (Rule rule in Rules)
            {
                Rule newRule = rule.duplicate();
                retVal.appendRules(newRule);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a copy of the state machine in the designated dictionary. The namespace structure is copied over.
        ///     The new state machine is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public StateMachine CreateStateMachineUpdate(Dictionary dictionary)
        {
            StateMachine retVal = (StateMachine) Duplicate();
            retVal.SetUpdateInformation(this);

            String[] names = FullName.Split('.');
            names = names.Take(names.Count() - 1).ToArray();

            if (Enclosing is NameSpace)
            {
                NameSpace nameSpace = dictionary.GetNameSpaceUpdate(names, Dictionary);
                nameSpace.appendStateMachines(retVal);
            }
            else
            {
                String[] nameSpaceRef = names.Take(names.Count() - 1).ToArray();

                if (Enclosing is Structure)
                {
                    NameSpace nameSpace = dictionary.GetNameSpaceUpdate(nameSpaceRef, Dictionary);
                    Structure structure = nameSpace.GetStructureUpdate(names.Last(), (NameSpace)nameSpace.Updates);
                    structure.appendStateMachines(retVal);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Sets the update information for this state machine (this state machine updates source)
        /// </summary>
        /// <param name="source"></param>
        public override void SetUpdateInformation(ModelElement source)
        {
            base.SetUpdateInformation(source);
            StateMachine sourceStateMachine = (StateMachine) source;

            foreach (State state in States)
            {
                State baseState = sourceStateMachine.FindState(state.Name);
                if (baseState != null)
                {
                    state.SetUpdateInformation(baseState);
                }
            }

            foreach (Rule rule in Rules)
            {
                Rule baseRule = sourceStateMachine.FindRule(rule.Name);
                if (baseRule != null)
                {
                    rule.SetUpdateInformation(baseRule);
                }
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            base.GetExplain(explanation, explainSubElements);
            explanation.Write("STATE MACHINE ");
            explanation.WriteLine(Name);
            explanation.Indent(2, () =>
            {
                foreach (State state in States)
                {
                    state.GetExplain(explanation, false);
                }
                foreach (Rule rule in Rules)
                {
                    rule.GetExplain(explanation, explainSubElements);
                }
            });
        }
    }
}