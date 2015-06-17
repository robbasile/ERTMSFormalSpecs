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
        ///     The EFS System for which this view is built
        /// </summary>
        private EFSSystem EFSSystem { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public RulesPerformances()
        {
            InitializeComponent();

            DockAreas = DockAreas.DockBottom;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public RulesPerformances(EFSSystem efsSystem)
        {
            EFSSystem = efsSystem;
            InitializeComponent();
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

            public GetSlowest(EFSSystem efsSystem)
            {
                Rules = new List<Rule>();
                foreach (Dictionary dictionary in efsSystem.Dictionaries)
                {
                    visit(dictionary, true);
                }
            }

            public override void visit(DataDictionary.Generated.Rule obj, bool visitSubNodes)
            {
                Rule rule = obj as Rule;

                Rules.Add(rule);
            }

            private int Comparer(Rule r1, Rule r2)
            {
                if (r1.ExecutionTimeInMilli < r2.ExecutionTimeInMilli)
                {
                    return 1;
                }
                else if (r1.ExecutionTimeInMilli > r2.ExecutionTimeInMilli)
                {
                    return -1;
                }

                return 0;
            }

            /// <summary>
            ///     Provides the rules associated with their descending execution time
            /// </summary>
            /// <returns></returns>
            public List<Rule> getRulesDesc()
            {
                List<Rule> retVal = Rules;

                retVal.Sort(new Comparison<Rule>(Comparer));

                return retVal;
            }
        }

        private class DisplayObject
        {
            private Rule Rule { get; set; }

            public DisplayObject(Rule rule)
            {
                Rule = rule;
            }

            public String RuleName
            {
                get { return Rule.FullName; }
            }

            public long ExecutionTime
            {
                get { return Rule.ExecutionTimeInMilli; }
            }

            public int ExecutionCount
            {
                get { return Rule.ExecutionCount; }
            }

            public int Average
            {
                get
                {
                    if (ExecutionCount > 0)
                    {
                        return (int) (ExecutionTime/ExecutionCount);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public override void Refresh()
        {
            if (EFSSystem != null && dataGridView != null)
            {
                GetSlowest getter = new GetSlowest(EFSSystem);
                List<Rule> rules = getter.getRulesDesc();
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