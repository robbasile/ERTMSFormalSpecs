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
using DataDictionary.Tests;
using DataDictionary.Tests.Runner;
using DataDictionary.Tests.Runner.Events;
using Utils;
using Util = DataDictionary.Util;

namespace EFSTester
{
    internal class Program
    {
        /// <summary>
        ///     Perform all functional tests defined in the .EFS file provided
        /// </summary>
        /// <param name="args"></param>
        /// <returns>the error code of the program</returns>
        private static int Main(string[] args)
        {
            int retVal = 0;

            EfsSystem efsSystem = EfsSystem.Instance;
            try
            {
                Console.Out.WriteLine("EFS Tester");

                // Load the dictionaries provided as parameters
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

                // Translate the sub sequences, if required
                Console.Out.WriteLine("Translating sub sequences");
                foreach (Dictionary dictionary in efsSystem.Dictionaries)
                {
                    foreach (Frame frame in dictionary.Tests)
                    {
                        foreach (SubSequence subSequence in frame.SubSequences)
                        {
                            if (subSequence.getCompleted())
                            {
                                if (dictionary.TranslationDictionary != null)
                                {
                                    subSequence.Translate(dictionary.TranslationDictionary);
                                }
                            }
                        }
                    }
                }

                // Make sure everything is recompiled
                Console.Out.WriteLine("Recompiling everything");
                efsSystem.Compiler.Compile_Synchronous(true, false);

                // Ensure the model is consistent
                Console.Out.WriteLine("Checking model");
                foreach (Dictionary dictionary in efsSystem.Dictionaries)
                {
                    RuleCheckerVisitor checker = new RuleCheckerVisitor(dictionary);
                    checker.visit(dictionary);
                }
 
                // Dumps all errors found
                Util.IsThereAnyError isThereAnyError = new Util.IsThereAnyError();
                if (isThereAnyError.ErrorsFound.Count > 0)
                {
                    foreach (ElementLog error in isThereAnyError.ErrorsFound)
                    {
                        Console.Out.WriteLine(error.Log);
                    }
                    return -1;
                }

                {
                    // Perform functional test for last loaded dictionary
                    Dictionary dictionary = efsSystem.Dictionaries.FindLast(x => true);
                    Console.Out.WriteLine("Processing tests from dictionary " + dictionary.Name);
                    foreach (Frame frame in dictionary.Tests)
                    {
                        Console.Out.WriteLine("Executing frame " + frame.FullName);
                        foreach (SubSequence subSequence in frame.SubSequences)
                        {
                            Console.Out.WriteLine("Executing sub sequence " + subSequence.FullName);
                            if (subSequence.getCompleted())
                            {
                                Runner runner = new Runner(subSequence, false, true);
                                runner.RunUntilStep(null);

                                bool failed = false;
                                foreach (ModelEvent evt in runner.FailedExpectations())
                                {
                                    Expect expect = evt as Expect;
                                    if (expect != null)
                                    {
                                        string message = expect.Message.Replace('\n', ' ');
                                        TestCase testCase = EnclosingFinder<TestCase>.find(expect.Expectation);
                                        if (testCase.ImplementationCompleted)
                                        {
                                            Console.Out.WriteLine(" failed (unexpected) :" + message);
                                            failed = true;
                                        }
                                        else
                                        {
                                            Console.Out.WriteLine(" failed (expected) : " + message);
                                        }
                                    }
                                    else
                                    {
                                        ModelInterpretationFailure modelInterpretationFailure =
                                            evt as ModelInterpretationFailure;
                                        if (modelInterpretationFailure != null)
                                        {
                                            Console.Out.WriteLine(" failed : " + modelInterpretationFailure.Message);
                                            failed = true;
                                        }
                                    }
                                }

                                if (failed)
                                {
                                    Console.Out.WriteLine("  -> Failed");
                                    retVal = -1;
                                }
                                else
                                {
                                    Console.Out.WriteLine("  -> Success");
                                }
                            }
                            else
                            {
                                Console.Out.WriteLine("  -> Not executed because it is not marked as completed");
                            }
                        }
                    }
                }
            }
            finally
            {
                Util.UnlockAllFiles();
                efsSystem.Stop();
            }

            return retVal;
        }
    }
}