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
using DataDictionary.Interpreter;
using Utils;
using NameSpace = DataDictionary.Types.NameSpace;

namespace DataDictionary.Tests.Runner.Events
{
    public abstract class ModelEvent : ITextualExplain
    {
        /// <summary>
        ///     The event Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The message associated to this event
        /// </summary>
        private string _message = null;

        public string Message
        {
            get
            {
                string retVal = _message;

                if (Explanation != null)
                {
                    retVal = Explanation.Message;
                }

                return retVal;
            }
            set { _message = value; }
        }

        /// <summary>
        ///     The explanation about this model event
        /// </summary>
        public ExplanationPart Explanation { get; set; }

        /// <summary>
        ///     The event time
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        ///     The time line in which the model event sits
        /// </summary>
        public EventTimeLine TimeLine { get; set; }

        /// <summary>
        ///     The instance on which this event occured
        /// </summary>
        public INamable Instance { get; private set; }

        /// <summary>
        ///     The namespace associated to this event
        /// </summary>
        public abstract NameSpace NameSpace { get; }

        /// <summary>
        ///     The priority when the event occurs
        /// </summary>
        public acceptor.RulePriority? Priority { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <param name="priority"></param>
        public ModelEvent(string id, INamable instance, acceptor.RulePriority? priority)
        {
            Id = id;
            Message = id;
            Instance = instance;
            Priority = priority;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public ModelEvent(string id, string message, acceptor.RulePriority? priority)
        {
            Id = id;
            Message = message;
            Priority = priority;
        }

        /// <summary>
        ///     Indicates that the changes have been computed
        /// </summary>
        private bool changesComputed = false;

        /// <summary>
        ///     Computes the changes related to this event
        /// </summary>
        /// <param name="apply">Indicates that the changes should be applied directly</param>
        /// <param name="runner"></param>
        /// <returns>True if changes should be computed</returns>
        public virtual bool ComputeChanges(bool apply, Runner runner)
        {
            bool retVal = !changesComputed;

            changesComputed = true;

            return retVal;
        }

        /// <summary>
        ///     Applies the changes related to this event
        /// </summary>
        /// <param name="runner"></param>
        public virtual void Apply(Runner runner)
        {
            if (!changesComputed)
            {
                ComputeChanges(false, runner);
            }
        }

        /// <summary>
        ///     Rollsback the changes performed during this event
        /// </summary>
        public virtual void RollBack()
        {
            // By default, nothing to rollback
        }

        /// <summary>
        ///     Displays informations related to this model event
        /// </summary>
        public override string ToString()
        {
            return Time + ": " + Message;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Write("MODEL EVENT ");
            explanation.WriteLine(ToString());
        }
    }
}