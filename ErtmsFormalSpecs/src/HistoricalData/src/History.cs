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

using System.Collections;
using System.Collections.Generic;
using HistoricalData.Generated;

namespace HistoricalData
{
    /// <summary>
    ///     The complete history
    /// </summary>
    public class History : Generated.History
    {
        /// <summary>
        ///     The file name of the history file
        /// </summary>
        public const string HISTORY_FILE_NAME = "HistoryCache.hst";

        /// <summary>
        ///     All the commits in this history
        /// </summary>
        public ArrayList Commits
        {
            get
            {
                if (allCommits() == null)
                {
                    setAllCommits(new ArrayList());
                }
                return allCommits();
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public History()
            : base()
        {
            Blame = new Dictionary<string, SortedSet<Change>>();
        }

        /// <summary>
        ///     Traces back all changes related to each single element
        /// </summary>
        protected Dictionary<string, SortedSet<Change>> Blame { get; private set; }

        private class BlameVisitor : Visitor
        {
            /// <summary>
            ///     The blame cache to update
            /// </summary>
            private Dictionary<string, SortedSet<Change>> Blame = new Dictionary<string, SortedSet<Change>>();

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="blame"></param>
            public BlameVisitor(Dictionary<string, SortedSet<Change>> blame)
            {
                Blame = blame;
            }

            /// <summary>
            ///     Updates the Blame dictionary with the current change
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(Generated.Change obj, bool visitSubNodes)
            {
                Change change = (Change) obj;

                SortedSet<Change> changes;
                if (!Blame.TryGetValue(change.Guid, out changes))
                {
                    changes = new SortedSet<Change>();
                    Blame[change.Guid] = changes;
                }
                changes.Add(change);

                base.visit(obj, visitSubNodes);
            }
        }

        public void UpdateBlame()
        {
            BlameVisitor visitor = new BlameVisitor(Blame);
            visitor.visit(this, true);
        }

        /// <summary>
        ///     Indicates that the corresponding commit already exists in the history
        /// </summary>
        /// <param name="sha"></param>
        /// <returns>True if the commit has been found</returns>
        public bool CommitExists(string sha)
        {
            bool retVal = false;

            foreach (Commit commit in Commits)
            {
                if (commit.Sha == sha)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Appends a new commit in the history.
        ///     IF the commit is already present, it replaces that commit instead
        /// </summary>
        /// <param name="commit"></param>
        public void AppendOrReplaceCommit(Commit commit)
        {
            int found = -1;
            int index = 0;
            while (index < Commits.Count && found < 0)
            {
                Commit other = (Commit) Commits[index];
                if (other.Sha == commit.Sha)
                {
                    found = index;
                }
                index += 1;
            }

            Commits.Insert(index, commit);
            commit.setFather(this);
            if (found >= 0)
            {
                Commits.RemoveAt(found);
            }
        }

        /// <summary>
        ///     Provides the commit which corresponds to the Sha provided
        /// </summary>
        /// <param name="sha">The commit Sha to find</param>
        /// <returns>null if no Commit correspond to the Sha</returns>
        public Commit Commit(string sha)
        {
            Commit retVal = null;

            foreach (Commit commit in Commits)
            {
                if (commit.Sha == sha)
                {
                    retVal = commit;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the ordered changes on a given model element
        /// </summary>
        /// <param name="guid">The guid of the object where changes should be found</param>
        /// <returns></returns>
        public SortedSet<Change> GetChanges(string guid)
        {
            SortedSet<Change> retVal;

            if (!Blame.TryGetValue(guid, out retVal))
            {
                retVal = new SortedSet<Change>();
            }

            return retVal;
        }

        public void save()
        {
            save(HISTORY_FILE_NAME);
        }
    }
}