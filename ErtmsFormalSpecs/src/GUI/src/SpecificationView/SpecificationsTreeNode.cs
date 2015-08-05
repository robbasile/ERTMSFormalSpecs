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
using Dictionary = DataDictionary.Dictionary;
using Specification = DataDictionary.Specification.Specification;

namespace GUI.SpecificationView
{
    public class SpecificationsTreeNode : ModelElementTreeNode<Dictionary>
    {
        /// <summary>
        ///     The editor
        /// </summary>
        private class SpecificationEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Instanciates the editor
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new SpecificationEditor();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public SpecificationsTreeNode(Dictionary item, bool buildSubNodes)
            : base(item, buildSubNodes, "Specifications", true)
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

            foreach (Specification specification in Item.Specifications)
            {
                subNodes.Add(new SpecificationTreeNode(specification, recursive));
            }
            subNodes.Sort();
        }

        public void AddSpecificationHandler(object sender, EventArgs args)
        {
            Specification specification = (Specification) acceptor.getFactory().createSpecification();
            specification.setName("Specification" + (Item.countSpecifications() + 1));
            Item.appendSpecifications(specification);
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = base.GetMenuItems();

            retVal.Add(new MenuItem("Add specification", AddSpecificationHandler));

            return retVal;
        }
    }
}