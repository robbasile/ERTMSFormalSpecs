using System.Collections.Generic;
using DataDictionary.Types;
using DataDictionary.Variables;
using NUnit.Framework;
using Utils;

namespace DataDictionary.test.updateModel
{
    [TestFixture]
    internal class UpdateChecks : BaseModelTest
    {
        /// <summary>
        ///     Test that the checker correctly flags model elements that have been updated more than once
        /// </summary>
        [Test]
        public void TestElementUpdatedTwice()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Variable variable = CreateVariable(nameSpace, "var", "Boolean");

            Dictionary dictionaryUpdate1 = CreateDictionary("TestUpdate1");
            dictionaryUpdate1.setUpdates(dictionary.Guid);
            Variable variableUpdate1 = variable.CreateVariableUpdate(dictionaryUpdate1);

            Dictionary dictionaryUpdate2 = CreateDictionary("TestUpdate2");
            dictionaryUpdate2.setUpdates(dictionary.Guid);
            Variable variableUpdate2 = variable.CreateVariableUpdate(dictionaryUpdate2);

            dictionary.CheckRules();

            // Check that exactly 3 model elements contain errors
            Assert.AreEqual(3, ModelElement.Errors.Count);

            // Check that each model element only contains one error, and what that error is
            int updateErrors = 0;
            foreach (KeyValuePair<Utils.ModelElement, List<ElementLog>> pair in ModelElement.Errors)
            {
                Assert.AreEqual(1, pair.Value.Count);
                if (pair.Value[0].Log.Contains("Updates conflict"))
                {
                    updateErrors ++;
                }
            }

            // Check taht all three errors are related to the updates conflict
            Assert.AreEqual(updateErrors, 3);
        }

        /// <summary>
        ///     Test that the checker correctly flags model element updates that cannot find their base
        /// </summary>
        [Test]
        public void TestNoUpdatedElement()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Variable variable = CreateVariable(nameSpace, "var", "Boolean");

            Dictionary dictionaryUpdate1 = CreateDictionary("TestUpdate1");
            dictionaryUpdate1.setUpdates(dictionary.Guid);
            Variable variableUpdate1 = variable.CreateVariableUpdate(dictionaryUpdate1);

            variable.Delete();

            dictionary.CheckRules();
            dictionaryUpdate1.CheckRules();

            // There is only one error
            Assert.AreEqual(1, ModelElement.Errors.Count);

            // The only model element with errors is the variable update
            Assert.That(ModelElement.Errors.ContainsKey(variableUpdate1));

            List<ElementLog> errorsList = ModelElement.Errors[variableUpdate1];
            // That model element only has one error
            Assert.AreEqual(errorsList.Count, 1);

            // The error is that it does not have a base element to update
            Assert.That(errorsList[0].Log == "Update02: Cannot find the element updated by this.");
        }
    }
}