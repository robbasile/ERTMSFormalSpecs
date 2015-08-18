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
using System.Threading;
using DataDictionary.Generated;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Interpreter.ListOperators;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Values;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    public class Parser
    {
        /// <summary>
        ///     The root element for which this expression is built and interpreted
        /// </summary>
        public ModelElement Root { get; private set; }

        /// <summary>
        ///     The element for logs should be done
        /// </summary>
        public ModelElement RootLog { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="efsSystem">The system in which the parser is built</param>
        public Parser(EFSSystem efsSystem)
        {
            EFSSystem = efsSystem;
            NoReentrance = new Mutex(false, "Parser mutex");
        }

        /// <summary>
        ///     The EFSSystem for which the parser is built
        /// </summary>
        public EFSSystem EFSSystem { get; private set; }

        /// <summary>
        ///     The buffer which holds the expression
        /// </summary>
        private char[] buffer;

        private char[] Buffer
        {
            get { return buffer; }
            set
            {
                buffer = value;
                Index = 0;
            }
        }

        /// <summary>
        ///     The current index in the buffer
        /// </summary>
        private int index;

        private int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        ///     Skips white spaces, tab and new lines
        /// </summary>
        private void skipWhiteSpaces()
        {
            while (Index < Buffer.Length && Char.IsWhiteSpace(Buffer[index]))
            {
                Index = Index + 1;
            }
        }

        /// <summary>
        ///     Provides the identifier at the current position
        /// </summary>
        /// <returns></returns>
        private string Identifier(bool acceptDot = false)
        {
            string retVal = null;

            skipWhiteSpaces();
            if (Index < Buffer.Length)
            {
                if (Char.IsLetter(Buffer[Index]) || Buffer[Index] == '_' || Buffer[Index] == '%')
                {
                    int i = 1;

                    while (Index + i < Buffer.Length && (Char.IsLetterOrDigit(Buffer[Index + i]) ||
                                                         Buffer[Index + i] == '_' ||
                                                         (acceptDot && Buffer[Index + i] == '.')))
                    {
                        i = i + 1;
                    }

                    retVal = new String(Buffer, Index, i);
                    Index = Index + i;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Indicates if the string provided corresponds to an identifier
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsIdentifier(string value)
        {
            Buffer = value.ToCharArray();
            Index = 0;

            string id = Identifier();

            return Index == Buffer.Length && !string.IsNullOrEmpty(id);
        }

        /// <summary>
        ///     Provides the designator at position Index of the Buffer.
        /// </summary>
        /// <param name="root">The root element for which this designator is built</param>
        /// <returns>null if the element at position Index is not an identifier</returns>
        private Designator Designator()
        {
            Designator retVal = null;

            skipWhiteSpaces();
            int start = Index;
            string identifier = Identifier();
            if (identifier != null)
            {
                retVal = new Designator(Root, RootLog, identifier, start, start + identifier.Length);
            }

            return retVal;
        }

        /// <summary>
        ///     Ensures that a signle string is at position index in the buffer
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <returns></returns>
        private bool LookAhead(string expected)
        {
            bool retVal = false;

            skipWhiteSpaces();
            int i = 0;
            while (Index + i < Buffer.Length && i < expected.Length)
            {
                if (expected[i].CompareTo(Buffer[Index + i]) != 0)
                {
                    return false;
                }
                i = i + 1;
            }
            retVal = i == expected.Length;

            char lastChar = expected[expected.Length - 1];
            if (retVal && (Char.IsLetterOrDigit(lastChar) || '_'.Equals(lastChar)))
            {
                // Ensure that the next character is not an identifier constituent
                // (=> is a separator)
                if (i < Buffer.Length)
                {
                    if (Char.IsLetterOrDigit(Buffer[Index + i]) || Buffer[Index + i] == '_')
                    {
                        retVal = false;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Ensures that one of the expected strings is at position index in the buffer
        /// </summary>
        /// <param name="expected">The expected string</param>
        /// <returns></returns>
        private string LookAhead(string[] expected)
        {
            foreach (string value in expected)
            {
                if (LookAhead(value))
                {
                    return value;
                }
            }
            return null;
        }

        /// <summary>
        ///     Matches a single string in the buffer
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        private void Match(string expected)
        {
            if (LookAhead(expected))
            {
                Index = Index + expected.Length;
            }
            else
            {
                throw new ParseErrorException("Expecting " + expected, Index, Buffer);
            }
        }

        /// <summary>
        ///     Evaluates the value of a literal
        /// </summary>
        /// <param name="designator">The designator currently parsed, if any</param>
        /// <param name="root">The root element for which this literal is built</param>
        /// <returns></returns>
        public Expression EvaluateLiteral()
        {
            Expression retVal = null;

            retVal = EvaluateString();
            if (retVal != null)
            {
                return retVal;
            }

            retVal = EvaluateInt();
            if (retVal != null)
            {
                return retVal;
            }

            retVal = EvaluateList();
            if (retVal != null)
            {
                return retVal;
            }

            return retVal;
        }

        /// <summary>
        ///     Evaluates the current input as a string
        /// </summary>
        /// <param name="root">The root element for which this string is built</param>
        /// <returns></returns>
        public StringExpression EvaluateString()
        {
            StringExpression retVal = null;
            int backup = Index;

            if (LookAhead("'"))
            {
                Match("'");
                int start = Index;
                while (!LookAhead("'") && Index < Buffer.Length)
                {
                    Index = Index + 1;
                }

                if (LookAhead("'"))
                {
                    Match("'");
                    retVal = new StringExpression(Root, RootLog, new String(Buffer, start, Index - start - 1), start - 1,
                        Index);
                }
                else
                {
                    Index = backup;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Evaluates the current input as a integer
        /// </summary>
        /// <returns></returns>
        public NumberExpression EvaluateInt()
        {
            NumberExpression retVal = null;

            int start = Index;

            int len = 0;
            bool digitFound = false;
            Type type = EFSSystem.IntegerType;

            if (Index < Buffer.Length && Buffer[Index] == '-')
            {
                len += 1;
            }
            while (Index + len < Buffer.Length && Char.IsDigit(Buffer[Index + len]))
            {
                digitFound = true;
                len = len + 1;
            }

            if (len > 0 && Index + len < Buffer.Length && Buffer[Index + len] == '.')
            {
                type = EFSSystem.DoubleType;
                len = len + 1;
                while (Index + len < Buffer.Length && Char.IsDigit(Buffer[Index + len]))
                {
                    len = len + 1;
                }
            }

            if (Index + len < Buffer.Length && Buffer[Index + len] == 'E')
            {
                type = EFSSystem.DoubleType;
                len = len + 1;
                while (Index + len < Buffer.Length && Char.IsDigit(Buffer[Index + len]))
                {
                    len = len + 1;
                }
            }

            if (digitFound)
            {
                string str = new String(Buffer, Index, len);
                retVal = new NumberExpression(Root, RootLog, str, type, start, str.Length);
                Index += len;
            }

            if (retVal == null)
            {
                Index = start;
            }

            return retVal;
        }

        /// <summary>
        ///     Evaluates the current input as a list
        /// </summary>
        /// <param name="root">the root element for which this list is built</param>
        /// <returns></returns>
        public ListExpression EvaluateList()
        {
            ListExpression retVal = null;

            skipWhiteSpaces();
            int start = Index;
            if (LookAhead("["))
            {
                Match("[");
                List<Expression> list = new List<Expression>();
                Type elementType = null;

                if (LookAhead("]"))
                {
                    Match("]");
                    retVal = new ListExpression(Root, RootLog, list, start, Index);
                }
                else
                {
                    bool cont = true;
                    while (cont)
                    {
                        Expression expression = Expression(0);
                        if (expression != null)
                        {
                            list.Add(expression);

                            if (LookAhead(","))
                            {
                                Match(",");
                                continue;
                            }
                            else if (LookAhead("]"))
                            {
                                Match("]");

                                retVal = new ListExpression(Root, RootLog, list, start, Index);
                                break;
                            }
                            else
                            {
                                RootLog.AddError("] expected");
                                break;
                            }
                        }
                        else
                        {
                            RootLog.AddError("Cannot parse expression");
                            break;
                        }
                    }
                }
            }

            if (retVal == null)
            {
                Index = start;
            }

            return retVal;
        }

        /// <summary>
        ///     Evaluates the current input as a structure
        /// </summary>
        /// <returns></returns>
        public Expression EvaluateStructure()
        {
            StructExpression retVal = null;

            skipWhiteSpaces();
            int start = Index;
            Expression structureId = DerefExpression();
            if (structureId != null)
            {
                if (LookAhead("{"))
                {
                    Match("{");
                    Dictionary<Designator, Expression> associations = new Dictionary<Designator, Expression>();

                    if (LookAhead("}"))
                    {
                        Match("}");
                        retVal = new StructExpression(Root, RootLog, structureId, associations, start, Index);
                    }
                    else
                    {
                        while (true)
                        {
                            skipWhiteSpaces();
                            int startId = Index;
                            string id = Identifier();
                            if (id != null)
                            {
                                Designator designator = new Designator(Root, RootLog, id, startId, startId + id.Length);
                                string assignOp = LookAhead(ASSIGN_OPS);
                                if (assignOp != null)
                                {
                                    Match(assignOp);
                                    Expression expression = Expression(0);
                                    if (expression != null)
                                    {
                                        associations[designator] = expression;
                                    }
                                    else
                                    {
                                        RootLog.AddError("Cannot parse expression after " + id + " " + assignOp + " ");
                                        break;
                                    }
                                }
                                else
                                {
                                    throw new ParseErrorException("<- or => expected", Index, Buffer);
                                }
                            }
                            else
                            {
                                if (Index < Buffer.Length)
                                {
                                    RootLog.AddError("Identifier expected, but found " + Buffer[Index]);
                                }
                                else
                                {
                                    RootLog.AddError("Identifier expected, but EOF found ");
                                }
                                break;
                            }
                            if (LookAhead(","))
                            {
                                Match(",");
                                continue;
                            }
                            else if (LookAhead("}"))
                            {
                                Match("}");
                                retVal = new StructExpression(Root, RootLog, structureId, associations, start, Index);
                                break;
                            }
                            else
                            {
                                if (Index < Buffer.Length)
                                {
                                    RootLog.AddError(", or } expected, but found " + Buffer[Index]);
                                }
                                else
                                {
                                    RootLog.AddError(", or } expected, but EOF found ");
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (retVal == null)
            {
                Index = start;
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a redef expression based on the input of the parser
        /// </summary>
        /// <returns></returns>
        private Expression DerefExpression()
        {
            Expression retVal = null;

            List<Expression> derefArguments = new List<Expression>();
            skipWhiteSpaces();
            int start = Index;
            string id = Identifier();
            while (id != null)
            {
                Designator designator = new Designator(Root, RootLog, id, start, start + id.Length);
                Term term = new Term(Root, RootLog, designator, designator.Start, designator.End);
                UnaryExpression unaryExpression = new UnaryExpression(Root, RootLog, term, term.Start, term.End);
                derefArguments.Add(unaryExpression);

                id = null;
                if (LookAhead("."))
                {
                    Match(".");
                    skipWhiteSpaces();
                    start = Index;
                    id = Identifier();
                }
            }

            if (derefArguments.Count == 1)
            {
                retVal = derefArguments[0];
            }
            else if (derefArguments.Count > 1)
            {
                retVal = new DerefExpression(Root, RootLog, derefArguments, derefArguments[0].Start,
                    derefArguments[derefArguments.Count - 1].End);
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a redef expression based on the input of the parser
        /// </summary>
        /// <returns></returns>
        public Expression DerefExpression(ModelElement root, string expression)
        {
            Buffer = expression.ToCharArray();
            Root = root;

            return DerefExpression();
        }

        /// <summary>
        ///     Provides the Term at position Index of the Buffer.
        /// </summary>
        /// <param name="root">The root element for which this term is built</param>
        /// <returns></returns>
        public Term Term()
        {
            Term retVal = null;

            Expression literalValue = EvaluateLiteral();
            if (literalValue != null)
            {
                retVal = new Term(Root, RootLog, literalValue, literalValue.Start, literalValue.End);
            }

            if (retVal == null)
            {
                Designator designator = Designator();
                if (designator != null)
                {
                    retVal = new Term(Root, RootLog, designator, designator.Start, designator.End);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Finds a value in a list of values
        /// </summary>
        /// <param name="val">The value to find</param>
        /// <param name="list">The list of values to evaluate</param>
        /// <returns></returns>
        private bool FindInList(IValue val, ListValue list)
        {
            bool retVal = false;

            foreach (Value value in list.Val)
            {
                if (val == value)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        private const int DOT_CONTINUATION = 7;

        /// <summary>
        ///     Provides the parse tree associated to the expression stored in the buffer
        /// </summary>
        /// <param name="expressionLevel">the current level of the expression</param>
        /// <param name="root">the root element for which this expression should be parsed</param>
        /// <returns></returns>
        private Expression Expression(int expressionLevel)
        {
            Expression retVal = null;

            if (expressionLevel == 0 && LookAhead("LET"))
            {
                int start = Index;

                Match("LET");
                string boundVariable = Identifier();

                string assignOp = LookAhead(ASSIGN_OPS);
                if (assignOp != null)
                {
                    Match(assignOp);
                    Expression bindinExpression = Expression(0);
                    if (bindinExpression != null)
                    {
                        Match("IN");
                        Expression expression = Expression(0);
                        if (expression != null)
                        {
                            retVal = new LetExpression(Root, RootLog, boundVariable, bindinExpression, expression, start,
                                Index);
                        }
                        else
                        {
                            RootLog.AddError("Cannot parse expression after IN keyword");
                        }
                    }
                    else
                    {
                        RootLog.AddError("Cannot parse expression after " + boundVariable + " " + assignOp + " ");
                    }
                }
                else
                {
                    RootLog.AddError("<- or => expected after " + boundVariable);
                }
            }
            else
            {
                switch (expressionLevel)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        ///
                        /// Binary expressions
                        ///
                        retVal = Expression(expressionLevel + 1);
                        if (retVal != null)
                        {
                            retVal = ExpressionContinuation(expressionLevel, retVal);
                        }
                        break;

                    case 6:
                        // Continuation, either by . operator, or by function call
                        retVal = Continuation(Expression(expressionLevel + 1));
                        break;

                    case 7:
                        ///
                        /// List operations
                        /// 
                        retVal = EvaluateListExpression();
                        if (retVal == null)
                        {
                            ////
                            /// Unary expressions
                            /// 
                            retVal = EvaluateUnaryExpression();
                        }
                        break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Implements the following grammar rules
        ///     Expression_iCont -> {op_i+1} Expression_i+1 Expression_iCont
        ///     Expression_iCont -> Epsilon
        /// </summary>
        /// <param name="expressionLevel">the current level of the expression</param>
        /// <param name="expressionLeft">the left part of the current expression</param>
        /// <param name="enclosing">the root element for which this expression should be parsed</param>
        /// <returns></returns>
        private Expression ExpressionContinuation(int expressionLevel, Expression expressionLeft)
        {
            Expression retVal = expressionLeft;

            string[] operators = BinaryExpression.Images(BinaryExpression.OperatorsByLevel[expressionLevel]);
            string op = LookAhead(operators);
            if (op != null && op.CompareTo("<") == 0)
            {
                // Avoid <- to be confused with < -1
                if (Index < Buffer.Length - 1 && Buffer[Index + 1] == '-')
                {
                    op = null;
                }
            }

            if (op != null) // Expression_iCont -> {op_i+1} Expression_i+1 Expression_iCont
            {
                Match(op);
                BinaryExpression.Operator oper = BinaryExpression.FindOperatorByName(op);
                Expression expressionRight = Expression(expressionLevel + 1);
                if (expressionRight != null)
                {
                    retVal = new BinaryExpression(Root, RootLog, expressionLeft, oper, expressionRight,
                        expressionLeft.Start, expressionRight.End); // {op_i+1} Expression_i+1
                    retVal = ExpressionContinuation(expressionLevel, retVal); // Expression_iCont
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The continuation operators
        /// </summary>
        private static string[] CONTINUATION_OPERATORS = new string[] {".", "("};

        /// <summary>
        ///     Implements the dot continuation or the function call continuation
        /// </summary>
        /// <param name="expressionLevel">the level of the right expression</param>
        /// <param name="expressionLeft">the left part of the current expression</param>
        /// <returns></returns>
        private Expression Continuation(Expression expressionLeft)
        {
            Expression current = expressionLeft;
            int first = Index;

            List<Expression> derefArguments = new List<Expression>();
            while (!Utils.Util.isEmpty(LookAhead(CONTINUATION_OPERATORS)))
            {
                List<Expression> tmp = new List<Expression>();
                while (LookAhead("."))
                {
                    if (current != null)
                    {
                        tmp.Add(current);
                    }
                    else
                    {
                        string invalidDeref = expressionLeft + (new String(Buffer).Substring(first, Index - first));
                        RootLog.AddWarning("Invalid deref expression for [" + invalidDeref +
                                           "] skipping empty dereference");
                    }
                    Match(".");
                    current = Expression(7);
                }
                if (tmp.Count > 0)
                {
                    if (current != null)
                    {
                        tmp.Add(current);
                    }
                    else
                    {
                        string invalidDeref = expressionLeft + (new String(Buffer).Substring(first, Index - first));
                        RootLog.AddWarning("Invalid deref expression for [" + invalidDeref +
                                           "] skipping empty dereference");
                    }
                    current = new DerefExpression(Root, RootLog, tmp, expressionLeft.Start, tmp[tmp.Count - 1].End);
                }

                while (LookAhead("("))
                {
                    current = EvaluateFunctionCallExpression(current);
                }
            }

            if (derefArguments.Count > 0)
            {
                derefArguments.Add(current);
                current = new DerefExpression(Root, RootLog, derefArguments, derefArguments[0].Start,
                    derefArguments[derefArguments.Count - 1].End);
            }

            return current;
        }

        /// <summary>
        ///     Evaluates a function call, when the left part (function identification) has been parsed
        /// </summary>
        /// <param name="left">The left part of the function call expression</param>
        /// <returns></returns>
        private Expression EvaluateFunctionCallExpression(Expression left)
        {
            Call retVal = null;
            int current = Index;

            skipWhiteSpaces();
            if (LookAhead("("))
            {
                retVal = new Call(Root, RootLog, left, left.Start, -1);
                Match("(");
                bool cont = true;
                while (cont)
                {
                    skipWhiteSpaces();
                    if (LookAhead(")"))
                    {
                        Match(")");
                        cont = false;
                    }
                    else
                    {
                        // Handle named parameters
                        int current2 = Index;
                        string id = Identifier();
                        Designator parameter = null;
                        if (id != null)
                        {
                            string assignOp = LookAhead(ASSIGN_OPS);
                            if (assignOp != null)
                            {
                                Match(assignOp);
                                parameter = new Designator(Root, RootLog, id, current2, current2 + id.Length);
                            }
                            else
                            {
                                id = null;
                                Index = current2;
                            }
                        }

                        Expression arg = Expression(0);
                        if (arg != null)
                        {
                            retVal.AddActualParameter(parameter, arg);
                            if (LookAhead(","))
                            {
                                Match(",");
                            }
                            else if (LookAhead(")"))
                            {
                                Match(")");
                                cont = false;
                            }
                        }
                        else
                        {
                            throw new ParseErrorException("Syntax error", Index, Buffer);
                        }
                    }
                }
                retVal.End = Index;
            }

            return retVal;
        }

        /// <summary>
        ///     Evaluates a list expression
        /// </summary>
        /// <returns></returns>
        private Expression EvaluateListExpression()
        {
            Expression retVal = null;

            skipWhiteSpaces();
            int start = Index;
            string listOp = LookAhead(ListOperatorExpression.LIST_OPERATORS);
            if (listOp != null)
            {
                Match(listOp);

                if (listOp.CompareTo(MapExpression.Operator) == 0
                    || listOp.CompareTo(ReduceExpression.Operator) == 0
                    || listOp.CompareTo(SumExpression.Operator) == 0)
                {
                    Expression listExpression = Expression(0);
                    if (listExpression != null)
                    {
                        Expression condition = null;
                        if (LookAhead("|"))
                        {
                            Match("|");
                            condition = Expression(0);
                        }

                        Match("USING");
                        string iteratorIdentifier = Identifier();
                        Match("IN");
                        Expression iteratorExpression = Expression(0);
                        if (iteratorExpression != null)
                        {
                            if (MapExpression.Operator.CompareTo(listOp) == 0)
                            {
                                retVal = new MapExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                    iteratorExpression, start, Index);
                            }
                            else if (SumExpression.Operator.CompareTo(listOp) == 0)
                            {
                                retVal = new SumExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                    iteratorExpression, start, Index);
                            }
                            else if (ReduceExpression.Operator.CompareTo(listOp) == 0)
                            {
                                Match("INITIAL_VALUE");
                                Expression initialValue = Expression(0);
                                if (initialValue != null)
                                {
                                    retVal = new ReduceExpression(Root, RootLog, listExpression, iteratorIdentifier,
                                        condition, iteratorExpression, initialValue, start, Index);
                                }
                                else
                                {
                                    throw new ParseErrorException("REDUCE requires an initial value", Index, Buffer);
                                }
                            }
                        }
                        else
                        {
                            throw new ParseErrorException("Function designator expected", Index, Buffer);
                        }
                    }
                }
                else
                {
                    string iteratorIdentifier = Identifier();
                    Match("IN");
                    Expression listExpression = Expression(6);
                    if (listExpression != null)
                    {
                        Expression condition = null;
                        if (LookAhead("|"))
                        {
                            Match("|");
                            condition = Expression(0);
                        }

                        // Create the right class for this list operation
                        if (ThereIsExpression.Operator.CompareTo(listOp) == 0)
                        {
                            retVal = new ThereIsExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                start, Index);
                        }
                        else if (ForAllExpression.Operator.CompareTo(listOp) == 0)
                        {
                            retVal = new ForAllExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                start, Index);
                        }
                        else if (FirstExpression.Operator.CompareTo(listOp) == 0)
                        {
                            retVal = new FirstExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                start, Index);
                        }
                        else if (LastExpression.Operator.CompareTo(listOp) == 0)
                        {
                            retVal = new LastExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                start, Index);
                        }
                        else if (CountExpression.Operator.CompareTo(listOp) == 0)
                        {
                            retVal = new CountExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                start, Index);
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Evaluates a unary expression
        ///     . NOT expression
        ///     . Term
        /// </summary>
        /// <param name="enclosing"></param>
        /// <returns></returns>
        private Expression EvaluateUnaryExpression()
        {
            Expression retVal = null;

            skipWhiteSpaces();
            int start = Index;
            string unaryOp = LookAhead(UnaryExpression.UnaryOperators);
            if (unaryOp != null)
            {
                Match(unaryOp);
                Expression expression = Expression(6);
                retVal = new UnaryExpression(Root, RootLog, expression, unaryOp, start, Index);
            }
            else
            {
                if (LookAhead("STABILIZE"))
                {
                    Match("STABILIZE");
                    Expression expression = Expression(0);
                    Match("INITIAL_VALUE");
                    Expression initialValue = Expression(0);
                    Match("STOP_CONDITION");
                    Expression condition = Expression(0);

                    retVal = new StabilizeExpression(Root, RootLog, expression, initialValue, condition, start, Index);
                }
                else
                {
                    retVal = EvaluateStructure();
                    if (retVal == null)
                    {
                        retVal = EvaluateFunction();
                    }
                    if (retVal == null)
                    {
                        Term term = Term();
                        if (term != null)
                        {
                            retVal = new UnaryExpression(Root, RootLog, term, start, Index);
                        }
                        else if (LookAhead("("))
                        {
                            Match("(");
                            retVal = new UnaryExpression(Root, RootLog, Expression(0), null, start, -1);
                            Match(")");
                            retVal.End = Index;

                            if (retVal != null)
                            {
                                retVal = Continuation(retVal);
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Evaluates a function declaration expression
        /// </summary>
        /// <returns></returns>
        private FunctionExpression EvaluateFunction()
        {
            FunctionExpression retVal = null;

            skipWhiteSpaces();
            int start = Index;
            if (LookAhead("FUNCTION"))
            {
                List<Parameter> parameters = new List<Parameter>();
                bool cont = true;
                Match("FUNCTION");
                while (cont)
                {
                    string id = Identifier();
                    if (id != null)
                    {
                        skipWhiteSpaces();
                        Match(":");
                        skipWhiteSpaces();
                        string typeName = Identifier(true);
                        if (typeName != null)
                        {
                            Parameter parameter = (Parameter) acceptor.getFactory().createParameter();
                            parameter.Name = id;
                            parameter.TypeName = typeName;
                            parameters.Add(parameter);
                        }
                        else
                        {
                            throw new ParseErrorException("Parameter type expected", Index, Buffer);
                        }
                    }
                    else
                    {
                        throw new ParseErrorException("Parameter identifier expected", Index, Buffer);
                    }

                    cont = LookAhead(",");
                    if (cont)
                    {
                        Match(",");
                    }
                }

                skipWhiteSpaces();
                string assignOp = LookAhead(ASSIGN_OPS);
                if (assignOp != null)
                {
                    Match(assignOp);
                    Expression expression = Expression(0);
                    if (expression != null)
                    {
                        retVal = new FunctionExpression(Root, RootLog, parameters, expression, start, Index);
                    }
                    else
                    {
                        throw new ParseErrorException("Function expression expected", Index, Buffer);
                    }
                }
                else
                {
                    throw new ParseErrorException("=> or <- expected", Index, Buffer);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Ensures that the code is not reentrant
        /// </summary>
        private Mutex NoReentrance { get; set; }

        /// <summary>
        ///     Provides the parse tree according to the expression provided
        /// </summary>
        /// <param name="root">the element for which this expression should be parsed</param>
        /// <param name="expression">the expression to parse</param>
        /// <param name="filter">The filter to apply when performing the semantic analysis</param>
        /// <param name="doSemanticalAnalysis">true indicates that the semantical analysis should be performed</param>
        /// <param name="log">the element on which errors should be raised. By default, this is root</param>
        /// <param name="silent">Indicates whether errors should be reported (silent = false) or not</param>
        /// <returns></returns>
        public Expression Expression(ModelElement root, string expression, BaseFilter filter = null,
            bool doSemanticalAnalysis = true, ModelElement log = null, bool silent = false)
        {
            Expression retVal = null;

            NoReentrance.WaitOne();
            ModelElement.DontRaiseError(silent, () =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                Util.DontNotify(() =>
                {
                    try
                    {
                        // Setup context
                        Root = root;
                        RootLog = log;
                        if (RootLog == null)
                        {
                            RootLog = Root;
                        }

                        Buffer = expression.ToCharArray();
                        retVal = Expression(0);

                        skipWhiteSpaces();
                        if (Index != Buffer.Length)
                        {
                            retVal = null;
                            if (Index < Buffer.Length)
                            {
                                RootLog.AddError("End of expression expected, but found " + Buffer[Index]);
                            }
                            else
                            {
                                RootLog.AddError("End of expression expected, but found EOF");
                            }
                        }
                        if (retVal != null && doSemanticalAnalysis)
                        {
                            if (filter == null)
                            {
                                retVal.SemanticAnalysis(IsVariableOrValue.INSTANCE);
                            }
                            else
                            {
                                retVal.SemanticAnalysis(filter);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        root.AddException(e);
                    }
                    finally
                    {
                        NoReentrance.ReleaseMutex();
                    }
                });
            });

            return retVal;
        }

        /// <summary>
        ///     The assignment operators
        /// </summary>
        private static string[] ASSIGN_OPS = new string[] {"<-", "=>"};

        /// <summary>
        ///     Parses a statement
        /// </summary>
        /// <returns></returns>
        private Statement.Statement innerParseStatement()
        {
            Statement.Statement retVal = null;

            int start = Index;
            if (LookAhead("APPLY"))
            {
                Match("APPLY");
                int startCall = Index;
                Statement.Statement appliedStatement = innerParseStatement();
                if (appliedStatement != null)
                {
                    Match("ON");
                    Expression listExpression = Expression(0);
                    Expression condition = null;
                    if (LookAhead("|"))
                    {
                        Match("|");
                        condition = Expression(0);
                    }
                    retVal = new ApplyStatement(Root, RootLog, appliedStatement, listExpression, condition, start, Index);
                }
                else
                {
                    RootLog.AddError("Cannot parse call expression");
                }
            }
            else if (LookAhead("INSERT"))
            {
                Match("INSERT");
                Expression value = Expression(0);
                if (value != null)
                {
                    Match("IN");
                    Expression list = Expression(0);
                    Expression replaceElement = null;
                    if (LookAhead("WHEN"))
                    {
                        Match("WHEN");
                        Match("FULL");
                        Match("REPLACE");

                        replaceElement = Expression(0);
                    }
                    retVal = new InsertStatement(Root, RootLog, value, list, replaceElement, start, Index);
                }
            }
            else if (LookAhead("REMOVE"))
            {
                Match("REMOVE");

                RemoveStatement.PositionEnum position = RemoveStatement.PositionEnum.First;
                if (LookAhead("FIRST"))
                {
                    Match("FIRST");
                }
                else if (LookAhead("LAST"))
                {
                    Match("LAST");
                    position = RemoveStatement.PositionEnum.Last;
                }
                else if (LookAhead("ALL"))
                {
                    Match("ALL");
                    position = RemoveStatement.PositionEnum.All;
                }

                Expression condition = null;
                if (!LookAhead("IN"))
                {
                    condition = Expression(0);
                }
                Match("IN");
                Expression list = Expression(0);
                retVal = new RemoveStatement(Root, RootLog, condition, position, list, start, Index);
            }
            else if (LookAhead("REPLACE"))
            {
                Match("REPLACE");
                Expression condition = Expression(0);
                Match("IN");
                Expression list = Expression(0);
                Match("BY");
                Expression value = Expression(0);

                retVal = new ReplaceStatement(Root, RootLog, value, list, condition, start, Index);
            }
            else
            {
                Expression expression = Expression(0);
                if (expression != null)
                {
                    string assignOp = LookAhead(ASSIGN_OPS);
                    if (assignOp != null)
                    {
                        // This is a variable update
                        Match(assignOp);
                        if (LookAhead("%"))
                        {
                            Match("%");
                        }
                        Expression expression2 = Expression(0);

                        if (expression2 != null)
                        {
                            retVal = new VariableUpdateStatement(Root, RootLog, expression, expression2, start, Index);
                        }
                        else
                        {
                            RootLog.AddError("Invalid <- right side");
                        }
                        expression.Enclosing = retVal;
                    }
                    else
                    {
                        // This is a procedure call
                        Call call = expression as Call;
                        if (call != null)
                        {
                            retVal = new ProcedureCallStatement(Root, RootLog, call, start, Index);
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the Term at position Index of the Buffer.
        /// </summary>
        /// <param name="root">The root element for which this term is built</param>
        /// <param name="log">The element on which error messages should be done. By default, this is root</param>
        /// <returns></returns>
        private Statement.Statement Statement(ModelElement root, ModelElement log = null)
        {
            Statement.Statement retVal = null;

            try
            {
                NoReentrance.WaitOne();

                // Setup context
                Root = root;
                RootLog = log;
                if (RootLog == null)
                {
                    RootLog = Root;
                }

                retVal = innerParseStatement();
            }
            catch (Exception e)
            {
                Root.AddException(e);
            }
            finally
            {
                NoReentrance.ReleaseMutex();
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the parse tree according to the statement provided
        /// </summary>
        /// <param name="root">the element for which this statemennt should be parsed</param>
        /// <param name="expression"></param>
        /// <param name="silent">Indicates whether errors should be reported (silent == false) or not</param>
        /// <returns></returns>
        public Statement.Statement Statement(ModelElement root, string expression, bool silent = false)
        {
            Statement.Statement retVal = null;

            Util.DontNotify(() =>
            {
                // ReSharper disable once ConvertToLambdaExpression
                ModelElement.DontRaiseError(() =>
                {
                    try
                    {
                        Root = root;
                        Buffer = expression.ToCharArray();
                        retVal = Statement(root);

                        skipWhiteSpaces();
                        if (Index != Buffer.Length)
                        {
                            if (Index < Buffer.Length)
                            {
                                throw new ParseErrorException("End of statement expected", Index, Buffer);
                            }
                        }

                        if (retVal != null)
                        {
                            retVal.SemanticAnalysis();
                        }
                    }
                    catch (Exception exception)
                    {
                        root.AddException(exception);
                    }
                });
            });

            return retVal;
        }


        /// <summary>
        ///     Evaluates a term based on a string representation of that term
        /// </summary>
        /// <param name="root"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal Term Term(ModelElement root, string expression)
        {
            Term retVal = null;

            Root = root;
            Buffer = expression.ToCharArray();
            retVal = Term();

            return retVal;
        }
    }
}