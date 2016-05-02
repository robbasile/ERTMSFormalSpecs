// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using DataDictionary.Generated;
using DataDictionary.Specification;
using XmlBooster;
using Chapter = DataDictionary.Generated.Chapter;
using ChapterRef = DataDictionary.Generated.ChapterRef;
using Message = DataDictionary.Generated.Message;
using MsgVariable = DataDictionary.Generated.MsgVariable;
using Paragraph = DataDictionary.Generated.Paragraph;
using ParagraphRevision = DataDictionary.Generated.ParagraphRevision;
using RequirementSet = DataDictionary.Generated.RequirementSet;
using RequirementSetDependancy = DataDictionary.Generated.RequirementSetDependancy;
using RequirementSetReference = DataDictionary.Generated.RequirementSetReference;
using TypeSpec = DataDictionary.Generated.TypeSpec;
using RuleCheckIdentifier = DataDictionary.RuleCheck.RuleCheckIdentifier;

namespace DataDictionary
{
    /// <summary>
    ///     Sets the default values of the objects
    /// </summary>
    public class DefaultValueSetter : Visitor
    {
        /// <summary>
        ///     Indicates that the Guid should be automatically set when creating a new object
        /// </summary>
        public bool AutomaticallyGenerateGuid { get; set; }

        /// <summary>
        ///     Sets the default values for the element provided as parameter
        /// </summary>
        /// <param name="model"></param>
        public void SetDefaultValue<T>(T model)
            where T : IXmlBBase
        {
            // We are creating a new element, notification about changes in that element
            // are not necessary.
            Util.DontNotify(() =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                dispatch(model);
            });
        }

        public override void visit(IXmlBBase obj, bool visitSubNodes)
        {
            IGraphicalDisplay graphicalDisplay = obj as IGraphicalDisplay;
            if (graphicalDisplay != null)
            {
                graphicalDisplay.X = -1;
                graphicalDisplay.Y = -1;
                graphicalDisplay.Width = 0;
                graphicalDisplay.Height = 0;
                graphicalDisplay.Hidden = false;
                graphicalDisplay.Pinned = false;
            }
            base.visit(obj);
        }

