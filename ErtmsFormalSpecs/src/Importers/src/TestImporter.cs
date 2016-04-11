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
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using DataDictionary.Generated;
using DataDictionary.Tests.Translations;
using DBField = DataDictionary.Tests.DBElements.DBField;
using DBMessage = DataDictionary.Tests.DBElements.DBMessage;
using DBPacket = DataDictionary.Tests.DBElements.DBPacket;
using Frame = DataDictionary.Tests.Frame;
using Step = DataDictionary.Tests.Step;
using SubSequence = DataDictionary.Tests.SubSequence;
using TestCase = DataDictionary.Tests.TestCase;

namespace Importers
{
    public class TestImporter
    {
        /// <summary>
        ///     The path to the access database
        /// </summary>
        private string _filePath;

        /// <summary>
        ///     The path to the access database
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            private set { _filePath = value.Replace("\\", "/"); }
        }

        /// <summary>
        ///     The password used to access the database, if any
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        ///     The connection to the database
        /// </summary>
        private OleDbConnection _connection;

        /// <summary>
        ///     The connection to the database
        /// </summary>
        public OleDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new OleDbConnection
                    {
                        ConnectionString = "PROVIDER=Microsoft.Jet.OLEDB.4.0; Data Source=" + FilePath +
                                           ";Jet OLEDB:Database Password=" + Password + ";"
                    };
                    _connection.Open();
                }
                return _connection;
            }
            private set
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
                _connection = value;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="filePath">The path to the file to import</param>
        /// <param name="password">The password used to access the database</param>
        public TestImporter(string filePath, string password)
        {
            FilePath = filePath;
            Password = password;
        }

        /// <summary>
        ///     Imports the database into the corresponding frame by creating a new subsequence
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="keepManualTranslations">Indicates that manual translation for be kept during import</param>
        public void Import(Frame frame, bool keepManualTranslations)
        {
            try
            {
                ImportSubSequence(frame, keepManualTranslations);
            }
            finally
            {
                Connection = null;
            }
        }

        /// <summary>
        ///     Imports the subsequence stored in the database
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="keepManualTranslations">Indicates that manual translation for be kept during import</param>
        private void ImportSubSequence(Frame frame, bool keepManualTranslations)
        {
            const string sql = "SELECT TestSequenceID, TestSequenceName FROM TSW_TestSequence";

            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, Connection);
            DataSet dataSet = new DataSet();

            adapter.Fill(dataSet);

            if (dataSet.Tables.Count > 0)
            {
                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    int subSequenceId = (int) dataRow.ItemArray.GetValue(0);
                    string subSequenceName = (string) dataRow.ItemArray.GetValue(1);

                    SubSequence newSubSequence = (SubSequence) acceptor.getFactory().createSubSequence();
                    newSubSequence.Name = subSequenceName;
                    ImportInitialValues(newSubSequence, subSequenceId);
                    ImportSteps(newSubSequence);
                    newSubSequence.setCompleted(false);

                    SubSequence previousSubSequence = frame.findSubSequence(subSequenceName);
                    if (previousSubSequence != null)
                    {
                        newSubSequence.setGuid(previousSubSequence.getGuid());
                        newSubSequence.setCompleted(previousSubSequence.getCompleted());
                        int cnt = 0;
                        foreach (TestCase previousTestCase in previousSubSequence.TestCases)
                        {
                            if (cnt < newSubSequence.TestCases.Count)
                            {
                                TestCase newTestCase = newSubSequence.TestCases[cnt] as TestCase;
                                if (newTestCase != null)
                                {
                                    if (previousTestCase.Name.Equals(newTestCase.Name))
                                    {
                                        newTestCase.Merge(previousTestCase, keepManualTranslations);
                                    }
                                    else
                                    {
                                        throw new Exception(newTestCase.FullName + " is found instead of " +
                                                            previousTestCase.FullName + " while importing sub-sequence " +
                                                            newSubSequence.FullName);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("The test case " + previousTestCase.FullName +
                                                    " is not present in the new data base");
                            }
                            cnt++;
                        }

                        previousSubSequence.Delete();
                    }

                    frame.appendSubSequences(newSubSequence);
                }
            }
            else
            {
                throw new Exception("Cannot find table TSW_TestSequence in database");     
            }
        }

        /// <summary>
        ///     Imports the subsequence stored in the database
        /// </summary>
        /// <param name="subSequence"></param>
        /// <param name="subSequenceId"></param>
        private void ImportInitialValues(SubSequence subSequence, int subSequenceId)
        {
            // Level is a reserved word...
            string sql =
                "SELECT D_LRBG, TSW_TestSeqSCItl.Level, Mode, NID_LRBG, Q_DIRLRBG, Q_DIRTRAIN, Q_DLRBG, RBC_ID, RBCPhone FROM TSW_TestSeqSCItl WHERE TestSequenceID = " +
                subSequenceId;

            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, Connection);
            DataSet dataSet = new DataSet();

            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
            {
                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    int i = 0;
                    // ReSharper disable InconsistentNaming
                    string D_LRBG = dataRow.ItemArray.GetValue(i++) as string;
                    string Level = dataRow.ItemArray.GetValue(i++) as string;
                    string Mode = dataRow.ItemArray.GetValue(i++) as string;
                    string NID_LRBG = dataRow.ItemArray.GetValue(i++) as string;
                    string Q_DIRLRBG = dataRow.ItemArray.GetValue(i++) as string;
                    string Q_DIRTRAIN = dataRow.ItemArray.GetValue(i++) as string;
                    string Q_DLRBG = dataRow.ItemArray.GetValue(i++) as string;
                    string RBC_ID = dataRow.ItemArray.GetValue(i++) as string;
                    string RBCPhone = dataRow.ItemArray.GetValue(i++) as string;
                    // ReSharper restore InconsistentNaming

                    subSequence.setD_LRBG(D_LRBG);
                    subSequence.setLevel(Level);
                    subSequence.setMode(Mode);
                    subSequence.setNID_LRBG(NID_LRBG);
                    subSequence.setQ_DIRLRBG(Q_DIRLRBG);
                    subSequence.setQ_DIRTRAIN(Q_DIRTRAIN);
                    subSequence.setQ_DLRBG(Q_DLRBG);
                    subSequence.setRBC_ID(RBC_ID);
                    subSequence.setRBCPhone(RBCPhone);

                    TestCase testCase = (TestCase) acceptor.getFactory().createTestCase();
                    testCase.Name = "Setup";
                    subSequence.appendTestCases(testCase);

                    Step initializeTrainDataStep = (Step) acceptor.getFactory().createStep();
                    
                    initializeTrainDataStep.setTCS_Order(0);
                    initializeTrainDataStep.setDistance("0");
                    initializeTrainDataStep.setDescription("Initialize train data");
                    initializeTrainDataStep.setTranslationRequired(true);
                    testCase.appendSteps(initializeTrainDataStep);

                    Step defaultValuesStep = (Step) acceptor.getFactory().createStep();
                    
                    defaultValuesStep.setTCS_Order(0);
                    defaultValuesStep.setDistance("0");
                    defaultValuesStep.setDescription("Set default values");
                    defaultValuesStep.setTranslationRequired(true);
                    testCase.appendSteps(defaultValuesStep);

                    Step manualSetupStep = (Step) acceptor.getFactory().createStep();
                    
                    manualSetupStep.setTCS_Order(0);
                    manualSetupStep.setDistance("0");
                    manualSetupStep.setDescription("Manual setup test sequence");
                    manualSetupStep.setTranslationRequired(false);
                    testCase.appendSteps(manualSetupStep);
                }
            }
            else
            {
                throw new Exception("Cannot find entry in table TSW_TestSeqSCItl WHERE TestSequenceID = " + subSequenceId);
            }
        }

        /// <summary>
        ///     Imports the steps in a sub sequence
        /// </summary>
        /// <param name="subSequence"></param>
        private void ImportSteps(SubSequence subSequence)
        {
            const string sql = "SELECT TCSOrder, Distance, FT_NUMBER, TC_NUMBER, ST_STEP, ST_DESCRIPTION, UserComment, ST_IO, ST_INTERFACE, ST_COMMENTS, TestLevelIn, TestLevelOut, TestModeIn, TestModeOut FROM TSW_TCStep ORDER BY TCSOrder";

            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, Connection);
            DataSet dataSet = new DataSet();

            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
            {
                TestCase testCase = null;

                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    object[] items = dataRow.ItemArray;
                    int order = (int) items[0];
                    string distance = ((int) items[1]).ToString(CultureInfo.InvariantCulture) + ".0";
                    int feature = (int) items[2];
                    int testCaseNr = (int) items[3];
                    string stepType = items[4] as string;
                    string description = items[5] as string;
                    string userComment = items[6] as string;
                    string io = items[7] as string;
                    string intrface = items[8] as string;
                    string comment = items[9] as string;
                    string testLevelIn = items[10] as string;
                    string testLevelOut = items[11] as string;
                    string testModeIn = items[12] as string;
                    string testModeOut = items[13] as string;

                    // we do not want to import steps "Followed by" or "Preceded by"
                    if (io != null && stepType != null && !stepType.Equals("Followed by") &&
                        !stepType.Equals("Preceded by"))
                    {
                        if (testCase != null)
                        {
                            if (testCase.getFeature() != feature || testCase.getCase() != testCaseNr)
                            {
                                testCase = null;
                            }
                        }

                        if (testCase == null)
                        {
                            testCase = (TestCase) acceptor.getFactory().createTestCase();
                            testCase.Name = "Feature " + feature + " Test case " + testCaseNr;
                            testCase.setCase(testCaseNr);
                            testCase.setFeature(feature);
                            subSequence.appendTestCases(testCase);
                            Step setupTestCaseStep = (Step) acceptor.getFactory().createStep();
                            setupTestCaseStep.Name = "Setup test case";
                            setupTestCaseStep.setDistance(distance);
                            setupTestCaseStep.setDescription(setupTestCaseStep.Name);
                            setupTestCaseStep.setComment("This step is used to setup the test case " + testCaseNr +
                                                         " feature " + feature);
                            setupTestCaseStep.setTranslationRequired(true);
                            testCase.appendSteps(setupTestCaseStep);
                        }

                        Step step = (Step) acceptor.getFactory().createStep();
                        step.Name = "Step " + order;
                        step.setTCS_Order(order);
                        step.setDistance(distance);
                        step.setDescription(description);
                        step.setUserComment(userComment);
                        step.setIO_AsString(io);
                        if (intrface != null)
                        {
                            step.setInterface_AsString(intrface);
                        }
                        step.setComment(comment);
                        if (testLevelIn != null)
                        {
                            step.setLevelIN_AsString(testLevelIn);
                        }
                        if (testLevelOut != null)
                        {
                            step.setLevelOUT_AsString(testLevelOut);
                        }
                        if (testModeIn != null)
                        {
                            step.setModeIN_AsString(testModeIn);
                        }
                        if (testModeOut != null)
                        {
                            step.setModeOUT_AsString(testModeOut);
                        }
                        step.setTranslationRequired(true);

                        ImportStepMessages(step);

                        testCase.appendSteps(step);
                    }
                }
            }
            else
            {
                throw new Exception("Cannot find sub sequence table in database");
            }
        }


        /// <summary>
        ///     Imports all the messages used by this step
        /// </summary>
        /// <param name="aStep"></param>
        private void ImportStepMessages(Step aStep)
        {
            const string sql = "SELECT TCSOrder, MessageOrder, MessageType, Var_Name, Var_Value FROM TSW_MessageHeader ORDER BY MessageOrder, Var_Row";

            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, Connection);
            DataSet dataSet = new DataSet();

            adapter.Fill(dataSet);
            if (dataSet.Tables.Count > 0)
            {
                int messageNumber = 0;
                DBMessage message = null;
                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    object[] items = dataRow.ItemArray;
                    int order = (int) items[0];
                    if (order == aStep.getTCS_Order())
                    {
                        short messageOrder = (short) items[1];
                        if (messageNumber != messageOrder) // we create a new Message
                        {
                            if (messageNumber != 0)
                            {
                                aStep.AddMessage(message);
                                ImportPackets(message, order);
                            }
                            short messageTypeNumber = (short) items[2];
                            acceptor.DBMessageType messageType = acceptor.DBMessageType.defaultDBMessageType;
                            switch (messageTypeNumber)
                            {
                                case 0:
                                    messageType = acceptor.DBMessageType.aEUROBALISE;
                                    break;
                                case 1:
                                    messageType = acceptor.DBMessageType.aEUROLOOP;
                                    break;
                                case 2:
                                    messageType = acceptor.DBMessageType.aEURORADIO;
                                    break;
                            }
                            message = (DBMessage) acceptor.getFactory().createDBMessage();
                            message.MessageOrder = messageOrder;
                            message.MessageType = messageType;
                            messageNumber = messageOrder;
                        }
                        DBField field = (DBField) acceptor.getFactory().createDBField();
                        string variable = items[3] as string;
                        if (variable != null)
                        {
                            field.Variable = variable;
                        }
                        string value = items[4] as string;
                        if (value != null)
                        {
                            field.Value = VariableConverter.INSTANCE.Convert(variable, value).ToString();
                        }
                        message.AddField(field);
                    }
                }
                if (message != null)
                {
                    aStep.AddMessage(message);
                    ImportPackets(message, aStep.getTCS_Order());
                }
            }
        }


        /// <summary>
        ///     Impports all the packets for a given message
        /// </summary>
        /// <param name="aMessage"></param>
        /// <param name="TCS_order"></param>
        // ReSharper disable once InconsistentNaming
        private void ImportPackets(DBMessage aMessage, int TCS_order)
        {
            string sql = "SELECT Pac_ID, Var_Name, Var_Value FROM TSW_MessageBody WHERE (TCSOrder = " + TCS_order +
                         ") AND (MessageOrder = " + aMessage.MessageOrder + ") ORDER BY Var_Row";

            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, Connection);
            DataSet dataSet = new DataSet();

            adapter.Fill(dataSet);

            DBPacket packet = null;
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    object[] items = dataRow.ItemArray;
                    if ("NID_PACKET".Equals(items[1]))
                    {
                        if (packet != null)
                        {
                            aMessage.AddPacket(packet);
                        }
                        packet = (DBPacket) acceptor.getFactory().createDBPacket();
                    }

                    if (packet != null)
                    {
                        DBField field = (DBField) acceptor.getFactory().createDBField();
                        string variable = items[1] as string;
                        if (variable != null)
                        {
                            field.Variable = variable;
                        }
                        string value = items[2] as string;
                        if (value != null)
                        {
                            field.Value = VariableConverter.INSTANCE.Convert(variable, value).ToString();
                        }
                        packet.AddField(field);
                    }
                }

                if (packet != null)
                {
                    aMessage.AddPacket(packet);
                }
            }
        }

        /// <summary>
        /// Indicates if the specific packet was found in the database
        /// </summary>
        /// <param name="packetNumber"></param>
        /// <returns></returns>
        public bool FindSpecificPacket(int packetNumber)
        {
            bool retVal = false;
            string sql = "SELECT * FROM TSW_MessageBody WHERE Pac_Id = " + packetNumber;

            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, Connection);
            DataSet dataSet = new DataSet();

            adapter.Fill(dataSet);
            retVal = dataSet.Tables[0].Rows.Count > 0;

            Connection = null;

            return retVal;
        }
    }
}