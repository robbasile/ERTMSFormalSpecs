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

using System.Drawing;
using System.Windows.Forms;
using DataDictionary.Types;

namespace GUIUtils.Images
{
    /// <summary>
    /// Allows to select an image from the list of available images
    /// </summary>
    public partial class ImagesSelector : Form
    {
        /// <summary>
        /// The namespace for which this selector is built
        /// </summary>
        public NameSpace Model { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        public ImagesSelector(NameSpace model)
        {
            InitializeComponent();

            Model = model;
            
            Point location = new Point(0,0);
            for (int imageIndex = 1; imageIndex<=NameSpaceImages.Instance.Images.Images.Count; imageIndex++)
            {
                Button button= new SelectionButton(model, imageIndex);
                button.Location = location;
                panel.Controls.Add(button);
                if (imageIndex % 6 == 0)
                {
                    location = new Point(0, location.Y + 32);
                }
                else
                {
                    location = new Point(location.X + 32, location.Y);
                }
            }
        }
    }
}
