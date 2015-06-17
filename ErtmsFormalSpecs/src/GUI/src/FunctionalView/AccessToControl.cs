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

using System.ComponentModel;
using System.Drawing;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Types.AccessMode;
using GUI.BoxArrowDiagram;

namespace GUI.FunctionalView
{
    public partial class AccessToControl : ArrowControl<NameSpace, AccessMode>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public AccessToControl()
            : base()
        {
            ArrowMode = ArrowModeEnum.Half;
            ArrowFill = ArrowFillEnum.Fill;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="container"></param>
        public AccessToControl(IContainer container)
            : base()
        {
            container.Add(this);
        }

        public override AccessMode Model
        {
            get { return base.Model; }
            set
            {
                base.Model = value;
                AccessToVariable accessToVariable = value as AccessToVariable;
                if (accessToVariable != null)
                {
                    switch (accessToVariable.AccessMode)
                    {
                        case Usage.ModeEnum.Read:
                            NORMAL_COLOR = Color.Green;
                            NORMAL_PEN = new Pen(NORMAL_COLOR);
                            break;

                        case Usage.ModeEnum.ReadAndWrite:
                            NORMAL_COLOR = Color.Orange;
                            NORMAL_PEN = new Pen(NORMAL_COLOR);
                            break;

                        case Usage.ModeEnum.Write:
                            NORMAL_COLOR = Color.Red;
                            NORMAL_PEN = new Pen(NORMAL_COLOR);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Provides the name of the target state
        /// </summary>
        /// <returns></returns>
        public override string getTargetName()
        {
            string retVal = "<Unknown>";

            if (Model.Target != null)
            {
                retVal = Model.Target.FullName;
            }

            return retVal;
        }
    }
}