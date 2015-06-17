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

using DataDictionary.Tests.Translations;

namespace GUI.TranslationRules
{
    public class TranslationTreeView : TypedTreeView<TranslationDictionary>
    {
        /// <summary>
        ///     The tests tree node
        /// </summary>
        private TranslationDictionaryTreeNode dictionary;

        /// <summary>
        ///     Constructor
        /// </summary>
        public TranslationTreeView() : base()
        {
            Refactor = false;
        }

        /// <summary>
        ///     Builds the tree model according to the root node
        /// </summary>
        protected override void BuildModel()
        {
            Nodes.Clear();
            dictionary = new TranslationDictionaryTreeNode(Root, true);
            Nodes.Add(dictionary);
        }

        /// <summary>
        ///     Creates a new frame
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TranslationTreeNode createTranslation(Translation translation)
        {
            return dictionary.createTranslation(translation);
        }
    }
}