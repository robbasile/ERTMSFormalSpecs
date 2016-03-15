using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.RuleCheck;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Variables;
using NUnit.Framework;

namespace DataDictionary.test
{
    [TestFixture]
    public class RuleCheckerTest : BaseModelTest
    {
        /// <summary>
        ///     Tests that the Max and Min values for ranges have the right precision
        /// </summary>
        [Test]
        public void TestMaxMinValuesInRange()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "NameSpace");

            Range r1 = CreateRange(nameSpace, "r1", Generated.acceptor.PrecisionEnum.aIntegerPrecision, "0.5", "100");
            Range r2 = CreateRange(nameSpace, "r2", Generated.acceptor.PrecisionEnum.aDoublePrecision, "0.0", "100");
            Range r3 = CreateRange(nameSpace, "r3", Generated.acceptor.PrecisionEnum.aIntegerPrecision, "A", "100.0");

            RuleCheckerVisitor visitor = new RuleCheckerVisitor(dictionary);
            visitor.visit(nameSpace);

            Assert.True(HasMessage(r1, "Type08: Invalid min value for integer range : must be an integer"));
            Assert.False(HasMessage(r1, "Type08: Invalid max value for integer range : must be an integer"));

            Assert.False(HasMessage(r2, "Type09: Invalid min value for float range : must have a decimal part"));
            Assert.True(HasMessage(r2, "Type09: Invalid max value for float range : must have a decimal part"));

            Assert.True(HasMessage(r3, "Type10: Cannot parse min value for range"));
            Assert.False(HasMessage(r3, "Type10: Cannot parse max value for range"));
        }

        /// <summary>
        ///     Tests that the dereferencing of a structure is done on an instance
        /// </summary>
        [Test]
        public void TestStructureDereference()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Structure structure = CreateStructure (nameSpace, "Struct");
            StructureElement el1 = CreateStructureElement (structure, "e1", "Boolean");
            Function f = CreateFunction (nameSpace, "f", "Struct");
            Variable v = CreateVariable (nameSpace, "v", "Struct");
            Variable v2 = CreateVariable(nameSpace, "v2", "Boolean");

            Parser parser = new Parser ();
            {
                DerefExpression expression = parser.Expression (nameSpace, "N1.v.e1") as DerefExpression;
                Assert.IsNotNull (expression);
                Assert.AreEqual(3, expression.Arguments.Count);
                Assert.AreEqual(v, expression.Arguments[1].Ref);
                Assert.AreEqual (el1, expression.Arguments [2].Ref);
            }
            {
                DerefExpression expression = parser.Expression (nameSpace, "N1.f().e1") as DerefExpression;
                Assert.IsNotNull (expression);
                Assert.AreEqual (2, expression.Arguments.Count);
                Assert.AreEqual (structure, expression.Arguments [0].Ref);
                Assert.AreEqual (el1, expression.Arguments [1].Ref);
            }
            {
                DerefExpression expression = parser.Expression(nameSpace, "N1.Struct.e1") as DerefExpression;
                Assert.IsNotNull(expression);
                Assert.AreEqual(3, expression.Arguments.Count);
                Assert.AreEqual(structure, expression.Arguments[1].Ref);
                Assert.AreEqual(el1, expression.Arguments[2].Ref);
            }

            RuleCondition condition = CreateRuleAndCondition (nameSpace, "Test");
            Action action1 = CreateAction (condition, "Struct.e1 <- True");
            Action action2 = CreateAction(condition, "v2 <- Struct.e1");
            Action action3 = CreateAction(condition, "f().e1 <- True");
            Action action4 = CreateAction(condition, "v2 <- f().e1");

            Collection collection = CreateCollection (nameSpace, "Col", "Struct", 10);
            Variable v3 = CreateVariable (nameSpace, "v3", "Col");
            Action action5 = CreateAction (condition, "(FIRST X IN v3).e1 <- True");
            
            RuleCondition ruleCondition2 = CreateRuleAndCondition(structure, "Rule");
            Action action6 = CreateAction (ruleCondition2, "THIS.e1 <- True");
            Action action7 = CreateAction(ruleCondition2, "v2 <- THIS.e1");

            RuleCheckerVisitor visitor = new RuleCheckerVisitor(dictionary);
            visitor.visit(nameSpace);

            Assert.True(HasMessagePart(action1, "structure should not be used to reference an instance"));
            Assert.True(HasMessagePart(action2, "structure should not be used to reference an instance"));
            Assert.False(HasMessagePart(action3, "structure should not be used to reference an instance"));
            Assert.False(HasMessagePart(action4, "structure should not be used to reference an instance"));
            Assert.False(HasMessagePart(action5, "structure should not be used to reference an instance"));
            Assert.False(HasMessagePart(action6, "structure should not be used to reference an instance"));
            Assert.False(HasMessagePart(action7, "structure should not be used to reference an instance"));
        }
    }
}