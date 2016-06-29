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
using System.Globalization;
using System.IO;
using DataDictionary.Generated;
using DataDictionary.Tests.Issue;
using Utils;
using DBMessage = DataDictionary.Tests.DBElements.DBMessage;
using SourceText = DataDictionary.Tests.Translations.SourceText;
using SourceTextComment = DataDictionary.Tests.Translations.SourceTextComment;
using Translation = DataDictionary.Tests.Translations.Translation;

namespace DataDictionary.Tests
{
    public class Step : Generated.Step, ITextualExplain, IEditable
    {
        /// <summary>
        ///     The text displayed for a step that has not been named
        /// </summary>
        public static string DefaultName = "New step";

        public override string Name
        {
            get
            {
                string retVal;

                if (getTCS_Order() != 0)
                {
                    retVal = "Step " + getTCS_Order() + ": " + getDescription();
                }
                else
                {
                    retVal = base.Name;

                    if (Utils.Util.isEmpty(retVal))
                    {
                        retVal = getDescription();
                    }

                    if (Utils.Util.isEmpty(retVal))
                    {
                        retVal = DefaultName;
                    }
                }

                return retVal;
            }
            set
            {
                if (getTCS_Order() == 0)
                {
                    base.Name = value;
                }
            }
        }

        public ArrayList SubSteps
        {
            get
            {
                if (allSubSteps() == null)
                {
                    setAllSubSteps(new ArrayList());
                }
                return allSubSteps();
            }
        }


        /// <summary>
        ///     The messages associated to this step
        /// </summary>
        public ArrayList StepMessages
        {
            get
            {
                if (allMessages() == null)
                {
                    setAllMessages(new ArrayList());
                }
                return allMessages();
            }
        }

        /// <summary>
        ///     The enclosing test case
        /// </summary>
        public TestCase TestCase
        {
            get { return Enclosing as TestCase; }
        }

