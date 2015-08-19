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

using DataDictionary.Types;

namespace GUI.DataDictionaryView
{
    public abstract class TypeTreeNode<T> : ReqRelatedTreeNode<T>
        where T : Type
    {
        /// <summary>
        ///     The editor for message variables
        /// </summary>
        protected class TypeEditor : ReqRelatedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="buildSubNodes"></param>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <param name="isFolder"></param>
        /// <param name="addRequirements"></param>
        protected TypeTreeNode(T item, bool buildSubNodes, string name = null, bool isFolder = false,
            bool addRequirements = true)
            : base(item, buildSubNodes, name, isFolder, addRequirements)
        {
        }
    }
}