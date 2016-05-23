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
using System.Collections.Generic;
using System.Drawing;
using DataDictionary;

namespace GUIUtils.Editor.Patterns
{
    /// <summary>
    /// Recognizes elements of EFS in a string
    /// </summary>
    public class EfsRecognizer
    {
        /// <summary>
        ///     The patterns to apply
        /// </summary>
        public List<Pattern> Patterns { get; private set; }

        /// <summary>
        ///     The keywords
        /// </summary>
        private static readonly string[] Keywords =
        {
            "FORALL", "THERE_IS", "FIRST", "LAST", "IN", "MAP", "FILTER", "REDUCE", "COUNT", "SUM", "\\|", "USING", "=>", "<-", "NOT",
            "AND", "OR", "LET", "STABILIZE", "INITIAL_VALUE", "STOP_CONDITION",
            "==", "!=", "in", "not in", "<=", ">=", "<", ">", "is", "as", "\\+", "\\-", "\\*", "/", "\\^", "\\.",
            "APPLY", "ON", "INSERT", "WHEN", "FULL", "REPLACE", "REMOVE", "FIRST", "LAST", "ALL", "BY"
        };

        /// <summary>
        ///     The keywords
        /// </summary>
        private static readonly string[] ModelElements =
        {
            "NAMESPACE", "RANGE", "FROM", "TO", "ENUMERATION", "COLLECTION", "OF", "STRUCTURE", "FIELD", "INTERFACE",
            "IMPLEMENTS",
            "STATE",
            "FUNCTION", "RETURNS", "PROCEDURE", "PARAMETER", "RULE", "IF", "ELSIF", "ELSE", "THEN", "END",
            "FOLDER", "TRANSLATION", "IMPLIES"
        };

        /// <summary>
        ///     The compound keywords
        /// </summary>
        public static readonly Tuple<string, string>[] CompoundModelElements =
        {
            new Tuple<string, string>("STATE","MACHINE"), 
            new Tuple<string, string>("SOURCE","TEXT"),
            new Tuple<string, string>("REQUIREMENT","SET"),
            new Tuple<string, string>("MODEL","EVENT")
        };

        /// <summary>
        ///     Regular expression for a qualified name
        /// </summary>
        private const string QualifiedName = "\\b[a-zA-Z][\\.0-9a-zA-Z_]*\\b";

        /// <summary>
        /// The regular font
        /// </summary>
        private Font RegularFont { get; set; }

        /// <summary>
        /// Constrctor
        /// </summary>
        /// <param name="font"></param>
        public EfsRecognizer(Font font)
        {
            RegularFont = font;

            Pattern commentPattern = new Pattern(RegularFont, "//.*$")
            {
                Color = Color.Green
            };
            Pattern holeInTemplatePattern = new Pattern(RegularFont, "<[a-zA-Z\\b]+>")
            {
                Color = Color.OrangeRed
            };
            Pattern integerPattern = new Pattern(RegularFont, "\\b(?:[0-9]*\\.)?[0-9]+\\b")
            {
                Color = Color.BlueViolet
            };
            Pattern stringPattern = new Pattern(RegularFont, "\'[^\']*\'")
            {
                Color = Color.BlueViolet
            };
            ConstantPattern constantPattern = new ConstantPattern(RegularFont, QualifiedName)
            {
                Color = Color.BlueViolet
            };
            Pattern keywordPattern = new ListPattern(RegularFont, Keywords)
            {
                FontStyle = FontStyle.Bold
            };
            Pattern modelElementPattern = new ListPattern(RegularFont, ModelElements)
            {
                FontStyle = FontStyle.Bold | FontStyle.Underline
            };
            Pattern compoundModelElementPattern = new ListPattern(RegularFont, CompoundModelElements)
            {
                FontStyle = FontStyle.Bold | FontStyle.Underline
            };
            TypePattern typePattern = new TypePattern(RegularFont, QualifiedName)
            {
                Color = Color.Blue
            };

            Patterns = new List<Pattern>
            {
                commentPattern,
                holeInTemplatePattern,
                keywordPattern,
                modelElementPattern,
                compoundModelElementPattern,
                typePattern,
                integerPattern,
                stringPattern,
                constantPattern,
            };
        }

        /// <summary>
        /// Colorizes a single line in the rich text box using instance to provide context information
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="instance"></param>
        /// <param name="start"></param>
        /// <param name="line"></param>
        public void Colorize(SyntaxRichTextBox richTextBox, ModelElement instance, int start, string line)
        {
            List<ColorizedLocation> colored = new List<ColorizedLocation>();
            foreach (Pattern pattern in Patterns)
            {
                pattern.Colorize(richTextBox, instance, start, line, colored);
            }
        }

        /// <summary>
        /// Tokenize a single line
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public List<TextPart> TokenizeLine(ModelElement instance, string line)
        {
            List<TextPart> tmp = new List<TextPart>();

            List<ColorizedLocation> colored = new List<ColorizedLocation>();
            foreach (Pattern pattern in Patterns)
            {
                pattern.TokenizeLine(instance, line, tmp, colored);
            }
            tmp.Sort();

            List<TextPart> retVal = new List<TextPart>();
            int pos = 0;
            foreach (TextPart part in tmp)
            {
                // Don't consider overlapping parts
                bool overlapping = false;
                foreach (TextPart other in retVal)
                {
                    if (part.Start >= other.Start && part.Start <= other.Start + other.Length)
                    {
                        overlapping = true;
                        break;
                    }
                }

                if (!overlapping)
                {
                    // Create empty part, if needed
                    if (part.Start > pos)
                    {
                        retVal.Add(new TextPart(pos, part.Start - pos, Color.Black, RegularFont));
                    }

                    // Add the processed part
                    retVal.Add(part);

                    // Prepare for next loop iteration
                    pos = part.Start + part.Length;
                }
            }

            if (pos < line.Length)
            {
                retVal.Add(new TextPart(pos, line.Length - pos, Color.Black, RegularFont));                
            }
            retVal.Sort();

            return retVal;
        }
    }
}
