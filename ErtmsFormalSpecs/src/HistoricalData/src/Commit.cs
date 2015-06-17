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
using System.Collections;

namespace HistoricalData
{
    /// <summary>
    ///     A commit
    /// </summary>
    public class Commit : Generated.Commit
    {
        /// <summary>
        ///     All the changes in this commit
        /// </summary>
        public ArrayList Changes
        {
            get
            {
                if (allChanges() == null)
                {
                    setAllChanges(new ArrayList());
                }
                return allChanges();
            }
        }

        /// <summary>
        ///     The Hash of the commit
        /// </summary>
        public string Sha
        {
            get { return getHash(); }
            set { setHash(value); }
        }

        /// <summary>
        ///     The commit message
        /// </summary>
        public string Message
        {
            get { return getMessage(); }
            set { setMessage(value); }
        }

        /// <summary>
        ///     The date when the commit was performed
        /// </summary>
        public DateTime Date
        {
            get
            {
                DateTime retVal = DateTime.MinValue;

                try
                {
                    retVal = DateTime.Parse(getDate());
                }
                catch (Exception)
                {
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Commit()
            : base()
        {
        }
    }
}