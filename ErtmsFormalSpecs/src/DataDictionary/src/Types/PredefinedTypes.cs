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
using System.Globalization;
using DataDictionary.Functions;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Values;
using Utils;
using EnumValue = DataDictionary.Constants.EnumValue;
using Function = DataDictionary.Functions.Function;

namespace DataDictionary.Types
{
    public class PredefinedType : Type
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        public PredefinedType(EFSSystem system, string name)
        {
            Enclosing = system;
            Name = name;
        }

        /// <summary>
        ///     Provides the values whose name matches the name provided
        /// </summary>
        /// <param name="index">the index in names to consider</param>
        /// <param name="names">the simple value names</param>
        public virtual IValue findValue(string[] names, int index)
        {
            IValue retVal = null;

            if (index == names.Length - 1)
            {
                retVal = getValue(names[index]);
            }

            return retVal;
        }
    }

    public class BoolType : PredefinedType, IEnumerateValues, ISubDeclarator
    {
        public override string Name
        {
            get { return "Boolean"; }
            set { }
        }

        /// <summary>
        ///     The true constant value
        /// </summary>
        private BoolValue trueValue;

        public BoolValue True
        {
            get { return trueValue; }
            private set { trueValue = value; }
        }

        /// <summary>
        ///     The false constant value
        /// </summary>
        private BoolValue falseValue;

        public BoolValue False
        {
            get { return falseValue; }
            private set { falseValue = value; }
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            ISubDeclaratorUtils.AppendNamable(this, True);
            ISubDeclaratorUtils.AppendNamable(this, False);
        }

        /// <summary>
        ///     The elements declared by this declarator
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
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public BoolType(EFSSystem efsSystem)
            : base(efsSystem, "Boolean")
        {
            True = new BoolValue(this, true);
            False = new BoolValue(this, false);

            InitDeclaredElements();
        }

        /// <summary>
        ///     Provides all constant values for this type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="retVal"></param>
        public void Constants(string scope, Dictionary<string, object> retVal)
        {
            if (Utils.Utils.isEmpty(scope))
            {
                retVal[True.Name] = True;
                retVal[False.Name] = False;
            }
        }

        /// <summary>
        ///     Gets a value based on its image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override IValue getValue(string image)
        {
            IValue retVal = null;

            if (Char.IsLetter(image[0]) || image[0] == '_')
            {
                int lastDotPosition = image.LastIndexOf('.');
                if (lastDotPosition > 0)
                {
                    string prefix = image.Substring(0, lastDotPosition);
                    Expression typeExpression = EFSSystem.Parser.Expression(this, prefix,
                        IsType.INSTANCE, true, null, true);
                    if (typeExpression != null && typeExpression.Ref == this)
                    {
                        image = image.Substring(lastDotPosition + 1);
                    }
                }

                List<INamable> tmp = new List<INamable>();
                ISubDeclaratorUtils.Find(this, image, tmp);
                if (tmp.Count == 1)
                {
                    retVal = tmp[0] as IValue;
                }

                if (retVal == null)
                {
                    Log.Error("Cannot find boolean from " + image);
                }
            }

            return retVal;
        }

        public override string Default
        {
            get { return "False"; }
        }

        public override bool CompareForEquality(IValue left, IValue right)
        {
            bool retVal = false;

            BoolValue bool1 = left as BoolValue;
            BoolValue bool2 = right as BoolValue;

            if (bool1 != null && bool2 != null)
            {
                retVal = bool1.Val == bool2.Val;
            }

            return retVal;
        }
    }

    public class IntegerType : PredefinedType
    {
        public override string Name
        {
            get { return "Integer"; }
            set { }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public IntegerType(EFSSystem efsSystem)
            : base(efsSystem, "Integer")
        {
        }

        /// <summary>
        ///     The default value
        /// </summary>
        public override IValue DefaultValue
        {
            get
            {
                IValue retVal = new IntValue(EFSSystem.IntegerType, 0);

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates that the other type can be placed in variables of this type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            if (!retVal)
            {
                Range range = otherType as Range;
                if (range != null && range.getPrecision() == acceptor.PrecisionEnum.aIntegerPrecision)
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Gets a value based on its image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override IValue getValue(string image)
        {
            IValue retVal = null;

            try
            {
                retVal = new IntValue(this, Decimal.Parse(image));
            }
            catch (Exception e)
            {
                AddException(e);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the int value from the IValue provided
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private int getValue(IValue val)
        {
            int retVal = 0;

            EnumValue enumValue = val as EnumValue;
            if (enumValue != null)
            {
                val = enumValue.Value;
            }

            IntValue vi = val as IntValue;
            if (vi != null)
            {
                retVal = (int) vi.Val;
            }
            else
            {
                throw new Exception("Cannot get integer value from " + val.LiteralName);
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
            BinaryExpression.OPERATOR Operation, IValue right) // left +/-/*/div/exp right
        {
            IntValue retVal = null;

            int int1 = getValue(left);
            int int2 = getValue(right);

            switch (Operation)
            {
                case BinaryExpression.OPERATOR.EXP:
                    retVal = new IntValue(EFSSystem.IntegerType, (Decimal) Math.Pow((double) int1, (double) int2));
                    break;

                case BinaryExpression.OPERATOR.MULT:
                    retVal = new IntValue(EFSSystem.IntegerType, (int1*int2));
                    break;

                case BinaryExpression.OPERATOR.DIV:
                    if (int2 == 0)
                        throw new Exception("Division by zero");
                    else
                        retVal = new IntValue(EFSSystem.IntegerType, (int1/int2));
                    break;

                case BinaryExpression.OPERATOR.ADD:
                    retVal = new IntValue(EFSSystem.IntegerType, (int1 + int2));
                    break;

                case BinaryExpression.OPERATOR.SUB:
                    retVal = new IntValue(EFSSystem.IntegerType, (int1 - int2));
                    break;
            }

            return retVal;
        }

        public override bool CompareForEquality(IValue left, IValue right)
        {
            bool retVal = false;

            int int1 = getValue(left);
            int int2 = getValue(right);

            retVal = int1 == int2;

            return retVal;
        }

        public override bool Less(IValue left, IValue right) // left < right
        {
            bool retVal = false;

            int int1 = getValue(left);
            int int2 = getValue(right);

            retVal = int1 < int2;

            return retVal;
        }

        public override bool Greater(IValue left, IValue right) // left > right
        {
            bool retVal = false;

            int int1 = getValue(left);
            int int2 = getValue(right);

            retVal = int1 > int2;

            return retVal;
        }

        /// <summary>
        ///     One can cast into a int
        /// </summary>
        public override bool CanBeCastInto
        {
            get { return true; }
        }

        /// <summary>
        ///     Converts a value into an int value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IValue convert(IValue value)
        {
            IValue retVal = new IntValue(this, getValue(value));

            return retVal;
        }
    }

    public class DoubleType : PredefinedType
    {
        public override string Name
        {
            get { return "Double"; }
            set { }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        public DoubleType(EFSSystem efsSystem)
            : base(efsSystem, "Double")
        {
        }

        /// <summary>
        ///     The default value
        /// </summary>
        public override IValue DefaultValue
        {
            get
            {
                IValue retVal = new DoubleValue(EFSSystem.DoubleType, 0.0);

                return retVal;
            }
        }

        /// <summary>
        ///     Indicates that the other type can be placed in variables of this type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool retVal = base.Match(otherType);

            if (!retVal)
            {
                Range range = otherType as Range;
                if (range != null && range.getPrecision() == acceptor.PrecisionEnum.aDoublePrecision)
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Gets a value based on its image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override IValue getValue(string image)
        {
            CultureInfo info = CultureInfo.InvariantCulture;
            DoubleValue retVal = new DoubleValue(this, double.Parse(image, info.NumberFormat));

            return retVal;
        }

        /// <summary>
        ///     Provides the double value from the IValue provided
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private double getValue(IValue val)
        {
            double retVal = 0;

            EnumValue enumValue = val as EnumValue;
            if (enumValue != null)
            {
                val = enumValue.Value;
            }

            DoubleValue vd = val as DoubleValue;
            if (vd != null)
            {
                retVal = vd.Val;
            }
            else
            {
                IntValue vi = val as IntValue;
                if (vi != null)
                {
                    retVal = (double) vi.Val;
                }
                else
                {
                    Function function = val as Function;
                    if (function != null)
                    {
                        Graph graph = function.Graph;
                        if (graph != null)
                        {
                            if (graph.Segments.Count == 1)
                            {
                                retVal = graph.Evaluate(0);
                            }
                        }
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
            BinaryExpression.OPERATOR Operation, IValue right) // left +/-/*/div/exp right
        {
            DoubleValue retVal = null;

            double double1 = getValue(left);
            double double2 = getValue(right);

            switch (Operation)
            {
                case BinaryExpression.OPERATOR.EXP:
                    retVal = new DoubleValue(this, Math.Pow(double1, double2));
                    break;

                case BinaryExpression.OPERATOR.MULT:
                    retVal = new DoubleValue(this, (double1*double2));
                    break;

                case BinaryExpression.OPERATOR.DIV:
                    if (double2 == 0)
                        throw new Exception("Division by zero");
                    else
                        retVal = new DoubleValue(this, (double1/double2));
                    break;

                case BinaryExpression.OPERATOR.ADD:
                    retVal = new DoubleValue(this, (double1 + double2));
                    break;

                case BinaryExpression.OPERATOR.SUB:
                    retVal = new DoubleValue(this, (double1 - double2));
                    break;
            }

            return retVal;
        }

        public override bool CompareForEquality(IValue left, IValue right)
        {
            return CompareDoubleForEquality(getValue(left), getValue(right));
        }

        /// <summary>
        ///     Compares two double values for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool CompareDoubleForEquality(double left, double right)
        {
            bool retVal = false;
            const double EPSILON = 0.000000001;

            retVal = Math.Abs(left - right) < EPSILON;

            return retVal;
        }

        public override bool Less(IValue left, IValue right) // left < right
        {
            bool retVal = false;

            double double1 = getValue(left);
            double double2 = getValue(right);
            retVal = double1 < double2;

            return retVal;
        }

        public override bool Greater(IValue left, IValue right) // left > right
        {
            bool retVal = false;

            double double1 = getValue(left);
            double double2 = getValue(right);
            retVal = double1 > double2;

            return retVal;
        }

        /// <summary>
        ///     Combines two types to create a new one
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public override Type CombineType(Type right, BinaryExpression.OPERATOR Operator)
        {
            Type retVal = null;

            if (Operator == BinaryExpression.OPERATOR.MULT)
            {
                Range range = right as Range;
                if (range != null)
                {
                    if (range.getPrecision() == acceptor.PrecisionEnum.aIntegerPrecision)
                    {
                        retVal = this;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     One can cast into a double
        /// </summary>
        public override bool CanBeCastInto
        {
            get { return true; }
        }

        /// <summary>
        ///     Converts a value into a double value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IValue convert(IValue value)
        {
            IValue retVal = new DoubleValue(this, getValue(value));

            return retVal;
        }
    }

    public class StringType : PredefinedType
    {
        public override string Name
        {
            get { return "String"; }
            set { }
        }

        public StringType(EFSSystem efsSystem)
            : base(efsSystem, "String")
        {
        }

        /// <summary>
        ///     Gets a value based on its image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public override IValue getValue(string image)
        {
            return new StringValue(this, image);
        }

        public override string Default
        {
            get { return ""; }
        }

        public override IValue DefaultValue
        {
            get { return getValue(Default); }
        }

        public override bool CompareForEquality(IValue left, IValue right)
        {
            bool retVal = false;

            StringValue val1 = left as StringValue;
            StringValue val2 = right as StringValue;

            if (val1 != null && val2 != null)
            {
                retVal = val1.Val.CompareTo(val2.Val) == 0;
            }

            return retVal;
        }
    }
}