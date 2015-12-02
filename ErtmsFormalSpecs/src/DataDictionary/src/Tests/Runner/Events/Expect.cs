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
using NameSpace = DataDictionary.Types.NameSpace;

namespace DataDictionary.Tests.Runner.Events
{
    public class Expect : ModelEvent
    {
        /// <summary>
        ///     The time out for this expectation
        /// </summary>
        public double TimeOut
        {
            get
            {
                double retVal = double.MaxValue;

                if (Expectation.DeadLine >= 0)
                {
                    retVal = Time + Expectation.DeadLine;
                }

                return retVal;
            }
        }


        /// <summary>
        ///     The state of this expect :
        ///     Active     : still expecting something
        ///     Fullfilled : the expectation has been reached
        ///     TimeOut    : the time before the expectation has been reached
        /// </summary>
        public enum EventState
        {
            Active,
            Fullfilled,
            TimeOut
        }

        private EventState state = EventState.Active;

        public EventState State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        ///     The corresponding expectation
        /// </summary>
        private Expectation expectation;

        public Expectation Expectation
        {
            get { return expectation; }
            private set { expectation = value; }
        }

        /// <summary>
        ///     The namespace associated to this event
        /// </summary>
        public override NameSpace NameSpace
        {
            get { return null; }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        public Expect(Expectation expectation, acceptor.RulePriority? priority)
            : base(expectation.ExpressionText, expectation, priority)
        {
            Expectation = expectation;
            State = EventState.Active;
        }

        /// <summary>
        ///     Adds this expectation in the list of active expectations in the time line
        /// </summary>
        /// <param name="runner"></param>
        public override void Apply(Runner runner)
        {
            base.Apply(runner);
            TimeLine.ActiveExpectations.Add(this);
        }

        /// <summary>
        ///     Rolls back this event
        /// </summary>
        /// <param name="runner"></param>
        public override void RollBack(Runner runner)
        {
            TimeLine.ActiveExpectations.Remove(this);

            base.RollBack(runner);
        }
    }
}