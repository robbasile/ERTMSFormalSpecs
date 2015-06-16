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
using System.Reflection;
using DataDictionary.Generated;
using log4net;
using Microsoft.Office.Interop.Excel;
using Utils;
using Chapter = DataDictionary.Specification.Chapter;
using Dictionary = DataDictionary.Dictionary;
using Paragraph = DataDictionary.Specification.Paragraph;
using Range = Microsoft.Office.Interop.Excel.Range;
using ReqRef = DataDictionary.ReqRef;
using RequirementSet = DataDictionary.Specification.RequirementSet;
using RequirementSetReference = DataDictionary.Specification.RequirementSetReference;
using Specification = DataDictionary.Specification.Specification;

namespace Importers.ExcelImporter
{
    public class SpecsImporter : ProgressHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public Dictionary TheDictionary;
        public string FileName;


        // The sheets of the workbook are:
        // Sheet number 1: INPUTS
        // Sheet number 2: OUTPUTS
        // Sheet number 3: Cover
        // Sheet number 4: Amendment Record
        // Sheet number 5: Reference documents
        // Sheet number 6: Introduction
        // Sheet number 7: ETCS DMI inputs => to import
        // Sheet number 8: ETCS DMI outputs => to import
        // Sheet number 9: INPUT ITEMS
        // Sheet number 10: OUTPUT ITEMS
        // Sheet number 11: ANALYSIS


        /// <summary>
        ///     Launches import of the excel file in the background task
        /// </summary>
        /// <param name="arg"></param>
        public override void ExecuteWork()
        {
            if (TheDictionary != null)
            {
                Application application = new Application();
                if (application != null)
                {
                    Workbook workbook = application.Workbooks.Open(FileName);
                    if (workbook.Sheets.Count == 11)
                    {
                        Specification newSpecification = (Specification) acceptor.getFactory().createSpecification();
                        newSpecification.Name = "Start Stop Conditions";
                        TheDictionary.appendSpecifications(newSpecification);

                        Chapter newChapter = (Chapter) acceptor.getFactory().createChapter();
                        newChapter.setId("1 - DMI inputs");
                        newSpecification.appendChapters(newChapter);
                        Worksheet aWorksheet = workbook.Sheets[7] as Worksheet;
                        importParagraphs(newChapter, "1", aWorksheet);

                        newChapter = (Chapter) acceptor.getFactory().createChapter();
                        newChapter.setId("2 - DMI outputs");
                        newSpecification.appendChapters(newChapter);
                        aWorksheet = workbook.Sheets[8] as Worksheet;
                        importParagraphs(newChapter, "2", aWorksheet);
                    }
                    workbook.Close(false);
                }
                else
                {
                    Log.ErrorFormat("Error while opening the excel file");
                }
                application.Quit();
            }
        }


