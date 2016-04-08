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
using System.Globalization;
using DataDictionary;
using MigraDoc.DocumentObjectModel;
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
        Subseet76Report Report { get; set; }

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
            return Math.Round(number*100/(double)totalNumber) + " %";
        }

        /// <summary>
        /// Creates the section which describes the translated test and the result of application of those tests on the model
        /// </summary>
        private void CreateTestSequenceReport()
        {
            Report.AddSubParagraph("Sub sequence report");
            Report.AddParagraph(
                "This section presents the sub sequences that have been translated and applied on the Subset-026 model, along with their execution result and the issues found in the test sequence definition.");
            Report.AddParagraph("The following table presents summarises the test sequences that have been analysed, along with the number of issues found.");

            Counter counter = new Counter();
            counter.visit(Dictionary, true);

            Report.AddTable(new []{"", "Count", "Ratio"}, new []{100,35, 35});
            Report.AddRow(
                "Imported sequences", 
                counter.SubSequences.ToString(CultureInfo.InvariantCulture), 
                Percent(counter.SubSequences, 800));

            Report.AddRow(
                "Sequence analysis completed", 
                counter.CompletedSubSequences.Count.ToString(CultureInfo.InvariantCulture),
                Percent(counter.CompletedSubSequences.Count, counter.SubSequences));

            Report.AddRow(
                "Invalid sequences",
                counter.BlockingSubSequences.Count.ToString(CultureInfo.InvariantCulture),
                Percent(counter.BlockingSubSequences.Count, counter.SubSequences));

            Report.AddRow(
                "Total number of issues found",
                counter.Issues.ToString(CultureInfo.InvariantCulture),
                Math.Round(counter.Issues/(double)counter.SubSequences).ToString(CultureInfo.InvariantCulture));

            Report.AddSubParagraph("Summary");
            Report.AddParagraph("This section summarises the results for each sub sequence");
            foreach (Frame frame in Dictionary.Tests)
            {
                Report.AddTable(new[] { "Subsequence", "Actions", "Checks", "Issues", "Status" }, new[] { 60, 30, 30, 30, 30 });

                foreach (SubSequence subSequence in frame.SubSequences)
                {
                    counter = new Counter();
                    counter.visit(subSequence, true);

                    string status = "Ongoing";
                    Color color = Colors.Orange;
                    if (counter.BlockingIssues > 0)
                    {
                        status = "Invalid";
                        color = Colors.Red;
                    }
                    else
                    {
                        if (subSequence.getCompleted())
                        {
                            status = "Completed";
                            color = Colors.Green;
                        }
                    }

                    Report.AddRow(
                        subSequence.Name, 
                        counter.Actions.ToString(CultureInfo.InvariantCulture),
                        counter.Checks.ToString(CultureInfo.InvariantCulture),
                        counter.Issues.ToString(CultureInfo.InvariantCulture),
                        status);

                    Report.lastRow.Cells[4].Shading.Color = color;

                    if (counter.Issues > 0)
                    {
                        foreach (TestCase testCase in subSequence.TestCases)
                        {
                            foreach (ReqRef reqRef in testCase.Requirements)
                            {
                                Report.AddRow(
                                    testCase.Name,
                                    reqRef.Paragraph.Text);
                                Report.SetLastRowColor(Colors.IndianRed);
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
                                        reqRef.Paragraph.Text);

                                    Report.SetLastRowColor(Colors.IndianRed);
                                }
                            }
                        }
                    }
                }
            }
            Report.CloseSubParagraph();

            Report.AddSubParagraph("Detailed sub sequence report");
            foreach (Frame frame in Dictionary.Tests)
            {
                foreach (SubSequence subSequence in frame.SubSequences)
                {
                    CreateSubSequenceDescription(subSequence);
                }
            }
            Report.CloseSubParagraph();

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
                Report.SetLastRowColor(Colors.IndianRed);
            }
        }

        /// <summary>
        /// Summarises the findings found on the test sequences
        /// </summary>
        private void CreateFindingsReport()
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
                Report.SetLastRowColor(Colors.LightGray);

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
    }
}