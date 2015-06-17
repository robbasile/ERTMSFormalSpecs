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
using System.Text.RegularExpressions;
using DataDictionary.Tests.Runner.Events;
using DataDictionary.Types;
using DataDictionary.Variables;

namespace GUI.TestRunnerView.TimeLineControl
{
    public class FilterConfiguration
    {
        /// <summary>
        ///     Keep expectation events
        /// </summary>
        public bool Expect { get; set; }

        /// <summary>
        ///     Keep rule activations
        /// </summary>
        public bool RuleFired { get; set; }

        /// <summary>
        ///     Keep variable updates
        /// </summary>
        public bool VariableUpdate { get; set; }

        /// <summary>
        ///     Namespace that should be kept
        /// </summary>
        public List<NameSpace> NameSpaces { get; private set; }

        /// <summary>
        ///     Variables that should be kept
        /// </summary>
        public List<Variable> Variables { get; private set; }

        /// <summary>
        ///     The regular expression used for filtering
        /// </summary>
        public string RegExp { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public FilterConfiguration()
        {
            Expect = true;
            RuleFired = true;
            VariableUpdate = true;
            NameSpaces = new List<NameSpace>();
            Variables = new List<Variable>();
        }

        /// <summary>
        ///     Indicates that an event should be shown
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public bool VisibleEvent(ModelEvent evt)
        {
            bool retVal = true;

            // Check event type
            retVal = retVal && (!(evt is Expect) || Expect);
            retVal = retVal && (!(evt is RuleFired) || RuleFired);

            // Ignore the following internal events
            retVal = retVal && (!(evt is ExpectationStateChange));
            retVal = retVal && (!(evt is SubStepActivated));

            if (retVal)
            {
                // Checks the variable update
                VariableUpdate variableUpdate = evt as VariableUpdate;
                if (variableUpdate != null)
                {
                    if (VariableUpdate)
                    {
                        // Do not filter out variables updates for which the rule is not available
                        // because these updates are related to test steps or external input (using EFS service)
                        if (variableUpdate.Action.RuleCondition != null)
                        {
                            retVal = false;
                            foreach (Variable variable in Variables)
                            {
                                retVal = variableUpdate.Changes.ImpactVariable(variable);
                                if (retVal)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        retVal = false;
                    }
                }
                else
                {
                    // Check event namespace
                    if (evt.NameSpace != null)
                    {
                        retVal = NameSpaces.Contains(evt.NameSpace);
                    }
                }
            }

            // Keep messages that match the regular expression
            if (!Utils.Utils.isEmpty(RegExp))
            {
                Regex regularExpression = new Regex(RegExp);
                retVal = retVal || regularExpression.IsMatch(evt.Message);
            }

            return retVal;
        }
    }
}