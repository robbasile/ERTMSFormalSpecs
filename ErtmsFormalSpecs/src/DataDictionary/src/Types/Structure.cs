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
using System.Linq;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.src;
using DataDictionary.Values;
using DataDictionary.Variables;
using Utils;
using Procedure = DataDictionary.Functions.Procedure;
using Rule = DataDictionary.Rules.Rule;

namespace DataDictionary.Types
{
    public class Structure : Generated.Structure, ISubDeclarator, IFinder, ITextualExplain
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Structure()
        {
            FinderRepository.INSTANCE.Register(this);
        }

        /// <summary>
        ///     The structure elements
        /// </summary>
        public ArrayList Elements
        {
            get
            {
                if (allElements() == null)
                {
                    setAllElements(new ArrayList());
                }
                return allElements();
            }
        }

        /// <summary>
        ///     The structure procedures
        /// </summary>
        public ArrayList Procedures
        {
            get
            {
                if (allProcedures() == null)
                {
                    setAllProcedures(new ArrayList());
                }
                return allProcedures();
            }
        }

        /// <summary>
        ///     The state machines
        /// </summary>
        public ArrayList StateMachines
        {
            get
            {
                if (allStateMachines() == null)
                {
                    setAllStateMachines(new ArrayList());
                }
                return allStateMachines();
            }
        }

        /// <summary>
        ///     Indicates if the structure is abstract
        /// </summary>
        public override bool IsAbstract
        {
            get { return getIsAbstract(); }
            set { setIsAbstract(value); }
        }

        /// <summary>
        ///     Provides the list of implemented interfaces (not recursive)
        /// </summary>
        public List<Structure> Interfaces
        {
            get
            {
                List<Structure> result = new List<Structure>();
                foreach (StructureRef structureRef in allInterfaces())
                {
                    result.Add(structureRef.ReferencedStructure);
                }
                return result;
            }
        }

        /// <summary>
        ///     Provides the list of implemented interfaces (not recursive)
        /// </summary>
        public List<StructureRef> InterfaceRefs
        {
            get
            {
                List<StructureRef> result = new List<StructureRef>();
                foreach (StructureRef structureRef in allInterfaces())
                {
                    result.Add(structureRef);
                }
                return result;
            }
        }

        /// <summary>
        ///     Provides the recursive list of the interfaces implemented by the structure,
        ///     together with the structure itself
        /// </summary>
        public List<Structure> ImplementedStructures
        {
            get
            {
                List<Structure> result = new List<Structure>();
                result.Add(this);
                foreach (StructureRef structureRef in allInterfaces())
                {
                    result.AddRange(structureRef.ImplementedStructures);
                }
                return result;
            }
        }

        public override void ClearCache()
        {
            base.ClearCache();
            _unifiedStructure = null;
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            foreach (StructureElement element in Elements)
            {
                ISubDeclaratorUtils.AppendNamable(this, element);
            }
            foreach (Procedure procedure in Procedures)
            {
                ISubDeclaratorUtils.AppendNamable(this, procedure);
            }

            foreach (StateMachine stateMachine in StateMachines)
            {
                ISubDeclaratorUtils.AppendNamable(this, stateMachine);
            }
        }

        /// <summary>
        ///     The declared elements of the structure
        /// </summary>
        public Dictionary<string, List<INamable>> DeclaredElements { get; set; }

        /// <summary>
        ///     Appends the INamable which match the name provided in retVal
        /// </summary>
        /// <param name="name"></param>
        /// <param name="retVal"></param>
        public void Find(string name, List<INamable> retVal)
        {
            ISubDeclaratorUtils.Find(this, name, retVal);
        }

        /// <summary>
        ///     The structure rules
        /// </summary>
        public ArrayList Rules
        {
            get
            {
                if (allRules() == null)
                {
                    setAllRules(new ArrayList());
                }
                return allRules();
            }
        }

        /// <summary>
        ///     Provides the structure element which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StructureElement FindStructureElement(string name)
        {
            return (StructureElement) NamableUtils.FindByName(name, Elements);
        }

        /// <summary>
        ///     Provides the procedure which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Procedure FindProcedure(string name)
        {
            return (Procedure) NamableUtils.FindByName(name, Procedures);
        }

        /// <summary>
        ///     Provides the rule which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Rule FindRule(string name)
        {
            return (Rule) NamableUtils.FindByName(name, Rules);
        }

        /// <summary>
        ///     Provides the state machine which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StateMachine FindStateMachine(string name)
        {
            return (StateMachine) NamableUtils.FindByName(name, StateMachines);
        }

        public override ArrayList EnclosingCollection
        {
            get { return NameSpace.Structures; }
        }

        /// <summary>
        ///     Provides the default value for this structure
        /// </summary>
        public override IValue DefaultValue
        {
            get
            {
                StructureValue retVal = new StructureValue(this);

                return retVal;
            }
        }

        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(IModelElement element)
        {
            {
                StructureRef item = element as StructureRef;
                if (item != null)
                {
                    appendInterfaces(item);
                }
            }
            {
                StructureElement item = element as StructureElement;
                if (item != null)
                {
                    appendElements(item);
                }
            }
            {
                Procedure item = element as Procedure;
                if (item != null)
                {
                    appendProcedures(item);
                }
            }
            {
                StateMachine item = element as StateMachine;
                if (item != null)
                {
                    appendStateMachines(item);
                }
            }
            {
                Rule item = element as Rule;
                if (item != null)
                {
                    appendRules(item);
                }
            }

            base.AddModelElement(element);
        }

