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

namespace DataDictionary.RuleCheck
{
    /// <summary>
    ///     Enumeration of the different rule checks performed on the model
    /// </summary>
    public enum RuleChecksEnum
    {
        // Interpreter tree node
        SyntaxError,
        SemanticAnalysisError,
        ExecutionFailed,
        // Rule check - more specific
        Access01,
        Access02,
        Naming01,
        Naming02,
        Process01,
        Process02,
        Process03,
        Process04,
        Process05,
        Process06,
        Process07,
        Process08,
        Process09,
        Process10,
        Process11,
        Process12,
        Process13,
        Process14,
        Process15,
        Request01,
        Requirements01,
        Requirements02,
        Requirements03,
        StateMachine01,
        StateMachine02,
        StateMachine03,
        StateMachine04,
        Structure01,
        Test01,
        Test02,
        Test03,
        Test04,
        Translation01,
        Translation02,
        Translation03,
        Type01,
        Type02,
        Type03,
        Type04,
        Type05,
        Type06,
        Type07,
        Type08,
        Type09,
        Type10,
        Type11,
        Type12,
        Update01,
        Update02
    }

    /// <summary>
    ///     Subset of the RuleChecksEnum
    ///     Enumeration of the different rule checks that could be disabled
    /// </summary>
    public enum DisableableRuleChecksEnum
    {
        Access01 = RuleChecksEnum.Access01,
        Access02 = RuleChecksEnum.Access01,
        Naming01 = RuleChecksEnum.Naming01,
        Naming02 = RuleChecksEnum.Naming02,
        Process01 = RuleChecksEnum.Process01,
        Process02 = RuleChecksEnum.Process02,
        Process03 = RuleChecksEnum.Process03,
        Process04 = RuleChecksEnum.Process04,
        Process05 = RuleChecksEnum.Process05,
        Process06 = RuleChecksEnum.Process06,
        Process07 = RuleChecksEnum.Process07,
        Process08 = RuleChecksEnum.Process08,
        Process09 = RuleChecksEnum.Process09,
        Process10 = RuleChecksEnum.Process10,
        Process11 = RuleChecksEnum.Process11,
        Process12 = RuleChecksEnum.Process12,
        Process13 = RuleChecksEnum.Process13,
        Process14 = RuleChecksEnum.Process14,
        Process15 = RuleChecksEnum.Process15,
        Request01 = RuleChecksEnum.Request01,
        Requirements01 = RuleChecksEnum.Requirements01,
        Requirements02 = RuleChecksEnum.Requirements02,
        Requirements03 = RuleChecksEnum.Requirements03,
        StateMachine01 = RuleChecksEnum.StateMachine01,
        StateMachine02 = RuleChecksEnum.StateMachine02,
        StateMachine03 = RuleChecksEnum.StateMachine03,
        StateMachine04 = RuleChecksEnum.StateMachine04,
        Structure01 = RuleChecksEnum.Structure01,
        Test01 = RuleChecksEnum.Test01,
        Test02 = RuleChecksEnum.Test02,
        Test03 = RuleChecksEnum.Test03,
        Test04 = RuleChecksEnum.Test04,
        Translation01 = RuleChecksEnum.Translation01,
        Translation02 = RuleChecksEnum.Translation02,
        Translation03 = RuleChecksEnum.Translation03,
        Type01 = RuleChecksEnum.Type01,
        Type02 = RuleChecksEnum.Type02,
        Type03 = RuleChecksEnum.Type03,
        Type04 = RuleChecksEnum.Type04,
        Type05 = RuleChecksEnum.Type05,
        Type06 = RuleChecksEnum.Type06,
        Type07 = RuleChecksEnum.Type07,
        Type08 = RuleChecksEnum.Type08,
        Type09 = RuleChecksEnum.Type09,
        Type10 = RuleChecksEnum.Type10,
        Type11 = RuleChecksEnum.Type11,
        Type12 = RuleChecksEnum.Type12,
        Update01 = RuleChecksEnum.Update01,
        Update02 = RuleChecksEnum.Update02
    }
}
