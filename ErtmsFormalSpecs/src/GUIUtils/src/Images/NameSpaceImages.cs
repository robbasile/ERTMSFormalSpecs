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
using DataDictionary;
using DataDictionary.Types;
using GUIUtils.Properties;
using Utils;

namespace GUIUtils.Images
{
    public class NameSpaceImages
    {
        /// <summary>
        /// All the images available for namespaces
        /// </summary>
        public ImageList Images { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NameSpaceImages()
        {
            Images = new ImageList();
            Images.Images.Add(Resources.antenna);
            Images.Images.Add(Resources.balance);
            Images.Images.Add(Resources.speed_control);
            Images.Images.Add(Resources.train);
            Images.Images.Add(Resources.wheel);            
        }

        /// <summary>
        /// Provides the image associated to a model element
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Image GetImage(IModelElement model)
        {
            Image retVal = null;

            NameSpace nameSpace = EnclosingNameSpaceFinder.find(model, true);
            while (retVal == null && nameSpace != null)
            {
                if (nameSpace.getImageIndex() != 0 && nameSpace.getImageIndex() <= Images.Images.Count)
                {
                    retVal = Images.Images[nameSpace.getImageIndex() - 1];
                }
                nameSpace = EnclosingNameSpaceFinder.find(nameSpace, false);
            }

            return retVal;
        }

        /// <summary>
        /// The singleton
        /// </summary>
        private static NameSpaceImages _instance = null;

        /// <summary>
        /// The singleton
        /// </summary>
        public static NameSpaceImages Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NameSpaceImages();
                }

                return _instance;
            }
        }
    }
}
