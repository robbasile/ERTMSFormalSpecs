using System.Collections;
using System.Collections.Generic;
using DataDictionary;
using Utils;

namespace GUI.NavigationView
{
    public class NamableWrapper : IGraphicalDisplay
    {
        /// <summary>
        /// The model element referenced by this wrapper
        /// </summary>
        public INamable Namable { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="namable"></param>
        public NamableWrapper(INamable namable)
        {
            Namable = namable;
        }

        /// <summary>
        /// The enclosing element
        /// </summary>
        public object Enclosing
        {
            get
            {
                object retVal = null;

                IEnclosed enclosed = Namable as IEnclosed;
                if (enclosed != null)
                {
                    retVal = enclosed.Enclosing;
                }

                return retVal;
            }
            set
            {
                IEnclosed enclosed = Namable as IEnclosed;
                if (enclosed != null)
                {
                    enclosed.Enclosing = value;
                }

            }
        }

        /// <summary>
        /// The name to display
        /// </summary>
        public string Name 
        {
            get { return Namable.Name; }
            set { Namable.Name = value; }
        }

        /// <summary>
        /// The fullname
        /// </summary>
        public string FullName
        {
            get { return Namable.FullName; }             
        }

        public int CompareTo(IModelElement other)
        {
            return System.String.Compare(Namable.FullName, other.FullName, System.StringComparison.Ordinal);
        }

        public ArrayList SubElements { get; private set; }
        public ArrayList EnclosingCollection { get; private set; }
        public void Delete()
        {
        }

        public string ExpressionText { get; set; }
        public List<ElementLog> Messages { get; private set; }
        public void ClearMessages(bool precise)
        {
        }

        public MessageInfoEnum MessagePathInfo { get; private set; }
        public bool IsRemoved { get; private set; }
        public void AddModelElement(IModelElement element)
        {
        }

        public bool IsParent(IModelElement element)
        {
            return false;
        }

        public void ClearCache()
        {
        }

        public void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Guid { get; private set; }
        public string GraphicalName { get { return Namable.Name; }}
        public bool Hidden { get; set; }
        public bool Pinned { get; set; }

        /// <summary>
        /// Indicates that the model is expanded
        /// </summary>
        public ExpandableEnum Expanded { get; set; }

    }
}
