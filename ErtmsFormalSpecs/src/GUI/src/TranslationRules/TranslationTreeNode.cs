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
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Generated;
using GUI.TestRunnerView;
using Dictionary = DataDictionary.Dictionary;
using SourceText = DataDictionary.Tests.Translations.SourceText;
using SubStep = DataDictionary.Tests.SubStep;
using Translation = DataDictionary.Tests.Translations.Translation;

namespace GUI.TranslationRules
{
    public class TranslationTreeNode : ReferencesParagraphTreeNode<Translation>
    {
        private class ItemEditor : ReferencesParagraphEditor
        {
            /// <summary>
            ///     The step name
            /// </summary>
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public bool Implemented
            {
                get { return Item.getImplemented(); }
                set { Item.setImplemented(value); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public TranslationTreeNode(Translation item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            subNodes.Add(new SourceTextsTreeNode(Item, recursive));
            foreach (SubStep subStep in Item.SubSteps)
            {
                subNodes.Add(new SubStepTreeNode(subStep, recursive));
            }
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddSourceHandler(object sender, EventArgs args)
        {
            SourceText sourceText = (SourceText) acceptor.getFactory().createSourceText();
            sourceText.Name = "<SourceText " + (Item.SourceTexts.Count + 1) + ">";
            Item.appendSourceTexts(sourceText);
        }

        /// <summary>
        ///     Adds a step after this one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddSubStepHandler(object sender, EventArgs args)
        {
            SubStep subStep = (SubStep) acceptor.getFactory().createSubStep();
            subStep.Name = "Sub-step" + Nodes.Count;
            subStep.Enclosing = Item;
            Item.appendSubSteps(subStep);
        }

        /// <summary>
        ///     Finds all steps that are translated using a specific translation rule
        /// </summary>
        private class MarkUsageVisitor : Visitor
        {
            /// <summary>
            ///     The translation to be found
            /// </summary>
            private Translation Translation { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="translation"></param>
            public MarkUsageVisitor(Translation translation)
            {
                Translation = translation;
            }

            public override void visit(Step obj, bool visitSubNodes)
            {
                DataDictionary.Tests.Step step = (DataDictionary.Tests.Step) obj;

                if (Translation ==
                    Translation.TranslationDictionary.findTranslation(step.getDescription(), step.Comment))
                {
                    step.AddInfo("Translation " + Translation.Name + " used");
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Marks all steps that use this translation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void MarkUsageHandler(object sender, EventArgs args)
        {
            MarkingHistory.PerformMark(() =>
            {
                MarkUsageVisitor finder = new MarkUsageVisitor(Item);
                foreach (Dictionary dictionary in EfsSystem.Instance.Dictionaries)
                {
                    finder.visit(dictionary);
                }
            });
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add source text", AddSourceHandler),
                new MenuItem("Add sub-step", AddSubStepHandler),
                new MenuItem("-"),
                new MenuItem("Mark usages", MarkUsageHandler),
                new MenuItem("-"),
                new MenuItem("Delete", DeleteHandler)
            };

            return retVal;
        }

        /// <summary>
        ///     Handles drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);
            AcceptDropForTranslation(this, sourceNode);
        }

        /// <summary>
        ///     Accepts the drop event
        /// </summary>
        /// <param name="translationTreeNode"></param>
        /// <param name="sourceNode"></param>
        public static void AcceptDropForTranslation(TranslationTreeNode translationTreeNode, BaseTreeNode sourceNode)
        {
            if (sourceNode is SourceTextTreeNode)
            {
                SourceTextTreeNode text = sourceNode as SourceTextTreeNode;

                SourceText otherText = (SourceText) text.Item.Duplicate();
                translationTreeNode.Item.appendSourceTexts(otherText);
                text.Delete();
            }
            else if (sourceNode is StepTreeNode)
            {
                StepTreeNode step = sourceNode as StepTreeNode;

                if (string.IsNullOrEmpty(step.Item.getDescription()))
                {
                    MessageBox.Show("Step has no description and cannot be automatically translated",
                        "No description available", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    translationTreeNode.Item.appendSourceTexts(step.Item.createSourceText());
                }
            }
        }
    }
}