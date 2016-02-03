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

namespace DataDictionary.Interpreter
{
    /// <summary>
    /// Additional data about parsing process
    /// </summary>
    public class ParsingData 
    {        
        /// <summary>
        ///     The start character in the string for this expression
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        ///     The end character in the string for this expression
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Indicates that the element has been completely parsed
        /// </summary>
        public bool CompletelyParsed { get; set; }

        /// <summary>
        /// When a parsing error has been found, provides the expected input
        /// </summary>
        public string[] Expected { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        /// <param name="completelyParsed">Indicates that the element has been completely parsed</param>
        /// <param name="expected">The expected input when error is found (CompletelyParsed == false)</param>
        public ParsingData(int start, int end, bool completelyParsed = true, string[] expected = null)
        {
            Start = start;
            End = end;
            CompletelyParsed = completelyParsed;
            Expected = expected;
        }

        /// <summary>
        /// Represents the parsing information for a synthetic tree node
        /// </summary>
        public static ParsingData SyntheticElement = new ParsingData(-1, -1);
    }
}