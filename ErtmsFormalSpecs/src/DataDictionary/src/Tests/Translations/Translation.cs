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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Action = DataDictionary.Rules.Action;
using Collection = DataDictionary.Types.Collection;
using DBField = DataDictionary.Tests.DBElements.DBField;
using DBMessage = DataDictionary.Tests.DBElements.DBMessage;
using DBPacket = DataDictionary.Tests.DBElements.DBPacket;
using Enum = DataDictionary.Types.Enum;
using EnumValue = DataDictionary.Constants.EnumValue;
using NameSpace = DataDictionary.Types.NameSpace;
using Range = DataDictionary.Types.Range;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;

namespace DataDictionary.Tests.Translations
{
    public class Translation : Generated.Translation, ITextualExplain, IEditable
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Translation()
            : base()
        {
        }

        /// <summary>
        ///     Provides the name of the translation
        /// </summary>
        public override string Name
        {
            get
            {
                string retVal = base.Name;

                if (Utils.Util.isEmpty(retVal) && countSourceTexts() > 0)
                {
                    retVal = getSourceTexts(0).Name;
                }

                return retVal;
            }
            set { base.Name = value; }
        }

        /// <summary>
        ///     Provides the source texts for this dictionary
        /// </summary>
        public ArrayList SourceTexts
        {
            get
            {
                if (allSourceTexts() == null)
                {
                    setAllSourceTexts(new ArrayList());
                }
                return allSourceTexts();
            }
            set { setAllSourceTexts(value); }
        }

        /// <summary>
        ///     Provides the sub-steps for this dictionary
        /// </summary>
        public ArrayList SubSteps
        {
            get
            {
                if (allSubSteps() == null)
                {
                    setAllSubSteps(new ArrayList());
                }
                return allSubSteps();
            }
            set { setAllSubSteps(value); }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Write("TRANSLATION ");
            explanation.WriteLine(Name);

            if (SourceTexts.Count > 0)
            {
                foreach (SourceText text in SourceTexts)
                {
                    text.GetExplain(explanation, explainSubElements);
                }
            }

            foreach (SubStep subStep in SubSteps)
            {
                subStep.GetExplain(explanation, explainSubElements);
            }

            explanation.WriteLine("END TRANSLATION ");
        }

        /// <summary>
        ///     The enclosing translation dictionary
        /// </summary>
        public TranslationDictionary TranslationDictionary
        {
            get { return EnclosingFinder<TranslationDictionary>.find(this); }
        }

