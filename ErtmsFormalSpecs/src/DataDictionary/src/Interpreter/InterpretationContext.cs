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
using DataDictionary.Variables;
using Utils;

namespace DataDictionary.Interpreter
{
    /// <summary>
    ///     An interpretation context
    /// </summary>
    public class InterpretationContext
    {
        /// <summary>
        ///     The instance on which the expression is checked
        /// </summary>
        public INamable Instance { get; set; }

        /// <summary>
        ///     The local scope for interpretation
        /// </summary>
        public SymbolTable LocalScope { get; private set; }

        /// <summary>
        ///     Indicates that default values should be used when no value is specifically provided
        /// </summary>
        public bool UseDefaultValue { get; set; }

        /// <summary>
        ///     Indicates that the enclosing element could cause side effects
        /// </summary>
        public bool HasSideEffects { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public InterpretationContext()
        {
            LocalScope = new SymbolTable();
            Instance = null;
            UseDefaultValue = true;
            HasSideEffects = false;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="instance">The instance on which interpretation should be performed</param>
        public InterpretationContext(INamable instance)
        {
            LocalScope = new SymbolTable();
            Instance = instance;
            UseDefaultValue = true;
            HasSideEffects = false;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="other">Copies the other interpretation context contents</param>
        public InterpretationContext(InterpretationContext other)
        {
            LocalScope = other.LocalScope;
            Instance = other.Instance;
            UseDefaultValue = other.UseDefaultValue;
            HasSideEffects = other.HasSideEffects;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="other">Copies the other interpretation context contents</param>
        /// <param name="instance">The evaluation instance</param>
        public InterpretationContext(InterpretationContext other, INamable instance)
        {
            LocalScope = other.LocalScope;
            Instance = instance;
            UseDefaultValue = true;
            HasSideEffects = other.HasSideEffects;
        }

        /// <summary>
        ///     Provides the list of parameters whose value is a placeholder
        /// </summary>
        /// <returns></returns>
        public List<Parameter> PlaceHolders()
        {
            return LocalScope.PlaceHolders();
        }

        /// <summary>
        ///     Provides the actual variable which corresponds to this parameter on the stack
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IVariable FindOnStack(Parameter parameter)
        {
            return LocalScope.find(parameter);
        }

        /// <summary>
        ///     Provides the current stack index
        /// </summary>
        public int StackIndex
        {
            get { return LocalScope.Index; }
        }
    }
}
