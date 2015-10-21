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

using System.Drawing;
using DataDictionary;
using DataDictionary.Constants;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;

namespace GUIUtils.Editor.Patterns
{
    /// <summary>
    ///     A pattern used to identify types
    /// </summary>
    public class ConstantPattern : Pattern
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="regExp"></param>
        /// <param name="baseFont"></param>
        public ConstantPattern(Font baseFont, string regExp)
            : base(baseFont, regExp)
        {
        }

        /// <summary>
        ///     Ensures that the string provided is a type
        /// </summary>
        /// <param name="text"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override bool AdditionalCheck(string text, ModelElement instance)
        {
            bool retVal = base.AdditionalCheck(text, instance);

            if (retVal && instance != null)
            {
                Expression expression = new Parser().Expression(instance, text, IsValue.INSTANCE, true,
                    null, true);
                retVal = (expression != null && expression.Ref is EnumValue);
            }

            return retVal;
        }
    }
}
