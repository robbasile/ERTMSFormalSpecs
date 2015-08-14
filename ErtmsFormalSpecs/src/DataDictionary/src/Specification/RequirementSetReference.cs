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
using DataDictionary.Generated;

namespace DataDictionary.Specification
{
    /// <summary>
    ///     Represents a reference to a requirement set
    /// </summary>
    public class RequirementSetReference : Generated.RequirementSetReference
    {
        public override string Name
        {
            get
            {
                string retVal = "<unknown>";

                if (Ref != null)
                {
                    retVal = Ref.Name;
                }

                return retVal;
            }
            set { }
        }

        public override ArrayList EnclosingCollection
        {
            get
            {
                Paragraph paragraph = (Paragraph) Enclosing;

                return paragraph.allRequirementSets();
            }
        }

        /// <summary>
        ///     Provides the requirement set referenced by this requirement set reference
        /// </summary>
        public RequirementSet Ref
        {
            get { return GuidCache.Instance.GetModel(getTarget()) as RequirementSet; }
        }

        /// <summary>
        ///     This visitor can be used to recursively remove all requirement set references
        /// </summary>
        public class RemoveReferencesVisitor : Visitor
        {
            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                while (paragraph.RequirementSetReferences.Count > 0)
                {
                    RequirementSetReference reference = (RequirementSetReference) paragraph.RequirementSetReferences[0];
                    reference.Delete();
                }

                base.visit(obj, visitSubNodes);
            }
        }
    }
}