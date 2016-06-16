using DataDictionary.Constants;
using DataDictionary.Functions;
using DataDictionary.Rules;
using DataDictionary.Tests;
using DataDictionary.Types;
using DataDictionary.Variables;
using NUnit.Framework;

namespace DataDictionary.test
{
    [TestFixture]
    public class RefactoringTest : BaseModelTest
    {
        [Test]
        public void TestRefactorStructureName()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("N1.S1 { E1 => True }");
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Action a = CreateAction(rc, "V <- N1.S1 { E1 => False }");

            Refactor(s1, "NewS1");
            Assert.AreEqual("NewS1", el2.TypeName);
            Assert.AreEqual("NewS1 { E1 => True }", v.getDefaultValue());
            Assert.AreEqual("V <- NewS1 { E1 => False }", a.ExpressionText);
        }

        [Test]
        public void TestRefactorElementStructureName()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            RuleCondition rc1 = CreateRuleAndCondition(s2, "Rule1");
            Action action = CreateAction(rc1, "E2.E1 <- False");

            Variable variable = CreateVariable(n1, "A", "S1");
            Action action2 = CreateAction(rc1, "A <- S1 { E1 => False }");

            Refactor(el1, "NewE1");
            Assert.AreEqual("E2.NewE1 <- False", action.ExpressionText);
        }

        [Test]
        public void TestRefactorElementStructureName2()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Variable variable = CreateVariable(n1, "A", "S1");
            RuleCondition rc1 = CreateRuleAndCondition(n1, "Rule1");
            Action action = CreateAction(rc1, "A <- S1 { E1 => False }");

            Refactor(el1, "NewE1");
            Assert.AreEqual("A <- S1 { NewE1 => False }", action.ExpressionText);
        }

        [Test]
        public void TestRefactorNameSpaceName()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("N1.S1 { E1 => True }");
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Action a = CreateAction(rc, "V <- N1.S1 { E1 => False }");

            Refactor(n1, "NewN1");
            Assert.AreEqual("NewN1.S1 { E1 => True }", v.getDefaultValue());
            Assert.AreEqual("V <- NewN1.S1 { E1 => False }", a.ExpressionText);
        }

        [Test]
        public void TestRefactorNameSpaceName2()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            NameSpace n1 = CreateNameSpace(n0, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("N0.N1.S1 { E1 => True }");
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Action a = CreateAction(rc, "V <- N0.N1.S1 { E1 => False }");

            Refactor(n0, "NewN0");
            Assert.AreEqual("NewN0.N1.S1 { E1 => True }", v.getDefaultValue());
            Assert.AreEqual("V <- NewN0.N1.S1 { E1 => False }", a.ExpressionText);
        }

        [Test]
        public void TestRefactorNameSpaceName3()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            NameSpace n1 = CreateNameSpace(n0, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("S1 { E1 => True }");
            Function f = CreateFunction(n1, "f", "S2");

            NameSpace n2 = CreateNameSpace(n0, "N2");
            RuleCondition rc = CreateRuleAndCondition(n2, "Rule1");
            PreCondition p = CreatePreCondition(rc, "N1.f().E2.E1");
            Action a = CreateAction(rc, "N1.V <- N1.S1 { E1 => False }");

            Refactor(n1, "NewN1");
            Assert.AreEqual("S1 { E1 => True }", v.getDefaultValue());
            Assert.AreEqual("NewN1.V <- NewN1.S1 { E1 => False }", a.ExpressionText);
            Assert.AreEqual("NewN1.f().E2.E1", p.ExpressionText);

            RefactorAndRelocate(p);
            Assert.AreEqual("NewN1.f().E2.E1", p.ExpressionText);
        }

        [Test]
        public void TestRefactorNameSpaceName4()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            NameSpace n1 = CreateNameSpace(n0, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StateMachine stataMachine = CreateStateMachine(s1, "StateMachine");
            StructureElement el1 = CreateStructureElement(s1, "E1", "StateMachine");

            NameSpace n2 = CreateNameSpace(n0, "N2");

            Refactor(n2, "NewN2");
            Assert.AreEqual(stataMachine, el1.Type);
            Assert.AreEqual("StateMachine", el1.TypeName);
        }

        [Test]
        public void TestRefactorInterfaceName()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            Structure i1 = CreateStructure(n0, "I1");
            i1.setIsAbstract(true);
            Structure s1 = CreateStructure(n0, "S1");
            StructureRef sr = CreateStructureRef(s1, "I1");

            Refactor(i1, "NewI1");
            Assert.AreEqual("NewI1", sr.ExpressionText);
        }

        [Test]
        public void TestRefactorInterfaceField()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            Structure i1 = CreateStructure(n0, "I1");
            i1.setIsAbstract(true);
            StructureElement el1 = CreateStructureElement(i1, "E1", "Boolean");
            Structure s1 = CreateStructure(n0, "S1");
            StructureRef sr = CreateStructureRef(s1, "I1");
            StructureElement el2 = CreateStructureElement(s1, "E1", "Boolean");

            Refactor(el1, "NewE1");
            Assert.AreEqual("NewE1", el2.Name);
        }

        [Test]
        public void TestMoveFromDefaultNameSpace()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace defaultNameSpace = CreateNameSpace(test, "Default");
            Enum e1 = CreateEnum(defaultNameSpace, "E1");
            EnumValue v1 = CreateEnumValue(e1, "X");

            NameSpace n1 = CreateNameSpace(test, "N1");
            Variable var1 = CreateVariable(n1, "V1", "Default.E1");
            RuleCondition ruleCondition = CreateRuleAndCondition(n1, "R");
            Action action = CreateAction(ruleCondition, "V1 <- Default.E1.X");

            MoveToNameSpace(e1, n1);
            Assert.AreEqual("V1 <- E1.X", action.ExpressionText);
            Assert.AreEqual("E1", var1.TypeName);
        }

        [Test]
        public void TestMoveBugReport555()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace MANamespace = CreateNameSpace(test, "MA");
            NameSpace ModeProfileNamespace = CreateNameSpace(MANamespace, "ModeProfile");
            Variable TackVariable = CreateVariable(ModeProfileNamespace, "Tack", "Boolean");

            NameSpace KernelNameSpace = CreateNameSpace(test, "Kernel");
            RuleCondition ruleCondition = CreateRuleAndCondition(KernelNameSpace, "R");
            Action action = CreateAction(ruleCondition, "MA.ModeProfile.Tack <- False");

            NameSpace DMINamespace = CreateNameSpace(test, "DMI");
            NameSpace InNamespace = CreateNameSpace(DMINamespace, "IN");
            NameSpace DisplayNamespace = CreateNameSpace(InNamespace, "Display");

            MoveToNameSpace(TackVariable, DisplayNamespace);
            Assert.AreEqual("DMI.IN.Display.Tack <- False", action.ExpressionText);
        }

        [Test]
        public void TestMoveBugReport568()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace namespace1 = CreateNameSpace(test, "Kernel");
            NameSpace nameSpace2 = CreateNameSpace(namespace1, "MA");
            Function function1 = CreateFunction(nameSpace2, "SpeedRestriction", "Boolean");
            NameSpace nameSpace3 = CreateNameSpace(namespace1, "MRSP");
            Function function2 = CreateFunction(nameSpace3, "SpeedRestriction", "Boolean");

            Frame frame = CreateTestFrame(test, "frame");
            SubSequence subSequence = CreateSubSequence(frame, "subsequence");
            TestCase testCase = CreateTestCase(subSequence, "TestCase");
            Step step = CreateStep(testCase, "Step");
            SubStep subStep = CreateSubStep(step, "SubStep");
            Expectation expectation = CreateExpectation(subStep, "Kernel.MA.SpeedRestriction()");
            Expectation expectation2 = CreateExpectation(subStep,
                "MIN(Kernel.MA.SpeedRestriction, Kernel.MRSP.SpeedRestriction)");

            Refactor(function1, "SpeedRestriction");

            Assert.AreEqual("Kernel.MA.SpeedRestriction()", expectation.ExpressionText);
            Assert.AreEqual("MIN(Kernel.MA.SpeedRestriction, Kernel.MRSP.SpeedRestriction)", expectation2.ExpressionText);
        }

        /// <summary>
        ///     Test refactoring of a state machine
        /// </summary>
        [Test]
        public void TestRefactorStateMachine()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            StateMachine stateMachine = CreateStateMachine(nameSpace, "TestSM");
            State state1 = CreateState(stateMachine, "S1");
            State state2 = CreateState(stateMachine, "S2");
            stateMachine.Default = "S1";

            RuleCondition ruleCondition = CreateRuleAndCondition(stateMachine, "Test");
            PreCondition preCondition = CreatePreCondition(ruleCondition, "THIS in S1");
            Action action = CreateAction(ruleCondition, "THIS <- S2");

            Variable var = CreateVariable(nameSpace, "TheVariable", "TestSM");
            var.setDefaultValue("TestSM.S1");

            Refactor(stateMachine, "TestSM2");

            Assert.AreEqual("THIS in S1", preCondition.ExpressionText);
            Assert.AreEqual("THIS <- S2", action.ExpressionText);
            Assert.AreEqual(var.Type, stateMachine);
        }

        /// <summary>
        ///     Test refactoring of a state name and actions using that state name
        /// </summary>
        [Test]
        public void TestRefactorStateMachine2()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            StateMachine stateMachine = CreateStateMachine(nameSpace, "TestSM");
            State state1 = CreateState(stateMachine, "S1");
            State state2 = CreateState(stateMachine, "S2");
            stateMachine.Default = "S1";

            RuleCondition ruleCondition = CreateRuleAndCondition(state1.StateMachine, "Test");
            Action action = CreateAction(ruleCondition, "THIS <- TestSM.S2");

            Refactor(stateMachine, "TestSM2");
            Assert.AreEqual("THIS <- TestSM2.S2", action.ExpressionText);

            Refactor(state2, "S3");
            Assert.AreEqual("THIS <- S3", action.ExpressionText);
        }

        /// <summary>
        ///     Test refactoring of a state machine with sub states
        /// </summary>
        [Test]
        public void TestRefactorStateMachine3()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            StateMachine stateMachine = CreateStateMachine(nameSpace, "TestSM");
            State state1 = CreateState(stateMachine, "S1");
            State state2 = CreateState(stateMachine, "S2");
            stateMachine.Default = "S1";

            StateMachine stateMachine2 = state2.StateMachine;
            State state21 = CreateState(stateMachine2, "S21");
            State state22 = CreateState(stateMachine2, "S22");
            stateMachine.Default = "S21";

            RuleCondition ruleCondition = CreateRuleAndCondition(state21.StateMachine, "Test");
            Action action = CreateAction(ruleCondition, "THIS <- TestSM.S2.S22");

            Refactor(stateMachine, "TestSM2");
            Assert.AreEqual("THIS <- TestSM2.S2.S22", action.ExpressionText);

            Refactor(state2, "S3");
            Assert.AreEqual("THIS <- S3.S22", action.ExpressionText);

            Refactor(nameSpace, "N2");
            Assert.AreEqual("THIS <- S3.S22", action.ExpressionText);
        }

        /// <summary>
        ///     Test that refactoring does not changes the referenced element
        /// </summary>
        [Test]
        public void TestRefactorEnsureSameReference()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");
            NameSpace nameSpace2 = CreateNameSpace(nameSpace1, "N2");

            Enum enumeration = CreateEnum(nameSpace1, "Mode");
            CreateEnumValue(enumeration, "E1");
            CreateEnumValue(enumeration, "E2");

            Function function = CreateFunction(nameSpace2, "Mode", "N1.Mode");
            Assert.AreEqual(function.ReturnType, enumeration);

            Refactor(nameSpace1, "N3");

            Assert.AreEqual(function.ReturnType, enumeration);
        }

        /// <summary>
        ///     Test that cleanup updates the dereferences correctly
        /// </summary>
        [Test]
        public void TestCleanUp1()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");
            NameSpace nameSpace2 = CreateNameSpace(nameSpace1, "N2");

            Enum enumeration = CreateEnum(nameSpace2, "Mode");
            CreateEnumValue(enumeration, "E1");
            CreateEnumValue(enumeration, "E2");

            Variable variable1 = CreateVariable(nameSpace1, "V", "N1.N2.Mode");
            variable1.Default = "N1.N2.Mode.E1";

            RuleCondition ruleCondition = CreateRuleAndCondition(nameSpace1, "Test");
            PreCondition preCondition = CreatePreCondition(ruleCondition, "V == N1.N2.Mode.E1");
            Action action = CreateAction(ruleCondition, "V <- N1.N2.Mode.E2");

            Variable variable2 = CreateVariable(nameSpace2, "V2", "Mode");
            variable2.Default = "N1.N2.Mode.E1";

            CleanUpModel();
            Assert.AreEqual("V == N2.Mode.E1", preCondition.ExpressionText);
            Assert.AreEqual("V <- N2.Mode.E2", action.ExpressionText);
            Assert.AreEqual("N2.Mode.E1", variable1.Default);
            Assert.AreEqual("Mode.E1", variable2.Default);
        }

        /// <summary>
        ///     Test that renaming of a structure also renamed references to that structure
        /// </summary>
        [Test]
        public void TestChangeStructureNameAndUpdateVariableReferences()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");

            Structure structure = CreateStructure(nameSpace1, "Struct");
            Variable variable = CreateVariable(nameSpace1, "V", "Struct");

            Assert.AreEqual(variable.Type, structure);
            Assert.AreEqual(variable.TypeName, "Struct");

            Refactor(structure, "Struct2");

            Assert.AreEqual(variable.Type, structure);
            Assert.AreEqual(variable.TypeName, "Struct2");
        }

        /// <summary>
        ///     Test that renaming of an enumeration is correctly taken into account.
        /// This is related to bug 669
        /// </summary>
        [Test]
        public void TestBugReport669()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");

            Enum enumeration = CreateEnum(nameSpace1, "Mode");
            Enum enum2 = CreateEnum(enumeration, "M");

            Structure structure = CreateStructure(nameSpace1, "Struct");
            StructureElement element = CreateStructureElement(structure, "E", "Mode.M");

            Function function = CreateFunction(nameSpace1, "F", "Mode.M");

            Refactor(enumeration, "ModeEnum");

            Assert.AreEqual(element.Type, enum2);
            Assert.AreEqual(element.TypeName, "ModeEnum.M");
            Assert.AreEqual(function.TypeName, "ModeEnum.M");
        }

        /// <summary>
        ///     Test that the displayed value of a variable, whose type is refactored,
        ///     is not invalidated by the refactoring.
        /// </summary>
        [Test]
        public void TestRefactorStructureNameAndVariableDefinition()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");

            Structure structure = CreateStructure(nameSpace1, "Test");
            Variable variable = CreateVariable(nameSpace1, "V", "N1.Test");
            variable.setDefaultValue("");

            Assert.AreEqual(variable.Type, structure);

            Refactor(structure, "TestStruct");

            Assert.AreEqual(structure.Name, "TestStruct");
            Assert.AreEqual(variable.TypeName, "TestStruct");
            Assert.AreEqual(variable.Default, "");
        }

        /// <summary>
        ///     Test that moving a namespace does not cause elements inside it to be renamed
        /// </summary>
        [Test]
        public void TestRelocateNameSpaceWithStructure()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");

            Structure structure = CreateStructure(nameSpace1, "Test");
            StructureElement structureElement = CreateStructureElement(structure, "El1", "Boolean");
            Procedure procedure = CreateProcedure(structure, "ValidateEl1");
            RuleCondition condition1 = CreateRuleAndCondition(procedure, "Set to True");
            Action setTrue = CreateAction(condition1, "THIS.El1 <- True");

            Variable variable = CreateVariable(nameSpace1, "V", "N1.Test");
            
            RefactorAndRelocate(nameSpace1);

            Assert.AreEqual(((Action)condition1.Actions[0]).ExpressionText, "THIS.El1 <- True");
        }

        /// <summary>
        ///     Test that renaming a namespace does not cause elements inside it to be renamed
        /// </summary>
        [Test]
        public void TestRefactorNameSpaceWithStructure()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");

            Structure structure = CreateStructure(nameSpace1, "Test");
            StructureElement structureElement = CreateStructureElement(structure, "El1", "Boolean");
            Procedure procedure = CreateProcedure(structure, "ValidateEl1");
            RuleCondition condition1 = CreateRuleAndCondition(procedure, "Set to True");
            Action setTrue = CreateAction(condition1, "THIS.El1 <- True");

            Variable variable = CreateVariable(nameSpace1, "V", "N1.Test");

            Refactor(nameSpace1, "NameSpace");

            Assert.AreEqual(((Action)condition1.Actions[0]).ExpressionText, "THIS.El1 <- True");
        }

        /// <summary>
        ///     Test that renaming a structure does not replace expressions of the type
        ///         FIRST X IN ... where X is of the structure's type
        /// </summary>
        [Test]
        public void TestRefactorStructureAccessedwithFIRST_IN()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");

            Structure structure = CreateStructure(nameSpace1, "Test");
            StructureElement element = CreateStructureElement(structure, "El", "Boolean");
            Collection collection = CreateCollection(nameSpace1, "TestCol", "Test", 10);

            Variable varCol = CreateVariable(nameSpace1, "var", "TestCol");

            Function function = CreateFunction(nameSpace1, "TestFun", "Boolean");
            Case case1 = CreateCase(function, "Value", "(FIRST X IN var).El");

            Refactor(structure, "TestStructure");

            Assert.AreEqual(((Case)function.Cases[0]).ExpressionText, "(FIRST X IN var).El");
        }

        /// <summary>
        ///     Test that when a structure is renamed, the procedures it contains with an expression
        ///     such as THIS.Element are not modified
        /// </summary>
        [Test]
        public void TestRenameStructureWithProcedureReferencingElement()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");

            Structure structure = CreateStructure(nameSpace1, "Test");
            CreateStructureElement(structure, "El1", "Boolean");
            Procedure procedure = CreateProcedure(structure, "ValidateEl1");
            RuleCondition condition1 = CreateRuleAndCondition(procedure, "Set to True");
            CreateAction(condition1, "THIS.El1 <- True");


            Refactor(structure, "Structure");

            Assert.AreEqual(((Action)condition1.Actions[0]).ExpressionText, "THIS.El1 <- True");
        }


        /// <summary>
        ///     Test that the FIRST x IN expression is correctly relocated
        /// </summary>
        [Test]
        public void TestRefactorFIRST_IN()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");

            Structure structure = CreateStructure(nameSpace1, "S");
            CreateStructureElement(structure, "El1", "Boolean");
            Collection collection = CreateCollection(nameSpace1, "Col", "S", 10);
            Variable variable = CreateVariable(nameSpace1, "V", "Col");
            Variable variable2 = CreateVariable(nameSpace1, "B", "Boolean");

            RuleCondition condition1 = CreateRuleAndCondition(nameSpace1, "RuleCondition");
            CreateAction(condition1, "B <- (FIRST el IN V).El1");

            RefactorAndRelocate(condition1);

            Assert.AreEqual(((Action)condition1.Actions[0]).ExpressionText, "B <- (FIRST el IN V).El1");
        }


        /// <summary>
        ///     Test that the LAST x IN expression is correctly relocated
        /// </summary>
        [Test]
        public void TestRefactorLAST_IN()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace1 = CreateNameSpace(dictionary, "N1");

            Structure structure = CreateStructure(nameSpace1, "S");
            CreateStructureElement(structure, "El1", "Boolean");
            Collection collection = CreateCollection(nameSpace1, "Col", "S", 10);
            Variable variable = CreateVariable(nameSpace1, "V", "Col");
            Variable variable2 = CreateVariable(nameSpace1, "B", "Boolean");

            RuleCondition condition1 = CreateRuleAndCondition(nameSpace1, "RuleCondition");
            CreateAction(condition1, "B <- (FIRST el IN V).El1");

            RefactorAndRelocate(condition1);

            Assert.AreEqual(((Action)condition1.Actions[0]).ExpressionText, "B <- (FIRST el IN V).El1");
        }
    }
}