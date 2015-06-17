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
using DataDictionary.Types;

namespace DataDictionary.Interpreter.Refactor
{
    /// <summary>
    ///     This visitor is used to handle refactoring of expressions.
    /// </summary>
    public class RelocateTree : BaseRefactorTree
    {
        /// <summary>
        ///     The new location of the element
        /// </summary>
        private ModelElement BaseLocation { get; set; }

        protected override void VisitDerefExpression(DerefExpression derefExpression)
        {
            ModelElement backup = BaseLocation;
            foreach (Expression expression in derefExpression.Arguments)
            {
                if (expression != null)
                {
                    ModelElement model = expression.Ref as ModelElement;
                    if (model != null)
                    {
                        string referenceName = model.ReferenceName(BaseLocation);
                        ReplaceText(referenceName, expression.Start, expression.End);
                        break;
                    }
                    else
                    {
                        BaseLocation = backup;
                        VisitExpression(expression);
                        BaseLocation = expression.GetExpressionType();
                    }

                    if (expression.Ref != null)
                    {
                        ITypedElement typedElement = expression.Ref as ITypedElement;
                        if (typedElement != null && typedElement.Type != null)
                        {
                            BaseLocation = typedElement.Type;
                        }
                        else
                        {
                            BaseLocation = expression.Ref as ModelElement;
                        }
                    }
                }
            }

            BaseLocation = backup;
        }

        protected override void VisitDesignator(Designator designator)
        {
            ModelElement model = designator.Ref as ModelElement;
            if (model != null && !designator.IsPredefined())
            {
                ReplaceText(model.ReferenceName(BaseLocation), designator.Start, designator.End);
            }
        }

        protected override void VisitStructExpression(StructExpression structExpression)
        {
            if (structExpression.Structure != null)
            {
                VisitExpression(structExpression.Structure);
            }

            ModelElement backup = BaseLocation;
            BaseLocation = structExpression.Structure.GetExpressionType();
            foreach (KeyValuePair<Designator, Expression> pair in structExpression.Associations)
            {
                if (pair.Key != null)
                {
                    VisitDesignator(pair.Key);
                }
                if (pair.Value != null)
                {
                    VisitExpression(pair.Value);
                }
            }
            BaseLocation = backup;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="baseLocation"></param>
        public RelocateTree(ModelElement baseLocation)
        {
            BaseLocation = baseLocation;
        }
    }
}