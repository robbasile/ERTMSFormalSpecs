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

using System;
using System.Collections.Generic;
using DataDictionary.Types;
using Utils;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    /// <summary>
    /// The grabber is used to determine the context (either type or namespace)
    /// for a given location in the source text
    /// </summary>
    public class ContextGrabber : Visitor
    {
        /// <summary>
        /// The positions where the instance should be retrieved
        /// </summary>
        private int Position { get; set; }

        /// <summary>
        /// The instance (either namespace or type)
        /// </summary>
        public INamable Context { get; private set; }

        /// <summary>
        /// Gets the context (type or namespace) for a given position
        /// </summary>
        /// <param name="position">The position in the source text</param>
        /// <param name="node">The base node on which the search should be performed</param>
        /// <returns></returns>
        public INamable GetContext(int position, InterpreterTreeNode node)
        {
            Position = position;
            Context = null;

            visitInterpreterTreeNode(node);

            return Context;
        }

        /// <summary>
        /// Indicates if the node belongs to the search
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool ShouldCheck(InterpreterTreeNode node)
        {
            return (Context == null) && (node.Start <= Position && Position <= node.End);
        }

        /// <summary>
        /// Don't visit statements which do not cover the position given
        /// </summary>
        /// <param name="statement"></param>
        protected override void VisitStatement(Statement.Statement statement)
        {
            if (ShouldCheck(statement))
            {
                base.VisitStatement(statement);
            }
        }

        /// <summary>
        /// Don't visit expressions which do not cover the position given
        /// </summary>
        /// <param name="expression"></param>
        protected override void VisitExpression(Expression expression)
        {
            if (ShouldCheck(expression))
            {
                base.VisitExpression(expression);
            }
        }

        /// <summary>
        /// Gets the reference of the designator if it covers the position given
        /// </summary>
        /// <param name="designator"></param>
        protected override void VisitDesignator(Designator designator)
        {
            if (ShouldCheck(designator))
            {
                Type type = designator.Ref as Type;
                if ( Context == null && type != null )
                {
                    Context = type;
                }

                ITypedElement element = designator.Ref as ITypedElement;
                if (Context == null && element != null)
                {
                    Context = element;
                }

                ICallable callable = designator.Ref as ICallable;
                if (Context == null && callable != null)
                {
                    Context = callable;
                }

                NameSpace nameSpace = designator.Ref as NameSpace;
                if (Context == null && nameSpace != null)
                {
                    Context = nameSpace;
                }
            }
        }

        /// <summary>
        /// Gets the reference of the call if it covers the position given
        /// </summary>
        /// <param name="call"></param>
        protected override void VisitCall(Call call)
        {
            if (ShouldCheck(call))
            {
                bool stopLooking = false;

                // Provides the return type when we are at the end of the call
                if (Position == call.End)
                {
                    ICallable callable = call.Called.GetExpressionType() as ICallable;
                    if (callable != null)
                    {
                        stopLooking = true;
                        Context = callable.ReturnType;
                    }
                }
                else
                {
                    // Perform the analysis on the expressions providing the actual parameters for the call
                    foreach (Expression expression in call.ActualParameters)
                    {
                        if (expression != null && ShouldCheck(expression))
                        {
                            stopLooking = true;
                            VisitExpression(expression);
                            break;
                        }
                    }

                    foreach (KeyValuePair<Designator, Expression> pair in call.NamedActualParameters)
                    {
                        if (pair.Value != null && ShouldCheck(pair.Value))
                        {
                            stopLooking = true;
                            VisitExpression(pair.Value);
                            break;
                        }
                    }
                }

                // In all other cases, provide the function that is currently being called
                if (!stopLooking)
                {
                    Position = Math.Min(Position, call.Called.End);
                    VisitExpression(call.Called);
                }
            }
        }

        /// <summary>
        /// Gets the reference according to a struc expression
        /// </summary>
        /// <param name="structExpression"></param>
        protected override void VisitStructExpression(StructExpression structExpression)
        {
            if (ShouldCheck(structExpression))
            {
                bool stopLooking = false;

                // Perform the analysis on the expressions providing the values of the structure value
                foreach (KeyValuePair<Designator, Expression> pair in structExpression.Associations)
                {
                    if (pair.Value != null && ShouldCheck(pair.Value))
                    {
                        stopLooking = true;
                        VisitExpression(pair.Value);
                        break;
                    }
                }

                // In all other cases, provide the structure itself
                if (!stopLooking)
                {
                    Context = structExpression.Structure.Ref;
                }
            }
        }

        protected override void VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            if (ShouldCheck(unaryExpression))
            {
                if (unaryExpression.Expression != null && Position > unaryExpression.Expression.End)
                {
                    Context = unaryExpression.GetExpressionType();
                }
                else
                {
                    base.VisitUnaryExpression(unaryExpression);    
                }                
            }
        }
    }
}
