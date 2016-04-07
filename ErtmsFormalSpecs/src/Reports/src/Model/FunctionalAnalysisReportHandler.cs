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
using DataDictionary.Types;
using DataDictionary.Types.AccessMode;
using MigraDoc.DocumentObjectModel;

namespace Reports.Model
{
    public class FunctionalAnalysisReportHandler : ReportHandler
    {
        public FunctionalAnalysisReportHandler(Dictionary aDictionary)
            : base(aDictionary)
        {
            CreateFileName("FunctionalAnalysisReport");
        }

        /// <summary>
        ///     Creates a report on the model, according to user's choices
        /// </summary>
        /// <returns>The document created, or null</returns>
        public override Document BuildDocument()
        {
            Document retVal = new Document();

            retVal.Info.Title = "EFS Functional Analysis report";
            retVal.Info.Author = "ERTMS Solutions";
            retVal.Info.Subject = "Functional Analysis report";

            FunctionalAnalysisReport report = new FunctionalAnalysisReport(retVal);
            List<AccessMode> accesses = IEnclosesNameSpacesUtils.getAccesses(EFSSystem, null);
            foreach (NameSpace nameSpace in Dictionary.NameSpaces)
            {
                CreateNamespaceSection(report, nameSpace, accesses);
            }

            return retVal;
        }

        public void CreateNamespaceSection(FunctionalAnalysisReport report, NameSpace nameSpace,
            List<AccessMode> accesses)
        {
            report.CreateNameSpaceSection(nameSpace, accesses);
            foreach (NameSpace subNameSpace in nameSpace.NameSpaces)
            {
                CreateNamespaceSection(report, subNameSpace, accesses);
            }
        }
    }
}