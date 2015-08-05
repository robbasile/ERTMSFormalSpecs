using DataDictionary.Generated;
using DataDictionary.Interpreter;
using NUnit.Framework;
using Utils;
using Action = DataDictionary.Rules.Action;
using Case = DataDictionary.Functions.Case;
using Enum = DataDictionary.Types.Enum;
using EnumValue = DataDictionary.Constants.EnumValue;
using Expectation = DataDictionary.Tests.Expectation;
using Frame = DataDictionary.Tests.Frame;
using Function = DataDictionary.Functions.Function;
using NameSpace = DataDictionary.Types.NameSpace;
using PreCondition = DataDictionary.Rules.PreCondition;
using Procedure = DataDictionary.Functions.Procedure;
using Range = DataDictionary.Types.Range;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;
using Step = DataDictionary.Tests.Step;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;
using StructureRef = DataDictionary.Types.StructureRef;
using SubSequence = DataDictionary.Tests.SubSequence;
using SubStep = DataDictionary.Tests.SubStep;
using TestCase = DataDictionary.Tests.TestCase;
using Variable = DataDictionary.Variables.Variable;
using Visitor = DataDictionary.Generated.Visitor;

namespace DataDictionary.test
{
    public class BaseModelTest
    {
        /// <summary>
        ///     Initializes all tests
        /// </summary>
        [TestFixtureSetUp]
        public void Initialise()
        {
            acceptor.setFactory(new ObjectFactory());
        }

        /// <summary>
        ///     Clean up the environment after a test has been executed
        /// </summary>
        [TearDown]
        public void CleanUp()
        {
            System.Dictionaries.Clear();
            Utils.ModelElement.Errors.Clear();
        }

        #region Rule checking

        /// <summary>
        ///     Checks of there is an error message in the object hierarchy
        /// </summary>
        private class ErrorMessageVisitor : Visitor
        {
            /// <summary>
            ///     One error log message found in the model
            /// </summary>
            public ElementLog ErrorMessageFound { get; private set; }

            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                foreach (ElementLog log in obj.Messages)
                {
                    if (log.Level == ElementLog.LevelEnum.Error)
                    {
                        ErrorMessageFound = log;
                        break;
                    }
                }

                if (ErrorMessageFound == null)
                {
                    base.visit(obj, visitSubNodes);
                }
            }
        }

        /// <summary>
        ///     Provides an error message found in the model, or one of its sub elements, if any
        /// </summary>
        /// <param name="model"></param>
        protected ElementLog ErrorMessage(ModelElement model)
        {
            ErrorMessageVisitor visitor = new ErrorMessageVisitor();
            visitor.visit(model, true);
            return visitor.ErrorMessageFound;
        }

