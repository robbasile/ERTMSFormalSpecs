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
using System.Collections.Generic;
using DataDictionary.Generated;

namespace DataDictionary.Specification
{
    /// <summary>
    ///     Represents a requirement set
    /// </summary>
    public class RequirementSet : Generated.RequirementSet, IGraphicalDisplay, IHoldsParagraphs, IHoldsRequirementSets
    {
        /// <summary>
        ///     Provides all the dependances related to this requirement set
        /// </summary>
        public ArrayList Dependancies
        {
            get
            {
                if (allDependancies() == null)
                {
                    setAllDependancies(new ArrayList());
                }

                return allDependancies();
            }
        }

        /// <summary>
        ///     Provides all the sub set of this requirement set
        /// </summary>
        public ArrayList SubSets
        {
            get
            {
                if (allSubSets() == null)
                {
                    setAllSubSets(new ArrayList());
                }

                return allSubSets();
            }
        }

        /// <summary>
        ///     The X position
        /// </summary>
        public int X
        {
            get { return getX(); }
            set { setX(value); }
        }

        /// <summary>
        ///     The Y position
        /// </summary>
        public int Y
        {
            get { return getY(); }
            set { setY(value); }
        }

        /// <summary>
        ///     The width
        /// </summary>
        public int Width
        {
            get { return getWidth(); }
            set { setWidth(value); }
        }

        /// <summary>
        ///     The height
        /// </summary>
        public int Height
        {
            get { return getHeight(); }
            set { setHeight(value); }
        }

        /// <summary>
        ///     The name to be displayed
        /// </summary>
        public string GraphicalName
        {
            get { return Name; }
        }

        /// <summary>
        ///     Indicates that the element is hiddent
        /// </summary>
        public bool Hidden
        {
            get { return false; }
            set { }
        }

        /// <summary>
        ///     Indicates that the element is pinned
        /// </summary>
        public bool Pinned
        {
            get { return getPinned(); }
            set { setPinned(value); }
        }

        /// <summary>
        ///     The explanation of the element
        /// </summary>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public string getExplain(bool explainSubElements)
        {
            return Name;
        }

        /// <summary>
        ///     The collection in which this model element lies
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;

                RequirementSet enclosingSet = Enclosing as RequirementSet;
                if (enclosingSet != null)
                {
                    retVal = enclosingSet.allSubSets();
                }
                else
                {
                    retVal = Dictionary.allRequirementSets();
                }

                return retVal;
            }
        }

        private class ParagraphForRequirementSet : Visitor
        {
            /// <summary>
            ///     The requirement set for which the paragraphs should be found
            /// </summary>
            private RequirementSet RequirementSet { get; set; }

            /// <summary>
            ///     The list of paragraphs to be filled
            /// </summary>
            public List<Paragraph> Paragraphs { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="requirementSet"></param>
            public ParagraphForRequirementSet(RequirementSet requirementSet, List<Paragraph> paragraphs)
            {
                RequirementSet = requirementSet;
                Paragraphs = paragraphs;
            }

            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (paragraph.BelongsToRequirementSet(RequirementSet))
                {
                    Paragraphs.Add(paragraph);

                    if (RequirementSet.getRecursiveSelection())
                    {
                        paragraph.GetParagraphs(Paragraphs);
                    }
                    else
                    {
                        base.visit(obj, visitSubNodes);
                    }
                }
                else
                {
                    base.visit(obj, visitSubNodes);
                }
            }
        }

        /// <summary>
        ///     Provides the paragraphs related to this requirement set
        /// </summary>
        /// <param name="paragraphs"></param>
        public void GetParagraphs(List<Paragraph> paragraphs)
        {
            ParagraphForRequirementSet gatherer = new ParagraphForRequirementSet(this, paragraphs);
            foreach (Dictionary dictionary in EFSSystem.Dictionaries)
            {
                gatherer.visit(dictionary);
            }
        }

        /// <summary>
        ///     Provides the list of requirement sets in the system
        /// </summary>
        public List<RequirementSet> RequirementSets
        {
            get
            {
                List<RequirementSet> retVal = new List<RequirementSet>();

                foreach (RequirementSet requirementSet in SubSets)
                {
                    retVal.Add(requirementSet);
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the requirement set whose name corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <param name="create">Indicates that the requirement set should be created if it does not exists</param>
        /// <returns></returns>
        public RequirementSet findRequirementSet(string name, bool create)
        {
            RequirementSet retVal = null;

            foreach (RequirementSet requirementSet in SubSets)
            {
                if (requirementSet.Name == name)
                {
                    retVal = requirementSet;
                    break;
                }
            }

            if (retVal == null && create)
            {
                retVal = (RequirementSet) acceptor.getFactory().createRequirementSet();
                retVal.Name = name;
                appendSubSets(retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a new requirement set to this list of requirement sets
        /// </summary>
        /// <param name="requirementSet"></param>
        public void AddRequirementSet(RequirementSet requirementSet)
        {
            appendSubSets(requirementSet);
        }


        /// <summary>
        ///     The name of the requireement set for scoping information
        /// </summary>
        public const string ONBOARD_SCOPE_NAME = "Onboard";

        /// <summary>
        ///     The name of the requireement set for scoping information
        /// </summary>
        public const string TRACKSIDE_SCOPE_NAME = "Trackside";

        /// <summary>
        ///     The name of the requireement set for scoping information
        /// </summary>
        public const string ROLLING_STOCK_SCOPE_NAME = "Rolling stock";

        /// <summary>
        ///     Sets the default requirement sets for the paragraph
        /// </summary>
        /// <param name="paragraph"></param>
        public void setDefaultRequirementSets(Paragraph paragraph)
        {
            if (getDefault())
            {
                paragraph.AppendToRequirementSet(this);
            }
            foreach (RequirementSet subRequirementSet in RequirementSets)
            {
                subRequirementSet.setDefaultRequirementSets(paragraph);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Write("REQUIREMENT SET ");
            explanation.WriteLine(Name);
        }
    }
}