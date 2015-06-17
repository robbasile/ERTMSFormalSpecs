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
using Utils;
using NameSpace = DataDictionary.Types.NameSpace;

namespace DataDictionary.Tests.Runner.Events
{
    public class ModelInterpretationFailure : ModelEvent
    {
        /// <summary>
        ///     The log associated to this failure
        /// </summary>
        public ElementLog Log { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        public ModelInterpretationFailure(ElementLog log, INamable instance, acceptor.RulePriority? priority)
            : base(log.Log, instance, priority)
        {
            Log = log;
        }

        /// <summary>
        ///     Adds this expectation in the list of active expectations in the time line
        /// </summary>
        /// <param name="runner"></param>
        public override void Apply(Runner runner)
        {
            base.Apply(runner);
        }

        /// <summary>
        ///     Rolls back this event
        /// </summary>
        public override void RollBack()
        {
            base.RollBack();
        }

        /// <summary>
        ///     The namespace associated to this event
        /// </summary>
        public override NameSpace NameSpace
        {
            get { return null; }
        }
    }
}