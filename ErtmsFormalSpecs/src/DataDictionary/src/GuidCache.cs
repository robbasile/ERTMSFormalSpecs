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
        ///     The EFS system for this cache
        /// </summary>
        private EFSSystem EFSSystem { get; set; }

        /// <summary>
        ///     The cache between guid and ModelElement
        /// </summary>
        private Dictionary<string, ModelElement> cache = new Dictionary<string, ModelElement>();

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="system"></param>
        public GuidCache(EFSSystem system)
        {
            EFSSystem = system;
        }

        /// <summary>
        ///     Clears the cache
        /// </summary>
        public void ClearCache()
        {
            cache.Clear();
        }

        /// <summary>
        ///     Updates the cache according to the model
        /// </summary>
        private class GuidVisitor : Visitor
        {
            /// <summary>
            ///     The dictionary to update
            /// </summary>
            private Dictionary<string, ModelElement> Dictionary;

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="dictionary"></param>
            public GuidVisitor(Dictionary<string, ModelElement> dictionary)
            {
                Dictionary = dictionary;
            }

            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                ModelElement element = (ModelElement) obj;

                string guid = element.Guid;
                if (guid != null)
                {
                    ModelElement cachedElement;
                    if (Dictionary.TryGetValue(guid, out cachedElement))
                    {
                        if (element != cachedElement)
                        {
                            throw new Exception("Model element collision found");
                        }
                    }
                    else
                    {
                        Dictionary[element.Guid] = element;
                    }
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Provides the model element which corresponds to the guid provided
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ModelElement GetModel(string guid)
        {
            ModelElement retVal;

            if (!cache.ContainsKey(guid))
            {
                // Update cache's contents
                GuidVisitor visitor = new GuidVisitor(cache);
                foreach (Dictionary dictionary in EFSSystem.Dictionaries)
                {
                    visitor.visit(dictionary, true);
                }

                cache.TryGetValue(guid, out retVal);
                if (retVal == null)
                {
                    cache[guid] = null;
                }
            }
            else
            {
                cache.TryGetValue(guid, out retVal);
            }

            return retVal;
        }

        /// <summary>
        ///     The guid cache instance singleton
        /// </summary>
        private static GuidCache __instance;

        /// <summary>
        ///     The cache instance
        /// </summary>
        public static GuidCache INSTANCE
        {
            get
            {
                if (__instance == null)
                {
                    __instance = new GuidCache(EFSSystem.INSTANCE);
                    FinderRepository.INSTANCE.Register(__instance);
                }

                return __instance;
            }
        }
    }
}