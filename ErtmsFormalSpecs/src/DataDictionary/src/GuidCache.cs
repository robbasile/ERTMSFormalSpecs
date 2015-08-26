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
using System.Collections.Generic;
using System.Threading;
using DataDictionary.Generated;
using Utils;

namespace DataDictionary
{
    /// <summary>
    ///     Cache for Guid -> ModelElement lookup
    /// </summary>
    public class GuidCache : IFinder
    {
        /// <summary>
        ///     The cache between guid and ModelElement
        /// </summary>
        private readonly Dictionary<string, ModelElement> _cache = new Dictionary<string, ModelElement>();

        /// <summary>
        ///     Constructor
        /// </summary>
        public GuidCache()
        {
            UniqueAccess = new Mutex(false, "Access to GUID cache");
        }

        /// <summary>
        ///     Clears the cache
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>
        ///     Updates the cache according to the model
        /// </summary>
        private class GuidVisitor : Visitor
        {
            /// <summary>
            ///     The dictionary to update
            /// </summary>
            private readonly Dictionary<string, ModelElement> _dictionary;

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="dictionary"></param>
            public GuidVisitor(Dictionary<string, ModelElement> dictionary)
            {
                _dictionary = dictionary;
            }

            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                ModelElement element = (ModelElement) obj;

                string guid = element.Guid;
                if (guid != null)
                {
                    ModelElement cachedElement;
                    if (_dictionary.TryGetValue(guid, out cachedElement))
                    {
                        if (cachedElement != null && element != cachedElement)
                        {
                            throw new Exception("Model element collision found");
                        }
                    }
                    else
                    {
                        _dictionary[element.Guid] = element;
                    }
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Ensure there is only one thread trying to get a model
        /// </summary>
        private Mutex UniqueAccess { get; set; }

        /// <summary>
        ///     Provides the model element which corresponds to the guid provided
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ModelElement GetModel(string guid)
        {
            ModelElement retVal = null;

            if (guid != null)
            {
                UniqueAccess.WaitOne();
                if (!_cache.TryGetValue(guid, out retVal))
                {
                    // Update cache's contents
                    GuidVisitor visitor = new GuidVisitor(_cache);
                    foreach (Dictionary dictionary in EfsSystem.Instance.Dictionaries)
                    {
                        visitor.visit(dictionary, true);
                    }

                    // Retrieve the result of the visit
                    if (!_cache.TryGetValue(guid, out retVal))
                    {
                        // Avoid revisiting when the id does not exist
                        _cache[guid] = null;
                    }
                }
                UniqueAccess.ReleaseMutex();
            }

            return retVal;
        }

        /// <summary>
        ///     The guid cache instance singleton
        /// </summary>
        private static GuidCache _instance;

        /// <summary>
        ///     The cache instance
        /// </summary>
        public static GuidCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GuidCache();
                    FinderRepository.INSTANCE.Register(_instance);
                }

                return _instance;
            }
        }
    }
}