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
using System.Reflection;
using DataDictionary;
using DataDictionary.Generated;
using log4net;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Case = DataDictionary.Functions.Case;
using Collection = DataDictionary.Types.Collection;
using Enum = DataDictionary.Types.Enum;
using EnumValue = DataDictionary.Constants.EnumValue;
using Function = DataDictionary.Functions.Function;
using Parameter = DataDictionary.Parameter;
using PreCondition = DataDictionary.Rules.PreCondition;
using Procedure = DataDictionary.Functions.Procedure;
using Range = DataDictionary.Types.Range;
using ReqRef = DataDictionary.ReqRef;
using ReqRelated = DataDictionary.ReqRelated;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using State = DataDictionary.Constants.State;
using StateMachine = DataDictionary.Types.StateMachine;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;
using Variable = DataDictionary.Variables.Variable;

namespace Reports.Model
{
    public class ModelReport : ReportTools
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Indicates if only the implemented model elements should be included to the report
        /// </summary>
        private bool implementedOnly;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="document"></param>
        public ModelReport(Document document, bool ImplementedOnly)
            : base(document)
        {
            this.implementedOnly = ImplementedOnly;
        }

        /// <summary>
        ///     Counts the number of req related in a list which implementation is partially completed
        /// </summary>
        /// <param name="list">The list of req related to consider</param>
        /// <returns></returns>
        public int CountDisplayedReqRelated(ArrayList list)
        {
            int retVal = 0;

            foreach (ReqRelated reqRelated in list)
            {
                if (considerReqRelated(reqRelated))
                {
                    retVal += 1;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Counts the number of req related in a list which implementation is partially completed
        /// </summary>
        /// <param name="list">The list of req related to consider</param>
        /// <param name="inOutFilter">Consider only IN and OUT variables</param>
        /// <returns></returns>
        public int CountDisplayedVariables(ArrayList list, bool inOutOnly)
        {
            int retVal = 0;

            foreach (Variable variable in list)
            {
                if (considerVariable(variable, inOutOnly))
                {
                    retVal += 1;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a section for all the ranges of the given namespace
        /// </summary>
        /// <param name="ranges">The list of ranges to add</param>
        /// <param name="addDetails">Add details or simply enumerate the ranges</param>
        public void CreateRangesSection(ArrayList ranges, bool addDetails)
        {
            AddSubParagraph("Ranges");
            foreach (Range range in ranges)
            {
                if (considerReqRelated(range))
                {
                    if (addDetails)
                    {
                        AddSubParagraph(range.Name);
                        AddTable(new string[] {"Range " + range.Name}, new int[] {30, 40, 70});
                        if (range.Comment != "")
                        {
                            AddRow(range.Comment);
                        }
                        AddRow("Values", range.MinValue + ".." + range.MaxValue);
                        AddRow("Defaul value", range.Default);
                        AddRow("Precision", range.getPrecision_AsString());

                        if (range.SpecialValues.Count > 0)
                        {
                            AddRow("Special values", "Name", "Value");
                            Row firstRow = lastRow;
                            firstRow.Shading.Color = Colors.LightBlue;

                            foreach (EnumValue value in range.SpecialValues)
                            {
                                if (AddRow("", value.getValue().ToString(), value.Name) != null)
                                {
                                    firstRow.Cells[0].MergeDown += 1;
                                }
                                else
                                {
                                    Log.Error("Error: tried to add an empty row in the model report");
                                }
                            }
                        }
                        CreateStatusTable(range);

                        CloseSubParagraph();
                    }
                    else
                    {
                        AddParagraph(range.Name + " (" + GetRequirementsAsString(range.Requirements) + ")");
                    }
                }
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates a section for all the enums and sub-enums of the given namespace
        /// </summary>
        /// <param name="enumerations">The list of enumerations to add</param>
        /// <param name="addDetails">Add details or simply enumerate the enums</param>
        public void CreateEnumerationsSection(ArrayList enumerations, bool addDetails)
        {
            AddSubParagraph("Enumerations");
            foreach (Enum anEnum in enumerations)
            {
                if (considerReqRelated(anEnum))
                {
                    AddEnumerationSection(anEnum, addDetails);
                }
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Adds information about an enum (or sub-enum of an enum)
        /// </summary>
        /// <param name="anEnum">The enum to add</param>
        /// <param name="addDetails">Add details or simply enumerate the enums</param>
        private void AddEnumerationSection(Enum anEnum, bool addDetails)
        {
            if (addDetails)
            {
                AddSubParagraph(anEnum.Name);

                AddTable(new string[] {"Enumeration " + anEnum.Name}, new int[] {40, 20, 80});
                if (anEnum.Comment != "")
                {
                    AddRow(anEnum.Comment);
                }
                AddTableHeader("Name", "Value", "Comment");
                if (anEnum.Values.Count > 0)
                {
                    foreach (EnumValue value in anEnum.Values)
                    {
                        if (value.getName().Equals(anEnum.Default))
                        {
                            AddRow(value.Name, value.getValue().ToString(), "Default value");
                        }
                        else
                        {
                            AddRow(value.Name, value.getValue().ToString(), "");
                        }
                    }
                }

                CreateStatusTable(anEnum);

                foreach (Enum subEnum in anEnum.SubEnums)
                {
                    if (considerReqRelated(subEnum))
                    {
                        AddEnumerationSection(subEnum, addDetails);
                    }
                }
                CloseSubParagraph();
            }
            else
            {
                AddParagraph(anEnum.Name + " (" + GetRequirementsAsString(anEnum.Requirements) + ")");
            }
        }


        /// <summary>
        ///     Creates a section for all the structures of the given namespace
        /// </summary>
        /// <param name="structures">The list of structures to add</param>
        /// <param name="addDetails">Add details or simply enumerate the structures</param>
        public void CreateStructuresSection(ArrayList structures, bool addDetails)
        {
            AddSubParagraph("Structures");
            foreach (Structure structure in structures)
            {
                if (considerReqRelated(structure))
                {
                    if (addDetails)
                    {
                        AddSubParagraph(structure.Name);
                        if (structure.Comment != "")
                        {
                            AddParagraph(structure.Comment);
                        }

                        AddTable(new string[] {"Structure " + structure.Name}, new int[] {35, 15, 40, 50});
                        AddTableHeader("Sub element name", "Mode", "Type", "Comment");
                        foreach (StructureElement element in structure.Elements)
                        {
                            AddRow(element.Name, element.getMode_AsString(), element.TypeName, element.Comment);
                        }

                        if (CountDisplayedReqRelated(structure.Rules) > 0)
                        {
                            CreateRulesSection(structure.Rules, addDetails, true);
                        }

                        if (CountDisplayedReqRelated(structure.Procedures) > 0)
                        {
                            CreateProceduresSection(structure.Procedures, addDetails, true);
                        }

                        CreateStatusTable(structure);
                        CloseSubParagraph();
                    }
                    else
                    {
                        AddParagraph(structure.Name + " (" + GetRequirementsAsString(structure.Requirements) + ")");
                    }
                }
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates a section for all the collections of the given namespace
        /// </summary>
        /// <param name="collections">The list of collections to add</param>
        /// <param name="addDetails">Add details or simply enumerate the collections</param>
        /// <returns></returns>
        public void CreateCollectionsSection(ArrayList collections, bool addDetails)
        {
            AddSubParagraph("Collections");
            foreach (Collection collection in collections)
            {
                if (considerReqRelated(collection))
                {
                    if (addDetails)
                    {
                        AddSubParagraph(collection.Name);
                        AddTable(new string[] {"Collection " + collection.Name}, new int[] {40, 100});
                        if (collection.Comment != "")
                        {
                            AddRow(collection.Comment);
                        }
                        AddRow("Type", collection.getTypeName());
                        AddRow("Default value", collection.Default);
                        AddRow("Max size", collection.getMaxSize().ToString());
                        CreateStatusTable(collection);
                        CloseSubParagraph();
                    }
                    else
                    {
                        AddParagraph(collection.Name + " (" + GetRequirementsAsString(collection.Requirements) + ")");
                    }
                }
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates a section for all the state machines of the given namespace
        /// </summary>
        /// <param name="stateMachines">The list of state machines to add</param>
        /// <param name="addDetails">Add details or simply enumerate the state machines</param>
        /// <returns></returns>
        public void CreateStateMachinesSection(ArrayList stateMachines, bool addDetails)
        {
            AddSubParagraph("State machines");
            foreach (StateMachine stateMachine in stateMachines)
            {
                if (considerReqRelated(stateMachine))
                {
                    AddStateMachineSection(stateMachine, addDetails);
                }
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Adds information about a state machine
        /// </summary>
        /// <param name="aSM">The state machine</param>
        /// <param name="addDetails">Add details or simply enumerate the state machines</param>
        private void AddStateMachineSection(StateMachine aSM, bool addDetails)
        {
            string name = aSM.FullName;
            if (name == "" && aSM.EnclosingState != null)
            {
                name = aSM.EnclosingState.FullName;
            }
            AddSubParagraph(name);

            AddTable(new string[] {"State machine " + name}, new int[] {20, 40, 80});
            AddRow(aSM.Comment);
            if (aSM.States.Count > 0)
            {
                AddRow("States", "Name", "Comment");
                Row firstRow = lastRow;
                firstRow.Shading.Color = Colors.LightBlue;

                foreach (State state in aSM.States)
                {
                    string comment = "";
                    if (aSM.Default.Equals(state.Name))
                    {
                        comment = "Initial state";
                    }
                    if (AddRow("", state.Name, comment) != null)
                    {
                        firstRow.Cells[0].MergeDown += 1;
                    }
                    else
                    {
                        Log.Error("Error: tried to add an empty row in model report");
                    }
                }
            }

            if (CountDisplayedReqRelated(aSM.Rules) > 0)
            {
                CreateRulesSection(aSM.Rules, addDetails, true);
            }
            CreateStatusTable(aSM);
            CloseSubParagraph();

            foreach (State state in aSM.States)
            {
                if (state.StateMachine != null && considerReqRelated(state.StateMachine))
                {
                    AddStateMachineSection(state.StateMachine, addDetails);
                }
            }
        }

        /// <summary>
        ///     Creates a section for all the functions of the given namespace
        /// </summary>
        /// <param name="functions">The list of functions to add</param>
        /// <param name="addDetails">Add details or simply enumerate the collections</param>
        /// <returns></returns>
        public void CreateFunctionsSection(ArrayList functions, bool addDetails)
        {
            AddSubParagraph("Functions");
            foreach (Function function in functions)
            {
                if (considerReqRelated(function))
                {
                    if (addDetails)
                    {
                        AddSubParagraph(function.Name);
                        CreateParameters("Function " + function.Name, function.Comment, function.FormalParameters,
                            function.TypeName);

                        if (function.Cases.Count > 0)
                        {
                            AddTable(new string[] {"Behaviour"}, new int[] {70, 70});
                            AddTableHeader("Condition", "Value");
                            foreach (Case cas in function.Cases)
                            {
                                Row firstRow = null;
                                foreach (PreCondition preCondition in cas.PreConditions)
                                {
                                    if (firstRow == null)
                                    {
                                        AddRow(preCondition.Condition, cas.Expression.ToString());
                                        firstRow = lastRow;
                                    }
                                    else
                                    {
                                        AddRow(preCondition.Condition);
                                        firstRow.Cells[1].MergeDown += 1;
                                    }
                                }

                                if (firstRow == null)
                                {
                                    AddRow("", cas.Expression.ToString());
                                }
                            }
                        }
                        CreateStatusTable(function);
                        CloseSubParagraph();
                    }
                    else
                    {
                        AddParagraph(function.Name + " (" + GetRequirementsAsString(function.Requirements) + ")");
                    }
                }
            }
            CloseSubParagraph();
        }

        /// <summary>
        ///     Creates a section for all the procedures of the given namespace
        /// </summary>
        /// <param name="procedures">The list of procedures to add</param>
        /// <param name="addDetails">Add de tails or simply enumerate the procedures</param>
        /// <param name="addToExistingTable">Add information to an existing table or create a separate section</param>
        /// <returns></returns>
        public void CreateProceduresSection(ArrayList procedures, bool addDetails, bool addToExistingTable)
        {
            if (!addToExistingTable)
            {
                AddSubParagraph("Procedures");
            }
            else
            {
                AddTableHeader("Procedures");
            }

            foreach (Procedure procedure in procedures)
            {
                if (considerReqRelated(procedure))
                {
                    if (addDetails)
                    {
                        if (!addToExistingTable)
                        {
                            AddSubParagraph(procedure.Name);
                        }
                        CreateParameters("Procedure " + procedure.Name, procedure.Comment, procedure.FormalParameters,
                            null);
                        if (CountDisplayedReqRelated(procedure.Rules) > 0)
                        {
                            CreateRulesSection(procedure.Rules, addDetails, true);
                        }

                        CreateStatusTable(procedure);
                        if (!addToExistingTable)
                        {
                            CloseSubParagraph();
                        }
                    }
                    else
                    {
                        AddParagraph(procedure.Name + " (" + GetRequirementsAsString(procedure.Requirements) + ")");
                    }
                }
            }
            if (!addToExistingTable)
            {
                CloseSubParagraph();
            }
        }

        /// <summary>
        ///     Creates a section for all the variables of the given namespace
        /// </summary>
        /// <param name="variables">The list of variables to add</param>
        /// <param name="addDetails">Add details or simply enumerate the variables</param>
        /// <param name="inOutOnly">Add only IN or OUT variables</param>
        /// <returns></returns>
        public void CreateVariablesSection(ArrayList variables, bool addDetails, bool inOutOnly)
        {
            AddSubParagraph("Variables");
            foreach (Variable variable in variables)
            {
                if (considerVariable(variable, inOutOnly))
                {
                    if (addDetails)
                    {
                        AddSubParagraph(variable.Name);
                        AddTable(new string[] {"Variable " + variable.Name}, new int[] {40, 100});
                        AddRow(variable.Comment);
                        AddRow("Type", variable.getTypeName());
                        AddRow("Default value", variable.Default);
                        AddRow("Mode", variable.getVariableMode_AsString());
                        CreateStatusTable(variable);
                        CloseSubParagraph();
                    }
                    else
                    {
                        AddParagraph(variable.Name + " (" + GetRequirementsAsString(variable.Requirements) + ")");
                    }
                }
            }
            CloseSubParagraph();
        }


        /// <summary>
        ///     Creates a section for all the rules of the given namespace
        /// </summary>
        /// <param name="rules">The list of rules to add</param>
        /// <param name="addDetails">Add details or simply enumerate the rules</param>
        /// <param name="addToExistingTable">Add information to an existing table or create a separate section</param>
        /// <returns></returns>
        public void CreateRulesSection(ArrayList rules, bool addDetails, bool addToExistingTable)
        {
            if (!addToExistingTable)
            {
                AddSubParagraph("Rules");
            }
            else
            {
                AddTableHeader("Rules");
            }
            foreach (Rule rule in rules)
            {
                if (considerReqRelated(rule))
                {
                    AddRuleSection(rule, addDetails, addToExistingTable);
                }
            }
            if (!addToExistingTable)
                CloseSubParagraph();
        }

        /// <summary>
        ///     Adds information about a rule (or sub-rule of a rule)
        /// </summary>
        /// <param name="aRule">The rule to add</param>
        /// <param name="addDetails">Add details or simply enumerate the enums</param>
        /// <param name="addToExistingTable">Add information to an existing table or create a separate section</param>
        private void AddRuleSection(Rule aRule, bool addDetails, bool addToExitingTable)
        {
            if (addDetails)
            {
                if (!addToExitingTable)
                {
                    AddSubParagraph(aRule.Name);
                }
                AddRuleRow(aRule, addDetails);
                if (!addToExitingTable)
                {
                    CloseSubParagraph();
                }
            }
            else
            {
                AddParagraph(aRule.Name + " (" + GetRequirementsAsString(aRule.Requirements) + ")");
            }
        }

        /// <summary>
        ///     Adds a row for a rule (or sub-rule of a rule)
        /// </summary>
        /// <param name="aRule">The rule to add</param>
        /// <param name="addDetails">Add details or simply enumerate the enums</param>
        private void AddRuleRow(Rule aRule, bool addDetails)
        {
            if (addDetails)
            {
                if (aRule.EnclosingRuleCondition != null)
                {
                    AddTable(
                        new string[]
                        {
                            "Sub-rule " + aRule.Name + " (parent rule condition : " + aRule.EnclosingRuleCondition.Name +
                            ")"
                        }, new int[] {40, 100});
                }
                else
                {
                    AddTable(new string[] {"Rule " + aRule.Name}, new int[] {40, 100});
                }
                AddRow(aRule.Comment);
                AddRow("Activation priority", aRule.getPriority_AsString());

                TextualExplanation explanation = new TextualExplanation();
                aRule.GetExplain(explanation, false);
                AddRow(explanation.Text);
            }
            else
            {
                AddRow(aRule.Name + " (" + GetRequirementsAsString(aRule.Requirements) + ")");
            }

            foreach (RuleCondition ruleCondition in aRule.RuleConditions)
            {
                if (CountDisplayedReqRelated(ruleCondition.SubRules) > 0)
                {
                    AddTableHeader("Sub-rules of " + ruleCondition.Name);
                    foreach (Rule subRule in ruleCondition.SubRules)
                    {
                        if (considerReqRelated(subRule))
                        {
                            AddRuleRow(subRule, addDetails);
                        }
                    }
                }
            }
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


        /// <summary>
        ///     Provides a list enumerating all the parameters of the given list of parameters
        /// </summary>
        /// <param name="parameters">The list of parameters</param>
        /// <returns></returns>
        private void CreateParameters(string name, string comment, ArrayList parameters, string returnValue)
        {
            AddTable(new string[] {name}, new int[] {40, 80});
            if (comment != "")
            {
                AddRow(comment);
            }
            if (parameters.Count > 0)
            {
                AddTableHeader("Parameters");
                AddTableHeader("Name", "Type");
                foreach (Parameter parameter in parameters)
                {
                    AddRow(parameter.Name, parameter.getTypeName());
                }
            }
            if (returnValue != null)
            {
                AddTableHeader("Return value");
                AddRow(returnValue);
            }
        }

        /// <summary>
        ///     Creates a containing the implementation/verification status and the
        ///     requirements of a ReqRelated element
        /// </summary>
        /// <param name="aReqRelated">The ReqRelated element</param>
        /// <returns></returns>
        private void CreateStatusTable(ReqRelated aReqRelated)
        {
            AddTable(new string[] {"Modeling information"}, new int[] {40, 30, 70});

            string implemented = "not implemented";
            string verified = "not verified";
            if (aReqRelated.getImplemented())
            {
                implemented = "implemented";
            }
            if (aReqRelated.getVerified())
            {
                verified = "verified";
            }

            AddRow("Status", implemented, verified);
            AddRow("Requirements", GetRequirementsAsString(aReqRelated.Requirements));
        }

        /// <summary>
        ///     Indicates if a req related should be considered for the report
        /// </summary>
        /// <param name="aReqRelated"></param>
        /// <returns></returns>
        private bool considerReqRelated(ReqRelated aReqRelated)
        {
            return aReqRelated.ImplementationCompleted || !implementedOnly;
        }

        /// <summary>
        ///     Indicates if a variable should be considered for the report
        /// </summary>
        /// <param name="aVariable"></param>
        /// <param name="inOutOnly"></param>
        /// <returns></returns>
        private bool considerVariable(Variable aVariable, bool inOutOnly)
        {
            return (aVariable.ImplementationPartiallyCompleted || !implementedOnly) &&
                   (!inOutOnly || aVariable.Mode == acceptor.VariableModeEnumType.aIncoming ||
                    aVariable.Mode == acceptor.VariableModeEnumType.aOutgoing ||
                    aVariable.Mode == acceptor.VariableModeEnumType.aInOut);
        }
    }
}