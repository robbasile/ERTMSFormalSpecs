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

using DataDictionary.Variables;

namespace DataDictionary.Values
{
    /// <summary>
    ///     A place holder for a default value, not available through the EFS interface
    /// </summary>
    public class DefaultValue : Value
    {
        public override string Name { get; set; }

        public override string LiteralName
        {
            get { return Name; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        public DefaultValue(IVariable variable)
            : base(variable.Type)
        {
            Name = variable.Default;
        }
    }
}