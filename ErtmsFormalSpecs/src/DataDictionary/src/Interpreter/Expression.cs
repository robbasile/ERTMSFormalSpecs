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
using DataDictionary.Functions;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Type = DataDictionary.Types.Type;

namespace DataDictionary.Interpreter
{
    /// <summary>
    ///     Stores the association between a interpreter tree node and a value
    /// </summary>
    public class ReturnValueElement : IComparable<ReturnValueElement>
    {
        /// <summary>
        ///     The previous return value element in the
        /// </summary>
        public ReturnValueElement PreviousElement { get; set; }

        /// <summary>
        ///     The value
        /// </summary>
        public INamable Value { get; set; }

        /// <summary>
        ///     Indicates that the return value element was found as a type of its instance, instead of the instance itself.
        ///     This allow to differenciate between a structure and  the return value of a function of type structure
        ///     (in the latter case, the specific element cannot be statically identified in the model)
        /// </summary>
        public bool AsType { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="previous"></param>
        /// <param name="asType"></param>
        public ReturnValueElement(INamable value, ReturnValueElement previous = null, bool asType = false)
        {
            PreviousElement = previous;
            Value = value;
            AsType = asType;
        }

        // Summary:
        //     Compares the current object with another object of the same type.
        //
        // Parameters:
        //   other:
        //     An object to compare with this object.
        //
        // Returns:
        //     A value that indicates the relative order of the objects being compared.
        //     The return value has the following meanings: Value Meaning Less than zero
        //     This object is less than the other parameter.Zero This object is equal to
        //     other. Greater than zero This object is greater than other.
        public int CompareTo(ReturnValueElement other)
        {
            int retVal = 1;

            if (other != null)
            {
                if (Value == other.Value && AsType == other.AsType)
                {
                    // This seem to be the same return value
                    retVal = 0;

                    // Ensure that previous elements match.
                    if (PreviousElement != null)
                    {
                        retVal = PreviousElement.CompareTo(other.PreviousElement);
                    }
                    else
                    {
                        if (other.PreviousElement != null)
                        {
                            retVal = -1;
                        }
                    }
                }
            }

            return retVal;
        }

        public override string ToString()
        {
            string retVal = Value.ToString();

            if (PreviousElement != null)
            {
                retVal += " -> " + PreviousElement.ToString();
            }

            return retVal;
        }
    }

    /// <summary>
    ///     The possible return values for InnerGetValue
    /// </summary>
    public class ReturnValue
    {
        /// <summary>
        ///     The interpreter tree node on which these values are linked
        /// </summary>
        public InterpreterTreeNode Node { get; private set; }