        /// <summary>
        ///     The collection which encloses this step
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return TestCase.Steps; }
        }

        /// <summary>
        ///     Provides the sub sequence for this step
        /// </summary>
        public SubSequence SubSequence
        {
            get { return EnclosingFinder<SubSequence>.find(this); }
        }

        /// <summary>
        ///     The distance at which this step is executed
        /// </summary>
        public double Distance
        {
            get {
                double retVal;

                if (!double.TryParse(getDistance(), NumberStyles.Any, CultureInfo.InvariantCulture, out retVal))
                {
                    retVal = 0.0;
                }

                return retVal;                
            }
            set { setDistance(value.ToString(CultureInfo.InvariantCulture));}
        }

        /// <summary>
        ///     Indicates if this step contains some actions or expectations
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            bool retVal;

            if (SubSteps.Count == 0)
            {
                retVal = true;
            }
            else
            {
                retVal = true;
                foreach (SubStep subStep in SubSteps)
                {
                    if (!subStep.IsEmpty())
                    {
                        retVal = false;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Translates the current step according to the translation dictionary
        ///     Removes all preconditions, actions and expectations
        /// </summary>
        /// <param name="applyTranslation">Indicates that the translation should be applied. Otherwise, this method only cleans the step. 
        /// This is used to handle the fact that a blocking error has been found, and translating the sub sequence should be stopped, 
        /// but the next steps should be cleaned</param>
        /// <returns>False if an error has been found while translating this step, or if translations should not be applied</returns>
        public bool Translate(bool applyTranslation)
        {
            bool retVal = applyTranslation;

            Counter counter = new Counter(false);
            counter.visit(this);
            if (counter.Issues[IssueKind.Blocking] != 0)
            {
                retVal = false;
            }

            if (getTranslationRequired())
            {
                Util.DontNotify(() =>
                {
                    setTranslated(false);
                    SubSteps.Clear();

                    if (retVal)
                    {
                        Translation translation = EFSSystem.FindTranslation(this);
                        if (translation != null)
                        {
                            translation.UpdateStep(this);
                            setTranslated(true);
                        }
                        else
                        {
                            AddWarning("Cannot find translation for this step");
                        }
                    }
                });
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public override void AddModelElement(IModelElement element)
        {
            SubStep item = element as SubStep;
            if (item != null)
            {
                appendSubSteps(item);
            }
        }


        /// <summary>
        ///     Fills the actual step with information of another test case
        /// </summary>
        /// <param name="previousStep"></param>
        /// <param name="keepManualTranslations">Indicates that manual translation for be kept during import</param>
        public void Merge(Step previousStep, bool keepManualTranslations)
        {
            if (keepManualTranslations)
            {
                setAllSubSteps(previousStep.SubSteps);
            }

            setGuid(previousStep.getGuid());
            setComment(previousStep.Comment);
            setTranslated(previousStep.getTranslated());
            setTranslationRequired(previousStep.getTranslationRequired());

            int cnt = 0;
            foreach (DBMessage message in StepMessages)
            {
                if (cnt < previousStep.StepMessages.Count)
                {
                    message.Merge((DBMessage) previousStep.StepMessages[cnt]);
                }
                cnt += 1;
            }

            foreach (ReqRef reqRef in previousStep.Requirements)
            {
                appendRequirements(reqRef);
            }
        }

        /// <summary>
        ///     Adds a new message
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(DBMessage message)
        {
            allMessages().Add(message);
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static Step CreateDefault(ICollection enclosingCollection)
        {
            Step retVal = (Step) acceptor.getFactory().createStep();

            Util.DontNotify(() => retVal.appendSubSteps(SubStep.CreateDefault(retVal.SubSteps)));

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Comment(this);

            explanation.Indent(2, () =>
            {
                foreach (SubStep subStep in SubSteps)
                {
                    subStep.GetExplain(explanation, explainSubElements);
                    explanation.WriteLine();
                }
            });
        }

        /// <summary>
        ///     Creates the source text which corresponds to this step
        /// </summary>
        /// <returns></returns>
        public SourceText CreateSourceText()
        {
            SourceText retVal = (SourceText) acceptor.getFactory().createSourceText();
            retVal.Name = getDescription();

            if (!string.IsNullOrEmpty(Comment) && Comment.Trim() != "-")
            {
                SourceTextComment comment = (SourceTextComment) acceptor.getFactory().createSourceTextComment();
                comment.Name = Comment;
                retVal.appendComments(comment);
            }

            return retVal;
        }

        /// <summary>
        ///     !!! Clean HacK !!!
        ///     Do not save the substeps when the step requires automatic translation
        ///     !!! Clean HaCk !!!
        /// </summary>
        /// <param name="pw"></param>
        /// <param name="typeId"></param>
        /// <param name="headingTag"></param>
        /// <param name="endingTag"></param>
        public override void unParse(TextWriter pw, bool typeId, string headingTag, string endingTag)
        {
            if (getTranslationRequired())
            {
                ArrayList tmp = allSubSteps();
                bool translated = getTranslated();

                setAllSubSteps(null);
                setTranslated(false);

                base.unParse(pw, typeId, headingTag, endingTag);

                setAllSubSteps(tmp);
                setTranslated(translated);
            }
            else
            {
                base.unParse(pw, typeId, headingTag, endingTag);
            }
        }

        /// <summary>
        ///     Provides the previous step (if any) in the subsequence
        /// </summary>
        public Step PreviousStep
        {
            get
            {
                Step retVal = null;

                bool found = false;
                foreach (TestCase testCase in SubSequence.TestCases)
                {
                    foreach (Step step in testCase.Steps)
                    {
                        if (step == this)
                        {
                            found = true;
                            break;
                        }

                        retVal = step;
                    }

                    if (found)
                    {
                        break;
                    }
                }

                if (!found)
                {
                    retVal = null;
                }

                return retVal;
            }
        }

        /// <summary>
        /// The text to edit
        /// </summary>
        public string Text
        {
            get
            {
                string retVal;

                if (getTCS_Order() == 0)
                {
                    retVal = Name;
                }
                else
                {
                    retVal = getDescription();
                }

                return retVal;
            }
            set
            {
                if (getTCS_Order() == 0)
                {
                    Name = value;
                }
                else
                {
                    setDescription(value);
                }
            }
        }

        /// <summary>
        /// No syntax highlighting for this
        /// </summary>
        public bool SyntaxHightlight { get { return false; } }

        /// <summary>
        /// Editable window title
        /// </summary>
        public string Title { get { return "Step"; } }    
    }
}