        /// <summary>
        ///     Imports the paragraphs from the provided worksheet to the provided chapter
        /// </summary>
        /// <param name="aChapter"></param>
        /// <param name="aWorksheet"></param>
        private void importParagraphs(Chapter aChapter, string chapterId, Worksheet aWorksheet)
        {
            Range aRange = aWorksheet.UsedRange;
            int paragraphId = 1;
            string text = "";
            bool skipRow = false;

            for (int i = 2; i <= aRange.Rows.Count; i++)
            {
                string specId = (string) (aRange.Cells[i, 1] as Range).Value2;
                if (specId != null)
                {
                    // Create the new paragraph
                    Paragraph aParagraph = (Paragraph) acceptor.getFactory().createParagraph();
                    aParagraph.setId(chapterId + "." + paragraphId.ToString());
                    paragraphId++;
                    aParagraph.setType(acceptor.Paragraph_type.aNOTE);
                    aParagraph.setImplementationStatus(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable);


                    // Add the requirement set "Onboard"
                    aParagraph.setObsoleteScopeOnBoard(false);
                    aParagraph.setObsoleteScopeTrackside(false);
                    RequirementSetReference requirementSetReference =
                        (RequirementSetReference) acceptor.getFactory().createRequirementSetReference();
                    RequirementSet requirementSet = TheDictionary.findRequirementSet("Scope", false);
                    if (requirementSet != null)
                    {
                        requirementSet = requirementSet.findRequirementSet("Onboard", false);
                        if (requirementSet != null)
                        {
                            requirementSetReference.setTarget(requirementSet.Guid);
                            aParagraph.appendRequirementSets(requirementSetReference);
                        }
                        else
                        {
                            throw new Exception("Requirement set Onboard not found");
                        }
                    }
                    else
                    {
                        throw new Exception("Requirement set Scope not found");
                    }

                    // Add the paragraph to the chapter
                    aChapter.appendParagraphs(aParagraph);


                    // Create of the text of paragraph
                    aParagraph.Text = (string) (aRange.Cells[i, 2] as Range).Value2 + "\n"; // description
                    text = (string) (aRange.Cells[i, 6] as Range).Value2; // start condition
                    if (text != null)
                    {
                        aParagraph.Text += "START: " + text + "\n";
                        if (specId.Equals((string) (aRange.Cells[i + 1, 1] as Range).Value2))
                            // the following element can give the stop condition for the current element
                        {
                            text = (string) (aRange.Cells[i + 1, 7] as Range).Value2; // stop condition
                            if (text != null)
                            {
                                aParagraph.Text += "STOP: " + text + "\n";
                                skipRow = true;
                                // the remaining information of the following document is identical => let's skip it
                            }
                        }
                    }
                    text = (string) (aRange.Cells[i, 7] as Range).Value2; // stop condition
                    if (text != null)
                    {
                        aParagraph.Text += "STOP: " + text + "\n";
                    }
                    text = (string) (aRange.Cells[i, 8] as Range).Value2; // comment
                    if (text != null)
                    {
                        aParagraph.Text += "Comment: " + text + "\n";
                    }


                    // Create the reference to a paragraph from Subset-026
                    Specification subset026 = findSubset026Specification();

                    specId = specId.Replace(" ", ".");
                    Paragraph refParagraph = subset026.FindParagraphByNumber(specId);

                    if (refParagraph != null)
                    {
                        ReqRef aReqRef = (ReqRef) acceptor.getFactory().createReqRef();
                        aReqRef.Paragraph = aParagraph;
                        refParagraph.appendRequirements(aReqRef);
                    }
                    else
                    {
                        aParagraph.Text += "SUBSET-026 REFERENCE: " + specId + "\n";
                        Log.ErrorFormat("Paragraph id " + specId + " could not be found.");
                    }


                    // DMI references
                    text = (string) (aRange.Cells[i, 3] as Range).Value2; // DMI object
                    if (text != null)
                    {
                        aParagraph.Text += "DMI OBJECT: " + text + "\n";
                        text = (string) (aRange.Cells[i, 4] as Range).Value2; // DMI area
                        if (text != null)
                        {
                            aParagraph.Text += "DMI AREA: " + text + "\n";
                            object reference = (aRange.Cells[i, 5] as Range).Value2; // DMI reference
                            if (reference != null)
                            {
                                aParagraph.Text += "DMI REFERENCE: " + reference.ToString();
                            }
                        }
                    }

                    if (skipRow)
                    {
                        i++;
                        skipRow = false;
                    }
                }
            }
        }


        /// <summary>
        ///     Finds the specification corresponding to Subset-026
        /// </summary>
        /// <returns></returns>
        private Specification findSubset026Specification()
        {
            Specification retVal = null;

            for (int i = 0; i < TheDictionary.Specifications.Count; i++)
            {
                Specification aSpeficiation = TheDictionary.Specifications[i] as Specification;
                if (aSpeficiation.Name.Equals("Subset 26"))
                {
                    retVal = aSpeficiation;
                    break;
                }
            }

            return retVal;
        }
    }
}