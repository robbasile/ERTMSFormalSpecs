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
using DataDictionary.Constants;
using DataDictionary.Types;
using Utils;

namespace DataDictionary.Interpreter.Refactor
{
    /// <summary>
    ///     This visitor is used to handle refactoring of expressions.
    /// </summary>
    public class RefactorTree : BaseRefactorTree
    {
        /// <summary>
        ///     The model element which should be refactored
        /// </summary>
        private ModelElement Ref { get; set; }

        /// <summary>
        ///     The user of the element
        /// </summary>
        private ModelElement User { get; set; }

        /// <summary>
        ///     Indicates that all references should be updated
        /// </summary>
        private bool ReplaceAllReferences
        {
            get { return Ref == null; }
        }

        /// <summary>
        ///     The start index where prefixes should be removed
        /// </summary>
        private int StartRemove { get; set; }

        /// <summary>
        ///     The start index where prefixes should be removed
        /// </summary>
        private int EndRemove { get; set; }

        /// <summary>
        ///     Resets the StartRemove and EndRemove indexes to their default value (meaning : no removal)
        /// </summary>
        private void ResetRemoveIndexes()
        {
            StartRemove = int.MaxValue;
            EndRemove = int.MinValue;
        }

        /// <summary>
        ///     Indicates that the prefix should be removed
        /// </summary>
        private bool ShouldRemovePrefix
        {
            get { return StartRemove != int.MaxValue && EndRemove != int.MinValue && StartRemove != EndRemove; }
        }

        /// <summary>
        ///     Removes the prefix according to the StartRemove and EndRemove values
        /// </summary>
        /// <param name="treeNode">The tree node to replace</param>
        /// <param name="referencedElement">The element that should be replaced</param>
        /// <returns>if the replacement has been done</returns>
        private bool ReplaceNonTerminal(InterpreterTreeNode treeNode, ModelElement referencedElement)
        {
            bool retVal;

            if (ReplaceAllReferences)
            {
                // Only do the repleacement for elements defined in a dictionary
                retVal = EnclosingFinder<Dictionary>.find(referencedElement) != null;
            }
            else
            {
                // Check that the reference corresponds to the specific reference for this visitor
                retVal = referencedElement == Ref;
            }

            if (retVal)
            {
                // Removes the prefix
                if (ShouldRemovePrefix)
                {
                    ReplaceText("", StartRemove, EndRemove);
                    ResetRemoveIndexes();
                }

                string replacementValue = referencedElement.ReferenceName(User);
                ReplaceText(replacementValue, treeNode.Start, treeNode.End);
            }

            return retVal;
        }

        protected override void VisitDerefExpression(DerefExpression derefExpression)
        {
            ModelElement context = User;

            ResetRemoveIndexes();
            int i = 1;
            foreach (Expression expression in derefExpression.Arguments)
            {
                if (expression != null)
                {
                    bool replaced = false;
                    if (ReplaceAllReferences)
                    {
                        if (i == derefExpression.Arguments.Count)
                        {
                            replaced = ReplaceNonTerminal(expression, expression.Ref as ModelElement);
                        }
                    }
                    else
                    {
                        if (expression is Call && expression.Ref is Structure)
                        {
                            replaced = ReplaceNonTerminal(expression, null);
                        }
                        else
                        {
                            replaced = ReplaceNonTerminal(expression, expression.Ref as ModelElement);                            
                        }
                    }

                    if (!replaced)
                    {
                        if (expression.Ref is NameSpace || expression.Ref is StateMachine || expression.Ref is State)
                        {
                            // Remove all namespace prefixes, they will be taken into account in ReferenceName function
                            StartRemove = Math.Min(expression.Start, StartRemove);
                            EndRemove = Math.Max(expression.End + 1, EndRemove);
                        }
                        else
                        {
                            VisitExpression(expression);

                            ResetRemoveIndexes();
                            User = expression.GetExpressionType();
                        }
                    }
                }
                i += 1;
            }
            User = context;
        }

        protected override void VisitDesignator(Designator designator)
        {
            if (!designator.IsPredefined())
            {
                ReplaceNonTerminal(designator, designator.Ref as ModelElement);
            }
        }

        /// <summary>
        ///     Visits a struct expression
        /// </summary>
        /// <param name="structExpression"></param>
        protected override void VisitStructExpression(StructExpression structExpression)
        {
            ModelElement backup = User;
            Structure structure = null;
            if (structExpression.Structure != null)
            {
                structure = structExpression.Structure.GetExpressionType() as Structure;
                VisitExpression(structExpression.Structure);
            }

            foreach (KeyValuePair<Designator, Expression> pair in structExpression.Associations)
            {
                ResetRemoveIndexes();
                if (pair.Key != null)
                {
                    User = structure;
                    VisitDesignator(pair.Key);
                    User = backup;
                }

                ResetRemoveIndexes();
                if (pair.Value != null)
                {
                    VisitExpression(pair.Value);
                }
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="reference">
        ///     The specific reference to replace. If null, all references to a dictionary element should be
        ///     updated
        /// </param>
        /// <param name="user"></param>
        public RefactorTree(ModelElement reference, ModelElement user)
        {
            Ref = reference;
            User = user;
        }
    }
}