        public override void visit(BaseModelElement obj, bool visitSubNodes)
        {
            ModelElement element = (ModelElement) obj;

            if (AutomaticallyGenerateGuid)
            {
                // Side effect : creates the guid of the element
                // ReSharper disable once UnusedVariable
                string guid = element.Guid;
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.ReqRelated obj, bool visitSubNodes)
        {
            obj.setImplemented(false);
            obj.setVerified(false);
            obj.setNeedsRequirement(true);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(EnumValue obj, bool visitSubNodes)
        {
            obj.setValue("0");

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Type obj, bool visitSubNodes)
        {
            obj.setDefault("");

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Range obj, bool visitSubNodes)
        {
            obj.setPrecision(acceptor.PrecisionEnum.aIntegerPrecision);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(StructureElement obj, bool visitSubNodes)
        {
            obj.setDefault("");
            obj.setMode(acceptor.VariableModeEnumType.aInternal);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Collection obj, bool visitSubNodes)
        {
            obj.setMaxSize(10);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Function obj, bool visitSubNodes)
        {
            obj.setCacheable(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Variable obj, bool visitSubNodes)
        {
            obj.setVariableMode(acceptor.VariableModeEnumType.aInternal);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Rule obj, bool visitSubNodes)
        {
            obj.setPriority(acceptor.RulePriority.aProcessing);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Frame obj, bool visitSubNodes)
        {
            obj.setCycleDuration("0.1");

            base.visit(obj, visitSubNodes);
        }

        public override void visit(SubSequence obj, bool visitSubNodes)
        {
            obj.setD_LRBG("");
            obj.setLevel("");
            obj.setMode("");
            obj.setNID_LRBG("");
            obj.setQ_DIRLRBG("");
            obj.setQ_DIRTRAIN("");
            obj.setQ_DLRBG("");
            obj.setRBC_ID("");
            obj.setRBCPhone("");
            obj.setCompleted(true);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(TestCase obj, bool visitSubNodes)
        {
            obj.setFeature(9999);
            obj.setCase(9999);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Step obj, bool visitSubNodes)
        {
            obj.setTCS_Order(0);
            obj.setDistance("0");
            obj.setIO(acceptor.ST_IO.StIO_NA);
            obj.setLevelIN(acceptor.ST_LEVEL.StLevel_NA);
            obj.setLevelOUT(acceptor.ST_LEVEL.StLevel_NA);
            obj.setModeIN(acceptor.ST_MODE.Mode_NA);
            obj.setModeOUT(acceptor.ST_MODE.Mode_NA);
            obj.setTranslationRequired(false);
            obj.setTranslated(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(SubStep obj, bool visitSubNodes)
        {
            obj.setSkipEngine(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Expectation obj, bool visitSubNodes)
        {
            obj.setKind(acceptor.ExpectationKind.aInstantaneous);
            obj.setBlocking(true);
            obj.setDeadLine(1);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(DBMessage obj, bool visitSubNodes)
        {
            obj.setMessageOrder(0);
            obj.setMessageType(acceptor.DBMessageType.aEUROBALISE);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(DBField obj, bool visitSubNodes)
        {
            obj.setValue("");

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Translation obj, bool visitSubNodes)
        {
            obj.setImplemented(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(SourceText obj, bool visitSubNodes)
        {
            obj.setRegularExpression(false);

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Paragraph obj, bool visitSubNodes)
        {
            obj.setGuid("");
            obj.setType(acceptor.Paragraph_type.aREQUIREMENT);
            obj.setObsoleteScope(acceptor.Paragraph_scope.aFLAGS);
            obj.setObsoleteScopeOnBoard(false);
            obj.setObsoleteScopeTrackside(false);
            obj.setObsoleteScopeRollingStock(false);
            obj.setBl("");
            obj.setOptional(true);
            obj.setName("");
            obj.setReviewed(false);
            obj.setImplementationStatus(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA);
            obj.setTested(false);
            obj.setVersion("3.0.0");
            obj.setMoreInfoRequired(false);
            obj.setSpecIssue(false);
            obj.setObsoleteFunctionalBlock(false);
            obj.setObsoleteFunctionalBlockName("");

            base.visit(obj, visitSubNodes);
        }
    }

    /// <summary>
    ///     The factory
    /// </summary>
    public class ObjectFactory : Factory
    {
        /// <summary>
        ///     The class used to set the default values
        /// </summary>
        private readonly DefaultValueSetter _defaultValueSetter = new DefaultValueSetter();

        /// <summary>
        ///     Indicates that the Guid should be automatically set when creating a new object
        /// </summary>
        public bool AutomaticallyGenerateGuid
        {
            get { return _defaultValueSetter.AutomaticallyGenerateGuid; }
            set { _defaultValueSetter.AutomaticallyGenerateGuid = value; }
        }

        public override Generated.Dictionary createDictionary()
        {
            Generated.Dictionary retVal = new Dictionary();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Generated.ReqRef createReqRef()
        {
            Generated.ReqRef retVal = new ReqRef();

            _defaultValueSetter.SetDefaultValue(retVal);
            return retVal;
        }

        /// <summary>
        ///     Rule disabling is obsolete
        /// </summary>
        /// <returns></returns>
        public override RuleCheckDisabling createRuleCheckDisabling()
        {
            RuleCheckDisabling retVal = new RuleCheck.RuleCheckDisabling();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Generated.RuleCheckIdentifier createRuleCheckIdentifier()
        {
            RuleCheckIdentifier retVal = new RuleCheckIdentifier();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override NameSpace createNameSpace()
        {
            NameSpace retVal = new Types.NameSpace();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override NameSpaceRef createNameSpaceRef()
        {
            NameSpaceRef retVal = new Types.NameSpaceRef();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Enum createEnum()
        {
            Enum retVal = new Types.Enum();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override EnumValue createEnumValue()
        {
            EnumValue retVal = new Constants.EnumValue();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Range createRange()
        {
            Range retVal = new Types.Range();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Structure createStructure()
        {
            Structure retVal = new Types.Structure();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override StructureRef createStructureRef()
        {
            StructureRef retVal = new Types.StructureRef();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Collection createCollection()
        {
            Collection retVal = new Types.Collection();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override StructureElement createStructureElement()
        {
            StructureElement retVal = new Types.StructureElement();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Function createFunction()
        {
            Function retVal = new Functions.Function();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Generated.Parameter createParameter()
        {
            Generated.Parameter retVal = new Parameter();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Case createCase()
        {
            Case retVal = new Functions.Case();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Procedure createProcedure()
        {
            Procedure retVal = new Functions.Procedure();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override StateMachine createStateMachine()
        {
            StateMachine retVal = new Types.StateMachine();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override State createState()
        {
            State retVal = new Constants.State();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Variable createVariable()
        {
            Variable retVal = new Variables.Variable();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Rule createRule()
        {
            Rule retVal = new Rules.Rule();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override RuleCondition createRuleCondition()
        {
            RuleCondition retVal = new Rules.RuleCondition();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override PreCondition createPreCondition()
        {
            PreCondition retVal = new Rules.PreCondition();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Action createAction()
        {
            Action retVal = new Rules.Action();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override FrameRef createFrameRef()
        {
            FrameRef retVal = new Tests.FrameRef();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Frame createFrame()
        {
            Frame retVal = new Tests.Frame();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override SubSequence createSubSequence()
        {
            SubSequence retVal = new Tests.SubSequence();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override TestCase createTestCase()
        {
            TestCase retVal = new Tests.TestCase();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Step createStep()
        {
            Step retVal = new Tests.Step();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override SubStep createSubStep()
        {
            SubStep retVal = new Tests.SubStep();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Expectation createExpectation()
        {
            Expectation retVal = new Tests.Expectation();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override DBMessage createDBMessage()
        {
            DBMessage retVal = new Tests.DBElements.DBMessage();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override DBPacket createDBPacket()
        {
            DBPacket retVal = new Tests.DBElements.DBPacket();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override DBField createDBField()
        {
            DBField retVal = new Tests.DBElements.DBField();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Generated.Specification createSpecification()
        {
            Generated.Specification retVal = new Specification.Specification();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override RequirementSet createRequirementSet()
        {
            RequirementSet retVal = new Specification.RequirementSet();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override RequirementSetDependancy createRequirementSetDependancy()
        {
            RequirementSetDependancy retVal = new Specification.RequirementSetDependancy();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override RequirementSetReference createRequirementSetReference()
        {
            RequirementSetReference retVal = new Specification.RequirementSetReference();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Chapter createChapter()
        {
            Chapter retVal = new Specification.Chapter();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override ChapterRef createChapterRef()
        {
            ChapterRef retVal = new Specification.ChapterRef();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Paragraph createParagraph()
        {
            Paragraph retVal = new Specification.Paragraph();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Message createMessage()
        {
            Message retVal = new Specification.Message();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override MsgVariable createMsgVariable()
        {
            MsgVariable retVal = new Specification.MsgVariable();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override TypeSpec createTypeSpec()
        {
            TypeSpec retVal = new Specification.TypeSpec();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Generated.Values createValues()
        {
            Generated.Values retVal = new Specification.Values();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override special_or_reserved_values createspecial_or_reserved_values()
        {
            special_or_reserved_values retVal = new SpecialOrReservedValues();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override special_or_reserved_value createspecial_or_reserved_value()
        {
            special_or_reserved_value retVal = new SpecialOrReservedValue();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override mask createmask()
        {
            mask retVal = new Mask();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override match creatematch()
        {
            match retVal = new Match();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override meaning createmeaning()
        {
            meaning retVal = new Meaning();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override match_range creatematch_range()
        {
            match_range retVal = new MatchRange();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override resolution_formula createresolution_formula()
        {
            resolution_formula retVal = new ResolutionFormula();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override value createvalue()
        {
            value retVal = new Value();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override char_value createchar_value()
        {
            char_value retVal = new CharValue();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override ParagraphRevision createParagraphRevision()
        {
            ParagraphRevision retVal = new Specification.ParagraphRevision();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override TranslationDictionary createTranslationDictionary()
        {
            TranslationDictionary retVal = new Tests.Translations.TranslationDictionary();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Folder createFolder()
        {
            Folder retVal = new Tests.Translations.Folder();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Translation createTranslation()
        {
            Translation retVal = new Tests.Translations.Translation();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override SourceText createSourceText()
        {
            SourceText retVal = new Tests.Translations.SourceText();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override SourceTextComment createSourceTextComment()
        {
            SourceTextComment retVal = new Tests.Translations.SourceTextComment();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override ShortcutDictionary createShortcutDictionary()
        {
            ShortcutDictionary retVal = new Shortcuts.ShortcutDictionary();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override ShortcutFolder createShortcutFolder()
        {
            ShortcutFolder retVal = new Shortcuts.ShortcutFolder();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }

        public override Shortcut createShortcut()
        {
            Shortcut retVal = new Shortcuts.Shortcut();

            _defaultValueSetter.SetDefaultValue(retVal);

            return retVal;
        }
    }
}