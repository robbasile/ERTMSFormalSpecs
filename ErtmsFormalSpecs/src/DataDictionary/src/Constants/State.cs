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

using System.Collections;
using System.Collections.Generic;
using DataDictionary.Generated;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using NameSpace = DataDictionary.Types.NameSpace;
using Rule = DataDictionary.Rules.Rule;
using StateMachine = DataDictionary.Types.StateMachine;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Constants
{
    public class State : Generated.State, IValue, ISubDeclarator, IGraphicalDisplay
    {
        public string LiteralName
        {
            get
            {
                string retVal = Name;

                State stt = EnclosingState;
                while (stt != null)
                {
                    retVal = stt.Name + "." + retVal;
                    stt = stt.EnclosingState;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Adds a new element log attached to this model element
        /// </summary>
        /// <param name="log"></param>
        public override void AddElementLog(ElementLog log)
        {
            bool add = true;

            foreach (ElementLog other in base.Messages)
            {
                if (other.CompareTo(log) == 0)
                {
                    add = false;
                }
            }

            if (add)
            {
                base.Messages.Add(log);
            }
        }

        /// <summary>
        ///     State machines are not displayed in the tree view.
        /// </summary>
        public override List<ElementLog> Messages
        {
            get
            {
                List<ElementLog> retVal = new List<ElementLog>();

                retVal.AddRange(base.Messages);
                if (StateMachine != null)
                {
                    retVal.AddRange(StateMachine.Messages);
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Clears the messages associated to this model element
        /// </summary>
        /// <param name="precise">
        ///     Indicates that the MessagePathInfo should be recomputed precisely
        ///     according to the sub elements and should update the enclosing elements
        /// </param>
        public override void ClearMessages(bool precise)
        {
            base.Messages.Clear();
            UpdateMessageInfoAfterClear(precise);
        }


        /// <summary>
        ///     Provides the full name path to this element in the model structure
        /// </summary>
        public override string FullName
        {
            get
            {
                string retVal = Name;

                State stt = EnclosingState;
                while (stt != null)
                {
                    retVal = stt.Name + "." + retVal;
                    stt = stt.EnclosingState;
                }

                StateMachine parentStateMachine = EnclosingStateMachine;
                while (parentStateMachine.EnclosingStateMachine != null)
                {
                    parentStateMachine = parentStateMachine.EnclosingStateMachine;
                }

                if (parentStateMachine != null)
                {
                    retVal = parentStateMachine.FullName + "." + retVal;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     The states available in this state
        /// </summary>
        public HashSet<State> AllStates
        {
            get { return StateFinder.INSTANCE.find(this); }
        }

        /// <summary>
        ///     Finds a substate from this state
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public State FindSubState(string name)
        {
            return FindSubState(name.Split('.'), 0);
        }

        /// <summary>
        ///     Provides the state whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        public State FindSubState(string[] names, int index)
        {
            State retVal = null;

            foreach (State state in StateMachine.States)
            {
                if (state.Name.CompareTo(names[index]) == 0)
                {
                    retVal = state;
                    if (index < names.Length - 1)
                    {
                        retVal = retVal.FindSubState(names, index + 1);
                    }
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The value type
        /// </summary>
        public Type Type
        {
            get
            {
                StateMachine retVal = StateMachine;

                while (retVal.EnclosingStateMachine != null)
                {
                    retVal = retVal.EnclosingStateMachine;
                }

                return retVal;
            }
            set { }
        }

        /// <summary>
        ///     The sub state machine for this state
        /// </summary>
        public StateMachine StateMachine
        {
            get
            {
                if (getStateMachine() == null)
                {
                    StateMachine stateMachine = (StateMachine) acceptor.getFactory().createStateMachine();
                    stateMachine.setFather(this);
                    setStateMachine(stateMachine);
                }

                return (StateMachine) getStateMachine();
            }
            set { setStateMachine(value); }
        }

        /// <summary>
        ///     The width of the rectangle representing the state
        /// </summary>
        public int Width
        {
            get { return getWidth(); }
            set { setWidth(value); }
        }

        /// <summary>
        ///     The height of the rectangle representing the state
        /// </summary>
        public int Height
        {
            get { return getHeight(); }
            set { setHeight(value); }
        }

        /// <summary>
        ///     The X position of the rectangle representing the state
        /// </summary>
        public int X
        {
            get { return getX(); }
            set { setX(value); }
        }

        /// <summary>
        ///     The Y position of the rectangle representing the state
        /// </summary>
        public int Y
        {
            get { return getY(); }
            set { setY(value); }
        }

        /// <summary>
        ///     The name to be displayed on the graphical view
        /// </summary>
        public string GraphicalName
        {
            get
            {
                string retVal = Name;

                if (StateMachine.States.Count > 0)
                {
                    retVal += "*";
                }
                return retVal;
            }
        }

        /// <summary>
        ///     Indicates whether the state is hidden
        /// </summary>
        public bool Hidden
        {
            get { return false; }
            set { }
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
        ///     The enclosing state machine
        /// </summary>
        public StateMachine EnclosingStateMachine
        {
            get { return Enclosing as StateMachine; }
        }

        /// <summary>
        ///     The enclosing state, if any
        /// </summary>
        public State EnclosingState
        {
            get { return EnclosingStateMachine.getFather() as State; }
        }

        public override ArrayList EnclosingCollection
        {
            get { return EnclosingStateMachine.States; }
        }

        public void Constants(string scope, Dictionary<string, object> retVal)
        {
            string name = Utils.Util.concat(scope, Name);

            retVal[name] = this;
            if (StateMachine != null)
            {
                StateMachine.Constants(name, retVal);
            }
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            StateMachine.InitDeclaredElements();
        }

        /// <summary>
        ///     Provides all the states available through this state
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements
        {
            get { return StateMachine.DeclaredElements; }
        }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            StateMachine.Find(name, retVal);
        }

        /// <summary>
        ///     Creates a valid right side IValue, according to the target variable (left side)
        /// </summary>
        /// <param name="variable">The target variable</param>
        /// <param name="duplicate"></param>
        /// <param name="setEnclosing">Indicates that the new value enclosing element should be set</param>
        /// <returns></returns>
        public virtual IValue RightSide(IVariable variable, bool duplicate, bool setEnclosing)
        {
            State retVal = this;

            while (retVal.StateMachine.AllValues.Count > 0)
            {
                retVal = (State) retVal.StateMachine.DefaultValue;
            }

            return retVal;
        }

        /// <summary>
        ///     The namespace related to the typed element
        /// </summary>
        public NameSpace NameSpace
        {
            get { return null; }
        }

        /// <summary>
        ///     Provides the type name of the element
        /// </summary>
        public string TypeName
        {
            get { return Type.FullName; }
            set { }
        }

        /// <summary>
        ///     Provides the mode of the typed element
        /// </summary>
        public acceptor.VariableModeEnumType Mode
        {
            get { return acceptor.VariableModeEnumType.aInternal; }
        }

        /// <summary>
        ///     Provides the default value of the typed element
        /// </summary>
        public string Default
        {
            get { return FullName; }
            set { }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public override void AddModelElement(IModelElement element)
        {
            StateMachine.AddModelElement(element);
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Write("STATE ");
            explanation.WriteLine(Name);

            if (explainSubElements)
            {
                foreach (Rule rule in StateMachine.Rules)
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    rule.GetExplain(explanation, explainSubElements);
                    explanation.WriteLine();
                }
            }
        }

        /// <summary>
        ///     Duplicates this model element
        /// </summary>
        /// <returns></returns>
        public State duplicate()
        {
            State retVal = (State) acceptor.getFactory().createState();
            retVal.Name = Name;
            retVal.X = X;
            retVal.Y = Y;
            retVal.Width = Width;
            retVal.Height = Height;
            retVal.setFather(getFather());
            retVal.StateMachine = StateMachine.instanciate();
            return retVal;
        }

        /// <summary>
        ///     Converts a structure value to its corresponding structure expression.
        ///     null entries correspond to the default value
        /// </summary>
        /// <returns></returns>
        public string ToExpressionWithDefault()
        {
            return FullName;
        }

        /// <summary>
        ///     Sets the update information for this state (this state updates source)
        /// </summary>
        /// <param name="source"></param>
        public override void SetUpdateInformation(ModelElement source)
        {
            base.SetUpdateInformation(source);
            State sourceState = (State) source;

            if (getStateMachine() != null)
            {
                StateMachine baseSataStateMachine = sourceState.StateMachine;
                if (baseSataStateMachine != null)
                {
                    StateMachine.SetUpdateInformation(baseSataStateMachine);
                }
            }
        }

        /// <summary>
        ///     Ensures that all update information for this state (and sub-states) is removed
        /// </summary>
        public override void RecoverUpdateInformation()
        {
            base.RecoverUpdateInformation();

            if (StateMachine != null)
            {
                StateMachine.RecoverUpdateInformation();
            }
        }

        /// <summary>
        ///     Creates a copy of the state in the designated dictionary. The namespace structure is copied over.
        ///     The new state is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public State CreateStateUpdate(Dictionary dictionary)
        {
            State retVal = (State) acceptor.getFactory().createState();
            retVal.Name = Name;
            retVal.SetUpdateInformation(this);

            // Find the update for the enclosing state machine to add retVal to
            StateMachine enclosingSmUpdate = EnclosingStateMachine.CreateSubStateMachineUpdate(dictionary);

            enclosingSmUpdate.States.Add(retVal);
            retVal.setFather(enclosingSmUpdate);

            // If retVal is the first state added to the enclosing state machine update, it is the initial state
            if (enclosingSmUpdate.States.Count == 1)
            {
                enclosingSmUpdate.Default = retVal.Name;
            }

            return retVal;
        }

        /// <summary>
        ///     Merges this state, combining the state machine with the base
        /// </summary>
        public override void Merge()
        {
            StateMachine.Merge();

            base.Merge();
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static State CreateDefault(ICollection enclosingCollection)
        {
            State retVal = (State) acceptor.getFactory().createState();

            Util.DontNotify(() => { retVal.Name = "State" + GetElementNumber(enclosingCollection); });

            return retVal;
        }
    }
}