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
    public class SpecCoverageReportHandler : ReportHandler
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public SpecCoverageReportHandler(Dictionary dictionary)
            : base(dictionary)
        {
            createFileName("SpecificationCoverageReport");
            AddSpecification = false;
            ShowFullSpecification = false;
            AddCoveredParagraphs = false;
            ShowAssociatedReqRelated = false;
            AddNonCoveredParagraphs = false;
            AddReqRelated = false;
            ShowAssociatedParagraphs = false;
        }

        /// <summary>
        ///     Creates a report on specs coverage, according to user's choices
        /// </summary>
        /// <returns>The document created, or null</returns>
        public override Document BuildDocument()
        {
            Document retVal = new Document();

            Log.Info("Creating spec report");
            retVal.Info.Title = "EFS Specification report";
            retVal.Info.Author = "ERTMS Solutions";
            retVal.Info.Subject = "Specification report";

            SpecCoverageReport report = new SpecCoverageReport(retVal);
            if (AddSpecification)
            {
                Log.Info("..generating specifications");
                report.CreateSpecificationArticle(this);
            }
            if (AddCoveredParagraphs)
            {
                Log.Info("..generating covered paragraphs");
                report.CreateCoveredRequirementsArticle(this);
            }
            if (AddNonCoveredParagraphs)
            {
                Log.Info("..generating non covered paragraphs");
                report.CreateNonCoveredRequirementsArticle(this);
            }
            if (AddReqRelated)
            {
                Log.Info("..generating req related");
                report.CreateReqRelatedArticle(this);
            }

            return retVal;
        }


        public bool AddSpecification { set; get; }
        public bool ShowFullSpecification { set; get; }

        public bool AddCoveredParagraphs { set; get; }
        public bool ShowAssociatedReqRelated { set; get; }

        public bool AddNonCoveredParagraphs { set; get; }


        public bool AddReqRelated { set; get; }
        public bool ShowAssociatedParagraphs { set; get; }
    }
}