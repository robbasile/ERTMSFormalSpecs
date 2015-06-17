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
using DataDictionary;
using DataDictionary.Rules;
using DataDictionary.Tests;
using DataDictionary.Tests.Runner;
using MigraDoc.DocumentObjectModel;

namespace Reports.Tests
{
    public class TestsCoverageReportHandler : ReportHandler
    {
        /// <summary>
        ///     The system for which this report is built
        /// </summary>
        private EFSSystem __efsSystem;

        public override EFSSystem EFSSystem
        {
            get
            {
                EFSSystem retVal;

                if (Dictionary == null)
                {
                    retVal = __efsSystem;
                }
                else
                {
                    retVal = base.EFSSystem;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="system"></param>
        public TestsCoverageReportHandler(EFSSystem system)
            : base(null)
        {
            __efsSystem = system;

            init();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public TestsCoverageReportHandler(Dictionary dictionary)
            : base(dictionary)
        {
            init();
        }

        private void init()
        {
            createFileName("DynamicTestCoverageReport");

            AddFrames = false;
            AddActivatedRulesInFrames = false;

            AddSubSequences = false;
            AddActivatedRulesInSubSequences = false;

            AddTestCases = false;
            AddActivatedRulesInTestCases = false;

            AddSteps = false;
            AddActivatedRulesInSteps = false;
        }

        /// <summary>
        ///     Creates a report on tests coverage, according to user's choices
        /// </summary>
        /// <returns>The document created, or null</returns>
        public override Document BuildDocument()
        {
            Document retVal = new Document();

            Log.Info("Creating test report");
            retVal.Info.Title = "EFS Test report";
            retVal.Info.Author = "ERTMS Solutions";
            retVal.Info.Subject = "Test report";

            TestsCoverageReport report = new TestsCoverageReport(retVal);
            Log.Info("..gathering requirement coverage");
            report.CreateRequirementCoverageArticle(this);
            HashSet<RuleCondition> activatedRules = new HashSet<RuleCondition>();
            if (TestCase != null) /* We generate a report for a selected test case */
            {
                Log.Info("..creating test case report " + TestCase.Name);
                EFSSystem.Runner = new Runner(TestCase.SubSequence, false, false, true);
                Dictionary = TestCase.Dictionary;
                report.CreateTestCaseSection(EFSSystem.Runner, TestCase, this, activatedRules, true);
            }
            else if (SubSequence != null) /* We generate a report of a selected sub sequence */
            {
                Log.Info("..creating sub sequence report " + SubSequence.Name);
                Dictionary = SubSequence.Dictionary;
                report.CreateSubSequenceSection(SubSequence, this, activatedRules, true);
            }
            else if (Frame != null) /* We generate a report for a selected frame */
            {
                Log.Info("..creating frame report " + Frame.Name);
                Dictionary = Frame.Dictionary;

                report.CreateFrameArticle(Frame, this, activatedRules);
            }
            else if (Dictionary != null) /* We generate a full report */
            {
                Log.Info("..creating dictionary report ");
                foreach (Frame frame in Dictionary.Tests)
                {
                    report.CreateFrameArticle(frame, this, activatedRules);
                }
            }

            return retVal;
        }

        public bool AddFrames { set; get; }
        public bool AddActivatedRulesInFrames { set; get; }
        public Frame Frame { set; get; } /* if Frame is defined, we execute all its sub sequences */

        public bool AddSubSequences { set; get; }
        public bool AddActivatedRulesInSubSequences { set; get; }
        public SubSequence SubSequence { set; get; } /* if SubSequence is defined, we execute all its test cases */

        public bool AddTestCases { set; get; }
        public bool AddActivatedRulesInTestCases { set; get; }
        public TestCase TestCase { set; get; } /* if TestCase is defined, we execute all its steps */

        public bool AddSteps { set; get; }
        public bool AddActivatedRulesInSteps { set; get; }
    }
}