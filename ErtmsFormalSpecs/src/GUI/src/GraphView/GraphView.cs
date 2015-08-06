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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DataDictionary;
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using ErtmsSolutions.Etcs.Subset26.BrakingCurves;
using GUI.DataDictionaryView;
using GUI.Shortcuts;
using WeifenLuo.WinFormsUI.Docking;
using Graph = DataDictionary.Functions.Graph;

namespace GUI.GraphView
{
    public partial class GraphView : BaseForm
    {
        /// <summary>
        ///     The functions to be displayed in this graph view
        /// </summary>
        public List<Function> Functions { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public GraphView()
        {
            InitializeComponent();
            FormClosed += GraphView_FormClosed;

            AllowDrop = true;
            DragEnter += GraphView_DragEnter;
            DragDrop +=GraphView_DragDrop;

            Functions = new List<Function>();

            DockAreas = DockAreas.Document;

            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                tabPage.MouseEnter += (s, e) => tabPage.Focus();
            }
        }

        private void GraphView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void GraphView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WindowsForms10PersistentObject", false))
            {
                BaseTreeNode sourceNode = e.Data.GetData("WindowsForms10PersistentObject") as BaseTreeNode;
                if (sourceNode != null)
                {
                    FunctionTreeNode functionTreeNode = sourceNode as FunctionTreeNode;
                    if (functionTreeNode != null)
                    {
                        AddFunction(functionTreeNode.Item, null);
                    }
                    else
                    {
                        ShortcutTreeNode shortcutTreeNode = sourceNode as ShortcutTreeNode;
                        if (shortcutTreeNode != null)
                        {
                            AddFunction(shortcutTreeNode.Item.GetReference() as Function, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Adds a new function to this graph
        /// </summary>
        /// <param name="function"></param>
        /// <param name="explain"></param>
        private void AddFunction(Function function, ExplanationPart explain)
        {
            if (function != null)
            {
                InterpretationContext context = new InterpretationContext(function);
                if (function.FormalParameters.Count == 1)
                {
                    Parameter parameter = (Parameter) function.FormalParameters[0];
                    Graph graph = function.createGraph(context, parameter, explain);
                    if (graph != null)
                    {
                        Functions.Add(function);
                        Refresh();
                    }
                }
                else if (function.FormalParameters.Count == 2)
                {
                    Surface surface = function.createSurface(context, explain);
                    if (surface != null)
                    {
                        Functions.Add(function);
                        Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Cannot add this function to the display view", "Cannot display function",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        ///     Handles the close event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphView_FormClosed(object sender, FormClosedEventArgs e)
        {
            GUIUtils.MDIWindow.HandleSubWindowClosed(this);
        }

        /// <summary>
        ///     Allows to refresh the view, according to the fact that the structure for the model could change
        /// </summary>
        public override void Refresh()
        {
            Display();
        }

        /// <summary>
        ///     Refreshes the model
        /// </summary>
        public override void RefreshModel()
        {
            // The model is always the same function
        }

        /// <summary>
        /// Displays the graph
        /// </summary>
        /// <returns></returns>
        public void Display()
        {
            GraphVisualiser.Reset();
            String name = null;

            // Computes the expected end to display
            double expectedEndX = 0;
            Dictionary<Function, Graph> graphs = new Dictionary<Function, Graph>();
            foreach (Function function in Functions)
            {
                InterpretationContext context = new InterpretationContext(function);
                if (function.FormalParameters.Count == 1)
                {
                    Parameter parameter = (Parameter) function.FormalParameters[0];
                    Graph graph = function.createGraph(context, parameter, null);
                    if (graph != null)
                    {
                        expectedEndX = Math.Max(expectedEndX, graph.ExpectedEndX());
                        graphs.Add(function, graph);
                    }
                }
            }

            double expectedEndY = 0;
            Dictionary<Function, Surface> surfaces = new Dictionary<Function, Surface>();
            foreach (Function function in Functions)
            {
                InterpretationContext context = new InterpretationContext(function);
                if (function.FormalParameters.Count == 2)
                {
                    Surface surface = function.createSurface(context, null);
                    if (surface != null)
                    {
                        expectedEndX = Math.Max(expectedEndX, surface.ExpectedEndX());
                        expectedEndY = Math.Max(expectedEndY, surface.ExpectedEndY());
                        surfaces.Add(function, surface);
                    }
                }
            }

            try
            {
                int maxX = Int32.Parse(maximumValueTextBox.Text);
                expectedEndX = Math.Min(expectedEndX, maxX);
            }
            catch (Exception)
            {
            }

            int i = 0;
            // Creates the graphs
            foreach (KeyValuePair<Function, Graph> pair in graphs)
            {
                Function function = pair.Key;
                Graph graph = pair.Value;

                if (graph != null)
                {
                    EfsProfileFunction efsProfileFunction = new EfsProfileFunction(graph);
                    GraphVisualiser.AddGraph(new EfsProfileFunctionGraph(GraphVisualiser, efsProfileFunction, function.FullName));

                    if (name == null)
                    {
                        name = function.Name;
                    }
                }
                i += 1;
            }

            // Creates the surfaces
            /*foreach (KeyValuePair<Function, Surface> pair in surfaces)
            {
                Function function = pair.Key;
                Surface surface = pair.Value;

                if (surface != null)
                {
                    AccelerationSpeedDistanceSurface curve = surface.createAccelerationSpeedDistanceSurface(
                        expectedEndX, expectedEndY);
                    display.AddCurve(curve, function.FullName);
                    if (name == null)
                    {
                        name = function.Name;
                    }
                }
            }*/

            if (name != null)
            {
                try
                {
                    double val = double.Parse(minimumValueTextBox.Text);
                    GraphVisualiser.SetMinX(val);
                }
                catch (Exception)
                {
                }

                try
                {
                    double val = double.Parse(maximumValueTextBox.Text);
                    GraphVisualiser.SetMaxX(val);
                }
                catch (Exception)
                {
                }

                GraphVisualiser.DrawGraphs(expectedEndX);
            }
        }

        public void RefreshAfterStep()
        {
            Refresh();
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Handles the zoom on the chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphVisualiser_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Chart chart = sender as Chart;
                if (chart != null)
                {
                    if (e.Location.X >= 0 && e.Location.X <= chart.Size.Width &&
                        e.Location.Y >= 0 && e.Location.Y <= chart.Size.Height)
                    {
                        GraphVisualiser.HandleZoom(sender, e, 0);
                    }
                }
            }
            catch
            {
            }
        }
    }
}