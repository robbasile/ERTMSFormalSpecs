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

using System.Collections.Generic;
using DataDictionary.Functions;

namespace GUIUtils.GraphVisualization.Functions
{
    public abstract class ProfileSetFunction : ProfileFunction
    {
        /// <summary>
        /// The set of function
        /// </summary>
        public List<IGraph> Functions;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProfileSetFunction()
        {
            Functions = new List<IGraph>();
        }

        /// <summary>
        /// Erases the data of the function
        /// </summary>
        public override void ClearData()
        {
            base.ClearData();
            Functions.Clear();
        }

        /// <summary>
        /// Relocates the function according to the LRBG position
        /// </summary>
        /// <param name="lrbgPosition"></param>
        protected override void Relocate(double lrbgPosition)
        {
            base.Relocate(lrbgPosition);
            foreach (IGraph function in Functions)
            {
                UpdateFunction(function, lrbgPosition);
            }
        }
    }
}
