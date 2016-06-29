﻿// ------------------------------------------------------------------------------
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
using System.Globalization;
using DataDictionary;
using DataDictionary.Tests.Issue;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Utils;
using Chapter = DataDictionary.Specification.Chapter;
using Dictionary = DataDictionary.Dictionary;
using Frame = DataDictionary.Tests.Frame;
using Paragraph = DataDictionary.Specification.Paragraph;
using ReferencesParagraph = DataDictionary.ReferencesParagraph;
using ReqRef = DataDictionary.ReqRef;
using Specification = DataDictionary.Specification.Specification;
using Step = DataDictionary.Tests.Step;
using SubSequence = DataDictionary.Tests.SubSequence;
using TestCase = DataDictionary.Tests.TestCase;

namespace Reports.Specs.SubSet76
{
    public class Subseet76ReportHandler : ReportHandler
    {
        /// <summary>
        /// The findings report currently being built
        /// </summary>
        private Subseet76Report Report { get; set; }

        /// <summary>
        /// Indicates that details should be included in the report
        /// </summary>
        public bool IncludeDetails { get; set; }

        /// <summary>
        /// Indicates that details about each test sequence should be included in the report
        /// </summary>
        public bool IncludeTestSequencesDetails { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary">The dictionary holding the S76 issues and S76 test sequences</param>
        public Subseet76ReportHandler(Dictionary dictionary)
            : base(dictionary)
        {
            CreateFileName("FindingsReport");
        }

        public override Document BuildDocument()
        {
            Document retVal = new Document
            {
                Info =
                {
                    Title = "EFS Subset-076 Findings report",
                    Author = "ERTMS Solutions",
                    Subject = "Subset-076 findings report"
                }
            };

            // Apply translation rule to get the spec issues from the translated steps
            MarkingHistory.PerformMark(() =>
            {
                foreach (Frame frame in Dictionary.Tests)
                {
                    frame.Translate();
                }
            });

            // Fill the report
            Report = new Subseet76Report(retVal);
            Report.AddSubParagraph("Subset-076 Analysis report");
            CreateIntroduction();
            CreateTestSequenceReport();
            CreateFindingsReport();
            Report.CloseSubParagraph();

            return retVal;
        }

        /// <summary>
        /// Creates the introduction paragraph
        /// </summary>
        private void CreateIntroduction()
        {
            Report.AddSubParagraph("Introduction");
            Report.AddParagraph(
                "This report provides the current state of the application of test sequences from Subset-076 applied on the ERTMSFormalSpecs model.");
            Report.CloseSubParagraph();
        }

        /// <summary>
        /// Provides the string representation of a percentage between two values
        /// </summary>
        /// <param name="number"></param>
        /// <param name="totalNumber"></param>
        /// <returns></returns>
        private string Percent(int number, int totalNumber)
        {
            return Math.Round(number*100/(double)totalNumber) + "% (" + number + "/" + totalNumber + ")";
        }

        /// <summary>
        /// Creates the section which describes the translated test and the result of application of those tests on the model
        /// </summary>
        private void CreateTestSequenceReport()
        {
            Report.AddSubParagraph("Sub sequence report");
            Report.AddParagraph(
                "This section presents the sub sequences that have been translated and applied on the Subset-026 model, along with their execution result and the issues found in the test sequence definition.");

            Counter counter = new Counter(true);
            counter.visit(Dictionary, true);

            Report.AddParagraph("The following table presents summarises the test sequences that have been analysed.");

            Report.AddTable(new[] { "", "Description", "Count", "Ratio" }, new[] { 35, 105, 15, 15 });
            Report.AddRow(
                "Imported sequences",
                "The number of test sequences imported from the Subset-076 Access databases.",
                counter.SubSequences.ToString(CultureInfo.InvariantCulture),
                Percent(counter.SubSequences, 700));

            Report.AddRow(
                "Sequence analysis completed",
                "The number of imported test sequences for which the analysis has been completely executed.",
                counter.CompletedSubSequences.Count.ToString(CultureInfo.InvariantCulture),
                Percent(counter.CompletedSubSequences.Count, counter.SubSequences));
            Report.lastRow.Cells[0].MergeDown = 2;

            Report.AddRow(
                "",
                "The full test has been executed and is successful. We may have fixed several test steps to achieve this objective.",
                counter.CompleteAndValid.Count.ToString(CultureInfo.InvariantCulture),
                Percent(counter.CompleteAndValid.Count, counter.CompletedSubSequences.Count));

            Report.AddRow(
                "",
                "A blocking issue has been found while analyzing the sub sequence, however, all steps before that issue have been successfully executed.",
                counter.CompleteAndBlocking.Count.ToString(CultureInfo.InvariantCulture),
                Percent(counter.CompleteAndBlocking.Count, counter.CompletedSubSequences.Count));

            Report.AddRow(
                "Ongoing sequences",
                "The number of imported test sequences for which analysis is ongoing. No definitive result is provided yet for such test sequences",
                counter.OngoingSubSequences.Count.ToString(CultureInfo.InvariantCulture),
                Percent(counter.OngoingSubSequences.Count, counter.SubSequences));

            Report.AddParagraph(
                "The following table categorises the issues found during analysis of the test sequences. ");

            Report.AddTable(new[] { "Issue kind", "Definition", "Count" }, new[] { 35, 105, 30 });
            Report.AddRow(
                "Blocking issues",
                "Blocking issues are issues found in the text sequences that forbid execution of the sequence, for instance a missing message in the sequence definition. " + (IncludeDetails ? "These issues are reported in red." : ""),
            counter.Issues[IssueKind.Blocking].ToString(CultureInfo.InvariantCulture));
            if (IncludeDetails)
            {
                Report.SetLastRowColor(IssueColor(IssueKind.Blocking));
            }

            Report.AddRow(
                "Issues",
                "Issues are errors detected in the test sequences. The corresponding test has been manually updated to avoid the error and continue execution. " + (IncludeDetails ? "These issues are reported in red." : ""),
                counter.Issues[IssueKind.Issue].ToString(CultureInfo.InvariantCulture));
            if (IncludeDetails)
            {
                Report.SetLastRowColor(IssueColor(IssueKind.Issue));
            }

            Report.AddRow(
                "Questions",
                "Question raised during the test sequence analysis. " + (IncludeDetails ? "These issues are reported in yellow." : ""),
                counter.Issues[IssueKind.Question].ToString(CultureInfo.InvariantCulture));
            if (IncludeDetails)
            {
                Report.SetLastRowColor(IssueColor(IssueKind.Question));
            }

            Report.AddRow(
                "Comments",
                "Comments issued during the test sequence analysis. " + (IncludeDetails ? "These issues are reported in green." : ""),
                counter.Issues[IssueKind.Comment].ToString(CultureInfo.InvariantCulture));
            if (IncludeDetails)
            {
                Report.SetLastRowColor(IssueColor(IssueKind.Comment));
            }

            Report.AddSubParagraph("Summary");
            Report.AddParagraph("This section summarises the results for each sub sequence. The 'Actions' are the number of inputs required to execute the test sequence, and 'Checks' identifies the check points that are verified on the sub sequence. The column issues counts the number of issues (either blocking or non blocking) found during the analysis of the test sequence. The status can take several values");
            Report.AddListItem("Completed : the analysis of the test sequence is complete and the execution satisfies the constaints defined in the test sequence. ");
            Report.AddListItem("Ongoing : the analysis of the test sequence is ongoing, no definitive result can be provided yet.");
            Report.AddTable(new[] { "Subsequence", "Blocking issues", "Issues", "Questions", "Comments", "Status" }, new[] { 72, 20, 20, 20, 20, 28 });
            foreach (Frame frame in Dictionary.Tests)
            {
                foreach (SubSequence subSequence in frame.SubSequences)
                {
                    counter = new Counter(true);
                    counter.visit(subSequence, true);

                    string status = "Ongoing";
                    Color color = Colors.Orange;
                    if (subSequence.getCompleted())
                    {
                        status = "Completed";
                        color = Colors.Green;
                    }

                    Report.AddRow(
                        subSequence.Name,
                        counter.Issues[IssueKind.Blocking].ToString(CultureInfo.InvariantCulture),
                        counter.Issues[IssueKind.Issue].ToString(CultureInfo.InvariantCulture),
                        counter.Issues[IssueKind.Question].ToString(CultureInfo.InvariantCulture),
                        counter.Issues[IssueKind.Comment].ToString(CultureInfo.InvariantCulture),
                        status);
                    Row sequenceRow = Report.lastRow;
                    sequenceRow.Cells[5].Shading.Color = color;

                    // Provide the comment, if any
                    if (!string.IsNullOrEmpty(subSequence.Comment))
                    {
                        Report.AddRow(subSequence.Comment, "", "", "", "", "");
                        Report.SetLastRowColor(color);
                        Report.lastRow.Cells[0].MergeRight = 4;
                        sequenceRow.Cells[5].MergeDown += 1;
                    }

                    if (IncludeDetails)
                    {
                        // Provide the test cases and the steps related to this test sequence
                        foreach (TestCase testCase in subSequence.TestCases)
                        {
                            foreach (ReqRef reqRef in testCase.Requirements)
                            {
                                Report.AddRow(
                                    testCase.Name,
                                    reqRef.Paragraph.Text, "", "", "", "");
                                Report.SetLastRowColor(IssueColor(reqRef.Paragraph));
                                Report.lastRow.Cells[1].MergeRight = 3;
                                sequenceRow.Cells[5].MergeDown += 1;
                            }

                            foreach (Step step in testCase.Steps)
                            {
                                foreach (ReqRef reqRef in step.Requirements)
                                {
                                    string name = testCase.Name;

                                    if (step.getTCS_Order() != 0)
                                    {
                                        name += " Step  " + step.getTCS_Order().ToString(CultureInfo.InvariantCulture);
                                    }

                                    Report.AddRow(
                                        name,
                                        reqRef.Paragraph.Text, "", "", "", "");
                                    Report.SetLastRowColor(IssueColor(reqRef.Paragraph));
                                    Report.lastRow.Cells[1].MergeRight = 3;
                                    sequenceRow.Cells[5].MergeDown += 1;
                                }
                            }
                        }
                    }
                }
            }
            Report.CloseSubParagraph();

            if (IncludeDetails && IncludeTestSequencesDetails)
            {
                Report.AddSubParagraph("Detailed sub sequence report");
                foreach (Frame frame in Dictionary.Tests)
                {
                    foreach (SubSequence subSequence in frame.SubSequences)
                    {
                        CreateSubSequenceDescription(subSequence);
                    }
                }
                Report.CloseSubParagraph();
            }

            Report.CloseSubParagraph();
        }

        /// <summary>
        /// Creates the description of the sub sequence
        /// </summary>
        /// <param name="subSequence"></param>
        private void CreateSubSequenceDescription(SubSequence subSequence)
        {
            Report.AddSubParagraph(subSequence.Name);
            Report.AddParagraph("Subsequence description");

            Report.AddTable(new[] { "Step", "Description", "Comment" }, new[] { 10, 80, 80 });
            foreach (TestCase testCase in subSequence.TestCases)
            {
                bool testCaseIntroduced = false;

                // Report test cases where requirements have been associated to
                if (testCase.Requirements.Count > 0)
                {
                    IntroduceTestCase(testCase);
                    testCaseIntroduced = true;
                }

                foreach (Step step in testCase.Steps)
                {
                    if (step.getTCS_Order() != 0 || step.Requirements.Count > 0)
                    {
                        if (!testCaseIntroduced)
                        {
                            // Report test cases when there are relevant steps in it
                            IntroduceTestCase(testCase);
                            testCaseIntroduced = true;
                        }

                        Report.AddRow(
                            step.getTCS_Order().ToString(CultureInfo.InvariantCulture),
                            step.getDescription(),
                            step.Comment);

                        ReportIssue(step);
                    }
                }
            }

            Report.CloseSubParagraph();
        }

        /// <summary>
        /// Creates a table entry for a test case
        /// </summary>
        /// <param name="testCase"></param>
        private void IntroduceTestCase(TestCase testCase)
        {
            Report.AddRow(testCase.Name);
            Report.SetLastRowColor(Colors.LightBlue);
            ReportIssue(testCase);
        }

        /// <summary>
        /// Reports an issue for an element which can reference a paragraph
        /// </summary>
        /// <param name="item"></param>
        private void ReportIssue(ReferencesParagraph item)
        {
            foreach (ReqRef reqRef in item.Requirements)
            {
                Report.AddRow(reqRef.Paragraph.Text);
                Report.SetLastRowColor(IssueColor(reqRef.Paragraph));
            }
        }

        /// <summary>
        /// Summarises the findings found on the test sequences
        /// </summary>
        private void CreateFindingsReport()
        {
            if (IncludeDetails)
            {
                Report.AddSubParagraph("Findings and comments");
                Report.AddParagraph(
                    "This section presents the findings and the comments on the test cases. This information has been gathered while translating test sequences as described in the Access database files into executable test sequences and by executing such tests against the Subset-026 model.");

                foreach (Specification specification in Dictionary.Specifications)
                {
                    foreach (Chapter chapter in specification.Chapters)
                    {
                        Report.AddSubParagraph(chapter.Name);

                        foreach (Paragraph paragraph in chapter.Paragraphs)
                        {
                            DescribeSpecIssue(paragraph);
                        }
                        Report.CloseSubParagraph();
                    }
                }
                Report.CloseSubParagraph();
            }
        }

        /// <summary>
        /// Describes a specific Subset-076 specification issue
        /// </summary>
        /// <param name="paragraph"></param>
        private void DescribeSpecIssue(Paragraph paragraph)
        {
            if (paragraph.SubParagraphs.Count > 0)
            {
                Report.AddSubParagraph(paragraph.Name);
                Report.AddTable(new[] { "SubSequence", "Test case", "Step", "Comment" }, new[] { 60, 20, 10, 80 });
                foreach (Paragraph subParagraph in paragraph.SubParagraphs)
                {
                    DescribeSpecIssue(subParagraph);
                }
                Report.CloseSubParagraph();
            }
            else
            {
                Report.AddRow(paragraph.ExpressionText);
                Report.SetLastRowColor(IssueColor(paragraph));

                if (paragraph.Implementations.Count > 0)
                {
                    foreach (ReqRef reqRef in paragraph.Implementations)
                    {
                        SubSequence subSequence = EnclosingFinder<SubSequence>.find(reqRef, true);
                        TestCase testCase = EnclosingFinder<TestCase>.find(reqRef, true);
                        Step step = EnclosingFinder<Step>.find(reqRef, true);

                        Report.AddRow(
                            subSequence != null ? subSequence.Name : "",
                            testCase != null ? testCase.getFeature().ToString(CultureInfo.InvariantCulture) : "",
                            step != null ? step.getTCS_Order().ToString(CultureInfo.InvariantCulture) : "",
                            reqRef.Comment);
                    }
                }
            }
        }

        /// <summary>
        /// Provides the color associated to the issue kind
        /// </summary>
        /// <param name="issue"></param>
        /// <returns></returns>
        public static Color IssueColor(Paragraph issue)
        {
            return IssueColor(IssueKindUtil.GetKind(issue));
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