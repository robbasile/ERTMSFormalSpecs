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

using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using Utils;
using Action = DataDictionary.Rules.Action;
using NameSpace = DataDictionary.Types.NameSpace;

namespace DataDictionary.Tests.Runner.Events
{
    public class VariableUpdate : ModelEvent
    {
        /// <summary>
        ///     The action to execute
        /// </summary>
        public Action Action { get; private set; }

        /// <summary>
        ///     The namespace associated to this event
        /// </summary>
        public override NameSpace NameSpace
        {
            get { return Action.NameSpace; }
        }

        /// <summary>
        ///     The changes performed by this action execution
        /// </summary>
        public ChangeList Changes { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="action" The action which raised the variable update></param>
        public VariableUpdate(Action action, IModelElement instance, acceptor.RulePriority? priority)
            : base(action.ExpressionText, instance, priority)
        {
            Action = action;
        }

        /// <summary>
        ///     Computes the changes related to this event
        /// </summary>
        /// <param name="apply">Indicates that the changes should be applied directly</param>
        /// <param name="runner"></param>
        public override bool ComputeChanges(bool apply, Runner runner)
        {
            bool retVal = base.ComputeChanges(apply, runner);

            if (retVal)
            {
                if (runner.Explain)
                {
                    Explanation = new ExplanationPart(Action, "Action ", Action);
                }

                InterpretationContext context = new InterpretationContext(Instance);
                Changes = new ChangeList();
                Action.GetChanges(context, Changes, Explanation, apply, runner);
                Changes.CheckChanges(Action);
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the variable change
        /// </summary>
        /// <param name="context">The execution context used to compute the values</param>
        /// <param name="runner"></param>
        public override void Apply(Runner runner)
        {
            base.Apply(runner);
            Changes.Apply(runner);
        }

        /// <summary>
        ///     Rollsback the changes performed during this event
        /// </summary>
        public override void RollBack()
        {
            base.RollBack();
            Changes.RollBack();
        }

        /// <summary>
        ///     The explanation of the element
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public override string getExplain(bool explainSubElements)
        {
            return TextualExplainUtilities.Encapsule(Changes.ToString());
        }
    }
}