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

namespace DataDictionary.Specification
{
    /// <summary>
    ///     Represents a requirement set
    /// </summary>
    public class RequirementSetDependancy : Generated.RequirementSetDependancy, IGraphicalArrow<RequirementSet>
    {
        /// <summary>
        ///     The name of the dependancy (always the same)
        /// </summary>
        public override string Name
        {
            get { return "Depends on"; }
            set { }
        }

        /// <summary>
        ///     The source of the arrow
        /// </summary>
        public RequirementSet Source
        {
            get { return Enclosing as RequirementSet; }
        }

        /// <summary>
        ///     Sets the source box for this arrow
        /// </summary>
        /// <param name="initialBox"></param>
        public void SetInitialBox(IGraphicalDisplay initialBox)
        {
            Source.removeDependancies(this);
            RequirementSet newSource = (RequirementSet) initialBox;
            newSource.appendDependancies(this);
        }

        /// <summary>
        ///     The target of the arrow
        /// </summary>
        public RequirementSet Target
        {
            get { return GuidCache.INSTANCE.GetModel(getTarget()) as RequirementSet; }
        }

        /// <summary>
        ///     Sets the target box for this arrow
        /// </summary>
        /// <param name="targetBox"></param>
        public void SetTargetBox(IGraphicalDisplay targetBox)
        {
            RequirementSet target = (RequirementSet) targetBox;
            setTarget(target.Guid);
        }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public string GraphicalName
        {
            get { return Name; }
        }

        /// <summary>
        ///     The model element which is referenced by this arrow
        /// </summary>
        public ModelElement ReferencedModel
        {
            get { return this; }
        }

        /// <summary>
        ///     The collection in which this model element lies
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = Source.allDependancies();

                return retVal;
            }
        }
    }
}