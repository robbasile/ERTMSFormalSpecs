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

using System.Collections.Generic;
using Net.Sgoliver.NRtfTree.Core;

namespace Importers.RtfDeltaImporter
{
    /// <summary>
    ///     A simple RTF parser, which shall extract only the relevant data
    /// </summary>
    public class Parser : SarParser
    {
        /// <summary>
        ///     Stores the paragraphs found in the document
        /// </summary>
        public Document Doc { get; private set; }

        /// <summary>
        ///     The stack used during analysis
        /// </summary>
        private ModeStack Stack { get; set; }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="filePath"></param>
        public Parser(string filePath, Document document)
        {
            Doc = document;
            Stack = new ModeStack();
            RtfReader reader = new RtfReader(this);
            reader.LoadRtfFile(filePath);
            reader.Parse();
        }

        public override void StartRtfDocument()
        {
        }

        public override void EndRtfDocument()
        {
        }

        /// <summary>
        ///     Indicates whether the prefix corresponds to a paragraph number
        /// </summary>
        private enum ParagraphNumberEnum
        {
            None,
            ListText,
            PnText
        };

        /// <summary>
        ///     Indicates whether the text should be taken into consideration
        /// </summary>
        private enum IgnoreTextEnum
        {
            IgnoreText,
            DoNotIgnore
        };

        /// <summary>
        ///     The current mode
        /// </summary>
        private class Mode
        {
            /// <summary>
            ///     The local paragraph number mode
            /// </summary>
            public ParagraphNumberEnum ParagraphNumberMode { get; set; }

            /// <summary>
            ///     The information whether the text should be ommitted, or not
            /// </summary>
            public IgnoreTextEnum IgnoreTextMode { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            public Mode()
            {
                ParagraphNumberMode = ParagraphNumberEnum.None;
                IgnoreTextMode = IgnoreTextEnum.DoNotIgnore;
            }
        }

        /// <summary>
        ///     The stack of modes
        /// </summary>
        private class ModeStack
        {
            /// <summary>
            ///     The stacked modes
            /// </summary>
            private List<Mode> Stack { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            public ModeStack()
            {
                Stack = new List<Mode>();
            }

            /// <summary>
            ///     Pushes a new frame on the stack
            /// </summary>
            public void PushFrame()
            {
                Stack.Insert(0, new Mode());
            }

            /// <summary>
            ///     Pops the last frame from the stack
            /// </summary>
            public void PopFrame()
            {
                Stack.RemoveAt(0);
            }

            /// <summary>
            ///     Indicates that the text should be ignored
            /// </summary>
            /// <returns></returns>
            public bool IgnoreText()
            {
                bool retVal = false;

                foreach (Mode mode in Stack)
                {
                    if (mode.IgnoreTextMode == IgnoreTextEnum.IgnoreText)
                    {
                        retVal = true;
                        break;
                    }
                }

                return retVal;
            }

            /// <summary>
            ///     Provides the active frame
            /// </summary>
            /// <returns></returns>
            public Mode CurrentFrame
            {
                get { return Stack[0]; }
            }

            /// <summary>
            ///     Indicates whether the paragraph mode corresponds to the mode provided as parameter
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public bool IsParagraphNumberMode(ParagraphNumberEnum val)
            {
                bool retVal = false;

                foreach (Mode mode in Stack)
                {
                    if (mode.ParagraphNumberMode == val)
                    {
                        retVal = true;
                        break;
                    }
                }

                return retVal;
            }
        }

        public override void StartRtfGroup()
        {
            Stack.PushFrame();
        }

        /// <summary>
        ///     At the end of an Rtf group, reset all modes
        /// </summary>
        public override void EndRtfGroup()
        {
            Stack.PopFrame();
        }

        public override void RtfControl(string key, bool hasParameter, int parameter)
        {
        }

