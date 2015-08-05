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
using System.IO;
using DataDictionary;
using DataDictionary.Generated;
using Collection = DataDictionary.Types.Collection;
using Dictionary = DataDictionary.Dictionary;
using Range = DataDictionary.Types.Range;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;
using Type = DataDictionary.Types.Type;
using Variable = DataDictionary.Variables.Variable;

namespace EFSInterface
{
    internal class Program
    {
        // Visits the model and determine the variables used by the system
        private class VariableVisitor : Visitor
        {
            /// <summary>
            ///     Used to write the contents of the interface
            /// </summary>
            public TextWriter Writer { get; private set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="writer"></param>
            public VariableVisitor(TextWriter writer)
                : base()
            {
                Writer = writer;
            }

            private void displayIndent(int indent)
            {
                for (int i = 0; i < indent; i++)
                {
                    Writer.Write(" ");
                }
            }

            private void displayType(int indent, Type type)
            {
                Structure structureType = type as Structure;
                if (structureType != null)
                {
                    foreach (StructureElement element in structureType.Elements)
                    {
                        displayIndent(indent);
                        Type subType = element.Type;
                        Writer.WriteLine(element.Name + " : " + subType.FullName);
                        displayType(indent + 1, subType);
                    }
                }

                Range rangeType = type as Range;
                if (rangeType != null)
                {
                    displayIndent(indent);
                    Writer.WriteLine("RANGE " + rangeType.MinValueAsDouble + ".." + rangeType.MaxValueAsDouble +
                                     " DEFAULT VALUE = " + rangeType.DefaultValue.LiteralName);
                }

                Collection collectionType = type as Collection;
                if (collectionType != null)
                {
                    displayIndent(indent);
                    Type subType = collectionType.Type;
                    Writer.WriteLine("COLLECTION [" + collectionType.getMaxSize() + "] OF " + subType.FullName);
                    displayType(indent + 1, subType);
                }
            }

            private void displayVariable(Variable var)
            {
                switch (var.Mode)
                {
                    case acceptor.VariableModeEnumType.aIncoming:
                        Writer.Write("IN     ");
                        break;

                    case acceptor.VariableModeEnumType.aInOut:
                        Writer.Write("IN OUT ");
                        break;

                    case acceptor.VariableModeEnumType.aOutgoing:
                        Writer.Write("   OUT ");
                        break;

                    default:
                        return;
                }

                Type type = var.Type;
                Writer.WriteLine(var.FullName + " : " + type.FullName);

                displayType(1, type);
            }

            // Visits a variable declaration
            public override void visit(DataDictionary.Generated.Variable obj, bool visitSubNodes)
            {
                Variable variable = (Variable) obj;

                switch (variable.Mode)
                {
                    case acceptor.VariableModeEnumType.aIncoming:
                        displayVariable(variable);
                        break;

                    case acceptor.VariableModeEnumType.aInOut:
                        displayVariable(variable);
                        break;

                    case acceptor.VariableModeEnumType.aOutgoing:
                        displayVariable(variable);
                        break;

                    case acceptor.VariableModeEnumType.aConstant:
                        break;

                    case acceptor.VariableModeEnumType.aInternal:
                        break;

                    default:
                        break;
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Creates the interfaces of the the EFS model
        /// </summary>
        /// <param name="args"></param>
        /// <returns>the error code of the program</returns>
        private static int Main(string[] args)
        {
            int retVal = 0;

            try
            {
                Console.Out.WriteLine("EFS Interfaces");

                // Load the dictionaries provided as parameters
                EFSSystem efsSystem = EFSSystem.INSTANCE;
                Util.PleaseLockFiles = false;
                foreach (string arg in args)
                {
                    Console.Out.WriteLine("Loading dictionary " + arg);

                    Dictionary dictionary = Util.Load(efsSystem, new Util.LoadParams(arg)
                    {
                        LockFiles = false,
                        Errors = null,
                        UpdateGuid = false,
                        ConvertObsolete = false
                    });
                    if (dictionary == null)
                    {
                        Console.Out.WriteLine("Cannot load dictionary " + arg);
                        return -1;
                    }
                }

                TextWriter writer = new StreamWriter("Interface.out");
                VariableVisitor visitor = new VariableVisitor(writer);
                foreach (Dictionary dictionary in EFSSystem.INSTANCE.Dictionaries)
                {
                    visitor.visit(dictionary, true);
                }
            }
            finally
            {
                Util.UnlockAllFiles();
            }

            return retVal;
        }
    }
}