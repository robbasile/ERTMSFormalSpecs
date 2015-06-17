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

using DataDictionary;
using MigraDoc.DocumentObjectModel;

namespace Reports.Specs
{
    public class FindingsReportHandler : ReportHandler
    {
        public bool addQuestions { set; get; }
        public bool addComments { set; get; }
        public bool addBugs { set; get; }

        public bool addReviewed { set; get; }
        public bool addNotReviewed { set; get; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public FindingsReportHandler(Dictionary dictionary)
            : base(dictionary)
        {
            createFileName("FindingsReport");
            addQuestions = false;
            addComments = false;
            addBugs = false;

            addReviewed = true;
            addNotReviewed = true;
        }


        public override Document BuildDocument()
        {
            Document retVal = new Document();

            Log.Info("Creating findings report");
            retVal.Info.Title = "EFS Subset-076 Findings report";
            retVal.Info.Author = "ERTMS Solutions";
            retVal.Info.Subject = "Subset-076 findings report";

            FindingsReport report = new FindingsReport(retVal);

            if (addReviewed)
            {
                report.ReviewedParagraphs = false;
                BuildSections(report);
            }

            if (addNotReviewed)
            {
                report.ReviewedParagraphs = true;
                BuildSections(report);
            }

            return retVal;
        }

        /// <summary>
        ///     Add an Issues, Comments and Questions section to the report, as requested
        /// </summary>
        /// <param name="report"></param>
        private void BuildSections(FindingsReport report)
        {
            if (report.ReviewedParagraphs)
            {
                report.AddSubParagraph("Addressed findings");
                report.AddParagraph("Findings that have been reviewed by ERA, included for informational purposes only.");
            }
            else
            {
                report.AddSubParagraph("Findings");
            }


            if (addBugs)
            {
                Log.Info("..generating issues");
                report.CreateIssuesArticle(this);
            }

            if (addComments)
            {
                Log.Info("..generating remarks");
                report.CreateCommentsArticle(this);
            }

            if (addQuestions)
            {
                Log.Info("..generating questions");
                report.CreateQuestionsArticle(this);
            }

            report.CloseSubParagraph();
        }
    }
}