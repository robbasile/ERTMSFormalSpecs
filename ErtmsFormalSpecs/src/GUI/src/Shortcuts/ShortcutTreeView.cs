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

using DataDictionary;

namespace GUI.Shortcuts
{
    public partial class ShortcutTreeView : TypedTreeView<EFSSystem>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public ShortcutTreeView()
            : base()
        {
            KeepTrackOfSelection = false;
            Refactor = false;
        }

        /// <summary>
        ///     Builds the tree model according to the root node
        /// </summary>
        protected override void BuildModel()
        {
            Nodes.Clear();
            foreach (Dictionary dictionary in Root.Dictionaries)
            {
                Nodes.Add(new ShortcutDictionaryTreeNode(dictionary.ShortcutsDictionary, true));
            }

            foreach (BaseTreeNode node in Nodes)
            {
                node.Expand();
            }
        }
    }
}