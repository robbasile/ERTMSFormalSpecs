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

using System;
using DataDictionary.Interpreter;
using DataDictionary.Values;

namespace DataDictionary.Types
{

    /// <summary>
    ///     Nothing
    /// </summary>
    public class NoType : Type
    {
        public override string Name
        {
            get { return "NoType"; }
            set { }
        }

        public override string FullName
        {
            get { return Name; }
        }

        /// <summary>
        ///     Constrcutor
        /// </summary>
        public NoType(EfsSystem efsSystem)
        {
            Enclosing = efsSystem;
        }

        public override IValue PerformArithmericOperation(InterpretationContext context, IValue left,
            BinaryExpression.Operator operation, IValue right)
        {
            throw new Exception("Cannot perform arithmetic operation between " + left.LiteralName + " and " +
                                right.LiteralName);
        }

        /// <summary>
        ///     Indicates that the other type can be placed in variables of this type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            return false;
        }
    }
}
