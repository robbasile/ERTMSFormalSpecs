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

using System.Threading;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Tests.Runner;
using DataDictionary.Variables;

namespace DataDictionary.Rules
{
    /// <summary>
    /// Handles the insertion of a new element in a list. 
    /// This is done differently from the other changes to allows several insertions in the same list to occur in the same processing cycle.
    /// </summary>
    public class InsertInListChange : Change
    {
        /// <summary>
        /// The interpretation context to apply the change
        /// </summary>
        InterpretationContext Context { get; set; }

        /// <summary>
        /// The related insert statement
        /// </summary>
        InsertStatement Statement { get; set; }

        /// <summary>
        /// The explanation part to be filled when actually computing the change
        /// </summary>
        ExplanationPart Explanation { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statement"></param>
        /// <param name="variable"></param>
        /// <param name="explanation"></param>
        public InsertInListChange(InterpretationContext context, InsertStatement statement, IVariable variable, ExplanationPart explanation)
            : base(variable, null, null)
        {
            Context = context;
            Statement = statement;
            Explanation = explanation;
        }

        /// <summary>
        /// Applies the change
        /// </summary>
        /// <param name="runner"></param>
        public override void Apply(Runner runner)
        {
            if (! Applied)
            {
                // Compute the new value when applying the change (instead of when creating the change)
                Change change = Statement.GetChange(Context, Variable, Explanation, true, runner);
                PreviousValue = change.PreviousValue;
                NewValue = change.NewValue;

                base.Apply(runner);                
            }
        }

        /// <summary>
        /// Checks that the change is valid
        /// </summary>
        /// <param name="element"></param>
        public override bool CheckChange(ModelElement element)
        {
            bool retVal = true;

            if (Variable == null)
            {
                element.AddError("Cannot determine variable to be updated");
                retVal = false;
            }

            return retVal;
        }
    }
}
