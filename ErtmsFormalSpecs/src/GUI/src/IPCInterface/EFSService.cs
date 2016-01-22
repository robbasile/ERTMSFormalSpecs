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
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Functions;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Rules;
using DataDictionary.Tests.Runner;
using DataDictionary.Tests.Runner.Events;
using DataDictionary.Values;
using DataDictionary.Variables;
using GUI.IPCInterface.Values;
using GUI.LongOperations;
using GUIUtils;
using Utils;
using Action = DataDictionary.Rules.Action;
using BoolValue = DataDictionary.Values.BoolValue;
using Dictionary = DataDictionary.Dictionary;
using DoubleValue = DataDictionary.Values.DoubleValue;
using Enum = System.Enum;
using EnumValue = DataDictionary.Constants.EnumValue;
using Function = DataDictionary.Generated.Function;
using IntValue = DataDictionary.Values.IntValue;
using ListValue = DataDictionary.Values.ListValue;
using NameSpace = DataDictionary.Types.NameSpace;
using Parameter = DataDictionary.Parameter;
using State = DataDictionary.Constants.State;
using StringValue = DataDictionary.Values.StringValue;
using StructureValue = DataDictionary.Values.StructureValue;
using Util = DataDictionary.Util;
using Value = GUI.IPCInterface.Values.Value;

