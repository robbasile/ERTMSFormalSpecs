using DataDictionary.Interpreter;
using DataDictionary.Tests.Runner;
using DataDictionary.Values;
using NUnit.Framework;
using DataDictionary.Constants;
using DataDictionary.Functions;
using DataDictionary.Rules;
using DataDictionary.Tests;
using DataDictionary.Types;
using DataDictionary.Variables;

namespace DataDictionary.test
{
    [TestFixture]
    public class FunctionCacheTest : BaseModelTest
    {
        /// <summary>
        ///     Tests that a function F2 that depends on a function F1 is uncached when the return value of F1 changes
        /// </summary>
        [Test]
        public void TestUncacheFunctionOfFunction()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");

            Variable v = CreateVariable(n1, "Var", "Boolean");
            v.setDefaultValue("True");

            Function F1 = CreateFunction(n1, "FunOfVar", "Integer");
            Case cas1_1 = CreateCase(F1, "Var is true", "1", "Var");
            Case cas1_2 = CreateCase(F1, "Var is false", "2");

            Function F2 = CreateFunction(n1, "FunOfFun", "Boolean");
            Case cas2_1 = CreateCase(F2, "Value", "FunOfVar() == 2");

            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Action a = CreateAction(rc, "Var <- False");

            Compiler.Compile_Synchronous(true);

            Expression expression = new Parser().Expression(test, "N1.FunOfFun()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);

            Assert.AreEqual(value, System.BoolType.False);


            // ReSharper disable once UseObjectOrCollectionInitializer
            Runner runner = new Runner(false);
            runner.CheckForCompatibleChanges = true;
            runner.Cycle();

            value = expression.GetExpressionValue(new InterpretationContext(), null);

            Assert.AreEqual(value, System.BoolType.True);
        }

        /// <summary>
        ///     Tests that a function F2 that depends on a function F1 is uncached when the return value of F1 changes
        ///     In this case, the value of F1 depends on a structure that updates itself
        /// </summary>
        [Test]
        public void TestUncacheFunctionOfStructure()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");

            Structure structure = CreateStructure(n1, "Structure");
            StructureElement structElement = CreateStructureElement(structure, "Elem", "Boolean");
            structElement.setDefault("True");

            RuleCondition updElem = CreateRuleAndCondition(structure, "Update Elem");
            Action a = CreateAction(updElem, "Elem <- False");

            Variable v = CreateVariable(n1, "Var", "Structure");

            Function F1 = CreateFunction(n1, "FunOfVar", "Integer");
            Case cas1_1 = CreateCase(F1, "Var.Elem is true", "1", "Var.Elem");
            Case cas1_2 = CreateCase(F1, "Var.Elem is false", "2");

            Function F2 = CreateFunction(n1, "FunOfFun", "Boolean");
            Case cas2_1 = CreateCase(F2, "Value", "FunOfVar() == 2");

            Compiler.Compile_Synchronous(true);

            Expression expression = new Parser().Expression(test, "N1.FunOfFun()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);

            Assert.AreEqual(value, System.BoolType.False);


            // ReSharper disable once UseObjectOrCollectionInitializer
            Runner runner = new Runner(false);
            runner.CheckForCompatibleChanges = true;
            runner.Cycle();

            value = expression.GetExpressionValue(new InterpretationContext(), null);

            Assert.AreEqual(value, System.BoolType.True);
        }
    }
}
