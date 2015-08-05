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
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Structure = DataDictionary.Types.Structure;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace DataDictionary.Functions.PredefinedFunctions
{
    /// <summary>
    ///     Adds a number of milliseconds to a date, and returns a new date
    /// </summary>
    public class AddToDate : PredefinedFunction
    {
        /// <summary>
        ///     The base date which is being added to
        /// </summary>
        public Parameter StartDate { get; private set; }

        /// <summary>
        ///     The time added to the date, in milliseconds
        /// </summary>
        public Parameter Increment { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem"></param>
        public AddToDate(EFSSystem efsSystem)
            : base(efsSystem, "AddToDate")
        {
            StartDate = (Parameter) acceptor.getFactory().createParameter();
            StartDate.Name = "StartDate";
            StartDate.Type = EFSSystem.AnyType;
            StartDate.setFather(this);
            FormalParameters.Add(StartDate);

            Increment = (Parameter) acceptor.getFactory().createParameter();
            Increment.Name = "Increment";
            Increment.Type = EFSSystem.DoubleType;
            Increment.setFather(this);
            FormalParameters.Add(Increment);
        }

        /// <summary>
        ///     The return type of the function
        /// </summary>
        public override Type ReturnType
        {
            get
            {
                return
                    EFSSystem.FindType(
                        OverallNameSpaceFinder.INSTANCE.findByName(EFSSystem.Dictionaries[0], "Default"),
                        "Default.DateAndTime");
            }
        }


        private StructureValue GetStructFromParam(Parameter parameter)
        {
            StructureValue retVal = null;

            if (parameter != null)
            {
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the int value which corresponds to a specific field of the structure provided
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetIntValue(StructureValue structure, string name)
        {
            int retVal = 0;

            Variable variable = structure.Val[name] as Variable;
            IntValue intValue = variable.Value as IntValue;
            retVal = (int) intValue.Val;

            return retVal;
        }

        /// <summary>
        ///     Converts an int to a EFS IntValue
        /// </summary>
        /// <param name="value">The int value to be converted</param>
        /// <returns></returns>
        private IntValue ToEFSInt(int value)
        {
            return new IntValue(new IntegerType(EFSSystem), value);
        }

        /// <summary>
        ///     Creates a EFS DateAndTime structure from a System.DateTime
        /// </summary>
        /// <param name="value">The values that will go into the structure</param>
        /// <param name="structureType">The structure type</param>
        /// <returns></returns>
        private IValue GetEFSDate(DateTime value, Structure structureType)
        {
            IValue retVal = null;

            StructureValue structure = new StructureValue(structureType);

            structure.SubVariables["Year"].Value = ToEFSInt(value.Year);
            structure.SubVariables["Month"].Value = ToEFSInt(value.Month);
            structure.SubVariables["Day"].Value = ToEFSInt(value.Day);
            structure.SubVariables["Hour"].Value = ToEFSInt(value.Hour);
            structure.SubVariables["Minute"].Value = ToEFSInt(value.Minute);
            structure.SubVariables["Second"].Value = ToEFSInt(value.Second);
            structure.SubVariables["TTS"].Value = ToEFSInt(value.Millisecond);

            retVal = structure;

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

            StructureValue startDate = context.findOnStack(StartDate).Value as StructureValue;
            if (startDate != null)
            {
                int year = GetIntValue(startDate, "Year");
                int month = GetIntValue(startDate, "Month");
                int day = GetIntValue(startDate, "Day");
                int hour = GetIntValue(startDate, "Hour");
                int minute = GetIntValue(startDate, "Minute");
                int second = GetIntValue(startDate, "Second");
                int tts = GetIntValue(startDate, "TTS");

                DoubleValue addedTime = context.findOnStack(Increment).Value as DoubleValue;
                if (addedTime != null)
                {
                    DateTime start = new DateTime(year, month, day, hour, minute, second, tts);
                    DateTime currentTime = start.AddSeconds((double) addedTime.Val);

                    retVal = GetEFSDate(currentTime, startDate.Type as Structure);
                }
            }

            context.LocalScope.PopContext(token);

            return retVal;
        }
    }
}