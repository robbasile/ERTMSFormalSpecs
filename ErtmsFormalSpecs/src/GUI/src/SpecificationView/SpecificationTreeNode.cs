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
using GUIUtils;
using Importers.RtfDeltaImporter;
using Reports.Importer;
using Chapter = DataDictionary.Specification.Chapter;
using RequirementSetReference = DataDictionary.Specification.RequirementSetReference;
using Specification = DataDictionary.Specification.Specification;

namespace GUI.SpecificationView
{
    public class SpecificationTreeNode : ModelElementTreeNode<Specification>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : Editor
        {
            /// <summary>
            ///     The specification document name
            /// </summary>
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public string Document
            {
                get { return Item.Name; }
                set
                {
                    Item.Name = value;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     The specification version
            /// </summary>
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public string Version
            {
                get { return Item.Version; }
                set
                {
                    Item.Version = value;
                    RefreshNode();
                }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public SpecificationTreeNode(Specification item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
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

            foreach (Chapter chapter in Item.Chapters)
            {
                subNodes.Add(new ChapterTreeNode(chapter, recursive));
            }
            subNodes.Sort();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddChapterHandler(object sender, EventArgs args)
        {
            Item.appendChapters(Chapter.CreateDefault(Item.Chapters));
        }

        /// <summary>
        ///     Recursively marks all model elements as verified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RemoveRequirementSets(object sender, EventArgs e)
        {
            RequirementSetReference.RemoveReferencesVisitor remover =
                new RequirementSetReference.RemoveReferencesVisitor();
            remover.visit(Item);
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = base.GetMenuItems();

            retVal.Add(new MenuItem("Add chapter", AddChapterHandler));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Import new specification release", ImportNewSpecificationReleaseHandler));

            MenuItem recursiveActions = retVal.Find(x => x.Text.StartsWith("Recursive"));
            if (recursiveActions != null)
            {
                recursiveActions.MenuItems.Add(new MenuItem("-"));
                recursiveActions.MenuItems.Add(new MenuItem("Remove requirement sets", RemoveRequirementSets));
            }

            return retVal;
        }

        /// ------------------------------------------------------
        /// IMPORT SPEC OPERATIONS
        /// ------------------------------------------------------
        private void ImportNewSpecificationReleaseHandler(object sender, EventArgs args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open original specification file",
                Filter = "RTF Files (*.rtf)|*.rtf|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog(GuiUtils.MdiWindow) == DialogResult.OK)
            {
                string originalFileName = openFileDialog.FileName;
                openFileDialog.Title = "Open new specification file";
                openFileDialog.Filter = "RTF Files (*.rtf)|*.rtf|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog(GuiUtils.MdiWindow) == DialogResult.OK)
                {
                    string newFileName = openFileDialog.FileName;
                    string baseFileName = createBaseFileName(originalFileName, newFileName);

                    // Perform the importation
                    Importer importer = new Importer(originalFileName, newFileName, Item);
                    ProgressDialog dialog = new ProgressDialog("Opening file", importer);
                    dialog.ShowDialog();

                    // Creates the report based on the importation result
                    DeltaImportReportHandler reportHandler = new DeltaImportReportHandler(Item.Dictionary,
                        importer.NewDocument, baseFileName);
                    dialog = new ProgressDialog("Opening file", reportHandler);
                    dialog.ShowDialog();
                }
            }
        }

        /// <summary>
        ///     Creates the base file name for the report
        /// </summary>
        /// <param name="originalFileName"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        public string createBaseFileName(string originalFileName, string newFileName)
        {
            string baseFileName = "";
            for (int i = 0; i < originalFileName.Length && i < newFileName.Length; i++)
            {
                if (originalFileName[i] == newFileName[i])
                {
                    baseFileName += originalFileName[i];
                }
                else
                {
                    break;
                }
            }
            if (baseFileName.IndexOf("\\") > 0)
            {
                baseFileName = baseFileName.Substring(baseFileName.LastIndexOf("\\") + 1);
            }

            if (baseFileName.IndexOf("v") > 0)
            {
                baseFileName = baseFileName.Substring(0, baseFileName.LastIndexOf("v"));
            }

            return baseFileName;
        }
    }
}