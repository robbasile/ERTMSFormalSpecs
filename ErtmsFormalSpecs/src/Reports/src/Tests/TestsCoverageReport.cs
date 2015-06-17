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
using System.Collections.Generic;
using DataDictionary.Generated;
using DataDictionary.Tests.Runner;
using MigraDoc.DocumentObjectModel;
using Dictionary = DataDictionary.Dictionary;
using Frame = DataDictionary.Tests.Frame;
using Paragraph = DataDictionary.Specification.Paragraph;
using ReqRef = DataDictionary.ReqRef;
using ReqRelated = DataDictionary.ReqRelated;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using Step = DataDictionary.Tests.Step;
using SubSequence = DataDictionary.Tests.SubSequence;
using SubStep = DataDictionary.Tests.SubStep;
using TestCase = DataDictionary.Tests.TestCase;

namespace Reports.Tests
{
    public class TestsCoverageReport : ReportTools
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="document"></param>
        public TestsCoverageReport(Document document)
            : base(document)
        {
        }

        /// <summary>
        ///     Creates an article with informations about all the paragraphs of the specification
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        public void CreateRequirementCoverageArticle(TestsCoverageReportHandler aReportConfig)
        {
            AddSubParagraph("Test coverage");

            /* This section will contain the statistics on the modeled paragraphs in the dictionary:
             * - their number and percentage */
            GenerateStatistics(aReportConfig.Dictionary, true, false, true, true);
            CloseSubParagraph();
        }


        /// <summary>
        ///     Generates a table with specification coverage statistics
        /// </summary>
        /// <param name="aDictionary">The model</param>
        /// <param name="coveredParagraphs">Number and percentage of covered paragraphs</param>
        /// <param name="nonCoveredParagraphs">Number and percentage of non covered paragraphs</param>
        /// <param name="applicableParagraphs">Number of applicable paragraphs</param>
        /// <param name="allParagraphs">Total number of paragraphs</param>
        /// <returns></returns>
        private void GenerateStatistics(Dictionary aDictionary, bool coveredParagraphs, bool nonCoveredParagraphs,
            bool applicableParagraphs, bool allParagraphs)
        {
            AddSubParagraph("Statistics");
            AddTable(new string[] {"", "Value"}, new int[] {70, 70});

            if (allParagraphs)
            {
                AddRow("Total number of paragraphs", aDictionary.AllParagraphs.Count.ToString());
            }


            if (applicableParagraphs)
            {
                AddRow("Number of applicable paragraphs", aDictionary.ApplicableParagraphs.Count.ToString());
            }

            double applicableParagraphsCount = aDictionary.ApplicableParagraphs.Count;
            double coveredParagraphsCount = CoveredRequirements(aDictionary).Count;
            double coveredPercentage = (coveredParagraphsCount/applicableParagraphsCount)*100;
            double nonCoveredParagraphsCount = applicableParagraphsCount - coveredParagraphsCount;
            double nonCoveredPercentage = 100 - coveredPercentage;

            if (coveredParagraphs)
            {
                AddRow("Number of covered requirements",
                    String.Format("{0} ({1:0.##}%)", coveredParagraphsCount, coveredPercentage));
            }

            if (nonCoveredParagraphs)
            {
                AddRow("Number of non covered requirements",
                    String.Format("{0} ({1:0.##}%)", nonCoveredParagraphsCount, nonCoveredPercentage));
            }

            CloseSubParagraph();
        }


        /// <summary>
        ///     Creates an article for a given frame
        /// </summary>
        /// <param name="aFrame">Frame to be displayed</param>
        /// <param name="aReportConfig">The report configuration containing display details</param>
        /// <param name="activatedRules">The list that will contain the rules activated by this frame</param>
        /// <returns></returns>
        public void CreateFrameArticle(Frame aFrame, TestsCoverageReportHandler aReportConfig,
            HashSet<RuleCondition> activatedRules)
        {
            string title = "Frame " + aFrame.Name;

            AddSubParagraph(title);

            foreach (SubSequence subSequence in aFrame.SubSequences)
            {
                // SIDE EFFECT:
                // each sub-sequence will calculate the list of rules it activates
                // and add them to activatedRules list
                CreateSubSequenceSection(subSequence, aReportConfig, activatedRules, aReportConfig.AddSubSequences);
            }

            // now we  can create the section containing the activated rules of the frame
            CreateActivatedRulesSection(title,
                activatedRules,
                aReportConfig.Dictionary.ImplementedRules,
                aReportConfig.AddActivatedRulesInFrames);
            CloseSubParagraph();
        }


