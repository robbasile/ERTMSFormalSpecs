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
using Function = DataDictionary.Functions.Function;

namespace GUI.FunctionsPerformances
{
    public partial class FunctionsPerformances : DockContent
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public FunctionsPerformances()
        {
            InitializeComponent();
            Refresh();
        }

        /// <summary>
        ///     Provides the functions that consumed most of the time
        /// </summary>
        private class GetSlowest : Visitor
        {
            /// <summary>
            ///     The list of functions
            /// </summary>
            private List<Function> Functions { get; set; }

            public GetSlowest()
            {
                Functions = new List<Function>();
                foreach (Dictionary dictionary in EfsSystem.Instance.Dictionaries)
                {
                    visit(dictionary, true);
                }
            }

            public override void visit(DataDictionary.Generated.Function obj, bool visitSubNodes)
            {
                Function function = obj as Function;

                Functions.Add(function);
            }

            /// <summary>
            /// Compares two functions according to their execution time
            /// </summary>
            /// <param name="f1"></param>
            /// <param name="f2"></param>
            /// <returns></returns>
            private static int Comparer(Function f1, Function f2)
            {
                int retVal = 0;

                if (f1.ExecutionTimeInMilli < f2.ExecutionTimeInMilli)
                {
                    retVal = 1;
                }
                else if (f1.ExecutionTimeInMilli > f2.ExecutionTimeInMilli)
                {
                    retVal = -1;
                }

                return retVal;
            }

            /// <summary>
            ///     Provides the functions associated with their descending execution time
            /// </summary>
            /// <returns></returns>
            public IEnumerable<Function> GetFunctionsDesc()
            {
                List<Function> retVal = Functions;

                retVal.Sort(Comparer);

                return retVal;
            }
        }

        /// <summary>
        /// Display information about function performances
        /// </summary>
        private class DisplayObject
        {
            private Function Function { get; set; }

            /// <summary>
            /// Constuctor
            /// </summary>
            /// <param name="function"></param>
            public DisplayObject(Function function)
            {
                Function = function;
            }

            // ReSharper disable once UnusedMember.Local
            public String FunctionName
            {
                get { return Function.FullName; }
            }

            // ReSharper disable once MemberCanBePrivate.Local
            public long ExecutionTime
            {
                get { return Function.ExecutionTimeInMilli; }
            }

            // ReSharper disable once MemberCanBePrivate.Local
            public int ExecutionCount
            {
                get { return Function.ExecutionCount; }
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
                IEnumerable<Function> functions = getter.GetFunctionsDesc();
                List<DisplayObject> source = new List<DisplayObject>();
                foreach (Function function in functions)
                {
                    source.Add(new DisplayObject(function));
                }
                dataGridView.DataSource = source;
            }

            base.Refresh();
        }
    }
}