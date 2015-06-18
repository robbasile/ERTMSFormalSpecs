using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataDictionary.Specification;

namespace CodeGenerator
{
    /// <summary>
    /// Generates the code according to the efs file provided
    /// </summary>
    class Program
    {
        /// <summary>
        /// Displays the help for this program
        /// </summary>
        private static void displayHelp()
        {
            System.Console.Out.WriteLine("HTML Generator usage");
            System.Console.Out.WriteLine("  HTMLGenerator <dictionary.efs> <outputpath>");
            System.Console.Out.WriteLine("");
            System.Console.Out.WriteLine("where ");
            System.Console.Out.WriteLine("  dictionary.efs  is the EFS file used to generate code");
            System.Console.Out.WriteLine("  outputpath      is the directory where the HTML files shall be placed");
        }

        private const int PARAM_COUNT = 2;
        private const int DICTIONARY_FILE = 0;
        private const int OUTPUT_PATH = 1;

        /// <summary>
        /// The main program
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int Main(string[] args)
        {
            int retVal = 0;

            // We do not want auto compilation of the system
            DataDictionary.EFSSystem system = DataDictionary.EFSSystem.INSTANCE;
            system.Stop();

            // Check parameters
            if (args.Count() != PARAM_COUNT)
            {
                System.Console.Error.WriteLine("Incorrect number of parameters, found " + args.Count() + ", expected " + PARAM_COUNT);

                displayHelp();
                retVal = -1;
            }

            // Load dictionary
            DataDictionary.Dictionary dictionary = null;
            if (retVal == 0)
            {
                bool lockFiles = false;
                bool updateGuid = false;
                dictionary = DataDictionary.Util.load(args[DICTIONARY_FILE], system, lockFiles, null, updateGuid);
                if (dictionary == null)
                {
                    System.Console.Error.WriteLine("Cannot read dictionary file " + args[DICTIONARY_FILE]);
                    retVal = -1;
                }
            }

            // Generates the HTML pages
            if (retVal == 0)
            {
                HTMLGenerator generator = new HTMLGenerator(args[OUTPUT_PATH]);
                generator.GenerateHTML(dictionary);
            }

            return retVal;
        }
    }
}
