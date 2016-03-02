// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpecs software and documentation
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

namespace DataDictionary
{
    public class DeleteFilesHandler
    {
        /// <summary>
        /// Indicates whether the file to delete is a regular file or a directory
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// The path of the file/directory to delete
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isDirectory"></param>
        /// <param name="path"></param>
        public DeleteFilesHandler (bool isDirectory, string path)
        {
            IsDirectory = isDirectory;
            Path = path;
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        public void DeleteFile ()
        {
            try
            {
                if (IsDirectory)
                {
                    Directory.Delete(Path, true);
                }
                else
                {
                    File.Delete(Path);
                }
            }
            catch (IOException)
            {
            }
        }
    }
}
