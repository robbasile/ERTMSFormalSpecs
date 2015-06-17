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
using DataDictionary.Generated;
using Namable = DataDictionary.Namable;
using Shortcut = DataDictionary.Shortcuts.Shortcut;
using ShortcutFolder = DataDictionary.Shortcuts.ShortcutFolder;

namespace GUI.Shortcuts
{
    public class ShortcutFolderTreeNode : ModelElementTreeNode<ShortcutFolder>
    {
        private class ItemEditor : NamedEditor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public ShortcutFolderTreeNode(ShortcutFolder item, bool buildSubNodes)
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

            foreach (ShortcutFolder folder in Item.Folders)
            {
                Nodes.Add(new ShortcutFolderTreeNode(folder, buildSubNodes));
            }

            foreach (Shortcut shortcut in Item.Shortcuts)
            {
                Nodes.Add(new ShortcutTreeNode(shortcut, buildSubNodes));
            }
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
        ///     Creates a new folderTreeNode
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public ShortcutFolderTreeNode createFolder(ShortcutFolder folder)
        {
            ShortcutFolderTreeNode retVal;

            Item.appendFolders(folder);
            retVal = new ShortcutFolderTreeNode(folder, true);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        public void AddFolderHandler(object sender, EventArgs args)
        {
            ShortcutFolder folder = (ShortcutFolder) acceptor.getFactory().createShortcutFolder();
            folder.Name = "<Folder" + (Item.Folders.Count + 1) + ">";
            AddFolder(folder);
        }

        /// <summary>
        ///     Adds a sub folder in the corresponding folder
        /// </summary>
        /// <param name="nameSpace"></param>
        public ShortcutFolderTreeNode AddFolder(ShortcutFolder folder)
        {
            Item.appendFolders(folder);
            ShortcutFolderTreeNode retVal = new ShortcutFolderTreeNode(folder, true);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        /// <summary>
        ///     Creates a new shortcut based on a namable element
        /// </summary>
        /// <param name="step"></param>
        private void createShortcut(Namable namable)
        {
            Shortcut shortcut = (Shortcut) acceptor.getFactory().createShortcut();

            createShortcut(shortcut);
        }

        /// <summary>
        ///     Creates a new shortcut
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ShortcutTreeNode createShortcut(Shortcut shortcut)
        {
            ShortcutTreeNode retVal;

            Item.appendShortcuts(shortcut);
            retVal = new ShortcutTreeNode(shortcut, true);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add folder", new EventHandler(AddFolderHandler)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Rename", new EventHandler(LabelEditHandler)));
            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));

            return retVal;
        }

        /// <summary>
        ///     Sorts the sub nodes of this node
        /// </summary>
        public override void SortSubNodes()
        {
            List<BaseTreeNode> folders = new List<BaseTreeNode>();
            List<BaseTreeNode> shortcuts = new List<BaseTreeNode>();

            foreach (BaseTreeNode node in Nodes)
            {
                if (node is ShortcutFolderTreeNode)
                {
                    folders.Add(node);
                }
                else if (node is ShortcutTreeNode)
                {
                    shortcuts.Add(node);
                }
            }
            folders.Sort();
            shortcuts.Sort();

            Nodes.Clear();

            foreach (BaseTreeNode node in folders)
            {
                Nodes.Add(node);
            }
            foreach (BaseTreeNode node in shortcuts)
            {
                Nodes.Add(node);
            }
        }

        /// <summary>
        ///     Handles drop event
        /// </summary>
        /// <param name="SourceNode"></param>
        public override void AcceptDrop(BaseTreeNode SourceNode)
        {
            if (SourceNode is ShortcutTreeNode)
            {
                ShortcutTreeNode shortcut = SourceNode as ShortcutTreeNode;

                if (shortcut.Item.Dictionary == Item.Dictionary)
                {
                    Shortcut otherShortcut = (Shortcut) shortcut.Item.Duplicate();
                    createShortcut(otherShortcut);

                    shortcut.Delete();
                }
            }
            else if (SourceNode is ShortcutFolderTreeNode)
            {
                ShortcutFolderTreeNode folder = SourceNode as ShortcutFolderTreeNode;

                if (folder.Item.Dictionary == Item.Dictionary)
                {
                    ShortcutFolder otherFolder = (ShortcutFolder) folder.Item.Duplicate();
                    createFolder(otherFolder);

                    folder.Delete();
                }
            }
            else
            {
                Namable namable = SourceNode.Model as Namable;

                Shortcut shortcut = (Shortcut) acceptor.getFactory().createShortcut();
                shortcut.CopyFrom(namable);
                createShortcut(shortcut);
            }
        }
    }
}