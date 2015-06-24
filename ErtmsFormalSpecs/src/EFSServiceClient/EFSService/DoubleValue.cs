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
using System.Globalization;

namespace EFSServiceClient.EFSService
{   
    /// <summary>
    ///     Manually written code to access EFSModel
    /// </summary>
    public partial class DoubleValue
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        public DoubleValue(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Provides the double value for this
        /// </summary>
        public double Value
        {
            get
            {
                double retVal = 0.0;

                try
                {
                    retVal = double.Parse(Image, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                }

                return retVal;
            }
            set
            {
                Image = value.ToString(CultureInfo.InvariantCulture);
                if (!Image.Contains("."))
                {
                    Image = Image + ".0";
                }
            }
        }

        /// <summary>
        ///     Provides the display value of this value
        /// </summary>
        /// <returns></returns>
        public override string DisplayValue()
        {
            return Image;
        }
    }
}
