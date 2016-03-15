//------------------------------------------------------------------------------
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
    /// <summary>
    ///     Identifier of a disabled rule check
    /// </summary>
    public class RuleCheckIdentifier: Generated.RuleCheckIdentifier
    {
        /// <summary>
        ///     Provides the name of this rule checker disactivation
        ///     If this element has not been given a name, displays the Rule identifier instead
        /// </summary>
        public override string Name
        {
            get { return getName(); }
            set { setName(value); }
        }

        /// <summary>
        ///     Indicates whether the id matches this disabled rule check
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Match(RuleChecksEnum id)
        {
            return id.ToString().Equals(Name);
        }

        /// <summary>
        ///     Provides the enclosing collection to allow deletion of a rule set disabling
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return ((RuleCheckDisabling)Enclosing).DisabledRuleChecks; }
        }
    }
}
