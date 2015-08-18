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

using System.Windows.Forms;
using DataDictionary.Specification;
using GUI.RequirementSetDiagram;

namespace GUI.LongOperations
{
    public class OpenRequirementSetOperation : BaseLongOperation
    {
        /// <summary>
        ///     The window that should display the requirement set
        /// </summary>
        private RequirementSetDiagramWindow Window { get; set; }

        /// <summary>
        ///     The enclosing element
        /// </summary>
        private IHoldsRequirementSets Enclosing { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="window"></param>
        /// <param name="enclosing"></param>
        public OpenRequirementSetOperation(RequirementSetDiagramWindow window, IHoldsRequirementSets enclosing)
        {
            Window = window;
            Enclosing = enclosing;
        }

        /// <summary>
        ///     Performs the job as a background task
        /// </summary>
        public override void ExecuteWork()
        {
            Window.SetEnclosing(Enclosing);
            Window.BeginInvoke((MethodInvoker) (() => Window.Panel.RefreshControl()));
        }
    }
}