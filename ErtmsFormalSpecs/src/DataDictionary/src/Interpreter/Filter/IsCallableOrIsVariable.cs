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
    ///     Indicates that the namable is a callable or a variable
    /// </summary>
    public class IsCallableOrIsVariable : IsVariable
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        protected IsCallableOrIsVariable()
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
            bool retVal = base.AcceptableChoice(value);

            if (!retVal)
            {
                retVal = IsCallable.Predicate(value);
            }

            return retVal;
        }

        /// <summary>
        ///     Singleton
        /// </summary>
        public new static IsCallableOrIsVariable INSTANCE = new IsCallableOrIsVariable();
    }
}