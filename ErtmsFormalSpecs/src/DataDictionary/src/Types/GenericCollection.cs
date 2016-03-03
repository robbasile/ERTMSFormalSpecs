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
namespace DataDictionary.Types
{

    /// <summary>
    ///     A generic collection
    /// </summary>
    public class GenericCollection : Collection
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public GenericCollection(EfsSystem efsSystem)
        {
            Enclosing = efsSystem;
            setMaxSize(int.MaxValue);
        }

        /// <summary>
        ///     The type of the elements in this collection
        /// </summary>
        public override Type Type
        {
            get { return EFSSystem.AnyType; }
        }
    }
}
