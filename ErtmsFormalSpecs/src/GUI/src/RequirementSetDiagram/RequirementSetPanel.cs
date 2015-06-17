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
using RequirementSet = DataDictionary.Specification.RequirementSet;
using RequirementSetDependancy = DataDictionary.Specification.RequirementSetDependancy;

namespace GUI.RequirementSetDiagram
{
    public class RequirementSetPanel : BoxArrowPanel<RequirementSet, RequirementSetDependancy>
    {
        private ToolStripMenuItem addRequirementSetMenuItem;
        private ToolStripMenuItem addDependanceMenuItem;
        private ToolStripMenuItem selectParagraphsMenuItem;
        private ToolStripMenuItem selectRequirementsWhichDoNotBelongMenuItem;
        private ToolStripMenuItem selectNotImplementedRequirementsMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem deleteMenuItem;

        /// <summary>
        ///     Initializes the start menu
        /// </summary>
        public override void InitializeStartMenu()
        {
            base.InitializeStartMenu();

            addRequirementSetMenuItem = new ToolStripMenuItem();
            addDependanceMenuItem = new ToolStripMenuItem();
            selectParagraphsMenuItem = new ToolStripMenuItem();
            selectRequirementsWhichDoNotBelongMenuItem = new ToolStripMenuItem();
            selectNotImplementedRequirementsMenuItem = new ToolStripMenuItem();
            toolStripSeparator = new ToolStripSeparator();
            deleteMenuItem = new ToolStripMenuItem();
            // 
            // addRequirementSetMenuItem
            // 
            addRequirementSetMenuItem.Name = "addRequirementSetMenuItem";
            addRequirementSetMenuItem.Size = new Size(161, 22);
            addRequirementSetMenuItem.Text = "Add requirement set";
            addRequirementSetMenuItem.Click += new EventHandler(addBoxMenuItem_Click);
            // 
            // addDependanceMenuItem
            // 
            addDependanceMenuItem.Name = "addDependanceMenuItem";
            addDependanceMenuItem.Size = new Size(161, 22);
            addDependanceMenuItem.Text = "Add dependance";
            addDependanceMenuItem.Click += new EventHandler(addArrowMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator.Name = "toolStripSeparator1";
            toolStripSeparator.Size = new Size(158, 6);
            // 
            // selectParagraphsMenuItem
            // 
            selectParagraphsMenuItem.Name = "selectParagraphsMenuItem";
            selectParagraphsMenuItem.Size = new Size(161, 22);
            selectParagraphsMenuItem.Text = "Select paragraphs";
            selectParagraphsMenuItem.Click += new EventHandler(selectRequirements_Click);
            // 
            // selectRequirementsWhichDoNotBelongMenuItem
            // 
            selectRequirementsWhichDoNotBelongMenuItem.Name = "selectRequirementsWhichDoNotBelongMenuItem";
            selectRequirementsWhichDoNotBelongMenuItem.Size = new Size(161, 22);
            selectRequirementsWhichDoNotBelongMenuItem.Text =
                "Select requirements which do not belong to requirement set";
            selectRequirementsWhichDoNotBelongMenuItem.Click +=
                new EventHandler(selectRequirementsWhichDoNotBelongMenuItem_Click);
            // 
            // selectNotImplementedRequirements
            // 
            selectNotImplementedRequirementsMenuItem.Name = "selectNotImplementedRequirementsMenuItem";
            selectNotImplementedRequirementsMenuItem.Size = new Size(161, 22);
            selectNotImplementedRequirementsMenuItem.Text = "Select not implemented requirements";
            selectNotImplementedRequirementsMenuItem.Click +=
                new EventHandler(selectNotImplementedRequirementsMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            deleteMenuItem.Name = "toolStripMenuItem1";
            deleteMenuItem.Size = new Size(153, 22);
            deleteMenuItem.Text = "Delete selected";
            deleteMenuItem.Click += new EventHandler(deleteMenuItem1_Click);

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                addRequirementSetMenuItem,
                addDependanceMenuItem,
                toolStripSeparator,
                selectParagraphsMenuItem,
                selectRequirementsWhichDoNotBelongMenuItem,
                selectNotImplementedRequirementsMenuItem,
                toolStripSeparator,
                deleteMenuItem
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
            : base()
        {
            Init();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public RequirementSetPanel(IContainer container)
            : base()
        {
            container.Add(this);

            Init();
        }

        /// <summary>
        ///     Method used to create a box
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override BoxControl<RequirementSet, RequirementSetDependancy> createBox(RequirementSet model)
        {
            BoxControl<RequirementSet, RequirementSetDependancy> retVal = new RequirementSetControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        ///     Method used to create an arrow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ArrowControl<RequirementSet, RequirementSetDependancy> createArrow(
            RequirementSetDependancy model)
        {
            ArrowControl<RequirementSet, RequirementSetDependancy> retVal = new RequirementSetDependancyControl();
            retVal.Model = model;

            return retVal;
        }

        /// <summary>
        ///     The enclosing element, which holds requirement setsEFSSystem for which this panel is built
        /// </summary>
        public IHoldsRequirementSets Enclosing { get; set; }

        /// <summary>
        ///     Provides the boxes that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<RequirementSet> getBoxes()
        {
            List<RequirementSet> retVal = Enclosing.RequirementSets;

            return retVal;
        }

        /// <summary>
        ///     Provides the arrows that need be displayed
        /// </summary>
        /// <returns></returns>
        public override List<RequirementSetDependancy> getArrows()
        {
            List<RequirementSetDependancy> retVal = new List<RequirementSetDependancy>();

            foreach (RequirementSet requirementSet in Enclosing.RequirementSets)
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
            requirementSet.Name = "<set " + (Enclosing.RequirementSets.Count + 1) + ">";

            Enclosing.AddRequirementSet(requirementSet);
            RefreshControl();
        }

        private void addArrowMenuItem_Click(object sender, EventArgs e)
        {
            if (Enclosing.RequirementSets.Count > 1)
            {
                RequirementSet source = null;
                RequirementSet target = null;
                RequirementSetControl sourceControl = Selected as RequirementSetControl;
                if (sourceControl != null)
                {
                    source = sourceControl.Model;
                    target = Enclosing.RequirementSets[0];
                    if (target == source)
                    {
                        target = Enclosing.RequirementSets[1];
                    }
                }
                else
                {
                    source = Enclosing.RequirementSets[0];
                    target = Enclosing.RequirementSets[1];
                }

                RequirementSetDependancy dependancy =
                    (RequirementSetDependancy) acceptor.getFactory().createRequirementSetDependancy();
                dependancy.setTarget(target.Guid);
                source.appendDependancies(dependancy);

                RefreshControl();
                Refresh();

                ArrowControl<RequirementSet, RequirementSetDependancy> control = getArrowControl(dependancy);
                Select(control, false);
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

            BoxControl<RequirementSet, RequirementSetDependancy> box =
                Selected as BoxControl<RequirementSet, RequirementSetDependancy>;
            ArrowControl<RequirementSet, RequirementSetDependancy> arrow =
                Selected as ArrowControl<RequirementSet, RequirementSetDependancy>;

            if (box != null)
            {
                model = box.Model;
            }
            else if (arrow != null)
            {
                model = arrow.Model;
            }
            model.Delete();

            Select(null, false);
            RefreshControl();
            Refresh();
        }
    }
}