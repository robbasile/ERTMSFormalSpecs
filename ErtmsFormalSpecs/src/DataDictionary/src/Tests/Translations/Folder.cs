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
using Utils;

namespace DataDictionary.Tests.Translations
{
    public class Folder : Generated.Folder, ITextualExplain
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Folder()
        {
        }

        /// <summary>
        ///     Provides the folders
        /// </summary>
        public ArrayList Folders
        {
            get
            {
                if (allFolders() == null)
                {
                    setAllFolders(new ArrayList());
                }
                return allFolders();
            }
            set { setAllFolders(value); }
        }

        /// <summary>
        ///     Provides the translations for this dictionary
        /// </summary>
        public ArrayList Translations
        {
            get
            {
                if (allTranslations() == null)
                {
                    setAllTranslations(new ArrayList());
                }
                return allTranslations();
            }
            set { setAllTranslations(value); }
        }

        /// <summary>
        ///     Provides the number of translations of the current folder and its sub-folders
        /// </summary>
        public int TranslationsCount
        {
            get
            {
                int retVal = allTranslations().Count;
                foreach (Folder folder in Folders)
                {
                    retVal += folder.TranslationsCount;
                }
                return retVal;
            }
        }

        /// <summary>
        ///     Provides the enclosing collection
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get
            {
                ArrayList retVal = null;
                TranslationDictionary dictionary = Enclosing as TranslationDictionary;
                if (dictionary != null)
                {
                    retVal = dictionary.Folders;
                }
                else
                {
                    Folder folder = Enclosing as Folder;
                    if (folder != null)
                    {
                        retVal = folder.Folders;
                    }
                }
                return retVal;
            }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            Folder folder = element as Folder;
            if (folder != null)
            {
                appendFolders(folder);
            }
            else
            {
                Translation translation = element as Translation;
                if (translation != null)
                {
                    appendTranslations(translation);
                }
            }
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public virtual void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Write("FOLDER ");
            explanation.WriteLine(Name);

            if (explainSubElements)
            {
                explanation.Indent(2, () =>
                {
                    foreach (Folder folder in Folders)
                    {
                        folder.GetExplain(explanation, explainSubElements);
                    }
                });
            }

            explanation.Indent(2, () =>
            {
                foreach (Translation translation in Translations)
                {
                    translation.GetExplain(explanation, explainSubElements);
                }
            });
            explanation.Write("END FOLDER ");
            explanation.WriteLine(Name);
        }
    }
}