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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DataDictionary.Generated;
using GUI.Converters;
using Expectation = DataDictionary.Tests.Expectation;

namespace GUI.TestRunnerView
{
    public class ExpectationTreeNode : ModelElementTreeNode<Expectation>
    {
        /// <summary>
        ///     The value editor
        /// </summary>
        private class ItemEditor : Editor
        {
            [Category("Description")]
            [Editor(typeof (ExpressionableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (ExpressionableUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public Expectation Expression
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public bool Blocking
            {
                get { return Item.getBlocking(); }
                set { Item.setBlocking(value); }
            }

            [Category("Description"), TypeConverter(typeof (ExpectationKindConverter))]
            [ReadOnly(false)]
            // ReSharper disable once UnusedMember.Local
            public acceptor.ExpectationKind Kind
            {
                get { return Item.getKind(); }
                set
                {
                    Item.setKind(value);
                    UpdateActivation();
                }
            }

            [Category("Description"), DisplayName("Condition")]
            [Editor(typeof (ConditionUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (ConditionUITypeConverter))]
            // ReSharper disable once UnusedMember.Local
            public Expectation Condition
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public double DeadLine
            {
                get { return Item.DeadLine; }
                set { Item.DeadLine = value; }
            }

            /// <summary>
            ///     The item name
            /// </summary>
            [Category("Description"), TypeConverter(typeof (CyclePhaseConverter))]
            // ReSharper disable once UnusedMember.Local
            public acceptor.RulePriority CyclePhase
            {
                get { return Item.getCyclePhase(); }
                set { Item.setCyclePhase(value); }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public ExpectationTreeNode(Expectation item, bool buildSubNodes)
            : base(item, buildSubNodes)
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

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem> {new MenuItem("Delete", DeleteHandler)};

            retVal.AddRange(base.GetMenuItems());

            return retVal;
        }

        /// <summary>
        ///     Creates sub sequence tree nodes
        /// </summary>
        /// <param name="elements">The elements to be placed in the node</param>
        public static List<BaseTreeNode> CreateExpectations(ArrayList elements)
        {
            List<BaseTreeNode> retVal = new List<BaseTreeNode>();

            foreach (Expectation expectation in elements)
            {
                retVal.Add(new ExpectationTreeNode(expectation, true));
            }

            return retVal;
        }
    }
}