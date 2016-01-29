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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Values;
using Utils;
using EnumValue = DataDictionary.Constants.EnumValue;

namespace DataDictionary.Types
{
    public class Range : Generated.Range, IEnumerateValues, ISubDeclarator, ITextualExplain
    {
        /// <summary>
        ///     The min value of the range
        /// </summary>
        public string MinValue
        {
            get { return getMinValue(); }
            set
            {
                setMinValue(value);
                minValueSet = false;
            }
        }

        /// <summary>
        ///     A cache for the min value
        /// </summary>
        private bool minValueSet;

        private Decimal minValueAsLong;
        private double minValueAsDouble;

        /// <summary>
        ///     Sets the minimum value (both decimal and double)
        /// </summary>
        private void SetMinValue()
        {
            minValueAsLong = Decimal.Parse(MinValue, CultureInfo.InvariantCulture);
            minValueAsDouble = getDouble(MinValue);
            minValueSet = true;
        }

        public Decimal MinValueAsLong
        {
            get
            {
                if (!minValueSet)
                {
                    SetMinValue();
                }
                return minValueAsLong;
            }
        }

        public double MinValueAsDouble
        {
            get
            {
                if (!minValueSet)
                {
                    SetMinValue();
                }
                return minValueAsDouble;
            }
        }

        /// <summary>
        ///     The max value of the range
        /// </summary>
        public string MaxValue
        {
            get { return getMaxValue(); }
            set
            {
                setMaxValue(value);
                maxValueSet = false;
            }
        }

        /// <summary>
        ///     A cache for the min value
        /// </summary>
        private bool maxValueSet = false;

        private Decimal maxValueAsLong;
        private double maxValueAsDouble;

        /// <summary>
        ///     Sets the maximum value (both decimal and double)
        /// </summary>
        private void SetMaxValue()
        {
            maxValueAsLong = Decimal.Parse(MaxValue, CultureInfo.InvariantCulture);
            maxValueAsDouble = getDouble(MaxValue);
            maxValueSet = true;
        }

        public Decimal MaxValueAsLong
        {
            get
            {
                if (!maxValueSet)
                {
                    SetMaxValue();
                }
                return maxValueAsLong;
            }
        }

        public double MaxValueAsDouble
        {
            get
            {
                if (!maxValueSet)
                {
                    SetMaxValue();
                }
                return maxValueAsDouble;
            }
        }

        /// <summary>
        ///     The special values of the range
        /// </summary>
        public ArrayList SpecialValues
        {
            get
            {
                if (allSpecialValues() == null)
                {
                    setAllSpecialValues(new ArrayList());
                }
                return allSpecialValues();
            }
            set { setAllSpecialValues(value); }
        }

        /// <summary>
        ///     Just a cache
        /// </summary>
        private static Dictionary<string, double> cache = new Dictionary<string, double>();

