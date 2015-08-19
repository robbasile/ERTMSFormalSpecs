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
using System.ComponentModel;
using System.Globalization;
using DataDictionary.Generated;
using Type = System.Type;

namespace GUI.Converters
{
    public class GenericEnumConverter<T> : EnumConverter
    {
        /// <summary>
        ///     Provides the translation between a enum and its textual representation
        /// </summary>
        protected struct Converter
        {
            public T Val;
            public string Display;

            public Converter(T val, string display)
            {
                Val = val;
                Display = display;
            }
        }

        /// <summary>
        ///     Holds the convertion rules
        /// </summary>
        protected List<Converter> Converters = new List<Converter>();

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type"></param>
        public GenericEnumConverter(Type type)
            : base(type)
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof (string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destType)
        {
            string retVal = Converters[0].Display;

            T val = (T) value;
            foreach (Converter converter in Converters)
            {
                if (converter.Val.Equals(val))
                {
                    retVal = converter.Display;
                    break;
                }
            }

            return retVal;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof (string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            T retVal = Converters[0].Val;

            foreach (Converter converter in Converters)
            {
                if (converter.Display.CompareTo(value) == 0)
                {
                    retVal = converter.Val;
                    break;
                }
            }

            return retVal;
        }
    }

    public class VariableModeConverter : GenericEnumConverter<acceptor.VariableModeEnumType>
    {
        public VariableModeConverter(Type type)
            : base(type)
        {
            Converters.Add(new Converter(acceptor.VariableModeEnumType.aConstant, "Constant"));
            Converters.Add(new Converter(acceptor.VariableModeEnumType.aIncoming, "In"));
            Converters.Add(new Converter(acceptor.VariableModeEnumType.aInOut, "In Out"));
            Converters.Add(new Converter(acceptor.VariableModeEnumType.aInternal, "Internal"));
            Converters.Add(new Converter(acceptor.VariableModeEnumType.aOutgoing, "Out"));
        }
    }

    public class ImplementationStatusConverter : GenericEnumConverter<acceptor.SPEC_IMPLEMENTED_ENUM>
    {
        public ImplementationStatusConverter(Type type)
            : base(type)
        {
            Converters.Add(new Converter(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA, "Not implemented"));
            Converters.Add(new Converter(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented, "Implemented"));
            Converters.Add(new Converter(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable, "Not implementable"));
            Converters.Add(new Converter(acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NewRevisionAvailable,
                "New revision available"));
        }
    }

    public class ImplementationStatusConverterWithDefault : ImplementationStatusConverter
    {
        public ImplementationStatusConverterWithDefault(Type type)
            : base(type)
        {
            Converters.Add(new Converter(acceptor.SPEC_IMPLEMENTED_ENUM.defaultSPEC_IMPLEMENTED_ENUM, "Default"));
        }
    }

    public class ScopeConverter : GenericEnumConverter<acceptor.Paragraph_scope>
    {
        public ScopeConverter(Type type)
            : base(type)
        {
            Converters.Add(new Converter(acceptor.Paragraph_scope.aOBU, "On Board Unit"));
            Converters.Add(new Converter(acceptor.Paragraph_scope.aOBU_AND_TRACK, "On Board Unit and Track"));
            Converters.Add(new Converter(acceptor.Paragraph_scope.aTRACK, "Track"));
            Converters.Add(new Converter(acceptor.Paragraph_scope.aROLLING_STOCK, "Rolling stock"));
        }
    }

    public class SpecTypeConverter : GenericEnumConverter<acceptor.Paragraph_type>
    {
        public SpecTypeConverter(Type type)
            : base(type)
        {
            Converters.Add(new Converter(acceptor.Paragraph_type.aDEFINITION, "Definition"));
            Converters.Add(new Converter(acceptor.Paragraph_type.aDELETED, "Deleted"));
            Converters.Add(new Converter(acceptor.Paragraph_type.aNOTE, "Note"));
            Converters.Add(new Converter(acceptor.Paragraph_type.aPROBLEM, "Problem"));
            Converters.Add(new Converter(acceptor.Paragraph_type.aREQUIREMENT, "Requirement"));
            Converters.Add(new Converter(acceptor.Paragraph_type.aTITLE, "Title"));
            Converters.Add(new Converter(acceptor.Paragraph_type.aTABLE_HEADER, "Table header"));
        }
    }

    public class RulePriorityConverter : GenericEnumConverter<acceptor.RulePriority>
    {
        public RulePriorityConverter(Type type)
            : base(type)
        {
            Converters.Add(new Converter(acceptor.RulePriority.aProcessing, "Processing"));
            Converters.Add(new Converter(acceptor.RulePriority.aUpdateINTERNAL, "Update INTERNAL variables"));
            Converters.Add(new Converter(acceptor.RulePriority.aUpdateOUT, "Update OUT variables"));
            Converters.Add(new Converter(acceptor.RulePriority.aVerification, "INPUT Verification"));
            Converters.Add(new Converter(acceptor.RulePriority.aCleanUp, "Clean Up"));
        }
    }

    public class CyclePhaseConverter : RulePriorityConverter
    {
        public CyclePhaseConverter(Type type)
            : base(type)
        {
            Converters.Add(new Converter(acceptor.RulePriority.defaultRulePriority, "All"));
        }
    }

    public class RangePrecisionConverter : GenericEnumConverter<acceptor.PrecisionEnum>
    {
        public RangePrecisionConverter(Type type)
            : base(type)
        {
            Converters.Add(new Converter(acceptor.PrecisionEnum.aIntegerPrecision, "Integer"));
            Converters.Add(new Converter(acceptor.PrecisionEnum.aDoublePrecision, "Floating point"));
        }
    }

    public class ExpectationKindConverter : GenericEnumConverter<acceptor.ExpectationKind>
    {
        public ExpectationKindConverter(Type type)
            : base(type)
        {
            Converters.Add(new Converter(acceptor.ExpectationKind.aInstantaneous, "Instantaneous"));
            Converters.Add(new Converter(acceptor.ExpectationKind.aContinuous, "Continuous"));
        }
    }

    public class ChangeActionConverter : GenericEnumConverter<HistoricalData.Generated.acceptor.ChangeOperationEnum>
    {
        public ChangeActionConverter(Type type)
            : base(type)
        {
            Converters.Add(new Converter(HistoricalData.Generated.acceptor.ChangeOperationEnum.aAdd, "Add"));
            Converters.Add(new Converter(HistoricalData.Generated.acceptor.ChangeOperationEnum.aChange, "Change"));
            Converters.Add(new Converter(HistoricalData.Generated.acceptor.ChangeOperationEnum.aRemove, "Remove"));
        }
    }
}