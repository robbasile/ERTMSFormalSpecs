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
using DataDictionary;
using DataDictionary.Generated;
using GUIUtils.LongOperations;
using Parameter = DataDictionary.Parameter;

namespace GUI.LongOperations
{
    public class ApplyNamingConventionsOperation: BaseLongOperation
    {
        /// <summary>
        ///     Visitor enforcing the naming convention
        /// </summary>
        private class CheckNamingVisitor : Visitor
        {
            /// <summary>
            ///     The desired conventions ofr the different data types
            /// </summary>
            private const string EnumSuffix = "Enum";
            private const string StructureSuffix = "Struct";
            private const string CollectionSuffix = "Col";
            private const string StateMachineSuffix = "SM";
            private const string InterfacePrefix = "I";
            private const string ParameterPrefix = "a";

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="model"></param>
            public CheckNamingVisitor(BaseModelElement model)
            {
                DataDictionary.Generated.Namable namable = model as DataDictionary.Generated.Namable;

                if (namable != null)
                {
                    visit(namable);
                }
                else
                {
                    visit(model);
                }

            }


            /// <summary>
            ///     Takes the name of an element and a suffix 
            ///     if the name does not end with the desired suffix, adds it and then returns the name
            /// </summary>
            /// <param name="name"></param>
            /// <param name="suffix"></param>
            /// <returns></returns>
            private string EnsureSuffix(string name, string suffix)
            {
                string retVal = name;

                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!retVal.EndsWith(suffix))
                    {
                        retVal += suffix;
                    }
                }

                return retVal;
            }

            /// <summary>
            ///     Takes the name of an element and a prefix 
            ///     if the name does not start with the desired prefix, adds it and then returns the name
            /// </summary>
            /// <param name="name"></param>
            /// <param name="prefix"></param>
            /// <returns></returns>
            private string EnsurePrefix(string name, string prefix)
            {

                string retVal = name;

                if (!string.IsNullOrEmpty(prefix))
                {
                    if (!retVal.StartsWith(prefix))
                    {
                        retVal = prefix + retVal;
                    }
                }

                return retVal;
            }

            /// <summary>
            ///     Take the name of a step
            ///     If the name begins with "Step #", removes this and returns only the description of the step
            /// </summary>
            /// <param name="stepName"></param>
            /// <returns></returns>
            private string RemoveStepNumber(string stepName)
            {
                string retVal = stepName;

                if (!string.IsNullOrEmpty(retVal))
                {
                    if (retVal.StartsWith("Step") || retVal.StartsWith("step"))
                    {
                        //  Remove the "Step " substring at the start of the step's name
                        //  This converts step names of the type
                        //      Step 2 - Step description   or
                        //      Step 2: Step description
                        //  to
                        //      2 - Step description        or
                        //      2: Step description

                        retVal = retVal.Substring(4);


                        //  Iterate through the characters in the step name to get the index of the first letter
                        //  then remove all of the string before that index, getting
                        //      Step description
                        int indexOfFirstChar = 0;
                        for (int i = 0; i < retVal.Length; i++)
                        {
                            if (Char.IsLetter(retVal[i]))
                            {
                                indexOfFirstChar = i;
                                break;
                            }

                            // If no letters are found, set retVal to the empty string
                            // Since the original name of the step was just "Step #"
                            if (i == retVal.Length - 1)
                            {
                                indexOfFirstChar = i;
                            }
                        }

                        retVal = retVal.Substring(indexOfFirstChar);
                    }
                }

                return retVal;
            }

            /// <summary>
            ///     Updates the names of model elements to ensure that they match the defined naming convention
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(DataDictionary.Generated.Namable obj, bool visitSubNodes)
            {
                DataDictionary.Namable namable = (DataDictionary.Namable)obj;
                string newName = "";
                if (namable is Parameter)
                {
                    newName = EnsurePrefix(namable.Name, ParameterPrefix);
                }
                else if (namable is DataDictionary.Types.Structure)
                {
                    DataDictionary.Types.Structure structure = (DataDictionary.Types.Structure)namable;
                    if (structure.IsAbstract)
                    {
                        newName = EnsurePrefix(namable.Name, InterfacePrefix);
                    }
                    else
                    {
                        newName = EnsureSuffix(namable.Name, StructureSuffix);
                    }
                }
                else if (namable is DataDictionary.Types.Enum)
                {
                    newName = EnsureSuffix(namable.Name, EnumSuffix);
                }
                else if (namable is DataDictionary.Types.Collection)
                {
                    newName = EnsureSuffix(namable.Name, CollectionSuffix);
                }
                else if (namable is DataDictionary.Types.StateMachine)
                {
                    if (namable.Enclosing is DataDictionary.Types.NameSpace)
                    {
                        newName = EnsureSuffix(namable.Name, StateMachineSuffix);
                    }
                }
                else if (namable is DataDictionary.Tests.Step)
                {
                    newName = RemoveStepNumber(namable.Name);
                }

                if (!string.IsNullOrEmpty(newName))
                {
                    EfsSystem.Instance.Compiler.Refactor(obj, newName);
                }

                base.visit(obj, visitSubNodes);
            }
        }

        private readonly BaseModelElement _model;

        public ApplyNamingConventionsOperation(BaseModelElement model)
        {
            _model = model;
        }

        public override void ExecuteWork()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CheckNamingVisitor(_model);
        }
    }
}