        /// <summary>
        ///     Checks that a specific message is present in the model element
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        protected bool HasMessage(ModelElement model, string message)
        {
            bool retVal = false;

            foreach (ElementLog log in model.Messages)
            {
                if (log.Log == message)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        #endregion

        #region Compiler

        /// <summary>
        ///     The EFS System to test
        /// </summary>
        protected EFSSystem System
        {
            get { return EFSSystem.INSTANCE; }
        }

        /// <summary>
        ///     The Compiler
        /// </summary>
        protected Compiler Compiler
        {
            get { return System.Compiler; }
        }

        /// <summary>
        ///     The Parser
        /// </summary>
        protected Parser Parser
        {
            get { return System.Parser; }
        }

        /// <summary>
        ///     The factory used to create elements
        /// </summary>
        protected Factory Factory
        {
            get { return acceptor.getFactory(); }
        }

        #endregion

        #region Refactoring

        /// <summary>
        ///     Performs a refactor
        /// </summary>
        /// <param name="namable"></param>
        /// <param name="newName"></param>
        protected void Refactor(INamable namable, string newName)
        {
            Compiler.Compile_Synchronous(true);
            Compiler.Refactor(namable as ModelElement, newName);
        }

        /// <summary>
        ///     Performs a refactor
        /// </summary>
        /// <param name="namable"></param>
        protected void RefactorAndRelocate(INamable namable)
        {
            Compiler.Compile_Synchronous(true);
            Compiler.RefactorAndRelocate(namable as ModelElement);
        }

        /// <summary>
        ///     Performs a refactor
        /// </summary>
        /// <param name="element"></param>
        /// <param name="target"></param>
        protected void MoveToNameSpace(ModelElement element, NameSpace target)
        {
            Compiler.Compile_Synchronous(true);

            target.AddModelElement(element);
            Compiler.RefactorAndRelocate(element);
        }

        /// <summary>
        ///     Performs a dictionary cleanup
        /// </summary>
        protected void CleanUpModel()
        {
            Compiler.CleanUpModel();
        }

        #endregion

        #region Object creation

        /// <summary>
        ///     Creates a dictionary in the system
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Dictionary CreateDictionary(string name)
        {
            Dictionary retVal = (Dictionary) Factory.createDictionary();

            System.AddDictionary(retVal);
            retVal.FilePath = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a namespace in the dictionary/namespace provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected NameSpace CreateNameSpace(ModelElement enclosing, string name)
        {
            NameSpace retVal = (NameSpace) Factory.createNameSpace();

            Dictionary dictionary = enclosing as Dictionary;
            NameSpace nameSpace = enclosing as NameSpace;
            if (dictionary != null)
            {
                dictionary.appendNameSpaces(retVal);
            }
            else if (nameSpace != null)
            {
                nameSpace.appendNameSpaces(retVal);
            }
            else
            {
                Assert.Fail();
            }
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates an enum in the namespace provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Enum CreateEnum(NameSpace enclosing, string name)
        {
            Enum retVal = (Enum) Factory.createEnum();

            enclosing.appendEnumerations(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates an range in the namespace provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <param name="precision"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        protected Range CreateRange(NameSpace enclosing, string name, acceptor.PrecisionEnum precision, string minValue,
            string maxValue)
        {
            Range retVal = (Range) Factory.createRange();

            enclosing.appendRanges(retVal);
            retVal.Name = name;
            retVal.setPrecision(precision);
            retVal.MinValue = minValue;
            retVal.MaxValue = maxValue;

            return retVal;
        }

        /// <summary>
        ///     Appends an enum value to the enum provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected EnumValue CreateEnumValue(Enum enclosing, string name)
        {
            EnumValue retVal = (EnumValue) Factory.createEnumValue();

            enclosing.appendValues(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a variable in the namespace provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected Variable CreateVariable(NameSpace enclosing, string name, string typeName)
        {
            Variable retVal = (Variable) Factory.createVariable();

            enclosing.appendVariables(retVal);
            retVal.Name = name;
            retVal.TypeName = typeName;
            retVal.Type = System.FindType(enclosing, typeName);

            return retVal;
        }

        /// <summary>
        ///     Creates a structure in the namespace provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Structure CreateStructure(NameSpace enclosing, string name)
        {
            Structure retVal = (Structure) Factory.createStructure();

            enclosing.appendStructures(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a structure in the namespace provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected StructureRef CreateStructureRef(Structure enclosing, string name)
        {
            StructureRef retVal = (StructureRef) Factory.createStructureRef();

            enclosing.appendInterfaces(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a state machine in the structure provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected StateMachine CreateStateMachine(Structure enclosing, string name)
        {
            StateMachine retVal = (StateMachine) Factory.createStateMachine();

            enclosing.appendStateMachines(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a state machine in the structure provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected StateMachine CreateStateMachine(NameSpace enclosing, string name)
        {
            StateMachine retVal = (StateMachine) Factory.createStateMachine();

            enclosing.appendStateMachines(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a state in the state machine provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected State CreateState(StateMachine enclosing, string name)
        {
            State retVal = (State) Factory.createState();

            enclosing.appendStates(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a structure element in the structure provided
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected StructureElement CreateStructureElement(Structure enclosing, string name, string type)
        {
            StructureElement retVal = (StructureElement) Factory.createStructureElement();

            enclosing.appendElements(retVal);
            retVal.Name = name;
            retVal.TypeName = type;

            return retVal;
        }

        /// <summary>
        ///     Creates a rule and a rule condition in the namespace
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected RuleCondition CreateRuleAndCondition(NameSpace enclosing, string name)
        {
            Rule rule = (Rule) Factory.createRule();
            enclosing.appendRules(rule);
            rule.Name = name;

            return CreateRuleCondition(rule, name);
        }

        /// <summary>
        ///     Creates a rule and a rule condition in the state machine
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected RuleCondition CreateRuleAndCondition(StateMachine enclosing, string name)
        {
            Rule rule = (Rule) Factory.createRule();
            enclosing.appendRules(rule);
            rule.Name = name;

            return CreateRuleCondition(rule, name);
        }

        /// <summary>
        ///     Creates a rule and a rule condition in the structure
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected RuleCondition CreateRuleAndCondition(Structure enclosing, string name)
        {
            Rule rule = (Rule) Factory.createRule();
            enclosing.appendRules(rule);
            rule.Name = name;

            return CreateRuleCondition(rule, name);
        }

        /// <summary>
        ///     Creates a rule and a rule condition in the procedure
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected RuleCondition CreateRuleAndCondition(Procedure enclosing, string name)
        {
            Rule rule = (Rule) Factory.createRule();
            enclosing.appendRules(rule);
            rule.Name = name;

            return CreateRuleCondition(rule, name);
        }

        /// <summary>
        ///     Creates a rule condition in a rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private RuleCondition CreateRuleCondition(Rule rule, string name)
        {
            RuleCondition retVal = (RuleCondition) Factory.createRuleCondition();
            rule.appendConditions(retVal);
            retVal.Name = name;
            return retVal;
        }

        /// <summary>
        ///     Creates a function in the enclosing namespace
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected Function CreateFunction(NameSpace enclosing, string name, string typeName)
        {
            Function retVal = (Function) Factory.createFunction();
            enclosing.appendFunctions(retVal);
            retVal.Name = name;
            retVal.TypeName = typeName;

            return retVal;
        }

        /// <summary>
        ///     Creates a parameter in the enclosing function
        /// </summary>
        /// <param name="function"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Parameter CreateParameter(Function function, string name, string type)
        {
            Parameter retVal = (Parameter) Factory.createParameter();
            function.appendParameters(retVal);
            retVal.Name = name;
            retVal.TypeName = type;

            return retVal;
        }

        /// <summary>
        ///     Creates a function in the enclosing namespace
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Procedure CreateProcedure(NameSpace enclosing, string name)
        {
            Procedure retVal = (Procedure) Factory.createProcedure();
            enclosing.appendProcedures(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a function in the enclosing structure
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Procedure CreateProcedure(Structure enclosing, string name)
        {
            Procedure retVal = (Procedure) Factory.createProcedure();
            enclosing.appendProcedures(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a parameter in the enclosing procedure
        /// </summary>
        /// <param name="procedure"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Parameter CreateParameter(Procedure procedure, string name, string type)
        {
            Parameter retVal = (Parameter) Factory.createParameter();
            procedure.appendParameters(retVal);
            retVal.Name = name;
            retVal.TypeName = type;

            return retVal;
        }

        /// <summary>
        ///     Creates a case in the enclosing function
        /// </summary>
        /// <param name="function"></param>
        /// <param name="name"></param>
        /// <param name="expression"></param>
        /// <param name="preConditionExpression"></param>
        /// <returns></returns>
        protected Case CreateCase(Function function, string name, string expression, string preConditionExpression = "")
        {
            Case retVal = (Case) Factory.createCase();
            function.appendCases(retVal);
            retVal.Name = name;
            retVal.ExpressionText = expression;

            if (preConditionExpression != "")
            {
                PreCondition preCondition = (PreCondition) Factory.createPreCondition();
                preCondition.ExpressionText = preConditionExpression;
                retVal.appendPreConditions(preCondition);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates an action in the enclosing rule condition
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected Action CreateAction(RuleCondition enclosing, string expression)
        {
            Action retVal = (Action) Factory.createAction();
            enclosing.appendActions(retVal);
            retVal.ExpressionText = expression;

            return retVal;
        }

        /// <summary>
        ///     Creates a precondition in the enclosing rule condition
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected PreCondition CreatePreCondition(RuleCondition enclosing, string expression)
        {
            PreCondition retVal = (PreCondition) Factory.createPreCondition();
            enclosing.appendPreConditions(retVal);
            retVal.ExpressionText = expression;

            return retVal;
        }

        /// <summary>
        ///     Creates a precondition in the enclosing function case
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected PreCondition CreatePreCondition(Case enclosing, string name)
        {
            PreCondition retVal = (PreCondition) Factory.createPreCondition();
            enclosing.appendPreConditions(retVal);
            retVal.ExpressionText = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a test frame in the enclosing dictionary
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Frame CreateTestFrame(Dictionary enclosing, string name)
        {
            Frame retVal = (Frame) Factory.createFrame();
            enclosing.appendTests(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a test sub sequence in the enclosing test frame
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected SubSequence CreateSubSequence(Frame enclosing, string name)
        {
            SubSequence retVal = (SubSequence) Factory.createSubSequence();
            enclosing.appendSubSequences(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a test case in the enclosing test sub sequence
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected TestCase CreateTestCase(SubSequence enclosing, string name)
        {
            TestCase retVal = (TestCase) Factory.createTestCase();
            enclosing.appendTestCases(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a step in a test case
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Step CreateStep(TestCase enclosing, string name)
        {
            Step retVal = (Step) Factory.createStep();
            enclosing.appendSteps(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates a step in a test case
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected SubStep CreateSubStep(Step enclosing, string name)
        {
            SubStep retVal = (SubStep) Factory.createSubStep();
            enclosing.appendSubSteps(retVal);
            retVal.Name = name;

            return retVal;
        }

        /// <summary>
        ///     Creates an expectation in a sub step
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Expectation CreateExpectation(SubStep enclosing, string name)
        {
            Expectation retVal = (Expectation) Factory.createExpectation();
            enclosing.appendExpectations(retVal);
            retVal.ExpressionText = name;

            return retVal;
        }

        #endregion
    }
}