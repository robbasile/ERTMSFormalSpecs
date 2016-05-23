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
using System.Linq;
using DataDictionary;
using GUI.BoxArrowDiagram;
using GUI.ModelDiagram.Arrows;
using GUIUtils.Editor.Patterns;
using Utils;

namespace GUI.ModelDiagram.Boxes
{
    /// <summary>
    ///     The boxes that represent a model element
    /// </summary>
    public abstract class ModelControl : BoxControl<IModelElement, IGraphicalDisplay, ModelArrow>
    {

        /// <summary>
        /// The bold font
        /// </summary>
        public Font Bold { get; set; }

        /// <summary>
        /// The italic font
        /// </summary>
        public Font Italic { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        protected ModelControl(ModelDiagramPanel panel, IGraphicalDisplay model)
            : base(panel, model)
        {
            Bold = new Font(Font, FontStyle.Bold);
            Italic = new Font(Font, FontStyle.Italic);
            BoxMode = BoxModeEnum.Custom;
            Texts = new List<TextPosition>();
            ProcessedWords = new List<List<List<TextPosition>>>();
        }

        /// <summary>
        /// Registers a text to display
        /// </summary>
        public class TextPosition
        {
            public Point Location { get; set; }
            public string Text { get; set; }
            public Font Font { get; set; }
            public Color Color { get; set; }
        }

        public List<TextPosition> Texts { get; set; }
        
        /// <summary>
        /// Each text is cut into lines and then words.
        /// </summary>
        public List<List<List<TextPosition>>> ProcessedWords { get; set; }


        /// <summary>
        /// Regroup words of a given compounElement in a line cut into words.
        /// </summary>
        private static List<string> RegroupCompoundElement(List<string> line, Tuple<string, string> compoundElement)
        {
            List<string> retVal = new List<string>();
            foreach (string word in line)
            {
                // First possibility: the first word of the compoundElement, then add the concatenation to the list
                if (word == compoundElement.Item1 && 
                    line.Count > line.IndexOf(word) + 1 && 
                    line[line.IndexOf(word) + 1] == compoundElement.Item2)
                {
                    retVal.Add(word + ' ' + line[line.IndexOf(word) + 1]);
                }
                    // Second possibility: the second word of the compoundElement, then add nothing
                else if (word == compoundElement.Item2 &&
                         0 < line.IndexOf(word) &&
                         line[line.IndexOf(word) - 1] == compoundElement.Item1)
                {
                }
                    // Otherwise, add the word
                else
                {
                    retVal.Add(word);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Takes a stringlist, i.e. a line cut into words, and regroups compound modelElements.
        /// </summary>
        private static List<string> RegroupCompoundElements(List<string> line)
        {
            List<string> retVal = line;
            foreach (Tuple<string, string> compoundElement in EfsRecognizer.CompoundModelElements)
            {
                retVal = RegroupCompoundElement(retVal, compoundElement);
            }

            return retVal;
        }

        /// <summary>
        /// Process a text by cutting it into lines and then words. Turns these words into TextPosition.
        /// Finally, add everything to ProcessedWords.
        /// </summary>
        public void ProcessText(string text, Font font, Color color, Point location, Graphics graphics)
        {
            Point currentLineLocation = location;
            Point currentWordLocation = location;
            List<List<TextPosition>> currentProcessedWords = new List<List<TextPosition>>();
            SizeF textSize = new SizeF();

            List<string> lines = text.Split('\n').ToList();
            string[] wordSeparator = { " " };
            foreach (string line in lines)
            {
                SizeF currentLineSize = new SizeF();
                List<TextPosition> processedLine = new List<TextPosition>();
                List<string> words = RegroupCompoundElements(line.Split(wordSeparator, StringSplitOptions.None).ToList());
                foreach (string word in words)
                {
                    TextPosition processedWord = new TextPosition
                    {
                        Text = word,
                        Location = currentWordLocation
                    };
                    // Highlighted text
                    if (color == Color.Transparent)
                    {
                    }
                    // Normal text
                    else
                    {
                        processedWord.Color = color;
                        processedWord.Font = font;
                    }
                    SizeF wordSize = graphics.MeasureString(processedWord.Text, processedWord.Font);
                    currentLineSize += wordSize;
                    currentWordLocation = new Point(currentWordLocation.X + Convert.ToInt32(wordSize.Width), currentWordLocation.Y);
                    processedLine.Add(processedWord);
                }
                currentLineLocation = new Point(currentLineLocation.X, currentLineLocation.Y + font.Height + 1);
                currentWordLocation = currentLineLocation;
                textSize = new SizeF(Math.Max(textSize.Width, currentLineSize.Width), textSize.Height + font.Height +1);
                currentProcessedWords.Add(processedLine);
            }
            ProcessedWords.Add(currentProcessedWords);
        }


        /// <summary>
        /// Provides the computed position and size
        /// </summary>
        public bool ComputedPositionAndSize { get; set; }

        public Point ComputedLocation { get; set; }
        public Size ComputedSize { get; set; }

        /// <summary>
        ///     The location of the box
        /// </summary>
        public override Point Location
        {
            get
            {
                Point retVal;

                if (ComputedPositionAndSize)
                {
                    retVal = ComputedLocation;
                }
                else
                {
                    retVal = new Point(TypedModel.X, TypedModel.Y);
                }

                return retVal;
            }
            set
            {
                if (ComputedPositionAndSize)
                {
                    ComputedLocation = value;
                }
                else
                {
                    TypedModel.X = value.X;
                    TypedModel.Y = value.Y;
                }
            }
        }

        /// <summary>
        ///     The size of the box
        /// </summary>
        public override Size Size
        {
            get
            {
                Size retVal;

                if (ComputedPositionAndSize)
                {
                    retVal = ComputedSize;
                }
                else
                {
                    retVal = new Size(TypedModel.Width, TypedModel.Height);
                }

                return retVal;
            }
            set
            {
                if (ComputedPositionAndSize)
                {
                    ComputedSize = value;
                }
                else
                {
                    TypedModel.Width = value.Width;
                    TypedModel.Height = value.Height;
                }
            }
        }

        /// <summary>
        ///     The name of the kind of model
        /// </summary>
        public abstract string ModelName { get; }

        public override void PaintInBoxArrowPanel(Graphics graphics)
        {
            base.PaintInBoxArrowPanel(graphics);

            if (BoxMode == BoxModeEnum.Custom)
            {
                Pen pen = SelectPen();

                // Create the box
                Brush innerBrush = new SolidBrush(NormalColor);
                graphics.FillRectangle(innerBrush, Location.X, Location.Y, Width, Height);
                graphics.DrawRectangle(pen, Location.X, Location.Y, Width, Height);
            }

            if (Texts.Count == 0)
            {
                // Write the title
                string typeName = GuiUtils.AdjustForDisplay(ModelName, Width - 4, Bold);
                Brush textBrush = new SolidBrush(Color.Black);
                graphics.DrawString(typeName, Bold, textBrush, Location.X + 2, Location.Y + 2);
                graphics.DrawLine(NormalPen, new Point(Location.X, Location.Y + Font.Height + 2),
                    new Point(Location.X + Width, Location.Y + Font.Height + 2));

                // Write the text in the box
                // Center the element name
                string name = GuiUtils.AdjustForDisplay(TypedModel.GraphicalName, Width, Font);
                SizeF textSize = graphics.MeasureString(name, Font);
                int boxHeight = Height - Bold.Height - 4;
                graphics.DrawString(name, Font, textBrush, Location.X + Width/2 - textSize.Width/2,
                    Location.Y + Bold.Height + 4 + boxHeight/2 - Font.Height/2);
            }
            else
            {
                // Draw the line between the title and the rest of the box
                graphics.DrawLine(
                    NormalPen,
                    new Point(Location.X, Location.Y + Font.Height + 2),
                    new Point(Location.X + Width, Location.Y + Font.Height + 2));

                // Display the pre computed text at their corresponding locaations
                foreach (TextPosition textPosition in Texts)
                {
                    if (textPosition.Color != Color.Transparent)
                    {
                        graphics.DrawString(
                            textPosition.Text,
                            textPosition.Font,
                            new SolidBrush(textPosition.Color),
                            textPosition.Location.X,
                            textPosition.Location.Y);
                    }
                    else
                    {
                        // Syntax highlighting
                        ModelDiagramPanel panel = (ModelDiagramPanel) Panel;
                        string[] lines = textPosition.Text.Split('\n');
                        PointF location = textPosition.Location;
                        foreach (string line in lines)
                        {
                            List<TextPart> parts = panel.Recognizer.TokenizeLine(TypedModel as DataDictionary.ModelElement, line);
                            foreach (TextPart part in parts)
                            {
                                string str = line.Substring(part.Start, part.Length);
                                SizeF size;
                                if (str == " ")
                                {
                                    size = new SizeF(2.0F, Font.Height);
                                }
                                else
                                {
                                    StringFormat sf = StringFormat.GenericTypographic;
                                    sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                                    size = GuiUtils.Graphics.MeasureString(str, part.Font, location, sf);                                    
                                }

                                graphics.DrawString(str, part.Font, new SolidBrush(part.Color), location.X, location.Y);

                                // Measure string does not handle spaces correctly
                                int increment = 1;
                                if (str.StartsWith(" "))
                                {
                                    increment += 1;
                                }
                                if (str.EndsWith(" "))
                                {
                                    increment += 1;
                                }
                                if (part.Font.Bold)
                                {
                                    increment += 3;
                                }
                                location = new PointF(location.X + size.Width + increment, location.Y);
                            }
                            location = new PointF(textPosition.Location.X, location.Y + Font.Height);
                        }
                    }
                }
            }
        }
    }
}