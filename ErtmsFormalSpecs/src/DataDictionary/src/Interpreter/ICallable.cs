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
using System.Collections.Generic;
using Utils;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    /// <summary>
    ///     Something that can be called
    /// </summary>
    public interface ICallable : INamable
    {
        /// <summary>
        ///     Formal parameters of the callable
        /// </summary>
        ArrayList FormalParameters { get; }

        /// <summary>
        ///     Provides the formal parameter which matches the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Parameter GetFormalParameter(string name);

        /// <summary>
        ///     Provides the return type of the called element
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        ///     Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        void AdditionalChecks(ModelElement root, Dictionary<string, Expression> actualParameters);
    }
}
