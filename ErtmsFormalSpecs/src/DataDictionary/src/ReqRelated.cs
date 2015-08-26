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

using DataDictionary.Specification;

namespace DataDictionary
{
    public class ReqRelated : Generated.ReqRelated, ICommentable
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public ReqRelated()
            : base()
        {
            // By default, a requirement is required
            NeedsRequirement = true;
        }

        /// <summary>
        ///     Indicates if this ReqRelated has to be associated with a requirement
        /// </summary>
        public virtual bool NeedsRequirement
        {
            get { return getNeedsRequirement(); }
            set { setNeedsRequirement(value); }
        }

        /// <summary>
        ///     Indicates if the implementation of this ReqRelated is completed
        /// </summary>
        public virtual bool ImplementationCompleted
        {
            get { return getImplemented(); }
        }

        /// <summary>
        ///     Indicates if this ReqRelated contains implemented sub-elements
        /// </summary>
        public virtual bool ImplementationPartiallyCompleted
        {
            get { return getImplemented(); }
        }

        /// <summary>
        ///     Provides the description of all requirements
        /// </summary>
        /// <returns></returns>
        public override string RequirementDescription()
        {
            string retVal = "";

            foreach (Paragraph paragraph in ModeledParagraphs)
            {
                if (EfsSystem.Instance.DisplayRequirementsAsList)
                {
                    retVal += paragraph.FullId + ", ";
                }
                else
                {
                    retVal += paragraph.FullId + ":" + paragraph.getText() + "\n\n";
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Ensures that the traceability of merged elements is updated correctly
        /// </summary>
        /// <param name="source"></param>
        protected override void UpdateModelElementAccordingToSource(ModelElement source)
        {
            base.UpdateModelElementAccordingToSource(source);

            KeepTraceability(source);
        }

        /// <summary>
        ///     Removes al requirements from this element
        /// </summary>
        public virtual void ClearAllRequirements()
        {
            Requirements.Clear();
        }

        /// <summary>
        ///     Copy traceability information from the element provided in parameter (source) to the current one (target)
        ///     Do the union between traceability information
        /// </summary>
        /// <param name="source"></param>
        protected void KeepTraceability(ModelElement source)
        {
            ReqRelated reqSource = source as ReqRelated;
            if (reqSource != null)
            {
                foreach (ReqRef sourceReqRef in reqSource.Requirements)
                {
                    bool isPresent = false;
                    foreach (ReqRef reqRef in Requirements)
                    {
                        if (sourceReqRef.Paragraph.SourceOfUpdateChain == reqRef.Paragraph.SourceOfUpdateChain)
                        {
                            isPresent = true;
                            break;
                        }
                    }

                    if (!isPresent)
                    {
                        Requirements.Add(sourceReqRef.Duplicate());
                    }
                }
            }
        }
    }
}