        public override void RtfKeyword(string key, bool hasParameter, int parameter)
        {
            // Try to recognise a paragraph number \listtext\... 
            if (key.Equals("listtext"))
            {
                Stack.CurrentFrame.ParagraphNumberMode = ParagraphNumberEnum.ListText;
            }
                // Try to recognise a list number \pntext
            else if (key.Equals("pntext"))
            {
                Stack.CurrentFrame.ParagraphNumberMode = ParagraphNumberEnum.PnText;
            }


                // When an outline level is encountered, this starts a new paragraph
            else if (key.Equals("outlinelevel"))
            {
                if (CurrentParagraph != null && (CurrentParagraph.Text == null || CurrentParagraph.Text.Length == 0))
                {
                    // The current paragraph has not yet been filled, continue with it.
                }
                else
                {
                    CurrentParagraph = null;
                }
            }

            // \bkmkstart and \bkmkend seems to be related to bookmarks. Ignore the corresponding text
            if (key.Equals("xmlopen") ||
                key.Equals("bkmkstart") ||
                key.Equals("bkmkend") ||
                key.Equals("objclass") ||
                key.Equals("objdata") ||
                key.Equals("field") ||
                key.Equals("datafield") ||
                key.Equals("shpgrp") ||
                key.Equals("headerr") ||
                key.Equals("footerr") ||
                key.Equals("pntxta") ||
                key.Equals("shp") ||
                key.Equals("themedata") ||
                key.Equals("colorschememapping") ||
                key.Equals("lsdlockedexcept") ||
                key.Equals("datastore") ||
                key.Equals("pntxta") ||
                key.Equals("pntxtb") ||
                key.Equals("pict"))
            {
                Stack.CurrentFrame.IgnoreTextMode = IgnoreTextEnum.IgnoreText;
            }

            // Do the paragraph modifications
            if (key.Equals("par"))
            {
                addTextToCurrentParagraph("\n");
            }
            else if (key.Equals("line"))
            {
                addTextToCurrentParagraph("\n");
            }
            else if (key.Equals("cell"))
            {
                addTextToCurrentParagraph(",");
            }

            if (key.Equals("trowd"))
            {
                if (CurrentParagraph != null)
                {
                    CurrentParagraph = new TableRow(CurrentParagraph);
                    Doc.AddParagraph(CurrentParagraph);
                    EnclosingParagraphId = null;
                }
            }
        }


        /// <summary>
        ///     The paragraph currently processed
        /// </summary>
        private Paragraph CurrentParagraph = null;

        private string EnclosingParagraphId = null;

        public override void RtfText(string text)
        {
            bool isParagraphNumber = false;

            if (Stack.IsParagraphNumberMode(ParagraphNumberEnum.ListText))
            {
                isParagraphNumber = text.IndexOf(".") > 0;
                if (isParagraphNumber)
                {
                    CurrentParagraph = new Paragraph(text);
                    Doc.AddParagraph(CurrentParagraph);
                    EnclosingParagraphId = null;
                }
            }

            if (!isParagraphNumber)
            {
                if (Stack.IsParagraphNumberMode(ParagraphNumberEnum.PnText))
                {
                    text = text.Trim();

                    // This paragraph is a continuation of the preceding one. 
                    if (CurrentParagraph != null)
                    {
                        if (text.CompareTo("-") == 0)
                        {
                            // Add dash separated list in the same paragraph
                            // because there is no way to uniquely identify 
                            // each element
                            addTextToCurrentParagraph(text);
                        }
                        else
                        {
                            if (EnclosingParagraphId == null)
                            {
                                EnclosingParagraphId = CurrentParagraph.Id;
                            }

                            // Create the paragraph number
                            string id = EnclosingParagraphId + "." + text;
                            if (id.EndsWith(")"))
                            {
                                id = id.Substring(0, id.Length - 1);
                            }

                            CurrentParagraph = new Paragraph(id);
                            Doc.AddParagraph(CurrentParagraph);
                        }
                    }
                }
                else
                {
                    addTextToCurrentParagraph(text);
                }
            }
        }

        /// <summary>
        ///     Adds some text to the current paragraph
        /// </summary>
        /// <param name="text"></param>
        private void addTextToCurrentParagraph(string text)
        {
            if (CurrentParagraph != null)
            {
                if (!Stack.IgnoreText())
                {
                    CurrentParagraph.Text += text;
                }
            }
        }
    }
}