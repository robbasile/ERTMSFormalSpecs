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
using System.Collections.Generic;
using DataDictionary.Generated;
using Utils;
using Comparer = Utils.Comparer;
using RuleCheckDisabling = DataDictionary.RuleCheck.RuleCheckDisabling;

namespace DataDictionary.Tests
{
    public class SubSequence : Generated.SubSequence, IComparable<SubSequence>, ICommentable, RuleCheck.IRuleCheckDisabling
    {
        /// <summary>
        /// 
        /// </summary>
        public RuleCheckDisabling Disabling
        {
            get { return (RuleCheckDisabling)getRuleCheckDisabling(); }
            set { setRuleCheckDisabling(value); }
        }
        
        /// <summary>
        ///     The comparer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SubSequence other)
        {
            return Comparer.StringComparer.Compare(Name, other.Name);
        }

        public override string ExpressionText
        {
            get
            {
                string retVal = "";

                retVal += "D_LRBG = " + getD_LRBG() + "\n";
                retVal += "Level = " + getLevel() + "\n";
                retVal += "Mode = " + getMode() + "\n";
                retVal += "NID_LRBG = " + getNID_LRBG() + "\n";
                retVal += "Q_DIRLRBG = " + getQ_DIRLRBG() + "\n";
                retVal += "Q_DIRTRAIN = " + getQ_DIRTRAIN() + "\n";
                retVal += "Q_DLRBG = " + getQ_DLRBG() + "\n";
                retVal += "RBC_ID = " + getRBC_ID() + "\n";
                retVal += "RBCPhone = " + getRBCPhone() + "\n";

                return retVal;
            }
        }

        public ArrayList TestCases
        {
            get
            {
                if (allTestCases() == null)
                {
                    setAllTestCases(new ArrayList());
                }
                return allTestCases();
            }
        }

        /// <summary>
        ///     The enclosing frame
        /// </summary>
        public Frame Frame
        {
            get { return Enclosing as Frame; }
        }

        /// <summary>
        ///     Executes the test cases for this test sequence
        /// </summary>
        /// <param name="runner">The runner used to execute the test case</param>
        /// <returns>the number of failed test cases</returns>
        public int ExecuteAllTestCases(Runner.Runner runner)
        {
            int retVal = 0;

            foreach (TestCase testCase in TestCases)
            {
                int currentFailed = runner.FailedExpectations().Count;
                runner.RunUntilStep(null);
                if (runner.FailedExpectations().Count > currentFailed)
                {
                    retVal = retVal + 1;
                }
            }

            return retVal;
        }

        public override ArrayList EnclosingCollection
        {
            get { return Frame.SubSequences; }
        }

        /// <summary>
        ///     Provides the test cases for this sub sequence
        /// </summary>
        /// <param name="testCases"></param>
        public void FillTestCases(List<TestCase> testCases)
        {
            foreach (TestCase testCase in TestCases)
            {
                testCases.Add(testCase);
            }
        }

        /// <summary>
        ///     Translates the sub sequence, according to the tanslation dictionary provided
        /// </summary>
        public void Translate()
        {
            foreach (TestCase testCase in TestCases)
            {
                testCase.Translate();
            }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                TestCase item = element as TestCase;
                if (item != null)
                {
                    appendTestCases(item);
                }
            }
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static SubSequence CreateDefault(ICollection enclosingCollection)
        {
            SubSequence retVal = (SubSequence) acceptor.getFactory().createSubSequence();

            Util.DontNotify(() =>
            {
                retVal.Name = "SubSequence" + GetElementNumber(enclosingCollection);
                retVal.appendTestCases(TestCase.CreateDefault(retVal.TestCases));
            });

            return retVal;
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
        /// Indicates that the sub sequence contains a manual translation
        /// </summary>
        /// <returns></returns>
        public bool ContainsManualTranslation()
        {
            bool retVal = false;

            foreach (TestCase testCase in TestCases)
            {
                foreach (Step step in testCase.Steps)
                {
                    if (!step.getTranslationRequired())
                    {
                        retVal = true;
                        break;
                    }
                }

                if (retVal)
                {
                    break;
                }
            }

            return retVal;
        }
    }
}