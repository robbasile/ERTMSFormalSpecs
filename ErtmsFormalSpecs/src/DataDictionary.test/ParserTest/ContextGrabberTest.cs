using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.ListOperators;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Variables;
using NUnit.Framework;

namespace DataDictionary.test.ParserTest
{
    [TestFixture]
    public class ContextGrabberTest : BaseModelTest
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

            {
                //                   0123456789
                const string text = "V <- N1.S";
                VariableUpdateStatement statement = parser.Statement(rc, text, true, true) as VariableUpdateStatement;
                ContextGrabber grabber = new ContextGrabber();
                Assert.AreEqual(v, grabber.GetContext(0, statement));
                Assert.AreEqual(v, grabber.GetContext(1, statement));
                Assert.IsNull(grabber.GetContext(2, statement));
                Assert.IsNull(grabber.GetContext(3, statement));
                Assert.IsNull(grabber.GetContext(4, statement));
                Assert.AreEqual(n1, grabber.GetContext(5, statement));
                Assert.AreEqual(n1, grabber.GetContext(6, statement));
                Assert.AreEqual(n1, grabber.GetContext(7, statement));
                Assert.IsNull(grabber.GetContext(8, statement));
                Assert.IsNull(grabber.GetContext(9, statement));
                Assert.IsNull(grabber.GetContext(10, statement));
            }

            {
                //                   0123456789
                const string text = "V <- N1.";
                VariableUpdateStatement statement = parser.Statement(rc, text, true, true) as VariableUpdateStatement;
                ContextGrabber grabber = new ContextGrabber();
                Assert.AreEqual(v, grabber.GetContext(0, statement));
                Assert.AreEqual(n1, grabber.GetContext(6, statement));
            }
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

            {
                //                   0         1  
                //                   012345678901
                const string text = "V <- f().S";
                VariableUpdateStatement statement = parser.Statement(rc, text, true, true) as VariableUpdateStatement;
                ContextGrabber grabber = new ContextGrabber();
                Assert.AreEqual(v, grabber.GetContext(0, statement));
                Assert.AreEqual(v, grabber.GetContext(1, statement));
                Assert.IsNull(grabber.GetContext(2, statement));
                Assert.IsNull(grabber.GetContext(3, statement));
                Assert.IsNull(grabber.GetContext(4, statement));
                Assert.AreEqual(function, grabber.GetContext(5, statement));
                Assert.AreEqual(function, grabber.GetContext(6, statement));
                Assert.AreEqual(function, grabber.GetContext(7, statement));
                Assert.AreEqual(s1, grabber.GetContext(8, statement));
                Assert.IsNull(grabber.GetContext(9, statement));
                Assert.IsNull(grabber.GetContext(10, statement));
            }

