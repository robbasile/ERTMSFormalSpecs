using System.Collections.Generic;
using DataDictionary.Constants;
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Rules;
using DataDictionary.Tests.Runner;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using NUnit.Framework;

namespace DataDictionary.test.updateModel
{
    [TestFixture]
    class UpdateProcedureTests : BaseModelTest
    {
        [Test]
        public void TestUpdateRemoved()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Variable var = CreateVariable(nameSpace, "Variable", "Integer");

            Procedure procedure = CreateProcedure(nameSpace, "Procedure");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Procedure updatedProcedure = procedure.CreateProcedureUpdate(dictionary2);
            updatedProcedure.setIsRemoved(true);

            Compiler.Compile_Synchronous(true);

            Expression expression = Parser.Expression(dictionary, "N1.Procedure()");

            Assert.AreEqual(Utils.ModelElement.Errors.Count, 1);
        }
    }
}
