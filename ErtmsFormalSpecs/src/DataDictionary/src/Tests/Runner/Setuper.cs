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

namespace DataDictionary.Tests.Runner
{
    /// <summary>
    ///     Sets up all variables before any execution on the system
    /// </summary>
    public class Setuper : Visitor
    {
        /// <summary>
        ///     Sets the default values to each variable
        /// </summary>
        /// <param name="variable">The variable to set</param>
        /// <param name="subNodes">Indicates whether sub nodes should be considered</param>
        public override void visit(Variable variable, bool subNodes)
        {
            Variables.Variable var = (Variables.Variable)variable;

            var.Value = var.DefaultValue;

            base.visit(variable, subNodes);
        }

        /// <summary>
        ///     Indicates which rules are not active
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Rule obj, bool visitSubNodes)
        {
            Rules.Rule rule = obj as Rules.Rule;
            if (rule != null)
            {
                rule.ActivationPriorities = null;
            }

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        ///     Clear the cache of all functions
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Function obj, bool visitSubNodes)
        {
            Functions.Function function = obj as Functions.Function;

            if (function != null)
            {
                function.ClearCache();
            }

            base.visit(obj, visitSubNodes);
        }
    }
}