namespace GUI.IPCInterface
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class EFSService : IEFSService
    {
        /// <summary>
        ///     Indicates that the explain view should be updated according to the scenario execution
        /// </summary>
        public bool Explain { get; private set; }

        /// <summary>
        ///     Indicates that the events should be logged
        /// </summary>
        public bool LogEvents { get; private set; }

        /// <summary>
        ///     The duration (in ms) of an execution cycle
        /// </summary>
        public int CycleDuration { get; private set; }

        /// <summary>
        ///     The number of events that should be kept in memory
        /// </summary>
        public int KeepEventCount { get; private set; }

        /// <summary>
        ///     Resource protection
        /// </summary>
        private Dictionary<Step, Mutex> StepAccess { get; set; }

        /// <summary>
        ///     Mutual exclusion for accessing EFS structures
        /// </summary>
        private Mutex EfsAccess { get; set; }

        /// <summary>
        ///     Keeps track of each connection status
        /// </summary>
        private class ConnectionStatus
        {
            /// <summary>
            ///     Indicates that the connection is still active
            /// </summary>
            public bool Active { get; set; }

            /// <summary>
            /// Indicates whether the connection is suspended or awake
            /// </summary>
            public bool Suspended { get; set; }

            /// <summary>
            ///     The step for which the client is waiting
            /// </summary>
            public Step ExpectedStep { get; set; }

            /// <summary>
            ///     Indicates that the client is a listener, and should not create a runner
            /// </summary>
            public bool Listener { get; private set; }

            /// <summary>
            ///     The last time a cycle request has been performed
            /// </summary>
            public DateTime LastCycleRequest { get; set; }

            /// <summary>
            ///     The last time a cycle activity has been resumed
            /// </summary>
            public DateTime LastCycleResume { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="listener"></param>
            public ConnectionStatus(bool listener)
            {
                Active = true;
                Suspended = false;
                Listener = listener;
                LastCycleRequest = DateTime.MinValue;
                LastCycleResume = DateTime.MinValue;
            }
        }

        /// <summary>
        ///     The list of connection statuses
        /// </summary>
        private List<ConnectionStatus> Connections { get; set; }

        /// <summary>
        ///     The last step being executed
        /// </summary>
        private Step LastStep { get; set; }

        /// <summary>
        ///     Provides the next step to execute
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private Step NextStep(Step current)
        {
            Step retVal = Step.CleanUp;

            switch (current)
            {
                case Step.CleanUp:
                    retVal = Step.Verification;
                    break;
                case Step.Verification:
                    retVal = Step.UpdateInternal;
                    break;
                case Step.UpdateInternal:
                    retVal = Step.Process;
                    break;
                case Step.Process:
                    retVal = Step.UpdateOutput;
                    break;
                case Step.UpdateOutput:
                    retVal = Step.CleanUp;
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     The thread which is used to launch the runner
        /// </summary>
        private LaunchRunner LaunchRunnerSynchronizer { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public EFSService()
        {
            Connections = new List<ConnectionStatus>();
            Explain = true;
            LogEvents = false;
            CycleDuration = 100;
            KeepEventCount = 10000;

            LastStep = Step.CleanUp;

            StepAccess = new Dictionary<Step, Mutex>();
            LaunchRunnerSynchronizer = new LaunchRunner(this, 10);

            EfsAccess = new Mutex(false, "EFS access");
        }

        /// <summary>
        ///     Adds a client for this server
        /// </summary>
        /// <param name="listener"></param>
        /// <returns>The client id</returns>
        private int AddClient(bool listener)
        {
            int retVal = 0; 
            
            // Try to find an empty slot in the connection list
            // (e.g. a client which disconnected from the service)
            while (retVal < Connections.Count)
            {
                if (Connections[retVal] == null)
                {
                    Connections[retVal] = new ConnectionStatus(listener);
                    break;
                }

                retVal += 1;
            }

            // No empty slot has been found in the connection list
            if (retVal == Connections.Count)
            {
                Connections.Add(new ConnectionStatus(listener));
            }

            return Connections.Count - 1;
        }

        /// <summary>
        ///     Connects to the service
        /// </summary>
        /// <param name="listener">Indicates that the client is a listener</param>
        /// <returns>The client identifier</returns>
        public int ConnectUsingDefaultValues(bool listener)
        {
            EfsAccess.WaitOne();
            int clientId = AddClient(listener);
            EfsAccess.ReleaseMutex();

            return clientId;
        }

        /// <summary>
        ///     Connects to the service
        /// </summary>
        /// <param name="listener">Indicates that the client is a listener</param>
        /// <param name="explain">Indicates that the explain view should be updated according to the scenario execution</param>
        /// <param name="logEvents">Indicates that the events should be logged</param>
        /// <param name="cycleDuration">The duration (in ms) of an execution cycle</param>
        /// <param name="keepEventCount">The number of events that should be kept in memory</param>
        public int Connect(bool listener, bool explain, bool logEvents, int cycleDuration, int keepEventCount)
        {
            EfsAccess.WaitOne();

            int clientId = AddClient(listener);

            Explain = explain;
            LogEvents = logEvents;
            CycleDuration = cycleDuration;
            KeepEventCount = keepEventCount;

            EfsAccess.ReleaseMutex();

            return clientId;
        }

        /// <summary>
        ///     Ensures that the client id is valid
        /// </summary>
        /// <param name="clientId"></param>
        private void CheckClient(int clientId)
        {
            if (clientId >= Connections.Count)
            {
                throw new FaultException<EFSServiceFault>(new EFSServiceFault("Invalid client id " + clientId));
            }
            else
            {
                // The client is alive again. Reconnect it.
                Connections[clientId].Active = true;
            }
        }

        /// <summary>
        ///     Checks that a cycle can be launched, that is each client has voted for his next step
        /// </summary>
        /// <returns></returns>
        private bool CheckLaunch()
        {
            bool retVal = false;

            // Checks that there are active connections
            foreach (ConnectionStatus status in Connections)
            {
                if (status.Active && !status.Suspended)
                {
                    retVal = true;
                    break;
                }
            }

            if (retVal)
            {
                // Checks that all active connection have selected their next step
                foreach (ConnectionStatus status in Connections)
                {
                    if (status.Active && !status.Suspended && status.LastCycleRequest <= status.LastCycleResume)
                    {
                        retVal = false;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates if there is a client pending for a specific step
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private bool PendingClients(Step step)
        {
            bool retVal = false;

            // Checks that there are active connections
            foreach (ConnectionStatus status in Connections)
            {
                if (status.ExpectedStep == step && status.LastCycleRequest > status.LastCycleResume)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     5s between each client decisions
        /// </summary>
        private static readonly TimeSpan MaxDelta = new TimeSpan(0, 0, 0, 5, 0);

        /// <summary>
        ///     Performs a single cycle
        /// </summary>
        public void Cycle()
        {
            EfsAccess.WaitOne();

            try
            {
                DateTime now = DateTime.Now;

                // Close inactive connections
                foreach (ConnectionStatus status in Connections)
                {
                    TimeSpan delta = now - status.LastCycleRequest;
                    if (delta > MaxDelta && !status.Suspended)
                    {
                        status.Active = false;
                    }
                }

                // Launches the runner when all active client have selected their next step
                while (CheckLaunch())
                {
                    LastStep = NextStep(LastStep);

                    if (Runner != null)
                    {
                        try
                        {
                            if (!AllListeners)
                            {
                                Util.DontNotify(() =>
                                {
                                    Runner.ExecuteOnePriority(convertStep2Priority(LastStep));
                                    if (LastStep == Step.CleanUp)
                                    {
                                        EfsSystem.Instance.Context.HandleEndOfCycle();
                                        ClearFunctionCaches();
                                    }
                                });
                            }
                        }
                        catch (Exception)
                        {
                            // Ignore
                        }
                    }

                    while (PendingClients(LastStep))
                    {
                        // Let the processes waiting for the end of this step run
                        StepAccess[LastStep].ReleaseMutex();

                        // Let the other processes wake up
                        Thread.Sleep(1);

                        // Wait until all processes for this step have executed their work
                        StepAccess[LastStep].WaitOne();
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                EfsAccess.ReleaseMutex();
            }
        }

        /// <summary>
        ///     Cleanup function cache every
        /// </summary>
        private int _cacheCycle = 1;

        /// <summary>
        ///     The number of cycles after which a clear cache is required
        /// </summary>
        private const int CleanUpCycleCount = 50;

        /// <summary>
        ///     Clears the function cache after each full cycle
        /// </summary>
        /// <param name="force">Forces the clear</param>
        private void ClearFunctionCaches(bool force = false)
        {
            _cacheCycle = (_cacheCycle + 1)%CleanUpCycleCount;
            if (force || _cacheCycle == 0)
            {
                EfsSystem.Instance.ClearFunctionCache();
            }
        }

        /// <summary>
        ///     Continuously launch the runner when all client have selected their next stop step
        /// </summary>
        private class LaunchRunner : GenericSynchronizationHandler<EFSService>
        {
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="service"></param>
            /// <param name="cycleTime"></param>
            public LaunchRunner(EFSService service, int cycleTime)
                : base(service, cycleTime)
            {
            }

            /// <summary>
            ///     Initialize the task
            /// </summary>
            /// <param name="instance"></param>
            public override void Initialize(EFSService instance)
            {
                // Allocates all critical section
                foreach (Step step in Enum.GetValues(typeof (Step)))
                {
                    instance.StepAccess[step] = new Mutex(true, step.ToString());
                }
            }

            /// <summary>
            ///     Actually performes the synchronization, that is launch the runner when all client are ready
            /// </summary>
            /// <param name="instance"></param>
            public override void HandleSynchronization(EFSService instance)
            {
                instance.Cycle();
            }
        }

        /// <summary>
        ///     Activates the execution of a single cycle, as the given priority level
        /// </summary>
        /// <param name="clientId">The id of the client</param>
        /// <param name="step">The cycle step to execute</param>
        /// <returns>true if cycle execution is successful, false when the client is asked not to perform his work</returns>
        public bool Cycle(int clientId, Step step)
        {
            bool retVal = false;

            Runner runner = Runner;
            if (runner != null && !runner.PleaseWait)
            {
                retVal = true;

                CheckClient(clientId);

                Connections[clientId].LastCycleRequest = DateTime.Now;
                Connections[clientId].LastCycleResume = DateTime.MinValue;
                Connections[clientId].ExpectedStep = step;

                StepAccess[step].WaitOne();

                Connections[clientId].LastCycleResume = DateTime.Now;
                StepAccess[step].ReleaseMutex();
            }
            else
            {
                Thread.Sleep(300);
            }

            return retVal;
        }

        /// <summary>
        /// Suspends the execution of the connection
        /// </summary>
        /// <param name="clientId"></param>
        public void Suspend(int clientId)
        {
            CheckClient(clientId);
            EfsAccess.WaitOne();
            Connections[clientId].Suspended = true;
            EfsAccess.ReleaseMutex();
        }

        /// <summary>
        /// Awakes the connection
        /// </summary>
        /// <param name="clientId"></param>
        public void Awake(int clientId)
        {
            CheckClient(clientId);
            EfsAccess.WaitOne();
            Connections[clientId].Suspended = false;
            EfsAccess.ReleaseMutex();
        }

        /// <summary>
        ///     Restarts the engine with default values
        /// </summary>
        public void Restart()
        {
            EfsAccess.WaitOne();
            ClearFunctionCaches(true);
            EfsSystem.Instance.Runner = new Runner(Explain, CycleDuration, KeepEventCount);
            EfsAccess.ReleaseMutex();
        }

        /// <summary>
        ///     Loads the dictionary designated by filename
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            EfsAccess.WaitOne();
            MainWindow window = GuiUtils.MdiWindow;
            window.Invoke((MethodInvoker) (() => window.OpenFile(fileName)));
            EfsAccess.ReleaseMutex();
        }

        /// <summary>
        ///     Stops the session by closing the main window
        /// </summary>
        public void Stop()
        {
            EfsAccess.WaitOne();
            GuiUtils.MdiWindow.Close();
            EfsAccess.ReleaseMutex();
        }

        /// <summary>
        ///     Close the connection
        /// </summary>
        /// <param name="clientId">The id of the client</param>
        /// <returns>true if cycle execution is successful, false when the client is asked not to perform his work</returns>
        public void Close(int clientId)
        {
            CheckClient(clientId);
            Connections[clientId] = null;
        }

        /// <summary>
        ///     Provides the runner on which the service is applied
        /// </summary>
        public Runner Runner
        {
            get
            {
                EfsSystem efsSystem = EfsSystem.Instance;

                if (efsSystem.Runner == null && !AllListeners)
                {
                    EfsSystem.Instance.Runner = new Runner(Explain, CycleDuration, KeepEventCount);
                }

                return efsSystem.Runner;
            }
        }

        /// <summary>
        ///     Indicates that all connections are listeners
        /// </summary>
        /// <returns></returns>
        private bool AllListeners
        {
            get
            {
                bool retVal = true;
                foreach (ConnectionStatus status in Connections)
                {
                    if (status.Active && !status.Listener)
                    {
                        retVal = false;
                        break;
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public Value GetVariableValue(string variableName)
        {
            Value retVal = null;

            EfsAccess.WaitOne();
            try
            {
                IVariable variable = EfsSystem.Instance.FindByFullName(variableName) as IVariable;
                if (variable != null)
                {
                    retVal = ConvertOut(variable.Value);
                }
            }
            catch (Exception)
            {
                // TODO
            }
            finally
            {
                EfsAccess.ReleaseMutex();
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value of an expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Value GetExpressionValue(string expression)
        {
            Value retVal = null;

            EfsAccess.WaitOne();
            try
            {
                Expression expressionTree = new Parser().Expression(EfsSystem.Instance.Dictionaries[0],
                    expression);
                if (expressionTree != null)
                {
                    Util.DontNotify(() =>
                    {
                        retVal =
                            ConvertOut(expressionTree.GetExpressionValue(new InterpretationContext(), null));
                    });
                }
            }
            catch (Exception)
            {
                // TODO
            }
            finally
            {
                EfsAccess.ReleaseMutex();
            }

            return retVal;
        }

        /// <summary>
        ///     Converts a DataDictionary.Values.IValue into an EFSIPCInterface.Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Value ConvertOut(IValue value)
        {
            // Handles the boolean case
            {
                BoolValue v = value as BoolValue;
                if (v != null)
                {
                    return new Values.BoolValue(v.Val);
                }
            }

            // Handles the integer case
            {
                IntValue v = value as IntValue;
                if (v != null)
                {
                    return new Values.IntValue(v.Val);
                }
            }

            // Handles the double case
            {
                DoubleValue v = value as DoubleValue;
                if (v != null)
                {
                    return new Values.DoubleValue(v.Val);
                }
            }

            // Handles the string case
            {
                StringValue v = value as StringValue;
                if (v != null)
                {
                    return new Values.StringValue(v.Val);
                }
            }

            // Handles the state case
            {
                State v = value as State;
                if (v != null)
                {
                    return new StateValue(v.FullName);
                }
            }

            // Handles the enumeration value case
            {
                EnumValue v = value as EnumValue;
                if (v != null)
                {
                    return new Values.EnumValue(v.FullName);
                }
            }

            // Handles the list case
            {
                ListValue v = value as ListValue;
                if (v != null)
                {
                    List<Value> list = new List<Value>();

                    foreach (IValue item in v.Val)
                    {
                        list.Add(ConvertOut(item));
                    }

                    return new Values.ListValue(list);
                }
            }

            // Handles the structure case
            {
                StructureValue v = value as StructureValue;
                if (v != null)
                {
                    Dictionary<string, Value> record = new Dictionary<string, Value>();

                    foreach (KeyValuePair<string, INamable> pair in v.Val)
                    {
                        IVariable variable = pair.Value as IVariable;
                        if (variable != null)
                        {
                            record.Add(variable.Name, ConvertOut(variable.Value));
                        }
                    }

                    return new Values.StructureValue(record);
                }
            }

            // Handles the function case
            {
                DataDictionary.Functions.Function v = value as DataDictionary.Functions.Function;
                if (v != null)
                {
                    List<Segment> segments = new List<Segment>();

                    if (v.FormalParameters.Count == 1)
                    {
                        Graph graph = v.CreateGraph(new InterpretationContext(), (Parameter) v.FormalParameters[0], null);

                        if (graph != null)
                        {
                            foreach (Graph.Segment segment in graph.Segments)
                            {
                                double length = segment.End - segment.Start;
                                segments.Add(new Segment
                                {
                                    A = segment.Expression.A,
                                    V0 = segment.Expression.V0,
                                    D0 = segment.Start,
                                    Length = length
                                });
                            }
                        }
                    }

                    return new FunctionValue(segments);
                }
            }

            // Handles the 'empty' value
            {
                EmptyValue emptyValue = value as EmptyValue;
                if (emptyValue != null)
                {
                    return null;
                }
            }

            throw new FaultException<EFSServiceFault>(new EFSServiceFault("Cannot convert value " + value));
        }

        private class SyntheticVariableUpdateAction : Action
        {
            /// <summary>
            ///     The variable identification that is modified by this variable update action
            /// </summary>
            private IVariable Variable { get; set; }

            /// <summary>
            ///     The value that is assigned to this variable
            /// </summary>
            private IValue Value { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="variable"></param>
            /// <param name="value"></param>
            public SyntheticVariableUpdateAction(IVariable variable, IValue value)
            {
                Variable = variable;
                Value = value;
            }

            public override string ExpressionText
            {
                get { return Variable.FullName + " <- " + Value.FullName; }
            }

            public override void GetChanges(InterpretationContext context, ChangeList changes,
                ExplanationPart explanation, bool apply, Runner runner)
            {
                Change change = new Change(Variable, Variable.Value, Value);
                changes.Add(change, apply, runner);
            }
        }

        /// <summary>
        ///     Sets the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public void SetVariableValue(string variableName, Value value)
        {
            EfsAccess.WaitOne();
            try
            {
                if (Runner != null)
                {
                    IVariable variable = EfsSystem.Instance.FindByFullName(variableName) as IVariable;

                    if (variable != null)
                    {
                        Util.DontNotify(() =>
                        {
                            Runner.CacheImpact = new CacheImpact();
                            SyntheticVariableUpdateAction action = new SyntheticVariableUpdateAction(variable,
                                value.ConvertBack(variable.Type));
                            VariableUpdate variableUpdate = new VariableUpdate(action, null, null);
                            Runner.EventTimeLine.AddModelEvent(variableUpdate, Runner, true);
                            Runner.ClearCaches();
                        });
                    }
                    else
                    {
                        throw new FaultException<EFSServiceFault>(
                            new EFSServiceFault("Cannot find variable " + variableName));
                    }
                }
            }
            finally
            {
                EfsAccess.ReleaseMutex();
            }
        }

        /// <summary>
        ///     Applies a specific statement on the model
        /// </summary>
        /// <param name="statementText"></param>
        public void ApplyStatement(string statementText)
        {
            EfsAccess.WaitOne();
            try
            {
                if (Runner != null)
                {
                    const bool silent = true;
                    using (Parser parser = new Parser())
                    {
                        Statement statement = parser.Statement(EfsSystem.Instance.Dictionaries[0],
                            statementText, silent);

                        if (statement != null)
                        {
                            Util.DontNotify(() =>
                            {
                                Runner.CacheImpact = new CacheImpact();
                                Action action = (Action) acceptor.getFactory().createAction();
                                action.ExpressionText = statementText;
                                VariableUpdate variableUpdate = new VariableUpdate(action, null, null);
                                Runner.EventTimeLine.AddModelEvent(variableUpdate, Runner, true);
                                Runner.ClearCaches();
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                // TODO
            }
            finally
            {
                EfsAccess.ReleaseMutex();
            }
        }

        /// <summary>
        ///     Converts an interface priority to a Runner priority
        /// </summary>
        /// <param name="priority"></param>
        private acceptor.RulePriority convertStep2Priority(Step priority)
        {
            acceptor.RulePriority retVal = acceptor.RulePriority.defaultRulePriority;

            switch (priority)
            {
                case Step.Verification:
                    retVal = acceptor.RulePriority.aVerification;
                    break;

                case Step.UpdateInternal:
                    retVal = acceptor.RulePriority.aUpdateINTERNAL;
                    break;

                case Step.Process:
                    retVal = acceptor.RulePriority.aProcessing;
                    break;

                case Step.UpdateOutput:
                    retVal = acceptor.RulePriority.aUpdateOUT;
                    break;

                case Step.CleanUp:
                    retVal = acceptor.RulePriority.aCleanUp;
                    break;
            }

            return retVal;
        }

        private static EFSService _instance;

        /// <summary>
        ///     The service instance
        /// </summary>
        public static EFSService INSTANCE
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EFSService();
                }

                return _instance;
            }
        }
    }
}