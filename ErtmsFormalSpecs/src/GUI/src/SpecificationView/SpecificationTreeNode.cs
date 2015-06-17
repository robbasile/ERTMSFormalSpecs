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
using DataDictionary.Generated;
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
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            /// <summary>
            ///     The specification document name
            /// </summary>
            [Category("Description")]
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
        public SpecificationTreeNode(Specification item, bool buildSubNodes)
            : base(item, buildSubNodes, null, true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="buildSubNodes">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(bool buildSubNodes)
        {
            base.BuildSubNodes(buildSubNodes);

            foreach (Chapter chapter in Item.Chapters)
            {
                Nodes.Add(new ChapterTreeNode(chapter, buildSubNodes));
            }

            SortSubNodes();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }


        /// <summary>
        ///     Adds a new chapter to this specification
        /// </summary>
        /// <param name="chapter"></param>
        public void AddChapter(Chapter chapter)
        {
            Item.appendChapters(chapter);
            Nodes.Add(new ChapterTreeNode(chapter, true));
            RefreshNode();
        }

        public void AddChapterHandler(object sender, EventArgs args)
        {
            Chapter chapter = (Chapter) acceptor.getFactory().createChapter();
            chapter.setId("" + (Item.countChapters() + 1));
            AddChapter(chapter);
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

            retVal.Add(new MenuItem("Add chapter", new EventHandler(AddChapterHandler)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Import new specification release",
                new EventHandler(ImportNewSpecificationReleaseHandler)));

            MenuItem recursiveActions = retVal.Find(x => x.Text.StartsWith("Recursive"));
            if (recursiveActions != null)
            {
                recursiveActions.MenuItems.Add(new MenuItem("-"));
                recursiveActions.MenuItems.Add(new MenuItem("Remove requirement sets",
                    new EventHandler(RemoveRequirementSets)));
            }

            return retVal;
        }

        /// <summary>
        ///     Update counts according to the selected chapter
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            Window window = BaseForm as Window;
            if (window != null)
            {
                GUIUtils.MDIWindow.SetCoverageStatus(Item);
            }
        }


        /// ------------------------------------------------------
        /// IMPORT SPEC OPERATIONS
        /// ------------------------------------------------------
        private void ImportNewSpecificationReleaseHandler(object sender, EventArgs args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open original specification file";
            openFileDialog.Filter = "RTF Files (*.rtf)|*.rtf|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(GUIUtils.MDIWindow) == DialogResult.OK)
            {
                string OriginalFileName = openFileDialog.FileName;
                openFileDialog.Title = "Open new specification file";
                openFileDialog.Filter = "RTF Files (*.rtf)|*.rtf|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog(GUIUtils.MDIWindow) == DialogResult.OK)
                {
                    string NewFileName = openFileDialog.FileName;

                    string baseFileName = createBaseFileName(OriginalFileName, NewFileName);

                    /// Perform the importation
                    Importer importer = new Importer(OriginalFileName, NewFileName, Item);
                    ProgressDialog dialog = new ProgressDialog("Opening file", importer);
                    dialog.ShowDialog();

                    /// Creates the report based on the importation result
                    DeltaImportReportHandler reportHandler = new DeltaImportReportHandler(Item.Dictionary,
                        importer.NewDocument, baseFileName);
                    dialog = new ProgressDialog("Opening file", reportHandler);
                    dialog.ShowDialog();

                    GUIUtils.MDIWindow.RefreshModel();
                }
            }
        }

        /// <summary>
        ///     Creates the base file name for the report
        /// </summary>
        /// <param name="OriginalFileName"></param>
        /// <param name="NewFileName"></param>
        /// <returns></returns>
        private string createBaseFileName(string OriginalFileName, string NewFileName)
        {
            string baseFileName = "";
            for (int i = 0; i < OriginalFileName.Length && i < NewFileName.Length; i++)
            {
                if (OriginalFileName[i] == NewFileName[i])
                {
                    baseFileName += OriginalFileName[i];
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