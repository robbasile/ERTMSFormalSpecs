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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DataDictionary.Types;
using GUI.RuleDisabling;
using GUI.TestRunnerView;
using Folder = DataDictionary.Tests.Translations.Folder;
using SourceText = DataDictionary.Tests.Translations.SourceText;
using Step = DataDictionary.Tests.Step;
using Translation = DataDictionary.Tests.Translations.Translation;

namespace GUI.TranslationRules
{
    public class FolderTreeNode : ModelElementTreeNode<Folder>, RuleDisabling.IDisablesRules<Folder>
    {
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Extension ot the tree node for handling the display of rule check disabling
        /// </summary>
        public DisablesRulesTreeNodeExtension<Folder> Disabling { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public FolderTreeNode(Folder item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
        {
            Disabling = new DisablesRulesTreeNodeExtension<Folder>(item);
        }

        /// <summary>
        /// Compares two nodes to sort them
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        private int CompareNodes(object node1, object node2)
        {
            int retVal = -1;

            FolderTreeNode folder1 = node1 as FolderTreeNode;
            FolderTreeNode folder2 = node2 as FolderTreeNode;

            TranslationTreeNode translation1 = node1 as TranslationTreeNode;
            TranslationTreeNode translation2 = node2 as TranslationTreeNode;

            if (folder1 != null && translation2 != null)
            {
                retVal = -1;
            }
            else if (folder2 != null && translation1 != null)
            {
                retVal = 1;
            }
            else if ( folder1 != null && folder2 != null )
            {
                retVal = String.CompareOrdinal(folder1.Text, folder2.Text);
            }
            else if (translation1 != null && translation2 != null)
            {
                retVal = String.CompareOrdinal(translation1.Text, translation2.Text);
            }

            return retVal;
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            Disabling.BuildSubNodes(subNodes, recursive);

            foreach (Folder folder in Item.Folders)
            {
                subNodes.Add(new FolderTreeNode(folder, recursive));
            }

            foreach (Translation translation in Item.Translations)
            {
                subNodes.Add(new TranslationTreeNode(translation, recursive));
            }

            subNodes.Sort(CompareNodes);
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddFolderHandler(object sender, EventArgs args)
        {
            Item.appendFolders(Folder.CreateDefault(Item.Folders));
        }

        /// <summary>
        ///     Creates a new translation
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public void CreateTranslation(Translation translation)
        {
            Translation existingTranslation = null;
            foreach (SourceText sourceText in translation.SourceTexts)
            {
                existingTranslation = Item.Dictionary.TranslationDictionary.FindExistingTranslation(sourceText);
                if (existingTranslation != null)
                {
                    break;
                }
            }

            if (existingTranslation != null)
            {
                DialogResult dialogResult =
                    MessageBox.Show(
                        @"Translation already exists. Do you want to create a new one (Cancel will select the existing translation) ?",
                        @"Already existing translation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.OK)
                {
                    existingTranslation = null;
                }
            }

            if (existingTranslation == null)
            {
                Item.appendTranslations(translation);
            }
        }

        /// <summary>
        ///     Creates a new translation based on a step's source text
        /// </summary>
        /// <param name="step"></param>
        private void CreateTranslation(Step step)
        {
            CreateTranslation(Translation.CreateDefault(Item.Translations, step.CreateSourceText()));
        }

        /// <summary>
        /// Adds a new translation 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AddTranslationHandler(object sender, EventArgs args)
        {
            CreateTranslation(Translation.CreateDefault(Item.Translations, null));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add folder", AddFolderHandler),
                new MenuItem("Add translation", AddTranslationHandler),
                new MenuItem("-"),
                new MenuItem("Delete", DeleteHandler)
            };

            Disabling.AddMenuItems(retVal);

            return retVal;
        }

        /// <summary>
        ///     Handles drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);
            if (sourceNode is StepTreeNode)
            {
                StepTreeNode step = sourceNode as StepTreeNode;

                CreateTranslation(step.Item);
            }
            else if (sourceNode is TranslationTreeNode)
            {
                TranslationTreeNode translation = sourceNode as TranslationTreeNode;
                Translation otherTranslation = translation.Item;
                translation.Delete();
                CreateTranslation(otherTranslation);
            }
        }
    }
}