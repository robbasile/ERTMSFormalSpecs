using DataDictionary.Constants;
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.RuleCheck;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Values;
using NUnit.Framework;

namespace DataDictionary.test.updateModel
{
    [TestFixture]
    public class UpdateFunctionTest : BaseModelTest
    {
        /// <summary>
        ///     Tests that a call to an updated function is evaluated correctly
        /// </summary>
        [Test]
        public void TestUpdateFunction()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case) updatedFunction.Cases[0];
            cas3.ExpressionText = "NOT q()";

            Compiler.Compile_Synchronous(true);

            Expression expression = new Parser().Expression(dictionary, "N1.f()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }

        [Test]
        public void TestUpdateMultipleFunctions()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case) updatedFunction.Cases[0];
            cas3.ExpressionText = "NOT q(param => 3)";

            Function updatedFunction2 = function2.CreateFunctionUpdate(dictionary2);
            Parameter intParameter = CreateParameter(updatedFunction2, "param", "Integer");
            Case cas4 = (Case) updatedFunction2.Cases[0];
            cas4.ExpressionText = "False";

            Compiler.Compile_Synchronous(true);

            Expression expression = new Parser().Expression(dictionary, "N1.f()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.True, value);
        }


        [Test]
        public void TestUpdateCalledFunction()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function2.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case) updatedFunction.Cases[0];
            cas3.ExpressionText = "False";

            Compiler.Compile_Synchronous(true);

            Expression expression = new Parser().Expression(dictionary, "N1.f()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }

        [Test]
        public void TestParameterTypeReference()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            Enum enumeration = CreateEnum(nameSpace, "Enum");
            EnumValue value1 = CreateEnumValue(enumeration, "First");

            Function function = CreateFunction(nameSpace, "f", "Bool");

            Parameter param = new Parameter();
            param.setTypeName("N1.Enum");
            param.setName("Value");

            function.appendParameters(param);
            Case cas1 = CreateCase(function, "Case 1", "True", "Value == Enum.First");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case) updatedFunction.Cases[0];
            cas3.ExpressionText = "False";

            Compiler.Compile_Synchronous(true);

            Expression expression = new Parser().Expression(dictionary, "N1.f(N1.Enum.First)");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }

        [Test]
        public void TestParameterTypeInDefaultNameSpace()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace defaultNameSpace = CreateNameSpace(dictionary, "Default");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            Enum enumeration = CreateEnum(defaultNameSpace, "Enum");
            EnumValue value1 = CreateEnumValue(enumeration, "First");
            EnumValue value2 = CreateEnumValue(enumeration, "Second");

            Function function = CreateFunction(nameSpace, "F1", "Boolean");

            Parameter param = new Parameter();
            param.setTypeName("Enum");
            param.setName("Value");
            function.appendParameters(param);

            Case cas1 = CreateCase(function, "Case 1", "True", "Value == Enum.First");
            Case cas2 = CreateCase(function, "Case 2", "False");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case) updatedFunction.Cases[0];
            PreCondition preCondition = (PreCondition) cas3.PreConditions[0];
            preCondition.ExpressionText = "Value == Enum.Second";

            Compiler.Compile_Synchronous(true);

            RuleCheckerVisitor ruleChecker = new RuleCheckerVisitor(dictionary2);
            ruleChecker.visit(updatedFunction);
            Assert.IsNull(ErrorMessage(updatedFunction));

            Expression expression = new Parser().Expression(dictionary, "N1.F1(Enum.Second)");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.True, value);
        }

        [Test]
        public void TestParameterTypeName_RelativePath()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            NameSpace subNameSpace = CreateNameSpace(nameSpace, "N2");

            Enum enumeration = CreateEnum(subNameSpace, "Enum");
            EnumValue value1 = CreateEnumValue(enumeration, "First");
            EnumValue value2 = CreateEnumValue(enumeration, "Second");

            Function function = CreateFunction(nameSpace, "F1", "Boolean");

            Parameter param = new Parameter();
            param.setTypeName("N2.Enum");
            param.setName("Value");
            function.appendParameters(param);

            Case cas1 = CreateCase(function, "Case 1", "True", "Value == N2.Enum.First");
            Case cas2 = CreateCase(function, "Case 2", "False");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case) updatedFunction.Cases[0];
            PreCondition preCondition = (PreCondition) cas3.PreConditions[0];
            preCondition.ExpressionText = "Value == N2.Enum.Second";

            Compiler.Compile_Synchronous(true);

            RuleCheckerVisitor ruleChecker = new RuleCheckerVisitor(dictionary2);
            ruleChecker.visit(updatedFunction);
            Assert.IsNull(ErrorMessage(updatedFunction));

            Expression expression = new Parser().Expression(dictionary, "N1.F1(N1.N2.Enum.Second)");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.True, value);
        }

        [Test]
        public void TestUpdateRemoved()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            updatedFunction.setIsRemoved(true);

            Compiler.Compile_Synchronous(true);

            Expression expression = new Parser().Expression(dictionary, "N1.f()");

            Assert.AreEqual(Utils.ModelElement.Errors.Count, 1);
        }
    }
}