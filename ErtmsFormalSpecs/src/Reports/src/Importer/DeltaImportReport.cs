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

using Importers.RtfDeltaImporter;
using Document = MigraDoc.DocumentObjectModel.Document;
using Paragraph = Importers.RtfDeltaImporter.Paragraph;

namespace Reports.Importer
{
    /// <summary>
    ///     Generates the report based on the result of the Delta import
    /// </summary>
    public class DeltaImportReport : ReportTools
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="doc"></param>
        public DeltaImportReport(Document doc)
            : base(doc)
        {
        }

        /// <summary>
        ///     The maximum text bound
        /// </summary>
        private const int TEXT_BOUND = 2048;

        /// <summary>
        ///     Shortens the text provided if greated than a given bound
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ValidText(string text)
        {
            string retVal = text;

            if (retVal == null)
            {
                retVal = "";
            }

            if (retVal.Length > TEXT_BOUND)
            {
                retVal = retVal.Substring(0, TEXT_BOUND) + "...";
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the document whith the result of the import
        /// </summary>
        /// <param name="importResult"></param>
        public void CreateDocument(Importers.RtfDeltaImporter.Document importResult)
        {
            AddParagraph(
                "This document lists the changes that have been applied during the imporation of a new release of the specification");

            AddSubParagraph("Summary");
            AddParagraph("This section presents the summary of the document importation");
            AddTable(new string[] {"Item", "Count", "Comment"}, new int[] {40, 40, 60});
            AddRow("Paragraph count", importResult.Paragraphs.Count.ToString(),
                "This is the number of paragraphs processed during the importation of the document");
            AddRow("Modified paragraphs", importResult.ChangedParagraphs.Count.ToString(),
                "This is the number of paragraphs that have been changed between the two revisions");
            AddRow("New paragraphs", importResult.NewParagraphs.Count.ToString(),
                "The is the number of new paragraphs in this new revision");
            AddRow("Moved paragraphs", importResult.MovedParagraphs.Count.ToString(),
                "The is the number of paragraphs that have been moved around in this new revision");
            AddRow("Deleted paragraphs", importResult.RemovedParagraphs.Count.ToString(),
                "This is the number of paragraphs that have been deleted in this new revision");
            CloseSubParagraph();

            AddSubParagraph("Modified paragraphs");
            AddParagraph("This section lists the paragraphs that have been modified during the importation");
            foreach (Paragraph paragraph in importResult.ChangedParagraphs)
            {
                AddSubParagraph(paragraph.Id);
                AddSubParagraph("Original contents");
                AddParagraph(ValidText(paragraph.OriginalText));
                CloseSubParagraph();
                AddSubParagraph("New contents");
                AddParagraph(ValidText(paragraph.Text));
                CloseSubParagraph();
                CloseSubParagraph();
            }
            CloseSubParagraph();

            AddSubParagraph("New paragraphs");
            AddParagraph("This section lists the paragraphs that have been added during the importation");
            AddTable(new string[] {"Paragraph", "Contents"}, new int[] {40, 100});
            foreach (Paragraph paragraph in importResult.NewParagraphs)
            {
                AddRow(paragraph.Id, paragraph.Text);
            }
            CloseSubParagraph();
            AddSubParagraph("Removed paragraphs");
            AddParagraph("This section lists the paragraphs that have been removed during the importation");
            AddTable(new string[] {"Paragraph", "Contents"}, new int[] {40, 100});
            foreach (Paragraph paragraph in importResult.RemovedParagraphs)
            {
                AddRow(paragraph.Id, paragraph.Text);
            }
            CloseSubParagraph();

            AddSubParagraph("Moved paragraphs");
            AddParagraph(
                "This section lists the paragraphs that have been moved during the importation. No change has been performed in the model. Review should be performed manually.");
            AddTable(new string[] {"Paragraph", "Contents", "Initial position"}, new int[] {40, 100, 40});
            foreach (Paragraph paragraph in importResult.MovedParagraphs)
            {
                AddRow(paragraph.Id, paragraph.Text, paragraph.OriginalText);
            }
            CloseSubParagraph();

            AddSubParagraph("Errors during importation");
            AddParagraph(
                "This section lists the errors encountered during importation. No change has been performed in the model. Review should be performed manually.");
            AddTable(new string[] {"Paragraph", "Text", "Error"}, new int[] {30, 80, 80});
            foreach (ImportationError error in importResult.Errors)
            {
                AddRow(error.Paragraph.Id, error.Paragraph.Text, error.Message);
            }
            CloseSubParagraph();

            AddSubParagraph("List of paragraphs");
            AddParagraph("This section lists the paragraphs that have been processed during the importation");
            AddTable(new string[] {"Paragraph", "", "", "", ""}, new int[] {30, 30, 30, 30, 30});
            int i = 0;
            string[] data = new string[5] {"", "", "", "", ""};
            foreach (Paragraph paragraph in importResult.Paragraphs.Values)
            {
                data[i] = paragraph.Id;
                i += 1;

                if (i == data.Length)
                {
                    AddRow(data);
                    i = 0;
                    data = new string[5] {"", "", "", "", ""};
                }
            }

            if (i > 0)
            {
                AddRow(data);
            }
            CloseSubParagraph();
        }
    }
}