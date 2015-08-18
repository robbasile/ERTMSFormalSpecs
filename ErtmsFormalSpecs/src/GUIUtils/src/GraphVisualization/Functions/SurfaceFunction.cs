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

using DataDictionary.Functions;

namespace GUIUtils.GraphVisualization.Functions
{
    public abstract class SurfaceFunction : Function
    {
        /// <summary>
        ///     The surface to display
        /// </summary>
        public ISurface Surface { get; set; }

        /// <summary>
        ///     Provides the maximum value of the surface
        /// </summary>
        /// <returns></returns>
        public double MaxVal()
        {
            double retVal = 0;
            if (Surface != null)
            {
                foreach (ISurfaceSegment surfaceSegment in Surface.Segments)
                {
                    for (int i = 0; i < surfaceSegment.Graph.CountSegments(); i++)
                    {
                        ISegment segment = surfaceSegment.Graph.GetSegment(i);
                        if (segment.D0 + segment.Length > retVal)
                        {
                            if (segment.Length > 1000)
                            {
                                retVal += 20;
                            }
                            else
                            {
                                retVal = segment.D0 + segment.Length;
                            }
                        }
                    }
                }
            }
            return retVal;
        }
    }
}