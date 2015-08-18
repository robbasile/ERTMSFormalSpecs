﻿// ------------------------------------------------------------------------------
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

namespace EFSServiceClient.EFSService
{
    public partial class EFSServiceClient
    {
        /// <summary>
        ///     Gets the value of an expression, using the EFS engine
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public double GetDoubleValue(string expression)
        {
            double retVal = double.NaN;

            DoubleValue tmp = GetExpressionValue(expression) as DoubleValue;
            if (tmp != null)
            {
                retVal = tmp.Value;
            }

            return retVal;
        }
    }
}