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
using System.Net.Mime;
using System.Text.RegularExpressions;
using Utils;

namespace DataDictionary.Tests.Translations
{
    public class TranslationDictionary : Generated.TranslationDictionary, IFinder
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public TranslationDictionary()
            : base()
        {
            FinderRepository.INSTANCE.Register(this);
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
        ///     Provides the number of translations of the TranslationDictionary and all its folders
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
        ///     Strips the text from useless characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StripText(string text)
        {
            string retVal = "";

            if (!string.IsNullOrEmpty(text))
            {
                char[] source = text.ToCharArray();
                char[] tmp = new char[source.Length];

                int i = 0;
                int j = 0;
                while (i < source.Length)
                {
                    if (Char.IsLetterOrDigit(source[i]))
                    {
                        tmp[j] = source[i];
                        j += 1;
                    }

                    i += 1;
                }

                retVal = new string(tmp, 0, j);
            }

            return retVal;
        }

        /// <summary>
        ///     The cache for texts
        /// </summary>
        private Dictionary<string, Dictionary<string, Translation>> theCache = null;

        /// <summary>
        /// The cache for regular expressions
        /// </summary>
        private Dictionary<Regex, Dictionary<string, Translation>> theRegularExpressionCache = null;

        /// <summary>
        /// Builds the caches
        /// </summary>
        private void BuildCache()
        {
            if (theCache == null || theRegularExpressionCache == null)
            {
                theCache = new Dictionary<string, Dictionary<string, Translation>>();
                theRegularExpressionCache = new Dictionary<Regex, Dictionary<string, Translation>>();

                foreach (Folder folder in Folders)
                {
                    StoreTranslationsInFolder(folder);
                }
                foreach (Translation translation in Translations)
                {
                    storeTranslationInCache(translation);
                }
            }
        }


        private void StoreTranslationsInFolder(Folder folder)
        {
            foreach (Folder subFolder in folder.Folders)
            {
                StoreTranslationsInFolder(subFolder);
            }
            foreach (Translation translation in folder.Translations)
            {
                storeTranslationInCache(translation);
            }
        }

        /// <summary>
        ///     Indicates that the source text is applicable for any comment
        /// </summary>
        private static string NO_SPECIFIC_COMMENT = "___NOSPECIFICCOMMENT___";

        private void storeTranslationInCache(Translation translation)
        {
            foreach (SourceText sourceText in translation.SourceTexts)
            {
                Dictionary<string, Translation> tmp = null;

                if (sourceText.getRegularExpression())
                {
                    Regex regex = new Regex(sourceText.Name);
                    foreach (KeyValuePair<Regex, Dictionary<string, Translation>> pair in theRegularExpressionCache)
                    {
                        if (pair.Key.ToString() == regex.ToString())
                        {
                            tmp = pair.Value;
                            break;
                        }                        
                    }

                    if (tmp == null)
                    {
                        tmp = new Dictionary<string, Translation>();
                        theRegularExpressionCache[regex] = tmp;
                    }
                }
                else
                {
                    string textDescription = StripText(sourceText.Name);

                    if (!theCache.TryGetValue(textDescription, out tmp))
                    {
                        tmp = new Dictionary<string, Translation>();
                        theCache[textDescription] = tmp;
                    }
                }

                if (sourceText.Comments.Count > 0)
                {
                    foreach (SourceTextComment comment in sourceText.Comments)
                    {
                        string commentValue = StripText(comment.Name);
                        tmp[commentValue] = translation;
                    }
                }
                else
                {
                    tmp[NO_SPECIFIC_COMMENT] = translation;
                }
            }
        }

        /// <summary>
        ///     Provides the translation which matches the description provided. Matching is performed the following way
        ///     1.  First try to find a translation whose source text corresponds to the step description
        ///     2.  If that source text holds any associated comment, ensure that the step comment matches one of them
        /// </summary>
        /// <param name="description"></param>
        /// <param name="comment">the comment associated to the step</param>
        /// <returns></returns>
        public Translation findTranslation(string description, string comment)
        {
            Translation retVal = null;

            if (description != null)
            {
                BuildCache();

                Dictionary<string, Translation> tmp = null;

                // Try to find a perfect match
                string text = StripText(description);
                if (theCache.ContainsKey(text))
                {
                    tmp = theCache[text];
                }

                if (tmp != null)
                {
                    string commentValue = StripText(comment);
                    if (!tmp.TryGetValue(commentValue, out retVal))
                    {
                        tmp.TryGetValue(NO_SPECIFIC_COMMENT, out retVal);
                    }
                }

                if (retVal == null)
                {
                    // Try to find in the regular expressions
                    foreach (KeyValuePair<Regex, Dictionary<string, Translation>> pair in theRegularExpressionCache)
                    {
                        if (pair.Key.IsMatch(description))
                        {
                            tmp = pair.Value;
                        }
                    }

                    if (tmp != null)
                    {
                        string commentValue = StripText(comment);
                        if (!tmp.TryGetValue(commentValue, out retVal))
                        {
                            commentValue = NO_SPECIFIC_COMMENT;
                            tmp.TryGetValue(commentValue, out retVal);
                        }

                        if (retVal != null)
                        {
                            // Store this result for further use
                            string textDescription = StripText(description);
                            if (!theCache.TryGetValue(textDescription, out tmp))
                            {
                                tmp = new Dictionary<string, Translation>();
                                theCache[textDescription] = tmp;
                            }
                            tmp[commentValue] = retVal;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Clears the cache
        /// </summary>
        public void ClearCache()
        {
            theCache = null;
            theRegularExpressionCache = null;
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
        ///     Provides the existing translation corresponding to the source text provided
        /// </summary>
        /// <param name="sourceText"></param>
        /// <returns></returns>
        public Translation FindExistingTranslation(SourceText sourceText)
        {
            Translation existingTranslation = null;

            if (sourceText.Comments.Count > 0)
            {
                foreach (SourceTextComment comment in sourceText.Comments)
                {
                    existingTranslation = findTranslation(sourceText.Name, comment.Name);
                    if (existingTranslation != null)
                    {
                        break;
                    }
                }
            }
            else
            {
                existingTranslation = findTranslation(sourceText.Name, "");
            }

            return existingTranslation;
        }
    }
}