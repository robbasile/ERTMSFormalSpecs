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
using System.Linq;
using DataDictionary.Generated;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Interpreter.ListOperators;
using DataDictionary.Interpreter.Statement;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    public class Parser : IDisposable
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
        ///     The buffer which holds the expression
        /// </summary>
        private char[] _buffer;

        private char[] Buffer
        {
            get { return _buffer; }
            set
            {
                _buffer = value;
                Index = 0;
            }
        }

        /// <summary>
        ///     The current index in the buffer
        /// </summary>
        private int Index { get; set; }

        /// <summary>
        ///     Skips white spaces, tab and new lines
        /// </summary>
        private void SkipWhiteSpaces()
        {
            while (Index < Buffer.Length && Char.IsWhiteSpace(Buffer[Index]))
            {
                Index = Index + 1;
            }
        }

        /// <summary>
        /// Indicates that partial parsing is currently being done
        /// </summary>
        private bool PartialParsing { get; set; }

        /// <summary>
        /// Indicates, in Partial parsing whether a parsing error has been found
        /// </summary>
        private bool ParsingErrorFound { get; set; }

        /// <summary>
        /// When an error is found, provides the expected input
        /// </summary>
        private string[] Expected { get; set; }

        /// <summary>
        /// Creates the parsing data according to the parameters provided and the parsing status
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="complete"></param>
        /// <returns></returns>
        private ParsingData CreateParsingData(int start, int end, bool complete = true)
        {
            ParsingData retVal = new ParsingData(start, end, complete && !ParsingErrorFound);

            return retVal;
        }

        /// <summary>
        /// The keywords, which should not be taken as identifiers
        /// </summary>
        private static readonly string[] Keywords = {
            "OR", "AND", "in", "not in", "is", "as", 
            "LET", 
            "STABILIZE", "INITIAL_VALUE", "STOP_CONDITION", 
            CountExpression.Operator, "USING", 
            FilterExpression.Operator, 
            FirstExpression.Operator, 
            ForAllExpression.Operator, 
            LastExpression.Operator, 
            MapExpression.Operator, 
            ReduceExpression.Operator,
            SumExpression.Operator, 
            ThereIsExpression.Operator, 
            "APPLY", "ON",
            "INSERT", "WHEN", "FULL", "REPLACE",
            "REMOVE", "FIRST", "LAST", "ALL",
            "REPLACE", "BY"
        };

        /// <summary>
        ///     Provides the identifier at the current position
        /// </summary>
        /// <returns></returns>
        private string Identifier(bool acceptDot = false)
        {
            string retVal = null;

            int start = Index;
            SkipWhiteSpaces();
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

            // Ensure that the identifier is not a keyword
            if (Keywords.Contains(retVal))
            {
                retVal = null;
                Index = start;
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
        /// <returns>null if the element at position Index is not an identifier</returns>
        private Designator Designator()
        {
            Designator retVal = null;

            SkipWhiteSpaces();
            int start = Index;
            string identifier = Identifier();
            if (identifier != null)
            {
                retVal = new Designator(Root, RootLog, identifier, CreateParsingData(start, start + identifier.Length));
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
            SkipWhiteSpaces();
            int i = 0;
            while (Index + i < Buffer.Length && i < expected.Length)
            {
                if (expected[i].CompareTo(Buffer[Index + i]) != 0)
                {
                    return false;
                }
                i = i + 1;
            }

            bool retVal = i == expected.Length;

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
        private string LookAhead(IEnumerable<string> expected)
        {
            string retVal = null;

            foreach (string value in expected)
            {
                if (LookAhead(value))
                {
                    retVal = value;
                    break;
                }
            }

            return retVal;
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
                if (PartialParsing)
                {
                    ParsingErrorFound = true;
                    Expected = new[] {expected};
                }
                else
                {
                    throw new ParseErrorException("Expecting " + expected, Index, Buffer);
                }
            }
        }

        /// <summary>
        ///     Parse part of an expression
        /// </summary>
        public delegate T ParseSubPart<out T>();

        /// <summary>
        /// Provides the expression according to the fact that it is prefixed by the prefix provided as parameter
        /// </summary>
        /// <param name="parse">The function used to part the conditional part</param>
        /// <param name="prefix">The prefix which identifies the conditional part</param>
        /// <returns></returns>
        private T ConditionalParse<T>(ParseSubPart<T> parse, params string[] prefix)
            where T : class
        {
            T retVal = null;

            string lookAhead = LookAhead(prefix);
            if (lookAhead != null)
            {
                Match(lookAhead);
                retVal = parse();
            }

            return retVal;
        }

        /// <summary>
        /// Provides the expression which must be prefixed by the prefix provided as parameter. 
        /// Otherwise, throws an exception. 
        /// This exception is not raised when doing a partial parsing 
        /// </summary>
        /// <param name="parse">The function used to part the conditional part</param>
        /// <param name="prefix">The prefix which identifies the conditional part</param>
        /// <returns></returns>
        private T MandatoryParse<T>(ParseSubPart<T> parse, params string[] prefix)
            where T : class
        {
            T retVal = null;

            string lookAhead = LookAhead(prefix);
            if (lookAhead != null)
            {
                Match(lookAhead);
                retVal = parse();
            }
            else
            {
                if (PartialParsing)
                {
                    ParsingErrorFound = true;
                    Expected = prefix;
                }
                else
                {
                    throw new ParseErrorException("Expected " + prefix, Index, Buffer);                    
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Evaluates the value of a literal
        /// </summary>
        /// <returns></returns>
        public Expression EvaluateLiteral()
        {
            Expression retVal = EvaluateString();

            if (retVal == null)
            {
                retVal = EvaluateInt();                
            }

            if (retVal == null)
            {
                retVal = EvaluateList();                
            }

            return retVal;
        }

        /// <summary>
        ///     Evaluates the current input as a string
        /// </summary>
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
                    retVal = new StringExpression(Root, RootLog, 
                        new String(Buffer, start, Index - start - 1), 
                        CreateParsingData(start - 1, Index));
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
            Type type = EfsSystem.Instance.IntegerType;

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
                type = EfsSystem.Instance.DoubleType;
                len = len + 1;
                while (Index + len < Buffer.Length && Char.IsDigit(Buffer[Index + len]))
                {
                    len = len + 1;
                }
            }

            if (Index + len < Buffer.Length && Buffer[Index + len] == 'E')
            {
                type = EfsSystem.Instance.DoubleType;
                len = len + 1;
                while (Index + len < Buffer.Length && Char.IsDigit(Buffer[Index + len]))
                {
                    len = len + 1;
                }
            }

            if (digitFound)
            {
                string str = new String(Buffer, Index, len);
                retVal = new NumberExpression(Root, RootLog, str, type, CreateParsingData(start, str.Length));
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
        /// <returns></returns>
        public ListExpression EvaluateList()
        {
            ListExpression retVal = null;

            SkipWhiteSpaces();
            int start = Index;
            if (LookAhead("["))
            {
                Match("[");
                List<Expression> list = new List<Expression>();

                if (LookAhead("]"))
                {
                    Match("]");
                    retVal = new ListExpression(Root, RootLog, list, CreateParsingData(start, Index));
                }
                else
                {
                    bool findListEntries = true;
                    while (findListEntries)
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
                            
                            if (LookAhead("]"))
                            {
                                Match("]");

                                retVal = new ListExpression(Root, RootLog, list, CreateParsingData(start, Index));
                                findListEntries = false;
                            }
                            else
                            {
                                if (PartialParsing)
                                {
                                    retVal = new ListExpression(Root, RootLog, list, CreateParsingData(start, Index, false));
                                }
                                else
                                {
                                    RootLog.AddError("] expected");                                    
                                }
                                findListEntries = false;
                            }
                        }
                        else
                        {
                            if (!PartialParsing)
                            {
                                RootLog.AddError("Cannot parse expression");                                
                            }
                            findListEntries = false;
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

            SkipWhiteSpaces();
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
                        retVal = new StructExpression(Root, RootLog, structureId, associations, CreateParsingData(start, Index));
                    }
                    else
                    {
                        while (true)
                        {
                            SkipWhiteSpaces();
                            int startId = Index;
                            string id = Identifier();
                            if (id != null)
                            {
                                Designator designator = new Designator(Root, RootLog, id, CreateParsingData(startId, startId + id.Length));
                                Expression expression = MandatoryParse(()=>Expression(0), AssignOps);
                                if (expression != null)
                                {
                                    associations[designator] = expression;
                                }
                                else
                                {
                                    RootLog.AddError("Cannot parse expression after " + id + " => ");
                                    break;
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
                            }
                            else if (LookAhead("}"))
                            {
                                Match("}");
                                retVal = new StructExpression(Root, RootLog, structureId, associations, CreateParsingData(start, Index));
                                break;
                            }
                            else
                            {
                                if (PartialParsing)
                                {
                                    retVal = new StructExpression(Root, RootLog, structureId, associations,
                                        CreateParsingData(start, Index, false));
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
            SkipWhiteSpaces();
            bool dotFound = false;
            int start = Index;
            string id = Identifier();
            while (id != null)
            {
                dotFound = false;
                Designator designator = new Designator(Root, RootLog, id, CreateParsingData(start, start + id.Length));
                Term term = new Term(Root, RootLog, designator, CreateParsingData(designator.Start, designator.End));
                UnaryExpression unaryExpression = new UnaryExpression(Root, RootLog, term, CreateParsingData(term.Start, term.End));
                derefArguments.Add(unaryExpression);

                id = null;
                if (LookAhead("."))
                {
                    Match(".");
                    SkipWhiteSpaces();
                    start = Index;
                    id = Identifier();
                    dotFound = true;
                }
            }

            if (derefArguments.Count == 1 && ! dotFound)
            {
                retVal = derefArguments[0];
            }
            else if (derefArguments.Count > 1)
            {
                retVal = new DerefExpression(Root, RootLog, derefArguments,
                    CreateParsingData(derefArguments[0].Start, derefArguments[derefArguments.Count - 1].End));
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
        /// <returns></returns>
        public Term Term()
        {
            Term retVal = null;

            Expression literalValue = EvaluateLiteral();
            if (literalValue != null)
            {
                retVal = new Term(Root, RootLog, literalValue, CreateParsingData(literalValue.Start, literalValue.End));
            }

            if (retVal == null)
            {
                Designator designator = Designator();
                if (designator != null)
                {
                    retVal = new Term(Root, RootLog, designator, CreateParsingData(designator.Start, designator.End));
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the parse tree associated to the expression stored in the buffer
        /// </summary>
        /// <param name="expressionLevel">the current level of the expression</param>
        /// <returns></returns>
        private Expression Expression(int expressionLevel)
        {
            Expression retVal = null;

            if (expressionLevel == 0 && LookAhead("LET"))
            {
                int start = Index;

                Match("LET");
                string boundVariable = Identifier();

                string assignOp = LookAhead(AssignOps);
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
                            retVal = new LetExpression(Root, RootLog, boundVariable, bindinExpression, expression,
                                CreateParsingData(start, Index));
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
                        //
                        // Binary expressions
                        //
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
                        //
                        // List operations
                        // 
                        retVal = EvaluateListExpression();
                        if (retVal == null)
                        {
                            //
                            // Unary expressions
                            //
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
        /// <returns></returns>
        private Expression ExpressionContinuation(int expressionLevel, Expression expressionLeft)
        {
            Expression retVal = expressionLeft;

            string[] operators = BinaryExpression.Images(BinaryExpression.OperatorsByLevel[expressionLevel]);
            string op = LookAhead(operators);
            if (op != null && "<".Equals(op))
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
                        CreateParsingData(expressionLeft.Start, expressionRight.End)); // {op_i+1} Expression_i+1
                    retVal = ExpressionContinuation(expressionLevel, retVal); // Expression_iCont
                }
            }

            return retVal;
        }

        /// <summary>
        ///     The continuation operators
        /// </summary>
        private static readonly string[] ContinuationOperators = {".", "("};

        /// <summary>
        ///     Implements the dot continuation or the function call continuation
        /// </summary>
        /// <param name="expressionLeft">the left part of the current expression</param>
        /// <returns></returns>
        private Expression Continuation(Expression expressionLeft)
        {
            Expression current = expressionLeft;
            int first = Index;

            List<Expression> derefArguments = new List<Expression>();
            while (!Utils.Util.isEmpty(LookAhead(ContinuationOperators)))
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
                    current = new DerefExpression(Root, RootLog, tmp,
                        CreateParsingData(expressionLeft.Start, tmp[tmp.Count - 1].End));
                }

                while (LookAhead("("))
                {
                    current = EvaluateFunctionCallExpression(current);
                }
            }

            if (derefArguments.Count > 0)
            {
                derefArguments.Add(current);
                current = new DerefExpression(Root, RootLog, derefArguments,
                    CreateParsingData(derefArguments[0].Start, derefArguments[derefArguments.Count - 1].End));
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

            SkipWhiteSpaces();
            if (LookAhead("("))
            {
                retVal = new Call(Root, RootLog, left, CreateParsingData(left.Start, -1));
                Match("(");
                bool cont = true;
                while (cont)
                {
                    SkipWhiteSpaces();
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
                            string assignOp = LookAhead(AssignOps);
                            if (assignOp != null)
                            {
                                Match(assignOp);
                                parameter = new Designator(Root, RootLog, id, CreateParsingData(current2, current2 + id.Length));
                            }
                            else
                            {
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
                            if (PartialParsing)
                            {
                                retVal.ParsingData.CompletelyParsed = false;
                                cont = false;
                            }
                            else
                            {
                                throw new ParseErrorException("Syntax error", Index, Buffer);                                
                            }
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

            SkipWhiteSpaces();
            int start = Index;
            string listOp = LookAhead(ListOperatorExpression.ListOperators);
            if (listOp != null)
            {
                Match(listOp);

                if (MapExpression.Operator.Equals(listOp)
                    || ReduceExpression.Operator.Equals(listOp)
                    || SumExpression.Operator.Equals(listOp)
                    || FilterExpression.Operator.Equals(listOp))
                {
                    Expression listExpression = Expression(0);
                    Expression condition = ConditionalParse(() => Expression(0), "|");
                    string iteratorIdentifier = MandatoryParse(() => Identifier(), "USING");

                    if (FilterExpression.Operator.Equals(listOp))
                    {
                        retVal = new FilterExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                            CreateParsingData(start, Index));
                    }
                    else
                    {
                        Expression iteratorExpression = MandatoryParse(() => Expression(0), "IN");
                        if (MapExpression.Operator.Equals(listOp))
                        {
                            retVal = new MapExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                iteratorExpression, CreateParsingData(start, Index));
                        }
                        else if (SumExpression.Operator.Equals(listOp))
                        {
                            retVal = new SumExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                iteratorExpression, CreateParsingData(start, Index));
                        }
                        else if (ReduceExpression.Operator.Equals(listOp))
                        {
                            Expression initialValue = MandatoryParse(() => Expression(0), "INITIAL_VALUE");
                            if (initialValue != null)
                            {
                                retVal = new ReduceExpression(Root, RootLog, listExpression, iteratorIdentifier,
                                    condition, iteratorExpression, initialValue, CreateParsingData(start, Index));
                            }
                        }
                    }
                }
                else
                {
                    string iteratorIdentifier = Identifier();
                    Expression listExpression = MandatoryParse(() => Expression(6), "IN");
                    if (listExpression != null)
                    {
                        Expression condition = ConditionalParse(() => Expression(0), "|");

                        // Create the right class for this list operation
                        if (ThereIsExpression.Operator.Equals(listOp))
                        {
                            retVal = new ThereIsExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                CreateParsingData(start, Index));
                        }
                        else if (ForAllExpression.Operator.Equals(listOp))
                        {
                            retVal = new ForAllExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                CreateParsingData(start, Index));
                        }
                        else if (FirstExpression.Operator.Equals(listOp))
                        {
                            retVal = new FirstExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                CreateParsingData(start, Index));
                        }
                        else if (LastExpression.Operator.Equals(listOp))
                        {
                            retVal = new LastExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                CreateParsingData(start, Index));
                        }
                        else if (CountExpression.Operator.Equals(listOp))
                        {
                            retVal = new CountExpression(Root, RootLog, listExpression, iteratorIdentifier, condition,
                                CreateParsingData(start, Index));
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
        /// <returns></returns>
        private Expression EvaluateUnaryExpression()
        {
            Expression retVal;

            SkipWhiteSpaces();
            int start = Index;
            string unaryOp = LookAhead(UnaryExpression.UnaryOperators);
            if (unaryOp != null)
            {
                Match(unaryOp);
                Expression expression = Expression(6);
                retVal = new UnaryExpression(Root, RootLog, expression, unaryOp, CreateParsingData(start, Index));
            }
            else
            {
                if (LookAhead("STABILIZE"))
                {
                    Expression expression = MandatoryParse(() => Expression(0), "STABILIZE");
                    Expression initialValue = MandatoryParse(() => Expression(0), "INITIAL_VALUE");
                    Expression condition = MandatoryParse(() => Expression(0), "STOP_CONDITION");

                    retVal = new StabilizeExpression(Root, RootLog, expression, initialValue, condition,
                        CreateParsingData(start, Index));
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
                            retVal = new UnaryExpression(Root, RootLog, term, CreateParsingData(start, Index));
                        }
                        else if (LookAhead("("))
                        {
                            Match("(");
                            retVal = new UnaryExpression(Root, RootLog, Expression(0), null, CreateParsingData(start, -1));
                            Match(")");
                            retVal.End = Index;

                            retVal = Continuation(retVal);
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

            SkipWhiteSpaces();
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
                        SkipWhiteSpaces();
                        Match(":");
                        SkipWhiteSpaces();
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

                SkipWhiteSpaces();
                string assignOp = LookAhead(AssignOps);
                if (assignOp != null)
                {
                    Match(assignOp);
                    Expression expression = Expression(0);
                    if (expression != null)
                    {
                        retVal = new FunctionExpression(Root, RootLog, parameters, expression,
                            CreateParsingData(start, Index));
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
        ///     Provides the parse tree according to the expression provided
        /// </summary>
        /// <param name="root">the element for which this expression should be parsed</param>
        /// <param name="expression">the expression to parse</param>
        /// <param name="filter">The filter to apply when performing the semantic analysis</param>
        /// <param name="doSemanticalAnalysis">true indicates that the semantical analysis should be performed</param>
        /// <param name="log">the element on which errors should be raised. By default, this is root</param>
        /// <param name="silent">Indicates whether errors should be reported (silent = false) or not</param>
        /// <param name="partial">Indicates that the expression can be partially matched</param>
        /// <returns></returns>
        public Expression Expression(ModelElement root, string expression, BaseFilter filter = null,
            bool doSemanticalAnalysis = true, ModelElement log = null, bool silent = false, bool partial = false)
        {
            Expression retVal = null;

            ModelElement.DontRaiseError(silent, () =>
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
                    PartialParsing = partial;
                    retVal = Expression(0);

                    if (!PartialParsing)
                    {
                        SkipWhiteSpaces();
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
            });

            return retVal;
        }

        /// <summary>
        ///     The assignment operators
        /// </summary>
        private static readonly string[] AssignOps = {"<-", "=>"};

        /// <summary>
        ///     Parses a statement
        /// </summary>
        /// <returns></returns>
        private Statement.Statement InnerParseStatement()
        {
            Statement.Statement retVal = null;

            int start = Index;
            if (LookAhead("APPLY"))
            {
                Match("APPLY");
                Statement.Statement appliedStatement = InnerParseStatement();
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
                    retVal = new ApplyStatement(Root, RootLog, appliedStatement, listExpression, condition,
                        CreateParsingData(start, Index));
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
                    retVal = new InsertStatement(Root, RootLog, value, list, replaceElement, 
                        CreateParsingData(start, Index));
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
                retVal = new RemoveStatement(Root, RootLog, condition, position, list, 
                    CreateParsingData(start, Index));
            }
            else if (LookAhead("REPLACE"))
            {
                Match("REPLACE");
                Expression condition = Expression(0);
                Match("IN");
                Expression list = Expression(0);
                Match("BY");
                Expression value = Expression(0);

                retVal = new ReplaceStatement(Root, RootLog, value, list, condition, 
                    CreateParsingData(start, Index));
            }
            else
            {
                Expression expression = Expression(0);
                if (expression != null)
                {
                    string assignOp = LookAhead(AssignOps);
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
                            retVal = new VariableUpdateStatement(Root, RootLog, expression, expression2, 
                                CreateParsingData(start, Index));
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
                            retVal = new ProcedureCallStatement(Root, RootLog, call, CreateParsingData(start, Index));
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
                // Setup context
                Root = root;
                RootLog = log;
                if (RootLog == null)
                {
                    RootLog = Root;
                }

                retVal = InnerParseStatement();
            }
            catch (Exception e)
            {
                Root.AddException(e);
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the parse tree according to the statement provided
        /// </summary>
        /// <param name="root">the element for which this statemennt should be parsed</param>
        /// <param name="expression"></param>
        /// <param name="silent">Indicates whether errors should be reported (silent == false) or not</param>
        /// <param name="partial">Indicates that partial partial is currently being done</param>
        /// <returns></returns>
        public Statement.Statement Statement(ModelElement root, string expression, bool silent = false, bool partial = false)
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
                        PartialParsing = partial;
                        retVal = Statement(root);

                        if (!PartialParsing)
                        {
                            SkipWhiteSpaces();
                            if (Index != Buffer.Length)
                            {
                                if (Index < Buffer.Length)
                                {
                                    throw new ParseErrorException("End of statement expected", Index, Buffer);
                                }
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
            Root = root;
            Buffer = expression.ToCharArray();

            return Term();
        }

        /// <summary>
        /// Implements IDisposable
        /// </summary>
        public void Dispose()
        {
        }
    }
}