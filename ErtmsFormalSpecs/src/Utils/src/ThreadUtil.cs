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

using System.Globalization;
using System.Threading;

namespace Utils
{
    public static class ThreadUtil
    {
        /// <summary>
        ///     Single location where threads are created
        /// </summary>
        /// <param name="name"></param>
        /// <param name="threadStart"></param>
        /// <returns></returns>
        public static Thread CreateThread(string name, ParameterizedThreadStart threadStart)
        {
            Thread retVal = new Thread(threadStart);
            retVal.Name = name;
            retVal.CurrentCulture = CultureInfo.InvariantCulture;

            return retVal;
        }

        /// <summary>
        ///     Single location where threads are created
        /// </summary>
        /// <param name="name"></param>
        /// <param name="threadStart"></param>
        /// <returns></returns>
        public static Thread CreateThread(string name, ThreadStart threadStart)
        {
            Thread retVal = new Thread(threadStart);
            retVal.Name = name;
            retVal.CurrentCulture = CultureInfo.InvariantCulture;

            return retVal;
        }
    }
}