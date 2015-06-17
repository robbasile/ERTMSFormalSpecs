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

namespace DataDictionary.Compare
{
    /// <summary>
    ///     Tools used during version comparison
    /// </summary>
    public static class CompareUtil
    {
        /// <summary>
        ///     Computes a canonic form of a string
        /// </summary>
        /// <param name="s1"></param>
        /// <returns></returns>
        public static string canonicString(string s1)
        {
            string retVal = s1;

            if (retVal == null)
            {
                retVal = "";
            }

            retVal = retVal.Replace('\n', ' ');
            retVal = retVal.Replace('\t', ' ');
            retVal = retVal.Replace('\r', ' ');

            while (retVal.IndexOf("  ") >= 0)
            {
                retVal = retVal.Replace("  ", " ");
            }

            return retVal;
        }

        /// <summary>
        ///     Compares two strings
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool canonicalStringEquality(string s1, string s2)
        {
            bool retVal;

            s1 = canonicString(s1);
            s2 = canonicString(s2);
            retVal = s1 == s2;

            return retVal;
        }
    }
}