using DataDictionary.Rules;
using DataDictionary.Tests.Runner;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using NUnit.Framework;

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
            runner.Cycle();

            ListValue listValue = v.Value as ListValue;
            Assert.IsNotNull(listValue);
            Assert.AreEqual(2, listValue.Val.Count);
        }
    }
    // ReSharper restore UnusedVariable
}
