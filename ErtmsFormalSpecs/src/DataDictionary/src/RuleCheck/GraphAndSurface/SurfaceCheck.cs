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
    public class SurfaceCheck
    {
        /// <summary>
        /// The enclosing graph and surface checker
        /// </summary>
        private GraphAndSurfaceCheck Enclosing { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enclosing"></param>
        public SurfaceCheck(GraphAndSurfaceCheck enclosing)
        {
            Enclosing = enclosing;
        }

        /// <summary>
        /// Checks that the parameter provided to the call can be transformed into a Surface
        /// </summary>
        /// <param name="call"></param>
        /// <param name="parameter">The parameter creating the surface value</param>
        /// <param name="distance">The distance parameter</param>
        /// <param name="speed">The speed parameter</param>
        public void CheckCall(Call call, Parameter parameter, Parameter distance, Parameter speed)
        {
            Functions.Function surfaceFunction = null;
            Expression surfaceFunctionExpression;
            if (call.ParameterAssociation.TryGetValue(parameter, out surfaceFunctionExpression))
            {
                CheckExpression(surfaceFunctionExpression, distance, speed);
            }
        }

        /// <summary>
        /// Ensures that a function can be transformed into a surface
        /// </summary>
        /// <param name="function">The functions to verify that it is surface-able</param>
        private void CheckFunction(Functions.Function function)
        {
            if (function != null)
            {
                Parameter distance = Enclosing.GetDistanceParameter(function);
                Parameter speed = Enclosing.GetSpeedParameter(function);
                if (distance != null)
                {
                    if (speed != null)
                    {
                        CheckFunction(function, distance, speed);
                    }
                    else
                    {
                        function.AddRuleCheckMessage(
                            RuleChecksEnum.SemanticAnalysisError,
                            ElementLog.LevelEnum.Error,
                            "Surface cannot be computed on the function, no parameter can be interpreted as a speed");
                    }
                }
                else
                {
                    function.AddRuleCheckMessage(
                        RuleChecksEnum.SemanticAnalysisError,
                        ElementLog.LevelEnum.Error,
                        "Surface cannot be computed on the function, no parameter can be interpreted as a distance");
                }
            }
        }

        /// <summary>
        /// Ensures that a function can be transformed into a surface
        /// </summary>
        /// <param name="function">The functions to verify that it is surface-able</param>
        /// <param name="distance">The x parameter on which the surface is built</param>
        /// <param name="speed">The y parameter on which the surface is built</param>
        private void CheckFunction(Functions.Function function, Parameter distance, Parameter speed)
        {
            if (Enclosing.IsDouble(distance))
            {
                if (Enclosing.IsDouble(speed))
                {
                    foreach (Functions.Case cas in function.Cases)
                    {
                        CheckCase(cas, distance, speed);
                    }
                }
                else
                {
                    function.AddRuleCheckMessage(
                        RuleChecksEnum.SemanticAnalysisError,
                        ElementLog.LevelEnum.Error,
                        "Parameter " + speed.Name + " is not a valid range for a surface");
                }
            }
            else
            {
                function.AddRuleCheckMessage(
                    RuleChecksEnum.SemanticAnalysisError,
                    ElementLog.LevelEnum.Error,
                    "Parameter " + distance.Name + " is not a valid range for a surface");
            }
        }

        /// <summary>
        /// Ensures that the case is valid for a surface function, according to the parameter provided
        /// </summary>
        /// <param name="cas"></param>
        /// <param name="distance">The x parameter on which the surface is built</param>
        /// <param name="speed">The y parameter on which the surface is built</param>
        private void CheckCase(Functions.Case cas, Parameter distance, Parameter speed)
        {
            Parameter preConditionParameter = null;
            foreach (Rules.PreCondition preCondition in cas.PreConditions)
            {
                Parameter constrainedParameter = CheckPrecondition(preCondition.Expression, distance, speed);
                if (preConditionParameter == null)
                {
                    preConditionParameter = constrainedParameter;
                }
                else
                {
                    if (preConditionParameter != constrainedParameter && constrainedParameter != null )
                    {
                        cas.AddRuleCheckMessage(
                            RuleChecksEnum.SemanticAnalysisError,
                            ElementLog.LevelEnum.Error,
                            "Precondition in a single function should always constraint the same parameter to be able to create a surface");                        
                    }
                }
            }

            CheckExpression(cas.Expression, distance, speed);
        }

        /// <summary>
        /// Ensure that the precondition is valid to create a surface
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="distance">The x parameter on which the surface is built</param>
        /// <param name="speed">The y parameter on which the surface is built</param>
        /// <returns></returns>
        private Parameter CheckPrecondition(Expression expression, Parameter distance, Parameter speed)
        {
            Parameter retVal = null;

            if (Enclosing.IsConstant(expression, distance))
            {
                if (Enclosing.IsConstant(expression, speed))
                {
                    // OK
                }
                else
                {
                    retVal = speed;
                }
            }
            else
            {
                if (Enclosing.IsConstant(expression, speed))
                {
                    retVal = distance;
                }
                else
                {
                    expression.AddError(
                        "Precondition cannot reference both distance and speed to be able to create a surface",                        
                        RuleChecksEnum.SemanticAnalysisError);
                }                
            }

            if (!Enclosing.IsConstant(expression, distance, speed))
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
                            if (binaryExpression.Left.Ref != retVal)
                            {
                                expression.AddError(
                                    binaryExpression.Left + " : No operation can be performed on the distance/speed parameter to be able to compute a surface",
                                    RuleChecksEnum.SemanticAnalysisError);
                            }

                            if (!Enclosing.IsConstant(binaryExpression.Right))
                            {
                                expression.AddError(
                                    binaryExpression.Right + " should not rely on the distance/speed, since left expression already does to be able to compute a surface",
                                    RuleChecksEnum.SemanticAnalysisError);
                            }
                        }
                        else
                        {
                            // Left is constant => Right is not constant
                            if (binaryExpression.Right.Ref != retVal)
                            {
                                expression.AddError(
                                    binaryExpression.Right + " : No operation can be performed on the distance/speed parameter to be able to compute a graph",
                                    RuleChecksEnum.SemanticAnalysisError);
                            }
                        }
                    }
                    else
                    {
                        expression.AddError(
                            "Only comparison can be used to be able to compute a surface",
                            RuleChecksEnum.SemanticAnalysisError);                        
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Checks that the value provided is valid to compute a surface
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="distance">The parameter for the distance</param>
        /// <param name="speed">The parameter for the speed</param>
        /// <returns></returns>
        private void CheckExpression(Expression expression, Parameter distance, Parameter speed)
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
                    binaryExpression.Operation == BinaryExpression.Operator.Div)
                {
                    CheckExpression(binaryExpression.Left, distance, speed);
                    CheckExpression(binaryExpression.Right, distance, speed);

                    if (binaryExpression.Operation == BinaryExpression.Operator.Mult ||
                        binaryExpression.Operation == BinaryExpression.Operator.Div)
                    {
                        if (!Enclosing.IsConstant(binaryExpression.Left, distance, speed) &&
                            !Enclosing.IsConstant(binaryExpression.Right, distance, speed))
                        {
                            expression.AddError(
                                "* and / should be performed with a constant factor to be able to compute a surface",
                                RuleChecksEnum.SemanticAnalysisError);
                        }
                    }
                }
                else
                {
                    expression.AddError(
                        "Only +, -, *, / can be used to be able to compute a surface",
                        RuleChecksEnum.SemanticAnalysisError);
                }
            }
            else if (reduceExpression != null)
            {
                CheckExpression(reduceExpression.IteratorExpression, distance, speed);
                CheckExpression(reduceExpression.InitialValue, distance, speed);

                if (!Enclosing.IsConstant(reduceExpression.ListExpression, distance, speed))
                {
                    expression.AddError(
                        "List expression should not rely on the distance/speed to be able to compute a surface",
                        RuleChecksEnum.SemanticAnalysisError);
                }

                if (!Enclosing.IsConstant(reduceExpression.Condition, distance, speed))
                {
                    expression.AddError(
                        "Condition expression should not rely on the distance/speed to be able to compute a surface",
                        RuleChecksEnum.SemanticAnalysisError);                    
                }
            }
            else if (functionExpression != null)
            {
                Parameter newDistance = Enclosing.GetDistanceParameter(functionExpression);
                Parameter newSpeed = Enclosing.GetSpeedParameter(functionExpression);

                if (newDistance != null)
                {
                    if (newSpeed != null)
                    {
                        CheckExpression(functionExpression.Expression, newDistance, newSpeed);
                    }
                    else
                    {
                        expression.AddError(
                            "Function expression should declare a speed parameter to be able to compute a surface",
                            RuleChecksEnum.SemanticAnalysisError);
                    }
                }
                else
                {
                    expression.AddError(
                        "Function expression should declare a distance parameter to be able to compute a surface",
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
                        CheckExpression(casted, distance, speed);
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

                    CheckExpression(left, distance, speed);
                    CheckExpression(right, distance, speed);

                    bool noOperationOnDistanceOrSpeed = true;
                    foreach (Expression callParameter in call.AllParameters)
                    {
                        noOperationOnDistanceOrSpeed = noOperationOnDistanceOrSpeed && Enclosing.IsConstantOrParameter(callParameter, distance, speed);
                    }

                    if (!noOperationOnDistanceOrSpeed)
                    {
                        expression.AddError(
                            "No operation can be performed on the distance/speed parameter to be able to compute a graph",
                            RuleChecksEnum.SemanticAnalysisError);
                    }
                }
                else
                {
                    bool noOperationOnDistanceOrSpeed = true;

                    Parameter newDistance = null;
                    Parameter newSpeed = null;
                    if (call.ParameterAssociation != null)
                    {
                        foreach (KeyValuePair<Parameter, Expression> callParameter in call.ParameterAssociation)
                        {
                            if (callParameter.Value.Ref == distance)
                            {
                                newDistance = callParameter.Key;
                            }
                            else if (callParameter.Value.Ref == speed)
                            {
                                newSpeed = callParameter.Key;
                            }
                            else
                            {
                                noOperationOnDistanceOrSpeed = noOperationOnDistanceOrSpeed && Enclosing.IsConstant(callParameter.Value, distance, speed);
                            }
                        }
                    }
                    else
                    {
                        // This seems to be a HaCk
                        CheckExpression(call.Called, distance, speed);
                    }

                    if (!noOperationOnDistanceOrSpeed)
                    {
                        expression.AddError(
                            "No operation can be performed on the distance/speed parameter to be able to compute a graph",
                            RuleChecksEnum.SemanticAnalysisError);
                    }

                    Functions.Function function = call.Called.Ref as Functions.Function;
                    if (function != null)
                    {
                        if (newDistance != null)
                        {
                            if (newSpeed != null)
                            {
                                CheckFunction(function, newDistance, newSpeed);
                            }
                            else
                            {
                                Enclosing.GraphCheck.CheckFunction(function, newDistance);
                            }
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
                    if (!Enclosing.IsConstant(expression, distance, speed))
                    {
                        expression.AddError(
                            expression + " should not rely on distance/speed to be able to compute a surface",
                            RuleChecksEnum.SemanticAnalysisError);
                    }
                }
            }
            else
            {
                if (!Enclosing.IsConstant(expression, distance, speed))
                {
                    expression.AddError(
                        expression + " should not rely on distance/speed to be able to compute a surface",
                        RuleChecksEnum.SemanticAnalysisError);
                }
            }
        }
    }
}