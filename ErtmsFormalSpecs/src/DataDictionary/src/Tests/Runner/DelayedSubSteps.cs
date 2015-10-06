// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpecs software and documentation
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

namespace DataDictionary.Tests.Runner
{
    public class DelayedSubStep : IComparable<DelayedSubStep>
    {
        /// <summary>
        /// The sub-step to execute
        /// </summary>
        public SubStep SubStep { get; private set; }

        /// <summary>
        /// The time at which the sub-step has to be executed
        /// </summary>
        public long Time { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subStep"></param>
        /// <param name="time"></param>
        public DelayedSubStep(SubStep subStep, long time)
        {
            SubStep = subStep;
            Time = time;
        }

        /// <summary>
        /// Comparator of two delayed sub-steps
        /// </summary>
        /// <param name="otherSubStep"></param>
        /// <returns></returns>
        int IComparable<DelayedSubStep>.CompareTo(DelayedSubStep otherSubStep)
        {
            return Time.CompareTo(otherSubStep.Time);
        }
    }

    public class DelayedSubSteps
    {
        /// <summary>
        /// The list of delayed steps
        /// </summary>
        public List<DelayedSubStep> Steps { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DelayedSubSteps()
        {
            Steps = new List<DelayedSubStep>();
        }

        /// <summary>
        /// Adds a new delayed sub-step
        /// </summary>
        /// <param name="delayedSubStep"></param>
        public void AddStep(DelayedSubStep delayedSubStep)
        {
            Steps.Add(delayedSubStep);
            Steps.Sort();
        }

        /// <summary>
        /// Clears the steps
        /// </summary>
        public void Clear()
        {
            Steps.Clear();
        }
    }
}
