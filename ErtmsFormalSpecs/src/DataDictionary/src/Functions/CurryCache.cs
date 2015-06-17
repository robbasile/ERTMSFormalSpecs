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
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;

namespace DataDictionary.Functions
{
    /// <summary>
    ///     An association between a value and its explanation
    /// </summary>
    public class ExplainedValue
    {
        /// <summary>
        ///     The value
        /// </summary>
        public IValue Value { get; private set; }

        /// <summary>
        ///     The value's computation explanation
        /// </summary>
        public ExplanationPart Explanation { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="explanation"></param>
        public ExplainedValue(IValue value, ExplanationPart explanation)
        {
            Value = value;
            Explanation = explanation;
        }
    }

    /// <summary>
    ///     Caches the result of a function in a Curry-like fashion
    /// </summary>
    public class CurryCache
    {
        /// <summary>
        ///     The function for which this cache is built
        /// </summary>
        private Function Function { get; set; }

        /// <summary>
        ///     A curried cache
        /// </summary>
        private class FunctionCache : Dictionary<IValue, FunctionCache>
        {
            /// <summary>
            ///     The value associated to the last parameter of the function
            /// </summary>
            public ExplainedValue Value { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            public FunctionCache() : base()
            {
            }
        }

        /// <summary>
        ///     Provides the value
        /// </summary>
        private FunctionCache Curry { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public CurryCache(Function function)
        {
            Function = function;
            Curry = new FunctionCache();
        }


        /// <summary>
        ///     Gets the value of this curry cache according to the parameter association
        /// </summary>
        /// <param name="association"></param>
        /// <returns></returns>
        public ExplainedValue GetValue(Dictionary<Actual, IValue> association)
        {
            ExplainedValue retVal = null;

            FunctionCache current = Curry;
            foreach (IValue val in OrderedParameters(association))
            {
                FunctionCache next;
                if (current.TryGetValue(val, out next))
                {
                    current = next;
                }
                else
                {
                    current = null;
                    break;
                }
            }

            if (current != null)
            {
                retVal = current.Value;
            }

            return retVal;
        }

        /// <summary>
        ///     Sets the value according to the parameter association
        /// </summary>
        /// <param name="association"></param>
        /// <param name="value"></param>
        public void SetValue(Dictionary<Actual, IValue> association, IValue value, ExplanationPart explanation)
        {
            FunctionCache current = Curry;
            foreach (IValue val in OrderedParameters(association))
            {
                FunctionCache next;
                if (!current.TryGetValue(val, out next))
                {
                    next = new FunctionCache();
                    current.Add(val, next);
                }
                current = next;
            }

            current.Value = new ExplainedValue(value, explanation);
        }

        /// <summary>
        ///     Provides the parameter values ordered by the formal parameter they are associated with
        /// </summary>
        /// <param name="association"></param>
        /// <returns></returns>
        private List<IValue> OrderedParameters(Dictionary<Actual, IValue> association)
        {
            List<IValue> retVal = new List<IValue>();

            foreach (Parameter p in Function.FormalParameters)
            {
                // Order the actual according to the function parameter 
                foreach (KeyValuePair<Actual, IValue> pair in association)
                {
                    if (pair.Key.Parameter == p)
                    {
                        retVal.Add(pair.Value);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Clears the cache
        /// </summary>
        public void Clear()
        {
            Curry.Clear();
        }
    }
}