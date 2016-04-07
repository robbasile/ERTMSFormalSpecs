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
using DataDictionary.Types;
using MigraDoc.DocumentObjectModel;

namespace Reports.Model
{
    public class ModelReportHandler : ReportHandler
    {
        public ModelReportHandler(Dictionary aDictionary)
            : base(aDictionary)
        {
            CreateFileName("ModelReport");
            AddRanges = false;
            AddRangesDetails = false;
            AddEnumerations = false;
            AddEnumerationsDetails = false;
            AddStructures = false;
            AddStructuresDetails = false;
            AddCollections = false;
            AddCollectionsDetails = false;
            AddFunctions = false;
            AddFunctionsDetails = false;
            AddProcedures = false;
            AddProceduresDetails = false;
            AddVariables = false;
            AddVariablesDetails = false;
            AddRules = false;
            AddRulesDetails = false;
            ImplementedOnly = true;
        }

        /// <summary>
        ///     Creates a report on the model, according to user's choices
        /// </summary>
        /// <returns>The document created, or null</returns>
        public override Document BuildDocument()
        {
            Document retVal = new Document();

            retVal.Info.Title = "EFS Model report";
            retVal.Info.Author = "ERTMS Solutions";
            retVal.Info.Subject = "Model report";

            ModelReport report = new ModelReport(retVal, ImplementedOnly);
            foreach (NameSpace nameSpace in Dictionary.NameSpaces)
            {
                CreateNamespaceSection(report, nameSpace);
            }

            return retVal;
        }

        public void CreateNamespaceSection(ModelReport report, NameSpace aNameSpace)
        {
            bool informationAdded = false;

            if (!aNameSpace.FullName.StartsWith("Messages"))
            {
                if (AddRanges)
                {
                    if (report.CountDisplayedReqRelated(aNameSpace.Ranges) > 0)
                    {
                        report.AddSubParagraph("Namespace " + aNameSpace.FullName);
                        report.CreateRangesSection(aNameSpace.Ranges, AddRangesDetails);
                        informationAdded = true;
                    }
                }
                if (AddEnumerations)
                {
                    if (report.CountDisplayedReqRelated(aNameSpace.Enumerations) > 0)
                    {
                        if (!informationAdded)
                        {
                            report.AddSubParagraph("Namespace " + aNameSpace.FullName);
                            informationAdded = true;
                        }
                        report.CreateEnumerationsSection(aNameSpace.Enumerations, AddEnumerationsDetails);
                    }
                }
                if (AddStructures)
                {
                    if (report.CountDisplayedReqRelated(aNameSpace.Structures) > 0)
                    {
                        if (!informationAdded)
                        {
                            report.AddSubParagraph("Namespace " + aNameSpace.FullName);
                            informationAdded = true;
                        }
                        report.CreateStructuresSection(aNameSpace.Structures, AddStructuresDetails);
                    }
                }
                if (AddCollections)
                {
                    if (report.CountDisplayedReqRelated(aNameSpace.Collections) > 0)
                    {
                        if (!informationAdded)
                        {
                            report.AddSubParagraph("Namespace " + aNameSpace.FullName);
                            informationAdded = true;
                        }
                        report.CreateCollectionsSection(aNameSpace.Collections, AddCollectionsDetails);
                    }
                }
                if (AddStateMachines)
                {
                    if (report.CountDisplayedReqRelated(aNameSpace.StateMachines) > 0)
                    {
                        if (!informationAdded)
                        {
                            report.AddSubParagraph("Namespace " + aNameSpace.FullName);
                            informationAdded = true;
                        }
                        report.CreateStateMachinesSection(aNameSpace.StateMachines, AddStateMachinesDetails);
                    }
                }
                if (AddFunctions)
                {
                    if (report.CountDisplayedReqRelated(aNameSpace.Functions) > 0)
                    {
                        if (!informationAdded)
                        {
                            report.AddSubParagraph("Namespace " + aNameSpace.FullName);
                            informationAdded = true;
                        }
                        report.CreateFunctionsSection(aNameSpace.Functions, AddFunctionsDetails);
                    }
                }
                if (AddProcedures)
                {
                    if (report.CountDisplayedReqRelated(aNameSpace.Procedures) > 0)
                    {
                        if (!informationAdded)
                        {
                            report.AddSubParagraph("Namespace " + aNameSpace.FullName);
                            informationAdded = true;
                        }
                        report.CreateProceduresSection(aNameSpace.Procedures, AddProceduresDetails, false);
                    }
                }
                if (AddVariables)
                {
                    if (report.CountDisplayedVariables(aNameSpace.Variables, InOutOnly) > 0)
                    {
                        if (!informationAdded)
                        {
                            report.AddSubParagraph("Namespace " + aNameSpace.FullName);
                            informationAdded = true;
                        }
                        report.CreateVariablesSection(aNameSpace.Variables, AddVariablesDetails, InOutOnly);
                    }
                }
                if (AddRules)
                {
                    if (report.CountDisplayedReqRelated(aNameSpace.Rules) > 0)
                    {
                        if (!informationAdded)
                        {
                            report.AddSubParagraph("Namespace " + aNameSpace.FullName);
                            informationAdded = true;
                        }
                        report.CreateRulesSection(aNameSpace.Rules, AddRulesDetails, false);
                    }
                }

                if (informationAdded)
                {
                    report.CloseSubParagraph();
                }

                foreach (NameSpace nameSpace in aNameSpace.NameSpaces)
                {
                    CreateNamespaceSection(report, nameSpace);
                }
            }
        }

        public bool AddRanges { set; get; }
        public bool AddRangesDetails { set; get; }

        public bool AddEnumerations { set; get; }
        public bool AddEnumerationsDetails { set; get; }

        public bool AddStructures { set; get; }
        public bool AddStructuresDetails { set; get; }

        public bool AddCollections { set; get; }
        public bool AddCollectionsDetails { set; get; }

        public bool AddStateMachines { set; get; }
        public bool AddStateMachinesDetails { set; get; }

        public bool AddFunctions { set; get; }
        public bool AddFunctionsDetails { set; get; }

        public bool AddProcedures { set; get; }
        public bool AddProceduresDetails { set; get; }

        public bool AddVariables { set; get; }
        public bool AddVariablesDetails { set; get; }
        public bool InOutOnly { set; get; }

        public bool AddRules { set; get; }
        public bool AddRulesDetails { set; get; }

        public bool ImplementedOnly { set; get; }
    }
}