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

using DataDictionary.Types;

namespace DataDictionary.Values
{
    public class BoolValue : BaseValue<Type, bool>
    {
        public override string Name
        {
            get
            {
                if (!Val)
                {
                    return "False";
                }
                return "True";
            }
        }

        public override string FullName
        {
            get { return Type.FullName + "." + Name; }
        }

        public override string LiteralName
        {
            get { return Name; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        public BoolValue(Type type, bool val)
            : base(type, val)
        {
        }
    }
}