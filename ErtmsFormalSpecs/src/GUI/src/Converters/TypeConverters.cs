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

using System.Collections.Generic;
using System.ComponentModel;
using DataDictionary;
using DataDictionary.Constants;
using DataDictionary.Interpreter;
using DataDictionary.Rules;
using DataDictionary.Specification;
using DataDictionary.Types;
using Utils;

namespace GUI.Converters
{
    /// <summary>
    ///     Types converter. Provides the available types
    /// </summary>
    public class TypesConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public StandardValuesCollection GetValues(IModelElement element)
        {
            FinderRepository.INSTANCE.ClearCache();

            Dictionary dictionary = EnclosingFinder<Dictionary>.find(element);
            NameSpace nameSpace = EnclosingNameSpaceFinder.find(element);

            List<string> retVal = new List<string>();
            if (nameSpace != null)
            {
                OverallTypeFinder.INSTANCE.findAllValueNames("", nameSpace, true, retVal);
            }
            else
            {
                OverallTypeFinder.INSTANCE.findAllValueNames("", dictionary, false, retVal);
            }
            retVal.Sort();

            foreach (string name in dictionary.EFSSystem.PredefinedTypes.Keys)
            {
                retVal.Add(name);
            }

            return new StandardValuesCollection(retVal);
        }
    }

    /// <summary>
    ///     Values converter. Provides all possible values for a specific variable.
    /// </summary>
    public class ValuesConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public StandardValuesCollection GetValues(NameSpace nameSpace, Type type)
        {
            FinderRepository.INSTANCE.ClearCache();

            List<string> retVal = new List<string>();
            if (type != null)
            {
                string prefix = type.FullName;
                if (nameSpace == type.NameSpace && nameSpace != null)
                {
                    prefix = prefix.Substring(nameSpace.FullName.Length + 1);
                }
                OverallValueFinder.INSTANCE.findAllValueNames(prefix, type, false, retVal);
                retVal.Sort();
            }

            return new StandardValuesCollection(retVal);
        }
    }

    /// <summary>
    ///     Provides the list of sub systems
    /// </summary>
    public class NameSpaceConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true; // display drop
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // drop-down vs combo
        }

        public StandardValuesCollection GetValues(Dictionary dictionary)
        {
            FinderRepository.INSTANCE.ClearCache();

            List<string> retVal = new List<string>();
            OverallNameSpaceFinder.INSTANCE.findAllValueNames("", dictionary, false, retVal);
            retVal.Sort();

            return new StandardValuesCollection(retVal);
        }
    }

    /// <summary>
    ///     Provides the list of requirement paragraphs
    /// </summary>
    public class TracesConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true; // display drop
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // drop-down vs combo
        }

        public StandardValuesCollection GetValues(ReqRef req)
        {
            FinderRepository.INSTANCE.ClearCache();

            List<string> retVal = new List<string>();

            if (req.Model is Rule)
            {
                Rule rule = req.Model as Rule;
                foreach (Paragraph paragraph in rule.ApplicableParagraphs)
                {
                    retVal.Add(paragraph.getId());
                }
            }
            else
            {
                foreach (Dictionary dictionary in req.EFSSystem.Dictionaries)
                {
                    foreach (Specification specification in dictionary.Specifications)
                    {
                        foreach (string paragraph in specification.ParagraphList())
                        {
                            retVal.Add(paragraph);
                        }
                    }
                }
            }

            return new StandardValuesCollection(retVal);
        }
    }

    /// <summary>
    ///     Provides the list of operators
    /// </summary>
    public class StateTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true; // display drop
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // drop-down vs combo
        }

        public StandardValuesCollection GetValues(StateMachine StateMachine)
        {
            FinderRepository.INSTANCE.ClearCache();

            List<string> retVal = new List<string>();
            foreach (State subState in StateMachine.States)
            {
                retVal.Add(subState.Name);
            }
            retVal.Sort();

            if (retVal.Count == 0)
            {
                retVal.Add("");
            }

            return new StandardValuesCollection(retVal);
        }

        public StandardValuesCollection GetValues(State State)
        {
            return GetValues(State.StateMachine);
        }
    }

    /// <summary>
    ///     Provides the list of operators
    /// </summary>
    public class OperatorConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true; // display drop
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // drop-down vs combo
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            FinderRepository.INSTANCE.ClearCache();

            return new StandardValuesCollection(BinaryExpression.OperatorsImages);
        }
    }

    /// <summary>
    ///     Provides the list of requirement sets
    /// </summary>
    public class RequirementSetTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true; // display drop
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // drop-down vs combo
        }

        public StandardValuesCollection GetValues(IHoldsRequirementSets enclosing)
        {
            FinderRepository.INSTANCE.ClearCache();

            List<string> retVal = new List<string>();
            foreach (RequirementSet requirementSet in enclosing.RequirementSets)
            {
                retVal.Add(requirementSet.Name);
            }
            retVal.Sort();

            if (retVal.Count == 0)
            {
                retVal.Add("");
            }

            return new StandardValuesCollection(retVal);
        }
    }
}