        /// <summary>
        ///     Indicates that the other type can be placed in variables of this type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool Match(Type otherType)
        {
            bool result = base.Match(otherType);

            if (!result)
            {
                Structure structure = otherType as Structure;
                if (structure != null)
                {
                    result = structure.ImplementedStructures.Contains(this);
                }
            }

            return result;
        }

        /// <summary>
        ///     Indicates that binary operation is valid for this type and the other type
        /// </summary>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public override bool ValidBinaryOperation(BinaryExpression.OPERATOR operation, Type otherType)
        {
            bool retVal;

            if (operation == BinaryExpression.OPERATOR.LESS || operation == BinaryExpression.OPERATOR.LESS_OR_EQUAL ||
                operation == BinaryExpression.OPERATOR.GREATER ||
                operation == BinaryExpression.OPERATOR.GREATER_OR_EQUAL)
            {
                retVal = false;
            }
            else
            {
                retVal = base.ValidBinaryOperation(operation, otherType);
            }

            return retVal;
        }

        public override bool CompareForEquality(IValue left, IValue right) // left == right
        {
            bool retVal = base.CompareForEquality(left, right);

            if (!retVal)
            {
                if (left.Type == right.Type)
                {
                    StructureValue leftValue = left as StructureValue;
                    StructureValue rightValue = right as StructureValue;

                    if (left != null && right != null)
                    {
                        retVal = true;

                        foreach (KeyValuePair<string, IVariable> pair in leftValue.SubVariables)
                        {
                            IVariable leftVar = pair.Value;
                            IVariable rightVar = rightValue.getVariable(pair.Key);

                            if (leftVar.Type != null)
                            {
                                retVal = leftVar.Type.CompareForEquality(leftVar.Value, rightVar.Value);
                                if (!retVal)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Builds the explanation of the element
        /// </summary>
        /// <param name="explanation"></param>
        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        public override void GetExplain(TextualExplanation explanation, bool explainSubElements)
        {
            explanation.Comment(this);

            if (!IsAbstract)
            {
                explanation.Write("STRUCTURE ");
            }
            else
            {
                explanation.Write("INTERFACE ");
            }
            explanation.WriteLine(Name);

            explanation.Indent(2, () =>
            {
                foreach (Structure structure in Interfaces)
                {
                    if (structure != null)
                    {
                        explanation.Write("IMPLEMENTS ");
                        explanation.WriteLine(structure.Name);
                    }
                }

                foreach (StructureElement element in Elements)
                {
                    element.GetExplain(explanation, explainSubElements);
                }

                if (!IsAbstract)
                {
                    foreach (Procedure procedure in Procedures)
                    {
                        procedure.GetExplain(explanation, explainSubElements);
                    }

                    foreach (StateMachine stateMachine in StateMachines)
                    {
                        stateMachine.GetExplain(explanation, explainSubElements);
                    }

                    foreach (Rule rule in Rules)
                    {
                        rule.GetExplain(explanation, explainSubElements);
                    }
                }
            });

            if (!IsAbstract)
            {
                explanation.WriteLine("END STRUCTURE");
            }
            else
            {
                explanation.WriteLine("END INTERFACE");                
            }
        }

        /// <summary>
        ///     Generates the fields from the interited interfaces, if they are missing
        /// </summary>
        public void GenerateInheritedFields()
        {
            foreach (Structure inheritedStructure in Interfaces)
            {
                foreach (StructureElement inheritedElement in inheritedStructure.Elements)
                {
                    StructureElement correspondingElement = null;
                    foreach (StructureElement element in Elements)
                    {
                        if (element.Name.Equals(inheritedElement.Name))
                        {
                            correspondingElement = element;
                            break;
                        }
                    }
                    if (correspondingElement == null) // no correspondance found => create that element
                    {
                        appendElements(inheritedElement.Duplicate() as StructureElement);
                    }
                    else // correspondace found => update that element
                    {
                        correspondingElement.TypeName = inheritedElement.TypeName;
                    }
                }
            }
        }

        /// <summary>
        ///     Indicates if the structure implements the interface provided as parameter
        /// </summary>
        /// <param name="anInterface"></param>
        /// <returns></returns>
        public bool InterfaceIsInherited(Structure anInterface)
        {
            bool retVal = false;
            if (anInterface.IsAbstract && anInterface != this)
            {
                retVal = ImplementedStructures.Contains(anInterface);
            }
            return retVal;
        }

        /// <summary>
        ///     Indicates if the given structure element is inherited from an interface or not
        /// </summary>
        /// <param name="anElement"></param>
        /// <returns></returns>
        public bool StructureElementIsInherited(StructureElement anElement)
        {
            bool retVal = false;

            foreach (Structure inheritedStructure in Interfaces)
            {
                if (inheritedStructure != null)
                {
                    foreach (StructureElement inheritedElement in inheritedStructure.Elements)
                    {
                        if (anElement.Name.Equals(inheritedElement.Name))
                        {
                            retVal = true;
                            break;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates a copy of the structure in the designated dictionary. The namespace structure is copied over.
        ///     The new structure is set to update this one.
        /// </summary>
        /// <param name="dictionary">The target dictionary of the copy</param>
        /// <returns></returns>
        public Structure CreateStructureUpdate(Dictionary dictionary)
        {
            Structure retVal = (Structure)acceptor.getFactory().createStructure();
            retVal.Name = Name;
            retVal.Comment = Comment;
            retVal.IsAbstract = IsAbstract;
            retVal.SetUpdateInformation(this);

            String[] names = FullName.Split('.');
            names = names.Take(names.Count() - 1).ToArray();
            NameSpace nameSpace = dictionary.GetNameSpaceUpdate(names, Dictionary);
            nameSpace.appendStructures(retVal);

            return retVal;
        }

        /// <summary>
        ///     The unified structure of this with its updates and updated structures
        /// </summary>
        private Structure _unifiedStructure;

        /// <summary>
        ///     Creates a structure that is the combination of this and the updating structure (if there is only one)
        /// </summary>
        /// <returns>A combination of this structure and its update</returns>
        public Structure UnifiedStructure
        {
            get
            {
                if (_unifiedStructure == null)
                {
                    if (Updates != null || UpdatedBy.Count > 0)
                    {
                        _unifiedStructure = new UnifiedStructure(this);
                    }
                    else
                    {
                        _unifiedStructure = this;
                    }
                }

                return _unifiedStructure;
            }
            set { _unifiedStructure = value; }
        }

        /// <summary>
        ///     Checks that the proposed new value for the unified structure used this and, in that case,
        ///     sets the Unified structure to the proposed value
        /// </summary>
        /// <param name="newValue">The proposed new value for the unified structure</param>
        public void ResetUnifiedStructure(UnifiedStructure newValue)
        {
            if (newValue.MergedStructures.Contains(this))
            {
                UnifiedStructure = newValue;
            }
        }

        /// <summary>
        ///     Sets the update information for this structure
        /// </summary>
        /// <param name="source">The source structure for which this structure has been created (as an update)</param>
        public override void SetUpdateInformation(ModelElement source)
        {
            base.SetUpdateInformation(source);
            Structure sourceStructure = (Structure) source;

            foreach (StructureElement element in Elements)
            {
                StructureElement baseElement = sourceStructure.FindStructureElement(element.Name);
                if (baseElement != null)
                {
                    element.SetUpdateInformation(baseElement);
                }
            }

            foreach (Procedure procedure in Procedures)
            {
                Procedure baseProcedure = sourceStructure.FindProcedure(procedure.Name);
                if (baseProcedure != null)
                {
                    procedure.SetUpdateInformation(baseProcedure);
                }
            }

            foreach (Rule rule in Rules)
            {
                Rule baseRule = sourceStructure.FindRule(rule.Name);
                if (baseRule != null)
                {
                    rule.SetUpdateInformation(baseRule);
                }
            }

            foreach (StateMachine stateMachine in StateMachines)
            {
                StateMachine baseStateMachine = sourceStructure.FindStateMachine(stateMachine.Name);
                if (baseStateMachine != null)
                {
                    stateMachine.SetUpdateInformation(baseStateMachine);
                }
            }
        }

        /// <summary>
        ///     Ensures that all update information has been deleted
        /// </summary>
        public override void RecoverUpdateInformation()
        {
            base.RecoverUpdateInformation();

            foreach (StructureElement element in Elements)
            {
                element.RecoverUpdateInformation();
            }

            foreach (Procedure procedure in Procedures)
            {
                procedure.RecoverUpdateInformation();
            }

            foreach (Rule rule in Rules)
            {
                rule.RecoverUpdateInformation();
            }

            foreach (StateMachine stateMachine in StateMachines)
            {
                stateMachine.RecoverUpdateInformation();
            }
        }

        /// <summary>
        ///     Creates the status message 
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public override string CreateStatusMessage()
        {
            string result = base.CreateStatusMessage();

            if (IsAbstract)
            {
                result += "Interface " + Name;
            }
            else
            {
                result += "Structure " + Name;
            }
            result += " with " + Elements.Count + " elements and " + Procedures.Count + " procedures";

            return result;
        }

        /// <summary>
        /// Creates a default element
        /// </summary>
        /// <param name="enclosingCollection"></param>
        /// <param name="isInterface"></param>
        /// <returns></returns>
        public static Structure CreateDefault(ICollection enclosingCollection, bool isInterface)
        {
            Structure retVal = (Structure)acceptor.getFactory().createStructure();

            if (isInterface)
            {
                retVal.Name = "Interface" + GetElementNumber(enclosingCollection);
                retVal.IsAbstract = true;
            }
            else
            {
                retVal.Name = "Structure" + GetElementNumber(enclosingCollection);                
            }

            return retVal;
        }
    }
}