using System.Collections;
using System.Collections.Generic;
using DataDictionary.Constants;
using DataDictionary.Rules;
using DataDictionary.Types;

namespace DataDictionary.src
{
    public class UnifiedStateMachine : StateMachine
    {
        /// <summary>
        ///     The state machines taht have been combined to make the
        /// </summary>
        public List<StateMachine> MergedStateMachines;
        
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="stateMachine">The state machine that is unified wit hits update</param>
        public UnifiedStateMachine(StateMachine stateMachine)
        {
            Name = stateMachine.Name;

            KeepAllUpdates(stateMachine);
            ApplyUpdates();
        }

        /// <summary>
        ///     Constructor for merging
        /// </summary>
        /// <param name="baseStateMachine"></param>
        /// <param name="updateStateMachine"></param>
        public UnifiedStateMachine(StateMachine baseStateMachine, StateMachine updateStateMachine)
        {
            Name = updateStateMachine.Name;

            MergedStateMachines = new List<StateMachine>();
            MergedStateMachines.Add(baseStateMachine);
            MergedStateMachines.Add(updateStateMachine);
            
            ApplyUpdates();
        }

        public override string FullName
        {
            get { return MergedStateMachines[0].FullName; }
        }

        public override object Enclosing
        {
            get
            {
                return MergedStateMachines[MergedStateMachines.Count - 1].Enclosing;
            }
        }

        /// <summary>
        ///     Adds all state machines in the update chain to MergedStateMachines
        /// </summary>
        /// <param name="stateMachine"></param>
        private void KeepAllUpdates(StateMachine stateMachine)
        {
            MergedStateMachines = new List<StateMachine>();

            // Find the base state machine
            StateMachine current = stateMachine;
            StateMachine next = current.Updates as StateMachine;
            while (next != null)
            {
                current = next;
                next = current.Updates as StateMachine;
            }

            // current is now the state machine at the start of the update chain
            while (current != null)
            {
                MergedStateMachines.Add(current);
                if (current.UpdatedBy.Count == 1)
                {
                    current = current.UpdatedBy[0] as StateMachine;
                }
                else
                {
                    current = null;
                }
            }
        }

        /// <summary>
        ///     Updates this unified state machine to combine all the states and rules of the merged state machines
        /// </summary>
        private void ApplyUpdates()
        {
            foreach (StateMachine stateMachine in MergedStateMachines)
            {
                CombineWithUpdate(stateMachine);
            }

            // Indicates to all the merged state machines that this is their Unified State Machine
            foreach (StateMachine stateMachine in MergedStateMachines)
            {
                stateMachine.ResetUnifiedStateMachine(this);
            }
        }

        /// <summary>
        ///     Applies the effect of an update
        /// </summary>
        /// <param name="stateMachine">The state machine update that is being applied</param>
        private void CombineWithUpdate(StateMachine stateMachine)
        {
            foreach (State state in stateMachine.States)
            {
                ApplyElementUpdate(state, States);
            }

            foreach (Rule rule in stateMachine.Rules)
            {
                ApplyElementUpdate(rule, Rules);
            }
        }

        /// <summary>
        ///     Updates the unified state machine to include the element update.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="collection"></param>
        private void ApplyElementUpdate(ModelElement element, ArrayList collection)
        {
            // If the element was updated and not removed, replace it in the StateMachine
            if (!element.IsRemoved && element.UpdatedBy.Count == 0)
            {
                AddModelElement(element.Duplicate());
            }
        }
    }
}