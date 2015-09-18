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
using Importers.RtfDeltaImporter;

namespace Reports.Importer
{
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class DeltaImportReportHandler : ReportHandler
    {
        /// <summary>
        ///     The document on which the report should be created
        /// </summary>
        private Document ImportResult { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="aDictionary"></param>
        /// <param name="importResult"></param>
        public DeltaImportReportHandler(Dictionary aDictionary, Document importResult, string baseFileName)
            : base(aDictionary)
        {
            createFileName(baseFileName);
            ImportResult = importResult;
        }

        /// <summary>
        ///     Creates a report on the model, according to user's choices
        /// </summary>
        /// <returns>The document created, or null</returns>
        public override MigraDoc.DocumentObjectModel.Document BuildDocument()
        {
            MigraDoc.DocumentObjectModel.Document retVal = new MigraDoc.DocumentObjectModel.Document();

            retVal.Info.Title = "EFS Model report";
            retVal.Info.Author = "ERTMS Solutions";
            retVal.Info.Subject = "Model report";

            DeltaImportReport report = new DeltaImportReport(retVal);
            report.CreateDocument(ImportResult);

            return retVal;
        }
    }
}