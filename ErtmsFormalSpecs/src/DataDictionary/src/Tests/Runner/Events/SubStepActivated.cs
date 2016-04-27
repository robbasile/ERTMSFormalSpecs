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
using System.Collections.Generic;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using Action = DataDictionary.Rules.Action;
using NameSpace = DataDictionary.Types.NameSpace;

namespace DataDictionary.Tests.Runner.Events
{
    public class SubStepActivated : ModelEvent
    {
        /// <summary>
        ///     The activated step
        /// </summary>
        public SubStep SubStep { get; private set; }

        ///     <summary>
        ///         The namespace associated to this event
        ///     </summary>
        public override NameSpace NameSpace
        {
            get { return null; }
        }

        /// <summary>
        ///     The list of changes related to this sub step
        /// </summary>
        public List<VariableUpdate> Updates { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subStep">The activated step</param>
        /// <param name="priority"></param>
        public SubStepActivated(SubStep subStep, acceptor.RulePriority? priority)
            : base(subStep.Name, subStep, priority)
        {
            SubStep = subStep;
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
                // Computes the list of variable updates
                Updates = new List<VariableUpdate>();
                foreach (Action action in SubStep.Actions)
                {
                    if (action.Statement != null)
                    {
                        VariableUpdate update = new VariableUpdate(action, SubStep.Dictionary, null);
                        update.ComputeChanges(false, runner);
                        Updates.Add(update);
                    }
                    else
                    {
                        action.AddError("Cannot parse action statement");
                    }
                }
                // TODO : Determine if we want to do this
                // runner.HandleEnterAndLeaveStateActions(null, Updates);
                runner.CheckUpdatesCompatibility(Updates);
            }

            return retVal;
        }

        /// <summary>
        ///     Applies this step activation be registering it in the activation cache
        /// </summary>
        /// <param name="runner"></param>
        public override void Apply(Runner runner)
        {
            base.Apply(runner);

            TimeLine.SubStepActivationCache[SubStep] = this;
            foreach (VariableUpdate update in Updates)
            {
                TimeLine.AddModelEvent(update, true);
            }

            // Store the step corresponding expectations
            foreach (Expectation expectation in SubStep.Expectations)
            {
                bool addExpectation = true;

                if (expectation.getKind() == acceptor.ExpectationKind.aInstantaneous)
                {
                    if (!String.IsNullOrEmpty(expectation.getCondition()))
                    {
                        Expression expression = new Parser().Expression(expectation,
                            expectation.getCondition());
                        BoolValue value =
                            expression.GetExpressionValue(new InterpretationContext(expectation), null) as BoolValue;
                        if (value != null)
                        {
                            addExpectation = value.Val;
                        }
                        else
                        {
                            throw new Exception("Cannot evaluate " + expectation.getCondition() + " as a boolean value");
                        }
                    }
                }

                if (addExpectation)
                {
                    TimeLine.AddModelEvent(new Expect(expectation, runner.CurrentPriority), true);
                }
            }
        }

        /// <summary>
        ///     Rolls back this step be removing it from the cache
        /// </summary>
        /// <param name="runner"></param>
        public override void RollBack(Runner runner)
        {
            TimeLine.SubStepActivationCache.Remove(SubStep);

            base.RollBack(runner);
        }
    }
}