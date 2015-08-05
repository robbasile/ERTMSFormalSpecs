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

using HistoricalData.Generated;
using Change = HistoricalData.Change;
using Commit = HistoricalData.Commit;

namespace DataDictionary.Compare
{
    /// <summary>
    ///     Stores a difference between two versions of a dictionary
    /// </summary>
    public class Diff : Change
    {
        /// <summary>
        ///     The element affected by that change
        /// </summary>
        public ModelElement Model
        {
            get { return GuidCache.INSTANCE.GetModel(Guid); }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="action"></param>
        public Diff(ModelElement model, acceptor.ChangeOperationEnum action, string field = "", string before = "",
            string after = "") :
                base(model.Guid, action, field, before, after)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Diff()
            : base()
        {
        }

        /// <summary>
        ///     Marks the model according to this diff
        /// </summary>
        public void markModel()
        {
            switch (Action)
            {
                case acceptor.ChangeOperationEnum.aAdd:
                    Model.AddInfo("ADDED");
                    break;

                case acceptor.ChangeOperationEnum.aRemove:
                    Model.AddInfo("REMOVED " + Field + ", previously was : " + Before);
                    break;

                case acceptor.ChangeOperationEnum.aChange:
                    Model.AddInfo("CHANGED " + Field + " \nFROM : " + Before + " \nTO : " + After + "\n");
                    break;
            }
        }
    }

    public class VersionDiff : Commit
    {
        /// <summary>
        ///     Marks the model according to the version changes
        /// </summary>
        public void MarkVersionChanges(Dictionary dictionary)
        {
            MarkingHistory.PerformMark(() =>
            {
                foreach (Diff diff in Changes)
                {
                    diff.markModel();
                }
            });
        }
    }
}