            {
                //                   0           
                //                   0123456789
                const string text = "V <- f().";
                VariableUpdateStatement statement = parser.Statement(rc, text, true, true) as VariableUpdateStatement;
                ContextGrabber grabber = new ContextGrabber();
                Assert.AreEqual(v, grabber.GetContext(0, statement));
                Assert.AreEqual(v, grabber.GetContext(1, statement));
                Assert.IsNull(grabber.GetContext(2, statement));
                Assert.IsNull(grabber.GetContext(3, statement));
                Assert.IsNull(grabber.GetContext(4, statement));
                Assert.AreEqual(function, grabber.GetContext(5, statement));
                Assert.AreEqual(function, grabber.GetContext(6, statement));
                Assert.AreEqual(function, grabber.GetContext(7, statement));
                Assert.AreEqual(s1, grabber.GetContext(8, statement));
                Assert.IsNull(grabber.GetContext(9, statement));
            }
        }

        [Test]
        public void TestApplyStatement()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Function function = CreateFunction(n1, "f", "S1");

            Collection collection = CreateCollection(n1, "Col", "S1", 10);
            Variable v = CreateVariable(n1, "V", "Col");

            Compiler.Compile_Synchronous(true, true);

            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Parser parser = new Parser();

            {
                //                   0         1         2
                //                   012345678901234567890123
                const string text = "APPLY X <- X ON V | X.";
                ApplyStatement statement = parser.Statement(rc, text, true, true) as ApplyStatement;
                Assert.IsNotNull(statement);
                ContextGrabber grabber = new ContextGrabber();
                Assert.IsNull(grabber.GetContext(0, statement));
                Assert.IsNull(grabber.GetContext(1, statement));
                Assert.IsNull(grabber.GetContext(2, statement));
                Assert.IsNull(grabber.GetContext(3, statement));
                Assert.IsNull(grabber.GetContext(4, statement));
                Assert.IsNull(grabber.GetContext(5, statement));
                Assert.AreEqual(statement.IteratorVariable, grabber.GetContext(6, statement));
                Assert.AreEqual(statement.IteratorVariable, grabber.GetContext(7, statement));
                Assert.IsNull(grabber.GetContext(8, statement));
                Assert.IsNull(grabber.GetContext(9, statement));
                Assert.IsNull(grabber.GetContext(10, statement));
                Assert.AreEqual(statement.IteratorVariable, grabber.GetContext(11, statement));
                Assert.AreEqual(statement.IteratorVariable, grabber.GetContext(12, statement));
                Assert.IsNull(grabber.GetContext(13, statement));
                Assert.IsNull(grabber.GetContext(14, statement));
                Assert.IsNull(grabber.GetContext(15, statement));
                Assert.AreEqual(v, grabber.GetContext(16, statement));
                Assert.AreEqual(v, grabber.GetContext(17, statement));
                Assert.IsNull(grabber.GetContext(18, statement));
                Assert.IsNull(grabber.GetContext(19, statement));
                Assert.AreEqual(statement.IteratorVariable, grabber.GetContext(20, statement));
                Assert.AreEqual(statement.IteratorVariable, grabber.GetContext(21, statement));
                Assert.IsNull(grabber.GetContext(22, statement));
            }
        }

        [Test]
        public void TestMapExpression()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Function function = CreateFunction(n1, "f", "S1");

            Collection collection = CreateCollection(n1, "Col", "S1", 10);
            Variable v = CreateVariable(n1, "V", "Col");

            Compiler.Compile_Synchronous(true, true);

            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Parser parser = new Parser();

            {
                //                   0         1         2
                //                   012345678901234567890123456
                const string text = "MAP V | X. USING X IN X.E1";
                MapExpression expression = parser.Expression(rc, text, null, true, null, true, true) as MapExpression;
                Assert.IsNotNull(expression);
                ContextGrabber grabber = new ContextGrabber();
                Assert.IsNull(grabber.GetContext(0, expression));
                Assert.IsNull(grabber.GetContext(1, expression));
                Assert.IsNull(grabber.GetContext(2, expression));
                Assert.IsNull(grabber.GetContext(3, expression));
                Assert.AreEqual(v, grabber.GetContext(4, expression));
                Assert.AreEqual(v, grabber.GetContext(5, expression));
                Assert.IsNull(grabber.GetContext(6, expression));
                Assert.IsNull(grabber.GetContext(7, expression));
                Assert.AreEqual(expression.IteratorVariable, grabber.GetContext(8, expression));
                Assert.AreEqual(expression.IteratorVariable, grabber.GetContext(9, expression));
                Assert.IsNull(grabber.GetContext(10, expression));
                Assert.IsNull(grabber.GetContext(11, expression));
                Assert.IsNull(grabber.GetContext(12, expression));
                Assert.IsNull(grabber.GetContext(13, expression));
                Assert.IsNull(grabber.GetContext(14, expression));
                Assert.IsNull(grabber.GetContext(15, expression));
                Assert.IsNull(grabber.GetContext(16, expression));
                Assert.IsNull(grabber.GetContext(17, expression));
                Assert.IsNull(grabber.GetContext(18, expression));
                Assert.IsNull(grabber.GetContext(19, expression));
                Assert.IsNull(grabber.GetContext(20, expression));
                Assert.IsNull(grabber.GetContext(21, expression));
                Assert.AreEqual(expression.IteratorVariable, grabber.GetContext(22, expression));
                Assert.AreEqual(expression.IteratorVariable, grabber.GetContext(23, expression));
                Assert.AreEqual(el1, grabber.GetContext(24, expression));
                Assert.AreEqual(el1, grabber.GetContext(25, expression));
                Assert.AreEqual(el1, grabber.GetContext(26, expression));
                Assert.IsNull(grabber.GetContext(27, expression));
            }
        }

        [Test]
        public void TestListExpression()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Function function = CreateFunction(n1, "f", "S1");

            Collection collection = CreateCollection(n1, "Col", "S1", 10);
            Variable v = CreateVariable(n1, "V", "Col");

            Compiler.Compile_Synchronous(true, true);

            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Parser parser = new Parser();

            {
                //                   0         1         2
                //                   012345678901234567890123456
                const string text = "V <- [S1 { E1 => Tr";
                VariableUpdateStatement statement = parser.Statement(rc, text, true, true) as VariableUpdateStatement;
                ContextGrabber grabber = new ContextGrabber();
                Assert.AreEqual(v, grabber.GetContext(0, statement));
                Assert.AreEqual(v, grabber.GetContext(1, statement));
                Assert.IsNull(grabber.GetContext(2, statement));
                Assert.IsNull(grabber.GetContext(3, statement));
                Assert.IsNull(grabber.GetContext(4, statement));
                Assert.IsNull(grabber.GetContext(5, statement));
                Assert.AreEqual(s1, grabber.GetContext(6, statement));
                Assert.AreEqual(s1, grabber.GetContext(7, statement));
                Assert.AreEqual(s1, grabber.GetContext(8, statement));
                Assert.AreEqual(s1, grabber.GetContext(9, statement));
                Assert.AreEqual(s1, grabber.GetContext(10, statement));
                Assert.AreEqual(s1, grabber.GetContext(11, statement));
                Assert.AreEqual(s1, grabber.GetContext(12, statement));
                Assert.AreEqual(s1, grabber.GetContext(13, statement));
                Assert.AreEqual(s1, grabber.GetContext(14, statement));
                Assert.AreEqual(s1, grabber.GetContext(15, statement));
                Assert.AreEqual(s1, grabber.GetContext(16, statement));
                Assert.IsNull(grabber.GetContext(17, statement));
                Assert.IsNull(grabber.GetContext(18, statement));
            }
        }
    }
}
