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
using DataDictionary.Generated;
using Utils;
using Action = DataDictionary.Rules.Action;
using Translation = DataDictionary.Tests.Translations.Translation;

namespace DataDictionary.Tests
{
    public class SubStep : Generated.SubStep, ITextualExplain, ICommentable
    {
        /// <summary>
        ///     This step changes
        /// </summary>
        public ArrayList Actions
        {
            get
            {
                if (allActions() == null)
                {
                    setAllActions(new ArrayList());
                }
                return allActions();
            }
        }


        /// <summary>
        ///     This step expectations
        /// </summary>
        public ArrayList Expectations
        {
            get
            {
                if (allExpectations() == null)
                {
                    setAllExpectations(new ArrayList());
                }
                return allExpectations();
            }
        }

        /// <summary>
        ///     The enclosing step, if any
        /// </summary>
        public Step Step
        {
            get { return Enclosing as Step; }
        }

        /// <summary>
        ///     The enclosing translation, if any
        /// </summary>
        public Translation Translation
        {
            get { return Enclosing as Translation; }
        }

        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;

                if (Step != null)
                {
                    retVal = Step.SubSteps;
                }
                else if (Translation != null)
                {
                    retVal = Translation.SubSteps;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            if (element is Action)
            {
                Action item = element as Action;
                if (item != null)
                {
                    appendActions(item);
                }
            }
            else if (element is Expectation)
            {
                Expectation item = element as Expectation;
                if (item != null)
                {
                    appendExpectations(item);
                }
            }
        }

        /// <summary>
        ///     Indicates if this step contains some actions or expectations
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            bool retVal = true;
            if (Actions.Count != 0 || Expectations.Count != 0)
            {
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static SubStep CreateDefault(ICollection enclosingCollection)
        {
            SubStep retVal = (SubStep)acceptor.getFactory().createSubStep();
            retVal.Name = "SubStep" + GetElementNumber(enclosingCollection);

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Comment(Name);

            explanation.Indent(2, () =>
            {
                foreach (Action action in Actions)
                {
                    action.GetExplain(explanation, explainSubElements);
                    explanation.WriteLine();
                }                
            });

            explanation.WriteLine("IMPLIES");

            explanation.Indent(2, () =>
            {
                foreach (Expectation expectation in Expectations)
                {
                    expectation.GetExplain(explanation, explainSubElements);
                    explanation.WriteLine();
                }
            });
        }

        /// <summary>
        ///     The comment related to this element
        /// </summary>
        public string Comment
        {
            get { return getComment(); }
            set { setComment(value); }
        }


        /// <summary>
        ///     Indicates whether this substep references the special variable %Message_i
        /// </summary>
        /// <returns></returns>
        public bool ReferencesMessages()
        {
            bool retVal = false;

            foreach (Generated.Action action in Actions)
            {
                if (action.ExpressionText.Contains("%Message"))
                {
                    retVal = true;
                    break;
                }
            }

            foreach (Expectation expectation in Expectations)
            {
                if (expectation.ExpressionText.Contains("%Message"))
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }


        /// <summary>
        /// Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <param name="enclosingId"></param>
        /// <returns></returns>
        public static SubStep CreateDefault(ICollection enclosingCollection, string enclosingId)
        {
            SubStep retVal = (SubStep)acceptor.getFactory().createSubStep();
            retVal.Name = "SubStep" + GetElementNumber(enclosingCollection);

            return retVal;
        }
    }
}