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

using System.Collections.Generic;
using DataDictionary;

namespace GUI.TestRunnerView
{
    public class TestTreeView : TypedTreeView<EfsSystem>
    {
        /// <summary>
        ///     Build the model of this tree view
        /// </summary>
        /// <returns>the root nodes of the tree</returns>
        protected override List<BaseTreeNode> BuildModel()
        {
            List<BaseTreeNode> retVal = new List<BaseTreeNode>();

            foreach (Dictionary dictionary in Root.Dictionaries)
            {
                retVal.Add(new TestsTreeNode(dictionary, true));
            }

            return retVal;
        }
    }
}