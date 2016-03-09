// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpecs software and documentation
// --
// --  ERTMSFormalSpecs is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpecs is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using DataDictionary;
using DataDictionary.Interpreter;
using DataDictionary.Values;

namespace GUIUtils.GraphVisualization.Functions
{
    /// <summary>
    /// Contains the information related to train position
    /// </summary>
    public class TrainPosition
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public TrainPosition ()
        {
        }

        /// <summary>
        /// Provides the train's distance
        /// </summary>
        /// <returns></returns>
        public double GetDistance()
        {
            double retVal = double.NaN;
            DoubleValue value = GetExpressionValue("Kernel.TrainPosition.FrontEndPosition( PointOfInterest => Default.DistanceIntervalEnum.Nom )") as DoubleValue;
            if (value != null)
            {
                retVal = value.Val;
            }
            return retVal;
        }

        /// <summary>
        /// Provides the train's speed
        /// </summary>
        /// <returns></returns>
        public double GetSpeed()
        {
            double retVal = double.NaN;
            DoubleValue value = GetExpressionValue("Odometry.EstimatedSpeed") as DoubleValue;
            if (value != null)
            {
                retVal = value.Val;
            }
            return retVal;
        }

        /// <summary>
        /// Provides the train's underreading amount
        /// </summary>
        /// <returns></returns>
        public double GetUnderReadingAmount()
        {
            double retVal = double.NaN;
            DoubleValue value =
                GetExpressionValue(
                                    "Kernel.TrainPosition.ConfidenceInterval.ConfidenceInterval( Direction => Default.ConfidenceIntervalEnum.L_DOUBTUNDER )")
                as DoubleValue;
            if (value != null)
            {
                retVal = value.Val;
            }
            return retVal;
        }

        /// <summary>
        /// Provides the train's overreading amount
        /// </summary>
        /// <returns></returns>
        public double GetOverReadingAmount ()
        {
            double retVal = double.NaN;
            DoubleValue value =
                GetExpressionValue (
                                    "Kernel.TrainPosition.ConfidenceInterval.ConfidenceInterval( Direction => Default.ConfidenceIntervalEnum.L_DOUBTOVER )")
                as DoubleValue;
            if(value != null)
            {
                retVal = value.Val;
            }
            return retVal;
        }

        /// <summary>
        /// Provides the value corresponding to the provided expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static IValue GetExpressionValue(string expression)
        {
            IValue retVal = null;
            if (EfsSystem.Instance.Dictionaries.Count > 0)
            {
                Parser parser = new Parser();
                Expression tree = parser.Expression(EfsSystem.Instance.Dictionaries[0], expression, null, true, null, true);
                InterpretationContext context = new InterpretationContext();
                retVal = tree.GetExpressionValue(context, null);
            }
            return retVal;
        }
    }
}
