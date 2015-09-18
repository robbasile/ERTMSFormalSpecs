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
using DataDictionary.Interpreter;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Function defined on graphs
    /// </summary>
    public abstract class FunctionOnGraph : PredefinedFunction
    {
        /// <summary>
        ///     The return type of this function
        /// </summary>
        public Function Returns { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        /// <param name="name">the name of the function</param>
        public FunctionOnGraph(EfsSystem efsSystem, string name)
            : base(efsSystem, name)
        {
            Returns = (Function) acceptor.getFactory().createFunction();
            Returns.Name = Name + "ReturnType";
            Returns.ReturnType = EFSSystem.DoubleType;
            Returns.setFather(this);

            Parameter returnTypeParam = (Parameter) acceptor.getFactory().createParameter();
            returnTypeParam.Name = Name + "ReturnTypeParam";
            returnTypeParam.Type = EFSSystem.DoubleType;
            returnTypeParam.setFather(Returns);
            Returns.appendParameters(returnTypeParam);
        }

        /// <summary>
        ///     The return type of the available function
        /// </summary>
        public override Type ReturnType
        {
            get { return Returns; }
        }
    }
}