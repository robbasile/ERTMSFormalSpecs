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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.RuleCheck.GraphAndSurface;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Action = DataDictionary.Generated.Action;
using Case = DataDictionary.Generated.Case;
using Collection = DataDictionary.Types.Collection;
using Enum = DataDictionary.Generated.Enum;
using Function = DataDictionary.Generated.Function;
using NameSpace = DataDictionary.Types.NameSpace;
using Paragraph = DataDictionary.Specification.Paragraph;
using Range = DataDictionary.Generated.Range;
using RequirementSet = DataDictionary.Specification.RequirementSet;
using Rule = DataDictionary.Rules.Rule;
using SourceText = DataDictionary.Tests.Translations.SourceText;
using SourceTextComment = DataDictionary.Tests.Translations.SourceTextComment;
using StateMachine = DataDictionary.Generated.StateMachine;
using Structure = DataDictionary.Generated.Structure;
using StructureElement = DataDictionary.Generated.StructureElement;
using StructureRef = DataDictionary.Generated.StructureRef;
using SubStep = DataDictionary.Tests.SubStep;
using TestCase = DataDictionary.Tests.TestCase;
using Translation = DataDictionary.Tests.Translations.Translation;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Generated.Variable;
using Visitor = DataDictionary.Generated.Visitor;

namespace DataDictionary.RuleCheck
{
    /// <summary>
    ///     Logs messages on the rules according to the validity of the rule
    /// </summary>
    public class RuleCheckerVisitor : Visitor
    {
        /// <summary>
        ///     The dictionary used for this visit
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public RuleCheckerVisitor(Dictionary dictionary)
        {
            FinderRepository.INSTANCE.ClearCache();
            Dictionary = dictionary;
        }

        /// <summary>
        /// Checks all rules on the selected dictionary
        /// </summary>
        public void CheckRules()
        {
            visit(Dictionary, true);
            SpecificChecks();
        }

        /// <summary>
        /// Specific checks, which cannot be performed using the visitor
        /// </summary>
        private void SpecificChecks()
        {
            GraphAndSurfaceCheck graphAndSurfaceCheck = new GraphAndSurfaceCheck(Dictionary);
            graphAndSurfaceCheck.CheckGraphAndSurfaceExpression();
        }

        /// <summary>
        ///     Indicates whether one of the parent of the model element has been updated (by a path)
        /// </summary>
        /// <param name="modelElement"></param>
        /// <returns></returns>
        private bool ParentHasBeenUpdated(ModelElement modelElement)
        {
            bool retVal = false;

            ModelElement current = modelElement;
            while (current != null && !(current is NameSpace) && !retVal)
            {
                retVal = current.UpdatedBy.Count > 0;
                current = current.Enclosing as ModelElement;
            }

            return retVal;
        }

