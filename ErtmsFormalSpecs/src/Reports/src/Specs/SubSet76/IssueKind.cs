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
using MigraDoc.DocumentObjectModel;
using Paragraph = DataDictionary.Specification.Paragraph;

namespace Reports.Specs.SubSet76
{
    /// <summary>
    /// Identifies the kind of issue
    /// </summary>
    public enum IssueKind
    {
        Blocking,
        Issue,
        Comment,
        Question
    }

    /// <summary>
    /// Utility class for issue kind
    /// </summary>
    public static class IssueKindUtil
    {
        private const string Blocking = "Blocking issue";
        private const string Issue = "Issue";
        private const string Comment = "Comment";
        private const string Question = "Question";

        /// <summary>
        /// Provides the kind of issue
        /// </summary>
        /// <param name="issue"></param>
        /// <returns></returns>
        public static IssueKind? GetKind(Paragraph issue)
        {
            IssueKind? retVal = null;

            foreach (RequirementSetReference reference in issue.RequirementSetReferences)
            {
                RequirementSet requirementSet = reference.Ref;
                while (requirementSet != null && retVal == null)
                {
                    if (requirementSet.Name.Equals(Blocking))
                    {
                        retVal = IssueKind.Blocking;
                    }
                    else if (requirementSet.Name.Equals(Issue))
                    {
                        retVal = IssueKind.Issue;
                    }
                    else if (requirementSet.Name.Equals(Comment))
                    {
                        retVal = IssueKind.Comment;
                    }
                    else if (requirementSet.Name.Equals(Question))
                    {
                        retVal = IssueKind.Question;
                    }

                    requirementSet = requirementSet.Enclosing as RequirementSet;
                }

                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the color associated to the issue kind
        /// </summary>
        /// <param name="issue"></param>
        /// <returns></returns>
        public static Color IssueColor(Paragraph issue)
        {
            return IssueColor(GetKind(issue));
        }

        /// <summary>
        /// Provides the color associated to the issue kind
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static Color IssueColor(IssueKind? kind)
        {
            Color retVal = Colors.IndianRed;

            if (kind == IssueKind.Comment)
            {
                retVal = Colors.LightGreen;
            }
            else if (kind == IssueKind.Question)
            {
                retVal = Colors.LightGoldenrodYellow;
            }
            else if (kind == null)
            {
                retVal = Colors.Transparent;
            }

            return retVal;
        }
    }
}
