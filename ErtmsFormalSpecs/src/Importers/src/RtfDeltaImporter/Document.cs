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

namespace Importers.RtfDeltaImporter
{
    /// <summary>
    ///     A document, consisting of a set of paragraphs
    /// </summary>
    public class Document
    {
        /// <summary>
        ///     Stores the paragraphs found in the document
        /// </summary>
        public Dictionary<string, Paragraph> Paragraphs = new Dictionary<string, Paragraph>();

        /// <summary>
        ///     The list of errors encountered during importation
        /// </summary>
        public List<ImportationError> Errors { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="filePath"></param>
        public Document(string filePath)
        {
            Parser parser = new Parser(filePath, this);
            Errors = new List<ImportationError>();
            foreach (Paragraph p in Paragraphs.Values)
            {
                p.Text = p.Text.Trim();
            }
        }

        /// <summary>
        ///     Adds a paragraph in the document
        /// </summary>
        /// <param name="paragraph"></param>
        public void AddParagraph(Paragraph paragraph)
        {
            Paragraphs.Add(paragraph.Id, paragraph);
        }

        /// <summary>
        ///     Updates the state of the paragraphs located in source according to the original content, located in original
        /// </summary>
        /// <param name="source">The source paragraphs, to be updated</param>
        /// <param name="original">The original file content</param>
        public void UpdateState(Document original)
        {
            // Find paragraphs that have been inserted and those that have been modified
            foreach (Paragraph p in Paragraphs.Values)
            {
                Paragraph originalParagraph = original.FindParagraph(p.Id);
                if (originalParagraph != null)
                {
                    if (originalParagraph.Text.Equals(p.Text))
                    {
                        p.State = Paragraph.ParagraphState.NoChange;
                    }
                    else
                    {
                        p.OriginalText = originalParagraph.Text;
                        p.State = Paragraph.ParagraphState.Changed;

                        // Maybe the row has been moved (new row inserted before), or this is a new row
                        TableRow row = p as TableRow;
                        TableRow originalRow = originalParagraph as TableRow;
                        if (originalRow != null)
                        {
                            // Try to determine if the paragraph as been moved (instead of changed)
                            // The paragraph has been moved when we can find it, at another location in the original table
                            foreach (Paragraph otherParagraph in original.Paragraphs.Values)
                            {
                                TableRow otherRow = otherParagraph as TableRow;
                                if (otherRow != null && otherRow.EnclosingParagraph == originalRow.EnclosingParagraph)
                                {
                                    if (otherRow.Text.Equals(p.Text))
                                    {
                                        p.State = Paragraph.ParagraphState.Moved;
                                        p.OriginalText = otherRow.Id;
                                        break;
                                    }
                                }
                            }
                            if (p.State == Paragraph.ParagraphState.Changed)
                            {
                                // If the paragraph has not been moved, try to determine if the paragraph has been inserted
                                // The paragraph has been inserted if the corresponding paragraph in the original table 
                                // can be found at another location of this table
                                foreach (Paragraph otherParagraph in Paragraphs.Values)
                                {
                                    TableRow otherRow = otherParagraph as TableRow;
                                    if (otherRow != null && otherRow.EnclosingParagraph == row.EnclosingParagraph)
                                    {
                                        if (otherRow.Text.Equals(originalRow.Text))
                                        {
                                            p.State = Paragraph.ParagraphState.Inserted;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Original paragraph could not be found => This is a new paragraph
                    p.State = Paragraph.ParagraphState.Inserted;

                    TableRow originalRow = originalParagraph as TableRow;
                    if (originalRow != null)
                    {
                        // Try to determine if the paragraph as been moved (instead of changed)
                        // The paragraph has been moved when we can find it, at another location in the original table
                        foreach (Paragraph otherParagraph in original.Paragraphs.Values)
                        {
                            TableRow otherRow = otherParagraph as TableRow;
                            if (otherRow != null && otherRow.EnclosingParagraph == originalRow.EnclosingParagraph)
                            {
                                if (otherRow.Text.Equals(p.Text))
                                {
                                    p.State = Paragraph.ParagraphState.Moved;
                                    p.OriginalText = otherRow.Id;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Find paragraphs that have been deleted
            foreach (Paragraph p in original.Paragraphs.Values)
            {
                Paragraph newParagraph = FindParagraph(p.Id);
                if (newParagraph == null)
                {
                    p.State = Paragraph.ParagraphState.Deleted;
                    Paragraphs.Add(p.Id, p);
                }
            }
        }

        /// <summary>
        ///     Finds a paragraph whose Id corresponds to the Id provided
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Paragraph FindParagraph(string id)
        {
            Paragraph retVal = null;

            Paragraphs.TryGetValue(id, out retVal);

            return retVal;
        }

        /// <summary>
        ///     Provides the paragraph that have been added in this release
        /// </summary>
        public List<Paragraph> NewParagraphs
        {
            get { return findMatching(IsInserted); }
        }

        /// <summary>
        ///     Provides the paragraph that have been removed in this release
        /// </summary>
        public List<Paragraph> RemovedParagraphs
        {
            get { return findMatching(IsDeleted); }
        }

        /// <summary>
        ///     Provides the paragraph that have been changed in this release
        /// </summary>
        public List<Paragraph> ChangedParagraphs
        {
            get { return findMatching(IsChanged); }
        }

        /// <summary>
        ///     Provides the paragraph that have been moved in this release
        /// </summary>
        public List<Paragraph> MovedParagraphs
        {
            get { return findMatching(IsMoved); }
        }

        /// <summary>
        ///     Predicates on paragraphs
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private delegate bool ParagraphCondition(Paragraph p);

        /// <summary>
        ///     Paragraph has been inserted
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsInserted(Paragraph p)
        {
            return p.State == Paragraph.ParagraphState.Inserted && p.Text.Length > 0;
        }

        /// <summary>
        ///     Paragraph has been deleted
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsDeleted(Paragraph p)
        {
            return p.State == Paragraph.ParagraphState.Deleted;
        }

        /// <summary>
        ///     Paragraph contents has changed
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsChanged(Paragraph p)
        {
            return p.State == Paragraph.ParagraphState.Changed;
        }

        /// <summary>
        ///     Paragraph has been moved
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsMoved(Paragraph p)
        {
            return p.State == Paragraph.ParagraphState.Moved && p.Text.Length > 0;
        }

        /// <summary>
        ///     Find all paragraphs which match the predicate condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private List<Paragraph> findMatching(ParagraphCondition condition)
        {
            List<Paragraph> retVal = new List<Paragraph>();

            foreach (Paragraph p in Paragraphs.Values)
            {
                if (condition(p))
                {
                    retVal.Add(p);
                }
            }

            return retVal;
        }
    }
}