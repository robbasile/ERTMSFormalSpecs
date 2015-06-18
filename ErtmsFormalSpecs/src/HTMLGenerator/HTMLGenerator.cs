namespace CodeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This class generates HTML pages based on a dictionary
    /// </summary>
    public class HTMLGenerator : DataDictionary.Generated.Visitor
    {
        /// <summary>
        /// The location where the HTML files will be placed
        /// </summary>
        private string BasePath { get; set; }

        /// <summary>
        /// The writer used to output the code
        /// </summary>
        public System.IO.StreamWriter Writer { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basePath"></param>
        public HTMLGenerator(string basePath)
        {
            BasePath = basePath;
        }

        /// <summary>
        /// Provides a valid file name for the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string ValidName(string name)
        {
            string retVal = name;

            retVal = retVal.Replace(".", "_");

            return retVal;
        }

        /// <summary>
        /// Generates the code for a single specification paragraph
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="visitSubNodes"></param>
        public override void visit(DataDictionary.Generated.Paragraph obj, bool visitSubNodes)
        {
            DataDictionary.Specification.Paragraph paragraph = (DataDictionary.Specification.Paragraph)obj;

            Writer = new System.IO.StreamWriter(BasePath + System.IO.Path.DirectorySeparatorChar + ValidName(paragraph.getId()) + ".html");
            Writer.WriteLine("Paragraph " + paragraph.Name);
            Writer.WriteLine("Scope = " + paragraph.getScope());
            Writer.Close();

            base.visit(obj, visitSubNodes);
        }

        /// <summary>
        /// Generates the C code for the corresponding dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        public void GenerateHTML(DataDictionary.Dictionary dictionary)
        {
            visit(dictionary, true);
        }
    }
}
