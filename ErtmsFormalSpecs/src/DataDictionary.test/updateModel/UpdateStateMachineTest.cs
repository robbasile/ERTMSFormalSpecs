using DataDictionary.Constants;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using NUnit.Framework;

namespace DataDictionary.test.updateModel
{
    [TestFixture]
    internal class UpdateStateMachineTest : BaseModelTest
    {
        [Test]
        public void TestUpdateStateMachine()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            StateMachine stateMachine = CreateStateMachine(nameSpace, "SM");
            State state = CreateState(stateMachine, "State1");
            stateMachine.Default = "State1";

            Variable variable = CreateVariable(nameSpace, "Variable", "SM");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            StateMachine stateMachineUpdate = stateMachine.CreateStateMachineUpdate(dictionary2);
            State state2 = CreateState(stateMachineUpdate, "State2");
            stateMachineUpdate.Default = "State2";

            Compiler.Compile_Synchronous(true);

            Expression expression = new Parser().Expression(dictionary, "N1.Variable");
            IValue value = expression.GetExpressionValue(new InterpretationContext(), null);
            Assert.AreEqual(value, state);
        }
    }
}