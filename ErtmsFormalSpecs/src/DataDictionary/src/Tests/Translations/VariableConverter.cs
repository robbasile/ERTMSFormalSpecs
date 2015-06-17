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
using System.Globalization;

namespace DataDictionary.Tests.Translations
{
    public abstract class BaseConverter
    {
        /// <summary>
        ///     Converts the string value into the specific object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract object convertFrom(string value);
    }

    /// <summary>
    ///     Converts a string to an integer value
    /// </summary>
    public class ConvertInt : BaseConverter
    {
        public override object convertFrom(string value)
        {
            return Translation.format_decimal(value);
        }

        /// <summary>
        ///     The singleton
        /// </summary>
        public static ConvertInt INSTANCE = new ConvertInt();
    }

    /// <summary>
    ///     Converts a string to an integer value (coded as hexadecimal value)
    /// </summary>
    public class ConvertHex : BaseConverter
    {
        public override object convertFrom(string value)
        {
            return int.Parse(value, NumberStyles.HexNumber);
        }

        /// <summary>
        ///     The singleton
        /// </summary>
        public static ConvertHex INSTANCE = new ConvertHex();
    }

    /// <summary>
    ///     Converts a string to an integer value (coded as hexadecimal value, removing the trailing FF)
    /// </summary>
    public class ConvertHexTrailingFF : BaseConverter
    {
        public override object convertFrom(string value)
        {
            string val = "";

            bool add = false;

            // Remove trailing " h"  ->  value.Length - 3
            for (int i = value.Length - 3; i >= 0; i--)
            {
                if (value[i] != 'F')
                {
                    add = true;
                }

                if (add)
                {
                    val = value[i] + val;
                }
            }

            // There is a special case where all the characters are F
            if (val == "")
            {
                if (value.Length > 2)
                {
                    val = value.Substring(0, value.Length - 2);
                }
            }
            // Just return the number preceding the FF
            return val;
        }

        /// <summary>
        ///     The singleton
        /// </summary>
        public static ConvertHexTrailingFF INSTANCE = new ConvertHexTrailingFF();
    }

    /// <summary>
    ///     Converts a string to a string
    /// </summary>
    public class ConvertString : BaseConverter
    {
        public override object convertFrom(string value)
        {
            return value;
        }

        /// <summary>
        ///     The singleton
        /// </summary>
        public static ConvertString INSTANCE = new ConvertString();
    }

    /// <summary>
    ///     Provides the way the variable should be converted
    /// </summary>
    public partial class VariableConverter
    {
        /// <summary>
        ///     The converters for each variable
        /// </summary>
        private Dictionary<string, BaseConverter> Converters;

        /// <summary>
        ///     Constructor
        /// </summary>
        public VariableConverter()
        {
            Converters = new Dictionary<string, BaseConverter>();
            Initialise();
        }

        /// <summary>
        ///     Adds the conversion required for a specific variable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="converter"></param>
        public void AddVariable(string name, BaseConverter converter)
        {
            Converters.Add(name, converter);
        }

        /// <summary>
        ///     Converts the string provided to the specific object
        /// </summary>
        /// <param name="variable">The variable name which shall receive the value</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Convert(string variable, string value)
        {
            object retVal = null;

            if (Converters.ContainsKey(variable))
            {
                BaseConverter converter = Converters[variable];
                retVal = converter.convertFrom(value);
            }
            else
            {
                BaseConverter converter = ConverterForKey(variable);
                if (converter != null)
                {
                    retVal = converter.convertFrom(value);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     If the dictionary of variables does not contain a particular name, check for
        ///     cases where there is a more developed version of the variable name
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private BaseConverter ConverterForKey(string Key)
        {
            BaseConverter retVal = null;

            foreach (KeyValuePair<string, BaseConverter> variable in Converters)
            {
                // Check for a key with the name we are looking for, followed by an underscore
                if (variable.Key.StartsWith(Key + "_0"))
                {
                    retVal = variable.Value;
                    break;
                }
            }

            return retVal;
        }


        /// <summary>
        ///     The singleton
        /// </summary>
        public static VariableConverter INSTANCE = new VariableConverter();
    }
}