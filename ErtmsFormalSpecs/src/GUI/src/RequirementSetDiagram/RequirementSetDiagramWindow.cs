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

using DataDictionary.Specification;
using GUI.BoxArrowDiagram;

namespace GUI.RequirementSetDiagram
{
    public class RequirementSetDiagramWindow :
        BoxArrowWindow<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy>
    {
        /// <summary>
        ///     The panel used to display the state diagram
        /// </summary>
        public RequirementSetPanel Panel
        {
            get { return (RequirementSetPanel) BoxArrowContainerPanel; }
        }

        /// <summary>
        ///     Sets the system for this diagram
        /// </summary>
        /// <param name="enclosing"></param>
        public void SetEnclosing(IHoldsRequirementSets enclosing)
        {
            Model = enclosing;
            Panel.Model = enclosing;
        }

        public override BoxArrowPanel<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> CreatePanel()
        {
            BoxArrowPanel<IHoldsRequirementSets, RequirementSet, RequirementSetDependancy> retVal =
                new RequirementSetPanel();

            return retVal;
        }
    }
}