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
using DataDictionary.Generated;
using Utils;
using DataDictionary.src;
using RuleCheckDisabling = DataDictionary.RuleCheck.RuleCheckDisabling;

namespace DataDictionary.Specification
{
    public class Chapter : Generated.Chapter, IHoldsParagraphs, RuleCheck.IRuleCheckDisabling
    {
        /// <summary>
        ///     The chapter name
        /// </summary>
        public override string Name
        {
            get { return "Chapter " + getId(); }
            set { }
        }

        /// <summary>
        /// The id of the chapter
        /// </summary>
        public string Id
        {
            get { return getId (); }
            set
            {
                if (value != getId ())
                {
                    // if the name changes, the previous file has to be deleted
                    RecordFilesToDelete();
                }
                setId (value);
            }
            
        }

        /// <summary>
        ///     The paragraphs
        /// </summary>
        public ArrayList Paragraphs
        {
            get
            {
                if (allParagraphs() == null)
                {
                    setAllParagraphs(new ArrayList());
                }
                return allParagraphs();
            }
            private set { setAllParagraphs(value); }
        }

        /// <summary>
        ///     The type specs
        /// </summary>
        public ArrayList TypeSpecs
        {
            get
            {
                if (allTypeSpecs() == null)
                {
                    setAllTypeSpecs(new ArrayList());
                }
                return allTypeSpecs();
            }
            set { setAllTypeSpecs(value); }
        }

        /// <summary>
        ///     Looks for a specific paragraph in this chapter
        /// </summary>
        /// <param name="id">The id of the paragraph to find</param>
        /// <param name="create">If true, creates the paragraph tree if needed</param>
        /// <returns></returns>
        public Paragraph FindParagraph(String id, bool create = false)
        {
            Paragraph retVal = null;

            foreach (Paragraph paragraph in Paragraphs)
            {
                if (id.StartsWith(paragraph.FullId))
                {
                    retVal = paragraph.FindParagraph(id, create);
                    if (retVal != null)
                    {
                        break;
                    }
                }
            }

            return retVal;
        }

        /**
         * Provides the paragraphs that require an implementation
         */

        public ICollection<Paragraph> applicableParagraphs()
        {
            ICollection<Paragraph> retVal = new HashSet<Paragraph>();

            foreach (Paragraph p in Paragraphs)
            {
                applicableParagraphs(p, retVal);
            }

            return retVal;
        }

        private void applicableParagraphs(Paragraph paragraph, ICollection<Paragraph> retVal)
        {
            if (paragraph.IsApplicable())
            {
                retVal.Add(paragraph);
            }
            foreach (Paragraph p in paragraph.SubParagraphs)
            {
                applicableParagraphs(p, retVal);
            }
        }

        /// <summary>
        ///     Provides the enclosing specification
        /// </summary>
        public Specification EnclosingSpecification
        {
            get { return Enclosing as Specification; }
        }

