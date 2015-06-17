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
        ///     The EFS System for which this view is built
        /// </summary>
        private EFSSystem EFSSystem { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public FunctionsPerformances()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public FunctionsPerformances(EFSSystem efsSystem)
        {
            EFSSystem = efsSystem;
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

            public GetSlowest(EFSSystem efsSystem)
            {
                Functions = new List<Function>();
                foreach (Dictionary dictionary in efsSystem.Dictionaries)
                {
                    visit(dictionary, true);
                }
            }

            public override void visit(DataDictionary.Generated.Function obj, bool visitSubNodes)
            {
                Function function = obj as Function;

                Functions.Add(function);
            }

            private int Comparer(Function f1, Function f2)
            {
                if (f1.ExecutionTimeInMilli < f2.ExecutionTimeInMilli)
                {
                    return 1;
                }
                else if (f1.ExecutionTimeInMilli > f2.ExecutionTimeInMilli)
                {
                    return -1;
                }

                return 0;
            }

            /// <summary>
            ///     Provides the functions associated with their descending execution time
            /// </summary>
            /// <returns></returns>
            public List<Function> getFunctionsDesc()
            {
                List<Function> retVal = Functions;

                retVal.Sort(new Comparison<Function>(Comparer));

                return retVal;
            }
        }

        private class DisplayObject
        {
            private Function Function { get; set; }

            public DisplayObject(Function function)
            {
                Function = function;
            }

            public String FunctionName
            {
                get { return Function.FullName; }
            }

            public long ExecutionTime
            {
                get { return Function.ExecutionTimeInMilli; }
            }

            public int ExecutionCount
            {
                get { return Function.ExecutionCount; }
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
                List<Function> functions = getter.getFunctionsDesc();
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