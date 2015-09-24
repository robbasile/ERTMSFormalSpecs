using System.Collections;
using System.Collections.Generic;
using DataDictionary.Functions;
using DataDictionary.Rules;
using DataDictionary.Types;
using Utils;

namespace DataDictionary.src
{
    public class UnifiedStructure : Structure
    {
        public List<Structure> MergedStructures;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="structure">The structure that we are trying to find the unified structure for</param>
        public UnifiedStructure(Structure structure)
        {
            Name = structure.Name;

            KeepAllUpdates(structure);

            ApplyUpdates();
        }

        /// <summary>
        ///     Constructor for merging
        /// </summary>
        /// <param name="baseStateMachine"></param>
        /// <param name="updateStateMachine"></param>
        public UnifiedStructure(Structure baseStateMachine, Structure updateStateMachine)
        {
            Name = updateStateMachine.Name;

            MergedStructures = new List<Structure>();
            MergedStructures.Add(baseStateMachine);
            MergedStructures.Add(updateStateMachine);

            ApplyUpdates();
        }

        public override string FullName
        {
            get { return MergedStructures[MergedStructures.Count - 1].FullName; }
        }

        public override object Enclosing
        {
            get
            {
                return MergedStructures[MergedStructures.Count - 1].Enclosing;
            }
        }

        /// <summary>
        ///     Adds all the structures that are updated (directly or indirectly) to the list of merged structures,
        ///     then adds all structures updating it, as long as there is only one update per structure
        /// </summary>
        /// <param name="structure"></param>
        private void KeepAllUpdates(Structure structure)
        {
            MergedStructures = new List<Structure>();

            // Find the base structure
            Structure current = structure;
            Structure next = current.Updates as Structure;
            while (next != null)
            {
                current = next;
                next = current.Updates as Structure;
            }

            // current is now the structure at the start of the update chain
            while (current != null)
            {
                MergedStructures.Add(current);
                if (current.UpdatedBy.Count == 1)
                {
                    current = current.UpdatedBy[0] as Structure;
                }
                else
                {
                    current = null;
                }
            }
        }

        /// <summary>
        ///     Updates this unified structure to become the merge of all the structures in MergedStructures
        /// </summary>
        private void ApplyUpdates()
        {
            foreach (Structure merged in MergedStructures)
            {
                CombineWithUpdate(merged);
            }

            // Indicates to all the merged structure that this is their Unified Structure
            foreach (Structure structure in MergedStructures)
            {
                structure.ResetUnifiedStructure(this);
            }
        }

        /// <summary>
        ///     Applies the effect of an update
        /// </summary>
        /// <param name="updateStructure">The updating structure</param>
        private void CombineWithUpdate(Structure updateStructure)
        {
            foreach (StructureElement element in updateStructure.Elements)
            {
                ApplyElementUpdate(element, Elements);
            }

            foreach (Procedure procedure in updateStructure.Procedures)
            {
                ApplyElementUpdate(procedure, Procedures);
            }

            foreach (StateMachine stateMachine in updateStructure.StateMachines)
            {
                ApplyElementUpdate(stateMachine, StateMachines);
            }

            foreach (Rule rule in updateStructure.Rules)
            {
                ApplyElementUpdate(rule, Rules);
            }
        }

        /// <summary>
        ///     Applies the effect of an element update
        /// </summary>
        /// <param name="updateElement">The updated version of the structure element</param>
        /// <param name="collection">The collection in the structure that will hold the updated element</param>
        private void ApplyElementUpdate(ModelElement updateElement, ArrayList collection)
        {
            // If the element was not updated and is not removed
            if (!updateElement.IsRemoved && updateElement.UpdatedBy.Count == 0)
            {
                AddModelElement(updateElement);
            }
        }
    }
}