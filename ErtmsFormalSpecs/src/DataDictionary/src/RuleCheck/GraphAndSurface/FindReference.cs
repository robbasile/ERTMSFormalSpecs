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

namespace DataDictionary.RuleCheck.GraphAndSurface
{
    /// <summary>
    /// Used to find all nodes in an expression which reference a given model element
    /// </summary>
    public class FindReference<T> : Visitor
        where T : InterpreterTreeNode 
    {
        /// <summary>
        /// The model element to find
        /// </summary>
        protected ModelElement ModelElement{ get; private set; }

        /// <summary>
        /// The references to that model element 
        /// </summary>
        public List<T> References{ get; private set; } 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modelElement"></param>
        public FindReference(ModelElement modelElement)
        {
            ModelElement = modelElement;
            References= new List<T>();
        }

        /// <summary>
        /// Gathers all the references to the model element
        /// </summary>
        public void GatherReferences(InterpreterTreeNode node)
        {
            if (node != null)
            {
                visitInterpreterTreeNode(node);
            }
        }

        /// <summary>
        /// Provides all nodes which match the generic type and reference the designated model element
        /// </summary>
        /// <param name="interpreterTreeNode"></param>
        protected override void visitInterpreterTreeNode(InterpreterTreeNode interpreterTreeNode)
        {
            T node = interpreterTreeNode as T;
            if (node != null)
            {
                IReference reference = node as IReference;
                if (reference != null)
                {
                    if (reference.Ref == ModelElement)
                    {
                        References.Add(node);
                    }
                }
            }

            base.visitInterpreterTreeNode(interpreterTreeNode);
        }
    }
}
