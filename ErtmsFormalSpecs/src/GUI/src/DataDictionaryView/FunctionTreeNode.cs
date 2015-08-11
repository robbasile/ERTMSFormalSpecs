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
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using GUI.Converters;
using Case = DataDictionary.Functions.Case;
using Dictionary = DataDictionary.Dictionary;
using Function = DataDictionary.Functions.Function;
using Parameter = DataDictionary.Parameter;

namespace GUI.DataDictionaryView
{
    public class FunctionTreeNode : ReqRelatedTreeNode<Function>
    {
        private class ItemEditor : TypeEditor
        {
            [Category("Description")]
            public override string Name
            {
                get { return base.Name; }
                set { base.Name = value; }
            }

            /// <summary>
            ///     The variable type
            /// </summary>
            [Category("Description")]
            [Editor(typeof (TypeUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (TypeUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public Function Type
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     Indicates that the function result can be cached, from one cycle to the other
            /// </summary>
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public bool IsCacheable
            {
                get { return Item.getCacheable(); }
                set { Item.setCacheable(value); }
            }
        }

        /// <summary>
        ///     The editor for message variables
        /// </summary>
        protected class TypeEditor : ReqRelatedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public FunctionTreeNode(Function item, bool buildSubNodes)
            : base(item, buildSubNodes)
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

            subNodes.Add(new ParametersTreeNode(Item, recursive));
            subNodes.Add(new CasesTreeNode(Item, recursive));
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="isFolder"></param>
        /// <param name="addRequirements"></param>
        public FunctionTreeNode(Function item, bool buildSubNodes, string name, bool isFolder = false,
            bool addRequirements = true)
            : base(item, buildSubNodes, name, isFolder, addRequirements)
        {
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddParameterHandler(object sender, EventArgs args)
        {
            Item.appendParameters(Parameter.CreateDefault(Item.FormalParameters));
        }

        public void AddCaseHandler(object sender, EventArgs args)
        {
            Item.appendCases(Case.CreateDefault(Item.Cases));
        }

        protected override ModelElement FindOrCreateUpdate()
        {
            ModelElement retVal = null;

            Dictionary dictionary = GetPatchDictionary();
            if (dictionary != null)
            {
                retVal = dictionary.FindByFullName(Item.FullName) as ModelElement;
                if (retVal == null)
                {
                    // If the element does not already exist in the patch, add a copy to it
                    retVal = Item.CreateFunctionUpdate(dictionary);
                }
                // Navigate to the element, whether it was created or not
                EFSSystem.INSTANCE.Context.SelectElement(retVal, this, Context.SelectionCriteria.DoubleClick);
            }

            return retVal;
        }

        public void DisplayHandler(object sender, EventArgs args)
        {
            GraphView.GraphView view = new GraphView.GraphView();
            GuiUtils.MdiWindow.AddChildWindow(view);
            view.Functions.Add(Item);
            view.Refresh();
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            MenuItem newItem = new MenuItem("Add...");
            newItem.MenuItems.Add(new MenuItem("Parameter", AddParameterHandler));
            newItem.MenuItems.Add(new MenuItem("Case", AddCaseHandler));
            retVal.Add(newItem);

            MenuItem updateItem = new MenuItem("Update...");
            updateItem.MenuItems.Add(new MenuItem("Update", AddUpdate));
            updateItem.MenuItems.Add(new MenuItem("Remove", RemoveInUpdate));
            retVal.Add(updateItem);
            retVal.Add(new MenuItem("Delete", DeleteHandler));
            retVal.AddRange(base.GetMenuItems());

            ModelElement.DontRaiseError(() =>
            {
                InterpretationContext context = new InterpretationContext(Item);
                if (Item.FormalParameters.Count == 1)
                {
                    Parameter parameter = (Parameter) Item.FormalParameters[0];
                    Graph graph = Item.createGraph(context, parameter, null);
                    if (graph != null && graph.Segments.Count != 0)
                    {
                        retVal.Insert(7, new MenuItem("Display", DisplayHandler));
                    }
                }
                else if (Item.FormalParameters.Count == 2)
                {
                    Surface surface = Item.createSurface(context, null);
                    if (surface != null && surface.Segments.Count != 0)
                    {
                        retVal.Insert(7, new MenuItem("Display", DisplayHandler));
                    }
                }
            });

            return retVal;
        }
    }
}