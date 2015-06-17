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
using System.Drawing.Design;
using System.Windows.Forms;
using DataDictionary;
using GUI.Converters;
using GUI.SpecificationView;
using Window = GUI.DataDictionaryView.Window;

namespace GUI
{
    public class ReqRefTreeNode : ModelElementTreeNode<ReqRef>
    {
        /// <summary>
        ///     Indicates that this req ref can be removed from its model
        /// </summary>
        private bool CanBeDeleted { get; set; }

        public class InternalTracesConverter : TracesConverter
        {
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return GetValues(((ItemEditor) context.Instance).Item);
            }
        }

        private class ItemEditor : Editor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            [Category("Description"), TypeConverter(typeof (InternalTracesConverter))]
            public string Name
            {
                get { return Item.Name; }
            }

            [Category("Description")]
            [Editor(typeof (CommentableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (CommentableUITypeConverter))]
            public ReqRef Comment
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public ReqRefTreeNode(ReqRef item, bool buildSubNodes, bool canBeDeleted, string name = null)
            : base(item, buildSubNodes, name)
        {
            CanBeDeleted = canBeDeleted;
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        public override void DoubleClickHandler()
        {
            base.DoubleClickHandler();

            MainWindow mainWindow = GUIUtils.MDIWindow;

            foreach (Form form in mainWindow.SubWindows)
            {
                {
                    Window window = form as Window;
                    if (window != null)
                    {
                        window.TreeView.Select(Item.Model);
                    }
                }

                {
                    SpecificationView.Window window = form as SpecificationView.Window;
                    if (window != null)
                    {
                        window.TreeView.Select(Item.Paragraph);
                    }
                }

                {
                    TestRunnerView.Window window = form as TestRunnerView.Window;
                    if (window != null)
                    {
                        window.TreeView.Select(Item.Model);
                    }
                }

                {
                    TranslationRules.Window window = form as TranslationRules.Window;
                    if (window != null)
                    {
                        window.TreeView.Select(Item.Model);
                    }
                }
            }
        }

        /// <summary>
        ///     Handles the selection of the requirement
        /// </summary>
        public void SelectHandler(object sender, EventArgs args)
        {
            DoubleClickHandler();
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Select", new EventHandler(SelectHandler)));

            if (CanBeDeleted)
            {
                retVal.Add(new MenuItem("-"));
                retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            }

            return retVal;
        }


        /// <summary>
        ///     Accepts a drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            base.AcceptDrop(SourceNode);

            if (SourceNode is ParagraphTreeNode)
            {
                ParagraphTreeNode paragraph = SourceNode as ParagraphTreeNode;

                Item.Name = paragraph.Item.FullId;
                RefreshNode();
            }
        }
    }
}