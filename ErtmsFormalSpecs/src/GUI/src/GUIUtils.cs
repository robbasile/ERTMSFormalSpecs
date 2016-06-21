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

using System.Drawing;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Interpreter;
using Utils;

namespace GUI
{
    public static class GuiUtils
    {
        /// <summary>
        ///     The main window of the application
        /// </summary>
        public static MainWindow MdiWindow { get; set; }

        /// <summary>
        ///     Access to a graphics item
        /// </summary>
        public static Graphics Graphics { get; set; }

        /// --------------------------------------------------------------------
        /// Enclosing finder
        /// --------------------------------------------------------------------
        /// <summary>
        ///     Finds in an enclosing element the element whose type matches T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class EnclosingFinder<T> : IFinder
            where T : class
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public EnclosingFinder()
            {
                FinderRepository.INSTANCE.Register(this);
            }

            /// <summary>
            ///     Finds an enclosing element whose type is T
            /// </summary>
            /// <param name="el"></param>
            /// <returns></returns>
            public static T Find(Control el)
            {
                while (el != null && !(el is T))
                {
                    el = el.Parent;
                }
                return el as T;
            }

            /// <summary>
            ///     Clears the cache
            /// </summary>
            public void ClearCache()
            {
                // No cache
            }
        }


        public static SizeF MeasureDisplayedString(string text, Font font)
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            SizeF retVal;

            // The functions used to measure the string do not accept empty strings.
            if (string.IsNullOrEmpty(text))
            {
                retVal = new SizeF(0.0f, 0.0f);
            }
            else
            {
                StringFormat format = new StringFormat(StringFormat.GenericTypographic)
                {
                    FormatFlags = StringFormatFlags.MeasureTrailingSpaces
                };

                RectangleF rect = new RectangleF(0, 0, int.MaxValue, int.MaxValue);
                CharacterRange[] ranges =
                {
                    new CharacterRange(0, text.Length)
                };

                format.SetMeasurableCharacterRanges(ranges);

                Region[] regions = Graphics.MeasureCharacterRanges(text, font, rect, format);
                rect = regions[0].GetBounds(Graphics);

                retVal = new SizeF(rect.Right + 1.0f, rect.Height);

                if (font.Style == FontStyle.Italic)
                {
                    retVal = new SizeF(1.02f*retVal.Width, retVal.Height);
                }
            }
            return retVal;
        }

        /// <summary>
        ///     Adjust the text size according to the display size
        /// </summary>
        /// <param name="text">The text to be adjusted</param>
        /// <param name="width">The desired width</param>
        /// <param name="font">The font used to display the text</param>
        public static string AdjustForDisplay(string text, int width, Font font)
        {
            string retVal = text;

            if (Graphics.MeasureString(text, font).Width > width)
            {
                width = (int) (width - Graphics.MeasureString("...", font).Width);
                int i = text.Length;
                int step = i/2;
                while (step > 0 && Graphics.MeasureString(text.Substring(0, i), font).Width > width)
                {
                    i = i - step;
                    step = step/2;
                    while (Graphics.MeasureString(text.Substring(0, i), font).Width < width && step > 0)
                    {
                        i = i + step;
                    }
                }
                retVal = text.Substring(0, i) + "...";
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the selection criteria according to the mouse event arg
        /// </summary>
        /// <param name="mouseEventArgs"></param>
        /// <returns></returns>
        public static Context.SelectionCriteria SelectionCriteriaBasedOnMouseEvent(MouseEventArgs mouseEventArgs)
        {
            Context.SelectionCriteria retVal = Context.SelectionCriteria.None;

            if (mouseEventArgs != null)
            {
                if (mouseEventArgs.Button == MouseButtons.Left)
                {
                    retVal = retVal | Context.SelectionCriteria.LeftClick;
                }
                if (mouseEventArgs.Button == MouseButtons.Right)
                {
                    retVal = retVal | Context.SelectionCriteria.RightClick;
                }
                if (mouseEventArgs.Clicks > 1)
                {
                    retVal = retVal | Context.SelectionCriteria.DoubleClick;
                }
                if (Control.ModifierKeys == Keys.Control)
                {
                    retVal = retVal | Context.SelectionCriteria.Ctrl;
                }
                if (Control.ModifierKeys == Keys.Alt)
                {
                    retVal = retVal | Context.SelectionCriteria.Alt;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the node corresponding to the model element
        /// </summary>
        /// <param name="selectionContext"></param>
        /// <returns></returns>
        public static BaseTreeNode SourceNode(Context.SelectionContext selectionContext)
        {
            BaseTreeNode retVal = selectionContext.Sender as BaseTreeNode;

            if (retVal == null)
            {
                foreach (IBaseForm form in MdiWindow.SubWindows)
                {
                    if (form != null && form.TreeView != null)
                    {
                        retVal = form.TreeView.FindNode(selectionContext.Element, true);

                        if (retVal != null)
                        {
                            // Finds the upper node for the same model
                            BaseTreeNode parent = retVal.Parent as BaseTreeNode;
                            while (parent != null && parent.Model == retVal.Model)
                            {
                                retVal = parent;
                                parent = retVal.Parent as BaseTreeNode;
                            }

                            break;
                        }
                    }                    
                }
            }

            return retVal;
        }
    }
}