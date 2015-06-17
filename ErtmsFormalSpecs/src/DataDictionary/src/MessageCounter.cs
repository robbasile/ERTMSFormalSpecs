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

namespace DataDictionary
{
    /// <summary>
    ///     Counts the number of messages in the model
    /// </summary>
    public class MessageCounter : Visitor
    {
        /// <summary>
        ///     The number of information found
        /// </summary>
        public int Info { get; private set; }

        /// <summary>
        ///     The number of warnings found
        /// </summary>
        public int Warning { get; private set; }

        /// <summary>
        ///     The number of errors found
        /// </summary>
        public int Error { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="system"></param>
        public MessageCounter(EFSSystem system)
        {
            Info = 0;
            Warning = 0;
            Error = 0;
            foreach (Dictionary dictionary in system.Dictionaries)
            {
                visit(dictionary, true);
            }
        }

        /// <summary>
        ///     Actually counts the messages
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(BaseModelElement obj, bool visitSubNodes)
        {
            count(obj);

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        ///     Actually counts the messages
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(Generated.Namable obj, bool visitSubNodes)
        {
            count(obj);

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        ///     Actually counts
        /// </summary>
        /// <param name="obj"></param>
        private void count(BaseModelElement obj)
        {
            foreach (ElementLog log in obj.Messages)
            {
                switch (log.Level)
                {
                    case ElementLog.LevelEnum.Error:
                        Error += 1;
                        break;
                    case ElementLog.LevelEnum.Warning:
                        Warning += 1;
                        break;
                    case ElementLog.LevelEnum.Info:
                        Info += 1;
                        break;
                }
            }
        }
    }
}