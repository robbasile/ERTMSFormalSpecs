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
using DataDictionary.Functions;
using DataDictionary.Generated;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Collection = DataDictionary.Types.Collection;
using Function = DataDictionary.Functions.Function;
using StateMachine = DataDictionary.Types.StateMachine;
using Structure = DataDictionary.Types.Structure;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    public class BinaryExpression : Expression
    {
        /// <summary>
        ///     The left expression of this expression
        /// </summary>
        public Expression Left { get; set; }

        /// <summary>
        ///     The available operators
        /// </summary>
        public enum OPERATOR
        {
            EXP,
            MULT,
            DIV,
            ADD,
            SUB,
            EQUAL,
            NOT_EQUAL,
            IN,
            NOT_IN,
            LESS,
            LESS_OR_EQUAL,
            GREATER,
            GREATER_OR_EQUAL,
            AND,
            OR,
            UNDEF,
            IS,
            AS
        };

        public static OPERATOR[] OperatorsLevel0 = {OPERATOR.OR};
        public static OPERATOR[] OperatorsLevel1 = {OPERATOR.AND};

        public static OPERATOR[] OperatorsLevel2 =
        {
            OPERATOR.EQUAL, OPERATOR.NOT_EQUAL, OPERATOR.IN, OPERATOR.NOT_IN,
            OPERATOR.LESS_OR_EQUAL, OPERATOR.GREATER_OR_EQUAL, OPERATOR.LESS, OPERATOR.GREATER, OPERATOR.IS, OPERATOR.AS
        };

        public static OPERATOR[] OperatorsLevel3 = {OPERATOR.ADD, OPERATOR.SUB};
        public static OPERATOR[] OperatorsLevel4 = {OPERATOR.MULT, OPERATOR.DIV};
        public static OPERATOR[] OperatorsLevel5 = {OPERATOR.EXP};

        public static OPERATOR[][] OperatorsByLevel =
        {
            OperatorsLevel0, OperatorsLevel1, OperatorsLevel2,
            OperatorsLevel3, OperatorsLevel4, OperatorsLevel5
        };

        /// <summary>
        ///     The available operators
        /// </summary>
        public static OPERATOR[] Operators =
        {
            OPERATOR.OR, OPERATOR.AND,
            OPERATOR.EQUAL, OPERATOR.NOT_EQUAL, OPERATOR.IN, OPERATOR.NOT_IN, OPERATOR.LESS_OR_EQUAL,
            OPERATOR.GREATER_OR_EQUAL, OPERATOR.LESS, OPERATOR.GREATER, OPERATOR.IS, OPERATOR.AS,
            OPERATOR.ADD, OPERATOR.SUB,
            OPERATOR.MULT, OPERATOR.DIV,
            OPERATOR.EXP
        };

        /// <summary>
        ///     The corresponding operator images
        /// </summary>
        public static string[] OperatorsImages =
        {
            "OR", "AND",
            "==", "!=", "in", "not in", "<=", ">=", "<", ">", "is", "as",
            "+", "-",
            "*", "/",
            "^",
            ".",
        };

        /// <summary>
        ///     The operation for this expression
        /// </summary>
        public OPERATOR Operation { get; private set; }

        /// <summary>
        ///     The right expression of this expression
        /// </summary>
        public Expression Right { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public BinaryExpression(ModelElement root, ModelElement log, Expression left, OPERATOR op, Expression right,
            int start, int end)
            : base(root, log, start, end)
        {
            Left = left;
            Left.Enclosing = this;

            Operation = op;
            Right = right;
            Right.Enclosing = this;
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                // Left
                Left.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                StaticUsage.AddUsages(Left.StaticUsage, Usage.ModeEnum.Read);

                // Right
                if (Operation == OPERATOR.IS || Operation == OPERATOR.AS)
                {
                    Right.SemanticAnalysis(instance, IsType.INSTANCE);
                    StaticUsage.AddUsages(Right.StaticUsage, Usage.ModeEnum.Type);
                }
                else
                {
                    Right.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                    StaticUsage.AddUsages(Right.StaticUsage, Usage.ModeEnum.Read);
                }
            }

            return retVal;
        }

        private ICallable __staticCallable = null;

        public override ICallable getStaticCallable()
        {
            if (__staticCallable == null)
            {
                ICallable left = Left.getStaticCallable();
                if (left != null)
                {
                    ICallable right = Right.getStaticCallable();
                    if (right != null)
                    {
                        if (left.FormalParameters.Count == right.FormalParameters.Count)
                        {
                            bool match = true;
                            for (int i = 0; i < left.FormalParameters.Count; i++)
                            {
                                Type leftType = ((Parameter) left.FormalParameters[i]).Type;
                                Type rightType = ((Parameter) right.FormalParameters[i]).Type;
                                if (!leftType.Equals(rightType))
                                {
                                    AddError("Non matching formal parameter type for parameter " + i + " " + leftType +
                                             " vs " + rightType);
                                    match = false;
                                }
                            }

                            if (left.ReturnType != right.ReturnType)
                            {
                                AddError("Non matching return types " + left.ReturnType + " vs " + right.ReturnType);
                                match = false;
                            }

                            if (match)
                            {
                                // Create a dummy funciton for type analysis
                                Function function = (Function) acceptor.getFactory().createFunction();
                                function.Name = ToString();
                                function.ReturnType = left.ReturnType;
                                foreach (Parameter param in left.FormalParameters)
                                {
                                    Parameter parameter = (Parameter) acceptor.getFactory().createParameter();
                                    parameter.Name = param.Name;
                                    parameter.Type = param.Type;
                                    parameter.Enclosing = function;
                                    function.appendParameters(parameter);
                                }
                                function.Enclosing = Root;
                                __staticCallable = function;
                            }
                        }
                        else
                        {
                            AddError("Invalid number of parameters, " + Left + " and " + Right +
                                     " should have the same number of parameters");
                        }
                    }
                    else
                    {
                        // Left is not null, but right is. 
                        // Ensure that right type corresponds to left return type 
                        // and return left
                        Type rightType = Right.GetExpressionType();
                        if (rightType.Match(left.ReturnType))
                        {
                            __staticCallable = left;
                        }
                        else
                        {
                            AddError(Left + "(" + left.ReturnType + " ) does not correspond to " + Right + "(" +
                                     rightType + ")");
                        }
                    }
                }
                else
                {
                    ICallable right = Right.getStaticCallable();
                    if (right != null)
                    {
                        // Right is not null, but left is. 
                        // Ensure that left type corresponds to right return type 
                        // and return right
                        Type leftType = Left.GetExpressionType();
                        if ((leftType.Match(right.ReturnType)))
                        {
                            __staticCallable = right;
                        }
                        else
                        {
                            AddError(Left + "(" + leftType + ") does not correspond to " + Right + "(" +
                                     right.ReturnType + ")");
                        }
                    }
                }
            }

            return __staticCallable;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <returns></returns>
        public override Type GetExpressionType()
        {
            Type retVal = null;

            Type leftType = Left.GetExpressionType();
            if (leftType == null)
            {
                AddError("Cannot determine expression type (1) for " + Left.ToString());
            }
            else
            {
                if (Operation == OPERATOR.IS)
                {
                    retVal = EFSSystem.INSTANCE.BoolType;
                }
                else if (Operation == OPERATOR.AS)
                {
                    retVal = Right.Ref as Structure;
                }
                else
                {
                    Type rightType = Right.GetExpressionType();
                    if (rightType == null)
                    {
                        AddError("Cannot determine expression type (2) for " + Right.ToString());
                    }
                    else
                    {
                        switch (Operation)
                        {
                            case OPERATOR.EXP:
                            case OPERATOR.MULT:
                            case OPERATOR.DIV:
                            case OPERATOR.ADD:
                            case OPERATOR.SUB:
                                if (leftType.Match(rightType))
                                {
                                    if (leftType is IntegerType || leftType is DoubleType)
                                    {
                                        retVal = rightType;
                                    }
                                    else
                                    {
                                        retVal = leftType;
                                    }
                                }
                                else
                                {
                                    retVal = leftType.CombineType(rightType, Operation);
                                }

                                break;

                            case OPERATOR.AND:
                            case OPERATOR.OR:
                                if (leftType == EFSSystem.BoolType && rightType == EFSSystem.BoolType)
                                {
                                    retVal = EFSSystem.BoolType;
                                }
                                break;

                            case OPERATOR.EQUAL:
                            case OPERATOR.NOT_EQUAL:
                            case OPERATOR.LESS:
                            case OPERATOR.LESS_OR_EQUAL:
                            case OPERATOR.GREATER:
                            case OPERATOR.GREATER_OR_EQUAL:
                            case OPERATOR.IS:
                            case OPERATOR.AS:
                                if (leftType.Match(rightType) || rightType.Match(leftType))
                                {
                                    retVal = EFSSystem.BoolType;
                                }
                                break;

                            case OPERATOR.IN:
                            case OPERATOR.NOT_IN:
                                Collection collection = rightType as Collection;
                                if (collection != null)
                                {
                                    if (collection.Type == null)
                                    {
                                        retVal = EFSSystem.BoolType;
                                    }
                                    else if (collection.Type.Match(leftType))
                                    {
                                        retVal = EFSSystem.BoolType;
                                    }
                                }
                                else
                                {
                                    StateMachine stateMachine = rightType as StateMachine;
                                    if (stateMachine != null && leftType.Match(stateMachine))
                                    {
                                        retVal = EFSSystem.BoolType;
                                    }
                                }
                                break;

                            case OPERATOR.UNDEF:
                                break;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain">The explanation to fill, if any</param>
        /// <returns></returns>
        public override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = null;

            ExplanationPart binaryExpressionExplanation = ExplanationPart.CreateSubExplanation(explain, this);

            IValue leftValue = null;
            IValue rightValue = null;
            try
            {
                leftValue = Left.GetValue(context, binaryExpressionExplanation);
            }
            catch (Exception e)
            {
                AddErrorAndExplain("Cannot evaluate " + Left + ". Reason is " + e.Message, binaryExpressionExplanation);
                throw new Exception("inner evaluation failed");
            }

            try
            {
                if (leftValue != null)
                {
                    switch (Operation)
                    {
                        case OPERATOR.EXP:
                        case OPERATOR.MULT:
                        case OPERATOR.ADD:
                        case OPERATOR.SUB:
                        case OPERATOR.DIV:
                        {
                            rightValue = Right.GetValue(context, binaryExpressionExplanation);
                            if (rightValue != null)
                            {
                                retVal = leftValue.Type.PerformArithmericOperation(context, leftValue, Operation,
                                    rightValue);
                            }
                            else
                            {
                                AddError("Error while computing value for " + Right.ToString());
                            }
                        }
                            break;

                        case OPERATOR.AND:
                        {
                            if (leftValue.Type == EFSSystem.BoolType)
                            {
                                BoolValue lb = leftValue as BoolValue;

                                if (lb.Val)
                                {
                                    rightValue = Right.GetValue(context, binaryExpressionExplanation);
                                    if (rightValue != null)
                                    {
                                        if (rightValue.Type == EFSSystem.BoolType)
                                        {
                                            retVal = rightValue as BoolValue;
                                        }
                                        else
                                        {
                                            AddError("Cannot apply an operator " + Operation.ToString() +
                                                     " on a variable of type " + rightValue.GetType());
                                        }
                                    }
                                    else
                                    {
                                        AddError("Error while computing value for " + Right.ToString());
                                    }
                                }
                                else
                                {
                                    ExplanationPart.CreateSubExplanation(binaryExpressionExplanation,
                                        "Right part not evaluated");
                                    retVal = lb;
                                }
                            }
                            else
                            {
                                AddError("Cannot apply an operator " + Operation.ToString() + " on a variable of type " +
                                         leftValue.GetType());
                            }
                        }
                            break;

                        case OPERATOR.OR:
                        {
                            if (leftValue.Type == EFSSystem.BoolType)
                            {
                                BoolValue lb = leftValue as BoolValue;

                                if (!lb.Val)
                                {
                                    rightValue = Right.GetValue(context, binaryExpressionExplanation);
                                    if (rightValue != null)
                                    {
                                        if (rightValue.Type == EFSSystem.BoolType)
                                        {
                                            retVal = rightValue as BoolValue;
                                        }
                                        else
                                        {
                                            AddError("Cannot apply an operator " + Operation.ToString() +
                                                     " on a variable of type " + rightValue.GetType());
                                        }
                                    }
                                    else
                                    {
                                        AddError("Error while computing value for " + Right.ToString());
                                    }
                                }
                                else
                                {
                                    ExplanationPart.CreateSubExplanation(binaryExpressionExplanation,
                                        "Right part not evaluated");
                                    retVal = lb;
                                }
                            }
                            else
                            {
                                AddError("Cannot apply an operator " + Operation.ToString() + " on a variable of type " +
                                         leftValue.GetType());
                            }
                        }
                            break;

                        case OPERATOR.LESS:
                        {
                            rightValue = Right.GetValue(context, binaryExpressionExplanation);
                            if (rightValue != null)
                            {
                                retVal = EFSSystem.GetBoolean(leftValue.Type.Less(leftValue, rightValue));
                            }
                            else
                            {
                                AddError("Error while computing value for " + Right.ToString());
                            }
                        }
                            break;

                        case OPERATOR.LESS_OR_EQUAL:
                        {
                            rightValue = Right.GetValue(context, binaryExpressionExplanation);
                            if (rightValue != null)
                            {
                                retVal =
                                    EFSSystem.GetBoolean(leftValue.Type.CompareForEquality(leftValue, rightValue) ||
                                                         leftValue.Type.Less(leftValue, rightValue));
                            }
                            else
                            {
                                AddError("Error while computing value for " + Right.ToString());
                            }
                        }
                            break;

                        case OPERATOR.GREATER:
                        {
                            rightValue = Right.GetValue(context, binaryExpressionExplanation);
                            if (rightValue != null)
                            {
                                retVal = EFSSystem.GetBoolean(leftValue.Type.Greater(leftValue, rightValue));
                            }
                            else
                            {
                                AddError("Error while computing value for " + Right.ToString());
                            }
                        }
                            break;

                        case OPERATOR.GREATER_OR_EQUAL:
                        {
                            rightValue = Right.GetValue(context, binaryExpressionExplanation);
                            if (rightValue != null)
                            {
                                retVal =
                                    EFSSystem.GetBoolean(leftValue.Type.CompareForEquality(leftValue, rightValue) ||
                                                         leftValue.Type.Greater(leftValue, rightValue));
                            }
                            else
                            {
                                AddError("Error while computing value for " + Right.ToString());
                            }
                        }
                            break;

                        case OPERATOR.EQUAL:
                        {
                            rightValue = Right.GetValue(context, binaryExpressionExplanation);
                            if (rightValue != null)
                            {
                                retVal = EFSSystem.GetBoolean(leftValue.Type.CompareForEquality(leftValue, rightValue));
                            }
                            else
                            {
                                AddError("Error while computing value for " + Right.ToString());
                            }
                        }
                            break;

                        case OPERATOR.NOT_EQUAL:
                        {
                            rightValue = Right.GetValue(context, binaryExpressionExplanation);
                            if (rightValue != null)
                            {
                                retVal = EFSSystem.GetBoolean(!leftValue.Type.CompareForEquality(leftValue, rightValue));
                            }
                            else
                            {
                                AddError("Error while computing value for " + Right.ToString());
                            }
                        }
                            break;

                        case OPERATOR.IN:
                        {
                            rightValue = Right.GetValue(context, binaryExpressionExplanation);
                            if (rightValue != null)
                            {
                                retVal = EFSSystem.GetBoolean(rightValue.Type.Contains(rightValue, leftValue));
                            }
                            else
                            {
                                AddError("Error while computing value for " + Right.ToString());
                            }
                        }
                            break;

                        case OPERATOR.NOT_IN:
                        {
                            rightValue = Right.GetValue(context, binaryExpressionExplanation);
                            if (rightValue != null)
                            {
                                retVal = EFSSystem.GetBoolean(!rightValue.Type.Contains(rightValue, leftValue));
                            }
                            else
                            {
                                AddError("Error while computing value for " + Right.ToString());
                            }
                        }
                            break;

                        case OPERATOR.IS:
                        {
                            leftValue = Left.GetValue(context, binaryExpressionExplanation);
                            retVal = EFSSystem.GetBoolean(false);
                            if (leftValue != null)
                            {
                                Structure rightStructure = Right.Ref as Structure;
                                if (rightStructure != null)
                                {
                                    if (leftValue.Type is Structure)
                                    {
                                        Structure leftStructure = leftValue.Type as Structure;
                                        if (rightStructure.ImplementedStructures.Contains(leftStructure))
                                        {
                                            retVal = EFSSystem.GetBoolean(true);
                                        }
                                        else
                                        {
                                            AddError("Incompatible types for operator is");
                                        }
                                    }
                                    else
                                    {
                                        AddError("The operator is can only be applied on structures");
                                    }
                                }
                                else
                                {
                                    AddError("The right part of is operator should be a structure");
                                }
                            }
                            else
                            {
                                AddError("Error while computing value for " + Left.ToString());
                            }
                        }
                            break;

                        case OPERATOR.AS:
                        {
                            leftValue = Left.GetValue(context, binaryExpressionExplanation);
                            if (leftValue != null)
                            {
                                if (leftValue.Type == Right.GetExpressionType())
                                {
                                    retVal = leftValue;
                                }
                                else
                                {
                                    AddError("Incompatible types for operator as");
                                }
                            }
                            else
                            {
                                AddError("Error while computing value for " + Left.ToString());
                            }
                        }
                            break;
                    }
                }
                else
                {
                    AddError("Error while computing value for " + Left.ToString());
                }
            }
            catch (Exception e)
            {
                AddErrorAndExplain("Cannot evaluate " + Right + ". Reason is " + e.Message, binaryExpressionExplanation);
                throw new Exception("inner evaluation failed");
            }
            finally
            {
                ExplanationPart.SetNamable(binaryExpressionExplanation, retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Gets the unbound parameters from the function definition and place holders
        /// </summary>
        /// <param name="context"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        private List<Parameter> getUnboundParameter(InterpretationContext context, Function function)
        {
            List<Parameter> retVal = new List<Parameter>();

            if (function != null)
            {
                foreach (Parameter formal in function.FormalParameters)
                {
                    IVariable actual = context.findOnStack(formal);
                    if (actual != null)
                    {
                        PlaceHolder placeHolder = actual.Value as PlaceHolder;
                        if (placeHolder != null)
                        {
                            retVal.Add(formal);
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Gets the unbound parameters from either the surface or the graph of the function
        /// </summary>
        /// <param name="leftFunction"></param>
        /// <returns></returns>
        private List<Parameter> getUnboundParametersFromValue(Function leftFunction)
        {
            List<Parameter> retVal = new List<Parameter>();

            if (leftFunction != null)
            {
                if (leftFunction.Surface != null)
                {
                    retVal.Add(leftFunction.Surface.XParameter);
                    retVal.Add(leftFunction.Surface.YParameter);
                }
                else if (leftFunction.Graph != null)
                {
                    // TODO : Use the parameters from the graph when available
                    retVal.Add((Parameter) leftFunction.FormalParameters[0]);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the callable that is called by this expression
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override ICallable getCalled(InterpretationContext context, ExplanationPart explain)
        {
            ICallable retVal = null;

            Function leftFunction = Left.getCalled(context, explain) as Function;
            List<Parameter> unboundLeft = getUnboundParameter(context, leftFunction);
            if (leftFunction == null || unboundLeft.Count == 0)
            {
                leftFunction = Left.GetValue(context, explain) as Function;
                unboundLeft = getUnboundParametersFromValue(leftFunction);
            }

            Function rightFunction = Right.getCalled(context, explain) as Function;
            List<Parameter> unboundRight = getUnboundParameter(context, rightFunction);
            if (rightFunction == null || unboundRight.Count == 0)
            {
                rightFunction = Right.GetValue(context, explain) as Function;
                unboundRight = getUnboundParametersFromValue(rightFunction);
            }

            int max = Math.Max(unboundLeft.Count, unboundRight.Count);
            if (max == 0)
            {
                if (leftFunction == null)
                {
                    if (rightFunction == null)
                    {
                        retVal = GetValue(context, explain) as ICallable;
                    }
                    else
                    {
                        if (rightFunction.FormalParameters.Count == 1)
                        {
                            retVal = createGraphResult(context, leftFunction, unboundLeft, rightFunction, unboundRight,
                                explain);
                        }
                        else if (rightFunction.FormalParameters.Count == 2)
                        {
                            retVal = createSurfaceResult(context, leftFunction, unboundLeft, rightFunction, unboundRight,
                                explain);
                        }
                        else
                        {
                            retVal = GetValue(context, explain) as ICallable;
                        }
                    }
                }
                else if (rightFunction == null)
                {
                    if (leftFunction.FormalParameters.Count == 1)
                    {
                        retVal = createGraphResult(context, leftFunction, unboundLeft, rightFunction, unboundRight,
                            explain);
                    }
                    else if (leftFunction.FormalParameters.Count == 2)
                    {
                        retVal = createSurfaceResult(context, leftFunction, unboundLeft, rightFunction, unboundRight,
                            explain);
                    }
                    else
                    {
                        retVal = GetValue(context, explain) as ICallable;
                    }
                }
                else
                {
                    retVal = GetValue(context, explain) as ICallable;
                }

                if (retVal == null)
                {
                    AddError("Cannot create ICallable when there are no unbound parameters");
                }
            }
            else if (max == 1)
            {
                retVal = createGraphResult(context, leftFunction, unboundLeft, rightFunction, unboundRight, explain);
            }
            else if (max == 2)
            {
                retVal = createSurfaceResult(context, leftFunction, unboundLeft, rightFunction, unboundRight, explain);
            }
            else
            {
                AddError("Cannot create graph or structure when more that 2 parameters are unbound");
            }

            return retVal;
        }

        /// <summary>
        ///     Creates the result as a surface
        /// </summary>
        /// <param name="context"></param>
        /// <param name="leftFunction"></param>
        /// <param name="unboundLeft"></param>
        /// <param name="rightFunction"></param>
        /// <param name="unboundRight"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        private ICallable createGraphResult(InterpretationContext context, Function leftFunction,
            List<Parameter> unboundLeft, Function rightFunction, List<Parameter> unboundRight, ExplanationPart explain)
        {
            ICallable retVal = null;

            Graph leftGraph = createGraphForUnbound(context, Left, leftFunction, unboundLeft, explain);
            if (leftGraph != null)
            {
                Graph rightGraph = createGraphForUnbound(context, Right, rightFunction, unboundRight, explain);

                if (rightGraph != null)
                {
                    retVal = combineGraph(leftGraph, rightGraph).Function;
                }
                else
                {
                    AddError("Cannot create graph for " + Right);
                }
            }
            else
            {
                AddError("Cannot create graph for " + Left);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates the result as a surface
        /// </summary>
        /// <param name="context"></param>
        /// <param name="leftFunction"></param>
        /// <param name="unboundLeft"></param>
        /// <param name="rightFunction"></param>
        /// <param name="unboundRight"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        private ICallable createSurfaceResult(InterpretationContext context, Function leftFunction,
            List<Parameter> unboundLeft, Function rightFunction, List<Parameter> unboundRight, ExplanationPart explain)
        {
            ICallable retVal = null;

            Surface leftSurface = createSurfaceForUnbound(context, Left, leftFunction, unboundLeft, explain);
            if (leftSurface != null)
            {
                Surface rightSurface = createSurfaceForUnbound(context, Right, rightFunction, unboundRight, explain);
                if (rightSurface != null)
                {
                    retVal = combineSurface(leftSurface, rightSurface).Function;
                }
                else
                {
                    AddError("Cannot create surface for " + Right);
                }
            }
            else
            {
                AddError("Cannot create surface for " + Left);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates the graph for the unbound parameters provided
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expression"></param>
        /// <param name="function"></param>
        /// <param name="unbound"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        private Graph createGraphForUnbound(InterpretationContext context, Expression expression, Function function,
            List<Parameter> unbound, ExplanationPart explain)
        {
            Graph retVal = null;

            if (unbound.Count == 0)
            {
                if (function != null && function.FormalParameters.Count > 0)
                {
                    retVal = function.createGraph(context, (Parameter) function.FormalParameters[0], explain);
                }
                else
                {
                    retVal = Graph.createGraph(expression.GetValue(context, explain), null, explain);
                }
            }
            else
            {
                if (function == null)
                {
                    retVal = expression.createGraph(context, unbound[0], explain);
                }
                else
                {
                    retVal = function.createGraph(context, unbound[0], explain);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates the graph for the unbount parameters provided
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expression"></param>
        /// <param name="function"></param>
        /// <param name="unbound"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        private Surface createSurfaceForUnbound(InterpretationContext context, Expression expression, Function function,
            List<Parameter> unbound, ExplanationPart explain)
        {
            Surface retVal = null;

            if (unbound.Count == 0)
            {
                if (function != null)
                {
                    Parameter xAxis = null;

                    if (function.FormalParameters.Count > 0)
                    {
                        xAxis = (Parameter) function.FormalParameters[0];
                    }
                    Parameter yAxis = null;
                    if (function.FormalParameters.Count > 1)
                    {
                        yAxis = (Parameter) function.FormalParameters[1];
                    }
                    retVal = function.createSurfaceForParameters(context, xAxis, yAxis, explain);
                }
                else
                {
                    retVal = Surface.createSurface(expression.GetValue(context, explain), null, null);
                }
            }
            else if (unbound.Count == 1)
            {
                Graph graph = createGraphForUnbound(context, expression, function, unbound, explain);
                retVal = Surface.createSurface(graph.Function, unbound[0], null);
            }
            else
            {
                if (function == null)
                {
                    retVal = expression.createSurface(context, unbound[0], unbound[1], explain);
                }
                else
                {
                    retVal = function.createSurfaceForParameters(context, unbound[0], unbound[1], explain);
                }
            }
            return retVal;
        }


        /// <summary>
        ///     Combines two graphs using the operator of this binary expression
        /// </summary>
        /// <param name="leftGraph"></param>
        /// <param name="rightGraph"></param>
        /// <returns></returns>
        private Graph combineGraph(Graph leftGraph, Graph rightGraph)
        {
            Graph retVal = null;

            switch (Operation)
            {
                case OPERATOR.ADD:
                    retVal = leftGraph.AddGraph(rightGraph);
                    break;

                case OPERATOR.SUB:
                    retVal = leftGraph.SubstractGraph(rightGraph);
                    break;

                case OPERATOR.MULT:
                    retVal = leftGraph.MultGraph(rightGraph);
                    break;

                case OPERATOR.DIV:
                    retVal = leftGraph.DivGraph(rightGraph);
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Combines two surfaces using the operator of this binary expression
        /// </summary>
        /// <param name="leftSurface"></param>
        /// <param name="rightSurface"></param>
        /// <returns></returns>
        private Surface combineSurface(Surface leftSurface, Surface rightSurface)
        {
            Surface retVal = null;

            switch (Operation)
            {
                case OPERATOR.ADD:
                    retVal = leftSurface.AddSurface(rightSurface);
                    break;

                case OPERATOR.SUB:
                    retVal = leftSurface.SubstractSurface(rightSurface);
                    break;

                case OPERATOR.MULT:
                    retVal = leftSurface.MultiplySurface(rightSurface);
                    break;

                case OPERATOR.DIV:
                    retVal = leftSurface.DivideSurface(rightSurface);
                    break;
            }

            return retVal;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public override void fill(List<INamable> retVal, BaseFilter filter)
        {
            Left.fill(retVal, filter);
            Right.fill(retVal, filter);
        }

        /// <summary>
        ///     Indicates that the expression is an equality of the form variable == literal
        /// </summary>
        /// <returns></returns>
        public bool IsSimpleEquality()
        {
            bool retVal = false;

            if (Operation == OPERATOR.EQUAL)
            {
                retVal = IsLeftSide.INSTANCE.AcceptableChoice(Left.Ref) && IsLiteral.Predicate(Right.Ref);
            }

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements = true)
        {
            Left.GetExplain(explanation);
            explanation.Write(" " + Image(Operation) + " ");
            Right.GetExplain(explanation);
        }

        /// <summary>
        ///     Provides the image of a given operator
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static string Image(OPERATOR op)
        {
            string retVal = null;

            for (int i = 0; i < Operators.Length; i++)
            {
                if (op == Operators[i])
                {
                    retVal = OperatorsImages[i];
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the image of a given operator
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static string[] Images(OPERATOR[] ops)
        {
            string[] retVal = new string[ops.Length];

            for (int i = 0; i < ops.Length; i++)
            {
                retVal[i] = Image(ops[i]);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the operator, based on its image
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static OPERATOR FindOperatorByName(string op)
        {
            OPERATOR retVal = OPERATOR.UNDEF;

            for (int i = 0; i < Operators.Length; i++)
            {
                if (OperatorsImages[i].CompareTo(op) == 0)
                {
                    retVal = Operators[i];
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        /// <param name="context">The interpretation context</param>
        public override void checkExpression()
        {
            base.checkExpression();

            Left.checkExpression();
            Right.checkExpression();

            Type leftType = Left.GetExpressionType();
            if (leftType != null)
            {
                if (Operation == OPERATOR.IS || (Operation == OPERATOR.AS))
                {
                    Structure leftStructure = leftType as Structure;
                    if (leftStructure != null)
                    {
                        Structure rightStructure = Right.Ref as Structure;
                        if (rightStructure != null)
                        {
                            if (!rightStructure.ImplementedStructures.Contains(leftStructure))
                            {
                                AddError("No inheritance from " + Right + " to " + Left);
                            }
                        }
                        else
                        {
                            AddError("Right part of " + Operation + " operation should be a structure, found " +
                                     Right.Ref);
                        }
                    }
                    else
                    {
                        AddError("Left expression type of " + Operation + " operation should be a structure, found " +
                                 leftType);
                    }
                }
                else
                {
                    Type rightType = Right.GetExpressionType();
                    if (rightType != null)
                    {
                        if (!leftType.ValidBinaryOperation(Operation, rightType)
                            && !rightType.ValidBinaryOperation(Operation, leftType))
                        {
                            AddError("Cannot perform " + Operation + " operation between " + Left + "(" + leftType.Name +
                                     ") and " + Right + "(" + rightType.Name + ")");
                        }

                        if (Operation == OPERATOR.EQUAL)
                        {
                            if (leftType is StateMachine && rightType is StateMachine)
                            {
                                AddWarning("IN operator should be used instead of == between " + Left.ToString() +
                                           " and " + Right.ToString());
                            }

                            if (Right.Ref == EFSSystem.EmptyValue)
                            {
                                if (leftType is Collection)
                                {
                                    AddError("Cannot compare collections with " + Right.Ref.Name + ". Use [] instead");
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Creates the graph associated to this expression, when the given parameter ranges over the X axis
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameter">The parameters of *the enclosing function* for which the graph should be created</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph createGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = base.createGraph(context, parameter, explain);

            Graph leftGraph = Left.createGraph(context, parameter, explain);
            if (leftGraph != null)
            {
                Graph rightGraph = Right.createGraph(context, parameter, explain);

                if (rightGraph != null)
                {
                    retVal = combineGraph(leftGraph, rightGraph);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the surface of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the surface</param>
        /// <param name="xParam">The X axis of this surface</param>
        /// <param name="yParam">The Y axis of this surface</param>
        /// <param name="explain"></param>
        /// <returns>The surface which corresponds to this expression</returns>
        public override Surface createSurface(InterpretationContext context, Parameter xParam, Parameter yParam,
            ExplanationPart explain)
        {
            Surface retVal = base.createSurface(context, xParam, yParam, explain);

            Surface leftSurface = Left.createSurface(context, xParam, yParam, explain);
            if (leftSurface != null)
            {
                Surface rightSurface = Right.createSurface(context, xParam, yParam, explain);

                if (rightSurface != null)
                {
                    retVal = combineSurface(leftSurface, rightSurface);
                }
            }
            retVal.XParameter = xParam;
            retVal.YParameter = yParam;

            return retVal;
        }

        /// <summary>
        ///     Inverses the operator provided
        /// </summary>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static OPERATOR Inverse(OPERATOR Operator)
        {
            OPERATOR retVal = Operator;

            switch (Operator)
            {
                case OPERATOR.GREATER:
                    retVal = OPERATOR.LESS_OR_EQUAL;
                    break;

                case OPERATOR.GREATER_OR_EQUAL:
                    retVal = OPERATOR.LESS;
                    break;

                case OPERATOR.LESS:
                    retVal = OPERATOR.GREATER_OR_EQUAL;
                    break;

                case OPERATOR.LESS_OR_EQUAL:
                    retVal = OPERATOR.GREATER;
                    break;

                default:
                    throw new Exception("Cannot inverse operator " + Operator);
            }

            return retVal;
        }
    }
}