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
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Utils;
using Dictionary = DataDictionary.Dictionary;
using Frame = DataDictionary.Tests.Frame;
using Paragraph = DataDictionary.Specification.Paragraph;
using ReqRef = DataDictionary.ReqRef;
using ReqRelated = DataDictionary.ReqRelated;
using RequirementSet = DataDictionary.Specification.RequirementSet;
using Rule = DataDictionary.Rules.Rule;
using StateMachine = DataDictionary.Types.StateMachine;
using TestCase = DataDictionary.Tests.TestCase;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace Reports.Specs
{
    public class SpecCoverageReport : ReportTools
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="document"></param>
        public SpecCoverageReport(Document document)
            : base(document)
        {
        }

        /// <summary>
        ///     Creates an article with informations about all the paragraphs of the specification
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        public void CreateSpecificationArticle(SpecCoverageReportHandler aReportConfig)
        {
            AddSubParagraph("Specification coverage");
            GenerateStatistics(aReportConfig.Dictionary, true, false, true, true);

            if (aReportConfig.ShowFullSpecification)
            {
                AddSubParagraph("Coverage detail");
                CreateSpecificationTable(aReportConfig.Dictionary);
                CloseSubParagraph();
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates an article with informations about the covered requirements
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        public void CreateCoveredRequirementsArticle(SpecCoverageReportHandler aReportConfig)
        {
            AddSubParagraph("Covered requirements");
            GenerateStatistics(aReportConfig.Dictionary, true, false, true, false);

            HashSet<Paragraph> coveredParagraphs = CoveredRequirements(aReportConfig.Dictionary, true);
            if (coveredParagraphs.Count > 0)
                /* If we have some covered paragraphs, we create a section with informations about it */
            {
                AddSubParagraph("Coverage detail");
                CreateImplementedParagraphsTable(coveredParagraphs, aReportConfig.Dictionary);
                CloseSubParagraph();
            }
            CloseSubParagraph();
        }


        /// <summary>
        ///     Creates an article with informations about the non covered requirements
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        public void CreateNonCoveredRequirementsArticle(SpecCoverageReportHandler aReportConfig)
        {
            AddSubParagraph("Non covered requirements");
            GenerateStatistics(aReportConfig.Dictionary, false, true, true, false);

            HashSet<Paragraph> nonCoveredParagraphs = CoveredRequirements(aReportConfig.Dictionary, false);
            if (nonCoveredParagraphs.Count > 0)
                /* If we have some non covered paragraphs, we create a section containing the list of these paragraphs */
            {
                foreach (Paragraph paragraph in nonCoveredParagraphs)
                {
                    AddSubParagraph("Requirement " + paragraph.FullId);
                    AddTable(new string[] {"Requirement " + paragraph.FullId}, new int[] {100});
                    AddRow(paragraph.Text);
                    CloseSubParagraph();
                }
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates an article with informations about the implemented model elements
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        public void CreateReqRelatedArticle(SpecCoverageReportHandler aReportConfig)
        {
            AddSubParagraph("Model coverage");

            HashSet<ReqRelated> implementedReqRelated = aReportConfig.Dictionary.ImplementedReqRelated;
            HashSet<ReqRelated> implementedRules = new HashSet<ReqRelated>();
            HashSet<ReqRelated> implementedTypes = new HashSet<ReqRelated>();
            HashSet<ReqRelated> implementedVariables = new HashSet<ReqRelated>();

            ICollection<Paragraph> applicableParagraphs = aReportConfig.Dictionary.ApplicableParagraphs;
            HashSet<Paragraph> modeledParagraphs = new HashSet<Paragraph>();
            foreach (ReqRelated reqRelated in implementedReqRelated)
            {
                if (reqRelated is Rule)
                {
                    implementedRules.Add(reqRelated);
                }
                else if (reqRelated is Type)
                {
                    implementedTypes.Add(reqRelated);
                }
                else if (reqRelated is Variable)
                {
                    implementedVariables.Add(reqRelated);
                }
                modeledParagraphs.UnionWith(reqRelated.ModeledParagraphs);
            }
            double modeledPercentage = ((double) modeledParagraphs.Count/(double) applicableParagraphs.Count)*100;

            AddTable(new string[] {"Statistics"}, new int[] {70, 70});
            AddRow("Number of implemented model elements", implementedReqRelated.Count.ToString());
            AddRow("Number of modeled paragraphs",
                String.Format("{0} of {1} ({2:0.##}%)", modeledParagraphs.Count, applicableParagraphs.Count,
                    modeledPercentage));

            if (implementedRules.Count > 0)
            {
                /* This section will contain the list of implemented rules and possibly
                 * the list of paragraphs modeled by each rule */
                AddSubParagraph("Implemented rules");
                CreateReqRelatedTable("Implemented rules", implementedRules, aReportConfig.ShowAssociatedParagraphs);
                CloseSubParagraph();
            }
            if (implementedTypes.Count > 0)
            {
                /* This section will contain the list of implemented types and possibly
                 * the list of paragraphs modeled by each type */
                AddSubParagraph("Implemented types");
                CreateReqRelatedTable("Implemented types", implementedTypes, aReportConfig.ShowAssociatedParagraphs);
                CloseSubParagraph();
            }
            if (implementedVariables.Count > 0)
            {
                /* This section will contain the list of implemented variables and possibly
                 * the list of paragraphs modeled by each variable */
                AddSubParagraph("Implemented variables");
                CreateReqRelatedTable("Implemented variables", implementedVariables,
                    aReportConfig.ShowAssociatedParagraphs);
                CloseSubParagraph();
            }
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
            AddTable(new string[] {"Statistics"}, new int[] {70, 70});
            if (allParagraphs)
            {
                AddRow("Total number of paragraphs", aDictionary.AllParagraphs.Count.ToString());
            }
            if (applicableParagraphs)
            {
                AddRow("Number of applicable paragraphs", aDictionary.ApplicableParagraphs.Count.ToString());
            }

            double applicableParagraphsCount = aDictionary.ApplicableParagraphs.Count;
            double coveredParagraphsCount = CoveredRequirements(aDictionary, true).Count;
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
        }


        /// <summary>
        ///     Provides the set of covered requirements by the model
        /// </summary>
        /// <param name="aDictionary">The model</param>
        /// <param name="covered">Indicates if we need compute covered or non covered requirements</param>
        /// <returns></returns>
        public static HashSet<Paragraph> CoveredRequirements(Dictionary aDictionary, bool covered)
        {
            HashSet<Paragraph> retVal = new HashSet<Paragraph>();

            ICollection<Paragraph> applicableParagraphs = aDictionary.ApplicableParagraphs;
            Dictionary<Paragraph, List<ReqRef>> paragraphsReqRefDictionary = aDictionary.ParagraphsReqRefs;
            foreach (Paragraph paragraph in applicableParagraphs)
            {
                bool implemented = paragraph.getImplementationStatus() ==
                                   acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented;
                if (implemented)
                {
                    if (paragraphsReqRefDictionary.ContainsKey(paragraph))
                    {
                        List<ReqRef> implementations = paragraphsReqRefDictionary[paragraph];
                        for (int i = 0; i < implementations.Count; i++)
                        {
                            // the implementation may be also a ReqRef
                            if (implementations[i].Enclosing is ReqRelated)
                            {
                                ReqRelated reqRelated = implementations[i].Enclosing as ReqRelated;
                                // Do not consider tests
                                if (EnclosingFinder<Frame>.find(reqRelated) == null)
                                {
                                    implemented = implemented && reqRelated.ImplementationCompleted;
                                }
                            }
                        }
                    }
                }
                if (implemented == covered)
                {
                    retVal.Add(paragraph);
                }
            }
            return retVal;
        }


        /// <summary>
        ///     Creates a table resuming all requirements of the specification
        /// </summary>
        /// <param name="aDictionary">The model</param>
        /// <returns></returns>
        private void CreateSpecificationTable(Dictionary aDictionary)
        {
            AddTable(new string[] {"Model information"}, new int[] {40, 40, 30, 30});
            AddTableHeader("Requirement", "Target", "Type", "Implementation status");
            foreach (Paragraph paragraph in aDictionary.AllParagraphs)
            {
                string requirementSets = "";
                foreach (RequirementSet requirementSet in paragraph.ApplicableRequirementSets)
                {
                    requirementSets += requirementSet.Name + " ";
                }
                AddRow(paragraph.FullId, requirementSets, paragraph.getType_AsString(),
                    paragraph.getImplementationStatus_AsString());
            }
        }


        /// <summary>
        ///     Creates a table for a given set of paragraphs
        /// </summary>
        /// <param name="paragraphs">The paragraphs to display</param>
        /// <param name="dictionary">The dictionary</param>
        /// <returns></returns>
        private void CreateImplementedParagraphsTable(HashSet<Paragraph> paragraphs, Dictionary dictionary)
        {
            Dictionary<Paragraph, List<ReqRef>> paragraphsReqRefDictionary = dictionary.ParagraphsReqRefs;
            foreach (Paragraph paragraph in paragraphs)
            {
                Cell previousCell = null;

                if (paragraphsReqRefDictionary.ContainsKey(paragraph))
                {
                    AddSubParagraph("Requirement " + paragraph.FullId);
                    AddTable(new string[] {"Requirement " + paragraph.FullId}, new int[] {40, 60, 40});
                    AddRow(paragraph.Text);

                    foreach (ReqRef reqRef in paragraph.Implementations)
                    {
                        string fullName = null;
                        string comment = null;

                        ReqRelated reqRelated = reqRef.Enclosing as ReqRelated;
                        if (reqRelated != null)
                        {
                            if (reqRelated is TestCase) /* TODO: review it */
                            {
                                fullName = "TEST CASE " + reqRelated.Name;
                            }
                            else if (reqRelated is StateMachine) /* TODO: review it */
                            {
                                fullName = reqRelated.Name;
                            }
                            else
                            {
                                fullName = reqRelated.FullName;
                            }
                            comment = reqRelated.Comment;
                        }
                        else
                        {
                            Paragraph par = reqRef.Enclosing as Paragraph;
                            if (par != null) /* TODO: review it */
                            {
                                fullName = "PARAGRAPH " + paragraph.FullId;
                                comment = paragraph.Comment;
                            }
                        }

                        if (fullName != null && comment != null)
                        {
                            if (previousCell == null)
                            {
                                AddRow("Associated implementation", fullName, comment);
                                previousCell = lastRow.Cells[0];
                            }
                            else
                            {
                                if (AddRow("", fullName, comment) != null)
                                {
                                    previousCell.MergeDown += 1;
                                }
                                else
                                {
                                    throw new Exception("Error: tried to add an empty row in the spec coverage report");
                                }
                            }
                        }
                    }
                    CloseSubParagraph();
                }
            }
        }

        /// <summary>
        ///     Creates a table with implemented req related in the dictionary
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        private void CreateReqRelatedTable(string title, HashSet<ReqRelated> elements, bool showAssociatedParagraphs)
        {
            foreach (ReqRelated reqRelated in elements)
            {
                AddSubParagraph(reqRelated.FullName);
                AddTable(new string[] {reqRelated.FullName}, new int[] {40, 100});
                if (showAssociatedParagraphs)
                {
                    Row firstRow = null;
                    foreach (Paragraph paragraph in reqRelated.ModeledParagraphs)
                    {
                        if (firstRow == null)
                        {
                            AddRow("Associated paragraphs", paragraph.Name);
                            firstRow = lastRow;
                        }
                        else
                        {
                            AppendToRow(null, paragraph.Name);
                        }
                    }
                }
                CloseSubParagraph();
            }
        }
    }
}