        /// <summary>
        ///     Provides the enclosing collection
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;
                TranslationDictionary dictionary = Enclosing as TranslationDictionary;
                if (dictionary != null)
                {
                    retVal = dictionary.Translations;
                }
                else
                {
                    Folder folder = Enclosing as Folder;
                    if (folder != null)
                    {
                        retVal = folder.Translations;
                    }
                }
                return retVal;
            }
        }


        /// <summary>
        ///     Updates the step according to this translation
        /// </summary>
        /// <param name="step"></param>
        public void UpdateStep(Step step)
        {
            Step previousStep = step.PreviousStep;

            foreach (ReqRef reqRef in Requirements)
            {
                if (!IsRequirementPresent(step, reqRef))
                {
                    step.appendRequirements((ReqRef) reqRef.Duplicate());
                }
            }

            int subStepCounter = 1;
            foreach (SubStep subStep in SubSteps)
            {
                bool addSubStep = true;

                if (subStep.ReferencesMessages())
                {
                    addSubStep = step.Messages.Count > 0;
                }

                if (addSubStep)
                {
                    SubStep newSubStep = (SubStep) acceptor.getFactory().createSubStep();
                    newSubStep.setSkipEngine(subStep.getSkipEngine());
                    newSubStep.Comment = subStep.Comment;
                    newSubStep.Name = subStep.Name;
                    step.appendSubSteps(newSubStep);

                    if (previousStep != null && previousStep.Distance != step.Distance && subStepCounter == 1)
                    {
                        Action newAct = (Action) acceptor.getFactory().createAction();
                        string distance = step.getDistance();
                        if (!distance.Contains("."))
                        {
                            distance = distance + ".0";
                        }
                        newAct.ExpressionText = "OdometryInterface.UpdateDistance ( " + distance + " )";
                        newSubStep.setSkipEngine(false);
                        newSubStep.appendActions(newAct);
                    }

                    foreach (Action action in subStep.Actions)
                    {
                        Action newAct = (Action) action.Duplicate();
                        newSubStep.appendActions(newAct);
                        Review(newAct);
                    }

                    foreach (Expectation expectation in subStep.Expectations)
                    {
                        Expectation newExp = (Expectation) expectation.Duplicate();
                        newSubStep.appendExpectations(newExp);
                        Review(newExp);
                    }
                }
                subStepCounter++;
            }
        }

        /// <summary>
        ///     Indicates that the requirement is already present in the step
        /// </summary>
        /// <param name="step"></param>
        /// <param name="reqRef"></param>
        /// <returns></returns>
        private bool IsRequirementPresent(Step step, ReqRef reqRef)
        {
            bool retVal = false;

            foreach (ReqRef stepReqRef in step.Requirements)
            {
                if (reqRef.Paragraph == stepReqRef.Paragraph)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }


        public string getSourceTextExplain()
        {
            string result = "";
            string prefix = "";
            int textCount = 1;

            foreach (SourceText sourceText in SourceTexts)
            {
                if (SourceTexts.Count > 1)
                {
                    prefix = textCount + ". ";
                }
                else
                {
                    prefix = "";
                }
                result += prefix + sourceText.Name + "\n";
                textCount++;
            }

            return result;
        }

        /// <summary>
        ///     Review the expressions associated to this expectation
        /// </summary>
        /// <param name="expectation"></param>
        private void Review(Expectation expectation)
        {
            expectation.Value = ReviewExpression(expectation.Step, expectation.Value);
        }

        /// <summary>
        ///     Review the expressions associated to this action
        /// </summary>
        /// <param name="action"></param>
        private void Review(Action action)
        {
            try
            {
                action.ExpressionText = ReviewExpression(action.Step, action.ExpressionText);
            }
            catch (Exception e)
            {
                action.AddException(e);
            }
        }

        /// <summary>
        ///     Updates an expression according to translation rules
        /// </summary>
        /// <param name="step">the step in which the expression occurs</param>
        /// <param name="expression"></param>
        /// <returns>the updated string</returns>
        private string ReviewExpression(Step step, string expression)
        {
            string retVal = expression;

            if (expression.IndexOf('%') >= 0)
            {
                SubSequence subSequence = step.TestCase.SubSequence;

                retVal = retVal.Replace("%D_LRBG", format_decimal_as_str(subSequence.getD_LRBG()));
                retVal = retVal.Replace("%Level", format_level(subSequence.getLevel()));
                retVal = retVal.Replace("%Mode", format_mode(subSequence.getMode()));
                retVal = retVal.Replace("%NID_LRBG", format_decimal_as_str(subSequence.getNID_LRBG()));
                retVal = retVal.Replace("%Q_DIRLRBG", format_decimal_as_str(subSequence.getQ_DIRLRBG()));
                retVal = retVal.Replace("%Q_DIRTRAIN", format_decimal_as_str(subSequence.getQ_DIRTRAIN()));
                retVal = retVal.Replace("%Q_DLRBG", format_decimal_as_str(subSequence.getQ_DLRBG()));
                retVal = retVal.Replace("%RBC_ID", format_decimal_as_str(subSequence.getRBC_ID()));
                retVal = retVal.Replace("%RBCPhone", format_str(subSequence.getRBCPhone()));

                retVal = retVal.Replace("%Step_Distance", step.getDistance() + "");
                retVal = retVal.Replace("%Step_LevelIN", format_level(step.getLevelIN()));
                retVal = retVal.Replace("%Step_LevelOUT", format_level(step.getLevelOUT()));
                retVal = retVal.Replace("%Step_ModeIN", format_mode(step.getModeOUT()));
                retVal = retVal.Replace("%Step_ModeOUT", format_mode(step.getModeOUT()));

                int max_step_messages = 8;
                for (int i = 0; i < max_step_messages; i++)
                {
                    if (retVal.IndexOf("%Step_Messages_" + i) >= 0)
                    {
                        if (step.StepMessages.Count > i)
                        {
                            DBMessage message = step.StepMessages[i] as DBMessage;
                            if (message != null)
                            {
                                retVal = retVal.Replace("%Step_Messages_" + i, format_message(message));
                            }
                        }
                        else
                        {
                            retVal = retVal.Replace("%Step_Messages_" + i, format_default_message(expression));
                        }
                    }
                }

                if (retVal.IndexOf("%") > 0)
                {
                    step.AddError("Cannot completely translate this step");
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Takes the string provided and returns the corresponding Mode enum
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string format_mode(string str)
        {
            string retVal = "";

            str = format_str(str);
            int val = format_decimal(str);
            if (val != -1)
            {
                switch (val)
                {
                    case 6:
                        retVal = "ModeEnum.SB";
                        break;
                    default:
                        retVal = "ModeEnum.Unknown";
                        break;
                }
            }
            else
            {
                retVal = "ModeEnum." + str;
            }

            return retVal;
        }

        /// <summary>
        ///     Takes the enum provided and returns the corresponding Mode enum
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string format_mode(acceptor.ST_MODE mode)
        {
            string retVal = "";

            switch (mode)
            {
                case acceptor.ST_MODE.Mode_FS:
                    retVal = "ModeEnum.FS";
                    break;
                case acceptor.ST_MODE.Mode_IS:
                    retVal = "ModeEnum.IS";
                    break;
                case acceptor.ST_MODE.Mode_LS:
                    retVal = "ModeEnum.LS";
                    break;
                case acceptor.ST_MODE.Mode_NA:
                    retVal = "ModeEnum.NA";
                    break;
                case acceptor.ST_MODE.Mode_NL:
                    retVal = "ModeEnum.NL";
                    break;
                case acceptor.ST_MODE.Mode_NP:
                    retVal = "ModeEnum.NP";
                    break;
                case acceptor.ST_MODE.Mode_OS:
                    retVal = "ModeEnum.OS";
                    break;
                case acceptor.ST_MODE.Mode_PSH:
                    retVal = "ModeEnum.PSH";
                    break;
                case acceptor.ST_MODE.Mode_PT:
                    retVal = "ModeEnum.PT";
                    break;
                case acceptor.ST_MODE.Mode_RE:
                    retVal = "ModeEnum.RE";
                    break;
                case acceptor.ST_MODE.Mode_SB:
                    retVal = "ModeEnum.SB";
                    break;
                case acceptor.ST_MODE.Mode_SF:
                    retVal = "ModeEnum.SF";
                    break;
                case acceptor.ST_MODE.Mode_SH:
                    retVal = "ModeEnum.SH";
                    break;
                case acceptor.ST_MODE.Mode_SL:
                    retVal = "ModeEnum.SL";
                    break;
                case acceptor.ST_MODE.Mode_SN:
                    retVal = "ModeEnum.SN";
                    break;
                case acceptor.ST_MODE.Mode_SR:
                    retVal = "ModeEnum.SR";
                    break;
                case acceptor.ST_MODE.Mode_TR:
                    retVal = "ModeEnum.TR";
                    break;
                case acceptor.ST_MODE.Mode_UN:
                    retVal = "ModeEnum.UF";
                    break;
                default:
                    retVal = "ModeEnum.Unknown";
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Takes the message provided and returns the corresponding message value
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string format_message(DBMessage message)
        {
            string retVal = "";
            switch (message.MessageType)
            {
                case acceptor.DBMessageType.aEUROBALISE:
                    retVal = format_eurobalise_message(message);
                    break;
                case acceptor.DBMessageType.aEUROLOOP:
                    retVal = format_euroloop_message(message);
                    break;
                case acceptor.DBMessageType.aEURORADIO:
                    retVal = format_euroradio_message(message);
                    break;
            }
            return retVal;
        }


        public string format_default_message(string expression)
        {
            string retVal = "<not a structure type>";

            int index = expression.IndexOf("<-");
            if (index > 0)
            {
                string variableText = expression.Substring(0, index).Trim();
                Expression expressionTree = new Parser().Expression(Dictionary, variableText);
                if (expressionTree != null)
                {
                    Structure structureType = expressionTree.GetExpressionType() as Structure;
                    retVal = structureType.DefaultValue.LiteralName;
                }
            }

            return retVal;
        }

        private static NameSpace findNameSpace(string name)
        {
            NameSpace retVal = null;

            EfsSystem system = EfsSystem.Instance;
            foreach (Dictionary dictionary in system.Dictionaries)
            {
                retVal = OverallNameSpaceFinder.INSTANCE.findByName(dictionary, name);
                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }

        private static string format_eurobalise_message(DBMessage message)
        {
            EfsSystem system = EfsSystem.Instance;

            NameSpace nameSpace = findNameSpace("Messages.EUROBALISE");
            Structure structureType = (Structure) system.FindType(nameSpace, "Message");
            StructureValue structure = new StructureValue(structureType);

            int currentIndex = 0;
            FillStructure(nameSpace, message.Fields, ref currentIndex, structure); // fills the message fields


            // then we fill the packets
            IVariable subSequenceVariable;
            if (structure.SubVariables.TryGetValue("Sequence1", out subSequenceVariable))
            {
                subSequenceVariable.Value = get_message_packets(message, nameSpace, system);
            }
            else
            {
                throw new Exception("Cannot find SubSequence in variable");
            }

            return structure.Name;
        }


        private static ListValue get_message_packets(DBMessage message, NameSpace nameSpace, EfsSystem system)
        {
            ListValue retVal;

            Collection collectionType = (Collection) system.FindType(nameSpace, "Collection1");
            Structure subStructure1Type = (Structure) system.FindType(nameSpace, "SubStructure1");

            string packetLocation = "Messages.PACKET.";
            if (nameSpace.FullName.Contains("TRAIN_TO_TRACK"))
            {
                packetLocation += "TRAIN_TO_TRACK.Message";
            }
            else
            {
                packetLocation += "TRACK_TO_TRAIN.Message";
            }

            Structure packetStructureType = (Structure) system.FindType(nameSpace, packetLocation);

            retVal = new ListValue(collectionType, new List<IValue>());

            foreach (DBPacket packet in message.Packets)
            {
                DBField nidPacketField = packet.Fields[0] as DBField;
                if (nidPacketField.Value != "255") // 255 means "end of information"
                {
                    int packetId = int.Parse(nidPacketField.Value);
                    StructureValue subStructure = FindStructure(packetId);

                    int currentIndex = 0;
                    FillStructure(nameSpace, packet.Fields, ref currentIndex, subStructure);

                    StructureValue subStructure1 = new StructureValue(subStructure1Type);

                    // For Balise messages, we have an extra level of information to fill, so here we define StructureVal in one of two ways
                    StructureValue structureVal;
                    if (subStructure1.SubVariables.Count == 1 &&
                        subStructure1.SubVariables.ContainsKey("TRACK_TO_TRAIN"))
                    {
                        // For a Balise message, we have an extra level of structures for TRACK_TO_TRAIN
                        structureVal = new StructureValue(packetStructureType);

                        subStructure1.SubVariables["TRACK_TO_TRAIN"].Value = structureVal;
                    }
                    else
                    {
                        // For RBC, the collection directly holds the different packet types
                        structureVal = subStructure1;
                    }

                    // Find the right variable in the packet to add the structure we just created
                    foreach (KeyValuePair<string, IVariable> pair in structureVal.SubVariables)
                    {
                        string variableName = pair.Key;
                        if (subStructure.Structure.FullName.Contains(variableName))
                        {
                            pair.Value.Value = subStructure;

                            retVal.Val.Add(subStructure1);

                            break;
                        }
                    }
                }
            }

            return retVal;
        }


        /// <summary>
        ///     Finds the type of the structure corresponding to the provided NID_PACKET
        /// </summary>
        /// <param name="nameSpace">The namespace where the type has to be found</param>
        /// <param name="nidPacket">The id of the packet</param>
        /// <returns></returns>
        private static StructureValue FindStructure(int nidPacket)
        {
            EfsSystem system = EfsSystem.Instance;
            Structure structure = null;
            NameSpace nameSpace = findNameSpace("Messages.PACKET.TRACK_TO_TRAIN");
            foreach (NameSpace packetNameSpace in nameSpace.NameSpaces)
            {
                Structure structureType =
                    (Structure) system.FindType(packetNameSpace, packetNameSpace.FullName + ".Message");
                StructureValue structureValue = new StructureValue(structureType);

                foreach (KeyValuePair<string, IVariable> pair in structureValue.SubVariables)
                {
                    string variableName = pair.Key;
                    if (variableName.Equals("NID_PACKET"))
                    {
                        IntValue value = pair.Value.Value as IntValue;
                        if (value.Val == nidPacket)
                        {
                            structure = structureType;
                        }
                    }
                    if (structure != null)
                    {
                        break;
                    }
                }
                if (structure != null)
                {
                    break;
                }
            }

            StructureValue retVal = null;
            if (structure != null)
            {
                retVal = new StructureValue(structure);
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the given structure with the values provided from the database
        /// </summary>
        /// <param name="aNameSpace">Namespace of the structure</param>
        /// <param name="fields">Fields to be copied into the structure</param>
        /// <param name="index">Index (of fields list) from which we have to start copying</param>
        /// <param name="aStructure">The structure to be filled</param>
        private static void FillStructure(NameSpace aNameSpace, ArrayList fields, ref int currentIndex,
            StructureValue aStructure)
        {
            EfsSystem system = EfsSystem.Instance;

            int j = 0;
            while (currentIndex < fields.Count)
            {
                DBField field = fields[currentIndex] as DBField;

                KeyValuePair<string, IVariable> pair = aStructure.SubVariables.ElementAt(j);
                IVariable variable = pair.Value;

                // conditional variables can be missing in the database fields, but present in our structure => skip them
                while (!variable.Name.StartsWith(field.Variable) && j < aStructure.SubVariables.Count - 1)
                {
                    j++;
                    pair = aStructure.SubVariables.ElementAt(j);
                    variable = pair.Value;
                }

                // We use StartsWith and not Equals because we can have N_ITER_1 and N_ITER
                if (variable.Name.StartsWith(field.Variable))
                {
                    if (variable.Type is Enum)
                    {
                        Enum type = variable.Type as Enum;
                        foreach (EnumValue enumValue in type.Values)
                        {
                            int value = int.Parse(enumValue.getValue());
                            int other = int.Parse(field.Value);
                            if (value == other)
                            {
                                variable.Value = enumValue;
                                j++;
                                break;
                            }
                        }
                    }
                    else if (variable.Type is Range)
                    {
                        Range type = variable.Type as Range;
                        object v = VariableConverter.INSTANCE.Convert(variable.Name, field.Value);
                        variable.Value = new IntValue(type, (int) v);
                        j++;
                    }
                    else if (variable.Type is StringType)
                    {
                        StringType type = variable.Type as StringType;
                        variable.Value = new StringValue(type, field.Value);
                        j++;
                    }
                    else
                    {
                        throw new Exception("Unhandled variable type");
                    }

                    if (variable.Name.StartsWith("N_ITER")) // we have to create a sequence
                    {
                        KeyValuePair<string, IVariable> sequencePair = aStructure.SubVariables.ElementAt(j);
                        IVariable sequenceVariable = sequencePair.Value;
                        Collection collectionType = (Collection) system.FindType(aNameSpace, sequenceVariable.TypeName);
                        ListValue sequence = new ListValue(collectionType, new List<IValue>());

                        int value = int.Parse(field.Value);
                        for (int k = 0; k < value; k++)
                        {
                            currentIndex++;
                            Structure structureType =
                                (Structure) system.FindType(aNameSpace, sequence.CollectionType.Type.FullName);
                            StructureValue structureValue = new StructureValue(structureType);
                            FillStructure(aNameSpace, fields, ref currentIndex, structureValue);
                            sequence.Val.Add(structureValue);
                        }
                        sequenceVariable.Value = sequence;
                        j++;
                    }
                }

                // Special case for X_TEXT
                if ("Sequence1".Equals(variable.Name) && "X_TEXT".Equals(field.Variable))
                {
                    KeyValuePair<string, IVariable> sequencePair = aStructure.SubVariables.ElementAt(j);
                    IVariable sequenceVariable = sequencePair.Value;
                    Collection collectionType = (Collection)system.FindType(aNameSpace, sequenceVariable.TypeName);
                    ListValue sequence = new ListValue(collectionType, new List<IValue>());
                    while (field != null && "X_TEXT".Equals(field.Variable))
                    {
                        if (string.IsNullOrEmpty(field.Value))
                        {
                            field.Value = " ";
                        }
                        Structure structureType =
                            (Structure)system.FindType(aNameSpace, sequence.CollectionType.Type.FullName);
                        StructureValue structureValue = new StructureValue(structureType);
                        FillStructure(aNameSpace, fields, ref currentIndex, structureValue);
                        sequence.Val.Add(structureValue);
                        currentIndex += 1;
                        if ( currentIndex < fields.Count)
                        {
                            field = fields[currentIndex] as DBField;
                        }
                        else
                        {
                            field = null;
                        }
                    }
                    sequenceVariable.Value = sequence;
                    j++;
                }

                // if all the fields of the structue are filled, we terminated
                if (j == aStructure.SubVariables.Count)
                {
                    break;
                }
                else
                {
                    currentIndex += 1;
                }
            }
        }


        private static string format_euroloop_message(DBMessage message)
        {
            EfsSystem system = EfsSystem.Instance;

            NameSpace nameSpace = findNameSpace("Messages.EUROLOOP");
            Structure structureType = (Structure) system.FindType(nameSpace, "Message");
            StructureValue structure = new StructureValue(structureType);

            int currentIndex = 0;
            FillStructure(nameSpace, message.Fields, ref currentIndex, structure);


            // then we fill the packets
            IVariable subSequenceVariable;
            if (structure.SubVariables.TryGetValue("Sequence1", out subSequenceVariable))
            {
                subSequenceVariable.Value = get_message_packets(message, nameSpace, system);
            }
            else
            {
                throw new Exception("Cannot find SubSequence in variable");
            }

            return structure.Name;
        }

        private static string format_euroradio_message(DBMessage message)
        {
            EfsSystem system = EfsSystem.Instance;

            NameSpace rbcRoot = findNameSpace("Messages.MESSAGE");

            // Get the EFS namespace corresponding to the message 
            // Select the appropriate message type, tracktotrain or traintotrack
            DBField nidMessage = message.Fields[0] as DBField;
            string msg_id = get_namespace_from_ID(nidMessage.Value);

            NameSpace nameSpace = OverallNameSpaceFinder.INSTANCE.findByName(rbcRoot, msg_id);

            if (nameSpace == null)
            {
                throw new Exception("Message type not found in EFS");
            }

            // The EURORADIO messages are defined in the namespaces TRACK_TO_TRAIN and TRAIN_TO_TRACK, which enclose the specific message namespaces
            // So we get the message type from nameSpace.EnclosingNameSpace and the actual structure corresponding to the message in nameSpace
            Structure enclosingStructureType = (Structure) system.FindType(nameSpace.EnclosingNameSpace, "Message");
            StructureValue Message = new StructureValue(enclosingStructureType);


            // Within the message, get the appropriate field and get that structure
            Structure structureType = (Structure) system.FindType(nameSpace, "Message");
            StructureValue structure = new StructureValue(structureType);


            // Fill the structure
            int currentIndex = 0;
            FillStructure(nameSpace, message.Fields, ref currentIndex, structure);

            // Fill the default packets
            int translatedPackets = 0;
            foreach (KeyValuePair<string, IVariable> subVariable in structure.SubVariables)
            {
                if (subVariable.Value.TypeName.EndsWith("Message"))
                {
                    // The structure of packets will always be a Message, but in some cases, it is a message that contains
                    // the different options for a single field in the message
                    structure.GetVariable(subVariable.Value.Name).Value = FillDefaultPacket(message, subVariable.Value);

                    translatedPackets++;
                }
            }

            // and fill the packets
            IVariable subSequenceVariable;
            if (structure.SubVariables.TryGetValue("Sequence1", out subSequenceVariable)
                && message.Packets.Count > translatedPackets)
            {
                subSequenceVariable.Value = get_message_packets(message, nameSpace, system);
            }


            // Fill the correct field in Message with the structure.
            foreach (KeyValuePair<string, IVariable> pair in Message.SubVariables)
            {
                if (msg_id.EndsWith(pair.Key))
                {
                    pair.Value.Type = structureType;
                    pair.Value.Value = structure;
                }
            }

            return Message.Name;
        }

        private static IValue FillDefaultPacket(DBMessage message, IVariable structure)
        {
            IValue retVal = structure.Value;

            if (isPacket(structure))
            {
                Structure defaultPacketType = (Structure) structure.Type;
                StructureValue defaultPacket = new StructureValue(defaultPacketType);

                NameSpace packetNameSpace = structure.NameSpace;

                foreach (DBPacket packet in message.Packets)
                {
                    DBField nidPacketField = packet.Fields[0] as DBField;
                    int packetID = int.Parse(nidPacketField.Value);

                    Structure packetType = (Structure) FindStructure(packetID).Type;

                    if (packetType == defaultPacketType)
                    {
                        int defaultPacketIndex = 0;
                        FillStructure(packetNameSpace, packet.Fields, ref defaultPacketIndex, defaultPacket);

                        retVal = defaultPacket;
                    }
                }
            }
            else
            {
                Structure structureType = structure.Type as Structure;
                StructureValue Structure = new StructureValue(structureType);
                if (Structure != null)
                {
                    foreach (KeyValuePair<string, IVariable> subVariable in Structure.SubVariables)
                    {
                        if (isPacket(subVariable.Value))
                        {
                            Structure defaultPacketType = (Structure) subVariable.Value.Type;
                            StructureValue defaultPacket = new StructureValue(defaultPacketType);

                            NameSpace packetNameSpace = subVariable.Value.NameSpace;

                            foreach (DBPacket packet in message.Packets)
                            {
                                DBField nidPacketField = packet.Fields[0] as DBField;
                                int packetID = int.Parse(nidPacketField.Value);

                                Structure packetType = (Structure) FindStructure(packetID).Type;

                                if (packetType == defaultPacketType)
                                {
                                    int defaultPacketIndex = 0;
                                    FillStructure(packetNameSpace, packet.Fields, ref defaultPacketIndex, defaultPacket);

                                    Structure.GetVariable(subVariable.Value.Name).Value = defaultPacket;
                                }
                            }
                        }
                    }
                    retVal = Structure;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Determines whether a EFS structure corresponds to a packet
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        private static bool isPacket(IVariable structure)
        {
            bool retVal = false;

            foreach (ModelElement element in structure.Type.SubElements)
            {
                StructureElement subElement = element as StructureElement;
                if (subElement != null)
                {
                    if (subElement.Name == "NID_PACKET")
                    {
                        retVal = true;
                    }
                }
            }

            return retVal;
        }


        /// <summary>
        ///     Takes the string provided and returns the corresponding RBC message
        /// </summary>
        /// <param name="NID_MESSAGE"></param>
        /// <returns></returns>
        private static string get_namespace_from_ID(string str)
        {
            string retVal = "";
            if (format_decimal(str) < 100)
            {
                retVal = "TRACK_TO_TRAIN.";
            }
            else
            {
                retVal = "TRAIN_TO_TRACK.";
            }
            switch (format_decimal(str))
            {
                case 2:
                    retVal += "SR_AUTHORISATION";
                    break;
                case 3:
                    retVal += "MOVEMENT_AUTHORITY";
                    break;
                case 6:
                    retVal += "RECOGNITION_OF_EXIT_FROM_TRIP_MODE";
                    break;
                case 8:
                    retVal += "ACKNOWLEDGEMENT_OF_TRAIN_DATA";
                    break;
                case 9:
                    retVal += "REQUEST_TO_SHORTEN_MA";
                    break;
                case 15:
                    retVal += "CONDITIONAL_EMERGENCY_STOP";
                    break;
                case 16:
                    retVal += "UNCONDITIONAL_EMERGENCY_STOP";
                    break;
                case 18:
                    retVal += "REVOCATION_OF_EMERGENCY_STOP";
                    break;
                case 24:
                    retVal += "GENERAL_MESSAGE";
                    break;
                case 27:
                    retVal += "SH_REFUSED";
                    break;
                case 28:
                    retVal += "SH_AUTHORISED";
                    break;
                case 32:
                    retVal += "RBC.RIU_SYSTEM_VERSION";
                    break;
                case 33:
                    retVal += "MA_WITH_SHIFTED_LOCATION_REFERENCE";
                    break;
                case 34:
                    retVal += "TRACK_AHEAD_FREE_REQUEST";
                    break;
                case 37:
                    retVal += "INFILL_MA";
                    break;
                case 38:
                    retVal += "INITIATION_OF_A_COMMUNICATION_SESSION";
                    break;
                case 39:
                    retVal += "ACKNOWLEDGEMENT_OF_TERMINATION_OF_A_COMMUNICATION_SESSION";
                    break;
                case 40:
                    retVal += "TRAIN_REJECTED";
                    break;
                case 41:
                    retVal += "TRAIN_ACCEPTED";
                    break;
                case 43:
                    retVal += "SOM_POSITION_REPORT_CONFIRMED_BY_RBC";
                    break;
                case 45:
                    retVal += "ASSIGNMENT_OF_COORDINATE_SYSTEM";
                    break;
                case 129:
                    retVal += "VALIDATED_TRAIN_DATA";
                    break;
                case 130:
                    retVal += "REQUEST_FOR_SHUNTING";
                    break;
                case 132:
                    retVal += "MA_REQUEST";
                    break;
                case 136:
                    retVal += "TRAIN_POSITION_REPORT";
                    break;
                case 137:
                    retVal += "REQUEST_TO_SHORTEN_MA_IS_GRANTED";
                    break;
                case 138:
                    retVal += "REQUEST_TO_SHORTEN_MA_IS_REJECTED";
                    break;
                case 146:
                    retVal += "ACKNOWLEDGEMENT";
                    break;
                case 147:
                    retVal += "ACKNOWLEDGEMENT_OF_EMERGENCY_STOP";
                    break;
                case 149:
                    retVal += "TRACK_AHEAD_FREE_GRANTED";
                    break;
                case 150:
                    retVal += "END_OF_MISSION";
                    break;
                case 153:
                    retVal += "RADIO_INFILL_REQUEST";
                    break;
                case 154:
                    retVal += "NO_COMPATIBLE_VERSION_SUPPORTED";
                    break;
                case 155:
                    retVal += "INITIATION_OF_A_COMMUNICATION_SESSION";
                    break;
                case 156:
                    retVal += "TERMINATION_OF_A_COMMUNICATION_SESSION";
                    break;
                case 157:
                    retVal += "SOM_POSITION_REPORT";
                    break;
                case 158:
                    retVal += "TEXT_MESSAGE_ACKNOWLEDGED_BY_DRIVER";
                    break;
                case 159:
                    retVal += "SESSION_ESTABLISHED";
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Takes the string provided and returns the corresponding Level enum
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string format_level(string str)
        {
            string retVal = "";

            switch (format_decimal(str))
            {
                case 0:
                    retVal = "LevelEnum.L0";
                    break;
                case 1:
                    retVal = "LevelEnum.L1";
                    break;
                case 2:
                    retVal = "LevelEnum.LNTC";
                    break;
                case 3:
                    retVal = "LevelEnum.L2";
                    break;
                case 4:
                    retVal = "LevelEnum.L3";
                    break;
                default:
                    retVal = "LevelEnum." + str;
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Takes the string provided and returns the corresponding Level enum
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string format_level(acceptor.ST_LEVEL level)
        {
            string retVal = "";

            switch (level)
            {
                case acceptor.ST_LEVEL.StLevel_L0:
                    retVal = "LevelEnum.L0";
                    break;
                case acceptor.ST_LEVEL.StLevel_L1:
                    retVal = "LevelEnum.L1";
                    break;
                case acceptor.ST_LEVEL.StLevel_LNTC:
                    retVal = "LevelEnum.LNTC";
                    break;
                case acceptor.ST_LEVEL.StLevel_L2:
                    retVal = "LevelEnum.L2";
                    break;
                case acceptor.ST_LEVEL.StLevel_L3:
                    retVal = "LevelEnum.L3";
                    break;
                default:
                    retVal = "LevelEnum.Unknown";
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Takes the string provided and returns the formatted string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string format_str(string str)
        {
            if (str == null)
            {
                str = "";
            }

            int i, j = 0;

            i = str.IndexOf("(");
            if (i >= 0)
            {
                j = str.IndexOf(")", i);
            }

            while (i >= 0 && j >= 0)
            {
                str = str.Substring(0, i) + str.Substring(j + 1);

                i = str.IndexOf("(");
                if (i >= 0)
                {
                    j = str.IndexOf(")", i);
                }
            }

            return str.Trim();
        }

        /// <summary>
        ///     Takes the string provided and returns the corresponding decimal value
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int format_decimal(string str)
        {
            int retVal = -1;

            str = format_str(str);
            if (str.EndsWith("d"))
            {
                str = str.Substring(0, str.Length - 1).Trim();
                try
                {
                    retVal = int.Parse(str);
                }
                catch (FormatException)
                {
                }
            }
            else if (str.EndsWith("b"))
            {
                str = str.Substring(0, str.Length - 1).Trim();
                char[] tmp = str.ToCharArray();
                retVal = 0;
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i] == '0')
                    {
                        retVal = retVal*2;
                    }
                    else if (tmp[i] == '1')
                    {
                        retVal = retVal*2 + 1;
                    }
                }
            }
            else
            {
                try
                {
                    retVal = int.Parse(str);
                }
                catch (FormatException)
                {
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Takes the string provided and returns the corresponding decimal value, as a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string format_decimal_as_str(string str)
        {
            return "" + format_decimal(str);
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            SubStep subStep = element as SubStep;
            if (subStep != null)
            {
                appendSubSteps(subStep);
            }
        }

        /// <summary>
        /// The text to edit
        /// </summary>
        public string Text { get { return getName(); } set { setName(value); } }

        /// <summary>
        /// No syntax highlighting for this
        /// </summary>
        public bool SyntaxHightlight { get { return false; } }

        /// <summary>
        /// Editable window title
        /// </summary>
        public string Title { get { return "Translation"; } }    

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <param name="sourceText"></param>
        /// <returns></returns>
        public static Translation CreateDefault(ICollection enclosingCollection, SourceText sourceText)
        {
            Translation retVal = (Translation)acceptor.getFactory().createTranslation();

            Util.DontNotify(() =>
            {
                if (sourceText != null)
                {
                    retVal.Name = sourceText.Name;
                    retVal.appendSourceTexts(sourceText);
                }
                else
                {
                    retVal.Name = "Translation" + GetElementNumber(enclosingCollection);                    
                }

                retVal.appendSubSteps(SubStep.CreateDefault(retVal.SubSteps));
            });

            return retVal;
        }
    }
}