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
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public ShortcutFolderTreeNode(ShortcutFolder item, bool buildSubNodes)
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

            foreach (ShortcutFolder folder in Item.Folders)
            {
                subNodes.Add(new ShortcutFolderTreeNode(folder, recursive));
            }
            foreach (Shortcut shortcut in Item.Shortcuts)
            {
                subNodes.Add(new ShortcutTreeNode(shortcut, recursive));
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

        public void AddFolderHandler(object sender, EventArgs args)
        {
            ShortcutFolder folder = (ShortcutFolder) acceptor.getFactory().createShortcutFolder();
            folder.Name = "<Folder" + (Item.Folders.Count + 1) + ">";
            Item.appendFolders(folder);
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
                new MenuItem("-"),
                new MenuItem("Rename", LabelEditHandler),
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
            if (sourceNode is ShortcutTreeNode)
            {
                ShortcutTreeNode shortcut = sourceNode as ShortcutTreeNode;

                if (shortcut.Item.Dictionary == Item.Dictionary)
                {
                    Shortcut otherShortcut = (Shortcut) shortcut.Item.Duplicate();
                    Item.appendShortcuts(otherShortcut);
                    shortcut.Delete();
                }
            }
            else if (sourceNode is ShortcutFolderTreeNode)
            {
                ShortcutFolderTreeNode folder = sourceNode as ShortcutFolderTreeNode;

                if (folder.Item.Dictionary == Item.Dictionary)
                {
                    ShortcutFolder otherFolder = (ShortcutFolder) folder.Item.Duplicate();
                    Item.appendFolders(otherFolder);
                    folder.Delete();
                }
            }
            else
            {
                Namable namable = sourceNode.Model as Namable;

                Shortcut shortcut = (Shortcut) acceptor.getFactory().createShortcut();
                shortcut.CopyFrom(namable);
                Item.appendShortcuts(shortcut);
            }
        }
    }
}