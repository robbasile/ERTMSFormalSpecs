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
using System.Reflection;
using DataDictionary;
using DataDictionary.Specification;
using DataDictionary.Tests;
using DataDictionary.Tests.Translations;
using log4net;
using MigraDoc.DocumentObjectModel;
using Paragraph = DataDictionary.Specification.Paragraph;

namespace Reports.Specs
{
    internal class FindingsReport : ReportTools
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool ReviewedParagraphs { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public FindingsReport(Document document)
            : base(document)
        {
            ReviewedParagraphs = true;
        }

        // For each element in this report, have a Create method that does the paragraphing stuff and a 
        // Generate method, that involves loops and stuff

        /// <summary>
        ///     Provides the first-level paragraph under a chapter with the provided name
        /// </summary>
        /// <param name="aSectionName"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private Paragraph getSection(string aSectionName, Dictionary dictionary)
        {
            Paragraph retVal = new Paragraph();
            foreach (Specification specification in dictionary.Specifications)
            {
                foreach (Chapter chapter in specification.Chapters)
                {
                    foreach (Paragraph paragraph in chapter.Paragraphs)
                    {
                        if (paragraph.ExpressionText == aSectionName)
                        {
                            retVal = paragraph;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates an article for the questions
        /// </summary>
        /// <param name="aReportConfig"></param>
        public void CreateQuestionsArticle(FindingsReportHandler aReportConfig)
        {
            AddSubParagraph("Questions related to Subset-076");
            AddParagraph("This section contains the " +
                         getSection("Questions", aReportConfig.Dictionary).SubParagraphs.Count +
                         " questions raised in translating the Subset-076 test sequences.");
            GenerateQuestions(aReportConfig.Dictionary);
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates an article for the remarks
        /// </summary>
        /// <param name="aReportConfig"></param>
        public void CreateCommentsArticle(FindingsReportHandler aReportConfig)
        {
            AddSubParagraph("Comments for Subset-076");
            AddParagraph("This section contains the " +
                         getSection("Comments", aReportConfig.Dictionary).SubParagraphs.Count +
                         " comments for the Subset-076 test sequences. " +
                         "Comments are findings that are minor bugs, that could be assumptions the ERTMS Solutions team is not aware of, or information we feel is worth sharing in the present document.");
            GenerateComments(aReportConfig.Dictionary);
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates an article for the bugs
        /// </summary>
        /// <param name="aReportConfig"></param>
        public void CreateIssuesArticle(FindingsReportHandler aReportConfig)
        {
            AddSubParagraph("Issues for Subset-076");
            AddParagraph("This section contains the " + getSection("Bugs", aReportConfig.Dictionary).SubParagraphs.Count +
                         " discovered issues for the Subset-076 test sequences");
            AddParagraph(
                "These are the findings that are errors in the test sequences. They prevent execution of the test sequence.");
            GenerateIssues(aReportConfig.Dictionary);
            CloseSubParagraph();
        }


        /// <summary>
        ///     Creates a table for the questions
        /// </summary>
        /// <param name="aDictionary"></param>
        public void GenerateQuestions(Dictionary aDictionary)
        {
            AddSubParagraph("");
            Paragraph questions = getSection("Questions", aDictionary);
            foreach (Paragraph subparagraph in questions.SubParagraphs)
            {
                addEntry(subparagraph, "Question");
            }
            CloseSubParagraph();
        }


        private void addEntry(Paragraph paragraph, string entryType)
        {
            if (paragraph.getReviewed() == ReviewedParagraphs)
            {
                if (!paragraph.isTitle)
                {
                    AddSubParagraph(entryType + " " + paragraph.FullId);
                    AddParagraph(paragraph.ExpressionText);

                    // provide the translations the paragraph references
                    AddTestCases(paragraph);
                }
                else
                {
                    AddSubParagraph(entryType + " " + paragraph.FullId + ": " + paragraph.ExpressionText);
                }

                CloseSubParagraph();
            }

            if (paragraph.SubParagraphs.Count > 0)
            {
                foreach (Paragraph subParagraph in paragraph.SubParagraphs)
                {
                    addEntry(subParagraph, entryType);
                }
            }
        }

        private void addImplementations(Paragraph subparagraph)
        {
            bool first = true;
            foreach (ReqRef reference in subparagraph.Implementations)
            {
                Translation translation = reference.Model as Translation;
                if (translation != null)
                {
                    if (first)
                    {
                        AddTable(new string[] {"Related translations"}, new int[] {100});
                        first = false;
                    }
                    foreach (SourceText sourceText in translation.SourceTexts)
                    {
                        AddRow(sourceText.ExpressionText);
                        foreach (SourceTextComment comment in sourceText.Comments)
                        {
                            AddRow(comment.ExpressionText);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Builds the table of implementations of a paragraph
        /// </summary>
        /// <param name="subparagraph"></param>
        private void AddTestCases(Paragraph subparagraph)
        {
            Dictionary<string, string[]> TestFeatures = findSteps(subparagraph);

            if (TestFeatures.Count > 0)
            {
                AddTable(new string[] {"Test case", "Sequence", "Steps"}, new int[] {40, 65, 40});

                foreach (KeyValuePair<string, string[]> testFT in TestFeatures)
                {
                    AddRow(new string[] {testFT.Key, testFT.Value[1], testFT.Value[0]});
                }
            }
        }

        /// <summary>
        ///     Builds a dictionary of sequences containing a given test case, from the implementations of a paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        private Dictionary<string, string[]> findSteps(Paragraph paragraph)
        {
            Dictionary<string, string[]> retVal = new Dictionary<string, string[]>();

            foreach (ReqRef reference in paragraph.Implementations)
            {
                Step step = reference.Model as Step;
                if (step != null)
                {
                    if (retVal.ContainsKey(step.TestCase.Name))
                    {
                        string steps = retVal[step.TestCase.Name][0];
                        string sequences = retVal[step.TestCase.Name][1];

                        // Only add the subsequence if it is not already in the string
                        if (sequences.IndexOf(step.SubSequence.Name) == -1)
                        {
                            sequences = sequences + "\n" + step.SubSequence.Name;
                            steps = steps + "\n" + stepNumber(step);
                        }
                        else
                        {
                            int line = getLine(sequences, step.SubSequence.Name);
                            if (!stepPresent(line, step, steps))
                            {
                                steps = steps.Insert(stepIndex(line, steps), ", " + stepNumber(step));
                            }
                        }

                        retVal[step.TestCase.Name][0] = steps;
                        retVal[step.TestCase.Name][1] = sequences;
                    }
                    else
                    {
                        retVal[step.TestCase.Name] = new string[] {stepNumber(step), step.SubSequence.Name};
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     indicates whether a given line contains the step number provided
        /// </summary>
        /// <param name="line"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private bool stepPresent(int line, Step step, string steps)
        {
            bool retVal = false;

            if (steps.Substring(stepIndex(line, steps)).Contains("\n"))
            {
                retVal =
                    steps.Substring(stepIndex(line, steps), stepIndex(line + 1, steps) - stepIndex(line, steps))
                        .Contains(stepNumber(step));
            }
            else
            {
                retVal = steps.Substring(stepIndex(line, steps)).Contains(stepNumber(step));
            }
            return retVal;
        }

        /// <summary>
        ///     Provides the step number, if the name of the step begins with "Step #:"
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private string stepNumber(Step step)
        {
            string retVal = "";

            if (step.Name.IndexOf(":") != -1)
            {
                retVal = step.Name.Substring(0, step.Name.IndexOf(":"));
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the index of the point wher ea new step name needs to be inserted in a list of steps
        /// </summary>
        /// <param name="stepsList"></param>
        /// <returns></returns>
        private int stepIndex(int line, string stepslist)
        {
            int retVal = 0;

            // Get the position of each newline character, up to the one preceding the line we want to write on
            for (int i = line; i > 0; i--)
            {
                retVal = stepslist.IndexOf("\n", retVal) + 1;
            }

            // If the line we want is the last line, return the final index of the string,
            // otherwise, return the next newline
            if (stepslist.IndexOf("\n", retVal) == -1)
            {
                retVal = stepslist.Length;
            }
            else
            {
                retVal = stepslist.IndexOf("\n", retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     returns the number of line returns before the given string
        /// </summary>
        /// <param name="sequencesList"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private int getLine(string sequencesList, string sequence)
        {
            int retVal = 0;

            int sequenceIndex = sequencesList.IndexOf(sequence);

            int startIndex = 0;

            while (sequencesList.IndexOf("\n", startIndex, sequenceIndex - startIndex) != -1)
            {
                startIndex = sequencesList.IndexOf("\n", startIndex, sequenceIndex - startIndex) + 1;
                retVal++;
            }


            return retVal;
        }

        /// <summary>
        ///     Creates a table for the remarks
        /// </summary>
        /// <param name="aDictionary"></param>
        public void GenerateComments(Dictionary aDictionary)
        {
            AddSubParagraph("");
            Paragraph questions = getSection("Comments", aDictionary);
            foreach (Paragraph subparagraph in questions.SubParagraphs)
            {
                addEntry(subparagraph, "Comment");
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates a table for the bugs
        /// </summary>
        /// <param name="aDictionary"></param>
        public void GenerateIssues(Dictionary aDictionary)
        {
            AddSubParagraph("");
            Paragraph questions = getSection("Bugs", aDictionary);
            foreach (Paragraph subparagraph in questions.SubParagraphs)
            {
                addEntry(subparagraph, "Issue");
            }
            CloseSubParagraph();
        }
    }
}