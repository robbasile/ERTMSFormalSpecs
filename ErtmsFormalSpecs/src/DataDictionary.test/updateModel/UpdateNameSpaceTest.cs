using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Values;
using NUnit.Framework;

namespace DataDictionary.test.updateModel
{
    [TestFixture]
    public class UpdateNameSpaceTest : BaseModelTest
    {
        [Test]
        public void TestUpdateNameSpace()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "False");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            NameSpace updatedNameSpace = nameSpace.CreateUpdateInDictionary(dictionary2);
            Function updatedFunction = CreateFunction(nameSpace, "f", "Bool");
            Case updCase1 = CreateCase(updatedFunction, "Case 1", "True");
            updatedFunction.setUpdates(function.Guid);

            Compiler.Compile_Synchronous(true);

            Expression expression = Parser.Expression(dictionary, "N1.f()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.True, value);
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


            Expression expression = Parser.Expression(dictionary, "N1.f()");
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

            Expression expression = Parser.Expression(dictionary, "N1.q()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);

            Expression expression2 = Parser.Expression(dictionary, "N1.f()");
            IValue value2 = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value2);
        }

        [Test]
        public void TestUpdateNestedNamespaces()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            NameSpace subNameSpace = CreateNameSpace(nameSpace, "N2");
            NameSpace subSubNameSpace = CreateNameSpace(subNameSpace, "N3");
            NameSpace usedNameSpace = CreateNameSpace(subSubNameSpace, "N4");

            Function function = CreateFunction(usedNameSpace, "f", "Bool");
            Case cas = CreateCase(function, "Case 1", "True");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas2 = (Case) updatedFunction.Cases[0];
            cas2.ExpressionText = "False";


            Compiler.Compile_Synchronous(true);


            Expression expression = Parser.Expression(dictionary, "N1.N2.N3.N4.f()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }


        [Test]
        public void TestUpdateBranchingNameSpaces()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N");
            NameSpace nameSpace1 = CreateNameSpace(nameSpace, "N1");
            NameSpace nameSpace2 = CreateNameSpace(nameSpace, "N2");

            Function function1 = CreateFunction(nameSpace1, "f", "Bool");
            Case cas1 = CreateCase(function1, "Case 1", "N2.q()");

            Function function2 = CreateFunction(nameSpace2, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updateFunction1 = function1.CreateFunctionUpdate(dictionary2);
            updateFunction1.TypeName = "Integer";
            Case updcas1 = (Case) updateFunction1.Cases[0];
            updcas1.ExpressionText = "2";
            PreCondition precond = CreatePreCondition(updcas1, "N2.q()");
            Case newCase = CreateCase(updateFunction1, "Case 2", "1");

            Function updateFunction2 = function2.CreateFunctionUpdate(dictionary2);
            ((Case) updateFunction2.Cases[0]).ExpressionText = "False";


            Compiler.Compile_Synchronous(true);


            Expression expression = Parser.Expression(dictionary, "N.N1.f()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            IValue refVal = new IntValue(System.IntegerType, 1);
            Assert.AreEqual(refVal.LiteralName, value.LiteralName);
        }


        [Test]
        public void TestUpdateTargetBranchingNameSpace()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N");
            NameSpace nameSpace1 = CreateNameSpace(nameSpace, "N1");
            NameSpace nameSpace2 = CreateNameSpace(nameSpace, "N2");

            Function function1 = CreateFunction(nameSpace1, "f", "Bool");
            Case cas1 = CreateCase(function1, "Case 1", "N2.q()");

            Function function2 = CreateFunction(nameSpace2, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updateFunction2 = function2.CreateFunctionUpdate(dictionary2);
            ((Case) updateFunction2.Cases[0]).ExpressionText = "False";


            Compiler.Compile_Synchronous(true);


            Expression expression = Parser.Expression(dictionary, "N.N1.f()");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }
    }
}