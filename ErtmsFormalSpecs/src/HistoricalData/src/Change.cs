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
using HistoricalData.Generated;

namespace HistoricalData
{
    /// <summary>
    ///     A single change in a commit
    /// </summary>
    public class Change : Generated.Change, IComparable<Change>
    {
        /// <summary>
        ///     The name of the change
        /// </summary>
        public override string Name
        {
            get
            {
                string retVal = "";

                switch (Action)
                {
                    case acceptor.ChangeOperationEnum.aAdd:
                        retVal += "Add " + After + " in " + Field;
                        break;
                    case acceptor.ChangeOperationEnum.aChange:
                        retVal += "Change in " + Field;
                        break;
                    case acceptor.ChangeOperationEnum.aRemove:
                        retVal += "Remove " + Before + " from " + Field;
                        break;
                }

                return retVal;
            }
            set { }
        }

        /// <summary>
        ///     The commit related to this change
        /// </summary>
        public Commit Commit
        {
            get { return getFather() as Commit; }
        }

        /// <summary>
        ///     The action performed during the change
        /// </summary>
        public acceptor.ChangeOperationEnum Action
        {
            get { return getOperation(); }
            protected set { setOperation(value); }
        }

        /// <summary>
        ///     The guid on which the change occured
        /// </summary>
        public string Guid
        {
            get { return getGuid(); }
            protected set { setGuid(value); }
        }

        /// <summary>
        ///     The field that is affected, if applicable
        /// </summary>
        public string Field
        {
            get { return getField(); }
            protected set { setField(value); }
        }

        /// <summary>
        ///     The previous value of the field, if applicable
        /// </summary>
        public string Before
        {
            get { return getBefore(); }
            protected set { setBefore(value); }
        }

        /// <summary>
        ///     The value of the field after the change, if applicable
        /// </summary>
        public string After
        {
            get { return getAfter(); }
            protected set { setAfter(value); }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="action"></param>
        /// <param name="field"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public Change(string guid, acceptor.ChangeOperationEnum action, string field, string before, string after)
            : base()
        {
            Guid = guid;
            Action = action;
            Field = field;
            Before = before;
            After = after;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Change()
            : base()
        {
        }

        /// <summary>
        ///     The date when the change was done
        /// </summary>
        public DateTime Date
        {
            get { return Commit.Date; }
        }

        // Summary:
        //     Compares the current object with another object of the same type.
        //
        // Parameters:
        //   other:
        //     An object to compare with this object.
        //
        // Returns:
        //     A value that indicates the relative order of the objects being compared.
        //     The return value has the following meanings: Value Meaning Less than zero
        //     This object is less than the other parameter.Zero This object is equal to
        //     other. Greater than zero This object is greater than other.
        public int CompareTo(Change other)
        {
            return Date.CompareTo(other.Date);
        }
    }
}