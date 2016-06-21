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
    public class ProcessedLine
    {
        /// <summary>
        /// The processed chunks in the line
        /// </summary>
        public List<TextChunk> TextChunks { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessedLine()
        {
            TextChunks = new List<TextChunk>();
        }

        /// <summary>
        /// Handles a line and update this structure according to the recognized elements
        /// </summary>
        /// <param name="recognizer">The recognizer used to identify tokens</param>
        /// <param name="instance">The element used as root element by the recognizer (to detect types for instance)</param>
        /// <param name="line">The line to process</param>
        /// <param name="yPos">The top position of the line (vertical position)</param>
        public void Tokenize(EfsRecognizer recognizer, ModelElement instance, string line, float yPos)
        {
            float xPos = 0.0F;

            List<TextPart> tokens = recognizer.TokenizeLine(instance, line);
            foreach (var token in tokens)
            {
                TextChunk textChunk = new TextChunk(token, new Point((int)xPos, (int)yPos));
                TextChunks.Add(textChunk);
                xPos = xPos + textChunk.Size.Width;
            }
        }

        /// <summary>
        /// Provides the size of the line
        /// </summary>
        public SizeF Size
        {
            get
            {
                SizeF retVal = new SizeF(0,0);

                foreach (TextChunk textChunk in TextChunks)
                {
                    SizeF chunkSize = textChunk.Size;
                    retVal = new SizeF(retVal.Width + chunkSize.Width, Math.Max(retVal.Height, chunkSize.Height));                    
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
            foreach (TextChunk textChunk in TextChunks)
            {
                textChunk.Display(graphics, location);
            }
        }

        /// <summary>
        /// Add not highlighted text in the line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pos"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        public void AddRawText(string line, float pos, Font font, Color color)
        {
            TextPart textPart = new TextPart(line, 0, color, font);
            TextChunk textChunk = new TextChunk(textPart, new Point(0, (int) pos));
            TextChunks.Add(textChunk);
        }
    }
}
