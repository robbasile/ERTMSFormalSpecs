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
    public class InstanceGrabberTest : BaseModelTest
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
                VariableUpdateStatement statement =
                    parser.Statement(rc, "V <- N1.S", true, true) as VariableUpdateStatement;
                Assert.IsNotNull(statement);
                Assert.AreEqual(statement.VariableIdentification.Ref, v);

                DerefExpression deref = statement.Expression as DerefExpression;
                Assert.IsNotNull(deref);
                Assert.AreEqual(deref.Arguments[0].Ref, n1);
            }

            {
                VariableUpdateStatement statement = parser.Statement(rc, "V <- N1.", true, true) as VariableUpdateStatement;
                Assert.IsNotNull(statement);
                Assert.AreEqual(statement.VariableIdentification.Ref, v);

                DerefExpression deref = statement.Expression as DerefExpression;
                Assert.IsNotNull(deref);
                Assert.AreEqual(deref.Arguments[0].Ref, n1);
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
                VariableUpdateStatement statement =
                    parser.Statement(rc, "V <- f().S", true, true) as VariableUpdateStatement;
                Assert.IsNotNull(statement);
                Assert.AreEqual(statement.VariableIdentification.Ref, v);

                DerefExpression deref = statement.Expression as DerefExpression;
                Assert.IsNotNull(deref);
                Assert.AreEqual(deref.Arguments[0].Ref, s1);
            }

            {
                VariableUpdateStatement statement = parser.Statement(rc, "V <- f().", true, true) as VariableUpdateStatement;
                Assert.IsNotNull(statement);
                Assert.AreEqual(statement.VariableIdentification.Ref, v);

                DerefExpression deref = statement.Expression as DerefExpression;
                Assert.IsNotNull(deref);
                Assert.AreEqual(deref.Arguments[0].Ref, s1);
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
                ApplyStatement statement = parser.Statement(rc, "APPLY X <- X", true, true) as ApplyStatement;
                Assert.IsNotNull(statement);
            }

            {
                ApplyStatement statement = parser.Statement(rc, "APPLY X <- X ON V | X.", true, true) as ApplyStatement;
                Assert.IsNotNull(statement);

                DerefExpression deref = statement.ConditionExpression as DerefExpression;
                Assert.IsNotNull(deref);

                ITypedElement element = deref.Arguments[0].Ref as ITypedElement;
                Assert.IsNotNull(element);
                Assert.AreEqual(element.Type, s1);
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
                MapExpression expression = parser.Expression(rc, "MAP V | X. USING X IN X.E2", null, true, null, true, true) as MapExpression;
                Assert.IsNotNull(expression);

                DerefExpression deref = expression.Condition as DerefExpression;
                Assert.IsNotNull(deref);

                ITypedElement element = deref.Arguments[0].Ref as ITypedElement;
                Assert.IsNotNull(element);
                Assert.AreEqual(element.Type, s1);
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
                VariableUpdateStatement statement = parser.Statement(rc, "V <- [S1 { E1 => Tr", true, true) as VariableUpdateStatement;
                Assert.IsNotNull(statement);

                UnaryExpression unaryExpression = statement.Expression as UnaryExpression;
                Assert.IsNotNull(unaryExpression);
                ListExpression listExpression = unaryExpression.Term.LiteralValue as ListExpression;
                Assert.IsNotNull(listExpression);
                Assert.AreEqual(listExpression.ListElements.Count, 1);

                StructExpression structExpression = listExpression.ListElements[0] as StructExpression;
                Assert.IsNotNull(structExpression);
            }
        }
    }
}
