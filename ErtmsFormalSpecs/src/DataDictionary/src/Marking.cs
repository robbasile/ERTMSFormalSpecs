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

using System.Collections.Generic;
using DataDictionary.Generated;
using Utils;

namespace DataDictionary
{
    /// <summary>
    ///     Keeps track of all the model element which have a message
    /// </summary>
    public class Marking
    {
        private class Gatherer : Visitor
        {
            /// <summary>
            ///     Provides the logs associated to the model elements
            /// </summary>
            public Dictionary<ModelElement, List<ElementLog>> Markings { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            public Gatherer(EFSSystem system)
            {
                Markings = new Dictionary<ModelElement, List<ElementLog>>();

                foreach (Dictionary dictionary in system.Dictionaries)
                {
                    visit(dictionary);
                }
            }

            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                ModelElement element = (ModelElement) obj;

                if (element.Messages.Count > 0)
                {
                    List<ElementLog> messages = new List<ElementLog>();
                    messages.AddRange(element.Messages);
                    Markings[element] = messages;
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     The gatherer used to collect all logs
        /// </summary>
        private Gatherer TheGatherer { get; set; }

        /// <summary>
        ///     Creates a marking for the current system
        /// </summary>
        /// <param name="system"></param>
        public Marking(EFSSystem system)
        {
            TheGatherer = new Gatherer(system);
        }

        /// <summary>
        ///     Restores the marks
        /// </summary>
        public void RestoreMarks()
        {
            foreach (KeyValuePair<ModelElement, List<ElementLog>> pair in TheGatherer.Markings)
            {
                pair.Key.Messages.AddRange(pair.Value);
            }
        }
    }
}