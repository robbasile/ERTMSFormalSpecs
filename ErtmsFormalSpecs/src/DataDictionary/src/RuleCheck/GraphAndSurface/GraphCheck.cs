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
using DataDictionary.Functions.PredefinedFunctions;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.ListOperators;
using Utils;

namespace DataDictionary.RuleCheck.GraphAndSurface
{
    public class GraphCheck
    {
        /// <summary>
        /// The enclosing graph and surface checker
        /// </summary>
        private GraphAndSurfaceCheck Enclosing { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enclosing"></param>
        public GraphCheck(GraphAndSurfaceCheck enclosing)
        {
            Enclosing = enclosing;
        }

        /// <summary>
        /// Checks that the parameter
        /// </summary>
        /// <param name="call"></param>
        /// <param name="parameter">The parameter creating the graph value</param>
        /// <param name="distance">The distance parameter</param>
        public void CheckCall(Call call, Parameter parameter, Parameter distance)
        {
            Expression graphFunctionExpression;
            Functions.Function graphFunction = null;
            if (call.ParameterAssociation.TryGetValue(parameter, out graphFunctionExpression))
            {
                CheckExpression(graphFunctionExpression, distance);
            }
        }

        /// <summary>
        /// Ensures that a function can be transformed into a graph
        /// </summary>
        /// <param name="function">The functions to verify that it is graph-able</param>
        private void CheckFunction(Functions.Function function)
        {
            if (function != null)
            {
                Parameter distance = Enclosing.GetDistanceParameter(function);
                if (distance != null)
                {
                    CheckFunction(function, distance);
                }
                else
                {
                    function.AddRuleCheckMessage(
                        RuleChecksEnum.SemanticAnalysisError,
                        ElementLog.LevelEnum.Error, 
                        "Graph cannot be computed on the function, since it does not depend on a parameter which can interpreted as a distance");
                }
            }
        }

        /// <summary>
        /// Ensures that a function can be transformed into a graph
        /// </summary>
        /// <param name="function">The functions to verify that it is graph-able</param>
        /// <param name="distance">The x parameter on which the graph is built</param>
        public void CheckFunction(Functions.Function function, Parameter distance)
        {
            if (Enclosing.IsDouble(distance))
            {
                foreach (Functions.Case cas in function.Cases)
                {
                    CheckCase(cas, distance);
                }
            }
            else
            {
                function.AddRuleCheckMessage(
                    RuleChecksEnum.SemanticAnalysisError,
                    ElementLog.LevelEnum.Error,
                    "Parameter " + distance.Name + " is not a valid range for a graph");
            }
        }

        /// <summary>
        /// Ensures that the case is valid for a graph function, according to the parameter provided
        /// </summary>
        /// <param name="cas"></param>
        /// <param name="distance">The x parameter on which the graph is built</param>
        private void CheckCase(Functions.Case cas, Parameter distance)
        {
            foreach (Rules.PreCondition preCondition in cas.PreConditions)
            {
                CheckPrecondition(preCondition.Expression, distance);
            }

            CheckExpression(cas.Expression, distance);
        }

        /// <summary>
        /// Ensure that the precondition is valid to create a graph
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="distance">The x parameter on which the graph is built</param>
        private void CheckPrecondition(Expression expression, Parameter distance)
        {
            bool retVal = false;

            if (!Enclosing.IsConstant(expression, distance))
            {
                BinaryExpression binaryExpression = expression as BinaryExpression;
                if (binaryExpression != null)
                {
                    if (binaryExpression.Operation == BinaryExpression.Operator.Greater ||
                        binaryExpression.Operation == BinaryExpression.Operator.GreaterOrEqual ||
                        binaryExpression.Operation == BinaryExpression.Operator.Less ||
                        binaryExpression.Operation == BinaryExpression.Operator.LessOrEqual)
                    {
                        if (!Enclosing.IsConstant(binaryExpression.Left))
                        {
                            if (binaryExpression.Left.Ref != distance)
                            {
                                expression.AddError(
                                    binaryExpression.Left + " : No operation can be performed on the distance parameter to be able to compute a graph",
                                    RuleChecksEnum.SemanticAnalysisError);
                            }

                            if (!Enclosing.IsConstant(binaryExpression.Right))
                            {
                                expression.AddError(
                                    binaryExpression.Right + " should not rely on the distance, since left expression already does to be able to compute a graph",
                                    RuleChecksEnum.SemanticAnalysisError);                                
                            }
                        }
                        else
                        {
                            // Left is constant => Right is not constant
                            if (binaryExpression.Right.Ref != distance)
                            {
                                expression.AddError(
                                    binaryExpression.Right + " : No operation can be performed on the distance parameter to be able to compute a graph",
                                    RuleChecksEnum.SemanticAnalysisError);
                            }
                        }
                    }
                    else
                    {
                        expression.AddError(
                            "Only comparison can be used to be able to compute a graph",
                            RuleChecksEnum.SemanticAnalysisError);
                    }
                }
            }
        }

        /// <summary>
        /// Checks that the value provided is valid to compute a graph
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="distance">The parameter for the distance</param>
        private void CheckExpression(Expression expression, Parameter distance)
        {
            BinaryExpression binaryExpression = expression as BinaryExpression;
            ReduceExpression reduceExpression = expression as ReduceExpression;
            FunctionExpression functionExpression = expression as FunctionExpression;
            Call call = expression as Call;
            DerefExpression derefExpression = expression as DerefExpression;

            if (binaryExpression != null)
            {
                if (binaryExpression.Operation == BinaryExpression.Operator.Add ||
                    binaryExpression.Operation == BinaryExpression.Operator.Sub || 
                    binaryExpression.Operation == BinaryExpression.Operator.Mult ||
                    binaryExpression.Operation == BinaryExpression.Operator.Div )
                {
                    CheckExpression(binaryExpression.Left, distance);
                    CheckExpression(binaryExpression.Right, distance);

                    if (binaryExpression.Operation == BinaryExpression.Operator.Mult ||
                        binaryExpression.Operation == BinaryExpression.Operator.Div)
                    {
                        if (!Enclosing.IsConstant(binaryExpression.Left, distance) &&
                            !Enclosing.IsConstant(binaryExpression.Right, distance))
                        {
                            expression.AddError(
                                "* and / should be performed with a constant factor to be able to compute a graph",
                                RuleChecksEnum.SemanticAnalysisError);                            
                        }
                    }
                }
                else
                {
                    expression.AddError(
                        "Only +, -, *, / can be used to be able to compute a graph",
                        RuleChecksEnum.SemanticAnalysisError);
                }
            }
            else if (reduceExpression != null)
            {
                CheckExpression(reduceExpression.IteratorExpression, distance);
                CheckExpression(reduceExpression.InitialValue, distance);

                if (!Enclosing.IsConstant(reduceExpression.ListExpression, distance))
                {
                    expression.AddError(
                        "List expression should not rely on the distance to be able to compute a graph",
                        RuleChecksEnum.SemanticAnalysisError);
                }

                if (!Enclosing.IsConstant(reduceExpression.Condition, distance))
                {
                    expression.AddError(
                        "Condition expression should not rely on the distance to be able to compute a graph",
                        RuleChecksEnum.SemanticAnalysisError);
                }
            }
            else if (functionExpression != null)
            {
                Parameter newDistance = Enclosing.GetDistanceParameter(functionExpression);
                if (newDistance != null)
                {
                    CheckExpression(functionExpression.Expression, newDistance);
                }
                else
                {
                    expression.AddError(
                        "Function expression should declare a distance parameter to be able to compute a graph",
                        RuleChecksEnum.SemanticAnalysisError);                    
                }
            }
            else if (call != null)
            {
                if (call.Called.Ref is Cast)
                {
                    if (call.ActualParameters.Count == 1)
                    {
                        Expression casted = call.ActualParameters[0];
                        CheckExpression(casted, distance);
                    }
                    else
                    {
                        expression.AddError(
                            "Cast function should only take a single parameter",
                            RuleChecksEnum.SemanticAnalysisError);                                            
                    }
                }
                else if (call.Called.Ref == EfsSystem.Instance.MinPredefinedFunction ||
                         call.Called.Ref == EfsSystem.Instance.MaxPredefinedFunction ||
                         call.Called.Ref == EfsSystem.Instance.OverridePredefinedFunction ||
                         call.Called.Ref == EfsSystem.Instance.AddIncrementPredefinedFunction)
                {
                    Expression left = call.AllParameters[0];
                    Expression right = call.AllParameters[1];
                    CheckExpression(left, distance);
                    CheckExpression(right, distance);

                    bool noOperationOnDistance = true;
                    foreach (Expression callParameter in call.AllParameters)
                    {
                        noOperationOnDistance = noOperationOnDistance && Enclosing.IsConstantOrParameter(callParameter, distance);
                    }

                    if (!noOperationOnDistance)
                    {
                        expression.AddError(
                            "No operation can be performed on the distance parameter to be able to compute a graph",
                            RuleChecksEnum.SemanticAnalysisError);                                                                    
                    }
                }
                else
                {
                    bool noOperationOnDistance = true;

                    Parameter newDistance = null;
                    if (call.ParameterAssociation != null)
                    {
                        foreach (KeyValuePair<Parameter, Expression> callParameter in call.ParameterAssociation)
                        {
                            if (callParameter.Value.Ref == distance)
                            {
                                newDistance = callParameter.Key;
                            }
                            else
                            {
                                noOperationOnDistance = noOperationOnDistance && Enclosing.IsConstant(callParameter.Value, distance);
                            }
                        }
                    }
                    else
                    {
                        // This seems to be a HaCk
                        CheckExpression(call.Called, distance);
                    }

                    if (!noOperationOnDistance)
                    {
                        expression.AddError(
                            "No operation can be performed on the distance parameter to be able to compute a graph",
                            RuleChecksEnum.SemanticAnalysisError);
                    }

                    if (newDistance != null)
                    {
                        Functions.Function function = call.Called.Ref as Functions.Function;
                        if (function != null)
                        {
                            CheckFunction(function, newDistance);
                        }
                    }
                }
            }
            else if (derefExpression != null)
            {
                Functions.Function derefFunction = derefExpression.Ref as Functions.Function;
                if (derefFunction != null)
                {
                    CheckFunction(derefFunction);
                }
                else
                {
                    if (!Enclosing.IsConstant(expression, distance))
                    {
                        expression.AddError(
                            expression + " should not rely on distance to be able to compute a graph",
                            RuleChecksEnum.SemanticAnalysisError);                        
                    }
                }
            }
            else
            {
                if (!Enclosing.IsConstant(expression, distance))
                {
                    expression.AddError(
                        expression + " should not rely on distance to be able to compute a graph",
                        RuleChecksEnum.SemanticAnalysisError);
                }
            }
        }
    }
}