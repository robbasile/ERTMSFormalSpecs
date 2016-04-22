// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpecs software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using DataDictionary.Interpreter;
using DataDictionary.Rules;
using Utils;

namespace DataDictionary.Tests.Runner
{
    /// <summary>
    /// Represents a rule condition activation
    /// </summary>
    public class Activation
    {
        /// <summary>
        ///     The action to activate
        /// </summary>
        public RuleCondition RuleCondition { get; private set; }

        /// <summary>
        ///     The instance on which the action is applied
        /// </summary>
        public IModelElement Instance { get; private set; }

        /// <summary>
        ///     The explanation why this activation has been performed
        /// </summary>
        public ExplanationPart Explanation { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ruleCondition">The rule condition which leads to this activation</param>
        /// <param name="instance">The instance on which this rule condition's preconditions are evaluated to true</param>
        /// <param name="explanation"></param>
        public Activation(RuleCondition ruleCondition, IModelElement instance, ExplanationPart explanation)
        {
            RuleCondition = ruleCondition;
            Instance = instance;
            Explanation = explanation;
        }

        /// <summary>
        ///     Indicates that two Activations are the same when they share the action and,
        ///     if specified, the instance on which they are applied
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool retVal = false;

            Activation other = obj as Activation;
            if (other != null)
            {
                retVal = RuleCondition.Equals(other.RuleCondition);
                if (retVal && Instance != null)
                {
                    if (other.Instance != null)
                    {
                        retVal = Instance.Equals(other.Instance);
                    }
                    else
                    {
                        retVal = false;
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        ///     The hash code, according to Equal operator.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int retVal = RuleCondition.GetHashCode();

            if (Instance != null)
            {
                retVal = retVal + Instance.GetHashCode();
            }

            return retVal;
        }
    }
}
