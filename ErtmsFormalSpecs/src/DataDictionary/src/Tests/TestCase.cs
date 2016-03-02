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
using System.Collections;
using DataDictionary.Generated;
using Utils;
using TranslationDictionary = DataDictionary.Tests.Translations.TranslationDictionary;

namespace DataDictionary.Tests
{
    public class TestCase : Generated.TestCase, ITextualExplain, ICommentable
    {
        public ArrayList Steps
        {
            get
            {
                if (allSteps() == null)
                {
                    setAllSteps(new ArrayList());
                }
                return allSteps();
            }
        }

        public override string Name
        {
            get
            {
                string retVal = base.Name;

                if (Utils.Util.isEmpty(retVal))
                {
                    retVal = "Feature " + getFeature() + ", test case " + getCase();
                }

                return retVal;
            }
            set { base.Name = value; }
        }

        /// <summary>
        ///     Provides the sub sequence for this step
        /// </summary>
        public SubSequence SubSequence
        {
            get { return EnclosingFinder<SubSequence>.find(this); }
        }


        public override ArrayList EnclosingCollection
        {
            get { return SubSequence.TestCases; }
        }

        /// <summary>
        ///     Translates the current step, according to the translation dictionary
        /// </summary>
        public void Translate()
        {
            foreach (Step step in Steps)
            {
                step.Translate();
            }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                Step item = element as Step;
                if (item != null)
                {
                    appendSteps(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        ///     Fills the actual test case with steps of another test case
        /// </summary>
        /// <param name="oldTestCase"></param>
        public void Merge(TestCase aTestCase)
        {
            if (aTestCase != null)
            {
                setGuid(aTestCase.getGuid());
                foreach (Step step in Steps)
                {
                    Step oldStep = null;
                    foreach (Step other in aTestCase.Steps)
                    {
                        if (other.getDescription() == step.getDescription() &&
                            other.getTCS_Order() == step.getTCS_Order())
                        {
                            oldStep = other;
                            break;
                        }
                    }

                    if (oldStep != null)
                    {
                        if (step.getTCS_Order() == oldStep.getTCS_Order())
                        {
                            step.Merge(oldStep);
                        }
                        else
                        {
                            throw new Exception("The new version of the test case " + Name + " contains the step " +
                                                step.Name + " instead of " + oldStep.Name);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static TestCase CreateDefault(ICollection enclosingCollection)
        {
            TestCase retVal = (TestCase) acceptor.getFactory().createTestCase();

            Util.DontNotify(() =>
            {
                retVal.Name = "TestCase" + GetElementNumber(enclosingCollection);
                retVal.appendSteps(Step.CreateDefault(retVal.Steps));
            });

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
                foreach (Step step in Steps)
                {
                    step.GetExplain(explanation, explainSubElements);
                    explanation.WriteLine();
                }
            });
        }

        /// <summary>
        ///     Provides the number of actions & expectations for this test case
        /// </summary>
        public int ActionCount
        {
            get
            {
                int retVal = 0;

                foreach (Step step in Steps)
                {
                    foreach (SubStep subStep in step.SubSteps)
                    {
                        retVal += subStep.Actions.Count + subStep.Expectations.Count;
                    }
                }

                return retVal;
            }
        }
    }
}