using System.Collections.Generic;
using DataDictionary;
using DataDictionary.Rules;
using DataDictionary.test;
using DataDictionary.Types;
using DataDictionary.Variables;
using GUIUtils.Editor;
using NUnit.Framework;
using Utils;

namespace GUIUtils.test.Editor
{
    [TestFixture, RequiresSTA]
    public class TestBaseEditorTextBox : BaseModelTest
    {
        /// <summary>
        /// Indicates whether the list of object references contains the provided model element
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool Contains(IEnumerable<ObjectReference> choices, INamable model)
        {
            bool retVal = false;

            foreach (ObjectReference objectReference in choices)
            {
                if (objectReference.Model == model)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates whether the list of object references contains the provided proposal
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="proposal"></param>
        /// <returns></returns>
        private bool Contains(IEnumerable<ObjectReference> choices, string proposal)
        {
            bool retVal = false;

            foreach (ObjectReference objectReference in choices)
            {
                if (objectReference.DisplayName.Trim() == proposal.Trim())
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        [Test]
        public void TestSimpleExpression()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("N1.S1 { E1 => True }");

            Compiler.Compile_Synchronous(true, true);
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            BaseEditorTextBox textBox = new BaseEditorTextBox();
            {
                //              0123456789
                textBox.Text = "V <- N1.S";
                textBox.Instance = rc;
                List<ObjectReference> choices = textBox.AllChoices(7, "S");
                Assert.IsNotNull(choices);
                Assert.AreEqual(2, choices.Count);
                Assert.IsTrue(Contains(choices, s1));
                Assert.IsTrue(Contains(choices, s2));
            }

            {
                //              0123456789
                textBox.Text = "V <- N1.";
                textBox.Instance = rc;
                List<ObjectReference> choices = textBox.AllChoices(7, "");
                Assert.IsNotNull(choices);
                Assert.AreEqual(3, choices.Count);
                Assert.IsTrue(Contains(choices, s1));
                Assert.IsTrue(Contains(choices, s2));
                Assert.IsTrue(Contains(choices, v));
            }
        }

        /// <summary>
        /// Do not propose anything when the instance on which the "." is applied is null
        /// </summary>
        [Test]
        public void NoProposalWhenInstanceIsNull()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("N1.S1 { E1 => True }");

            Compiler.Compile_Synchronous(true, true);
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            BaseEditorTextBox textBox = new BaseEditorTextBox();

            {
                //              0123456789
                textBox.Text = "V <- N2.";
                textBox.Instance = rc;
                List<ObjectReference> choices = textBox.AllChoices(7, "");
                Assert.IsNotNull(choices);
                Assert.AreEqual(0, choices.Count);
            }
        }

        [Test]
        public void TestLetExpression()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S2");
            v.setDefaultValue("N1.S1 { E1 => True }");

            Compiler.Compile_Synchronous(true, true);
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            BaseEditorTextBox textBox = new BaseEditorTextBox();
            {
                //              0         1         2
                //              012345678901234567890123456789
                textBox.Text = "LET d <- V.E2 IN d.E ";
                textBox.Instance = rc;
                List<ObjectReference> choices = textBox.AllChoices(17, "E");
                Assert.IsNotNull(choices);
                Assert.AreEqual(1, choices.Count);
                Assert.IsTrue(Contains(choices, el1));
            }
        }

        [Test]
        public void TestTemplates()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S2");
            v.setDefaultValue("N1.S1 { E1 => True }");

            Compiler.Compile_Synchronous(true, true);
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            BaseEditorTextBox textBox = new BaseEditorTextBox();
            {
                //              0         1         2
                //              012345678901234567890123456789
                textBox.Text = "T";
                textBox.Instance = rc;
                List<ObjectReference> choices = textBox.AllChoices(0, "T");
                Assert.IsNotNull(choices);
                Assert.AreEqual(2, choices.Count);
                Assert.IsTrue(Contains(choices, "THERE_IS X IN <collection> | <condition>"));
                Assert.IsTrue(Contains(choices, "TARGETS"));
            }            
        }
    }
}
