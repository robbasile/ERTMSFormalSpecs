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
using DataDictionary.Values;
using DataDictionary.Variables;
using Collection = DataDictionary.Types.Collection;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Allocates an element in a collection
    /// </summary>
    public class Allocate : PredefinedFunction
    {
        /// <summary>
        ///     The value which is checked
        /// </summary>
        public Parameter Collection { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public Allocate(EFSSystem efsSystem)
            : base(efsSystem, "Allocate")
        {
            Collection = (Parameter) acceptor.getFactory().createParameter();
            Collection.Name = "Collection";
            Collection.Type = EFSSystem.GenericCollection;
            Collection.setFather(this);
            FormalParameters.Add(Collection);
        }

        /// <summary>
        ///     The return type of the available function
        /// </summary>
        public override Type ReturnType
        {
            get { return EFSSystem.AnyType; }
        }

        /// <summary>
        ///     Provides the value of the function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actuals">the actual parameters values</param>
        /// <param name="explain"></param>
        /// <returns>The value for the function application</returns>
        public override IValue Evaluate(InterpretationContext context, Dictionary<Actual, IValue> actuals,
            ExplanationPart explain)
        {
            IValue retVal = null;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            ListValue value = context.findOnStack(Collection) as ListValue;
            if (value != null)
            {
                Collection collectionType = value.Type as Collection;
                if (collectionType != null && collectionType.Type != null)
                {
                    Type elementType = collectionType.Type;

                    int i = 0;
                    while (i < value.Val.Count && value.Val[i] != EFSSystem.EmptyValue)
                    {
                        i += 1;
                    }

                    if (i < value.Val.Count)
                    {
                        retVal = elementType.DefaultValue;
                        value.Val[i] = retVal;
                    }
                    else
                    {
                        AddError("Cannot allocate element in list : list full");
                    }
                }
            }
            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}