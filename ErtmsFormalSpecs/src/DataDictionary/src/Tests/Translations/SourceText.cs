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

using System.Collections;
using DataDictionary.Interpreter;
using Utils;

namespace DataDictionary.Tests.Translations
{
    public class SourceText : Generated.SourceText, IExpressionable
    {
        public ArrayList Comments
        {
            get
            {
                ArrayList retVal = allComments();

                if (retVal == null)
                {
                    retVal = new ArrayList();
                    setAllComments(retVal);
                }

                return retVal;
            }
        }

        /// <summary>
        ///     The enclosing translation, if any
        /// </summary>
        public Translation Translation
        {
            get { return Enclosing as Translation; }
        }

        public override ArrayList EnclosingCollection
        {
            get { return Translation.SourceTexts; }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
        }

        /// <summary>
        ///     Explains the SourceText
        /// </summary>
        /// <returns></returns>
        public string getExplain(bool explainSubElements)
        {
            return getExplain(explainSubElements, 2);
        }

        /// <summary>
        ///     Explains the SourceText with indentation
        /// </summary>
        /// <returns></returns>
        public string getExplain(bool explainSubElements, int indent)
        {
            return TextualExplainUtilities.Pad(Name, indent);
        }

        /// <summary>
        ///     The expression text for this expressionable
        /// </summary>
        public override string ExpressionText
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        ///     The corresponding expression tree
        /// </summary>
        public InterpreterTreeNode Tree
        {
            get { return null; }
        }

        /// <summary>
        ///     Indicates that the expression is valid for this IExpressionable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool checkValidExpression(string expression)
        {
            return true;
        }

        /// <summary>
        ///     Clears the expression tree to ensure new compilation
        /// </summary>
        public void CleanCompilation()
        {
        }

        /// <summary>
        ///     Creates the tree according to the expression text
        /// </summary>
        public InterpreterTreeNode Compile()
        {
            return null;
        }
    }
}