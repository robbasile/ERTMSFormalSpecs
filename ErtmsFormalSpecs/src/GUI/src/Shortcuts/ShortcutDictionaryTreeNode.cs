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
using GUI.DataDictionaryView;
using Shortcut = DataDictionary.Shortcuts.Shortcut;
using ShortcutDictionary = DataDictionary.Shortcuts.ShortcutDictionary;
using ShortcutFolder = DataDictionary.Shortcuts.ShortcutFolder;

namespace GUI.Shortcuts
{
    public class ShortcutDictionaryTreeNode : ModelElementTreeNode<ShortcutDictionary>
    {
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public ShortcutDictionaryTreeNode(ShortcutDictionary item, bool buildSubNodes)
            : base(item, buildSubNodes, item.Name, true)
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

        public void AddFolderHandler(object sender, EventArgs args)
        {
            ShortcutFolder folder = (ShortcutFolder) acceptor.getFactory().createShortcutFolder();
            folder.Name = "<Folder " + (Item.Folders.Count + 1) + ">";
            Item.appendFolders(folder);
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Add folder", AddFolderHandler)};

            return retVal;
        }

        /// <summary>
        ///     Accepts a drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
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
                else if (sourceNode is RuleTreeNode)
                {
                    RuleTreeNode rule = sourceNode as RuleTreeNode;

                    if (rule.Item.Dictionary == Item.Dictionary)
                    {
                        Shortcut shortcut = (Shortcut) acceptor.getFactory().createShortcut();
                        shortcut.CopyFrom(rule.Item);
                        Item.appendShortcuts(shortcut);
                    }
                }
                else if (sourceNode is FunctionTreeNode)
                {
                    FunctionTreeNode function = sourceNode as FunctionTreeNode;

                    if (function.Item.Dictionary == Item.Dictionary)
                    {
                        Shortcut shortcut = (Shortcut) acceptor.getFactory().createShortcut();
                        shortcut.CopyFrom(function.Item);
                        Item.appendShortcuts(shortcut);
                    }
                }
                else if (sourceNode is ProcedureTreeNode)
                {
                    ProcedureTreeNode procedure = sourceNode as ProcedureTreeNode;

                    if (procedure.Item.Dictionary == Item.Dictionary)
                    {
                        Shortcut shortcut = (Shortcut) acceptor.getFactory().createShortcut();
                        shortcut.CopyFrom(procedure.Item);
                        Item.appendShortcuts(shortcut);
                    }
                }
                else if (sourceNode is VariableTreeNode)
                {
                    VariableTreeNode variable = sourceNode as VariableTreeNode;

                    if (variable.Item.Dictionary == Item.Dictionary)
                    {
                        Shortcut shortcut = (Shortcut) acceptor.getFactory().createShortcut();
                        shortcut.CopyFrom(variable.Item);
                        Item.appendShortcuts(shortcut);
                    }
                }
            }
        }
    }
}