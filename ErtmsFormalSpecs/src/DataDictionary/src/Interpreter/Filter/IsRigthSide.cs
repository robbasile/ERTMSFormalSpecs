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

using Utils;

namespace DataDictionary.Interpreter.Filter
{
    /// <summary>
    ///     Predicates which indicates that the namable can be the right side of an assignment
    /// </summary>
    public class IsRightSide : IsVariableOrValue
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        protected IsRightSide()
            : base()
        {
        }

        /// <summary>
        ///     Predicate which indicates whether the namable provided matches the expectation for the semantic analysis
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool AcceptableChoice(INamable value)
        {
            bool retVal = base.AcceptableChoice(value) && !IsCallable.Predicate(value);

            return retVal;
        }

        /// <summary>
        ///     Singleton
        /// </summary>
        public new static IsRightSide INSTANCE = new IsRightSide();
    }
}