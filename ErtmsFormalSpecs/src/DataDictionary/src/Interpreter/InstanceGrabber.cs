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

using DataDictionary.Types;
using Utils;

namespace DataDictionary.Interpreter
{
    /// <summary>
    /// The instance grabber is used to determine the instance type for a given location in the source text
    /// </summary>
    public class InstanceGrabber : Visitor
    {
        /// <summary>
        /// The positions where the instance should be retrieved
        /// </summary>
        private int Position { get; set; }

        /// <summary>
        /// The instance (either namespace or type)
        /// </summary>
        public INamable Instance { get; private set; }

        /// <summary>
        /// Grabs the instance (type or namespace) for a given position
        /// </summary>
        /// <param name="position">The position in the source text</param>
        /// <param name="node">The base node on which the search should be performed</param>
        /// <returns></returns>
        public INamable GrabInstanceType(int position, InterpreterTreeNode node)
        {
            Position = position;
            Instance = null;

            visitInterpreterTreeNode(node);

            return Instance;
        }

        /// <summary>
        /// Don't visit statements which do not cover the position given
        /// </summary>
        /// <param name="statement"></param>
        protected override void VisitStatement(Statement.Statement statement)
        {
            if (Instance == null)
            {
                if (statement.Start >= Position && statement.End <= Position)
                {
                    base.VisitStatement(statement);
                }
            }
        }

        /// <summary>
        /// Don't visit expressions which do not cover the position given
        /// </summary>
        /// <param name="expression"></param>
        protected override void VisitExpression(Expression expression)
        {
            if (Instance == null)
            {
                if (expression.Start >= Position && expression.End <= Position)
                {
                    base.VisitExpression(expression);
                }
            }
        }

        /// <summary>
        /// Gets the reference of the designator if it covers the position given
        /// </summary>
        /// <param name="designator"></param>
        protected override void VisitDesignator(Designator designator)
        {
            if (Instance == null)
            {
                if (designator.Start >= Position && designator.End <= Position)
                {
                    ITypedElement element = designator.Ref as ITypedElement;
                    if (element != null)
                    {
                        Instance = element.Type;
                    }

                    NameSpace nameSpace = designator.Ref as NameSpace;
                    if (nameSpace != null)
                    {
                        Instance = nameSpace;
                    }
                }
            }
        }
    }
}
