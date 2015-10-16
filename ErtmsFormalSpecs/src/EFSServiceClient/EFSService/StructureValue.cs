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

using System.Collections.Generic;

namespace EFSServiceClient.EFSService
{
    /// <summary>
    ///     Manually written code to access EFSModel
    /// </summary>
    public partial class StructureValue
    {
        /// <summary>
        /// The (full) name of the structure
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public StructureValue(string name)
        {
            Name = name;
            Value = new Dictionary<string, Value>();
        }

        /// <summary>
        ///     Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public override string DisplayValue()
        {
            string retVal = Name + "{";
            bool firstItem = true;

            foreach (KeyValuePair<string, Value> item in Value)
            {
                if (!firstItem)
                {
                    retVal += ", ";
                }
                else
                {
                    firstItem = false;
                }

                retVal += item.Key + " => " + item.Value.DisplayValue();
            }

            retVal += "}";

            return retVal;
        }
    }
}