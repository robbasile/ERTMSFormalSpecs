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

using System.Collections;
using System.Collections.Generic;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Types;
using DataDictionary.Types.AccessMode;
using DataDictionary.Variables;
using Utils;

namespace DataDictionary
{
    /// <summary>
    ///     Holds several namespaces
    /// </summary>
    public interface IEnclosesNameSpaces
    {
        /// <summary>
        ///     The EFS system in which this container lies
        /// </summary>
        EFSSystem EFSSystem { get; }

        /// <summary>
        ///     The namespaces referenced by this
        /// </summary>
        ArrayList NameSpaces { get; }
    }


    /// <summary>
    ///     Utility class to handle INameSpaceContainer
    /// </summary>
    public static class IEnclosesNameSpacesUtils
    {
        /// <summary>
        ///     Provides all the function calls related to this namespace
        /// </summary>
        /// <param name="system">The system in which the calls should be gathered</param>
        /// <param name="container">If provided, indicates that the calls should be limited to a given container</param>
        /// <returns></returns>
        public static List<AccessMode> getAccesses(EFSSystem system, IEnclosesNameSpaces container = null)
        {
            SortedSet<ProcedureOrFunctionCall> procedureCalls = new SortedSet<ProcedureOrFunctionCall>();
            SortedSet<AccessToVariable> accessesToVariables = new SortedSet<AccessToVariable>();
            foreach (Usage usage in system.FindReferences(IsCallableOrIsVariable.INSTANCE))
            {
                ModelElement target = (ModelElement) usage.Referenced;
                ModelElement source = usage.User;

                NameSpace sourceNameSpace = getCorrespondingNameSpace(source, container, true);
                NameSpace targetNameSpace = getCorrespondingNameSpace(target, container, false);

                if (IsCallable.Predicate(usage.Referenced))
                {
                    if (considerCall(usage, container, sourceNameSpace, targetNameSpace))
                    {
                        procedureCalls.Add(new ProcedureOrFunctionCall(sourceNameSpace, targetNameSpace,
                            (ICallable) target));
                    }
                }
                else
                {
                    // IsVariable(usage.Referenced)
                    if (considerVariableReference(usage, container, sourceNameSpace, targetNameSpace))
                    {
                        Usage.ModeEnum mode = (Usage.ModeEnum) usage.Mode;

                        // Find a corresponding access to variable (same source and target namespaces, and same variable
                        AccessToVariable otherAccess = null;
                        foreach (AccessToVariable access in accessesToVariables)
                        {
                            if (access.Target == usage.Referenced && access.Source == sourceNameSpace &&
                                access.Target == targetNameSpace)
                            {
                                otherAccess = access;
                                break;
                            }
                        }

                        if (otherAccess != null)
                        {
                            if (otherAccess.AccessMode != mode)
                            {
                                // Since the access mode is different, one of them is either Read or ReadWrite and the other is ReadWrite or Write. 
                                // So, in any case, the resulting access mode is ReadWrite
                                accessesToVariables.Remove(otherAccess);
                                accessesToVariables.Add(new AccessToVariable(sourceNameSpace, targetNameSpace,
                                    (IVariable) target, Usage.ModeEnum.ReadAndWrite));
                            }
                            else
                            {
                                // Already exists, do nothing
                            }
                        }
                        else
                        {
                            // Does not already exists, insert it in the list
                            accessesToVariables.Add(new AccessToVariable(sourceNameSpace, targetNameSpace,
                                (IVariable) target, mode));
                        }
                    }
                }
            }

            // Build the results based on the intermediate results
            List<AccessMode> retVal = new List<AccessMode>();
            retVal.AddRange(procedureCalls);
            retVal.AddRange(accessesToVariables);

            return retVal;
        }

        /// <summary>
        ///     Indicates whether a call should be considered in the ProcedureOrFunctionCalls
        /// </summary>
        /// <param name="functionCall"></param>
        /// <param name="container"></param>
        /// <param name="sourceNameSpace"></param>
        /// <param name="targetNameSpace"></param>
        /// <returns></returns>
        private static bool considerCall(Usage functionCall, IEnclosesNameSpaces container, NameSpace sourceNameSpace,
            NameSpace targetNameSpace)
        {
            bool retVal = considerCommon(container, sourceNameSpace, targetNameSpace);

            // Only consider callables
            retVal = retVal && functionCall.Referenced is ICallable;

            // Ignore casting
            retVal = retVal && !(functionCall.Referenced is Range);

            return retVal;
        }

        /// <summary>
        ///     Indicates whether a call should be considered in the ProcedureOrFunctionCalls
        /// </summary>
        /// <param name="variableReference"></param>
        /// <param name="container"></param>
        /// <param name="sourceNameSpace"></param>
        /// <param name="targetNameSpace"></param>
        /// <returns></returns>
        private static bool considerVariableReference(Usage variableReference, IEnclosesNameSpaces container,
            NameSpace sourceNameSpace, NameSpace targetNameSpace)
        {
            bool retVal = considerCommon(container, sourceNameSpace, targetNameSpace);

            // Only consider variables
            retVal = retVal && variableReference.Referenced is IVariable;

            // Only consider variable accesses when the mode is known
            retVal = retVal && variableReference.Mode != null;

            return retVal;
        }

        /// <summary>
        ///     Common part of the consideration of an access
        /// </summary>
        /// <param name="container"></param>
        /// <param name="sourceNameSpace"></param>
        /// <param name="targetNameSpace"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        private static bool considerCommon(IEnclosesNameSpaces container, NameSpace sourceNameSpace,
            NameSpace targetNameSpace)
        {
            bool retVal = true;

            // Only display things that are relevant namespacewise
            retVal = retVal && sourceNameSpace != null && targetNameSpace != null;

            // Do not consider internal accesses 
            retVal = retVal && sourceNameSpace != targetNameSpace;

            if (container != null)
            {
                // Only display things that can be displayed in this functional view
                // TODO : also consider sub namespaces in the diagram
                retVal = retVal &&
                         (container.NameSpaces.Contains(sourceNameSpace) ||
                          container.NameSpaces.Contains(targetNameSpace));
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the namespace of the element provided in this container
        /// </summary>
        /// <param name="source">The element from which the namespace should be found</param>
        /// <param name="container">The container which contains the namespace</param>
        /// <param name="allowOutsideContainer">Indicaes that namespace can be found outside of the container</param>
        /// <returns></returns>
        private static NameSpace getCorrespondingNameSpace(ModelElement source, IEnclosesNameSpaces container,
            bool allowOutsideContainer)
        {
            NameSpace retVal = null;

            IEnclosed current = source;
            while (current != null && retVal == null)
            {
                NameSpace nameSpace = EnclosingNameSpaceFinder.find(current);
                if (container == null || container.NameSpaces.Contains(nameSpace))
                {
                    retVal = nameSpace;
                }

                current = nameSpace;
            }

            // If no name space has been found in the container, take the first one
            if (retVal == null && allowOutsideContainer)
            {
                retVal = EnclosingNameSpaceFinder.find(source);
            }

            return retVal;
        }
    }
}