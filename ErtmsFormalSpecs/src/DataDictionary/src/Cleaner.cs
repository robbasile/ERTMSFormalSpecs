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

namespace DataDictionary
{
    using System.Collections.Generic;

    /// <summary>
    /// This class is used to clean up all string entries in the dictionary
    ///
    /// WARNING!!! 
    /// This class is generated, please refrain from altering it manually
    /// WARNING!!! 
    /// </summary>
    public class Cleaner : Generated.Visitor
    {
        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Namable obj, bool visitSubNodes)
        {
          if ( obj.getName() != null )
          {
            obj.setName(obj.getName().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.ReferencesParagraph obj, bool visitSubNodes)
        {
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.ReqRelated obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Dictionary obj, bool visitSubNodes)
        {
          if ( obj.getXsi() != null )
          {
            obj.setXsi(obj.getXsi().Trim());
          }
          if ( obj.getXsiLocation() != null )
          {
            obj.setXsiLocation(obj.getXsiLocation().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.NameSpaceRef obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.RuleCheckDisabling obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.RuleCheckIdentifier obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.NameSpace obj, bool visitSubNodes)
        {
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.ReqRef obj, bool visitSubNodes)
        {
          if ( obj.getId() != null )
          {
            obj.setId(obj.getId().Trim());
          }
          if ( obj.getSpecId() != null )
          {
            obj.setSpecId(obj.getSpecId().Trim());
          }
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Type obj, bool visitSubNodes)
        {
          if ( obj.getDefault() != null )
          {
            obj.setDefault(obj.getDefault().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Enum obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.EnumValue obj, bool visitSubNodes)
        {
          if ( obj.getValue() != null )
          {
            obj.setValue(obj.getValue().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Range obj, bool visitSubNodes)
        {
          if ( obj.getMinValue() != null )
          {
            obj.setMinValue(obj.getMinValue().Trim());
          }
          if ( obj.getMaxValue() != null )
          {
            obj.setMaxValue(obj.getMaxValue().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Structure obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.StructureRef obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.StructureElement obj, bool visitSubNodes)
        {
          if ( obj.getTypeName() != null )
          {
            obj.setTypeName(obj.getTypeName().Trim());
          }
          if ( obj.getDefault() != null )
          {
            obj.setDefault(obj.getDefault().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Collection obj, bool visitSubNodes)
        {
          if ( obj.getTypeName() != null )
          {
            obj.setTypeName(obj.getTypeName().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Function obj, bool visitSubNodes)
        {
          if ( obj.getTypeName() != null )
          {
            obj.setTypeName(obj.getTypeName().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Parameter obj, bool visitSubNodes)
        {
          if ( obj.getTypeName() != null )
          {
            obj.setTypeName(obj.getTypeName().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Case obj, bool visitSubNodes)
        {
          if ( obj.getExpression() != null )
          {
            obj.setExpression(obj.getExpression().Trim());
          }
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Procedure obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.StateMachine obj, bool visitSubNodes)
        {
          if ( obj.getInitialState() != null )
          {
            obj.setInitialState(obj.getInitialState().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.State obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Variable obj, bool visitSubNodes)
        {
          if ( obj.getTypeName() != null )
          {
            obj.setTypeName(obj.getTypeName().Trim());
          }
          if ( obj.getDefaultValue() != null )
          {
            obj.setDefaultValue(obj.getDefaultValue().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Rule obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.RuleCondition obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.PreCondition obj, bool visitSubNodes)
        {
          if ( obj.getCondition() != null )
          {
            obj.setCondition(obj.getCondition().Trim());
          }
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Action obj, bool visitSubNodes)
        {
          if ( obj.getExpression() != null )
          {
            obj.setExpression(obj.getExpression().Trim());
          }
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.FrameRef obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Frame obj, bool visitSubNodes)
        {
          if ( obj.getCycleDuration() != null )
          {
            obj.setCycleDuration(obj.getCycleDuration().Trim());
          }
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.SubSequence obj, bool visitSubNodes)
        {
          if ( obj.getD_LRBG() != null )
          {
            obj.setD_LRBG(obj.getD_LRBG().Trim());
          }
          if ( obj.getLevel() != null )
          {
            obj.setLevel(obj.getLevel().Trim());
          }
          if ( obj.getMode() != null )
          {
            obj.setMode(obj.getMode().Trim());
          }
          if ( obj.getNID_LRBG() != null )
          {
            obj.setNID_LRBG(obj.getNID_LRBG().Trim());
          }
          if ( obj.getQ_DIRLRBG() != null )
          {
            obj.setQ_DIRLRBG(obj.getQ_DIRLRBG().Trim());
          }
          if ( obj.getQ_DIRTRAIN() != null )
          {
            obj.setQ_DIRTRAIN(obj.getQ_DIRTRAIN().Trim());
          }
          if ( obj.getQ_DLRBG() != null )
          {
            obj.setQ_DLRBG(obj.getQ_DLRBG().Trim());
          }
          if ( obj.getRBC_ID() != null )
          {
            obj.setRBC_ID(obj.getRBC_ID().Trim());
          }
          if ( obj.getRBCPhone() != null )
          {
            obj.setRBCPhone(obj.getRBCPhone().Trim());
          }
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.TestCase obj, bool visitSubNodes)
        {
          if ( obj.getObsoleteComment() != null )
          {
            obj.setObsoleteComment(obj.getObsoleteComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Step obj, bool visitSubNodes)
        {
          if ( obj.getDistance() != null )
          {
            obj.setDistance(obj.getDistance().Trim());
          }
          if ( obj.getDescription() != null )
          {
            obj.setDescription(obj.getDescription().Trim());
          }
          if ( obj.getObsoleteComment() != null )
          {
            obj.setObsoleteComment(obj.getObsoleteComment().Trim());
          }
          if ( obj.getUserComment() != null )
          {
            obj.setUserComment(obj.getUserComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.SubStep obj, bool visitSubNodes)
        {
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Expectation obj, bool visitSubNodes)
        {
          if ( obj.getValue() != null )
          {
            obj.setValue(obj.getValue().Trim());
          }
          if ( obj.getCondition() != null )
          {
            obj.setCondition(obj.getCondition().Trim());
          }
          if ( obj.getComment() != null )
          {
            obj.setComment(obj.getComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.DBMessage obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.DBPacket obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.DBField obj, bool visitSubNodes)
        {
          if ( obj.getVariable() != null )
          {
            obj.setVariable(obj.getVariable().Trim());
          }
          if ( obj.getValue() != null )
          {
            obj.setValue(obj.getValue().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.TranslationDictionary obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Folder obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Translation obj, bool visitSubNodes)
        {
          if ( obj.getObsoleteComment() != null )
          {
            obj.setObsoleteComment(obj.getObsoleteComment().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.SourceText obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.SourceTextComment obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.ShortcutDictionary obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.ShortcutFolder obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Shortcut obj, bool visitSubNodes)
        {
          if ( obj.getShortcutName() != null )
          {
            obj.setShortcutName(obj.getShortcutName().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.RequirementSet obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.RequirementSetDependancy obj, bool visitSubNodes)
        {
          if ( obj.getTarget() != null )
          {
            obj.setTarget(obj.getTarget().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Specification obj, bool visitSubNodes)
        {
          if ( obj.getVersion() != null )
          {
            obj.setVersion(obj.getVersion().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.ChapterRef obj, bool visitSubNodes)
        {

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Chapter obj, bool visitSubNodes)
        {
          if ( obj.getId() != null )
          {
            obj.setId(obj.getId().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Paragraph obj, bool visitSubNodes)
        {
          if ( obj.getId() != null )
          {
            obj.setId(obj.getId().Trim());
          }
          if ( obj.getBl() != null )
          {
            obj.setBl(obj.getBl().Trim());
          }
          if ( obj.getText() != null )
          {
            obj.setText(obj.getText().Trim());
          }
          if ( obj.getVersion() != null )
          {
            obj.setVersion(obj.getVersion().Trim());
          }
          if ( obj.getObsoleteFunctionalBlockName() != null )
          {
            obj.setObsoleteFunctionalBlockName(obj.getObsoleteFunctionalBlockName().Trim());
          }
          if ( obj.getObsoleteGuid() != null )
          {
            obj.setObsoleteGuid(obj.getObsoleteGuid().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.RequirementSetReference obj, bool visitSubNodes)
        {
          if ( obj.getTarget() != null )
          {
            obj.setTarget(obj.getTarget().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Cleans all text fields in this element
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.ParagraphRevision obj, bool visitSubNodes)
        {
          if ( obj.getText() != null )
          {
            obj.setText(obj.getText().Trim());
          }
          if ( obj.getVersion() != null )
          {
            obj.setVersion(obj.getVersion().Trim());
          }

          base.visit(obj, visitSubNodes);
        }

    }
  }
