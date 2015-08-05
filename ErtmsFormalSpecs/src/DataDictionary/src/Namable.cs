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
using Utils;

namespace DataDictionary
{
    /// <summary>
    ///     Something that has a name
    /// </summary>
    public abstract class Namable : Generated.Namable, IComparable<Namable>, IComparable
    {
        /// <summary>
        ///     The name separator
        /// </summary>
        public static string Separator = ".";

        /// <summary>
        ///     The name
        /// </summary>
        public override string Name
        {
            get { return getName(); }
            set
            {
                if (value == null)
                {
                    setName("");
                }
                else
                {
                    setName(value.Trim());
                }
                __fullName = null;
            }
        }

        /// <summary>
        ///     Stored version of the full name
        /// </summary>
        private string __fullName = null;

        public void ClearFullName()
        {
            __fullName = null;
        }

        /// <summary>
        ///     Provides the full name path to this element in the model structure
        /// </summary>
        public override string FullName
        {
            get
            {
                if (__fullName == null)
                {
                    Namable enclosing = EnclosingFinder<Namable>.find(this);
                    if (enclosing != null)
                    {
                        __fullName = enclosing.FullName + "." + Name;
                    }
                    else
                    {
                        __fullName = Name;
                    }

                    while (__fullName != null && __fullName.EndsWith("."))
                    {
                        __fullName = __fullName.Substring(1, __fullName.Length - 1);
                    }
                }

                return __fullName;
            }
        }

        /// <summary>
        ///     The comparer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Namable other)
        {
            return Comparer.StringComparer.Compare(Name, other.Name);
        }

        /// <summary>
        ///     The comparer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            Namable namable = other as Namable;
            if (namable != null)
            {
                return Comparer.StringComparer.Compare(Name, namable.Name);
            }

            return 0;
        }
    }
}