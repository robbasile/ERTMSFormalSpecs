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

using System.Reflection;
using log4net;
using Utils;

namespace DataDictionary.Interpreter
{
    public abstract class InterpreterTreeNode : INamable, ITextualExplain
    {
        /// <summary>
        ///     Some logging facility
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
        ///     The start character in the string for this expression
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        ///     The end character in the string for this expression
        /// </summary>
        public int End { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="root">The root element for which this interpreter tree node is created</param>
        /// <param name="enclosingNode">The enclosing expression node</param>
        /// <param name="start">The start character for this expression in the original string</param>
        /// <param name="end">The end character for this expression in the original string</param>
        public InterpreterTreeNode(ModelElement root, ModelElement log, int start, int end)
        {
            Root = root;
            RootLog = log;
            StaticUsage = null;
            Start = start;
            End = end;
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
        ///     The EFS System for which this interpreter tree node is created
        /// </summary>
        public EFSSystem EFSSystem
        {
            get { return Root.EFSSystem; }
        }

        /// <summary>
        ///     The Dictionary for which this interpreter tree node is created
        /// </summary>
        public Dictionary Dictionary
        {
            get { return Root.Dictionary; }
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

        public string ToString()
        {
            TextualExplanation explanation = new TextualExplanation();
            GetExplain(explanation);
            return explanation.Text;
        }
    }
}