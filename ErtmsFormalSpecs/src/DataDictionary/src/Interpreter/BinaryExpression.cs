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
        public enum Operator
        {
            Exp,
            Mult,
            Div,
            Add,
            Sub,
            Equal,
            NotEqual,
            In,
            NotIn,
            Less,
            LessOrEqual,
            Greater,
            GreaterOrEqual,
            And,
            Or,
            Undef,
            Is,
            As
        };

        public static Operator[] OperatorsLevel0 = {Operator.Or};
        public static Operator[] OperatorsLevel1 = {Operator.And};

        public static Operator[] OperatorsLevel2 =
        {
            Operator.Equal, Operator.NotEqual, Operator.In, Operator.NotIn,
            Operator.LessOrEqual, Operator.GreaterOrEqual, Operator.Less, Operator.Greater, Operator.Is, Operator.As
        };

        public static Operator[] OperatorsLevel3 = {Operator.Add, Operator.Sub};
        public static Operator[] OperatorsLevel4 = {Operator.Mult, Operator.Div};
        public static Operator[] OperatorsLevel5 = {Operator.Exp};

        public static Operator[][] OperatorsByLevel =
        {
            OperatorsLevel0, OperatorsLevel1, OperatorsLevel2,
            OperatorsLevel3, OperatorsLevel4, OperatorsLevel5
        };

        /// <summary>
        ///     The available operators
        /// </summary>
        public static Operator[] Operators =
        {
            Operator.Or, Operator.And,
            Operator.Equal, Operator.NotEqual, Operator.In, Operator.NotIn, Operator.LessOrEqual,
            Operator.GreaterOrEqual, Operator.Less, Operator.Greater, Operator.Is, Operator.As,
            Operator.Add, Operator.Sub,
            Operator.Mult, Operator.Div,
            Operator.Exp
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
            "."
        };

        /// <summary>
        ///     The operation for this expression
        /// </summary>
        public Operator Operation { get; private set; }

        /// <summary>
        ///     The right expression of this expression
        /// </summary>
        public Expression Right { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="log"></param>
        /// <param name="left"></param>
        /// <param name="op"></param>
        /// <param name="right"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public BinaryExpression(ModelElement root, ModelElement log, Expression left, Operator op, Expression right,
            int start, int end)
            : base(root, log, start, end)
        {
            Left = SetEnclosed(left);
            Operation = op;
            Right = SetEnclosed(right);
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <param name="expectation">Indicates the kind of element we are looking for</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public override bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = base.SemanticAnalysis(instance, expectation);

            if (retVal)
            {
                // Left
                if (Left != null)
                {
                    Left.SemanticAnalysis(instance, IsRightSide.INSTANCE);
                    StaticUsage.AddUsages(Left.StaticUsage, Usage.ModeEnum.Read);
                }

                // Right
                if (Right != null)
                {
                    if (Operation == Operator.Is || Operation == Operator.As)
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
            }

            return retVal;
        }

        /// <summary>
        /// Caches the ICallable which corresponds to this expression
        /// </summary>
        private ICallable _staticCallable;

        /// <summary>
        /// Provides the ICallable which corresponds to this expression
        /// </summary>
        /// <returns></returns>
        public override ICallable GetStaticCallable()
        {
            if (_staticCallable == null)
            {
                ICallable left = Left.GetStaticCallable();
                if (left != null)
                {
                    ICallable right = Right.GetStaticCallable();
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
                                _staticCallable = function;
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
                            _staticCallable = left;
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
                    ICallable right = Right.GetStaticCallable();
                    if (right != null)
                    {
                        // Right is not null, but left is. 
                        // Ensure that left type corresponds to right return type 
                        // and return right
                        Type leftType = Left.GetExpressionType();
                        if ((leftType.Match(right.ReturnType)))
                        {
                            _staticCallable = right;
                        }
                        else
                        {
                            AddError(Left + "(" + leftType + ") does not correspond to " + Right + "(" +
                                     right.ReturnType + ")");
                        }
                    }
                }
            }

            return _staticCallable;
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
                AddError("Cannot determine expression type (1) for " + Left);
            }
            else
            {
                if (Operation == Operator.Is)
                {
                    retVal = EfsSystem.Instance.BoolType;
                }
                else if (Operation == Operator.As)
                {
                    retVal = Right.Ref as Structure;
                }
                else
                {
                    Type rightType = Right.GetExpressionType();
                    if (rightType == null)
                    {
                        AddError("Cannot determine expression type (2) for " + Right);
                    }
                    else
                    {
                        switch (Operation)
                        {
                            case Operator.Exp:
                            case Operator.Mult:
                            case Operator.Div:
                            case Operator.Add:
                            case Operator.Sub:
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

                            case Operator.And:
                            case Operator.Or:
                                if (leftType == EfsSystem.Instance.BoolType && rightType == EfsSystem.Instance.BoolType)
                                {
                                    retVal = EfsSystem.Instance.BoolType;
                                }
                                break;

                            case Operator.Equal:
                            case Operator.NotEqual:
                            case Operator.Less:
                            case Operator.LessOrEqual:
                            case Operator.Greater:
                            case Operator.GreaterOrEqual:
                            case Operator.Is:
                            case Operator.As:
                                if (leftType.Match(rightType) || rightType.Match(leftType))
                                {
                                    retVal = EfsSystem.Instance.BoolType;
                                }
                                break;

                            case Operator.In:
                            case Operator.NotIn:
                                Collection collection = rightType as Collection;
                                if (collection != null)
                                {
                                    if (collection.Type == null)
                                    {
                                        retVal = EfsSystem.Instance.BoolType;
                                    }
                                    else if (collection.Type.Match(leftType))
                                    {
                                        retVal = EfsSystem.Instance.BoolType;
                                    }
                                }
                                else
                                {
                                    StateMachine stateMachine = rightType as StateMachine;
                                    if (stateMachine != null && leftType.Match(stateMachine))
                                    {
                                        retVal = EfsSystem.Instance.BoolType;
                                    }
                                }
                                break;

                            case Operator.Undef:
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
        protected internal override IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            IValue retVal = null;

            ExplanationPart binaryExpressionExplanation = ExplanationPart.CreateSubExplanation(explain, this);

            IValue leftValue = Left.GetValue(context, binaryExpressionExplanation);
            if (leftValue != null)
            {
                IValue rightValue;
                switch (Operation)
                {
                    case Operator.Exp:
                    case Operator.Mult:
                    case Operator.Add:
                    case Operator.Sub:
                    case Operator.Div:
                    {
                        rightValue = Right.GetValue(context, binaryExpressionExplanation);
                        if (rightValue != null)
                        {
                            retVal = leftValue.Type.PerformArithmericOperation(context, leftValue, Operation,
                                rightValue);
                        }
                        else
                        {
                            AddError("Error while computing value for " + Right);
                        }
                    }
                        break;

                    case Operator.And:
                    {
                        if (leftValue.Type == EfsSystem.Instance.BoolType)
                        {
                            BoolValue lb = leftValue as BoolValue;

                            if (lb != null)
                            {
                                if (lb.Val)
                                {
                                    rightValue = Right.GetValue(context, binaryExpressionExplanation);
                                    if (rightValue != null)
                                    {
                                        if (rightValue.Type == EfsSystem.Instance.BoolType)
                                        {
                                            retVal = rightValue as BoolValue;
                                        }
                                        else
                                        {
                                            AddError("Cannot apply an operator " + Operation +
                                                     " on a variable of type " + rightValue.GetType());
                                        }
                                    }
                                    else
                                    {
                                        AddError("Error while computing value for " + Right);
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
                                AddError("Cannot evaluate " + Left + " as a boolean");
                            }
                        }
                        else
                        {
                            AddError("Cannot apply an operator " + Operation + " on a variable of type " +
                                     leftValue.GetType());
                        }
                    }
                        break;

                    case Operator.Or:
                    {
                        if (leftValue.Type == EfsSystem.Instance.BoolType)
                        {
                            BoolValue lb = leftValue as BoolValue;

                            if (lb != null)
                            {
                                if (!lb.Val)
                                {
                                    rightValue = Right.GetValue(context, binaryExpressionExplanation);
                                    if (rightValue != null)
                                    {
                                        if (rightValue.Type == EfsSystem.Instance.BoolType)
                                        {
                                            retVal = rightValue as BoolValue;
                                        }
                                        else
                                        {
                                            AddError("Cannot apply an operator " + Operation +
                                                     " on a variable of type " + rightValue.GetType());
                                        }
                                    }
                                    else
                                    {
                                        AddError("Error while computing value for " + Right);
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
                                AddError("Cannot evaluate " + Left + " as a boolean");
                            }
                        }
                        else
                        {
                            AddError("Cannot apply an operator " + Operation + " on a variable of type " +
                                     leftValue.GetType());
                        }
                    }
                        break;

                    case Operator.Less:
                    {
                        rightValue = Right.GetValue(context, binaryExpressionExplanation);
                        if (rightValue != null)
                        {
                            retVal = EfsSystem.Instance.GetBoolean(leftValue.Type.Less(leftValue, rightValue));
                        }
                        else
                        {
                            AddError("Error while computing value for " + Right);
                        }
                    }
                        break;

                    case Operator.LessOrEqual:
                    {
                        rightValue = Right.GetValue(context, binaryExpressionExplanation);
                        if (rightValue != null)
                        {
                            retVal =
                                EfsSystem.Instance.GetBoolean(leftValue.Type.CompareForEquality(leftValue, rightValue) ||
                                                     leftValue.Type.Less(leftValue, rightValue));
                        }
                        else
                        {
                            AddError("Error while computing value for " + Right);
                        }
                    }
                        break;

                    case Operator.Greater:
                    {
                        rightValue = Right.GetValue(context, binaryExpressionExplanation);
                        if (rightValue != null)
                        {
                            retVal = EfsSystem.Instance.GetBoolean(leftValue.Type.Greater(leftValue, rightValue));
                        }
                        else
                        {
                            AddError("Error while computing value for " + Right);
                        }
                    }
                        break;

                    case Operator.GreaterOrEqual:
                    {
                        rightValue = Right.GetValue(context, binaryExpressionExplanation);
                        if (rightValue != null)
                        {
                            retVal =
                                EfsSystem.Instance.GetBoolean(leftValue.Type.CompareForEquality(leftValue, rightValue) ||
                                                     leftValue.Type.Greater(leftValue, rightValue));
                        }
                        else
                        {
                            AddError("Error while computing value for " + Right);
                        }
                    }
                        break;

                    case Operator.Equal:
                    {
                        rightValue = Right.GetValue(context, binaryExpressionExplanation);
                        if (rightValue != null)
                        {
                            retVal = EfsSystem.Instance.GetBoolean(leftValue.Type.CompareForEquality(leftValue, rightValue));
                        }
                        else
                        {
                            AddError("Error while computing value for " + Right);
                        }
                    }
                        break;

                    case Operator.NotEqual:
                    {
                        rightValue = Right.GetValue(context, binaryExpressionExplanation);
                        if (rightValue != null)
                        {
                            retVal = EfsSystem.Instance.GetBoolean(!leftValue.Type.CompareForEquality(leftValue, rightValue));
                        }
                        else
                        {
                            AddError("Error while computing value for " + Right);
                        }
                    }
                        break;

                    case Operator.In:
                    {
                        rightValue = Right.GetValue(context, binaryExpressionExplanation);
                        if (rightValue != null)
                        {
                            retVal = EfsSystem.Instance.GetBoolean(rightValue.Type.Contains(rightValue, leftValue));
                        }
                        else
                        {
                            AddError("Error while computing value for " + Right);
                        }
                    }
                        break;

                    case Operator.NotIn:
                    {
                        rightValue = Right.GetValue(context, binaryExpressionExplanation);
                        if (rightValue != null)
                        {
                            retVal = EfsSystem.Instance.GetBoolean(!rightValue.Type.Contains(rightValue, leftValue));
                        }
                        else
                        {
                            AddError("Error while computing value for " + Right);
                        }
                    }
                        break;

                    case Operator.Is:
                    {
                        leftValue = Left.GetValue(context, binaryExpressionExplanation);
                        retVal = EfsSystem.Instance.GetBoolean(false);
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
                                        retVal = EfsSystem.Instance.GetBoolean(true);
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
                            AddError("Error while computing value for " + Left);
                        }
                    }
                        break;

                    case Operator.As:
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
                            AddError("Error while computing value for " + Left);
                        }
                    }
                        break;
                }
            }
            else
            {
                AddError("Error while computing value for " + Left);
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
                    IVariable actual = context.FindOnStack(formal);
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
        public override ICallable GetCalled(InterpretationContext context, ExplanationPart explain)
        {
            ICallable retVal = null;

            Function leftFunction = Left.GetCalled(context, explain) as Function;
            List<Parameter> unboundLeft = getUnboundParameter(context, leftFunction);
            if (leftFunction == null || unboundLeft.Count == 0)
            {
                leftFunction = Left.GetValue(context, explain) as Function;
                unboundLeft = getUnboundParametersFromValue(leftFunction);
            }

            Function rightFunction = Right.GetCalled(context, explain) as Function;
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
                            // ReSharper disable once ExpressionIsAlwaysNull
                            retVal = CreateGraphResult(context, leftFunction, unboundLeft, rightFunction, unboundRight,
                                explain);
                        }
                        else if (rightFunction.FormalParameters.Count == 2)
                        {
                            // ReSharper disable once ExpressionIsAlwaysNull
                            retVal = CreateSurfaceResult(context, leftFunction, unboundLeft, rightFunction, unboundRight,
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
                        // ReSharper disable once ExpressionIsAlwaysNull
                        retVal = CreateGraphResult(context, leftFunction, unboundLeft, rightFunction, unboundRight,
                            explain);
                    }
                    else if (leftFunction.FormalParameters.Count == 2)
                    {
                        // ReSharper disable once ExpressionIsAlwaysNull
                        retVal = CreateSurfaceResult(context, leftFunction, unboundLeft, rightFunction, unboundRight,
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
                retVal = CreateGraphResult(context, leftFunction, unboundLeft, rightFunction, unboundRight, explain);
            }
            else if (max == 2)
            {
                retVal = CreateSurfaceResult(context, leftFunction, unboundLeft, rightFunction, unboundRight, explain);
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
        private ICallable CreateGraphResult(InterpretationContext context, Function leftFunction,
            List<Parameter> unboundLeft, Function rightFunction, List<Parameter> unboundRight, ExplanationPart explain)
        {
            ICallable retVal = null;

            Graph leftGraph = createGraphForUnbound(context, Left, leftFunction, unboundLeft, explain);
            if (leftGraph != null)
            {
                Graph rightGraph = createGraphForUnbound(context, Right, rightFunction, unboundRight, explain);

                if (rightGraph != null)
                {
                    Graph combinedGraph = CombineGraph(leftGraph, rightGraph) as Graph;
                    if (combinedGraph != null)
                    {
                        retVal = combinedGraph.Function;
                    }
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
        private ICallable CreateSurfaceResult(InterpretationContext context, Function leftFunction,
            List<Parameter> unboundLeft, Function rightFunction, List<Parameter> unboundRight, ExplanationPart explain)
        {
            ICallable retVal = null;

            Surface leftSurface = CreateSurfaceForUnbound(context, Left, leftFunction, unboundLeft, explain);
            if (leftSurface != null)
            {
                Surface rightSurface = CreateSurfaceForUnbound(context, Right, rightFunction, unboundRight, explain);
                if (rightSurface != null)
                {
                    retVal = CombineSurface(leftSurface, rightSurface).Function;
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
            Graph retVal;

            if (unbound.Count == 0)
            {
                if (function != null && function.FormalParameters.Count > 0)
                {
                    retVal = function.CreateGraph(context, (Parameter) function.FormalParameters[0], explain);
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
                    retVal = expression.CreateGraph(context, unbound[0], explain);
                }
                else
                {
                    retVal = function.CreateGraph(context, unbound[0], explain);
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
        private Surface CreateSurfaceForUnbound(InterpretationContext context, Expression expression, Function function,
            List<Parameter> unbound, ExplanationPart explain)
        {
            Surface retVal;

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
                    retVal = function.CreateSurfaceForParameters(context, xAxis, yAxis, explain);
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
                    retVal = expression.CreateSurface(context, unbound[0], unbound[1], explain);
                }
                else
                {
                    retVal = function.CreateSurfaceForParameters(context, unbound[0], unbound[1], explain);
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
        private IGraph CombineGraph(IGraph leftGraph, IGraph rightGraph)
        {
            IGraph retVal = null;

            switch (Operation)
            {
                case Operator.Add:
                    retVal = leftGraph.AddGraph(rightGraph);
                    break;

                case Operator.Sub:
                    retVal = leftGraph.SubstractGraph(rightGraph);
                    break;

                case Operator.Mult:
                    retVal = leftGraph.MultGraph(rightGraph);
                    break;

                case Operator.Div:
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
        private Surface CombineSurface(Surface leftSurface, Surface rightSurface)
        {
            Surface retVal = null;

            switch (Operation)
            {
                case Operator.Add:
                    retVal = leftSurface.AddSurface(rightSurface);
                    break;

                case Operator.Sub:
                    retVal = leftSurface.SubstractSurface(rightSurface);
                    break;

                case Operator.Mult:
                    retVal = leftSurface.MultiplySurface(rightSurface);
                    break;

                case Operator.Div:
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
        public override void Fill(List<INamable> retVal, BaseFilter filter)
        {
            Left.Fill(retVal, filter);
            Right.Fill(retVal, filter);
        }

        /// <summary>
        ///     Indicates that the expression is an equality of the form variable == literal
        /// </summary>
        /// <returns></returns>
        public bool IsSimpleEquality()
        {
            bool retVal = false;

            if (Operation == Operator.Equal)
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
        public static string Image(Operator op)
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
        /// <param name="ops"></param>
        /// <returns></returns>
        public static string[] Images(Operator[] ops)
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
        public static Operator FindOperatorByName(string op)
        {
            Operator retVal = Operator.Undef;

            for (int i = 0; i < Operators.Length; i++)
            {
                if (OperatorsImages[i].Equals(op))
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
        public override void CheckExpression()
        {
            base.CheckExpression();

            Left.CheckExpression();
            Right.CheckExpression();

            Type leftType = Left.GetExpressionType();
            if (leftType != null)
            {
                DerefExpression derefExpression = Left as DerefExpression;
                if (derefExpression != null && !derefExpression.IsValidExpressionComponent())
                {
                    Left.AddError("Invalid value " + Left.FullName);
                }
                if (Operation == Operator.Is || (Operation == Operator.As))
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
                        derefExpression = Right as DerefExpression;
                        if (derefExpression != null && !derefExpression.IsValidExpressionComponent())
                        {
                            Left.AddError("Invalid value " + Right.FullName);
                        }
                        if (!leftType.ValidBinaryOperation(Operation, rightType)
                            && !rightType.ValidBinaryOperation(Operation, leftType))
                        {
                            AddError("Cannot perform " + Operation + " operation between " + Left + "(" + leftType.Name +
                                     ") and " + Right + "(" + rightType.Name + ")");
                        }

                        if (Operation == Operator.Equal)
                        {
                            if (leftType is StateMachine && rightType is StateMachine)
                            {
                                AddWarning("IN operator should be used instead of == between " + Left + " and " + Right);
                            }

                            if (Right.Ref == EfsSystem.Instance.EmptyValue)
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
        public override Graph CreateGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = base.CreateGraph(context, parameter, explain);

            Graph leftGraph = Left.CreateGraph(context, parameter, explain);
            if (leftGraph != null)
            {
                Graph rightGraph = Right.CreateGraph(context, parameter, explain);

                if (rightGraph != null)
                {
                    retVal = CombineGraph(leftGraph, rightGraph) as Graph;
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
        public override Surface CreateSurface(InterpretationContext context, Parameter xParam, Parameter yParam,
            ExplanationPart explain)
        {
            Surface retVal = base.CreateSurface(context, xParam, yParam, explain);

            Surface leftSurface = Left.CreateSurface(context, xParam, yParam, explain);
            if (leftSurface != null)
            {
                Surface rightSurface = Right.CreateSurface(context, xParam, yParam, explain);

                if (rightSurface != null)
                {
                    retVal = CombineSurface(leftSurface, rightSurface);
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
        public static Operator Inverse(Operator Operator)
        {
            Operator retVal;

            switch (Operator)
            {
                case Operator.Greater:
                    retVal = Operator.LessOrEqual;
                    break;

                case Operator.GreaterOrEqual:
                    retVal = Operator.Less;
                    break;

                case Operator.Less:
                    retVal = Operator.GreaterOrEqual;
                    break;

                case Operator.LessOrEqual:
                    retVal = Operator.Greater;
                    break;

                default:
                    throw new Exception("Cannot inverse operator " + Operator);
            }

            return retVal;
        }
    }
}