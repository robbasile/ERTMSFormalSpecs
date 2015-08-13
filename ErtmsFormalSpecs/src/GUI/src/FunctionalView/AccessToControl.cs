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
using DataDictionary;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Types.AccessMode;
using GUI.BoxArrowDiagram;

namespace GUI.FunctionalView
{
    public class AccessToControl : ArrowControl<IEnclosesNameSpaces, NameSpace, AccessMode>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="model"></param>
        public AccessToControl(FunctionalAnalysisPanel panel, AccessMode model) 
            : base(panel, model)
        {
            ArrowMode = ArrowModeEnum.Half;
            ArrowFill = ArrowFillEnum.Fill;
        }
        

        /// <summary>
        /// The model on which this element is related
        /// </summary>
        public override object Model
        {
            set
            {
                base.Model = value;

                AccessToVariable accessToVariable = value as AccessToVariable;
                if (accessToVariable != null)
                {
                    switch (accessToVariable.AccessMode)
                    {
                        case Usage.ModeEnum.Read:
                            NormalColor = Color.Green;
                            NormalPen = new Pen(NormalColor);
                            break;

                        case Usage.ModeEnum.ReadAndWrite:
                            NormalColor = Color.Orange;
                            NormalPen = new Pen(NormalColor);
                            break;

                        case Usage.ModeEnum.Write:
                            NormalColor = Color.Red;
                            NormalPen = new Pen(NormalColor);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Provides the name of the target state
        /// </summary>
        /// <returns></returns>
        public override string GetTargetName()
        {
            string retVal = "<Unknown>";

            if (TypedModel.Target != null)
            {
                retVal = TypedModel.Target.FullName;
            }

            return retVal;
        }
    }
}