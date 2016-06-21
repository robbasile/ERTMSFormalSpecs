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
using GUIUtils.Editor.Patterns;

namespace GUI.ModelDiagram.Boxes.TextProcessing
{
    public class ProcessedText
    {
        /// <summary>
        /// The processed lines in the text
        /// </summary>
        public List<ProcessedLine> Lines { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessedText()
        {
            Lines = new List<ProcessedLine>();
        }

        /// <summary>
        /// Clears the content of this
        /// </summary>
        public void Clear()
        {
            Lines.Clear();    
        }

        /// <summary>
        /// Indicates whether the text is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return Lines.Count == 0;
        }

        /// <summary>
        /// Handles a string and update this structure according to the recognized elements
        /// </summary>
        /// <param name="instance">The element used as root element by the recognizer (to detect types for instance)</param>
        /// <param name="text">The text to recognize</param>
        /// <param name="regularFont">The regular font (default font)</param>
        /// <param name="vOffset"></param>
        /// <returns>The increased size for this new text</returns>
        public float Tokenize(ModelElement instance, string text, Font regularFont, float vOffset = 0.0F)
        {
            float retVal = 0.0F;
            if (text != null)
            {
                // Compute the location of the next line, according to the existing lines
                float pos = NextLinePosition(vOffset);

                EfsRecognizer recognizer = new EfsRecognizer(regularFont);
                foreach (string line in text.Split('\n'))
                {
                    ProcessedLine processedLine = new ProcessedLine();
                    processedLine.Tokenize(recognizer, instance, line, pos);
                    Lines.Add(processedLine);

                    pos = pos + processedLine.Size.Height;
                    retVal += processedLine.Size.Height;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Add text which is not highlighted, e.g. for comments
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        /// <param name="vOffset"></param>
        /// <returns>The increased size for this new text</returns>
        public float AddRawText(string text, Font font, Color color, float vOffset = 0.0F)
        {
            float retVal = 0.0F;
            if (text != null)
            {
                // Compute the location of the next line, according to the existing lines
                float pos = NextLinePosition(vOffset);

                foreach (string line in text.Split('\n'))
                {
                    ProcessedLine processedLine = new ProcessedLine();
                    processedLine.AddRawText(line, pos, font, color);
                    Lines.Add(processedLine);

                    pos = pos + processedLine.Size.Height;
                    retVal += processedLine.Size.Height;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Computes the vertical position of the next line
        /// </summary>
        /// <returns></returns>
        private float NextLinePosition(float vOffset = 0.0F)
        {
            float pos = 0.0F;
            foreach (var line in Lines)
            {
                pos = pos + line.Size.Height;
            }
            return pos + vOffset;
        }

        /// <summary>
        /// Provides the size required to display this text
        /// </summary>
        public SizeF Size
        {
            get
            {
                SizeF retVal = new SizeF(0,0);
                foreach (ProcessedLine line in Lines)
                {
                    SizeF lineSize = line.Size;
                    retVal = new SizeF(Math.Max(retVal.Width, lineSize.Width), retVal.Height + lineSize.Height);
                }

                return retVal;
            }
        }

        /// <summary>
        /// Displays the text using the graphics provided
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="location">The relative position where the text should be displayed</param>
        public void Display(Graphics graphics, PointF location)
        {
            foreach (ProcessedLine line in Lines)
            {
                line.Display(graphics, location);
            }            
        }

        
    }
}
