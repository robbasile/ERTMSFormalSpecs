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
    public class SpecIssuesReportHandler : ReportHandler
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public SpecIssuesReportHandler(Dictionary dictionary)
            : base(dictionary)
        {
            CreateFileName("SpecificationIssuesReport");
            AddSpecIssues = false;
            AddDesignChoices = false;
            AddInformationNeeded = false;
            AddComments = false;
        }

        /// <summary>
        ///     Creates a report on specs issues, according to user's choices
        /// </summary>
        /// <returns>The document created, or null</returns>
        public override Document BuildDocument()
        {
            Document retVal = new Document();

            retVal.Info.Title = "EFS Specification issues report";
            retVal.Info.Author = "ERTMS Solutions";
            retVal.Info.Subject = "Specification issues report";

            SpecIssuesReport report = new SpecIssuesReport(retVal);
            if (AddInformationNeeded)
            {
                report.CreateMoreInformationArticle(this);
            }
            if (AddSpecIssues)
            {
                report.CreateSpecIssuesArticle(this);
            }
            if (AddDesignChoices)
            {
                report.CreateDesignChoicesArticle(this);
            }

            return retVal;
        }

        public bool AddSpecIssues { set; get; }
        public bool AddDesignChoices { set; get; }
        public bool AddInformationNeeded { set; get; }
        public bool AddComments { set; get; }
    }
}