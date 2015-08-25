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
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Values;
using DataDictionary.Variables;
using Collection = DataDictionary.Types.Collection;
using EnumValue = DataDictionary.Constants.EnumValue;
using NameSpace = DataDictionary.Types.NameSpace;
using Range = DataDictionary.Types.Range;
using Structure = DataDictionary.Types.Structure;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Creates a new function which provides a list of targets from a graph
    /// </summary>
    public class Targets : PredefinedFunction
    {
        /// <summary>
        ///     The function providing the speed restrictions
        /// </summary>
        public Parameter SpeedRestrictions { get; private set; }

        /// <summary>
        ///     The return type of the function
        /// </summary>
        public override Type ReturnType
        {
            get
            {
                return
                    EFSSystem.FindType(
                        OverallNameSpaceFinder.INSTANCE.findByName(EFSSystem.Dictionaries[0],
                            "Kernel.SpeedAndDistanceMonitoring.TargetSpeedMonitoring"),
                        "Kernel.SpeedAndDistanceMonitoring.TargetSpeedMonitoring.Targets");
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public Targets(EFSSystem efsSystem)
            : base(efsSystem, "TARGETS")
        {
            SpeedRestrictions = (Parameter) acceptor.getFactory().createParameter();
            SpeedRestrictions.Name = "SpeedRestrictions";
            SpeedRestrictions.Type = EFSSystem.AnyType;
            SpeedRestrictions.setFather(this);
            FormalParameters.Add(SpeedRestrictions);
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
            CheckFunctionalParameter(root, context, actualParameters[SpeedRestrictions.Name], 1);
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

            Collection collectionType =
                (Collection)
                    EFSSystem.FindType(
                        OverallNameSpaceFinder.INSTANCE.findByName(EFSSystem.Dictionaries[0],
                            "Kernel.SpeedAndDistanceMonitoring.TargetSpeedMonitoring"),
                        "Kernel.SpeedAndDistanceMonitoring.TargetSpeedMonitoring.SpeedRestrictions");
            ListValue collection = new ListValue(collectionType, new List<IValue>());

            // compute targets from the MRSP
            Function function1 = context.FindOnStack(SpeedRestrictions).Value as Function;
            if (function1 != null && !function1.Name.Equals("EMPTY"))
            {
                Graph graph1 = createGraphForValue(context, function1, explain);
                ComputeTargets(graph1.Function, collection);
            }

            context.LocalScope.PopContext(token);

            retVal = collection;
            return retVal;
        }

        /// <summary>
        ///     Coputes targets from the function and adds them to the collection
        /// </summary>
        /// <param name="function">Function containing targets</param>
        /// <param name="collection">Collection to be filled with targets</param>
        private void ComputeTargets(Function function, ListValue collection)
        {
            if (function != null)
            {
                Graph graph = function.Graph;
                if (graph != null && graph.Segments.Count > 1)
                {
                    NameSpace nameSpace = OverallNameSpaceFinder.INSTANCE.findByName(EFSSystem.Dictionaries[0],
                        "Kernel.SpeedAndDistanceMonitoring.TargetSpeedMonitoring");
                    Structure structureType = (Structure)
                        EFSSystem.FindType(
                            nameSpace,
                            "Kernel.SpeedAndDistanceMonitoring.TargetSpeedMonitoring.Target"
                        );

                    double prevSpeed = graph.Segments[0].Evaluate(graph.Segments[0].Start);
                    for (int i = 1; i < graph.Segments.Count; i++)
                    {
                        Graph.Segment s = graph.Segments[i];
                        StructureValue value = new StructureValue(structureType);

                        Field speed = value.CreateField(value, "Speed", structureType);
                        speed.Value = new DoubleValue(EFSSystem.DoubleType, s.Evaluate(s.Start));

                        Field location = value.CreateField(value, "Location", structureType);
                        location.Value = new DoubleValue(EFSSystem.DoubleType, s.Start);

                        Field length = value.CreateField(value, "Length", structureType);
                        length.Value = SegmentLength(s.End);

                        // Only add the target for the current segment to the collection if it brings a reduction in permitted speed
                        if (s.Evaluate(s.Start) < prevSpeed)
                        {
                            collection.Val.Add(value);
                        }
                        // But even if it is not added to the collection of targets, this segment is now the reference speed
                        prevSpeed = s.Evaluate(s.Start);
                    }
                }
            }
        }

        /// <summary>
        ///     Ensures that the length of the section is consistent with EFS's length scale
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private IValue SegmentLength(double length)
        {
            IValue retVal = new DoubleValue(EFSSystem.DoubleType, length);

            NameSpace defaultNameSpace = OverallNameSpaceFinder.INSTANCE.findByName(EFSSystem.Dictionaries[0],
                "Default.BaseTypes");
            Range LengthType = defaultNameSpace.findTypeByName("Length") as Range;

            EnumValue infinity = LengthType.findEnumValue("Infinity");
            if (!LengthType.Less(retVal, infinity))
            {
                retVal = infinity;
            }

            return retVal;
        }
    }
}