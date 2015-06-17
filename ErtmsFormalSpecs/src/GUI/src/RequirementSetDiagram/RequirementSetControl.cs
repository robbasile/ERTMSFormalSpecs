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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Specification;
using GUI.BoxArrowDiagram;
using GUI.SpecificationView;
using ModelElement = Utils.ModelElement;

namespace GUI.RequirementSetDiagram
{
    public partial class RequirementSetControl : BoxControl<RequirementSet, RequirementSetDependancy>
    {
        /// <summary>
        ///     The metrics associates to this requirements set
        /// </summary>
        private ParagraphTreeNode.ParagraphSetMetrics Metrics { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public RequirementSetControl()
            : base()
        {
            MouseDoubleClick += new MouseEventHandler(HandleMouseDoubleClick);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public RequirementSetControl(IContainer container)
            : base(container)
        {
            MouseDoubleClick += new MouseEventHandler(HandleMouseDoubleClick);
        }

        public override RequirementSet Model
        {
            set
            {
                base.Model = value;
                List<Paragraph> paragraphs = new List<Paragraph>();
                Model.GetParagraphs(paragraphs);
                Metrics = ParagraphTreeNode.CreateParagraphSetMetrics(EFSSystem.INSTANCE, paragraphs);
            }
        }

        public override void AcceptDrop(ModelElement element)
        {
            base.AcceptDrop(element);

            // Allows to allocate paragraphs in requirement sets
            Paragraph paragraph = element as Paragraph;
            if (paragraph != null)
            {
                if (!paragraph.AppendToRequirementSet(Model))
                {
                    MessageBox.Show("Paragraph not added to the requirement set because it already belongs to it",
                        "Paragraph not added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                GUIUtils.MDIWindow.SpecificationWindow.TreeView.Selected.SelectionChanged(false);
            }
        }

        public override void SelectBox()
        {
            base.SelectBox();

            GUIUtils.MDIWindow.SetCoverageStatus(Model);
        }

        /// <summary>
        ///     Handles a double click event on the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseDoubleClick(object sender, MouseEventArgs e)
        {
            SelectBox();

            RequirementSetPanel panel = (RequirementSetPanel) Panel;
            if (panel != null)
            {
                RequirementSetDiagramWindow window = new RequirementSetDiagramWindow();
                GUIUtils.MDIWindow.AddChildWindow(window);
                window.SetEnclosing(Model);
                window.Text = Model.Name;
            }
        }

        /// <summary>
        ///     Implemented color
        /// </summary>
        public static Color IMPLEMENTED_COLOR = Color.Green;

        public static Pen IMPLEMENTED_PEN = new Pen(IMPLEMENTED_COLOR);

        /// <summary>
        ///     Tested color
        /// </summary>
        public static Color TESTED_COLOR = Color.Yellow;

        public static Pen TESTED_PEN = new Pen(TESTED_COLOR);

        /// <summary>
        ///     Draws the box within the box-arrow panel
        /// </summary>
        /// <param name="g"></param>
        public override void PaintInBoxArrowPanel(Graphics g)
        {
            SetColor(Color.Transparent);

            // Draws the enclosing box
            g.FillRectangle(new SolidBrush(NORMAL_COLOR), Location.X, Location.Y, Width, Height);
            g.DrawRectangle(NORMAL_PEN, Location.X, Location.Y, Width, Height);

            // Draws the completion box
            g.DrawRectangle(NORMAL_PEN, Location.X + 10, Location.Y + Height - 20, Width - 20, 10);
            FillCompletion(g, Metrics.implementedCount, Metrics.implementableCount, IMPLEMENTED_COLOR, 10);
            FillCompletion(g, Metrics.testedCount, Metrics.implementableCount, TESTED_COLOR, 5);
        }

        /// <summary>
        ///     Fills the progression bar according to the task ratio completed
        /// </summary>
        /// <param name="g"></param>
        /// <param name="performed"></param>
        /// <param name="total"></param>
        /// <param name="color"></param>
        private void FillCompletion(Graphics g, int performed, int total, Color color, int width)
        {
            double ratio = 1;
            if (total > 0)
            {
                ratio = (double) performed/(double) total;
            }
            g.FillRectangle(new SolidBrush(color), Location.X + 10 + 1, Location.Y + Height - 20 + 10 - width + 1,
                (int) ((Width - 20)*ratio) - 1, width - 1);
        }
    }
}