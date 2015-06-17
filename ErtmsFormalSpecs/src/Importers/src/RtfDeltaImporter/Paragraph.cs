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

namespace Importers.RtfDeltaImporter
{
    /// <summary>
    ///     Stores data about the paragraphs found in the RTF document
    ///     - the paragraph ID
    ///     - the paragraph Text
    ///     - if there are changes in that paragraph
    /// </summary>
    public class Paragraph
    {
        /// <summary>
        ///     The paragraph Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The paragraph text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     The text before the change
        /// </summary>
        public string OriginalText { get; set; }

        /// <summary>
        ///     The state of the paragraph
        ///     - NoChange : there are no changes between this paragraph and the original one
        ///     - Changed : the paragraph text has changed
        ///     - Inserted : the paragraph was not present in the original
        ///     - Deleted : the paragraph is no more present in the document
        ///     - Moved : only for table row, the row has been moved in the table
        /// </summary>
        public enum ParagraphState
        {
            NoChange,
            Changed,
            Inserted,
            Deleted,
            Moved
        };

        public ParagraphState State { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        public Paragraph(string id)
        {
            Id = id.Trim();
            Text = "";
            State = ParagraphState.NoChange;
        }
    }

    public class TableRow : Paragraph
    {
        /// <summary>
        ///     The paragraph which encloses the current table row
        /// </summary>
        public Paragraph EnclosingParagraph { get; private set; }

        /// <summary>
        ///     The base row name
        /// </summary>
        private string BaseName { get; set; }

        /// <summary>
        ///     The row number of the table row
        /// </summary>
        public int RowNumber { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="previous">The paragraph before this one in the document</param>
        public TableRow(Paragraph previous)
            : base("")
        {
            TableRow previousRow = previous as TableRow;
            if (previousRow != null)
            {
                EnclosingParagraph = previousRow.EnclosingParagraph;
                BaseName = previousRow.BaseName;
                RowNumber = previousRow.RowNumber + 1;
            }
            else
            {
                EnclosingParagraph = previous;
                BaseName = previous.Id;
                RowNumber = 0;
            }

            Id = BaseName + ".Entry " + RowNumber;
        }
    }
}