        /// <summary>
        ///     Creates a section for a given sub-sequence
        /// </summary>
        /// <param name="aSubSequence">A sub-sequence to be displayed</param>
        /// <param name="aReportConfig">The report configuration containing display details</param>
        /// <param name="activatedRules">The list that will contain the rules activated by this sub-sequence</param>
        /// <param name="createPdf">Indicates if the information about this sub-sequence has to be added to the pdf</param>
        public void CreateSubSequenceSection(SubSequence aSubSequence, TestsCoverageReportHandler aReportConfig,
            HashSet<RuleCondition> activatedRules, bool createPdf)
        {
            string title = "Sub sequence " + aSubSequence.Name;
            if (createPdf)
            {
                AddSubParagraph(title);
            }

            HashSet<RuleCondition> rules = new HashSet<RuleCondition>();
            aSubSequence.EFSSystem.Runner = new Runner(aSubSequence, false, false, true);
            foreach (TestCase testCase in aSubSequence.TestCases)
            {
                // SIDE EFFECT:
                // each test case will calculate the list of rules it activates
                // and add them to activatedRules list
                CreateTestCaseSection(aSubSequence.EFSSystem.Runner, testCase, aReportConfig, rules,
                    createPdf && aReportConfig.AddTestCases);
            }
            activatedRules.UnionWith(rules);

            // now we  can create the table with the current sub sequence statistics
            if (createPdf)
            {
                CreateActivatedRulesSection(title,
                    rules,
                    aReportConfig.Dictionary.ImplementedRules,
                    aReportConfig.AddActivatedRulesInSubSequences);

                CloseSubParagraph();
            }
        }


        /// <summary>
        ///     Creates a section for a given test case
        /// </summary>
        /// <param name="runner">The runner to be used to execute the tests</param>
        /// <param name="aTestCase">Test case to be displayed</param>
        /// <param name="aReportConfig">The report configuration containing display details</param>
        /// <param name="activatedRuleConditions">The list that will contain the rules activated by this test case</param>
        /// <param name="createPdf">Indicates if the information about this sub-sequence has to be added to the pdf</param>
        public void CreateTestCaseSection(Runner runner, TestCase aTestCase, TestsCoverageReportHandler aReportConfig,
            HashSet<RuleCondition> activatedRuleConditions, bool createPdf)
        {
            string title = "Test case " + aTestCase.Name;
            if (createPdf)
            {
                AddSubParagraph(title);

                if (aTestCase.Requirements.Count > 0)
                {
                    AddSubParagraph(title + ": verified requirements:");
                    foreach (ReqRef reqRef in aTestCase.Requirements)
                    {
                        string text = "Requirement " + reqRef.Name;
                        if (!Utils.Utils.isEmpty(reqRef.Comment))
                        {
                            text = text + " : " + reqRef.Comment;
                        }
                        AddListItem(text);
                    }
                    CloseSubParagraph();
                }
            }

            runner.RunUntilStep(null);
            activatedRuleConditions.UnionWith(runner.EventTimeLine.GetActivatedRules());


            if (createPdf)
            {
                if (aReportConfig.AddSteps)
                {
                    foreach (Step step in aTestCase.Steps)
                    {
                        if (step.SubSteps.Count > 0)
                        {
                            SubStep firstSubStep = step.SubSteps[0] as SubStep;
                            SubStep lastSubStep = step.SubSteps[step.SubSteps.Count - 1] as SubStep;
                            double start = runner.EventTimeLine.GetSubStepActivationTime(firstSubStep);
                            double end = runner.EventTimeLine.GetNextSubStepActivationTime(lastSubStep);
                            List<RuleCondition> activatedRules = runner.EventTimeLine.GetActivatedRulesInRange(start,
                                end);

                            CreateStepSection(step, activatedRules, aReportConfig);
                        }
                    }
                }


                CreateActivatedRulesSection(title,
                    runner.EventTimeLine.GetActivatedRules(),
                    aReportConfig.Dictionary.ImplementedRules,
                    aReportConfig.AddActivatedRulesInTestCases);

                CloseSubParagraph();
            }
        }