        /// <summary>
        ///     The values of this return value
        /// </summary>
        public List<ReturnValueElement> Values { get; private set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ReturnValue(InterpreterTreeNode node)
        {
            Node = node;
            Values = new List<ReturnValueElement>();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public ReturnValue()
        {
            Node = null;
            Values = new List<ReturnValueElement>();
        }

        /// <summary>
        ///     Indicates if there is more than one value in the result set
        /// </summary>
        public bool IsAmbiguous
        {
            get { return Values.Count > 1; }
        }

        /// <summary>
        ///     Indicates if there is only one value in the result set
        /// </summary>
        public bool IsUnique
        {
            get { return Values.Count == 1; }
        }

        /// <summary>
        ///     Indicates if there is no more value in the result set
        /// </summary>
        public bool IsEmpty
        {
            get { return Values.Count == 0; }
        }

        /// <summary>
        ///     Adds a new value in the set of return values
        /// </summary>
        /// <param name="value">The value to add</param>
        /// <param name="previous">The previous element in the chain</param>
        public void Add(INamable value, ReturnValueElement previous = null, bool asType = false)
        {
            if (value != null)
            {
                ReturnValueElement element = new ReturnValueElement(value, previous, asType);
                foreach (ReturnValueElement elem in Values)
                {
                    if (elem.CompareTo(element) == 0)
                    {
                        element = null;
                        break;
                    }
                }

                if (element != null)
                {
                    Values.Add(element);
                }
            }
        }

        /// <summary>
        ///     Merges the other return value with this one
        /// </summary>
        /// <param name="previous">The previous ReturnValueElement which lead to this ReturnValueElement</param>
        /// <param name="other">The other return value to merge with</param>
        public void Merge(ReturnValueElement previous, ReturnValue other)
        {
            foreach (ReturnValueElement elem in other.Values)
            {
                Add(elem.Value, previous, elem.AsType);
            }
        }

        public override string ToString()
        {
            string retVal = null;

            if (Values.Count > 0)
            {
                foreach (ReturnValueElement elem in Values)
                {
                    if (retVal != null)
                    {
                        retVal = retVal + ", ";
                    }
                    else
                    {
                        retVal = "";
                    }

                    retVal = retVal + elem.Value.FullName + "(" + elem.Value.GetType() + ")";
                }
            }
            else
            {
                retVal = "<nothing>";
            }

            return retVal;
        }

        /// <summary>
        ///     Filters out value according to predicate
        /// </summary>
        /// <param name="accept"></param>
        public void filter(BaseFilter accept)
        {
            RemoveUpdated();
            DiscardRemoved();

            // Only keep the most specific elements.
            string mostSpecific = null;
            foreach (ReturnValueElement element in Values)
            {
                if (accept.AcceptableChoice(element.Value))
                {
                    if (mostSpecific == null)
                    {
                        mostSpecific = element.Value.FullName;
                    }
                    else
                    {
                        if (mostSpecific.Length < element.Value.FullName.Length)
                        {
                            mostSpecific = element.Value.FullName;
                        }
                    }
                }
            }

            // if the filtering is about variables, ensure that there is at least one variable in the element chain
            if (accept is IsVariable)
            {
                List<ReturnValueElement> tmp = new List<ReturnValueElement>();
                foreach (ReturnValueElement element in Values)
                {
                    bool variableFound = false;
                    bool onlyStructureElement = true;
                    ReturnValueElement current = element;
                    while (!variableFound && current != null)
                    {
                        variableFound = IsStrictVariableOrValue.INSTANCE.AcceptableChoice(current.Value) ||
                                        current.AsType;
                        onlyStructureElement = onlyStructureElement && current.Value is StructureElement;
                        current = current.PreviousElement;
                    }

                    if (variableFound)
                    {
                        tmp.Add(element);
                    }
                    else if (onlyStructureElement)
                    {
                        tmp.Add(element);
                    }
                }

                // HaCK : If tmp is empty, this indicates that the filter above is too restrictive. 
                // Keep the original set
                if (tmp.Count > 0)
                {
                    Values = tmp;
                }
            }

            // Build a new list with the filtered out elements
            bool variable = false;
            {
                List<ReturnValueElement> tmp = new List<ReturnValueElement>();
                foreach (ReturnValueElement element in Values)
                {
                    if (accept.AcceptableChoice(element.Value) && element.Value.FullName.Equals(mostSpecific))
                    {
                        tmp.Add(element);
                        variable = variable || element.Value is IVariable;
                    }
                }
                Values = tmp;
            }

            // HaCK : If both Variable and StructureElement are found, only keep the variable
            if (variable)
            {
                List<ReturnValueElement> tmp = new List<ReturnValueElement>();
                foreach (ReturnValueElement element in Values)
                {
                    if (!(element.Value is StructureElement) && !(element.Value is Type))
                    {
                        tmp.Add(element);
                    }
                }

                Values = tmp;
            }
        }

        /// <summary>
        ///     Removes the elements that have been marked as removed from the model
        /// </summary>
        public void DiscardRemoved()
        {
            Values.RemoveAll(ValueIsRemoved);
        }

        /// <summary>
        /// Indicates that the referenced value has been removed
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool ValueIsRemoved(ReturnValueElement element)
        {
            bool retVal = false;

            ModelElement model = element.Value as ModelElement;
            if (model != null)
            {
                retVal = model.IsRemoved;
            }

            return retVal;
        }

        /// <summary>
        ///     Removes the elements that have been updated by another element
        /// </summary>
        public void RemoveUpdated()
        {
            // We collect all the values that have been redefined to filter them out of the list
            HashSet<ModelElement> redefined = new HashSet<ModelElement>();
            foreach (ReturnValueElement element in Values)
            {
                ModelElement modelElement = element.Value as ModelElement;
                if (modelElement != null && modelElement.Updates != null)
                {
                    redefined.Add(modelElement.Updates);
                }
            }

            List<ReturnValueElement> tmp = new List<ReturnValueElement>();
            foreach (ReturnValueElement element in Values)
            {
                ModelElement modelElement = element.Value as ModelElement;
                if (modelElement != null)
                {
                    if (!redefined.Contains(modelElement))
                    {
                        tmp.Add(element);
                    }
                }
                else
                {
                    tmp.Add(element);
                }
            }

            RemoveDuplicates(tmp);
            Values = tmp;
        }

        /// <summary>
        ///     According to updates, several path may lead to the same model element.
        ///     Since that model element is uniquely idenfied, keep only one instance
        /// </summary>
        /// <param name="list">The list of elements from which we remove duplicates</param>
        /// <returns>The list of individual ReturnValueElements derived from List</returns>
        private void RemoveDuplicates(List<ReturnValueElement> list)
        {
            List<ReturnValueElement> tmp = new List<ReturnValueElement>();
            if (list.Count > 0)
            {
                foreach (ReturnValueElement element in list)
                {
                    bool isPresent = false;
                    foreach (ReturnValueElement elem in tmp)
                    {
                        if (elem.CompareTo(element) == 0)
                        {
                            isPresent = true;
                        }
                    }

                    if (!isPresent)
                    {
                        tmp.Add(element);
                    }
                }
            }
            list = tmp;
        }

        /// <summary>
        ///     The empty return value
        /// </summary>
        public static ReturnValue Empty = new ReturnValue();
    }

    /// <summary>
    ///     An interpretation context
    /// </summary>
    public class InterpretationContext
    {
        /// <summary>
        ///     The instance on which the expression is checked
        /// </summary>
        public INamable Instance { get; set; }

        /// <summary>
        ///     The local scope for interpretation
        /// </summary>
        public SymbolTable LocalScope { get; private set; }

        /// <summary>
        ///     Indicates that default values should be used when no value is specifically provided
        /// </summary>
        public bool UseDefaultValue { get; set; }

        /// <summary>
        ///     Indicates that the enclosing element could cause side effects
        /// </summary>
        public bool HasSideEffects { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="instance">The instance on which interpretation should be performed</param>
        public InterpretationContext()
        {
            LocalScope = new SymbolTable();
            Instance = null;
            UseDefaultValue = true;
            HasSideEffects = false;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="instance">The instance on which interpretation should be performed</param>
        public InterpretationContext(INamable instance)
        {
            LocalScope = new SymbolTable();
            Instance = instance;
            UseDefaultValue = true;
            HasSideEffects = false;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="other">Copies the other interpretation context contents</param>
        public InterpretationContext(InterpretationContext other)
        {
            LocalScope = other.LocalScope;
            Instance = other.Instance;
            UseDefaultValue = other.UseDefaultValue;
            HasSideEffects = other.HasSideEffects;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="other">Copies the other interpretation context contents</param>
        /// <param name="instance">The evaluation instance</param>
        public InterpretationContext(InterpretationContext other, INamable instance)
        {
            LocalScope = other.LocalScope;
            Instance = instance;
            UseDefaultValue = true;
            HasSideEffects = other.HasSideEffects;
        }

        /// <summary>
        ///     Provides the list of parameters whose value is a placeholder
        /// </summary>
        /// <returns></returns>
        public List<Parameter> PlaceHolders()
        {
            return LocalScope.PlaceHolders();
        }

        /// <summary>
        ///     Provides the actual variable which corresponds to this parameter on the stack
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public IVariable findOnStack(Parameter parameter)
        {
            return LocalScope.find(parameter);
        }

        /// <summary>
        ///     Provides the current stack index
        /// </summary>
        public int StackIndex
        {
            get { return LocalScope.Index; }
        }
    }

    /// <summary>
    ///     Allows to reference a namable
    /// </summary>
    public interface IReference
    {
        /// <summary>
        ///     Provides the referenced element
        /// </summary>
        INamable Ref { get; }
    }

    /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
     * The grammar is following:                                     *
     * Expression0      -> Expression1 Expression0Cont               *
     * Expression0Cont  -> OR Expression1 Expression0Cont            *
     * Expression0Cont  -> Epsilon                                   *
     * Expression1      -> Expression2 Expression1Cont               *
     * Expression1Cont  -> AND Expression2 Expression1Cont           *
     * Expression1Cont  -> Epsilon                                   *
     * Expression2      -> Expression3 Expression2Cont               *
     * Expression2Cont  -> {+, -} Expression3 Expression2Cont        *
     * Expression2Cont  -> Epsilon                                   *
     * Expression3      -> Expression4 Expression3Cont               *
     * Expression3Cont  -> {*, /} Expression4 Expression3Cont        *
     * Expression3Cont  -> Epsilon                                   *
     * Expression4      -> Expression5 Expression4Cont               *
     * Expression4Cont  -> {^} Expression5 Expression4Cont           *
     * Expression4Cont  -> Epsilon                                   *
     * Expression5      -> Term {+, -}                               *
     * Term             -> Literal                                   *
     * Term             -> Desig                                     *
     * Term             -> Desig (arg1, ...)                         *
     * Term             -> (Expression0)                             *
     *                                                               *
     * =>                                                            *
     * Expression_i     -> Expression_i+1 Expression_iCont           *
     * Expression_iCont -> {op_i+1} Expression_i+1 Expression_iCont  *
     * Expression_iCont -> Epsilon                                   *
     * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

    public abstract class Expression : InterpreterTreeNode, IReference, ITextualExplain
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root for which this expression should be evaluated</param>
        /// <param name="log"></param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public Expression(ModelElement root, ModelElement log, int start, int end)
            : base(root, log, start, end)
        {
        }

        /// <summary>
        ///     Indicates whether the semantic analysis has been performed for this expression
        /// </summary>
        protected bool SemanticAnalysisDone { get; private set; }

        /// <summary>
        ///     Provides the possible references for this expression (only available during semantic analysis)
        /// </summary>
        /// <param name="instance">the instance on which this element should be found.</param>
        /// <param name="expectation">the expectation on the element found</param>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public virtual ReturnValue getReferences(INamable instance, BaseFilter expectation, bool last)
        {
            return ReturnValue.Empty;
        }

        /// <summary>
        ///     Provides the possible references types for this expression (used in semantic analysis)
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <param name="last">indicates that this is the last element in a dereference chain</param>
        /// <returns></returns>
        public virtual ReturnValue getReferenceTypes(INamable instance, BaseFilter expectation, bool last)
        {
            ReturnValue retVal = new ReturnValue(this);

            SemanticAnalysis(instance, AllMatches.INSTANCE);
            bool asType = true;
            retVal.Add(GetExpressionType(), null, asType);

            return retVal;
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public virtual bool SemanticAnalysis(INamable instance, BaseFilter expectation)
        {
            bool retVal = !SemanticAnalysisDone;

            if (retVal)
            {
                StaticUsage = new Usages();
                SemanticAnalysisDone = true;
            }

            return retVal;
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <param name="instance">the reference instance on which this element should analysed</param>
        /// <returns>True if semantic analysis should be continued</returns>
        public bool SemanticAnalysis(INamable instance = null)
        {
            return SemanticAnalysis(instance, AllMatches.INSTANCE);
        }

        /// <summary>
        ///     Performs the semantic analysis of the expression
        /// </summary>
        /// <paraparam name="expectation">Indicates the kind of element we are looking for</paraparam>
        /// <returns>True if semantic analysis should be continued</returns>
        public bool SemanticAnalysis(BaseFilter expectation)
        {
            return SemanticAnalysis(null, expectation);
        }

        /// <summary>
        ///     Provides the INamable which is referenced by this expression, if any
        /// </summary>
        public virtual INamable Ref
        {
            get { return null; }
            protected set { }
        }

        /// <summary>
        ///     Provides the ICallable that is statically defined
        /// </summary>
        public virtual ICallable getStaticCallable()
        {
            ICallable retVal = Ref as ICallable;

            if (retVal == null)
            {
                Type type = Ref as Type;
                if (type != null)
                {
                    retVal = type.CastFunction;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type of this expression
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <returns></returns>
        public abstract Type GetExpressionType();

        /// <summary>
        ///     Provides all the steps used to get the value of the expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ExplanationPart Explain(InterpretationContext context = null)
        {
            ExplanationPart retVal = new ExplanationPart(Root, this);

            if (context == null)
            {
                context = new InterpretationContext();
            }
            try
            {
                IValue value = GetValue(context, retVal);
            }
            catch (Exception)
            {
            }

            return retVal;
        }

        /// <summary>
        ///     Adds an error message to the root element and explains it
        /// </summary>
        /// <param name="message"></param>
        public override void AddErrorAndExplain(string message, ExplanationPart explain)
        {
            if (RootLog != null)
            {
                ExplanationPart.CreateSubExplanation(explain, message);
                RootLog.AddErrorAndExplain(message, explain);
            }
        }

        /// <summary>
        ///     Provides the variable referenced by this expression, if any
        /// </summary>
        /// <param name="context">The context on which the variable must be found</param>
        /// <returns></returns>
        public virtual IVariable GetVariable(InterpretationContext context)
        {
            return null;
        }

        /// <summary>
        ///     Provides the value associated to this Expression
        /// </summary>
        /// <param name="context">The context on which the value must be found</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public virtual IValue GetValue(InterpretationContext context, ExplanationPart explain)
        {
            return null;
        }

        /// <summary>
        ///     Provides the callable that is called by this expression
        /// </summary>
        /// <param name="context"></param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public virtual ICallable getCalled(InterpretationContext context, ExplanationPart explain)
        {
            return null;
        }

        /// <summary>
        ///     Fills the list provided with the element matching the filter provided
        /// </summary>
        /// <param name="retVal">The list to be filled with the element matching the condition expressed in the filter</param>
        /// <param name="filter">The filter to apply</param>
        public abstract void fill(List<INamable> retVal, BaseFilter filter);

        /// <summary>
        ///     Provides the right sides used by this expression
        /// </summary>
        public List<ITypedElement> GetRightSides()
        {
            List<ITypedElement> retVal = new List<ITypedElement>();

            List<INamable> tmp = new List<INamable>();
            fill(tmp, IsRightSide.INSTANCE);

            foreach (INamable namable in tmp)
            {
                ITypedElement element = namable as ITypedElement;
                if (element != null)
                {
                    retVal.Add(element);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the variables used by this expression
        /// </summary>
        public List<IVariable> GetVariables()
        {
            List<IVariable> retVal = new List<IVariable>();

            List<INamable> tmp = new List<INamable>();
            fill(tmp, IsVariable.INSTANCE);

            foreach (INamable namable in tmp)
            {
                IVariable variable = namable as IVariable;
                if (variable != null)
                {
                    retVal.Add(variable);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the list of literals found in the expression
        /// </summary>
        public List<IValue> GetLiterals()
        {
            List<IValue> retVal = new List<IValue>();

            List<INamable> tmp = new List<INamable>();
            fill(tmp, IsValue.INSTANCE);

            foreach (INamable namable in tmp)
            {
                IValue value = namable as IValue;
                if (value != null)
                {
                    retVal.Add(value);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Checks the expression and appends errors to the root tree node when inconsistencies are found
        /// </summary>
        public virtual void checkExpression()
        {
        }

        /// <summary>
        ///     Creates the graph associated to this expression, when the given parameter ranges over the X axis
        /// </summary>
        /// <param name="context">The interpretation context</param>
        /// <param name="parameter">The parameters of *the enclosing function* for which the graph should be created</param>
        /// <param name="explain"></param>
        /// <returns></returns>
        public virtual Graph createGraph(InterpretationContext context, Parameter parameter, ExplanationPart explain)
        {
            Graph retVal = null;

            return retVal;
        }

        /// <summary>
        ///     Provides the surface of this function if it has been statically defined
        /// </summary>
        /// <param name="context">the context used to create the surface</param>
        /// <param name="xParam">The X axis of this surface</param>
        /// <param name="yParam">The Y axis of this surface</param>
        /// <param name="explain"></param>
        /// <returns>The surface which corresponds to this expression</returns>
        public virtual Surface createSurface(InterpretationContext context, Parameter xParam, Parameter yParam,
            ExplanationPart explain)
        {
            Surface retVal = null;

            return retVal;
        }
    }
}