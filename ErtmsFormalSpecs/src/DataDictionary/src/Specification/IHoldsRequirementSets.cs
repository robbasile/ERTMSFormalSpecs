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

namespace DataDictionary.Specification
{
    /// <summary>
    ///     Holds requirement sets
    /// </summary>
    public interface IHoldsRequirementSets
    {
        /// <summary>
        ///     Provides the list of requirement sets in the system
        /// </summary>
        List<RequirementSet> RequirementSets { get; }

        /// <summary>
        ///     Provides the requirement set whose name corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <param name="create">Indicates that the requirement set should be created if it does not exists</param>
        /// <returns></returns>
        RequirementSet findRequirementSet(string name, bool create);

        /// <summary>
        ///     Adds a new requirement set to this list of requirement sets
        /// </summary>
        /// <param name="requirementSet"></param>
        void AddRequirementSet(RequirementSet requirementSet);
    }
}