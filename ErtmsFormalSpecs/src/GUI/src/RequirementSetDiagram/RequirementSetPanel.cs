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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using DataDictionary.Specification;
using GUI.BoxArrowDiagram;
using Utils;
using Paragraph = DataDictionary.Specification.Paragraph;
using RequirementSet = DataDictionary.Specification.RequirementSet;
using RequirementSetDependancy = DataDictionary.Specification.RequirementSetDependancy;

namespace GUI.RequirementSetDiagram
{
    public class RequirementSetPanel : BoxArrowPanel<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy>
    {
        private ToolStripMenuItem _addRequirementSetMenuItem;
        private ToolStripMenuItem _addDependanceMenuItem;
        private ToolStripMenuItem _selectParagraphsMenuItem;
        private ToolStripMenuItem _selectRequirementsWhichDoNotBelongMenuItem;
        private ToolStripMenuItem _selectNotImplementedRequirementsMenuItem;
        private ToolStripSeparator _toolStripSeparator;
        private ToolStripMenuItem _deleteMenuItem;

        /// <summary>
        ///     Initializes the start menu
        /// </summary>
        public override void InitializeStartMenu()
        {
            base.InitializeStartMenu();

            _addRequirementSetMenuItem = new ToolStripMenuItem();
            _addDependanceMenuItem = new ToolStripMenuItem();
            _selectParagraphsMenuItem = new ToolStripMenuItem();
            _selectRequirementsWhichDoNotBelongMenuItem = new ToolStripMenuItem();
            _selectNotImplementedRequirementsMenuItem = new ToolStripMenuItem();
            _toolStripSeparator = new ToolStripSeparator();
            _deleteMenuItem = new ToolStripMenuItem();
            // 
            // addRequirementSetMenuItem
            // 
            _addRequirementSetMenuItem.Name = "addRequirementSetMenuItem";
            _addRequirementSetMenuItem.Size = new Size(161, 22);
            _addRequirementSetMenuItem.Text = "Add requirement set";
            _addRequirementSetMenuItem.Click += addBoxMenuItem_Click;
            // 
            // addDependanceMenuItem
            // 
            _addDependanceMenuItem.Name = "addDependanceMenuItem";
            _addDependanceMenuItem.Size = new Size(161, 22);
            _addDependanceMenuItem.Text = "Add dependancy";
            _addDependanceMenuItem.Click += addArrowMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            _toolStripSeparator.Name = "toolStripSeparator1";
            _toolStripSeparator.Size = new Size(158, 6);
            // 
            // selectParagraphsMenuItem
            // 
            _selectParagraphsMenuItem.Name = "selectParagraphsMenuItem";
            _selectParagraphsMenuItem.Size = new Size(161, 22);
            _selectParagraphsMenuItem.Text = "Select paragraphs";
            _selectParagraphsMenuItem.Click += selectRequirements_Click;
            // 
            // selectRequirementsWhichDoNotBelongMenuItem
            // 
            _selectRequirementsWhichDoNotBelongMenuItem.Name = "selectRequirementsWhichDoNotBelongMenuItem";
            _selectRequirementsWhichDoNotBelongMenuItem.Size = new Size(161, 22);
            _selectRequirementsWhichDoNotBelongMenuItem.Text =
                "Select requirements which do not belong to requirement set";
            _selectRequirementsWhichDoNotBelongMenuItem.Click +=
                selectRequirementsWhichDoNotBelongMenuItem_Click;
            // 
            // selectNotImplementedRequirements
            // 
            _selectNotImplementedRequirementsMenuItem.Name = "selectNotImplementedRequirementsMenuItem";
            _selectNotImplementedRequirementsMenuItem.Size = new Size(161, 22);
            _selectNotImplementedRequirementsMenuItem.Text = "Select not implemented requirements";
            _selectNotImplementedRequirementsMenuItem.Click +=
                selectNotImplementedRequirementsMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            _deleteMenuItem.Name = "toolStripMenuItem1";
            _deleteMenuItem.Size = new Size(153, 22);
            _deleteMenuItem.Text = "Delete selected";
            _deleteMenuItem.Click += deleteMenuItem1_Click;

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                _addRequirementSetMenuItem,
                _addDependanceMenuItem,
                _toolStripSeparator,
                _selectParagraphsMenuItem,
                _selectRequirementsWhichDoNotBelongMenuItem,
                _selectNotImplementedRequirementsMenuItem,
                _toolStripSeparator,
                _deleteMenuItem
            });
        }

        /// <summary>
        ///     Initialises the component
        /// </summary>
        private void Init()
        {
            InitializeStartMenu();
            DefaultBoxSize = new Size(150, 75);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public RequirementSetPanel()
        {
            Init();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public RequirementSetPanel(IContainer container)
        {
            container.Add(this);

            Init();
        }

        /// <summary>
        /// Metrics related to all requirement sets 
        /// </summary>
        public Dictionary<RequirementSet, Paragraph.ParagraphSetMetrics> Metrics { get; set; }

        /// <summary>
        /// Computes the metrics for all displayed requirement sets
        /// </summary>
        public override IHoldsRequirementSets Model
        {
            set
            {
                base.Model = value; 

                Metrics = new Dictionary<RequirementSet, Paragraph.ParagraphSetMetrics>();
                foreach (RequirementSet requirementSet in Model.RequirementSets)
                {
                    List<Paragraph> paragraphs = new List<Paragraph>();
                    requirementSet.GetParagraphs(paragraphs);
                    Metrics.Add(requirementSet, Paragraph.CreateParagraphSetMetrics(paragraphs));
                }
            }
        }

        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> CreateBox(RequirementSet model)
        {
            var retVal = new RequirementSetControl {Model = model};

            return retVal;
        }

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> CreateArrow(
            RequirementSetDependancy model)
        {
            ArrowControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> retVal = new RequirementSetDependancyControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        ///     Provides the boxes that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<RequirementSet> GetBoxes()
        {
            List<RequirementSet> retVal = Model.RequirementSets;

            return retVal;
        }

        /// <summary>
        ///     Provides the arrows that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<RequirementSetDependancy> GetArrows()
        {
            List<RequirementSetDependancy> retVal = new List<RequirementSetDependancy>();

            foreach (RequirementSet requirementSet in Model.RequirementSets)
            {
                foreach (RequirementSetDependancy dependance in requirementSet.Dependancies)
                {
                    retVal.Add(dependance);
                }
            }

            return retVal;
        }

        private void addBoxMenuItem_Click(object sender, EventArgs e)
        {
            RequirementSet requirementSet = (RequirementSet) acceptor.getFactory().createRequirementSet();
            requirementSet.Name = "<set " + (Model.RequirementSets.Count + 1) + ">";

            Model.AddRequirementSet(requirementSet);
            RefreshControl();
        }

        private void addArrowMenuItem_Click(object sender, EventArgs e)
        {
            if (Model.RequirementSets.Count > 1)
            {
                RequirementSet source;
                RequirementSet target;
                RequirementSetControl sourceControl = Selected as RequirementSetControl;
                if (sourceControl != null)
                {
                    source = sourceControl.Model;
                    target = Model.RequirementSets[0];
                    if (target == source)
                    {
                        target = Model.RequirementSets[1];
                    }
                }
                else
                {
                    source = Model.RequirementSets[0];
                    target = Model.RequirementSets[1];
                }

                RequirementSetDependancy dependancy =
                    (RequirementSetDependancy) acceptor.getFactory().createRequirementSetDependancy();
                dependancy.setTarget(target.Guid);
                source.appendDependancies(dependancy);

                EFSSystem.INSTANCE.Context.SelectElement(dependancy, this, Context.SelectionCriteria.LeftClick);
            }
        }

        private void selectRequirements_Click(object sender, EventArgs e)
        {
            RequirementSetControl control = Selected as RequirementSetControl;

            if (control != null)
            {
                EFSSystem.INSTANCE.MarkRequirementsForRequirementSet(control.Model);
            }
        }

        private void selectRequirementsWhichDoNotBelongMenuItem_Click(object sender, EventArgs e)
        {
            RequirementSetControl control = Selected as RequirementSetControl;

            if (control != null)
            {
                EFSSystem.INSTANCE.MarkRequirementsWhichDoNotBelongToRequirementSet(control.Model);
            }
        }

        private void selectNotImplementedRequirementsMenuItem_Click(object sender, EventArgs e)
        {
            RequirementSetControl control = Selected as RequirementSetControl;

            if (control != null)
            {
                EFSSystem.INSTANCE.MarkNotImplementedRequirements(control.Model);
            }
        }


        private void deleteMenuItem1_Click(object sender, EventArgs e)
        {
            IModelElement model = null;

            var box = Selected as BoxControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy>;
            var arrow = Selected as ArrowControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy>;

            if (box != null)
            {
                model = box.Model;
            }
            else if (arrow != null)
            {
                model = arrow.Model;
            }

            if (model != null)
            {
                model.Delete();
            }
        }
    }
}