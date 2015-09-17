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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using XmlBooster;

namespace Utils
{
    /// <summary>
    ///     Enumeration indicating the status of the current model element
    ///     According to the messages stored in this element and the sub elements
    /// </summary>
    [Flags]
    public enum MessageInfoEnum
    {
        NoMessage = 0,
        PathToInfo = 1,
        Info = 2,
        PathToWarning = 4,
        Warning = 16,
        PathToError = 32,
        Error = 64
    }

    public interface IEnclosed
    {
        /// <summary>
        ///     The enclosing model element
        /// </summary>
        object Enclosing { get; set; }
    }

    /// <summary>
    ///     Deletes this element from its enclosing node
    /// </summary>
    public interface IModelElement : IEnclosed, INamable, IComparable<IModelElement>
    {
        /// <summary>
        ///     The sub elements of this model element
        /// </summary>
        ArrayList SubElements { get; }

        /// <summary>
        ///     Provides the collection which holds this instance
        /// </summary>
        ArrayList EnclosingCollection { get; }

        /// <summary>
        ///     Deletes the element from its enclosing node
        /// </summary>
        void Delete();

        /// <summary>
        ///     The expression text data of this model element
        /// </summary>
        string ExpressionText { get; set; }

        /// <summary>
        ///     The messages logged on the model element
        /// </summary>
        List<ElementLog> Messages { get; }

        /// <summary>
        ///     Clears the messages associated to this model element
        /// </summary>
        /// <param name="precise">
        ///     Indicates that the MessagePathInfo should be recomputed precisely
        ///     according to the sub elements and should update the enclosing elements
        /// </param>
        void ClearMessages(bool precise);

        /// <summary>
        ///     Indicates if the element holds messages, or is part of a path to a message
        /// </summary>
        MessageInfoEnum MessagePathInfo { get; }

        /// <summary>
        ///     Indicates whether the model element is removed
        /// </summary>
        bool IsRemoved { get; }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        void AddModelElement(IModelElement element);

        /// <summary>
        ///     Indicates whether this is a parent of the element.
        ///     It also returns true then parent==element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool IsParent(IModelElement element);
    }

    public abstract class ModelElement : XmlBBase, IModelElement
    {
        /// <summary>
        ///     The Logger
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Constructor
        /// </summary>
        protected ModelElement()
        {
            Messages = new List<ElementLog>();
        }

        /// <summary>
        ///     The model element name
        /// </summary>
        public virtual string Name { get; set; }

        public virtual string FullName
        {
            get { return Name; }
        }

        /// <summary>
        ///     The enclosing model element
        /// </summary>
        private object _enclosing;

        /// <summary>
        ///     The enclosing model element
        /// </summary>
        public virtual object Enclosing
        {
            get
            {
                if (_enclosing == null)
                {
                    return getFather();
                }
                return _enclosing;
            }
            set
            {
                IXmlBBase val = value as IXmlBBase;
                if (val != null)
                {
                    setFather(val);
                }
                else
                {
                    _enclosing = value;
                }
            }
        }

        /// <summary>
        ///     The sub elements of this model element
        /// </summary>
        public ArrayList SubElements
        {
            get
            {
                ArrayList list = new ArrayList();

                subElements(list);

                return list;
            }
        }

        /// <summary>
        ///     The comparer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(IModelElement other)
        {
            int retVal = -1;

            if (Name != null)
            {
                retVal = Name.CompareTo(other.Name);
            }
            else if (this == other)
            {
                retVal = 0;
            }

            return retVal;
        }

        /// <summary>
        ///     The collection in which this model element lies
        /// </summary>
        public virtual ArrayList EnclosingCollection
        {
            get { return null; }
        }

        /// <summary>
        ///     Deletes this model element from its enclosing collection
        /// </summary>
        public virtual void Delete()
        {
            if (EnclosingCollection != null)
            {
                EnclosingCollection.Remove(this);
                setFather(null);
            }
        }

        /// <summary>
        ///     Inserts this element after the element provided as parameter
        /// </summary>
        /// <param name="other">The element after which this element should be inserted</param>
        public virtual void InsertAfter(ModelElement other)
        {
            setFather(other.getFather());
            if (other.EnclosingCollection != null)
            {
                int index = other.EnclosingCollection.IndexOf(other);
                if (index >= 0)
                {
                    other.EnclosingCollection.Insert(index + 1, this);
                }
                else
                {
                    other.EnclosingCollection.Insert(0, this);
                }
            }
        }

