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
using DataDictionary.Generated;
using DataDictionary.Variables;
using NameSpace = DataDictionary.Types.NameSpace;
using RuleCondition = DataDictionary.Rules.RuleCondition;

namespace DataDictionary.Tests.Runner.Events
{
    public class RuleFired : ModelEvent
    {
        /// <summary>
        ///     The activation that launched this rule condition
        /// </summary>
        public Runner.Activation Activation { get; private set; }

        /// <summary>
        ///     The rule condition associated to this rule fired event
        /// </summary>
        public RuleCondition RuleCondition
        {
            get { return Activation.RuleCondition; }
        }

        /// <summary>
        ///     The namespace associated to this event
        /// </summary>
        public override NameSpace NameSpace
        {
            get { return RuleCondition.EnclosingRule.NameSpace; }
        }

        /// <summary>
        ///     The variable updates triggered by this rule event
        /// </summary>
        private List<VariableUpdate> Updates { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        public RuleFired(Runner.Activation activation, acceptor.RulePriority priority)
            : base(activation.RuleCondition.Name, activation.RuleCondition, priority)
        {
            Activation = activation;
            Explanation = Activation.Explanation;
            Updates = new List<VariableUpdate>();
        }

        /// <summary>
        ///     Adds a new variable update triggered by this rule fired event
        /// </summary>
        /// <param name="variableUpdate"></param>
        public void AddVariableUpdate(VariableUpdate variableUpdate)
        {
            Updates.Add(variableUpdate);
        }

        /// <summary>
        ///     Indicates that the rule fired impacts the variable provided as parameter
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool ImpactVariable(IVariable variable)
        {
            bool retVal = false;

            foreach (VariableUpdate variableUpdate in Updates)
            {
                retVal = variableUpdate.Changes.ImpactVariable(variable);
                if (retVal)
                {
                    break;
                }
            }

            return retVal;
        }
    }
}