        /// <summary>
        ///     Faster method of getting a double from its string representation
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static double getDouble(string image)
        {
            double retVal;

            if (!cache.TryGetValue(image, out retVal))
            {
                retVal = double.Parse(image, CultureInfo.InvariantCulture);
                cache.Add(image, retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     Parses the image and provides the corresponding value
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override IValue getValue(string image)
        {
            IValue retVal = null;

            if (image.Length > 0 && Char.IsLetter(image[0]) || image[0] == '_')
            {
                int lastDotPosition = image.LastIndexOf('.');
                if (lastDotPosition > 0)
                {
                    string prefix = image.Substring(0, lastDotPosition);
                    Expression typeExpression = new Parser().Expression(this, prefix,
                        IsType.INSTANCE, true, null, true);
                    if (typeExpression != null && typeExpression.Ref == this)
                    {
                        image = image.Substring(lastDotPosition + 1);
                    }
                }

                retVal = findEnumValue(image);
                if (retVal == null)
                {
                    AddError("Cannot create range value from " + image);
                }
            }
            else
            {
                try
                {
                    switch (getPrecision())
                    {
                        case acceptor.PrecisionEnum.aIntegerPrecision:
                        {
                            Decimal val = Decimal.Parse(image);
                            Decimal min = MinValueAsLong;
                            Decimal max = MaxValueAsLong;
                            if (val >= min && val <= max)
                            {
                                retVal = new IntValue(this, val);
                            }
                        }
                            break;

                        case acceptor.PrecisionEnum.aDoublePrecision:
                        {
                            double val = getDouble(image);
                            double min = MinValueAsDouble;
                            double max = MaxValueAsDouble;
                            if (val >= min && val <= max)
                            {
                                retVal = new DoubleValue(this, val);
                            }
                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    AddException(exception);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates whether a value can be cast into this type
        /// </summary>
        public override bool CanBeCastInto
        {
            get { return true; }
        }

        /// <summary>
        ///     Converts a value in this type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public override IValue convert(IValue value)
        {
            IValue retVal = null;

            EnumValue enumValue = value as EnumValue;
            if (enumValue != null && enumValue.Range != null)
            {
                retVal = findEnumValue(enumValue.Name);
                if (retVal == null)
                {
                    AddError("Cannot convert " + enumValue.Name + " to " + FullName);
                }
            }
            else
            {
                try
                {
                    switch (getPrecision())
                    {
                        case acceptor.PrecisionEnum.aIntegerPrecision:
                        {
                            Decimal val = getValueAsInt(value);
                            Decimal min = MinValueAsLong;
                            Decimal max = MaxValueAsLong;
                            if (val >= min && val <= max)
                            {
                                retVal = new IntValue(this, val);
                            }
                        }
                            break;
                        case acceptor.PrecisionEnum.aDoublePrecision:
                        {
                            double val = getValueAsDouble(value);
                            double min = MinValueAsDouble;
                            double max = MaxValueAsDouble;
                            if (val >= min && val <= max)
                            {
                                retVal = new DoubleValue(this, val);
                            }
                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    AddException(exception);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value as a decimal value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Decimal getValueAsInt(IValue value)
        {
            Decimal retVal;

            IntValue intVal = value as IntValue;
            if (intVal != null)
            {
                retVal = intVal.Val;
            }
            else
            {
                DoubleValue doubleVal = value as DoubleValue;
                if (doubleVal != null)
                {
                    retVal = new Decimal(Math.Round(doubleVal.Val));
                }
                else
                {
                    throw new Exception("Cannot convert value " + value + " to " + FullName);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the value as a double value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Double getValueAsDouble(IValue value)
        {
            Double retVal;

            IntValue intVal = value as IntValue;
            if (intVal != null)
            {
                retVal = Decimal.ToDouble(intVal.Val);
            }
            else
            {
                DoubleValue doubleVal = value as DoubleValue;
                if (doubleVal != null)
                {
                    retVal = doubleVal.Val;
                }
                else
                {
                    throw new Exception("Cannot convert value " + value + " to " + FullName);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the enclosing collection to allow deletion of a range
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return NameSpace.Ranges; }
        }

        /// <summary>
        ///     Derefs enumerate values
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private IValue derefEnum(IValue val)
        {
            IValue retVal = val;

            EnumValue enumValue = retVal as EnumValue;
            if (enumValue != null)
            {
                retVal = enumValue.Value;
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates that binary operation is valid for this type and the other type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool ValidBinaryOperation(BinaryExpression.Operator operation, Type otherType)
        {
            bool retVal = base.ValidBinaryOperation(operation, otherType);

            if (!retVal)
            {
                if (operation == BinaryExpression.Operator.Add || operation == BinaryExpression.Operator.Div ||
                    operation == BinaryExpression.Operator.Mult || operation == BinaryExpression.Operator.Sub)
                {
                    // Allow implicit conversions
                    IntegerType integerType = otherType as IntegerType;
                    if (integerType != null)
                    {
                        retVal = true;
                    }
                    else
                    {
                        DoubleType doubleType = otherType as DoubleType;
                        retVal = (doubleType != null);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the arithmetic operation based on the type of the result
        /// </summary>
        /// <param name="context">The context used to perform this operation</param>
        /// <param name="left"></param>
        /// <param name="Operation"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public override IValue PerformArithmericOperation(InterpretationContext context, IValue left,
            BinaryExpression.Operator Operation, IValue right) // left +/-/*/div/exp right
        {
            IValue retVal = null;

            left = derefEnumForArithmeticOperation(left);
            right = derefEnumForArithmeticOperation(right);

            IntValue int1 = left as IntValue;
            IntValue int2 = right as IntValue;

            if (int1 == null || int2 == null)
            {
                retVal = EFSSystem.DoubleType.PerformArithmericOperation(context, left, Operation, right);
            }
            else
            {
                retVal = EFSSystem.IntegerType.PerformArithmericOperation(context, left, Operation, right);
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the dereferencing and launches an exception if the enum cannot be used for arithmetic operations
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IValue derefEnumForArithmeticOperation(IValue value)
        {
            IValue retVal = value;

            EnumValue enumValue = value as EnumValue;
            if (enumValue != null)
            {
                if (enumValue.getForbidArithmeticOperation())
                {
                    throw new Exception("Cannot perform arithmetic operation with " + value.LiteralName);
                }
                else
                {
                    retVal = enumValue.Value;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Compares two ranges for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public override bool CompareForEquality(IValue left, IValue right)
        {
            bool retVal = false;

            left = derefEnum(left);
            right = derefEnum(right);

            IntValue int1 = left as IntValue;
            IntValue int2 = right as IntValue;

            if (int1 != null && int2 != null)
            {
                retVal = (int1.Val == int2.Val);
            }
            else
            {
                DoubleValue double1 = left as DoubleValue;
                DoubleValue double2 = right as DoubleValue;

                if (double1 != null && double2 != null)
                {
                    retVal = DoubleType.CompareDoubleForEquality(double1.Val, double2.Val);
                    ;
                }
                else
                {
                    retVal = base.CompareForEquality(left, right);
                }
            }

            return retVal;
        }

        public override bool Less(IValue left, IValue right) // left < right
        {
            bool retVal = false;

            left = derefEnum(left);
            right = derefEnum(right);

            IntValue int1 = left as IntValue;
            IntValue int2 = right as IntValue;

            if (int1 != null && int2 != null)
            {
                retVal = int1.Val < int2.Val;
            }
            else
            {
                retVal = EFSSystem.DoubleType.Less(left, right);
            }

            return retVal;
        }

        public override bool Greater(IValue left, IValue right) // left > right
        {
            bool retVal = false;

            left = derefEnum(left);
            right = derefEnum(right);

            IntValue int1 = left as IntValue;
            IntValue int2 = right as IntValue;

            if (int1 != null && int2 != null)
            {
                retVal = int1.Val > int2.Val;
            }
            else
            {
                retVal = EFSSystem.DoubleType.Greater(left, right);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides all constant values for this type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="retVal"></param>
        public void Constants(string scope, Dictionary<string, object> retVal)
        {
            foreach (EnumValue value in SpecialValues)
            {
                string name = Utils.Util.concat(scope, value.Name);
                retVal[name] = retVal;
            }
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            foreach (EnumValue value in SpecialValues)
            {
                ISubDeclaratorUtils.AppendNamable(this, value);
            }
        }

        /// <summary>
        ///     Provides all the values that can be stored in this range
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; set; }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        ///     Provides the enum value which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EnumValue findEnumValue(string name)
        {
            EnumValue retVal = null;

            retVal = (EnumValue) NamableUtils.FindByName(name, SpecialValues);

            return retVal;
        }

        /// <summary>
        ///     Provides the values whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        public IValue findValue(string[] names, int index)
        {
            // HaCK: we should check the enclosing range names
            return findEnumValue(names[names.Length - 1]);
        }

        /// <summary>
        ///     Indicates that the other type may be placed in this range
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            if (!retVal)
            {
                if (otherType is IntegerType && getPrecision() == acceptor.PrecisionEnum.aIntegerPrecision)
                {
                    retVal = true;
                }
                else if (otherType is DoubleType && getPrecision() == acceptor.PrecisionEnum.aDoublePrecision)
                {
                    retVal = true;
                }
                else
                {
                    Range otherRange = otherType as Range;
                    if (otherRange != null && getPrecision() == otherRange.getPrecision())
                    {
                        retVal = true;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                EnumValue item = element as EnumValue;
                if (item != null)
                {
                    appendSpecialValues(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            base.GetExplain(explanation, explainSubElements);
            explanation.Write("RANGE ");
            explanation.Write(Name);
            explanation.Write(" FROM ");
            explanation.Write(MinValue);
            explanation.Write(" TO ");
            explanation.WriteLine(MaxValue);

            explanation.Indent(2, () =>
            {
                foreach (EnumValue enumValue in SpecialValues)
                {
                    enumValue.GetExplain(explanation, explainSubElements);
                }
            });
        }

        /// <summary>
        ///     Combines two types to create a new one
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public override Type CombineType(Type right, BinaryExpression.Operator Operator)
        {
            Type retVal = null;

            if (Operator == BinaryExpression.Operator.Mult)
            {
                if (FullName.CompareTo("Default.BaseTypes.Speed") == 0 &&
                    right.FullName.CompareTo("Default.BaseTypes.Time") == 0)
                {
                    NameSpace nameSpace = EnclosingNameSpaceFinder.find(this);
                    retVal = nameSpace.findTypeByName("Distance");
                }
            }
            else
            {
                if (IsDouble())
                {
                    if (right == EFSSystem.DoubleType)
                    {
                        retVal = this;
                    }
                }
                else
                {
                    if (right == EFSSystem.IntegerType)
                    {
                        retVal = this;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a copy of the range in the designated dictionary. The namespace structure is copied over.
        ///     The new range is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public Range CreateRangeUpdate(Dictionary dictionary)
        {
            Range retVal = (Range) Duplicate();
            retVal.setUpdates(Guid);
            retVal.ClearAllRequirements();

            String[] names = FullName.Split('.');
            names = names.Take(names.Count() - 1).ToArray();
            NameSpace nameSpace = dictionary.GetNameSpaceUpdate(names, Dictionary);
            nameSpace.appendRanges(retVal);

            return retVal;
        }

        /// <summary>
        ///     When importing a range, keep the special values then copy all fields
        /// </summary>
        /// <param name="source"></param>
        protected override void UpdateModelElementAccordingToSource(ModelElement source)
        {
            Range sourceRange = source as Range;
            if (sourceRange != null)
            {
                SpecialValues = sourceRange.SpecialValues;
            }

            base.UpdateModelElementAccordingToSource(source);
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string result = base.CreateStatusMessage();

            result += "Range " + Name + " with " + SpecialValues.Count + " special values";

            return result;
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static Range CreateDefault(ICollection enclosingCollection)
        {
            Range retVal = (Range) acceptor.getFactory().createRange();

            Util.DontNotify(() =>
            {
                retVal.Name = "Range" + GetElementNumber(enclosingCollection);
                retVal.MinValue = "0";
                retVal.MaxValue = "1";
            });

            return retVal;
        }
    }
}