        /// <summary>
        ///     Checks an expression associated to a model element
        /// </summary>
        /// <param name="model">The model element on which the expression is defined</param>
        /// <param name="expression">The expression to check</param>
        /// <returns>the expression parse tree</returns>
        public Expression CheckExpression(ModelElement model, string expression)
        {
            Expression retVal = null;

            // Only check expressions for which the model is not updated
            if (!ParentHasBeenUpdated(model))
            {
                Parser parser = new Parser();
                try
                {
                    retVal = parser.Expression(model, expression);

                    if (retVal != null)
                    {
                        retVal.CheckExpression();
                        Type type = retVal.GetExpressionType();
                        if (type == null)
                        {
                            model.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                                "Cannot determine expression type (5) for " + retVal);
                        }
                    }
                    else
                    {
                        model.AddRuleCheckMessage(RuleChecksEnum.SyntaxError, ElementLog.LevelEnum.Error, 
                            "Cannot parse expression");
                    }
                }
                catch (Exception exception)
                {
                    model.AddException(exception);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Checks a statement associated to a model element
        /// </summary>
        /// <param name="model">The model element on which the expression is defined</param>
        /// <param name="expression">The expression to check</param>
        /// <returns>the expression parse tree</returns>
        public Statement CheckStatement(ModelElement model, string expression)
        {
            Statement retVal = null;

            // Only check statements for model elements which are not updated
            if (!ParentHasBeenUpdated(model))
            {
                Parser parser = new Parser();
                try
                {
                    retVal = parser.Statement(model, expression);
                    if (retVal != null)
                    {
                        retVal.CheckStatement();
                    }
                    else
                    {
                        model.AddRuleCheckMessage(RuleChecksEnum.SyntaxError, ElementLog.LevelEnum.Error, 
                            "Cannot parse statement");
                    }
                }
                catch (Exception exception)
                {
                    model.AddException(exception);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Checks the consistency of this model's update
        /// </summary>
        /// <param name="model"></param>
        private void checkUpdate(ModelElement model)
        {
            if (model.UpdatedBy.Count > 1)
            {
                if (!(model is Dictionary || model is NameSpace))
                {
                    model.AddRuleCheckMessage(RuleChecksEnum.Update01, ElementLog.LevelEnum.Error, 
                        "Updates conflict: this model element is updated in multiple patches.");
                    foreach (ModelElement element in model.UpdatedBy)
                    {
                        element.AddRuleCheckMessage(RuleChecksEnum.Update01, ElementLog.LevelEnum.Error, 
                            "Updates conflict: the update target has also been updated in another patch.");
                    }
                }
            }

            if (model.getUpdates() != null && model.Updates == null)
            {
                model.AddRuleCheckMessage(RuleChecksEnum.Update02, ElementLog.LevelEnum.Error, 
                    "Cannot find the element updated by this.");
            }
        }

        public override void visit(BaseModelElement obj, bool visitSubNodes)
        {
            CheckComment(obj as ICommentable);

            ModelElement model = obj as ModelElement;
            if (model != null)
            {
                checkUpdate(model);
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Frame obj, bool visitSubNodes)
        {
            Tests.Frame frame = (Tests.Frame) obj;

            if (frame != null)
            {
                CheckExpression(frame, frame.getCycleDuration());

                Type type = frame.CycleDuration.GetExpressionType();
                if (type != null)
                {
                    if (!frame.EFSSystem.DoubleType.Match(type))
                    {
                        frame.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                            "Cycle duration should be compatible with the Time type");
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(SubSequence obj, bool visitSubNodes)
        {
            Tests.SubSequence subSequence = (Tests.SubSequence) obj;

            if (subSequence != null)
            {
                if (!subSequence.getCompleted())
                {
                    subSequence.AddRuleCheckMessage(RuleChecksEnum.Test06, ElementLog.LevelEnum.Info,
                        "Sequences should be marked as completed to be automatically executed when executing the frame");
                }
                if (subSequence.TestCases.Count == 0)
                {
                    subSequence.AddRuleCheckMessage(RuleChecksEnum.Test01, ElementLog.LevelEnum.Warning, 
                        "Sub sequences should hold at least one test case");
                }
                else
                {
                    TestCase testCase = (TestCase) subSequence.TestCases[0];

                    if (testCase.Steps.Count == 0)
                    {
                        testCase.AddRuleCheckMessage(RuleChecksEnum.Test02, ElementLog.LevelEnum.Warning, 
                            "First test case of a subsequence should hold at least one step");
                    }
                    else
                    {
                        Tests.Step step = (Tests.Step) testCase.Steps[0];
                        if (step.Name != "")
                        {
                            if (step.Name.IndexOf("Setup") < 0 && step.Name.IndexOf("Initialize") < 0)
                            {
                                step.AddRuleCheckMessage(RuleChecksEnum.Test03, ElementLog.LevelEnum.Warning, 
                                    "First step of the first test case of a subsequence should be used to setup the system, and should hold 'Setup' or 'Initialize' in its name");
                            }
                        }
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        ///     Ensure that all step that should be automatically translated have a translation
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Step obj, bool visitSubNodes)
        {
            Tests.Step step = (Tests.Step) obj;

            if (step.getTranslationRequired())
            {
                Translation translation = EfsSystem.Instance.FindTranslation(step);
                if (translation == null)
                {
                    step.AddRuleCheckMessage(RuleChecksEnum.Translation02, ElementLog.LevelEnum.Warning,
                        "Cannot find translation for this step");
                }

                if (step.getDescription() != null)
                {
                    // Specific checks for subset-076
                    if ((step.getDescription().IndexOf("balise group", StringComparison.InvariantCultureIgnoreCase) !=
                         -1) && step.getDescription().Contains("is received") )
                    {
                        if (step.StepMessages.Count == 0)
                        {
                            step.AddRuleCheckMessage(RuleChecksEnum.Translation03, ElementLog.LevelEnum.Warning,
                                "Cannot find Balise messages for this step");
                        }
                    }

                    if (
                        (step.getDescription().IndexOf("euroloop message", StringComparison.InvariantCultureIgnoreCase) !=
                         -1) && step.getDescription().Contains("is received"))
                    {
                        if (step.StepMessages.Count == 0)
                        {
                            step.AddRuleCheckMessage(RuleChecksEnum.Translation03, ElementLog.LevelEnum.Warning,
                                "Cannot find Euroloop messages for this step");
                        }
                    }

                    if (step.getDescription().Contains("SA-DATA") && step.getDescription().Contains("is received"))
                    {
                        if (step.StepMessages.Count == 0)
                        {
                            step.AddRuleCheckMessage(RuleChecksEnum.Translation03, ElementLog.LevelEnum.Warning,
                                "Cannot find RBC message for this step");
                        }
                    }
                }
            }
            else
            {
                if (step.TestCase.ContainsTranslations)
                {
                    if (step.Requirements.Count == 0 && step.SubSteps.Count > 0)
                    {
                        step.AddRuleCheckMessage(RuleChecksEnum.Translation04, ElementLog.LevelEnum.Warning,
                            "Manual translation for this step should be documented");                        
                    }
                }
            }

            if (step.Name == Tests.Step.DefaultName)
            {
                step.AddRuleCheckMessage(RuleChecksEnum.Test04, ElementLog.LevelEnum.Warning, 
                    "All steps should be given a name");
            }

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        ///     Applied to all nodes of the tree
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Namable obj, bool visitSubNodes)
        {
            Namable namable = (Namable) obj;

            if (obj is ITypedElement)
            {
                ITypedElement typedElement = obj as ITypedElement;

                Type type = typedElement.Type;
                if (type == null)
                {
                    namable.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                        "Cannot find type " + typedElement.TypeName);
                }
                else if (!(typedElement is Parameter) && !(type is Types.StateMachine))
                {
                    ITypedElement enclosingTypedElement = EnclosingFinder<ITypedElement>.find(typedElement);

                    while (enclosingTypedElement != null)
                    {
                        if (enclosingTypedElement.Type == type)
                        {
                            namable.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                                "Recursive types are not allowed for " + type.Name);
                            enclosingTypedElement = null;
                        }
                        else
                        {
                            enclosingTypedElement = EnclosingFinder<ITypedElement>.find(enclosingTypedElement);
                        }
                    }
                }

                // Searches all the namespaces that are visible to this element to check for types
                // with the same name. If any are found, adds a warning to the type.
                Type conflictingType = EfsSystem.Instance.FindType_silent(typedElement.NameSpace, typedElement.Name);
                if (conflictingType != null && conflictingType != typedElement)
                {
                    namable.AddRuleCheckMessage(RuleChecksEnum.Naming01, ElementLog.LevelEnum.Warning, 
                        namable.Name + " has the same name as type " + conflictingType.FullName);
                }

            }

            base.visit(obj, visitSubNodes);
        }


        /// <summary>
        ///     Indicates that the enclosed mode matches the enclosing mode, that is, that the enclosed mode is = or more
        ///     restrictive than the enclosing mode
        /// </summary>
        /// <param name="enclosing"></param>
        /// <param name="enclosed"></param>
        /// <returns></returns>
        private static bool ValidMode(acceptor.VariableModeEnumType enclosing, acceptor.VariableModeEnumType enclosed)
        {
            bool retVal = false;

            switch (enclosing)
            {
                case acceptor.VariableModeEnumType.aConstant:
                    retVal = enclosed == acceptor.VariableModeEnumType.aConstant;
                    break;

                case acceptor.VariableModeEnumType.aIncoming:
                    retVal = enclosed == acceptor.VariableModeEnumType.aIncoming
                             || enclosed == acceptor.VariableModeEnumType.aInternal
                             || enclosed == acceptor.VariableModeEnumType.aConstant;
                    break;

                case acceptor.VariableModeEnumType.aInOut:
                    retVal = enclosed == acceptor.VariableModeEnumType.aIncoming
                             || enclosed == acceptor.VariableModeEnumType.aInOut
                             || enclosed == acceptor.VariableModeEnumType.aInternal
                             || enclosed == acceptor.VariableModeEnumType.aOutgoing
                             || enclosed == acceptor.VariableModeEnumType.aConstant;
                    break;
                case acceptor.VariableModeEnumType.aInternal:
                    retVal = enclosed == acceptor.VariableModeEnumType.aInternal
                             || enclosed == acceptor.VariableModeEnumType.aConstant;
                    break;

                case acceptor.VariableModeEnumType.aOutgoing:
                    retVal = enclosed == acceptor.VariableModeEnumType.aInternal
                             || enclosed == acceptor.VariableModeEnumType.aOutgoing
                             || enclosed == acceptor.VariableModeEnumType.aConstant;
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Checks that a comment is attached to this ICommentable
        /// </summary>
        /// <param name="commentable"></param>
        private static void CheckComment(ICommentable commentable)
        {
            if (commentable != null)
            {
                if (string.IsNullOrEmpty(commentable.Comment))
                {
                    NameSpace nameSpace = EnclosingNameSpaceFinder.find((ModelElement) commentable, true);
                    bool requiresComment = nameSpace != null;

                    Types.StateMachine stateMachine = commentable as Types.StateMachine;
                    if (stateMachine != null)
                    {
                        requiresComment = stateMachine.EnclosingStateMachine == null;
                    }

                    Rules.RuleCondition ruleCondition = commentable as Rules.RuleCondition;
                    if (ruleCondition != null)
                    {
                        requiresComment = ruleCondition.EnclosingRule.RuleConditions.Count > 1;
                    }

                    if (commentable is Functions.Case
                        || commentable is Rules.PreCondition
                        || commentable is Rules.Action)
                    {
                        requiresComment = false;
                    }

                    Rule rule = commentable as Rule;
                    if (rule != null && rule.EnclosingProcedure != null)
                    {
                        requiresComment = rule.EnclosingProcedure.Rules.Count > 1;
                    }

                    ITypedElement typedElement = commentable as ITypedElement;
                    if (typedElement != null)
                    {
                        if (typedElement.Type != null)
                        {
                            requiresComment = requiresComment && string.IsNullOrEmpty(typedElement.Type.Comment);
                        }
                    }

                    IVariable variable = commentable as IVariable;
                    if (variable != null)
                    {
                        requiresComment = false;
                    }

                    if (requiresComment)
                    {
                        if (commentable is NameSpace)
                        {
                            ((ModelElement)commentable).AddRuleCheckMessage(RuleChecksEnum.Process16, ElementLog.LevelEnum.Info,
                                "Namespaces should have a description of their contents");
                        }
                        else
                        {
                            ((ModelElement)commentable).AddRuleCheckMessage(RuleChecksEnum.Process01, ElementLog.LevelEnum.Info,
                                "This element should be documented");
                        }
                    }
                }
                else if (commentable.Comment.Contains("TODO"))
                {
                    ((ModelElement) commentable).AddRuleCheckMessage(RuleChecksEnum.Process02, ElementLog.LevelEnum.Info, 
                        "The implementation of this element is unfinished - see comment");
                }
            }
        }

        public override void visit(StructureElement obj, bool visitSubNodes)
        {
            Types.StructureElement element = (Types.StructureElement) obj;

            if (!Utils.Util.isEmpty(element.getDefault()))
            {
                CheckExpression(element, element.getDefault());
            }

            if (element.DefaultValue != null)
            {
                if (!element.DefaultValue.Type.Match(element.Type))
                {
                    element.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error,
                        "Type of default value (" + element.DefaultValue.Type.FullName +
                        ") does not match element type (" + element.Type.FullName + ")");
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Variable obj, bool visitSubNodes)
        {
            Variables.Variable variable = obj as Variables.Variable;

            if (variable != null)
            {
                if (variable.Type == null)
                {
                    variable.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error,
                        "Cannot find type for variable");
                }
                else
                {
                    Types.Structure structure = variable.Type as Types.Structure;
                    if (structure != null)
                    {
                        foreach (Types.StructureElement element in structure.Elements)
                        {
                            if (!ValidMode(variable.Mode, element.Mode))
                            {
                                variable.AddRuleCheckMessage(RuleChecksEnum.Access01, ElementLog.LevelEnum.Warning,
                                    "Invalid mode for " + element.Name);
                            }
                        }
                    }
                    if (variable.Type.IsAbstract)
                    {
                        variable.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error,
                            "Instantiation of abstract types is forbidden");
                    }
                }
                if (Utils.Util.isEmpty(variable.Comment) && variable.Type != null &&
                    Utils.Util.isEmpty(variable.Type.Comment))
                {
                    variable.AddRuleCheckMessage(RuleChecksEnum.Process03, ElementLog.LevelEnum.Info, 
                        "Missing variable semantics. Update the 'Comment' associated to the variable or to the corresponding type");
                }

                if (!Utils.Util.isEmpty(variable.getDefaultValue()))
                {
                    CheckExpression(variable, variable.getDefaultValue());
                }

                if (variable.DefaultValue != null)
                {
                    if (!variable.DefaultValue.Type.Match(variable.Type))
                    {
                        variable.AddRuleCheckMessage(RuleChecksEnum.Process04, ElementLog.LevelEnum.Error, 
                            "Type of default value (" + variable.DefaultValue.Type.FullName +
                            ")does not match variable type (" + variable.Type.FullName + ")");
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Structure obj, bool visitSubNodes)
        {
            Types.Structure structure = obj as Types.Structure;

            if (structure != null)
            {
                foreach (Types.StructureElement element in structure.Elements)
                {
                    Types.Structure elementType = element.Type as Types.Structure;
                    if (elementType != null)
                    {
                        foreach (Types.StructureElement subElement in elementType.Elements)
                        {
                            if (!ValidMode(element.Mode, subElement.Mode))
                            {
                                element.AddRuleCheckMessage(RuleChecksEnum.Access01, ElementLog.LevelEnum.Warning, 
                                    "Invalid mode for " + subElement.Name);
                            }
                        }
                    }
                    if (element.Type != null && element.Type.IsAbstract)
                    {
                        element.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                            "Instantiation of abstract types is forbidden");
                    }
                    if (!Utils.Util.isEmpty(element.getDefault()))
                    {
                        CheckExpression(element, element.getDefault());
                    }
                }
                foreach (Types.Structure implementedStructure in structure.Interfaces)
                {
                    if (implementedStructure != null)
                    {
                        foreach (Types.StructureElement implementedElement in implementedStructure.Elements)
                        {
                            bool elementFound = false;
                            foreach (Types.StructureElement element in structure.Elements)
                            {
                                if (element.Name.Equals(implementedElement.Name))
                                {
                                    elementFound = true;
                                    if (element.Type != implementedElement.Type)
                                    {
                                        structure.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                                            "The type of element " + element.Name + " (" + element.TypeName +
                                            ") does not correspond to the type of the implemented element (" +
                                            implementedElement.TypeName + ")");
                                    }
                                    break;
                                }
                            }
                            if (elementFound == false)
                            {
                                structure.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                                    "Inherited member " + implementedElement.Name + " from interface " +
                                    implementedStructure.Name + " is not implemented");
                            }
                        }
                    }
                    else
                    {
                        structure.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error,
                            "Interface not found");
                    }
                }
            }

            CheckSubElementNames(structure);

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        ///     Check that all the SubElements of the structure have different names
        /// </summary>
        /// <param name="structure"></param>
        public void CheckSubElementNames(Types.Structure structure)
        {
            Dictionary<string, StructureElement> subElements = new Dictionary<string, StructureElement>();

            string ERROR_MESSAGE = "Structure elements should have unique names.";

            foreach (StructureElement element in structure.Elements)
            {
                if (subElements.ContainsKey(element.Name))
                {
                    element.AddRuleCheckMessage(RuleChecksEnum.Structure01, ElementLog.LevelEnum.Error, ERROR_MESSAGE);
                    subElements[element.Name].AddRuleCheckMessage(RuleChecksEnum.Structure01, ElementLog.LevelEnum.Error, ERROR_MESSAGE);
                }
                else
                {
                    subElements.Add(element.Name, element);
                }
            }
        }

        public override void visit(StructureRef obj, bool visitSubNodes)
        {
            bool errorDetected = true;

            Structure enclosingStructure = obj.Enclosing as Structure;
            if (enclosingStructure != null)
            {
                Structure structure = Dictionary.EFSSystem.FindType(enclosingStructure.NameSpace, obj.Name) as Structure;
                if (structure != null)
                {
                    if (structure.IsAbstract)
                    {
                        errorDetected = false;
                    }
                }
            }

            if (errorDetected)
            {
                obj.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                    "Referenced interface not found");
            }
        }

        public override void visit(Generated.ReqRef obj, bool visitSubNodes)
        {
            ReqRef reqRef = obj as ReqRef;
            if (reqRef != null)
            {
                if (reqRef.Paragraph == null)
                {
                    reqRef.AddRuleCheckMessage(RuleChecksEnum.Requirements01, ElementLog.LevelEnum.Error, 
                        "Invalid reference to a requirement (" + reqRef.getId() + ")");
                }
            }
        }

        public override void visit(Generated.ReqRelated obj, bool visitSubNodes)
        {
            ReqRelated reqRelated = obj as ReqRelated;

            ReqRelated current = reqRelated;
            if (reqRelated != null && reqRelated.NeedsRequirement) // the object must be associated to a requirement
            {
                // No requirement found (yet) for this object
                bool requirementFound = false;

                Types.StateMachine stateMachine = reqRelated as Types.StateMachine;
                if (stateMachine != null && stateMachine.EnclosingStateMachine != null)
                {
                    // We do not need requirements for a state machine since the traceability relationship
                    // is deduced from its enclosing state
                    requirementFound = true;
                }

                while (current != null && !requirementFound)
                {
                    foreach (ReqRef reqRef  in current.Requirements)
                    {
                        if (reqRef.Paragraph == null)
                        {
                            reqRef.AddRuleCheckMessage(RuleChecksEnum.Requirements02, ElementLog.LevelEnum.Error, 
                                "Cannot find paragraph corresponding to " + reqRef.getId());
                        }
                        else if (reqRef.Paragraph.getType() == acceptor.Paragraph_type.aREQUIREMENT)
                        {
                            // A requirement has been found
                            requirementFound = true;
                        }
                    }

                    // If no requirement found, we explore the requirements of the enclosing element
                    if (!requirementFound)
                    {
                        current = EnclosingFinder<ReqRelated>.find(current);
                    }
                }
                if (!requirementFound)
                {
                    reqRelated.AddRuleCheckMessage(RuleChecksEnum.Process05, ElementLog.LevelEnum.Info, 
                        "No requirement found for element");
                }
            }

            current = reqRelated;
            if (current != null && !current.getImplemented())
            {
                ModelElement parent = current.getFather() as ModelElement;
                while (parent != null)
                {
                    ReqRelated other = parent as ReqRelated;
                    if (other != null)
                    {
                        if (other.getImplemented())
                        {
                            other.AddRuleCheckMessage(RuleChecksEnum.Process06, ElementLog.LevelEnum.Warning,
                                "This element is set as implemented whereas one of its children is not");
                            current.AddRuleCheckMessage(RuleChecksEnum.Process06, ElementLog.LevelEnum.Warning,
                                "This element is set as not implemented whereas its parent is marked implemented");
                        }
                    }
                    parent = parent.getFather() as ModelElement;
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Rule obj, bool visitSubNodes)
        {
            Rules.Rule rule = obj as Rules.Rule;

            if (rule != null)
            {
                if (rule.EnclosingRule != null)
                {
                    if (rule.getPriority() != rule.EnclosingRule.getPriority())
                    {
                        rule.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, "Parent rule has not the same priority !");
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(RuleCondition obj, bool subNodes)
        {
            Rules.RuleCondition ruleCondition = obj as Rules.RuleCondition;

            if (ruleCondition != null)
            {
                try
                {
                    ruleCondition.Messages.Clear();

                    foreach (Rules.PreCondition preCondition in ruleCondition.PreConditions)
                    {
                        BinaryExpression expression =
                            CheckExpression(preCondition, preCondition.ExpressionText) as BinaryExpression;
                        if (expression != null)
                        {
                            if (expression.IsSimpleEquality())
                            {
                                ITypedElement variable = expression.Left.Ref as ITypedElement;
                                if (variable != null)
                                {
                                    if (variable.Type != null)
                                    {
                                        // Check that when preconditions are based on a request, 
                                        // the corresponding action affects the value Request.Disabled to the same variable
                                        if (variable.Type.Name.Equals("Request") && expression.Right is UnaryExpression)
                                        {
                                            IValue val2 = expression.Right.Ref as IValue;
                                            if (val2 != null && "Response".CompareTo(val2.Name) == 0)
                                            {
                                                bool found = false;
                                                foreach (Rules.Action action in ruleCondition.Actions)
                                                {
                                                    IVariable var = OverallVariableFinder.INSTANCE.findByName(
                                                        action, preCondition.findVariable());
                                                    VariableUpdateStatement update = action.Modifies(var);
                                                    if (update != null)
                                                    {
                                                        UnaryExpression updateExpr =
                                                            update.Expression as UnaryExpression;
                                                        if (updateExpr != null)
                                                        {
                                                            IValue val3 = updateExpr.Ref as IValue;
                                                            if (val3 != null && val3.Name.CompareTo("Disabled") == 0)
                                                            {
                                                                found = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }

                                                if (!found)
                                                {
                                                    preCondition.AddRuleCheckMessage(RuleChecksEnum.Request01, ElementLog.LevelEnum.Error,
                                                        "Rules where the Pre conditions is based on a Request type variable must assign that variable the value 'Request.Disabled'");
                                                }
                                            }
                                        }
                                    }

                                    // Check that the outgoing variables are not read
                                    if (variable.Mode == acceptor.VariableModeEnumType.aOutgoing)
                                    {
                                        if (ruleCondition.Reads(variable))
                                        {
                                            preCondition.AddRuleCheckMessage(RuleChecksEnum.Access02, ElementLog.LevelEnum.Error, 
                                                "An outgoing variable cannot be read");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ruleCondition.AddException(exception);
                }
            }

            base.visit(obj, subNodes);
        }

        public override void visit(PreCondition obj, bool subNodes)
        {
            Rules.PreCondition preCondition = obj as Rules.PreCondition;

            if (preCondition != null)
            {
                try
                {
                    // Check whether the expression is valid
                    Expression expression = CheckExpression(preCondition, preCondition.Condition);
                    if (expression != null)
                    {
                        if (!preCondition.Dictionary.EFSSystem.BoolType.Match(expression.GetExpressionType()))
                        {
                            preCondition.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                                "Expression type should be Boolean");
                        }

                        ITypedElement element = OverallTypedElementFinder.INSTANCE.findByName(preCondition,
                            preCondition.findVariable());
                        if (element != null)
                        {
                            if (element.Type is Types.StateMachine)
                            {
                                if (preCondition.findOperator() != null)
                                {
                                    if (preCondition.findOperator().CompareTo("==") == 0)
                                    {
                                        preCondition.AddRuleCheckMessage(RuleChecksEnum.StateMachine01, ElementLog.LevelEnum.Warning, 
                                            "Operator == should not be used for state machines");
                                    }
                                    else if (preCondition.findOperator().CompareTo("!=") == 0)
                                    {
                                        preCondition.AddRuleCheckMessage(RuleChecksEnum.StateMachine01, ElementLog.LevelEnum.Warning, 
                                            "Operator != should not be used for state machines");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    preCondition.AddException(exception);
                }
            }

            base.visit(obj, subNodes);
        }

        public override void visit(Action obj, bool subNodes)
        {
            Rules.Action action = obj as Rules.Action;

            if (action != null)
            {
                try
                {
                    action.Messages.Clear();
                    if (!action.ExpressionText.Contains('%'))
                    {
                        CheckStatement(action, action.ExpressionText);
                    }
                }
                catch (Exception exception)
                {
                    action.AddException(exception);
                }

                if (action.DeActivated)
                {
                    action.AddRuleCheckMessage(RuleChecksEnum.Process07, ElementLog.LevelEnum.Warning, 
                        "Action has been deactivated");
                }
            }

            base.visit(obj, subNodes);
        }

        public override void visit(Expectation obj, bool subNodes)
        {
            Tests.Expectation expect = obj as Tests.Expectation;

            if (expect != null)
            {
                try
                {
                    expect.Messages.Clear();
                    if (!expect.ExpressionText.Contains("%"))
                    {
                        Expression expression = CheckExpression(expect, expect.ExpressionText);
                        if (expression != null)
                        {
                            if (!expect.EFSSystem.BoolType.Match(expression.GetExpressionType()))
                            {
                                expect.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                                    "Expression type should be Boolean");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(expect.getCondition()) && !expect.getCondition().Contains("%"))
                    {
                        Expression expression = CheckExpression(expect, expect.getCondition());
                        if (expression != null)
                        {
                            if (!expect.EFSSystem.BoolType.Match(expression.GetExpressionType()))
                            {
                                expect.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                                    "Condition type should be Boolean");
                            }
                        }
                    }
                    if (expect.Blocking && expect.DeadLine < 0.0 && expect.getKind() == acceptor.ExpectationKind.aContinuous)
                    {
                        expect.AddRuleCheckMessage(RuleChecksEnum.Test05, ElementLog.LevelEnum.Warning,
                            "This expectation may cause the test to run indefinitely");
                    }
                }
                catch (Exception exception)
                {
                    expect.AddException(exception);
                }
            }

            base.visit(obj, subNodes);
        }

        public override void visit(StateMachine obj, bool visitSubNodes)
        {
            Types.StateMachine stateMachine = (Types.StateMachine) obj;

            if (stateMachine != null)
            {
                stateMachine.Messages.Clear();

                if (stateMachine.AllValues.Count > 0)
                {
                    if (Utils.Util.isEmpty(stateMachine.Default))
                    {
                        stateMachine.AddRuleCheckMessage(RuleChecksEnum.StateMachine02, ElementLog.LevelEnum.Error, 
                            "Empty initial state");
                    }
                    if (stateMachine.DefaultValue == null)
                    {
                        stateMachine.AddRuleCheckMessage(RuleChecksEnum.StateMachine03, ElementLog.LevelEnum.Error, 
                            "Cannot find default value");
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(State obj, bool visitSubNodes)
        {
            Constants.State state = (Constants.State) obj;

            if (state != null)
            {
                state.Messages.Clear();

                if (state.Name.Contains(' '))
                {
                    state.AddRuleCheckMessage(RuleChecksEnum.StateMachine04, ElementLog.LevelEnum.Error, 
                        "A state name cannot contain white spaces");
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Generated.Type obj, bool visitSubNodes)
        {
            Type type = obj as Type;

            if (type != null)
            {
                if (type is Types.StateMachine)
                {
                    // Ignore state machines
                }
                else
                {
                    if (!(type is Types.Structure) && !(type is Functions.Function))
                    {
                        if (Utils.Util.isEmpty(type.getDefault()))
                        {
                            type.AddRuleCheckMessage(RuleChecksEnum.Type01, ElementLog.LevelEnum.Error, 
                                "Types should define their default value");
                        }
                        else
                        {
                            if (type.DefaultValue == null)
                            {
                                type.AddRuleCheckMessage(RuleChecksEnum.Type02, ElementLog.LevelEnum.Error, 
                                    "Invalid default value");
                            }
                        }
                    }
                    if (type is Types.Range)
                    {
                        Types.Range range = type as Types.Range;

                        decimal val;
                        if (Decimal.TryParse(range.Default, NumberStyles.Any, CultureInfo.InvariantCulture,
                            out val))
                        {

                            if ((range.getPrecision() == acceptor.PrecisionEnum.aIntegerPrecision &&
                                 range.Default != null && range.Default.IndexOf('.') > 0)
                                ||
                                (range.getPrecision() == acceptor.PrecisionEnum.aDoublePrecision &&
                                 range.Default != null && range.Default.IndexOf('.') <= 0))
                            {
                                type.AddRuleCheckMessage(RuleChecksEnum.Type03, ElementLog.LevelEnum.Error, 
                                    "Default value's precision does not correspond to the type's precision");
                            }
                        }

                        foreach (Constants.EnumValue specValue in range.SpecialValues)
                        {
                            String value = specValue.getValue();
                            if (range.getPrecision() == acceptor.PrecisionEnum.aDoublePrecision &&
                                value.IndexOf('.') <= 0
                                ||
                                range.getPrecision() == acceptor.PrecisionEnum.aIntegerPrecision &&
                                value.IndexOf('.') > 0)
                            {
                                type.AddRuleCheckMessage(RuleChecksEnum.Type04, ElementLog.LevelEnum.Error, 
                                    "Precision of the special value + " + specValue.Name +
                                    " does not correspond to the type's precision");
                            }
                        }
                    }

                    Collection collection = type as Collection;
                    if (collection != null)
                    {
                        if (collection.getMaxSize() == 0)
                        {
                            type.AddRuleCheckMessage(RuleChecksEnum.Type05, ElementLog.LevelEnum.Error, 
                                "Collections should be upper bounded");
                        }
                    }
                }

                if (!Utils.Util.isEmpty(type.getDefault()))
                {
                    CheckExpression(type, type.getDefault());
                }
            }

            base.visit(obj, visitSubNodes);
        }


        /// <summary>
        ///     The sets of defined paragraphs
        /// </summary>
        private readonly Dictionary<string, Paragraph> _paragraphs = new Dictionary<string, Paragraph>();

        public override void visit(Generated.Paragraph obj, bool visitSubNodes)
        {
            Paragraph paragraph = obj as Paragraph;

            if (paragraph != null)
            {
                Paragraph otherParagraph;
                if (_paragraphs.TryGetValue(paragraph.Name, out otherParagraph))
                {
                    paragraph.AddRuleCheckMessage(RuleChecksEnum.Requirements03, ElementLog.LevelEnum.Error, 
                        "Duplicate paragraph id " + paragraph.Name);
                    otherParagraph.AddRuleCheckMessage(RuleChecksEnum.Requirements03, ElementLog.LevelEnum.Error, 
                        "Duplicate paragraph id " + paragraph.Name);
                }
                else
                {
                    _paragraphs.Add(paragraph.Name, paragraph);
                }

                switch (paragraph.getImplementationStatus())
                {
                    case acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented:
                        if (!paragraph.IsApplicable())
                        {
                            paragraph.AddRuleCheckMessage(RuleChecksEnum.Process08, ElementLog.LevelEnum.Warning, 
                                "Paragraph state does not correspond to implementation status (Implemented but not applicable)");
                        }
                        if (paragraph.getReviewed() == false)
                        {
                            paragraph.AddRuleCheckMessage(RuleChecksEnum.Process09, ElementLog.LevelEnum.Warning, 
                                "A non reviewed paragraph is marked as implemented");
                        }
                        break;

                    case acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA:
                    case acceptor.SPEC_IMPLEMENTED_ENUM.defaultSPEC_IMPLEMENTED_ENUM:
                        if (!paragraph.IsApplicable())
                        {
                            paragraph.AddRuleCheckMessage(RuleChecksEnum.Process10, ElementLog.LevelEnum.Warning, 
                                "Paragraph state does not correspond to implementation status (N/A but not applicable)");
                        }
                        break;

                    case acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable:
                        if (paragraph.IsApplicable())
                        {
                            paragraph.AddRuleCheckMessage(RuleChecksEnum.Process11, ElementLog.LevelEnum.Warning, 
                                "Paragraph state does not correspond to implementation status (Not implementable but applicable)");
                        }
                        break;
                }

                if (paragraph.getImplementationStatus() == acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented)
                {
                    foreach (ReqRef reqRef in ImplementedParagraphsFinder.INSTANCE.findRefs(paragraph))
                    {
                        ReqRelated model = reqRef.Enclosing as ReqRelated;
                        if (!model.ImplementationCompleted)
                        {
                            model.AddRuleCheckMessage(RuleChecksEnum.Process12, ElementLog.LevelEnum.Warning, 
                                "Requirement implementation is complete, while model element implementation is not");
                            paragraph.AddRuleCheckMessage(RuleChecksEnum.Process12, ElementLog.LevelEnum.Warning, 
                                "Requirement implementation is complete, while model element implementation is not");
                        }
                    }
                }

                RequirementSet scope = Dictionary.findRequirementSet(Dictionary.ScopeName, false);
                if (scope != null)
                {
                    bool scopeFound = false;
                    foreach (RequirementSet requirementSet in scope.SubSets)
                    {
                        if (paragraph.BelongsToRequirementSet(requirementSet))
                        {
                            scopeFound = true;
                        }

                        if ((!requirementSet.getRecursiveSelection()) &&
                            (!paragraph.BelongsToRequirementSet(requirementSet)) &&
                            paragraph.SubParagraphBelongsToRequirementSet(requirementSet))
                        {
                            paragraph.AddRuleCheckMessage(RuleChecksEnum.Process13, ElementLog.LevelEnum.Warning, 
                                "Paragraph scope should be " + requirementSet.Name +
                                ", according to its sub-paragraphs");
                        }
                    }

                    if ((!scopeFound) && (paragraph.getType() == acceptor.Paragraph_type.aREQUIREMENT))
                    {
                        paragraph.AddRuleCheckMessage(RuleChecksEnum.Process14, ElementLog.LevelEnum.Warning, 
                            "Paragraph scope not set");
                    }
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Function obj, bool visitSubNodes)
        {
            Functions.Function function = (Functions.Function) obj;

            if (function.ReturnType == null)
            {
                function.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                    "Cannot determine function return type");
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Case obj, bool visitSubNodes)
        {
            Functions.Case cas = obj as Functions.Case;

            try
            {
                Expression expression = cas.Expression;
                if (expression != null)
                {
                    expression.CheckExpression();
                    Type expressionType = cas.Expression.GetExpressionType();
                    if (expressionType != null && cas.EnclosingFunction != null &&
                        cas.EnclosingFunction.ReturnType != null)
                    {
                        if (!cas.EnclosingFunction.ReturnType.Match(expressionType))
                        {
                            cas.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                                "Expression type (" + expressionType.FullName + ") does not match function return type (" +
                                cas.EnclosingFunction.ReturnType.Name + ")");
                        }
                    }
                    else
                    {
                        cas.AddRuleCheckMessage(RuleChecksEnum.SemanticAnalysisError, ElementLog.LevelEnum.Error, 
                            "Cannot determine expression type (6) for " + cas.Expression);
                    }
                }
                else
                {
                    cas.AddRuleCheckMessage(RuleChecksEnum.SyntaxError, ElementLog.LevelEnum.Error, 
                        "Cannot evaluate expression " + cas.ExpressionText);
                }
            }
            catch (Exception e)
            {
                cas.AddException(e);
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(Enum obj, bool visitSubNodes)
        {
            Types.Enum enumeration = (Types.Enum) obj;

            List<Constants.EnumValue> valuesFound = new List<Constants.EnumValue>();
            foreach (Constants.EnumValue enumValue in enumeration.Values)
            {
                if (!string.IsNullOrEmpty(enumValue.getValue()))
                {
                    foreach (Constants.EnumValue other in valuesFound)
                    {
                        if (enumValue.getValue().CompareTo(other.getValue()) == 0)
                        {
                            enumValue.AddRuleCheckMessage(RuleChecksEnum.Type06, ElementLog.LevelEnum.Error, 
                                "Duplicated enumeration value");
                            other.AddRuleCheckMessage(RuleChecksEnum.Type06, ElementLog.LevelEnum.Error, 
                                "Duplicated enumeration value");
                        }

                        if (enumValue.LiteralName == other.LiteralName)
                        {
                            enumValue.AddRuleCheckMessage(RuleChecksEnum.Type07, ElementLog.LevelEnum.Error, 
                                "Duplicated enumeration value name");
                            other.AddRuleCheckMessage(RuleChecksEnum.Type07, ElementLog.LevelEnum.Error, 
                                "Duplicated enumeration value name");
                        }
                    }
                    valuesFound.Add(enumValue);
                }
            }

            base.visit(obj, visitSubNodes);
        }

        public override void visit(EnumValue obj, bool visitSubNodes)
        {
            Constants.EnumValue enumValue = (Constants.EnumValue) obj;

            CheckIdentifier(enumValue, enumValue.Name);

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        ///     Ensures that the identifier is correct
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        private static void CheckIdentifier(ModelElement model, string name)
        {
            if (!new Parser().IsIdentifier(name))
            {
                model.AddRuleCheckMessage(RuleChecksEnum.Naming02, ElementLog.LevelEnum.Error, 
                    "Invalid identifier");
            }
        }

        public override void visit(Range obj, bool visitSubNodes)
        {
            Types.Range range = (Types.Range) obj;

            if (range.getPrecision() == acceptor.PrecisionEnum.aIntegerPrecision)
            {
                if (range.getMinValue().IndexOf(".") >= 0)
                {
                    range.AddRuleCheckMessage(RuleChecksEnum.Type08, ElementLog.LevelEnum.Error, 
                        "Invalid min value for integer range : must be an integer");
                }

                if (range.getMaxValue().IndexOf(".") >= 0)
                {
                    range.AddRuleCheckMessage(RuleChecksEnum.Type08, ElementLog.LevelEnum.Error, 
                        "Invalid max value for integer range : must be an integer");
                }
            }
            else
            {
                if (range.getMinValue().IndexOf(".") < 0)
                {
                    range.AddRuleCheckMessage(RuleChecksEnum.Type09, ElementLog.LevelEnum.Error, 
                        "Invalid min value for float range : must have a decimal part");
                }

                if (range.getMaxValue().IndexOf(".") < 0)
                {
                    range.AddRuleCheckMessage(RuleChecksEnum.Type09, ElementLog.LevelEnum.Error, 
                        "Invalid max value for float range : must have a decimal part");
                }
            }
            try
            {
                Decimal min = range.MinValueAsLong;
            }
            catch (FormatException)
            {
                range.AddRuleCheckMessage(RuleChecksEnum.Type10, ElementLog.LevelEnum.Error, 
                    "Cannot parse min value for range");
            }

            try
            {
                Decimal max = range.MaxValueAsLong;
            }
            catch (FormatException)
            {
                range.AddRuleCheckMessage(RuleChecksEnum.Type10, ElementLog.LevelEnum.Error, 
                    "Cannot parse max value for range");
            }

            List<Constants.EnumValue> valuesFound = new List<Constants.EnumValue>();
            foreach (Constants.EnumValue enumValue in range.SpecialValues)
            {
                if (enumValue.Value == null)
                {
                    enumValue.AddRuleCheckMessage(RuleChecksEnum.Type11, ElementLog.LevelEnum.Error, 
                        "Value is not valid");
                }
                else
                {
                    foreach (Constants.EnumValue other in valuesFound)
                    {
                        if (range.CompareForEquality(enumValue.Value, other.Value))
                        {
                            enumValue.AddRuleCheckMessage(RuleChecksEnum.Type12, ElementLog.LevelEnum.Error, 
                                "Duplicate special value");
                            other.AddRuleCheckMessage(RuleChecksEnum.Type12, ElementLog.LevelEnum.Error, 
                                "Duplicate special value");
                        }

                        if (enumValue.LiteralName == other.LiteralName)
                        {
                            enumValue.AddRuleCheckMessage(RuleChecksEnum.Type12, ElementLog.LevelEnum.Error,
                                "Duplicate special value name");
                            other.AddRuleCheckMessage(RuleChecksEnum.Type12, ElementLog.LevelEnum.Error,
                                "Duplicate special value name");
                        }
                    }
                    valuesFound.Add(enumValue);
                }
            }

            base.visit(obj, visitSubNodes);
        }

        private Dictionary<string, Translation> Translations = new Dictionary<string, Translation>();

        public override void visit(Generated.Translation obj, bool visitSubNodes)
        {
            Translation translation = (Translation) obj;

            foreach (SourceText source in translation.SourceTexts)
            {
                Translation other = null;
                if (Translations.TryGetValue(source.Name, out other))
                {
                    // find the corresponding source text in the other translation
                    foreach (SourceText anotherSource in other.SourceTexts)
                    {
                        // compare comments of the source texts
                        if (anotherSource.Name == source.Name)
                        {
                            if (source.Comments.Count == anotherSource.Comments.Count)
                            {
                                bool matchFound = false;
                                foreach (SourceTextComment comment in source.Comments)
                                {
                                    foreach (SourceTextComment anotherComment in anotherSource.Comments)
                                    {
                                        if (comment.Name == anotherComment.Name)
                                        {
                                            matchFound = true;
                                            break;
                                        }
                                    }
                                    if (!matchFound)
                                        break;
                                }

                                // if a match was found for every comment or the source does not have a comment => problem
                                if (matchFound || source.Comments.Count == 0)
                                {
                                    if (translation != other)
                                    {
                                        translation.AddRuleCheckMessage(RuleChecksEnum.Translation01,
                                            ElementLog.LevelEnum.Error,
                                            "Found translation with duplicate source text " + source.Name);
                                        other.AddRuleCheckMessage(RuleChecksEnum.Translation01,
                                            ElementLog.LevelEnum.Error,
                                            "Found translation with duplicate source text " + source.Name);
                                    }
                                }
                            }
                        }
                    }
                }

                Translations[source.Name] = translation;
            }

            if (translation.Requirements.Count == 0 && string.IsNullOrEmpty(translation.Comment))
            {
                int countActions = 0;
                int countExpectations = 0;
                foreach (SubStep subStep in translation.SubSteps)
                {
                    countActions += subStep.Actions.Count;
                    countExpectations += subStep.Expectations.Count;
                }
                if (countActions == 0 && countExpectations == 0)
                {
                    translation.AddRuleCheckMessage(RuleChecksEnum.Process15, ElementLog.LevelEnum.Warning, 
                        "Empty translation which is not linked to a requirement, or does not hold any comment");
                }
            }

            base.visit(obj, visitSubNodes);
        }
    }
}