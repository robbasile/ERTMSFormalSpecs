using DataDictionary.Functions;
using DataDictionary.RuleCheck;
using DataDictionary.Rules;
using DataDictionary.Types;
using NUnit.Framework;

namespace DataDictionary.test.updateModel
{
    [TestFixture]
    public class UpdateStructureTest : BaseModelTest
    {
        /// <summary>
        ///     Tests that a structure can be updated
        /// </summary>
        [Test]
        public void TestUpdateFunction()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Structure structure = CreateStructure(nameSpace, "S");
            StructureElement el1 = CreateStructureElement(structure, "A", "Boolean");
            RuleCondition condition = CreateRuleAndCondition(structure, "Cond");
            Action action = CreateAction(condition, "A <- True");
            Procedure procedure = CreateProcedure(structure, "P");
            Parameter parameter = CreateParameter(procedure, "V", "Boolean");
            RuleCondition condition2 = CreateRuleAndCondition(procedure, "Cond");
            Action action2 = CreateAction(condition2, "A <- V");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Structure updatedStructure = structure.CreateStructureUpdate(dictionary2);

            Compiler.Compile_Synchronous(true);
            RuleCheckerVisitor visitor = new RuleCheckerVisitor(dictionary);
            visitor.visit(updatedStructure);

            Assert.IsNull(ErrorMessage(updatedStructure));
        }
    }
}