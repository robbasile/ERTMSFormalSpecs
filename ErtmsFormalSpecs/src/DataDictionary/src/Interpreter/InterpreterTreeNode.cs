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

using DataDictionary.Types;
using DataDictionary.Variables;
using Utils;

namespace DataDictionary.Interpreter
{
    public abstract class InterpreterTreeNode : INamable, ITextualExplain
    {        
        /// <summary>
        ///     The root element for which this interpreter tree node is created and interpreted
        /// </summary>
        public ModelElement Root { get; private set; }

        /// <summary>
        ///     The root element for which errors should be raised
        /// </summary>
        public ModelElement RootLog { get; private set; }

        /// <summary>
        ///     The enclosing interpreter tree node
        /// </summary>
        public InterpreterTreeNode Enclosing { get; set; }

        /// <summary>
        ///     The static usages performed by this statement
        /// </summary>
        public Usages StaticUsage { get; protected set; }

        /// <summary>
        ///     Additional information about parsing process
        /// </summary>
        public ParsingData ParsingData { get; private set; }

        /// <summary>
        /// The position of the first character
        /// </summary>
        public int Start
        {
            get { return ParsingData.Start; }
            set { ParsingData.Start = value; }
        }

        /// <summary>
        /// The position of the last character 
        /// </summary>
        public int End
        {
            get { return ParsingData.End; }
            set { ParsingData.End = value; }            
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this interpreter tree node is created</param>
        /// <param name="log">The element on which logs should be added</param>
        /// <param name="parsingData">Additional information about the parsing process</param>
        protected InterpreterTreeNode(ModelElement root, ModelElement log, ParsingData parsingData)
        {
            Root = root;
            RootLog = log;
            StaticUsage = null;
            ParsingData = parsingData;
        }

        public string Name
        {
            get { return ToString(); }
            set { }
        }

        public string FullName
        {
            get { return Name; }
        }

        /// <summary>
        ///     The Dictionary for which this interpreter tree node is created
        /// </summary>
        public Dictionary Dictionary
        {
            get { return Root.Dictionary; }
        }

        /// <summary>
        /// Sets this as the enclosing element
        /// </summary>
        /// <param name="enclosed"></param>
        protected T SetEnclosed<T>(T enclosed)
            where T : InterpreterTreeNode
        {
            if (enclosed != null)
            {
                enclosed.Enclosing = this;
            }

            return enclosed;
        }

        /// <summary>
        /// Creates a synthetic variable used to evaluate this interpreter tree node
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected IVariable CreateBoundVariable(string name, Type type)
        {
            return new SyntheticVariable(this, name, type);
        }

        /// <summary>
        ///     Adds an error message to the root element
        /// </summary>
        /// <param name="message"></param>
        public void AddError(string message)
        {
            if (RootLog != null)
            {
                RootLog.AddError(message);
            }
        }

        /// <summary>
        ///     Adds an error message to the root element and explains it
        /// </summary>
        /// <param name="message"></param>
        /// <param name="explain"></param>
        public virtual void AddErrorAndExplain(string message, ExplanationPart explain)
        {
            if (RootLog != null)
            {
                RootLog.AddError(message);
            }
        }

        /// <summary>
        ///     Adds an error message to the root element
        /// </summary>
        /// <param name="message"></param>
        public void AddWarning(string message)
        {
            if (RootLog != null)
            {
                RootLog.AddWarning(message);
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public abstract void GetExplain(TextualExplanation explanation, bool explainSubElements = true);

        public override string ToString()
        {
            TextualExplanation explanation = new TextualExplanation();
            GetExplain(explanation);
            return explanation.Text;
        }
    }
}