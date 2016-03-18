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
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using DataDictionary.Types;

namespace GUIUtils.Images
{
    public class SelectionButton : Button
    {
        /// <summary>
        /// The namespace for which this selector is built
        /// </summary>
        public NameSpace Model { get; private set; }

        /// <summary>
        /// The image index
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        public SelectionButton(NameSpace model, int index)
        {
            Model = model;
            Index = index;
            Image = NameSpaceImages.Instance.Images.Images[index-1];
            Click += OnClick;
            Size = new Size(32, 32);
        }

        /// <summary>
        /// Handles a click event to set the image index in the model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnClick(object sender, EventArgs eventArgs)
        {
            Model.setImageIndex(Index);
        }
    }
}