        /// <summary>
        ///     Creates a section for a given step
        /// </summary>
        /// <param name="aStep">The step to be displayed</param>
        /// <param name="activatedRules">The list of rules activated by this step</param>
        /// <param name="aReportConfig">The report config</param>
        private void CreateStepSection(Step aStep, List<RuleCondition> activatedRules,
            TestsCoverageReportHandler aReportConfig)
        {
            string title = "Step " + aStep.Name;
            AddSubParagraph(title);

            CreateActivatedRulesSection("",
                new HashSet<RuleCondition>(activatedRules),
                aReportConfig.Dictionary.ImplementedRules,
                aReportConfig.AddActivatedRulesInTestCases);

            CloseSubParagraph();
        }


        /// <summary>
        ///     Adds information about the activated rules of a frame, a section, a test case or a step
        /// </summary>
        /// <param name="activatedRules">The list of activated rules</param>
        /// <param name="implementedRules">The list of implemented rules</param>
        private void CreateActivatedRulesSection(string title, HashSet<RuleCondition> activatedRules,
            HashSet<Rule> implementedRules, bool addActivatedRules)
        {
            double implementedPercentage = (double) ((double) activatedRules.Count/(double) implementedRules.Count)*100;
            string titleOfParagraph;
            if (title != "")
            {
                titleOfParagraph = String.Format("{0}: activated rules: {1} ({2:0.##}%)", title,
                    activatedRules.Count.ToString(), implementedPercentage);
            }
            else
            {
                titleOfParagraph = String.Format("Activated rules: {0} ({1:0.##}%)", activatedRules.Count.ToString(),
                    implementedPercentage);
            }

            if (addActivatedRules)
            {
                AddSubParagraph(titleOfParagraph);
                foreach (RuleCondition ruleCondition in activatedRules)
                {
                    AddParagraph(ruleCondition.FullName);
                }
                CloseSubParagraph();
            }
            else
            {
                AddParagraph(titleOfParagraph);
            }
        }


        /// <summary>
        ///     Provides the set of covered requirements by the tests
        /// </summary>
        /// <param name="aDictionary">The model</param>
        /// <returns></returns>
        public static HashSet<Paragraph> CoveredRequirements(Dictionary aDictionary)
        {
            HashSet<Paragraph> retVal = new HashSet<Paragraph>();
            ICollection<Paragraph> applicableParagraphs = aDictionary.ApplicableParagraphs;
            Dictionary<Paragraph, List<ReqRef>> paragraphsReqRefDictionary = aDictionary.ParagraphsReqRefs;

            foreach (Paragraph paragraph in applicableParagraphs)
            {
                bool implemented = paragraph.getImplementationStatus() ==
                                   acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented;
                bool tested = false;
                if (implemented)
                {
                    if (paragraphsReqRefDictionary.ContainsKey(paragraph))
                    {
                        List<ReqRef> implementations = paragraphsReqRefDictionary[paragraph];
                        for (int i = 0; i < implementations.Count; i++)
                        {
                            ReqRelated reqRelated = implementations[i].Enclosing as ReqRelated;
                            if (reqRelated is TestCase && reqRelated.ImplementationCompleted == true)
                            {
                                tested = true;
                            }
                        }
                    }
                }

                if (implemented && tested)
                {
                    retVal.Add(paragraph);
                }
            }

            return retVal;
        }
    }
}