        /// <summary>
        ///     The editable expression
        /// </summary>
        public virtual string ExpressionText
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///     Logs associated to this model element
        /// </summary>
        public virtual List<ElementLog> Messages { get; private set; }

        /// <summary>
        ///     Clears the messages associated to this model element
        /// </summary>
        /// <param name="precise">
        ///     Indicates that the MessagePathInfo should be recomputed precisely
        ///     according to the sub elements and should update the enclosing elements
        /// </param>
        public virtual void ClearMessages(bool precise)
        {
            UpdateMessageInfoAfterClear(precise);
        }

        /// <summary>
        ///     Removes the messages after a ClearMessages
        /// </summary>
        /// <param name="precise">
        ///     Indicates that the MessagePathInfo should be recomputed precisely
        ///     according to the sub elements and should update the enclosing elements
        /// </param>
        protected virtual void UpdateMessageInfoAfterClear(bool precise)
        {
            Messages.Clear();

            // Build back the message info path
            MessagePathInfo = MessageInfoEnum.NoMessage;
            if (precise)
            {
                foreach (ModelElement subElement in SubElements)
                {
                    if ((subElement.MessagePathInfo & (MessageInfoEnum.Error | MessageInfoEnum.PathToError)) != 0)
                    {
                        MessagePathInfo = MessagePathInfo | MessageInfoEnum.PathToError;
                    }
                    if ((subElement.MessagePathInfo & (MessageInfoEnum.Warning | MessageInfoEnum.PathToWarning)) != 0)
                    {
                        MessagePathInfo = MessagePathInfo | MessageInfoEnum.PathToWarning;
                    }
                    if ((subElement.MessagePathInfo & (MessageInfoEnum.Info | MessageInfoEnum.PathToInfo)) != 0)
                    {
                        MessagePathInfo = MessagePathInfo | MessageInfoEnum.PathToInfo;
                    }
                }

                ModelElement enclosing = Enclosing as ModelElement;
                if (enclosing != null)
                {
                    enclosing.UpdateMessageInfo(MessagePathInfo);
                }
            }
        }

        /// <summary>
        ///     Indicates whether the model element is removed
        /// </summary>
        public abstract bool IsRemoved { get; }

        /// <summary>
        ///     Adds a new element log attached to this model element
        /// </summary>
        /// <param name="log"></param>
        public virtual void AddElementLog(ElementLog log)
        {
            bool add = true;

            if (log.Level == ElementLog.LevelEnum.Error)
            {
                if (!log.FailedExpectation) // if this is a failed expectation, this is not a model error
                {
                    if (!Errors.ContainsKey(this))
                    {
                        Errors.Add(this, new List<ElementLog>());
                    }

                    List<ElementLog> list;
                    if (Errors.TryGetValue(this, out list))
                    {
                        list.Add(log);
                    }
                }
            }
            foreach (ElementLog other in Messages)
            {
                if (other.CompareTo(log) == 0)
                {
                    add = false;
                    break;
                }
            }

            if (add)
            {
                Messages.Add(log);
                switch (log.Level)
                {
                    case ElementLog.LevelEnum.Error:
                        UpdateMessageInfo(MessageInfoEnum.Error);
                        break;
                    case ElementLog.LevelEnum.Warning:
                        UpdateMessageInfo(MessageInfoEnum.Warning);
                        break;
                    case ElementLog.LevelEnum.Info:
                        UpdateMessageInfo(MessageInfoEnum.Info);
                        break;
                }
            }
        }

        /// <summary>
        ///     Updates the message info when a new message is added
        /// </summary>
        /// <param name="info"></param>
        protected void UpdateMessageInfo(MessageInfoEnum info)
        {
            if ((MessagePathInfo & info) == 0)
            {
                // Flag is not yet handled in this model element
                MessagePathInfo = MessagePathInfo | info;

                ModelElement enclosing = Enclosing as ModelElement;
                if (enclosing != null)
                {
                    MessageInfoEnum nextInfo = info;
                    switch (info)
                    {
                        case MessageInfoEnum.Error:
                            nextInfo = MessageInfoEnum.PathToError;
                            break;
                        case MessageInfoEnum.Warning:
                            nextInfo = MessageInfoEnum.PathToWarning;
                            break;
                        case MessageInfoEnum.Info:
                            nextInfo = MessageInfoEnum.PathToInfo;
                            break;
                    }
                    enclosing.UpdateMessageInfo(nextInfo);
                }
            }
        }

