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
using System.ComponentModel;
using GUI.Converters;
using HistoricalData.Generated;
using Change = HistoricalData.Change;

namespace GUI.HistoryView
{
    public class ChangeTreeNode : ModelElementTreeNode<Change>
    {
        /// <summary>
        ///     The editor
        /// </summary>
        private class ChangeEditor : Editor
        {
            /// <summary>
            ///     The commit author
            /// </summary>
            // ReSharper disable once UnusedMember.Local
            public string Author
            {
                get { return Item.Commit.getCommitter(); }
            }

            /// <summary>
            ///     The commit message
            /// </summary>
            // ReSharper disable once UnusedMember.Local
            public string Message
            {
                get { return Item.Commit.getMessage(); }
            }

            /// <summary>
            ///     The commit date
            /// </summary>
            // ReSharper disable once UnusedMember.Local
            public DateTime Date
            {
                get { return Item.Commit.Date; }
            }

            /// <summary>
            ///     The commit action
            /// </summary>
            [Category("Description"), TypeConverter(typeof (ChangeActionConverter))]
            // ReSharper disable once UnusedMember.Local
            public acceptor.ChangeOperationEnum Action
            {
                get { return Item.Action; }
            }

            /// <summary>
            ///     The commit field change
            /// </summary>
            [Category("Description")]
            // ReSharper disable once UnusedMember.Local
            public string Field
            {
                get { return Item.Field; }
            }
        }

        /// <summary>
        ///     Instanciates the editor
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ChangeEditor();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public ChangeTreeNode(Change item, bool buildSubNodes)
            : base(item, buildSubNodes)
        {
        }
    }
}