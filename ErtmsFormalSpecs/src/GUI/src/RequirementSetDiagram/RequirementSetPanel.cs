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
        protected override ContextMenu BuildContextMenu(GraphicElement element)
        {
            ContextMenu retVal = base.BuildContextMenu(element);

            if (element != null)
            {
                retVal = new ContextMenu();

                // 
                // addRequirementSetMenuItem
                // 
                MenuItem addRequirementSetMenuItem = new MenuItem
                {
                    Name = "addRequirementSetMenuItem",
                    Text = "Add requirement set"
                };
                addRequirementSetMenuItem.Click += HandleAddRequirement;
                retVal.MenuItems.Add(addRequirementSetMenuItem);
                // 
                // addDependanceMenuItem
                // 
                MenuItem addDependanceMenuItem = new MenuItem
                {
                    Name = "addDependanceMenuItem",
                    Text = "Add dependancy"
                };
                addDependanceMenuItem.Click += HandleAddDependancy;
                retVal.MenuItems.Add(addDependanceMenuItem);
                // 
                // selectParagraphsMenuItem
                // 
                MenuItem selectParagraphsMenuItem = new MenuItem
                {
                    Name = "selectParagraphsMenuItem",
                    Text = "Select paragraphs"
                };
                selectParagraphsMenuItem.Click += HandleSelectParagraph;
                retVal.MenuItems.Add(selectParagraphsMenuItem);
                // 
                // selectRequirementsWhichDoNotBelongMenuItem
                // 
                MenuItem selectRequirementsWhichDoNotBelongMenuItem = new MenuItem
                {
                    Name = "selectRequirementsWhichDoNotBelongMenuItem",
                    Text = "Select requirements which do not belong to requirement set"
                };
                selectRequirementsWhichDoNotBelongMenuItem.Click +=
                    HandleSelectRequirementsWhichDoNotBelong;
                retVal.MenuItems.Add(selectRequirementsWhichDoNotBelongMenuItem);
                // 
                // selectNotImplementedRequirements
                // 
                MenuItem selectNotImplementedRequirementsMenuItem = new MenuItem
                {
                    Name = "selectNotImplementedRequirementsMenuItem",
                    Text = "Select not implemented requirements"
                };
                selectNotImplementedRequirementsMenuItem.Click +=
                    HandleSelectNotImplementedRequirements;
                retVal.MenuItems.Add(selectNotImplementedRequirementsMenuItem);
                // 
                // toolStripMenuItem1
                // 
                MenuItem deleteMenuItem = new MenuItem
                {
                    Name = "toolStripMenuItem1",
                    Text = "Delete selected"
                };
                deleteMenuItem.Click += HandleDelete;
                retVal.MenuItems.Add(deleteMenuItem);
            }

            return retVal;
        }

        /// <summary>
        ///     Initialises the component
        /// </summary>
        private void Init()
        {
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
        ///     Metrics related to all requirement sets
        /// </summary>
        public Dictionary<RequirementSet, Paragraph.ParagraphSetMetrics> Metrics { get; set; }

        /// <summary>
        ///     Computes the metrics for all displayed requirement sets
        /// </summary>
        public override IHoldsRequirementSets Model
        {
            set
            {
                base.Model = value;

                // Precompute the metrics
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
        public override BoxControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> CreateBox(
            RequirementSet model)
        {
            RequirementSetControl retVal = new RequirementSetControl(this, model);

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
            return new RequirementSetDependancyControl(this, model);
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

        private void HandleAddRequirement(object sender, EventArgs e)
        {
            RequirementSet requirementSet = (RequirementSet) acceptor.getFactory().createRequirementSet();
            requirementSet.Name = "<set " + (Model.RequirementSets.Count + 1) + ">";

            Model.AddRequirementSet(requirementSet);
            RefreshControl();
        }

        private void HandleAddDependancy(object sender, EventArgs e)
        {
            if (Model.RequirementSets.Count > 1)
            {
                RequirementSet source;
                RequirementSet target;
                RequirementSetControl sourceControl = Selected as RequirementSetControl;
                if (sourceControl != null)
                {
                    source = sourceControl.TypedModel;
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

                EfsSystem.Instance.Context.SelectElement(dependancy, this, Context.SelectionCriteria.LeftClick);
            }
        }

        private void HandleSelectParagraph(object sender, EventArgs e)
        {
            RequirementSetControl control = Selected as RequirementSetControl;

            if (control != null)
            {
                EfsSystem.Instance.MarkRequirementsForRequirementSet(control.TypedModel);
            }
        }

        private void HandleSelectRequirementsWhichDoNotBelong(object sender, EventArgs e)
        {
            RequirementSetControl control = Selected as RequirementSetControl;

            if (control != null)
            {
                EfsSystem.Instance.MarkRequirementsWhichDoNotBelongToRequirementSet(control.TypedModel);
            }
        }

        private void HandleSelectNotImplementedRequirements(object sender, EventArgs e)
        {
            RequirementSetControl control = Selected as RequirementSetControl;

            if (control != null)
            {
                EfsSystem.Instance.MarkNotImplementedRequirements(control.TypedModel);
            }
        }

        private void HandleDelete(object sender, EventArgs e)
        {
            if (Selected != null)
            {
                IModelElement model = Selected.Model as IModelElement;
                if (model != null)
                {
                    model.Delete();
                }
            }
        }

        /// <summary>
        ///     Factory for BoxEditor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override BoxEditor<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> CreateBoxEditor(
            BoxControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> control)
        {
            return new RequirementSetEditor(control);
        }

        /// <summary>
        ///     Factory for arrow editor
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected override ArrowEditor<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy>
            CreateArrowEditor(ArrowControl<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> control)
        {
            return new TransitionEditor(control);
        }
    }
}