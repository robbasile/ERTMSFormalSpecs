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

using DataDictionary.Functions.PredefinedFunctions;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using Utils;

namespace DataDictionary.RuleCheck.GraphAndSurface
{
    /// <summary>
    /// Statically checks that graph and surfaces can be computed using the provided model
    /// </summary>
    public class GraphAndSurfaceCheck
    {
        /// <summary>
        /// The class used to check graphs
        /// </summary>
        public GraphCheck GraphCheck { get; private set; }

        /// <summary>
        /// The class used to check surfaces
        /// </summary>
        public SurfaceCheck SurfaceCheck { get; private set; }

        /// <summary>
        /// The dictionary that should be checked
        /// </summary>
        private Dictionary Dictionary { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public GraphAndSurfaceCheck(Dictionary dictionary)
        {
            Dictionary = dictionary;
            GraphCheck = new GraphCheck(this);
            SurfaceCheck = new SurfaceCheck(this);
        }

        /// <summary>
        /// Ensures that Graphs and Surfaces will be correctly computed, when needed
        /// that is, when a graph or surface is provided to a function call : 
        ///  - DecelerationProfile
        /// </summary>
        public void CheckGraphAndSurfaceExpression()
        {
            CheckReferences(EfsSystem.Instance.DecelerationProfilePredefinedFunction);
            CheckReferences(EfsSystem.Instance.FullDecelerationForTargetPredefinedFunction);
        }

        /// <summary>
        /// Checks for references of a specific function in the given dictionary
        /// </summary>
        /// <param name="function"></param>
        private void CheckReferences(Function function)
        {
            foreach (Usage usage in EfsSystem.Instance.FindReferences(function))
            {
                ModelElement user = usage.User;
                if (Dictionary == EnclosingFinder<Dictionary>.find(user, true))
                {
                    // Only considers the users in the checked dictionary
                    IExpressionable expressionable = user as IExpressionable;
                    if (expressionable != null)
                    {
                        FindCall findCall = new FindCall(function);
                        findCall.GatherReferences(expressionable.Tree);
                        foreach (Call call in findCall.References)
                        {
                            CheckCall(call, null, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks that the call, if related to Graph or Surfaces, is valid
        /// </summary>
        /// <param name="call"></param>
        /// <param name="distance">The distance parameter</param>
        /// <param name="speed"></param>
        private void CheckCall(Call call, Parameter distance, Parameter speed)
        {
            DecelerationProfile decelerationFunction = EfsSystem.Instance.DecelerationProfilePredefinedFunction;
            FullDecelerationForTarget fullDecelerationForTargetFunction = EfsSystem.Instance.FullDecelerationForTargetPredefinedFunction;
            if (call.Called.Ref == decelerationFunction)
            {
                GraphCheck.CheckCall(call, decelerationFunction.SpeedRestrictions, distance);
                SurfaceCheck.CheckCall(call, decelerationFunction.DecelerationFactor, distance, speed);
            }
            else if (call.Called.Ref == fullDecelerationForTargetFunction)
            {
                SurfaceCheck.CheckCall(call, fullDecelerationForTargetFunction.DecelerationFactor, distance, speed);
            }
        }

        /// <summary>
        /// Indicates that the parameter references a double type
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool IsDouble(Parameter parameter)
        {
            bool retVal;

            Range range = parameter.Type as Range;
            if (range != null && range.getPrecision() == acceptor.PrecisionEnum.aDoublePrecision)
            {
                retVal = true;
            }
            else
            {
                retVal = parameter.Type == EfsSystem.Instance.DoubleType;
            }

            return retVal;
        }

        /// <summary>
        /// Provides the distance parameter of the function
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public Parameter GetDistanceParameter(Function function)
        {
            Parameter retVal = null;

            foreach (Parameter p in function.allParameters())
            {
                if (IsDouble(p))
                {
                    retVal = p;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the distance parameter of the function expression
        /// </summary>
        /// <param name="functionExpression"></param>
        /// <returns></returns>
        public Parameter GetDistanceParameter(FunctionExpression functionExpression)
        {
            Parameter retVal = null;

            foreach (Parameter p in functionExpression.Parameters)
            {
                if (IsDouble(p))
                {
                    retVal = p;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the speed parameter of the function
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public Parameter GetSpeedParameter(Function function)
        {
            Parameter retVal = null;

            Parameter distance = GetDistanceParameter(function);
            foreach (Parameter p in function.allParameters())
            {
                if (IsDouble(p) && p != distance)
                {
                    retVal = p;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provides the speed parameter of the function
        /// </summary>
        /// <param name="functionExpression"></param>
        /// <returns></returns>
        public Parameter GetSpeedParameter(FunctionExpression functionExpression)
        {
            Parameter retVal = null;

            Parameter distance = GetDistanceParameter(functionExpression);
            foreach (Parameter p in functionExpression.Parameters)
            {
                if (IsDouble(p) && p != distance)
                {
                    retVal = p;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates that the expression does not depend on a given (formal) parameter
        /// </summary>
        /// <param name="expression">The expression to check</param>
        /// <param name="parameters">The parameters in respect of which the function should be constant</param>
        /// <returns></returns>
        public bool IsConstant(Expression expression, params Parameter[] parameters)
        {
            bool retVal = true;

            foreach (Parameter parameter in parameters)
            {
                FindReference<InterpreterTreeNode> findReference = new FindReference<InterpreterTreeNode>(parameter);
                findReference.GatherReferences(expression);
                if (findReference.References.Count != 0)
                {
                    retVal = false;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Indicates that the expression does not depend on a given (formal) parameter
        /// </summary>
        /// <param name="expression">The expression to check</param>
        /// <param name="parameters">The parameters in respect of which the function should be constant</param>
        /// <returns></returns>
        public bool IsConstantOrParameter(Expression expression, params Parameter[] parameters)
        {
            bool retVal = IsConstant(expression, parameters);

            if (!retVal)
            {
                UnaryExpression unaryExpression = expression as UnaryExpression;
                if (unaryExpression != null && unaryExpression.Term != null )
                {
                    foreach (Parameter parameter in parameters)
                    {
                        if ( unaryExpression.Term.Ref == parameter)
                        {
                            retVal = true;
                            break;
                        }
                    }
                }
            }

            return retVal;
        }
    }
}