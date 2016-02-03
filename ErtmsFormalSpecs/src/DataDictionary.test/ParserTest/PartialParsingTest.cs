using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Variables;
using NUnit.Framework;

namespace DataDictionary.test.ParserTest
{
    [TestFixture]
    public class PartialParsingTest : BaseModelTest
    {
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
            Parser parser = new Parser();
            VariableUpdateStatement statement = parser.Statement(rc, "V <- N1.S", true, true) as VariableUpdateStatement;
            Assert.IsNotNull(statement);
            Assert.AreEqual(statement.VariableIdentification.Ref, v);

            DerefExpression deref = statement.Expression as DerefExpression;
            Assert.IsNotNull(deref);
            Assert.AreEqual(deref.Arguments[0].Ref, n1);

            statement = parser.Statement(rc, "V <- N1.", true, true) as VariableUpdateStatement;
            Assert.IsNotNull(statement);
            Assert.AreEqual(statement.VariableIdentification.Ref, v);

            deref = statement.Expression as DerefExpression;
            Assert.IsNotNull(deref);
            Assert.AreEqual(deref.Arguments[0].Ref, n1);
        }

        [Test]
        public void TestFunctionCall()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("N1.S1 { E1 => True }");
            Function function = CreateFunction(n1, "f", "S1");

            Compiler.Compile_Synchronous(true, true);

            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Parser parser = new Parser();
            VariableUpdateStatement statement = parser.Statement(rc, "V <- f().S", true, true) as VariableUpdateStatement;
            Assert.IsNotNull(statement);
            Assert.AreEqual(statement.VariableIdentification.Ref, v);

            DerefExpression deref = statement.Expression as DerefExpression;
            Assert.IsNotNull(deref);
            Assert.AreEqual(deref.Arguments[0].Ref, s1);

            statement = parser.Statement(rc, "V <- f().", true, true) as VariableUpdateStatement;
            Assert.IsNotNull(statement);
            Assert.AreEqual(statement.VariableIdentification.Ref, v);

            deref = statement.Expression as DerefExpression;
            Assert.IsNotNull(deref);
            Assert.AreEqual(deref.Arguments[0].Ref, s1);
        }
    }
}
