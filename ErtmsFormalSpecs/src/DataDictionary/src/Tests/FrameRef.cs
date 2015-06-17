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

namespace DataDictionary.Tests
{
    public class FrameRef : Generated.FrameRef
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public FrameRef()
            : base()
        {
        }

        /// <summary>
        ///     The file name which corresponds to this frame ref
        /// </summary>
        public string FileName
        {
            get
            {
                string retVal = Name + ".efs_tst";
                IModelElement current = Enclosing as IModelElement;
                while (current != null && !(current is Dictionary))
                {
                    retVal = current.Name + Path.DirectorySeparatorChar + retVal;
                    current = current.Enclosing as IModelElement;
                }

                return Dictionary.BasePath + Path.DirectorySeparatorChar + "TestFrames" + Path.DirectorySeparatorChar +
                       Util.validFilePath(retVal);
            }
        }

        /// <summary>
        ///     Saves the frame provided associated to this frame ref
        /// </summary>
        /// <param name="frame"></param>
        public void SaveFrame(Frame frame)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            VersionedWriter writer = new VersionedWriter(FileName);
            frame.unParse(writer, false);
            writer.Close();
        }

        /// <summary>
        ///     Loads the frame which corresponds to this frame ref
        /// </summary>
        /// <param name="lockFiles">Indicates that the files should be locked</param>
        /// <param name="allowErrors">Indicates that errors are tolerated during load</param>
        /// <returns></returns>
        public Frame LoadFrame(bool lockFiles, bool allowErrors)
        {
            return Util.loadFrame(FileName, Enclosing as ModelElement, lockFiles, allowErrors);
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