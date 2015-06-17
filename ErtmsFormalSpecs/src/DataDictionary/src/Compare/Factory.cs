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

namespace DataDictionary.Compare
{
    /// <summary>
    ///     The Historical data specific factory for DataDictionary
    /// </summary>
    public class Factory : HistoricalData.Factory
    {
        public override Change createChange()
        {
            Diff retVal = new Diff();
            return retVal;
        }

        public override Commit createCommit()
        {
            VersionDiff retVal = new VersionDiff();

            return retVal;
        }

        public override HistoricalData.Generated.History createHistory()
        {
            History retVal = new History();

            return retVal;
        }

        /// <summary>
        ///     The singleton instance
        /// </summary>
        private static Factory __instance = new Factory();

        /// <summary>
        ///     The singleton instance
        /// </summary>
        public new static Factory INSTANCE
        {
            get { return __instance; }
        }
    }
}