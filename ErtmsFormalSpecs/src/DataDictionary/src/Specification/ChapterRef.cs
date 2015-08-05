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
using System.IO;
using Utils;

namespace DataDictionary.Specification
{
    public class ChapterRef : Generated.ChapterRef
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public ChapterRef()
            : base()
        {
        }

        /// <summary>
        ///     The file name which corresponds to this chapter ref
        /// </summary>
        public string FileName
        {
            get
            {
                string retVal = Name + ".efs_ch";
                IModelElement current = Enclosing as IModelElement;
                while (current != null && !(current is Dictionary))
                {
                    retVal = current.Name + Path.DirectorySeparatorChar + retVal;
                    current = current.Enclosing as IModelElement;
                }

                retVal = Dictionary.BasePath + Path.DirectorySeparatorChar + "Specifications" +
                         Path.DirectorySeparatorChar + Util.ValidFilePath(retVal);

                return retVal;
            }
        }

        /// <summary>
        ///     The file name which corresponds to this chapter ref
        /// </summary>
        public string PreviousFileName
        {
            get
            {
                string retVal = Name + ".efs_ch";
                IModelElement current = Enclosing as IModelElement;
                while (current != null && !(current is Specification))
                {
                    retVal = current.Name + Path.DirectorySeparatorChar + retVal;
                    current = current.Enclosing as IModelElement;
                }

                retVal = Dictionary.BasePath + Path.DirectorySeparatorChar + "Specifications" +
                         Path.DirectorySeparatorChar + retVal;

                return retVal;
            }
        }

        /// <summary>
        ///     Saves the chapter provided associated to this chapter ref
        /// </summary>
        /// <param name="chapter"></param>
        public void SaveChapter(Chapter chapter)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            VersionedWriter writer = new VersionedWriter(FileName);
            chapter.unParse(writer, false);
            writer.Close();
        }

        /// <summary>
        ///     Loads the frame which corresponds to this frame ref
        /// </summary>
        /// <param name="lockFiles">Indicates that the files should be locked</param>
        /// <param name="allowErrors">Indicates that errors are tolerated during load</param>
        /// <returns></returns>
        public Chapter LoadChapter(bool lockFiles, bool allowErrors)
        {
            Chapter retVal;

            try
            {
                retVal = Util.LoadChapter(FileName, Enclosing as ModelElement, lockFiles, allowErrors);
            }
            catch (Exception)
            {
                // Maybe the other naming  ?
                retVal = Util.LoadChapter(PreviousFileName, Enclosing as ModelElement, lockFiles, allowErrors);
            }

            return retVal;
        }

        /// <summary>
        ///     Removes the temporary file associated to that item
        /// </summary>
        public void ClearTempFile()
        {
            VersionedWriter writer = new VersionedWriter(FileName);
            writer.Close();
        }
    }
}