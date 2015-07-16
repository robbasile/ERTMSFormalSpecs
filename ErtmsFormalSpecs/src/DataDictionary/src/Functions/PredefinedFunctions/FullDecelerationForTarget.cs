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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;
using ErtmsSolutions.Etcs.Subset26.BrakingCurves;
using ErtmsSolutions.SiUnits;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Returns the full deceleration curve (all the way down to zero speed) for the given target
    /// </summary>
    public class FullDecelerationForTarget : FunctionOnGraph
    {
        /// <summary>
        ///     The target used for the deceleration curve
        /// </summary>
        public Parameter Target { get; private set; }

        /// <summary>
        ///     The deceleration factor
        /// </summary>
        public Parameter DecelerationFactor { get; private set; }

        /// <summary>
        ///     The end speed for the calculation
        /// </summary>
        public Parameter EndSpeed { get; private set; }

        public FullDecelerationForTarget(EFSSystem efsSystem)
            : base(efsSystem, "FullDecelerationForTarget")
        {
            Target = (Parameter) acceptor.getFactory().createParameter();
            Target.Name = "Target";
            Target.Type = EFSSystem.AnyType;
            Target.setFather(this);
            FormalParameters.Add(Target);

            DecelerationFactor = (Parameter) acceptor.getFactory().createParameter();
            DecelerationFactor.Name = "DecelerationFactor";
            DecelerationFactor.Type = EFSSystem.AnyType;
            DecelerationFactor.setFather(this);
            FormalParameters.Add(DecelerationFactor);

            // Third parameter end speed
            EndSpeed = (Parameter)acceptor.getFactory().createParameter();
            EndSpeed.Name = "EndSpeed";
            EndSpeed.Type = EFSSystem.AnyType;
            EndSpeed.setFather(this);
            FormalParameters.Add(EndSpeed);
        }

        /// <summary>
        ///     Perform additional checks based on the parameter types
        /// </summary>
        /// <param name="root">The element on which the errors should be reported</param>
        /// <param name="context">The evaluation context</param>
        /// <param name="actualParameters">The parameters applied to this function call</param>
        public override void additionalChecks(ModelElement root, InterpretationContext context,
            Dictionary<string, Expression> actualParameters)
        {
            CheckFunctionalParameter(root, context, actualParameters[Target.Name], 1);
            CheckFunctionalParameter(root, context, actualParameters[DecelerationFactor.Name], 2);
            CheckFunctionalParameter(root, context, actualParameters[EndSpeed.Name], 3);
        }

        /// <summary>
        ///     Creates a graph for the function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameter"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public override Graph createGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = new Graph();

            StructureValue LocationStruct = context.findOnStack(Target).Value as StructureValue;

            SiDistance location;
            SiSpeed speed;

            if (LocationStruct != null)
            {
                Variable Location = LocationStruct.Val["Location"] as Variable;
                location = new SiDistance((Location.Value as DoubleValue).Val);
                Variable Speed = LocationStruct.Val["Speed"] as Variable;
                speed = new SiSpeed((Speed.Value as DoubleValue).Val, SiSpeed_SubUnits.KiloMeter_per_Hour);

                Function decelerationFactor = context.findOnStack(DecelerationFactor).Value as Function;
                if (decelerationFactor != null)
                {
                    Surface DecelerationSurface = decelerationFactor.createSurface(context, explain);
                    if (DecelerationSurface != null)
                    {
                        AccelerationSpeedDistanceSurface accelerationSurface =
                            DecelerationSurface.createAccelerationSpeedDistanceSurface(double.MaxValue, double.MaxValue);

                        QuadraticSpeedDistanceCurve BrakingCurve = null;

                        try
                        {
                            BrakingCurve = EtcsBrakingCurveBuilder.Build_Deceleration_Curve(accelerationSurface, speed,
                                location);
                        }
                        catch (Exception e)
                        {
                            retVal.addSegment(new Graph.Segment(0, double.MaxValue, new Graph.Segment.Curve(0, 0, 0)));
                        }

                        SiSpeed finalSpeed = new SiSpeed(getDoubleValue(context.findOnStack(EndSpeed).Value), SiSpeed_SubUnits.KiloMeter_per_Hour);
                        for (int i = 0; i < BrakingCurve.SegmentCount; i++)
                        {
                            QuadraticCurveSegment segment = BrakingCurve[i];

                            SiSpeed endSpeed = Max(finalSpeed, segment.Get(segment.X.X1));

                            SiDistance endDistance;
                            if (endSpeed == finalSpeed)
                            {
                                // Ensures that a braking curve is calculated until the finalSpeed
                                // but not further than the end of the curve segment
                                SiSpeed tmp = Max(  segment.Get(segment.X.X1),
                                                            endSpeed - new SiSpeed(0.001));
                                endDistance = segment.IntersectAt(tmp);
                            }
                            else
                            {
                                endDistance = segment.X.X1;
                            }

                            Graph.Segment newSegment = new Graph.Segment(
                                segment.X.X0.ToUnits(),
                                endDistance.ToUnits(),
                                new Graph.Segment.Curve(
                                    segment.A.ToSubUnits(SiAcceleration_SubUnits.Meter_per_SecondSquare),
                                    segment.V0.ToSubUnits(SiSpeed_SubUnits.KiloMeter_per_Hour),
                                    segment.D0.ToSubUnits(SiDistance_SubUnits.Meter)
                                    )
                                );
                            retVal.addSegment(newSegment);

                            if (endSpeed == finalSpeed)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        ///     Returns the maximum, non-NaN speed
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private SiSpeed Max(SiSpeed first, SiSpeed second)
        {
            SiSpeed retVal;

            if (double.IsNaN(first.Value))
            {
                retVal = second;
            }
            else if (double.IsNaN(second.Value))
            {
                retVal = first;
            }
            else
            {
                retVal = SiSpeed.Max(first, second);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value of the function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actuals">the actual parameters values</param>
        /// <param name="explain"></param>
        /// <returns>The value for the function application</returns>
        public override IValue Evaluate(InterpretationContext context, Dictionary<Actual, IValue> actuals,
            ExplanationPart explain)
        {
            IValue retVal = null;

            int token = context.LocalScope.PushContext();
            AssignParameters(context, actuals);

            Function function = (Function) acceptor.getFactory().createFunction();
            function.Name = "FullDecelerationForTarget ( Target => " + getName(Target) + ", DecelerationFactor => " +
                            getName(DecelerationFactor) + " )";
            function.Enclosing = EFSSystem;
            Parameter parameter = (Parameter) acceptor.getFactory().createParameter();
            parameter.Name = "X";
            parameter.Type = EFSSystem.DoubleType;
            function.appendParameters(parameter);
            function.ReturnType = EFSSystem.DoubleType;
            function.Graph = createGraph(context, parameter, explain);

            retVal = function;
            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}