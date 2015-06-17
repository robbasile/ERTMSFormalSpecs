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

using System.IO;
using Utils;

namespace DataDictionary.Types
{
    public class NameSpaceRef : Generated.NameSpaceRef
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public NameSpaceRef()
            : base()
        {
        }


        /// <summary>
        ///     The file name which corresponds to this namespace ref
        /// </summary>
        public string FileName
        {
            get
            {
                string retVal = Name + ".efs_ns";
                IModelElement current = Enclosing as IModelElement;
                while (current != null && !(current is Dictionary))
                {
                    retVal = current.Name + Path.DirectorySeparatorChar + retVal;
                    current = current.Enclosing as IModelElement;
                }

                return Dictionary.BasePath + Path.DirectorySeparatorChar + Util.validFilePath(retVal);
            }
        }

        /// <summary>
        ///     Saves the namespace provided associated to this namespace ref
        /// </summary>
        /// <param name="nameSpace"></param>
        public void SaveNameSpace(NameSpace nameSpace)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            VersionedWriter writer = new VersionedWriter(FileName);
            nameSpace.unParse(writer, false);
            writer.Close();
        }

        /// <summary>
        ///     Loads the namespace which corresponds to this namespace ref
        /// </summary>
        /// <param name="lockFiles">Indicates that the files should be locked</param>
        /// <param name="allowErrors">Allows errors during load (best effort)</param>
        /// <returns></returns>
        public NameSpace LoadNameSpace(bool lockFiles, bool allowErrors)
        {
            return Util.loadNameSpace(FileName, Enclosing as ModelElement, lockFiles, allowErrors);
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