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
using System.Drawing;

namespace GUIUtils.Editor.Patterns
{
    /// <summary>
    ///     A pattern consisting of a list of elements
    /// </summary>
    public class ListPattern : Pattern
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="baseFont"></param>
        public ListPattern(Font baseFont, params string[] elements)
            : base(baseFont, ComputeRegExp(elements))
        {
        }

        public ListPattern(Font baseFont, params Tuple<string,string>[] elements)
            : base(baseFont, ComputeTupleRegExp(elements))
        {
        }

        private static string ComputeTupleRegExp(Tuple<string, string>[] elements)
        {
            string retVal = "";

            foreach (Tuple<string, string> element in elements)
            {
                if (!string.IsNullOrEmpty(retVal))
                {
                    retVal += "|";
                }
                retVal += "\\b" + element.Item1 + " " + element.Item2 + "\\b";
            }
            return retVal;
        }

        /// <summary>
        ///     Compiles the keywords as a regular expression.
        /// </summary>
        public static string ComputeRegExp(string[] elements)
        {
            string retVal = "";

            foreach (string element in elements)
            {
                if (!string.IsNullOrEmpty(retVal))
                {
                    retVal += "|";
                }
                if (Char.IsLetterOrDigit(element[0]))
                {
                    retVal += "\\b" + element + "\\b";
                }
                else
                {
                    retVal += element;
                }
            }

            return retVal;
        }
    }
}
