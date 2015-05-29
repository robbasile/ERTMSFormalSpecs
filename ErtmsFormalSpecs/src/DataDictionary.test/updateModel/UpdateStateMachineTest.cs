using DataDictionary.Constants;
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using NUnit.Framework;

namespace DataDictionary.test.updateModel
{
    [TestFixture]
    internal class UpdateStateMachineTest : BaseModelTest
    {
        /// <summary>
        ///     Tests that a variable of type State Machine is correctly updated
        /// </summary>
        [Test]
        public void TestUpdateStateMachine()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            StateMachine stateMachine = CreateStateMachine(nameSpace, "TestSM");
            State state1 = CreateState(stateMachine, "S1");
            State state2 = CreateState(stateMachine, "S2");
            stateMachine.Default = "S1";

            Variable var = CreateVariable(nameSpace, "TheVariable", "TestSM");
            var.setDefaultValue("TestSM.S1");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            StateMachine updStateMachine = stateMachine.CreateStateMachineUpdate(dictionary2);

            State updState = updStateMachine.FindState("S1");
            State subState = CreateState(updState.StateMachine, "bis");
            updState.StateMachine.setDefault("bis");

            Compiler.Compile_Synchronous(true);

            Expression expression = Parser.Expression(dictionary, "N1.TheVariable");
            IValue value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(value, subState);
        }

        /// <summary>
        ///     Tests that the type of a State Machine variable is correctly updated
        /// </summary>
        [Test]
        public void TestUpdateStateMachineType()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            StateMachine stateMachine = CreateStateMachine(nameSpace, "TestSM");
            State state1 = CreateState(stateMachine, "S1");
            State state2 = CreateState(stateMachine, "S2");
            stateMachine.Default = "S1";

            Variable var = CreateVariable(nameSpace, "TheVariable", "TestSM");
            var.setDefaultValue("TestSM.S1");

            RuleCondition condition = CreateRuleAndCondition(nameSpace, "Test");
            PreCondition preCondition = CreatePreCondition(condition, "N1.TheVariable in [N1.TestSM.S1, N1.TestSM.S2]");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            StateMachine updStateMachine = stateMachine.CreateStateMachineUpdate(dictionary2);

            State updState = updStateMachine.FindState("S1");
            State subState = CreateState(updState.StateMachine, "bis");
            updState.StateMachine.setDefault("bis");

            dictionary.CheckRules();

            Assert.AreEqual(0, ModelElement.Errors.Count);
        }
    }
}