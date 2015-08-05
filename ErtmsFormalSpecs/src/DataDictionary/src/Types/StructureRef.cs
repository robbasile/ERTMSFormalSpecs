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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;

namespace DataDictionary.Types
{
    public class StructureRef : Generated.StructureRef, IExpressionable
    {
        /// <summary>
        ///     The type associated to this StructureRef
        /// </summary>
        public Structure ReferencedStructure
        {
            get
            {
                Structure retVal = null;

                Compile();
                if (ReferencedStructureExpression != null)
                {
                    retVal = ReferencedStructureExpression.Ref as Structure;
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Computes the recursive list of the interfaces implemented
        ///     by the structure corresponding to this StructureRef
        /// </summary>
        public List<Structure> ImplementedStructures
        {
            get
            {
                List<Structure> result = new List<Structure>();
                if (ReferencedStructure != null)
                {
                    result.Add(ReferencedStructure);
                    foreach (Structure aStructure in ReferencedStructure.Interfaces)
                    {
                        result.AddRange(aStructure.ImplementedStructures);
                    }
                }
                return result;
            }
        }

        /// <summary>
        ///     The expression text for this expressionable
        /// </summary>
        public override string ExpressionText
        {
            get { return getName(); }
            set { setName(value); }
        }

        /// <summary>
        ///     The expression which references the structure
        /// </summary>
        private Expression ReferencedStructureExpression { get; set; }

        /// <summary>
        ///     The tree which corresponds to the expression text
        /// </summary>
        public InterpreterTreeNode Tree
        {
            get
            {
                Compile();
                return ReferencedStructureExpression;
            }
        }

        /// <summary>
        ///     Indicates that the expression is valid for this IExpressionable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool checkValidExpression(string expression)
        {
            bool retVal = true;

            if (ReferencedStructure == null)
            {
                AddError("Does not references to a structure");
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        ///     Clears the expression tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
            ReferencedStructureExpression = null;
        }

        /// <summary>
        ///     Creates the tree according to the expression text
        /// </summary>
        public InterpreterTreeNode Compile()
        {
            if (ReferencedStructureExpression == null)
            {
                ReferencedStructureExpression = EFSSystem.Parser.Expression(this, ExpressionText, IsType.INSTANCE);
                if (ReferencedStructureExpression != null)
                {
                    foreach (Usage usage in ReferencedStructureExpression.StaticUsage.AllUsages)
                    {
                        usage.Mode = Usage.ModeEnum.Interface;
                    }
                }
            }

            return ReferencedStructureExpression;
        }

        /// <summary>
        /// Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static StructureRef CreateDefault(ICollection enclosingCollection)
        {
            StructureRef retVal = (StructureRef)acceptor.getFactory().createStructureRef();
            retVal.Name = "Interface" + GetElementNumber(enclosingCollection);

            return retVal;
        }

    }
}