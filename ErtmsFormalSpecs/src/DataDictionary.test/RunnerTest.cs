using DataDictionary.Generated;
using DataDictionary.Tests.Runner;
using DataDictionary.Types;
using DataDictionary.Values;
using NUnit.Framework;
using Action = DataDictionary.Rules.Action;
using Collection = DataDictionary.Types.Collection;
using NameSpace = DataDictionary.Types.NameSpace;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.test
{
    // ReSharper disable UnusedVariable
    [TestFixture]
    public class RunnerTest : BaseModelTest
    {
        [Test]
        public void TestRefactorStructureName()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Collection collection = CreateCollection(n1, "Col", "Integer", 10);
            Variable v = CreateVariable(n1, "V", "Col");
            v.setDefaultValue("[]");
            RuleCondition rc1 = CreateRuleAndCondition(n1, "Rule1");
            Action a1 = CreateAction(rc1, "INSERT 1 IN V");
            RuleCondition rc2 = CreateRuleAndCondition(n1, "Rule2");
            Action a2 = CreateAction(rc2, "INSERT 2 IN V");

            // ReSharper disable once UseObjectOrCollectionInitializer
            Runner runner = new Runner(false);
            runner.CheckForCompatibleChanges = true;
            runner.Cycle();

            ListValue listValue = v.Value as ListValue;
            Assert.IsNotNull(listValue);
            Assert.AreEqual(2, listValue.Val.Count);
            Assert.AreEqual(0, a1.Messages.Count);
            Assert.AreEqual(0, a2.Messages.Count);
        }

        /// <summary>
        /// When a subrule S2 has a priority which differs from its enclosing rule S1, 
        /// S2 shall be evaluated at its own priority when preconditions of S1 are satisfied at S2 priority
        /// </summary>
        [Test]
        public void TestSubRulesPriority()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Variable v1 = CreateVariable(n1, "V", "Integer");
            v1.setDefaultValue("0");
            Variable v2 = CreateVariable(n1, "V2", "Integer");
            v2.setDefaultValue("0");
            // Priority is Processing
            RuleCondition rc1 = CreateRuleAndCondition(n1, "Rule1");
            Action a1 = CreateAction(rc1, "V2 <- V2 + 1");
            // Priority is Update out
            RuleCondition rc2 = CreateRuleAndCondition(rc1, "Rule2");
            rc2.EnclosingRule.setPriority(acceptor.RulePriority.aUpdateOUT);
            Action a2 = CreateAction(rc2, "V <- V + 1");

            // ReSharper disable once UseObjectOrCollectionInitializer
            Runner runner = new Runner(false);
            runner.Cycle();

            IntValue value = v1.Value as IntValue;
            Assert.IsNotNull(value);
            Assert.AreEqual(1, value.Val);

            value = v2.Value as IntValue;
            Assert.IsNotNull(value);
            Assert.AreEqual(1, value.Val);
        }
    }

    // ReSharper restore UnusedVariable
}
