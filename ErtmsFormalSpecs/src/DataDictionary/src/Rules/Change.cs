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

using System.Collections.Generic;
using DataDictionary.Tests.Runner;
using DataDictionary.Values;
using DataDictionary.Variables;

namespace DataDictionary.Rules
{
    /// <summary>
    ///     Records a change event
    /// </summary>
    public class Change
    {
        /// <summary>
        ///     Indicates whether the change has already been applied
        /// </summary>
        public bool Applied { get; private set; }

        /// <summary>
        ///     The variable affected by the change
        /// </summary>
        public IVariable Variable { get; private set; }

        /// <summary>
        ///     The value the variable had before the change
        /// </summary>
        public IValue PreviousValue { get; protected set; }

        /// <summary>
        ///     The new value affected by the change
        /// </summary>
        public IValue NewValue { get; protected set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="previousValue"></param>
        /// <param name="newValue"></param>
        public Change(IVariable variable, IValue previousValue, IValue newValue)
        {
            Variable = variable;
            PreviousValue = previousValue;
            NewValue = newValue;
            Applied = false;
        }

        /// <summary>
        ///     Applies the change if it has not yet been applied
        /// </summary>
        /// <param name="runner"></param>
        public virtual void Apply(Runner runner)
        {
            if (!Applied)
            {
                ChangeVariableValue(runner, NewValue);
                Applied = true;
            }
        }

        /// <summary>
        ///     Rolls back the change
        /// </summary>
        /// <param name="runner"></param>
        public void RollBack(Runner runner)
        {
            if (Applied)
            {
                ChangeVariableValue(runner, PreviousValue);
                Applied = false;
            }
        }

        /// <summary>
        ///     Changes the value of the corresponding variable
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="value"></param>
        private void ChangeVariableValue(Runner runner, IValue value)
        {
            if (!Variable.Type.CompareForEquality(Variable.Value, value))
            {
                // Only update the variable when the value changes
                Variable.Value = value;
                Variable.HandleChange(runner.CacheImpact);
            }
        }

        /// <summary>
        ///     Indicates whether this change impacts the selected variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool ImpactVariable(IVariable variable)
        {
            bool retVal = false;

            IVariable current = Variable;
            while (!retVal && current != null)
            {
                retVal = current == variable;

                StructureValue enclosingStructureValue = current.Enclosing as StructureValue;
                if (enclosingStructureValue != null)
                {
                    current = enclosingStructureValue.Enclosing as IVariable;
                }
                else
                {
                    current = null;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Checks that the change is valid
        /// </summary>
        /// <param name="element"></param>
        public virtual bool CheckChange(ModelElement element)
        {
            bool retVal = true;

            if (Variable == null)
            {
                element.AddError("Cannot determine variable to be updated");
                retVal = false;
            }

            if (NewValue == null)
            {
                element.AddError(Variable.FullName + " <- <cannot evaluate value>");
                retVal = false;
            }

            return retVal;
        }
    }

    /// <summary>
    ///     Holds a list of changes
    /// </summary>
    public class ChangeList
    {
        /// <summary>
        ///     The changes stored in this change list
        /// </summary>
        public List<Change> Changes { get; set; }

        /// <summary>
        ///     Consdtructor
        /// </summary>
        public ChangeList()
        {
            Changes = new List<Change>();
        }

        /// <summary>
        ///     Adds a change to the list of changes
        /// </summary>
        /// <param name="change">The change to add</param>
        /// <param name="apply">Indicates whether the change should be applied immediately</param>
        /// <param name="runner"></param>
        public void Add(Change change, bool apply, Runner runner)
        {
            Changes.Add(change);
            if (apply)
            {
                // BUG: This is the case for procedure calls.
                // In this case, computing the next changes induced by the procedure must be based on this changes.
                // However, this contradicts a invariant : the state of the system does not change as long as all changes have not been computed
                // To fix this, changes should be unapplied at the end of the procedure call change evaluation to be applied back 
                // during the activation application.
                change.Apply(runner);
                runner.CacheImpact.ClearCaches();
            }
        }

        /// <summary>
        ///     Ensures that all changes are correct
        /// </summary>
        /// <param name="element"></param>
        public void CheckChanges(ModelElement element)
        {
            List<Change> invalidChanges = new List<Change>();
            foreach (Change change in Changes)
            {                
                if ( !change.CheckChange(element) )
                {
                    invalidChanges.Add(change);
                }
            }

            foreach (Change change in invalidChanges)
            {
                Changes.Remove(change);                
            }
        }

        /// <summary>
        ///     Apply all changes
        /// </summary>
        /// <param name="runner"></param>
        public void Apply(Runner runner)
        {
            foreach (Change change in Changes)
            {
                change.Apply(runner);
            }
        }

        /// <summary>
        ///     Roll back all changes in the list
        /// </summary>
        /// <param name="runner"></param>
        public void RollBack(Runner runner)
        {
            for (int i = Changes.Count - 1; i >= 0; i--)
            {
                Changes[i].RollBack(runner);
            }
        }

        /// <summary>
        ///     Indicates that the change list modifies the variable provided as parameter
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool ImpactVariable(IVariable variable)
        {
            bool retVal = false;

            foreach (Change change in Changes)
            {
                retVal = change.ImpactVariable(variable);

                if (retVal)
                {
                    break;
                }
            }

            return retVal;
        }

        public override string ToString()
        {
            string retVal = "";

            foreach (Change change in Changes)
            {
                if (!string.IsNullOrEmpty(retVal))
                {
                    retVal += "\n";
                }
                retVal += change.Variable.FullName + " <- " + change.NewValue.LiteralName;
            }

            return retVal;
        }
    }
}