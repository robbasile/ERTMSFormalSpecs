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
using DataDictionary;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using Utils;
using ModelElement = DataDictionary.ModelElement;

namespace GUI.UsageView
{
    public class UsageTreeNode : ModelElementTreeNode<ModelElement>
    {
        /// <summary>
        ///     The usage for which this tree node is built
        /// </summary>
        public Usage Usage { get; private set; }

        /// <summary>
        ///     The folder element for which this node is built
        /// </summary>
        public IModelElement FolderElement { get; set; }

        private class UsageEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new UsageEditor();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="usage"></param>
        /// <param name="buildSubNodes"></param>
        public UsageTreeNode(Usage usage, bool buildSubNodes)
            : base(usage.User, buildSubNodes, usage.DisplayName())
        {
            Usage = usage;
            ToolTipText = usage.User.FullName;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="buildSubNodes"></param>
        public UsageTreeNode(IModelElement folder, bool buildSubNodes)
            : base(null, buildSubNodes, folder.Name)
        {
            Usage = null;
            FolderElement = folder;
            ToolTipText = folder.FullName;

            NameSpace nameSpace = EnclosingFinder<NameSpace>.find(folder, true);
            if (nameSpace != null)
            {
                ChangeImageIndex(BaseTreeView.ModelImageIndex);
            }
            else
            {
                ChangeImageIndex(BaseTreeView.TestImageIndex);
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="buildSubNodes"></param>
        public UsageTreeNode(string name, bool buildSubNodes)
            : base(null, buildSubNodes, name, true)
        {
        }

        /// <summary>
        ///     Sets the image index for this node
        /// </summary>
        /// <param name="isFolder">Indicates whether this item represents a folder</param>
        public override void SetImageIndex(bool isFolder)
        {
            if (Usage != null)
            {
                switch (Usage.Mode)
                {
                    case Usage.ModeEnum.Read:
                        ChangeImageIndex(BaseTreeView.ReadAccessImageIndex);
                        break;

                    case Usage.ModeEnum.ReadAndWrite:
                    case Usage.ModeEnum.Write:
                        ChangeImageIndex(BaseTreeView.WriteAccessImageIndex);
                        break;

                    case Usage.ModeEnum.Call:
                        ChangeImageIndex(BaseTreeView.CallImageIndex);
                        break;

                    case Usage.ModeEnum.Type:
                        ChangeImageIndex(BaseTreeView.TypeImageIndex);
                        break;

                    case Usage.ModeEnum.Parameter:
                        ChangeImageIndex(BaseTreeView.ParameterImageIndex);
                        break;

                    case Usage.ModeEnum.Interface:
                        ChangeImageIndex(BaseTreeView.InterfaceImageIndex);
                        break;

                    case Usage.ModeEnum.Redefines:
                        ChangeImageIndex(BaseTreeView.RedefinesImageIndex);
                        break;
                }
            }
            else if (@"Test" == Text)
            {
                ChangeImageIndex(BaseTreeView.TestImageIndex);
            }
            else if (@"Model" == Text)
            {
                ChangeImageIndex(BaseTreeView.ModelImageIndex);
            }
            else if (FolderElement != null)
            {
                base.SetImageIndex(true);
            }
            else
            {
                base.SetImageIndex(isFolder);
            }
        }

        /// <summary>
        ///     Builds the sub nodes of this node if required
        /// </summary>
        /// <param name="modifiedElement">The element that has been modified</param>
        public override void BuildOrRefreshSubNodes(IModelElement modifiedElement)
        {
            // Subnodes are already built and should not be refreshed
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            if (Item != null)
            {
                retVal.Add(new MenuItem("Select", SelectHandler));
            }

            return retVal;
        }

        private void SelectHandler(object sender, EventArgs e)
        {
            if (Item != null)
            {
                EfsSystem.Instance.Context.SelectElement(Item, TreeView, Context.SelectionCriteria.LeftClick);
            }
        }

        public override void DoubleClickHandler()
        {
            if (Item != null)
            {
                EfsSystem.Instance.Context.SelectElement(Item, TreeView, Context.SelectionCriteria.DoubleClick);
            }
        }
    }
}