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

using System.Collections;

namespace DataDictionary.RuleCheck
{
    public class RuleCheckDisabling : Generated.RuleCheckDisabling
    {
        /// <summary>
        ///     The list of rule checks that have been disabled inside this namespace
        /// </summary>
        public ArrayList DisabledRuleChecks
        {
            get
            {
                if (allDisabledRuleChecks() == null)
                {
                    setAllDisabledRuleChecks(new ArrayList());
                }

                return allDisabledRuleChecks();
            }
            set
            {
                setAllDisabledRuleChecks(value);
            }
        }

        /// <summary>
        ///     Indicates whether the rule check identified by id is enabled inside this namespace
        /// </summary>
        /// <returns></returns>
        public bool Enabled(RuleChecksEnum id)
        {
            bool retVal = true;

            foreach (RuleCheckIdentifier identifier in DisabledRuleChecks)
            {
                if (identifier.Match(id))
                {
                    retVal = false;
                    break;
                }
            }

            return retVal;
        }
    }
}
