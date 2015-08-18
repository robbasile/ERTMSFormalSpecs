using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using NUnit.Framework;

namespace DataDictionary.test
{
    [TestFixture]
    internal class SemanticAnalysisTest : BaseModelTest
    {
        /// <summary>
        ///     Tests that an EFS expression that can refer to either a variable or a type will reference the variable
        /// </summary>
        [Test]
        public void TestVariableAndTypeWithSameName()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "NameSpace");

            Structure structure = CreateStructure(nameSpace, "ModelElement");
            StructureElement structElem = CreateStructureElement(structure, "Value", "Boolean");
            structElem.setDefault("True");

            Variable variable = CreateVariable(nameSpace, "ModelElement", "ModelElement");
            variable.SubVariables["Value"].Value = System.BoolType.False;

            Expression expression = Parser.Expression(dictionary, "NameSpace.ModelElement.Value");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);

            Assert.AreEqual(value, variable.SubVariables["Value"].Value);
        }
    }
}