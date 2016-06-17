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

using DataDictionary.Interpreter;

namespace DataDictionary.RuleCheck.GraphAndSurface
{
    /// <summary>
    /// Used to find all call in an expression which reference a given model element
    /// </summary>
    public class FindCall : FindReference<Call>         
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modelElement"></param>
        public FindCall(ModelElement modelElement) : base(modelElement)
        {
        }

        /// <summary>
        /// Also references the calls which Called expression reference the model element
        /// </summary>
        /// <param name="call"></param>
        protected override void VisitCall(Call call)
        {
            if (call.Called.Ref == ModelElement)
            {
                References.Add(call);                
            }

            base.VisitCall(call);
        }
    }
}
