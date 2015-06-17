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
using System.ComponentModel;
using System.Globalization;
using DataDictionary;
using DataDictionary.Tests;

namespace GUI.Converters
{
    /// <summary>
    ///     Converts IExpressionable to string, by getting the Expression property
    /// </summary>
    public class ExpressionableUITypeConverter : StringConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            BaseTreeNode.BaseEditor editor = context.Instance as BaseTreeNode.BaseEditor;
            string text = value as string;
            if (editor != null && text != null)
            {
                IExpressionable expressionable = editor.Model as IExpressionable;
                if (expressionable != null)
                {
                    expressionable.ExpressionText = text;
                    return expressionable;
                }
                else
                {
                    return base.ConvertFrom(context, culture, value);
                }
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            string retVal = "<unknown>";
            IExpressionable expressionable = value as IExpressionable;
            if (expressionable != null)
            {
                if (expressionable.ExpressionText != null)
                {
                    retVal = expressionable.ExpressionText.Trim();
                    if (retVal != null)
                    {
                        int index = retVal.IndexOf("\n");
                        if (index > 0)
                        {
                            retVal = retVal.Substring(0, index) + "...";
                        }
                    }
                    else
                    {
                        retVal = "";
                    }
                }
                else
                {
                    retVal = "";
                }
            }

            return retVal;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
    }


    /// <summary>
    ///     Converts and expectation into a string, by getting the Condition property
    /// </summary>
    public class ConditionUITypeConverter : StringConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            BaseTreeNode.BaseEditor editor = context.Instance as BaseTreeNode.BaseEditor;
            string text = value as string;
            if (editor != null && text != null)
            {
                Expectation expectation = editor.Model as Expectation;
                if (expectation != null)
                {
                    expectation.setCondition(text);
                    return expectation;
                }
                else
                {
                    return base.ConvertFrom(context, culture, value);
                }
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            string retVal = "<unknown>";
            Expectation expectation = value as Expectation;
            if (expectation != null)
            {
                if (expectation.getCondition() != null)
                {
                    retVal = expectation.getCondition().Trim();
                    if (retVal != null)
                    {
                        int index = retVal.IndexOf("\n");
                        if (index > 0)
                        {
                            retVal = retVal.Substring(0, index) + "...";
                        }
                    }
                    else
                    {
                        retVal = "";
                    }
                }
                else
                {
                    retVal = "";
                }
            }

            return retVal;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
    }
}