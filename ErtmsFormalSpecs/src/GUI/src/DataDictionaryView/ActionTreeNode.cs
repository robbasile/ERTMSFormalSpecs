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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Statement;
using GUI.Converters;
using Action = DataDictionary.Rules.Action;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using SubStep = DataDictionary.Tests.SubStep;

namespace GUI.DataDictionaryView
{
    public class ActionTreeNode : ModelElementTreeNode<Action>
    {
        private class ItemEditor : Editor
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }

            [Category("Description")]
            [Editor(typeof (ExpressionableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (ExpressionableUITypeConverter))]
            public Action Expression
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            [Category("Description")]
            [Editor(typeof (CommentableUITypedEditor), typeof (UITypeEditor))]
            [TypeConverter(typeof (CommentableUITypeConverter))]
            public Action Comment
            {
                get { return Item; }
                set
                {
                    Item = value;
                    RefreshNode();
                }
            }

            /// <summary>
            ///     Sets the verified flag of the the enclosing rule
            /// </summary>
            /// <param name="val"></param>
            private void SetRuleAsVerified(bool val)
            {
                if (Item.Rule != null)
                {
                    Item.Rule.setVerified(val);
                }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        public ActionTreeNode(Action item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }

        /// <summary>
        ///     Handles a selection change event
        /// </summary>
        /// <param name="displayStatistics">Indicates that statistics should be displayed in the MDI window</param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(displayStatistics);
            if (Item.Translation != null)
            {
                if (BaseTreeView != null && BaseTreeView.RefreshNodeContent)
                {
                    IBaseForm baseForm = BaseForm;
                    if (baseForm != null)
                    {
                        if (baseForm.RequirementsTextBox != null)
                        {
                            baseForm.RequirementsTextBox.Text = Item.Translation.getSourceTextExplain();
                        }
                    }
                }
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
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Delete", new EventHandler(DeleteHandler)));
            retVal.AddRange(base.GetMenuItems());
            retVal.Insert(4, new MenuItem("-"));
            retVal.Insert(5, new MenuItem("Split", new EventHandler(SplitHandler)));

            return retVal;
        }

        /// <summary>
        ///     Splits the selected action to several sub-actions
        /// </summary>
        public virtual void SplitHandler(object sender, EventArgs args)
        {
            Statement statement = Item.EFSSystem.Parser.Statement(Item, Item.ExpressionText);
            VariableUpdateStatement variableUpdateStatement = statement as VariableUpdateStatement;
            if (variableUpdateStatement != null)
            {
                Expression expression = variableUpdateStatement.Expression;
                StructExpression structExpression = expression as StructExpression;
                if (structExpression != null)
                {
                    Dictionary<Designator, Expression> associations = structExpression.Associations;
                    foreach (KeyValuePair<Designator, Expression> value in associations)
                    {
                        Action action = (Action) acceptor.getFactory().createAction();
                        action.ExpressionText = structExpression.Structure.ToString() + "." + value.Key + " <- " +
                                                value.Value.ToString();
                        string aString = value.Value.ToString();
                        ActionTreeNode actionTreeNode = new ActionTreeNode(action, true);

                        BaseTreeNode parent = Parent as BaseTreeNode;
                        if ((parent != null) && (parent.Nodes != null))
                        {
                            RuleCondition ruleCondition = Item.Enclosing as RuleCondition;
                            if (ruleCondition != null)
                            {
                                ruleCondition.appendActions(action);
                            }
                            else
                            {
                                SubStep subStep = Item.Enclosing as SubStep;
                                if (subStep != null)
                                {
                                    subStep.appendActions(action);
                                }
                            }
                            parent.Nodes.Add(actionTreeNode);
                        }
                    }
                }
            }
            Delete();
            SortSubNodes();
        }
    }
}