using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            Enclosing = MergedStateMachines[0].Enclosing;

            ApplyUpdates();
        }

        public override string FullName
        {
            get { return MergedStateMachines[0].FullName; }
        }

        /// <summary>
        ///     Adds all state machines in the update chain to MergedStateMachines
        /// </summary>
        /// <param name="stateMachine"></param>
        private void KeepAllUpdates(StateMachine stateMachine)
        {
            MergedStateMachines = new List<StateMachine>();

            // Find the base structure
            StateMachine current = stateMachine;
            StateMachine next = current.Updates as StateMachine;
            while (next != null)
            {
                current = next;
                next = current.Updates as StateMachine;
            }

            // current is now the structure at the start of the update chain
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
            foreach (StateMachine merged in MergedStateMachines)
            {
                CombineWithUpdate(merged);
            }

            // Indicates to all the merged structure that this is their Unified Structure
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
            // Remove the redefined structure element
            ModelElement baseElement = element.Updates;
            if (baseElement != null)
            {
                ArrayList temp = new ArrayList();
                foreach (ModelElement elem in collection)
                {
                    if (elem.Name != baseElement.Name)
                    {
                        temp.Add(elem);
                    }
                }
                collection = temp;
            }

            // If the element was updated and not removed, replace it in the baseStructure
            if (!element.IsRemoved)
            {
                AddModelElement(element);
            }
        }
    }
}