        /// <summary>
        ///     Adds an exception associated to this model element
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public ElementLog AddException(Exception exception)
        {
            ElementLog retVal = new ElementLog(ElementLog.LevelEnum.Error,
                "Exception raised : " + exception.Message + "\n\n" + exception.StackTrace);
            AddElementLog(retVal);
            return retVal;
        }

        /// <summary>
        ///     Adds an error associated to this model element
        /// </summary>
        /// <param name="message"></param>
        /// <param name="failedExpectation"></param>
        /// <returns></returns>
        public ElementLog AddError(string message, bool failedExpectation = false)
        {
            ElementLog retVal = new ElementLog(ElementLog.LevelEnum.Error, message);
            retVal.FailedExpectation = failedExpectation;
            AddElementLog(retVal);
            return retVal;
        }

        /// <summary>
        ///     Adds a warning associated to this model element
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ElementLog AddWarning(string message)
        {
            ElementLog retVal = new ElementLog(ElementLog.LevelEnum.Warning, message);
            AddElementLog(retVal);
            return retVal;
        }

        /// <summary>
        ///     Adds an informative message associated to this model element
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ElementLog AddInfo(string message)
        {
            ElementLog retVal = new ElementLog(ElementLog.LevelEnum.Info, message);
            AddElementLog(retVal);
            return retVal;
        }

        /// <summary>
        ///     Removes a log associated to a model element
        /// </summary>
        /// <param name="log"></param>
        public void RemoveLog(ElementLog log)
        {
            if (log != null)
            {
                Messages.Remove(log);
            }
        }

        /// <summary>
        ///     Indicates if the element holds messages, or is part of a path to a message
        /// </summary>
        public virtual MessageInfoEnum MessagePathInfo { get; set; }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public virtual void AddModelElement(IModelElement element)
        {
        }

        /// <summary>
        ///     Keeps track of the errors raised during the last cycle
        /// </summary>
        public static Dictionary<ModelElement, List<ElementLog>> Errors =
            new Dictionary<ModelElement, List<ElementLog>>();

        /// <summary>
        ///     The reverse cache dependancy.
        ///     All model elements beloging to the cache dependancy need be recomputed when this model element changes
        /// </summary>
        public HashSet<ModelElement> CacheDependancy { get; set; }

        /// <summary>
        ///     Adds an element whose value is dependant to this one
        /// </summary>
        /// <param name="dependant"></param>
        /// <returns>True if the dependancy has been added</returns>
        public bool AddDependancy(ModelElement dependant)
        {
            bool retVal = false;

            if (CacheDependancy == null)
            {
                CacheDependancy = new HashSet<ModelElement>();
                retVal = true;
            }

            bool change = CacheDependancy.Add(dependant);
            retVal = retVal || change;

            return retVal;
        }

        /// <summary>
        ///     Provides the names of the elements in the cache dependancy
        /// </summary>
        /// <returns></returns>
        public string CacheDependancyNames()
        {
            string retVal = "";

            if (CacheDependancy != null)
            {
                foreach (ModelElement element in CacheDependancy)
                {
                    retVal += element.FullName + " ";
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Handles a change of the model element by invalidating the cache of all element in CacheDependancy
        /// </summary>
        public virtual void HandleChange()
        {
            if (CacheDependancy != null)
            {
                foreach (ModelElement element in CacheDependancy)
                {
                    element.ClearCache();
                }
            }
        }

        /// <summary>
        ///     Clears the cache associated to this model element
        /// </summary>
        public virtual void ClearCache()
        {
        }

        /// <summary>
        ///     Indicates whether this is a parent of the element.
        ///     It also returns true then parent==element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsParent(IModelElement element)
        {
            bool retVal;

            IModelElement current = element;
            do
            {
                retVal = current == this;
                current = EnclosingFinder<IModelElement>.find(current);
            } while (!retVal && current != null);

            return retVal;
        }
    }
}