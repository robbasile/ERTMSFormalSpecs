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
    ///     Initializes the execution time for functions and rules
    /// </summary>
    public class ExecutionTimeInitializer : Visitor
    {
        public override void visit(Function obj, bool visitSubNodes)
        {
            Functions.Function function = obj as Functions.Function;

            if (function != null)
            {
                function.ExecutionCount = 0;
                function.ExecutionTimeInMilli = 0L;
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Rule obj, bool visitSubNodes)
        {
            Rules.Rule rule = obj as Rules.Rule;

            if (rule != null)
            {
                rule.ExecutionCount = 0;
                rule.ExecutionTimeInMilli = 0L;
            }

            base.visit(obj, visitSubNodes);
        }
    }
}
