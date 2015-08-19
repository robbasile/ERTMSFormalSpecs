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
using DataDictionary;
using DataDictionary.Generated;
using WeifenLuo.WinFormsUI.Docking;
using Dictionary = DataDictionary.Dictionary;
using Rule = DataDictionary.Rules.Rule;

namespace GUI.RulePerformances
{
    public partial class RulesPerformances : DockContent
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public RulesPerformances()
        {
            InitializeComponent();
            DockAreas = DockAreas.DockBottom;
            Refresh();
        }

        /// <summary>
        ///     Provides the rules that consumed most of the time
        /// </summary>
        private class GetSlowest : Visitor
        {
            /// <summary>
            ///     The list of rules
            /// </summary>
            private List<Rule> Rules { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            public GetSlowest()
            {
                Rules = new List<Rule>();
                foreach (Dictionary dictionary in EFSSystem.INSTANCE.Dictionaries)
                {
                    visit(dictionary, true);
                }
            }

            public override void visit(DataDictionary.Generated.Rule obj, bool visitSubNodes)
            {
                Rule rule = obj as Rule;

                Rules.Add(rule);
            }

            /// <summary>
            /// Compares two rules according to their execution time
            /// </summary>
            /// <param name="r1"></param>
            /// <param name="r2"></param>
            /// <returns></returns>
            private static int Comparer(Rule r1, Rule r2)
            {
                int retVal = 0;

                if (r1.ExecutionTimeInMilli < r2.ExecutionTimeInMilli)
                {
                    retVal = 1;
                }
                else if (r1.ExecutionTimeInMilli > r2.ExecutionTimeInMilli)
                {
                    retVal = -1;
                }

                return retVal;
            }

            /// <summary>
            ///     Provides the rules associated with their descending execution time
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Rule> GetRulesDesc()
            {
                List<Rule> retVal = Rules;

                retVal.Sort(Comparer);

                return retVal;
            }
        }

        /// <summary>
        /// Allows to display rule performances
        /// </summary>
        private class DisplayObject
        {
            private Rule Rule { get; set; }

            public DisplayObject(Rule rule)
            {
                Rule = rule;
            }

            // ReSharper disable once UnusedMember.Local
            public String RuleName
            {
                get { return Rule.FullName; }
            }

            // ReSharper disable once MemberCanBePrivate.Local
            public long ExecutionTime
            {
                get { return Rule.ExecutionTimeInMilli; }
            }

            // ReSharper disable once MemberCanBePrivate.Local
            public int ExecutionCount
            {
                get { return Rule.ExecutionCount; }
            }

            // ReSharper disable once UnusedMember.Local
            public int Average
            {
                get
                {
                    int retVal = 0;

                    if (ExecutionCount > 0)
                    {
                        retVal = (int) (ExecutionTime/ExecutionCount);
                    }

                    return retVal;
                }
            }
        }

        public override void Refresh()
        {
            if (dataGridView != null)
            {
                GetSlowest getter = new GetSlowest();
                IEnumerable<Rule> rules = getter.GetRulesDesc();
                List<DisplayObject> source = new List<DisplayObject>();
                foreach (Rule rule in rules)
                {
                    source.Add(new DisplayObject(rule));
                }
                dataGridView.DataSource = source;
            }

            base.Refresh();
        }
    }
}