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
using DataDictionary.Tests.Issue;
using Utils;

namespace DataDictionary.Tests
{
    public class TestCase : Generated.TestCase, ITextualExplain
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
        /// <param name="applyTranslations">Indicates that the translation should be applied. Otherwise, this method only cleans the step. 
        /// This is used to handle the fact that a blocking error has been found, and translating the sub sequence should be stopped, 
        /// but the next steps should be cleaned</param>
        /// <returns>False if an error has been found while translating this test case, or if translations should not be applied</returns>
        public bool Translate(bool applyTranslations)
        {
            bool retVal = applyTranslations;

            foreach (Step step in Steps)
            {
                retVal = step.Translate(retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
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
        /// <param name="previousTestCase"></param>
        /// <param name="keepManualTranslations">Indicates that manual translation for be kept during import</param>
        public void Merge(TestCase previousTestCase, bool keepManualTranslations)
        {
            if (previousTestCase != null)
            {
                setGuid(previousTestCase.getGuid());
                foreach (Step step in Steps)
                {
                    Step previousStep = null;
                    foreach (Step other in previousTestCase.Steps)
                    {
                        if (other.getDescription() == step.getDescription() &&
                            other.getTCS_Order() == step.getTCS_Order())
                        {
                            previousStep = other;
                            break;
                        }
                    }

                    if (previousStep != null)
                    {
                        if (step.getTCS_Order() == previousStep.getTCS_Order())
                        {
                            step.Merge(previousStep, keepManualTranslations);
                        }
                        else
                        {
                            throw new Exception("The new version of the test case " + Name + " contains the step " +
                                                step.Name + " instead of " + previousStep.Name);
                        }
                    }
                }

                if (keepManualTranslations)
                {
                    int index = 0;
                    foreach (Step step in previousTestCase.Steps)
                    {
                        if (step.getTCS_Order() == 0 && !step.getTranslationRequired())
                        {
                            // This step has been added manually
                            allSteps().Insert(index, step);
                            step.setFather(this);
                        }
                        index += 1;
                    }
                }

                foreach (ReqRef reqRef in previousTestCase.Requirements)
                {
                    appendRequirements(reqRef);
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

        /// <summary>
        /// Indicates that at least one step for this test case requires an automatic translation
        /// </summary>
        public bool ContainsTranslations
        {
            get
            {
                bool retVal = false;

                foreach (Step step in Steps)
                {
                    if (step.getTranslationRequired())
                    {
                        retVal = true;
                        break;
                    }
                }

                return retVal;
            } 
            
        }
    }
}