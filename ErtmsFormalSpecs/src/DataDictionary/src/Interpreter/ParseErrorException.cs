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

namespace DataDictionary.Interpreter
{
    public class ParseErrorException : Exception
    {
        /// <summary>
        ///     The size of the context to provide
        /// </summary>
        private static int CONTEXT_SIZE = 20;

        /// <summary>
        ///     Builds the context messag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="index"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static string buildContext(string message, int index, char[] buffer)
        {
            string retVal = message + " near ...";

            int i = Math.Max(0, index - CONTEXT_SIZE);
            while (i < index)
            {
                retVal += buffer[i];
                i += 1;
            }

            retVal += "^";

            while (i < index + CONTEXT_SIZE && i < buffer.Length)
            {
                retVal += buffer[i];
                i += 1;
            }

            retVal += "...";

            return retVal;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="index"></param>
        /// <param name="buffer"></param>
        public ParseErrorException(string message, int index, char[] buffer)
            : base(buildContext(message, index, buffer))
        {
        }
    }
}