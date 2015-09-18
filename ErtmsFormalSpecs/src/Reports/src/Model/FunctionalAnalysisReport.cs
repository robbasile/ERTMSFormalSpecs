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

using System.Collections;
using System.Collections.Generic;
using DataDictionary;
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Types.AccessMode;
using DataDictionary.Variables;
using MigraDoc.DocumentObjectModel;

namespace Reports.Model
{
    public class FunctionalAnalysisReport : ReportTools
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="document"></param>
        public FunctionalAnalysisReport(Document document)
            : base(document)
        {
        }

        /// <summary>
        ///     Creates a section for a namespace
        /// </summary>
        /// <param name="nameSpace">The namespace to consider</param>
        /// <param name="accesses">The accesses performed in the system</param>
        /// <returns></returns>
        public void CreateNameSpaceSection(NameSpace nameSpace, List<AccessMode> accesses)
        {
            AddSubParagraph("Namespace " + nameSpace.FullName);

            Dictionary<Procedure, List<ProcedureOrFunctionCall>> procedureCalls =
                new Dictionary<Procedure, List<ProcedureOrFunctionCall>>();
            Dictionary<Function, List<ProcedureOrFunctionCall>> functionCalls =
                new Dictionary<Function, List<ProcedureOrFunctionCall>>();
            Dictionary<IVariable, List<AccessToVariable>> accessesToVariables =
                new Dictionary<IVariable, List<AccessToVariable>>();

            foreach (AccessMode access in accesses)
            {
                ProcedureOrFunctionCall call = access as ProcedureOrFunctionCall;
                if (call != null)
                {
                    if (call.Target == nameSpace)
                    {
                        {
                            Function function = call.ReferencedModel as Function;
                            if (function != null)
                            {
                                List<ProcedureOrFunctionCall> calls;
                                if (!functionCalls.TryGetValue(function, out calls))
                                {
                                    calls = new List<ProcedureOrFunctionCall>();
                                    functionCalls.Add(function, calls);
                                }
                                calls.Add(call);
                            }
                        }

                        {
                            Procedure procedure = call.ReferencedModel as Procedure;
                            if (procedure != null)
                            {
                                List<ProcedureOrFunctionCall> calls;
                                if (!procedureCalls.TryGetValue(procedure, out calls))
                                {
                                    calls = new List<ProcedureOrFunctionCall>();
                                    procedureCalls.Add(procedure, calls);
                                }
                                calls.Add(call);
                            }
                        }
                    }
                }

                AccessToVariable accessToVariable = access as AccessToVariable;
                if (accessToVariable != null)
                {
                    if (accessToVariable.Target == nameSpace)
                    {
                        List<AccessToVariable> variableAccesses;
                        if (!accessesToVariables.TryGetValue(accessToVariable.Variable, out variableAccesses))
                        {
                            variableAccesses = new List<AccessToVariable>();
                            accessesToVariables.Add(accessToVariable.Variable, variableAccesses);
                        }
                        variableAccesses.Add(accessToVariable);
                    }
                }
            }

            if (procedureCalls.Keys.Count > 0)
            {
                AddSubParagraph("Exposed procedures");
                foreach (KeyValuePair<Procedure, List<ProcedureOrFunctionCall>> pair in procedureCalls)
                {
                    Procedure procedure = pair.Key;
                    AddSubParagraph("Procedure " + procedure.Name);
                    CreateProcedureOrFunctionHeader(procedure);
                    CreateProcedureOrFunctionUsage(pair.Value);
                    CloseSubParagraph();
                }
                CloseSubParagraph();
            }

            if (functionCalls.Keys.Count > 0)
            {
                AddSubParagraph("Exposed functions");
                foreach (KeyValuePair<Function, List<ProcedureOrFunctionCall>> pair in functionCalls)
                {
                    Function function = pair.Key;
                    AddSubParagraph("Function " + function.Name);
                    CreateProcedureOrFunctionHeader(function);
                    CreateProcedureOrFunctionUsage(pair.Value);
                    CloseSubParagraph();
                }
                CloseSubParagraph();
            }

            if (accessesToVariables.Count > 0)
            {
                AddSubParagraph("Exposed variables");
                foreach (KeyValuePair<IVariable, List<AccessToVariable>> pair in accessesToVariables)
                {
                    IVariable variable = pair.Key;
                    AddSubParagraph("Variable " + variable.Name);
                    CreateVariableHeader(variable);
                    CreateVariableUsage(pair.Value);
                    CloseSubParagraph();
                }
                CloseSubParagraph();
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Provides the description of a procedure or a function
        /// </summary>
        /// <param name="callable">The callable to describe</param>
        /// <returns></returns>
        private void CreateProcedureOrFunctionHeader(ICallable callable)
        {
            AddTable(new[] {callable.Name}, new[] {40, 80});

            ICommentable commentable = callable as ICommentable;
            if (commentable != null && !string.IsNullOrEmpty(commentable.Comment))
            {
                AddRow(commentable.Comment);
            }

            if (callable.FormalParameters.Count > 0)
            {
                AddTableHeader("Parameters");
                AddTableHeader("Name", "Type");
                foreach (Parameter parameter in callable.FormalParameters)
                {
                    AddRow(parameter.Name, parameter.getTypeName());
                }
            }

            ITypedElement typedElement = callable as ITypedElement;
            if (typedElement != null && !string.IsNullOrEmpty(typedElement.TypeName))
            {
                AddTableHeader("Return value");
                AddRow(typedElement.TypeName);
            }

            ReqRelated reqRelated = callable as ReqRelated;
            if (reqRelated != null)
            {
                AddTableHeader("Related requirements");
                AddRow(GetRequirementsAsString(reqRelated.Requirements));
            }
        }

        /// <summary>
        ///     Creates the procedure usage table of a list of access modes
        /// </summary>
        /// <param name="usages"></param>
        private void CreateProcedureOrFunctionUsage(List<ProcedureOrFunctionCall> usages)
        {
            AddSubParagraph("Known usages");

            AddTable(new string[] {"Usage"}, new int[] {100});
            foreach (ProcedureOrFunctionCall call in usages)
            {
                AddRow(call.Source.FullName);
            }

            CloseSubParagraph();
        }


        /// <summary>
        ///     Provides the description of a variable
        /// </summary>
        /// <param name="variable">The variable to describe</param>
        /// <returns></returns>
        private void CreateVariableHeader(IVariable variable)
        {
            AddTable(new string[] {variable.Name}, new int[] {40, 80});

            ICommentable commentable = variable as ICommentable;
            if (commentable != null && !string.IsNullOrEmpty(commentable.Comment))
            {
                AddRow(commentable.Comment);
            }

            if (!string.IsNullOrEmpty(variable.TypeName))
            {
                AddTableHeader("Type");
                AddRow(variable.TypeName);
            }

            ReqRelated reqRelated = variable as ReqRelated;
            if (reqRelated != null)
            {
                AddTableHeader("Related requirements");
                AddRow(GetRequirementsAsString(reqRelated.Requirements));
            }
        }

        /// <summary>
        ///     Creates the variable usage table of a list of access modes
        /// </summary>
        /// <param name="usages"></param>
        private void CreateVariableUsage(List<AccessToVariable> usages)
        {
            AddSubParagraph("Known usages");

            AddTable(new string[] {"Usage", "Mode"}, new int[] {80, 20});
            foreach (AccessToVariable access in usages)
            {
                AddRow(access.Source.FullName, access.AccessMode.ToString());
            }

            CloseSubParagraph();
        }

        /// <summary>
        ///     Provides a string enumerating all the requirements of the given list of requirements
        /// </summary>
        /// <param name="requirements">List of requirements</param>
        /// <returns></returns>
        private static string GetRequirementsAsString(ArrayList requirements)
        {
            string retVal = "";

            if (requirements.Count > 0)
            {
                bool first = true;
                foreach (ReqRef reqRef in requirements)
                {
                    if (first)
                    {
                        retVal += reqRef.Name;
                    }
                    else
                    {
                        retVal += ", " + reqRef.Name;
                    }
                    first = false;
                }
            }
            else
            {
                retVal += "No requirements related to this element";
            }

            return retVal;
        }
    }
}