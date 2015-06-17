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

using System.Reflection;
using DataDictionary;
using DataDictionary.Specification;
using DataDictionary.Tests;
using log4net;
using MigraDoc.DocumentObjectModel;
using Paragraph = DataDictionary.Specification.Paragraph;

namespace Reports.Specs
{
    public class SpecIssuesReport : ReportTools
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="document"></param>
        public SpecIssuesReport(Document document)
            : base(document)
        {
        }

        /// <summary>
        ///     Counts the number of requirements where more information is needed
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static int CountMoreInformationNeeded(Dictionary dictionary)
        {
            int retVal = 0;

            foreach (Specification specification in dictionary.Specifications)
            {
                retVal += specification.MoreInformationNeeded.Count;
            }

            return retVal;
        }

        /// <summary>
        ///     Creates an article with the requirements which require more information
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        public void CreateMoreInformationArticle(SpecIssuesReportHandler aReportConfig)
        {
            AddSubParagraph("More information needed");
            AddParagraph("This report describes the requirements " +
                         CountMoreInformationNeeded(aReportConfig.Dictionary) +
                         " that are not sufficiently precise and require more information. ");
            GenerateMoreInformationNeeded(aReportConfig.Dictionary);
            CloseSubParagraph();
        }

        /// <summary>
        ///     Counts the number of spec issues in a dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static int CountSpecIssues(Dictionary dictionary)
        {
            int retVal = 0;

            foreach (Specification specification in dictionary.Specifications)
            {
                retVal += specification.SpecIssues.Count;
            }

            return retVal;
        }

        /// <summary>
        ///     Creates an article with informations about all the paragraphs of the specification
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        public void CreateSpecIssuesArticle(SpecIssuesReportHandler aReportConfig)
        {
            AddSubParagraph("Specification issues report");
            AddParagraph("This report describes the specification " + CountSpecIssues(aReportConfig.Dictionary) +
                         " issues encountered during modeling. ");
            GenerateSpecIssues(aReportConfig.Dictionary);
            CloseSubParagraph();
        }

        /// <summary>
        ///     Counts the number of design choices in a dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static int CountDesignChoices(Dictionary dictionary)
        {
            int retVal = 0;

            foreach (Specification specification in dictionary.Specifications)
            {
                retVal += specification.DesignChoices.Count;
            }

            return retVal;
        }

        /// <summary>
        ///     Creates an article with informations about all the paragraphs of the specification
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        public void CreateDesignChoicesArticle(SpecIssuesReportHandler aReportConfig)
        {
            AddSubParagraph("Design choices report");
            AddParagraph("This report describes the " + CountDesignChoices(aReportConfig.Dictionary) +
                         " design choices made during modeling of " + aReportConfig.Dictionary.Name);
            GenerateDesignChoices(aReportConfig.Dictionary);
            CloseSubParagraph();
        }

        /// <summary>
        ///     Counts the number of design choices in a dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static int CountComments(Dictionary dictionary)
        {
            int retVal = 0;

            foreach (Specification specification in dictionary.Specifications)
            {
                retVal += specification.OnlyComments.Count;
            }

            return retVal;
        }

        /// <summary>
        ///     Creates an article with the comments on the requirements
        /// </summary>
        /// <param name="aReportConfig">The report config containing user's choices</param>
        /// <returns></returns>
        public void CreateCommentsArticle(SpecIssuesReportHandler aReportConfig)
        {
            AddSubParagraph("Comments on requirements");
            AddParagraph("This report describes the " + CountComments(aReportConfig.Dictionary) +
                         " comments we made on the requirements requirements.");
            GenerateComments(aReportConfig.Dictionary);
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates a table for more information needed
        /// </summary>
        /// <param name="aDictionary">The model</param>
        /// <returns></returns>
        private void GenerateMoreInformationNeeded(Dictionary aDictionary)
        {
            AddSubParagraph("More information needed");
            foreach (Paragraph paragraph in aDictionary.MoreInformationNeeded)
            {
                AddSubParagraph(paragraph.FullId + " is not precise enough");
                AddTable(new string[] {paragraph.FullId}, new int[] {30, 100});
                AddRow("Description", paragraph.Text);
                AddRow("Comment", paragraph.Comment);
                CloseSubParagraph();
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates a table for specification issues
        /// </summary>
        /// <param name="aDictionary">The model</param>
        /// <returns></returns>
        private void GenerateSpecIssues(Dictionary aDictionary)
        {
            AddSubParagraph("Specification issues");
            foreach (Paragraph paragraph in aDictionary.SpecIssues)
            {
                AddSubParagraph("Issue on " + paragraph.FullId);
                AddTable(new string[] {"Issue on " + paragraph.FullId}, new int[] {30, 100});
                AddRow("Description", paragraph.Text);
                AddRow("Comment", paragraph.Comment);
                CloseSubParagraph();
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates a table for design choices
        /// </summary>
        /// <param name="aDictionary">The model</param>
        /// <returns></returns>
        private void GenerateDesignChoices(Dictionary aDictionary)
        {
            AddSubParagraph("Design choices");
            foreach (Paragraph paragraph in aDictionary.DesignChoices)
            {
                AddSubParagraph("Design choice " + paragraph.FullId);
                AddTable(new string[] {"Design choice " + paragraph.FullId}, new int[] {60, 100});
                AddRow(paragraph.Text);

                // If the paragraph references steps, indicate them
                bool first = true;
                foreach (ReqRef refParagraph in paragraph.Implementations)
                {
                    Step step = refParagraph.Enclosing as Step;
                    if (step != null)
                    {
                        if (first)
                        {
                            AddTableHeader("Identification", "Step text");
                        }
                        AddRow(step.SubSequence.Name + " " + step.TestCase.Name, step.Name);
                        first = false;
                    }
                }
                CloseSubParagraph();
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates a table for comments
        /// </summary>
        /// <param name="aDictionary">The model</param>
        /// <returns></returns>
        private void GenerateComments(Dictionary aDictionary)
        {
            AddSubParagraph("Design choices");
            foreach (Paragraph paragraph in aDictionary.OnlyComments)
            {
                AddSubParagraph("Comments for " + paragraph.FullId);
                AddTable(new string[] {paragraph.FullId}, new int[] {30, 100});
                AddRow("Description", paragraph.Text);
                AddRow("Comment", paragraph.Comment);
                CloseSubParagraph();
            }
            CloseSubParagraph();
        }
    }
}