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
using Utils;

namespace DataDictionary.Tests.Issue
{
    /// <summary>
    /// Counts the number of items (actions, expectations, ...) in part of the subtree
    /// </summary>
    public class Counter : Generated.Visitor
    {
        /// <summary>
        /// The number of sub sequences found
        /// </summary>
        public int SubSequences { get; private set; }

        /// <summary>
        /// The subsequences that are completed
        /// </summary>
        public HashSet<SubSequence> CompletedSubSequences { get; private set; }

        /// <summary>
        /// The number of actions (changes) found
        /// </summary>
        public int Actions { get; private set; }

        /// <summary>
        /// The number of expectations found
        /// </summary>
        public int Checks { get; private set; }

        /// <summary>
        /// The number of issues found
        /// </summary>
        public Dictionary<IssueKind, int> Issues { get; private set; }

        /// <summary>
        /// The subsequences that are blocked by a blocking issue
        /// </summary>
        public HashSet<SubSequence> BlockingSubSequences { get; private set; }
       
        /// <summary>
        /// Constructor
        /// </summary>
        public Counter()
        {
            SubSequences = 0;
            CompletedSubSequences = new HashSet<SubSequence>();
            Actions = 0;
            Checks = 0;
            Issues = new Dictionary<IssueKind, int>
            {
                {IssueKind.Blocking, 0},
                {IssueKind.Issue, 0},
                {IssueKind.Comment, 0},
                {IssueKind.Question, 0}
            };

            BlockingSubSequences = new HashSet<SubSequence>();
        }

        /// <summary>
        /// Counts the number of sub sequences
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.SubSequence obj, bool visitSubNodes)
        {
            SubSequences += 1;

            SubSequence subSequence = obj as SubSequence;
            if (subSequence != null)
            {
                if (subSequence.getCompleted())
                {
                    CompletedSubSequences.Add(subSequence);
                }                
            }

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Counts the number of actions
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Action obj, bool visitSubNodes)
        {
            Actions += 1;

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Counts the number of checks
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Expectation obj, bool visitSubNodes)
        {
            Checks += 1;

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Counts the issues and the blocking issues 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.ReferencesParagraph obj, bool visitSubNodes)
        {
            ReferencesParagraph referencesParagraph = obj as ReferencesParagraph;

            if (referencesParagraph != null)
            {
                foreach (ReqRef reqRef in referencesParagraph.Requirements)
                {
                    IssueKind? kind = IssueKindUtil.GetKind(reqRef.Paragraph);
                    if (kind != null)
                    {
                        Issues[(IssueKind) kind] += 1;
                        if (kind == IssueKind.Blocking)
                        {
                            SubSequence enclosingSubSequence = EnclosingFinder<SubSequence>.find(referencesParagraph,
                                true);
                            if (enclosingSubSequence != null)
                            {
                                BlockingSubSequences.Add(enclosingSubSequence);
                            }
                        }
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }
    }
}
