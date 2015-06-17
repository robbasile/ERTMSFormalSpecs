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

using DataDictionary.Generated;
using Utils;
using ReqRef = DataDictionary.ReqRef;
using ReqRelated = DataDictionary.ReqRelated;
using Specification = DataDictionary.Specification.Specification;

namespace Importers.RtfDeltaImporter
{
    /// <summary>
    ///     This class is used to import a delta in the specifications based on a delta operation
    ///     performed in Word and saved in Rtf file format.
    ///     It detects the paragraphs that have been modified and updates the specification
    ///     according to the changes, that is
    ///     - save the current paragraph text, to ease the revision
    ///     - update the paragraph text with the new paragraph version
    ///     - sets the paragraph as needing a manual review
    ///     - invalidates the models to take this change into consideration
    /// </summary>
    public class Importer : ProgressHandler
    {
        /// <summary>
        ///     The file path of the original file
        /// </summary>
        private string OriginalFilePath { get; set; }

        /// <summary>
        ///     The document which corresponds to the original file path
        /// </summary>
        public Document OriginalDocument { get; private set; }

        /// <summary>
        ///     The file path of the new file
        /// </summary>
        private string NewFilePath { get; set; }

        /// <summary>
        ///     The document which corresponds to the new file path
        /// </summary>
        public Document NewDocument { get; private set; }

        /// <summary>
        ///     The specification to be udated
        /// </summary>
        private Specification Specifications { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="originalFilePath"></param>
        /// <param name="newFilePath"></param>
        /// <param name="specifications"></param>
        public Importer(string originalFilePath, string newFilePath, Specification specifications)
        {
            OriginalFilePath = originalFilePath;
            NewFilePath = newFilePath;
            Specifications = specifications;
        }

        /// <summary>
        ///     Performs the delta on the specification provided
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="specifications"></param>
        private void PerformDelta(Specification specifications)
        {
            foreach (Paragraph p in NewDocument.ChangedParagraphs)
            {
                DataDictionary.Specification.Paragraph par = specifications.FindParagraphByNumber(p.Id);

                if (par != null)
                {
                    par.Text = p.Text;
                    par.AddInfo("Paragraph has been changed");
                    NewParagraphRevision(par);
                }
                else
                {
                    AddError(specifications, p, "Cannot find paragraph " + p.Id + " for modification");
                }
            }

            foreach (Paragraph p in NewDocument.NewParagraphs)
            {
                DataDictionary.Specification.Paragraph par = specifications.FindParagraphByNumber(p.Id);

                if (par != null)
                {
                    AddError(specifications, p,
                        "Paragraph " + p.Id +
                        " already exists, whereas it has been detected as a new paragraph in the release");
                }
                else
                {
                    par = specifications.FindParagraphByNumber(p.Id, true);
                    if (par != null)
                    {
                        par.setText(p.Text);
                        par.AddInfo("New paragraph");
                        NewParagraphRevision(par);
                    }
                    else
                    {
                        AddError(specifications, p, "Paragraph " + p.Id + " cannot be found in the specification");
                    }
                }
            }

            foreach (Paragraph p in NewDocument.RemovedParagraphs)
            {
                DataDictionary.Specification.Paragraph par = specifications.FindParagraphByNumber(p.Id);

                if (par != null)
                {
                    par.Text = "<Removed in current release>";
                    NewParagraphRevision(par);
                    par.AddInfo("Paragraph has been removed");
                }
                else
                {
                    AddError(specifications, p, "Cannot find paragraph " + p.Id + " for removal");
                }
            }
        }

        /// <summary>
        ///     Indicates that this paragraph is a new revision
        /// </summary>
        /// <param name="par"></param>
        private void NewParagraphRevision(DataDictionary.Specification.Paragraph par)
        {
            if (par.Comment == null)
            {
                par.Comment = "";
            }
            par.Comment = par.Comment + "\nPrevious revision status was " + par.getImplementationStatus_AsString();
            par.setImplementationStatus(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NewRevisionAvailable);
            par.setReviewed(false);
            par.setBl("3.2.0");
            par.setVersion("3.2.0");
            foreach (ReqRef reqRef in par.Implementations)
            {
                ReqRelated reqRelated = reqRef.Model as ReqRelated;
                if (reqRelated != null)
                {
                    reqRelated.setImplemented(false);
                    reqRelated.setVerified(false);
                }
            }
        }

        /// <summary>
        ///     Adds an importation error
        /// </summary>
        /// <param name="specifications"></param>
        /// <param name="p"></param>
        /// <param name="error"></param>
        private void AddError(Specification specifications, Paragraph p, string error)
        {
            specifications.AddError(error);
            NewDocument.Errors.Add(new ImportationError(error, p));
        }

        /// <summary>
        ///     Executes the work in the background task
        /// </summary>
        public override void ExecuteWork()
        {
            OriginalDocument = new Document(OriginalFilePath);
            NewDocument = new Document(NewFilePath);

            NewDocument.UpdateState(OriginalDocument);

            PerformDelta(Specifications);
        }
    }
}