        /// <summary>
        ///     Provides the enclosing collection
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return EnclosingSpecification.Chapters; }
        }

        /// <summary>
        ///     The current index
        /// </summary>
        private static int index;

        /// <summary>
        ///     Restructure the paragraph nodes
        /// </summary>
        public void RestructureParagraphs()
        {
            index = 0;
            Paragraphs = InnerRestructureParagraphs(this.Paragraphs, 2);
        }

        /// <summary>
        ///     Restructure the paragraph nodes
        /// </summary>
        /// <param name="elements">The elements to be placed in the node</param>
        /// <param name="level">The current paragraph level</param>
        private static ArrayList InnerRestructureParagraphs(ArrayList elements, int level)
        {
            ArrayList retVal = new ArrayList();
            Paragraph current = null;

            while (index < elements.Count)
            {
                Paragraph paragraph = (Paragraph) elements[index];
                List<Paragraph> subNodes = new List<Paragraph>();

                if (paragraph.Level == level)
                {
                    index = index + 1;
                    if (current != null)
                    {
                        retVal.Add(current);
                    }
                    current = paragraph;
                }
                else if (paragraph.Level > level)
                {
                    if (current != null)
                    {
                        retVal.Add(current);
                        current.SubParagraphs = InnerRestructureParagraphs(elements, level + 1);
                        current = null;
                    }
                    else
                    {
                        retVal = InnerRestructureParagraphs(elements, level + 1);
                    }
                }
                else
                {
                    break;
                }
            }

            if (current != null)
            {
                retVal.Add(current);
            }
            return retVal;
        }

        /// <summary>
        ///     Restructures the names of the paragraphs
        /// </summary>
        public void RestructureParagraphsNames()
        {
            foreach (Paragraph paragraph in Paragraphs)
            {
                paragraph.RestructureName();
            }
        }

        /// <summary>
        ///     Looks for specific paragraphs in this chapter, whose number begins with the Id provided
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal void SubParagraphs(string id, List<Paragraph> retVal)
        {
            foreach (Paragraph paragraph in Paragraphs)
            {
                if (paragraph.getId() != null && paragraph.getId().StartsWith(id))
                {
                    retVal.Add(paragraph);
                }
            }
        }

        /// <summary>
        ///     Adds all the paragraphs in the set provided as parameter
        /// </summary>
        /// <param name="retVal"></param>
        public void GetParagraphs(List<Paragraph> retVal)
        {
            foreach (Paragraph paragraph in Paragraphs)
            {
                paragraph.FillCollection(retVal);
            }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                Paragraph item = element as Paragraph;
                if (item != null)
                {
                    appendParagraphs(item);
                }
            }
        }

        /// <summary>
        ///     The chapter ref which instanciated this chapter
        /// </summary>
        public ChapterRef ChapterRef { get; set; }

        /// <summary>
        ///     Provides the update to this chapter in target dictionary, if any
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public Chapter FindChapterUpdate(Dictionary dictionary)
        {
            Chapter retVal = null;

            foreach (ModelElement update in UpdatedBy)
            {
                if (update.Dictionary == dictionary)
                {
                    Chapter chapterUpdate = update as Chapter;
                    if (chapterUpdate != null)
                        retVal = chapterUpdate;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates an update of this chapter in the provided specification
        /// </summary>
        /// <param name="specification">The specification update</param>
        /// <returns></returns>
        public Chapter CreateChapterUpdate(Specification specification)
        {
            Chapter retVal = new Chapter();
            retVal.Id = Id;
            retVal.setUpdates(Guid);

            specification.appendChapters(retVal);
            ArrayList tmp = new ArrayList();
            foreach (Chapter chapter in EnclosingSpecification.Chapters)
            {
                if (chapter.UpdatedBy != null)
                {
                    tmp.Add(chapter.UpdatedBy);
                }
                if (chapter == this)
                {
                    tmp.Add(retVal);
                }
            }

            UpdatedBy.Add(retVal);

            return retVal;
        }

        /// <summary>
        ///     Inserts a paragraph update at the right location in the list
        /// </summary>
        /// <param name="paragraphUpdate">The updated paragraph</param>
        /// <param name="baseCollection">The base collection of elements, used as a reference for the order</param>
        public void InsertParagraph(Paragraph paragraphUpdate, ArrayList baseCollection)
        {
            ArrayList tmp = new ArrayList();
            int index = 0;
            foreach (Paragraph par in baseCollection)
            {
                if (Paragraphs.Count > index)
                {
                    ModelElement currentParagraphUpdate = Paragraphs[index] as ModelElement;
                    if (currentParagraphUpdate != null && par.UpdatedBy.Contains(currentParagraphUpdate))
                    {
                        tmp.Add(currentParagraphUpdate);
                        index++;
                    }
                }
                if (paragraphUpdate.Updates == par)
                {
                    tmp.Add(paragraphUpdate);
                }
            }

            Paragraphs = tmp;
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string retVal = base.CreateStatusMessage();

            List<Paragraph> paragraphs = new List<Paragraph>();
            GetParagraphs(paragraphs);

            retVal += Paragraph.CreateParagraphSetStatus(paragraphs);

            return retVal;
        }

        /// <summary>
        ///     Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <returns></returns>
        public static Chapter CreateDefault(ICollection enclosingCollection)
        {
            Chapter retVal = (Chapter) acceptor.getFactory().createChapter();

            Util.DontNotify(() =>
            {
                retVal.Name = "Chapter" + GetElementNumber(enclosingCollection);
                retVal.Id = GetElementNumber(enclosingCollection).ToString();
            });

            return retVal;
        }

        /// <summary>
        /// Removes the chapter and stores the file to delete
        /// </summary>
        public override void Delete()
        {
            RecordFilesToDelete ();
            base.Delete();
        }

        /// <summary>
        /// Stores the files to be deleted
        /// </summary>
        private void RecordFilesToDelete ()
        {
            if (Dictionary != null)
            {
                string path = Dictionary.FilePath.Remove (Dictionary.FilePath.LastIndexOf ('.'));
                path += "\\Specifications\\" + FullName.Replace (".", "\\") + ".efs_ch";
                Dictionary.AddDeleteFilesElement (new DeleteFilesHandler (false, path));
            }
        }

        /// <summary>
        /// Provides the RuleCheck disabling, if any
        /// </summary>
        public RuleCheckDisabling Disabling
        {
            get { return (RuleCheckDisabling)getRuleCheckDisabling(); }
            set { setRuleCheckDisabling(value); }
        }
    }
}