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

using System.Runtime.Serialization;
using System.ServiceModel;
using GUI.IPCInterface.Values;

namespace GUI.IPCInterface
{
    /// <summary>
    ///     The cycle priority to execute
    /// </summary>
    public enum Step
    {
        Verification,
        UpdateInternal,
        Process,
        UpdateOutput,
        CleanUp
    };

    /// <summary>
    ///     A fault occured while executing a service function
    /// </summary>
    [DataContract]
    public class EFSServiceFault
    {
        /// <summary>
        ///     The fault message
        /// </summary>
        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        ///     Provides the inner reason for this exception
        /// </summary>
        [DataMember]
        public EFSServiceFault InnerReason { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="message"></param>
        public EFSServiceFault(string message)
        {
            Message = message;
            InnerReason = null;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerReason"></param>
        public EFSServiceFault(string message, EFSServiceFault innerReason)
        {
            Message = message;
            InnerReason = innerReason;
        }
    }

    [ServiceContract]
    public interface IEFSService
    {
        /// <summary>
        ///     Connects to the service using the default parameters
        /// </summary>
        /// <param name="listener">Indicates that the client is a listener</param>
        /// <returns>The client identifier</returns>
        [OperationContract]
        [FaultContract(typeof (EFSServiceFault))]
        int ConnectUsingDefaultValues(bool listener);

        /// <summary>
        ///     Connects to the service
        /// </summary>
        /// <param name="listener">Indicates that the client is a listener</param>
        /// <param name="explain">Indicates that the explain view should be updated according to the scenario execution</param>
        /// <param name="logEvents">Indicates that the events should be logged</param>
        /// <param name="cycleDuration">The duration (in ms) of an execution cycle</param>
        /// <param name="keepEventCount">The number of events that should be kept in memory</param>
        /// <returns>The client identifier</returns>
        [OperationContract]
        [FaultContract(typeof (EFSServiceFault))]
        int Connect(bool listener, bool explain, bool logEvents, int cycleDuration, int keepEventCount);

        /// <summary>
        ///     Provides the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        [OperationContract]
        Value GetVariableValue(string variableName);

        /// <summary>
        ///     Provides the value of an expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        [OperationContract]
        Value GetExpressionValue(string expression);

        /// <summary>
        ///     Sets the value of a specific variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        [OperationContract]
        [FaultContract(typeof (EFSServiceFault))]
        void SetVariableValue(string variableName, Value value);

        /// <summary>
        ///     Applies a specific statement on the model
        /// </summary>
        /// <param name="statement">The statement to apply</param>
        [OperationContract]
        [FaultContract(typeof (EFSServiceFault))]
        void ApplyStatement(string statement);

        /// <summary>
        ///     Activates the execution of a single cycle, as the given priority level
        /// </summary>
        /// <param name="clientId">The id of the client</param>
        /// <param name="step">The cycle step to execute</param>
        /// <returns>true if cycle execution is successful, false when the client is asked not to perform his work</returns>
        [OperationContract]
        [FaultContract(typeof (EFSServiceFault))]
        bool Cycle(int clientId, Step step);

        /// <summary>
        /// Suspends the execution of the connection
        /// </summary>
        /// <param name="clientId"></param>
        [OperationContract]
        [FaultContract(typeof(EFSServiceFault))]
        void Suspend(int clientId);

        /// <summary>
        /// Awakes the connection
        /// </summary>
        /// <param name="clientId"></param>
        [OperationContract]
        [FaultContract(typeof(EFSServiceFault))]
        void Awake(int clientId);

        /// <summary>
        ///     Restarts the engine with default values
        /// </summary>
        [OperationContract]
        void Restart();

        /// <summary>
        ///     Close the connection
        /// </summary>
        /// <param name="clientId">The id of the client</param>
        /// <returns>true if cycle execution is successful, false when the client is asked not to perform his work</returns>
        [OperationContract]
        [FaultContract(typeof(EFSServiceFault))]
        void Close(int clientId);
    }
}