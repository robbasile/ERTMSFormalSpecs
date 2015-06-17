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

using HistoricalData;
using Utils;
using History = DataDictionary.Compare.History;
using ModelElement = DataDictionary.ModelElement;

namespace GUI.HistoryView
{
    public class HistoryTreeView : TypedTreeView<IModelElement>
    {
        protected override void BuildModel()
        {
            Nodes.Clear();
            ModelElement modelElement = Root as ModelElement;
            if (modelElement != null)
            {
                History history = modelElement.EFSSystem.History;
                foreach (Change change in history.GetChanges(modelElement))
                {
                    ChangeTreeNode node = new ChangeTreeNode(change, true);
                    Nodes.Add(node);
                }
            }
        }
    }
}