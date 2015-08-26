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

using System.Collections;
using DataDictionary.Specification;
using Utils;

namespace DataDictionary
{
    public class ReqRef : Generated.ReqRef, ICommentable
    {
        /// <summary>
        ///     The paragraph name of this trace
        /// </summary>
        public override string Name
        {
            get
            {
                string retVal = "<Cannot find paragraph>";

                if (Paragraph != null)
                {
                    Specification.Specification specification =
                        EnclosingFinder<Specification.Specification>.find(Paragraph);
                    if (specification != null)
                    {
                        retVal = specification.Name + " § " + Paragraph.getId();
                    }
                }

                return retVal;
            }
            set { }
        }

        /// <summary>
        ///     Provides the comment related to this requirement reference
        /// </summary>
        public string Comment
        {
            get
            {
                string retVal = getComment();
                if (retVal == null)
                {
                    retVal = "";
                }
                return retVal;
            }
            set { setComment(value); }
        }

        /// <summary>
        ///     The implementation of this trace
        /// </summary>
        public IModelElement Model
        {
            get { return Enclosing as IModelElement; }
        }

        public override ArrayList EnclosingCollection
        {
            get
            {
                if (Model is ReqRelated)
                {
                    ReqRelated reqRelated = Model as ReqRelated;
                    if (reqRelated != null)
                    {
                        return reqRelated.Requirements;
                    }
                }
                else if (Model is Paragraph)
                {
                    Paragraph paragraph = Model as Paragraph;
                    if (paragraph != null)
                    {
                        return paragraph.Requirements;
                    }
                }
                else if (Model is ReferencesParagraph)
                {
                    ReferencesParagraph referencesParagraph = Model as ReferencesParagraph;
                    if (referencesParagraph != null)
                    {
                        return referencesParagraph.Requirements;
                    }
                }
                return null;
            }
        }

        /// <summary>
        ///     The corresponding specification paragraph
        /// </summary>
        public Paragraph Paragraph
        {
            get
            {
                Paragraph retVal = GuidCache.Instance.GetModel(getId()) as Paragraph;

                if (retVal == null)
                {
                    foreach (Dictionary dictionary in EFSSystem.Dictionaries)
                    {
                        foreach (Specification.Specification specification in dictionary.Specifications)
                        {
                            if (string.IsNullOrEmpty(getSpecId()) || getSpecId() == specification.Guid)
                            {
                                retVal = specification.FindParagraphByNumber(getId(), false);

                                if (retVal != null)
                                {
                                    break;
                                }
                            }
                        }

                        if (retVal != null)
                        {
                            break;
                        }
                    }
                }

                return retVal;
            }
            set
            {
                // Sets the paragraph and specification guid
                Specification.Specification specification = EnclosingFinder<Specification.Specification>.find(value);
                setId(value.Guid);
                setSpecId(specification.Guid);
            }
        }

        /// <summary>
        ///     HaCK : this performes the same thing as Paragraph.ExpressionText
        ///     because when selecting a ReqRef in the req references of a paragraph,
        ///     this updates the paragraph text box => updates the paragraph text...
        /// </summary>
        public override string ExpressionText
        {
            get
            {
                if (Paragraph != null)
                {
                    return Paragraph.ExpressionText;
                }
                return "";
            }
        }


        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
        }


        /// <summary>
        ///     Provides the requirement description for this req ref
        /// </summary>
        /// <returns></returns>
        public override string RequirementDescription()
        {
            string retVal = "";

            if (Paragraph != null)
            {
                if (EfsSystem.Instance.DisplayRequirementsAsList)
                {
                    retVal = Paragraph.FullId + ", ";
                }
                else
                {
                    if (Paragraph != null)
                    {
                        retVal = Paragraph.FullId + ":" + Paragraph.getText();
                    }
                }
            }
            else
            {
                retVal = base.RequirementDescription();
            }

            return retVal;
        }

        /// <summary>
        ///     Merge deleted reqrefs into the base, removing both elements.
        /// </summary>
        public override void Merge()
        {
            if (IsRemoved)
            {
                Updates.Delete();
                Delete();
            }
        }

        /// <summary>
        ///     Creates an update for the reqref. Used to delete references in an update.
        /// </summary>
        /// <param name="modelUpdate"></param>
        /// <returns></returns>
        public ReqRef CreateReqRefUpdate(ModelElement modelUpdate)
        {
            ReqRef retVal = (ReqRef) Duplicate();
            retVal.setUpdates(Guid);
            ReqRelated reqRelatedUpdate = modelUpdate as ReqRelated;
            if (reqRelatedUpdate != null)
            {
                reqRelatedUpdate.appendRequirements(retVal);
            }


            return retVal;
        }
    }
}