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
using System.Linq;
using DataDictionary.Functions.PredefinedFunctions;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Interpreter.Filter;
using DataDictionary.Interpreter.Statement;
using DataDictionary.Specification;
using DataDictionary.Tests.Runner;
using DataDictionary.Types;
using DataDictionary.Values;
using HistoricalData;
using Utils;
using XmlBooster;
using Collection = DataDictionary.Types.Collection;
using Factory = DataDictionary.Compare.Factory;
using Function = DataDictionary.Functions.Function;
using History = DataDictionary.Compare.History;
using NameSpace = DataDictionary.Types.NameSpace;
using Paragraph = DataDictionary.Specification.Paragraph;
using RequirementSet = DataDictionary.Specification.RequirementSet;
using Rule = DataDictionary.Rules.Rule;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;
using Type = DataDictionary.Types.Type;
using Visitor = DataDictionary.Generated.Visitor;

namespace DataDictionary
{
    /// <summary>
    ///     A complete system, along with all dictionaries
    /// </summary>
    public class EfsSystem : IModelElement, ISubDeclarator, IHoldsParagraphs
    {
        /// <summary>
        ///     The dictionaries used in the system
        /// </summary>
        public List<Dictionary> Dictionaries { get; private set; }

        /// <summary>
        ///     The runner currently set for the system
        /// </summary>
        public Runner Runner { get; set; }

        /// <summary>
        ///     The context used to wake up listeners
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        ///     Indicates wheter the model should be recompiled (after a change or a load)
        /// </summary>
        public bool ShouldRebuild { get; set; }

        /// <summary>
        ///     Indicates wheter the model should be saved (after a change)
        /// </summary>
        public bool ShouldSave { get; set; }

        /// <summary>
        ///     The marking history
        /// </summary>
        public MarkingHistory Markings { get; private set; }

        /// <summary>
        ///     The compiler used to compile the system
        /// </summary>
        public Compiler Compiler { get; private set; }

        /// <summary>
        ///     Provides the history
        /// </summary>
        public History History { get; private set; }

        /// <summary>
        ///     The delegate to be called when the dictionary changed on the file system
        /// </summary>
        /// <param name="dictionary"></param>
        public delegate void HandleDictionaryChangesOnFileSystem(Dictionary dictionary);

        /// <summary>
        ///     The event raised when the dictionary changed on the file system
        /// </summary>
        public event HandleDictionaryChangesOnFileSystem DictionaryChangesOnFileSystem;

        /// <summary>
        ///     To be called when a dictionary changes on the file system
        /// </summary>
        /// <param name="dictionary"></param>
        public virtual void OnDictionaryChangesOnFileSystem(Dictionary dictionary)
        {
            if (DictionaryChangesOnFileSystem != null)
            {
                DictionaryChangesOnFileSystem(dictionary);
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        private EfsSystem()
        {
            Dictionaries = new List<Dictionary>();
            Context = new Context();
            Context.ValueChange += Context_ValueChange;
            acceptor.setFactory(new ObjectFactory());
            Compiler = new Compiler();
            Markings = new MarkingHistory();

            // Reads the history file and updates the blame information stored in it
            Factory historyFactory = Factory.INSTANCE;
            History = (History) HistoryUtils.Load(HistoricalData.History.HISTORY_FILE_NAME, historyFactory);
            if (History == null)
            {
                History = (History) historyFactory.createHistory();
            }
            History.UpdateBlame();

            CheckParentRelationship = true;
            CacheFunctions = true;
        }

        /// <summary>
        ///     The delegate used to handle the change of the value of a model element
        /// </summary>
        /// <param name="modelElement"></param>
        /// <param name="changeKind">Indicates the reason why the change occured</param>
        private void Context_ValueChange(IModelElement modelElement, Context.ChangeKind changeKind)
        {
            if (changeKind == Context.ChangeKind.ModelChange)
            {
                ShouldRebuild = true;
                ShouldSave = true;
            }
            else if (changeKind == Context.ChangeKind.Load)
            {
                ShouldRebuild = true;
            }
        }

        /// <summary>
        ///     Adds a new dictionary in the system
        /// </summary>
        /// <param name="dictionary"></param>
        public void AddDictionary(Dictionary dictionary)
        {
            if (dictionary != null)
            {
                // Remove the existing dictionaries
                Dictionary previous = Dictionaries.FirstOrDefault(other => other.FilePath == dictionary.FilePath);
                if (previous != null)
                {
                    Dictionaries.Remove(previous);
                    previous.Watcher.StopWatching();
                }

                // Add the new one
                dictionary.Enclosing = this;
                Dictionaries.Add(dictionary);

                // Setup the dictionary watcher
                DictionaryWatcher watcher = new DictionaryWatcher(this, dictionary);
                dictionary.Watcher = watcher;
            }
        }

        /// <summary>
        ///     The enclosing model element
        /// </summary>
        public object Enclosing
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///     The EFS System name
        /// </summary>
        public string Name
        {
            get { return "System"; }
            set { }
        }

        public string FullName
        {
            get { return Name; }
        }

        /// <summary>
        ///     The sub elements of this model element
        /// </summary>
        public ArrayList SubElements
        {
            get
            {
                ArrayList retVal = new ArrayList();

                foreach (Dictionary dictionary in Dictionaries)
                {
                    retVal.Add(dictionary);
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Provides the collection which holds this instance
        /// </summary>
        public ArrayList EnclosingCollection
        {
            get { return null; }
        }

        /// <summary>
        ///     Deletes the element from its enclosing node
        /// </summary>
        public void Delete()
        {
        }

        /// <summary>
        ///     The expression text data of this model element
        /// </summary>
        public string ExpressionText
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///     The messages logged on the model element
        /// </summary>
        public List<ElementLog> Messages
        {
            get { return new List<ElementLog>(); }
        }


        /// <summary>
        ///     Clears all marks related to model elements
        /// </summary>
        private class ClearMarksVisitor : Visitor
        {
            public override void visit(IXmlBBase obj, bool visitSubNodes)
            {
                IModelElement element = obj as IModelElement;

                if (element != null)
                {
                    element.ClearMessages(false);
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Clears the messages associated to this model element
        /// </summary>
        /// <param name="precise">
        ///     Indicates that the MessagePathInfo should be recomputed precisely
        ///     according to the sub elements and should update the enclosing elements
        /// </param>
        public void ClearMessages(bool precise)
        {
            ClearMarksVisitor visitor = new ClearMarksVisitor();
            foreach (Dictionary dictionary in Dictionaries)
            {
                visitor.visit(dictionary, true);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IModelElement other)
        {
            if (other == this)
            {
                return 0;
            }

            return -1;
        }

        /// --------------------------------------------------
        /// PREDEFINED ITEMS
        /// --------------------------------------------------
        /// <summary>
        ///     The predefined empty value
        /// </summary>
        private EmptyValue _emptyValue;

        public EmptyValue EmptyValue
        {
            get
            {
                if (_emptyValue == null)
                {
                    _emptyValue = new EmptyValue(this);
                }
                return _emptyValue;
            }
        }

        /// <summary>
        ///     The predefined any type
        /// </summary>
        private AnyType _anyType;

        public AnyType AnyType
        {
            get
            {
                if (_anyType == null)
                {
                    _anyType = new AnyType(this);
                }
                return _anyType;
            }
        }

        /// <summary>
        ///     The predefined no type
        /// </summary>
        private NoType _noType;

        public NoType NoType
        {
            get
            {
                if (_noType == null)
                {
                    _noType = new NoType(this);
                }
                return _noType;
            }
        }

        /// <summary>
        ///     The predefined bool type
        /// </summary>
        private BoolType _boolType;

        public BoolType BoolType
        {
            get
            {
                if (_boolType == null)
                {
                    _boolType = new BoolType(this);
                }
                return _boolType;
            }
        }

        /// <summary>
        ///     The predefined integer type
        /// </summary>
        private IntegerType _integerType;

        public IntegerType IntegerType
        {
            get
            {
                if (_integerType == null)
                {
                    _integerType = new IntegerType(this);
                }
                return _integerType;
            }
        }

        /// <summary>
        ///     The predefined double type
        /// </summary>
        private DoubleType _doubleType;

        public DoubleType DoubleType
        {
            get
            {
                if (_doubleType == null)
                {
                    _doubleType = new DoubleType(this);
                }
                return _doubleType;
            }
        }

        /// <summary>
        ///     The predefined string type
        /// </summary>
        private StringType _stringType;

        public StringType StringType
        {
            get
            {
                if (_stringType == null)
                {
                    _stringType = new StringType(this);
                }
                return _stringType;
            }
        }

        /// <summary>
        ///     The generic collection type
        /// </summary>
        /// <returns></returns>
        private Collection _genericCollection;

        public Collection GenericCollection
        {
            get
            {
                if (_genericCollection == null)
                {
                    _genericCollection = new GenericCollection(this);
                }

                return _genericCollection;
            }
        }

        /// <summary>
        ///     The predefined types
        /// </summary>
        private Dictionary<string, Type> _predefinedTypes;

        public Dictionary<string, Type> PredefinedTypes
        {
            get
            {
                if (_predefinedTypes == null)
                {
                    PredefinedTypes = new Dictionary<string, Type>();
                    PredefinedTypes[BoolType.Name] = BoolType;
                    PredefinedTypes[IntegerType.Name] = IntegerType;
                    PredefinedTypes[DoubleType.Name] = DoubleType;
                    PredefinedTypes[StringType.Name] = StringType;
                }
                return _predefinedTypes;
            }
            set { _predefinedTypes = value; }
        }

        /// <summary>
        ///     Gets the boolean value which corresponds to the bool provided
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public IValue GetBoolean(bool val)
        {
            IValue retVal;

            if (val)
            {
                retVal = BoolType.True;
            }
            else
            {
                retVal = BoolType.False;
            }

            return retVal;
        }

        /// <summary>
        ///     The predefined allocate function
        /// </summary>
        private Allocate _allocatePredefinedFunction;

        public Allocate AllocatePredefinedFunction
        {
            get
            {
                if (_allocatePredefinedFunction == null)
                {
                    _allocatePredefinedFunction = new Allocate(this);
                }
                return _allocatePredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined available function
        /// </summary>
        private Available _availablePredefinedFunction;

        public Available AvailablePredefinedFunction
        {
            get
            {
                if (_availablePredefinedFunction == null)
                {
                    _availablePredefinedFunction = new Available(this);
                }
                return _availablePredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined not function
        /// </summary>
        private Not _notPredefinedFunction;

        public Not NotPredefinedFunction
        {
            get
            {
                if (_notPredefinedFunction == null)
                {
                    _notPredefinedFunction = new Not(this);
                }
                return _notPredefinedFunction;
            }
        }


        /// <summary>
        ///     The predefined min function
        /// </summary>
        private Min _minPredefinedFunction;

        public Min MinPredefinedFunction
        {
            get
            {
                if (_minPredefinedFunction == null)
                {
                    _minPredefinedFunction = new Min(this);
                }
                return _minPredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined MinSurface function
        /// </summary>
        private MinSurface _minSurfacePredefinedFunction;

        public MinSurface MinSurfacePredefinedFunction
        {
            get
            {
                if (_minSurfacePredefinedFunction == null)
                {
                    _minSurfacePredefinedFunction = new MinSurface(this);
                }
                return _minSurfacePredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined max function
        /// </summary>
        private Max _maxPredefinedFunction;

        public Max MaxPredefinedFunction
        {
            get
            {
                if (_maxPredefinedFunction == null)
                {
                    _maxPredefinedFunction = new Max(this);
                }
                return _maxPredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined targets function
        /// </summary>
        private Targets _targetsPredefinedFunction;

        public Targets TargetsPredefinedFunction
        {
            get
            {
                if (_targetsPredefinedFunction == null)
                {
                    _targetsPredefinedFunction = new Targets(this);
                }
                return _targetsPredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined discontinuities function
        /// </summary>
        private Discontinuities _discontPredefinedFunction;

        public Discontinuities DiscontPredefinedFunction
        {
            get
            {
                if (_discontPredefinedFunction == null)
                {
                    _discontPredefinedFunction = new Discontinuities(this);
                }
                return _discontPredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined RoundToMultiple function
        /// </summary>
        private RoundToMultiple _roundToMultiplePredefinedFunction;

        public RoundToMultiple RoundToMultiplePredefinedFunction
        {
            get
            {
                if (_roundToMultiplePredefinedFunction == null)
                {
                    _roundToMultiplePredefinedFunction = new RoundToMultiple(this);
                }
                return _roundToMultiplePredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined DoubleToInteger function
        /// </summary>
        private DoubleToInteger _doubleToIntegerPredefinedFunction;

        public DoubleToInteger DoubleToIntegerPredefinedFunction
        {
            get
            {
                if (_doubleToIntegerPredefinedFunction == null)
                {
                    _doubleToIntegerPredefinedFunction = new DoubleToInteger(this);
                }
                return _doubleToIntegerPredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined deceleration profile function
        /// </summary>
        private DecelerationProfile _decelerationProfilePredefinedFunction;

        public DecelerationProfile DecelerationProfilePredefinedFunction
        {
            get
            {
                if (_decelerationProfilePredefinedFunction == null)
                {
                    _decelerationProfilePredefinedFunction = new DecelerationProfile(this);
                }
                return _decelerationProfilePredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined before function
        /// </summary>
        private Before _beforePredefinedFunction;

        public Before BeforePredefinedFunction
        {
            get
            {
                if (_beforePredefinedFunction == null)
                {
                    _beforePredefinedFunction = new Before(this);
                }
                return _beforePredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined checkNumber function
        /// </summary>
        private CheckNumber _checkNumberPredefinedFunction;

        public CheckNumber CheckNumberPredefinedFunction
        {
            get
            {
                if (_checkNumberPredefinedFunction == null)
                {
                    _checkNumberPredefinedFunction = new CheckNumber(this);
                }
                return _checkNumberPredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined AddIncrement function
        /// </summary>
        private AddIncrement _addIncrementPredefinedFunction;

        public AddIncrement AddIncrementPredefinedFunction
        {
            get
            {
                if (_addIncrementPredefinedFunction == null)
                {
                    _addIncrementPredefinedFunction = new AddIncrement(this);
                }
                return _addIncrementPredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined AddToDate function
        /// </summary>
        private AddToDate _addToDatePredefinedFunction;

        public AddToDate AddToDatePredefinedFunction
        {
            get
            {
                if (_addToDatePredefinedFunction == null)
                {
                    _addToDatePredefinedFunction = new AddToDate(this);
                }
                return _addToDatePredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined override function
        /// </summary>
        private Override _overridePredefinedFunction;

        public Override OverridePredefinedFunction
        {
            get
            {
                if (_overridePredefinedFunction == null)
                {
                    _overridePredefinedFunction = new Override(this);
                }
                return _overridePredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined DistanceForSpeed function
        /// </summary>
        private DistanceForSpeed _distanceForSpeedPredefinedFunction;

        public DistanceForSpeed DistanceForSpeedPredefinedFunction
        {
            get
            {
                if (_distanceForSpeedPredefinedFunction == null)
                {
                    _distanceForSpeedPredefinedFunction = new DistanceForSpeed(this);
                }
                return _distanceForSpeedPredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined IntersectAt function
        /// </summary>
        private IntersectAt _intersectAtFunction;

        public IntersectAt IntersectAtFunction
        {
            get
            {
                if (_intersectAtFunction == null)
                {
                    _intersectAtFunction = new IntersectAt(this);
                }
                return _intersectAtFunction;
            }
        }

        /// <summary>
        ///     The predefined Full Deceleration For Target function
        /// </summary>
        private FullDecelerationForTarget _fullDecelerationForTargetPredefinedFunction;

        public FullDecelerationForTarget FullDecelerationForTargetPredefinedFunction
        {
            get
            {
                if (_fullDecelerationForTargetPredefinedFunction == null)
                {
                    _fullDecelerationForTargetPredefinedFunction = new FullDecelerationForTarget(this);
                }
                return _fullDecelerationForTargetPredefinedFunction;
            }
        }

        /// <summary>
        /// The predefined Concat function
        /// </summary>
        private Concat _concatPredefinedFunction;

        public Concat ConcatPredefinedFunction
        {
            get
            {
                if (_concatPredefinedFunction == null)
                {
                    _concatPredefinedFunction = new Concat(this);
                }
                return _concatPredefinedFunction;
            }
        }

        /// <summary>
        ///     The predefined functions
        /// </summary>
        private Dictionary<string, PredefinedFunction> _predefinedFunctions;

        public Dictionary<string, PredefinedFunction> PredefinedFunctions
        {
            get
            {
                if (_predefinedFunctions == null)
                {
                    _predefinedFunctions = new Dictionary<string, PredefinedFunction>();
                    _predefinedFunctions[AvailablePredefinedFunction.Name] = AvailablePredefinedFunction;
                    _predefinedFunctions[AllocatePredefinedFunction.Name] = AllocatePredefinedFunction;
                    _predefinedFunctions[NotPredefinedFunction.Name] = NotPredefinedFunction;
                    _predefinedFunctions[MinPredefinedFunction.Name] = MinPredefinedFunction;
                    _predefinedFunctions[MinSurfacePredefinedFunction.Name] = MinSurfacePredefinedFunction;
                    _predefinedFunctions[MaxPredefinedFunction.Name] = MaxPredefinedFunction;
                    _predefinedFunctions[TargetsPredefinedFunction.Name] = TargetsPredefinedFunction;
                    _predefinedFunctions[DiscontPredefinedFunction.Name] = DiscontPredefinedFunction;
                    _predefinedFunctions[RoundToMultiplePredefinedFunction.Name] = RoundToMultiplePredefinedFunction;
                    _predefinedFunctions[DoubleToIntegerPredefinedFunction.Name] = DoubleToIntegerPredefinedFunction;
                    _predefinedFunctions[DecelerationProfilePredefinedFunction.Name] =
                        DecelerationProfilePredefinedFunction;
                    _predefinedFunctions[BeforePredefinedFunction.Name] = BeforePredefinedFunction;
                    _predefinedFunctions[CheckNumberPredefinedFunction.Name] = CheckNumberPredefinedFunction;
                    _predefinedFunctions[AddIncrementPredefinedFunction.Name] = AddIncrementPredefinedFunction;
                    _predefinedFunctions[AddToDatePredefinedFunction.Name] = AddToDatePredefinedFunction;
                    _predefinedFunctions[OverridePredefinedFunction.Name] = OverridePredefinedFunction;
                    _predefinedFunctions[DistanceForSpeedPredefinedFunction.Name] = DistanceForSpeedPredefinedFunction;
                    _predefinedFunctions[IntersectAtFunction.Name] = IntersectAtFunction;
                    _predefinedFunctions[FullDecelerationForTargetPredefinedFunction.Name] =
                        FullDecelerationForTargetPredefinedFunction;
                    _predefinedFunctions[ConcatPredefinedFunction.Name] = ConcatPredefinedFunction;
                }
                return _predefinedFunctions;
            }
            set { _predefinedFunctions = value; }
        }


        /// <summary>
        ///     All predefined items in the system
        /// </summary>
        private Dictionary<string, INamable> _predefinedItems;

        public Dictionary<string, INamable> PredefinedItems
        {
            get
            {
                if (_predefinedItems == null)
                {
                    _predefinedItems = new Dictionary<string, INamable>();

                    foreach (KeyValuePair<string, PredefinedFunction> pair in PredefinedFunctions)
                    {
                        _predefinedItems.Add(pair.Key, pair.Value);
                    }
                    foreach (KeyValuePair<string, Type> pair in PredefinedTypes)
                    {
                        _predefinedItems.Add(pair.Key, pair.Value);
                        IEnumerateValues enumerator = pair.Value as IEnumerateValues;
                        if (enumerator != null)
                        {
                            Dictionary<string, object> constants = new Dictionary<string, object>();
                            enumerator.Constants("", constants);
                            foreach (KeyValuePair<string, object> pair2 in constants)
                            {
                                INamable namable = pair2.Value as INamable;
                                if (namable != null)
                                {
                                    _predefinedItems.Add(pair2.Key, namable);
                                }
                            }
                        }
                    }

                    PredefinedItems.Add(EmptyValue.Name, EmptyValue);
                }

                return _predefinedItems;
            }
            set { _predefinedItems = value; }
        }

        /// <summary>
        ///     Provides the predefined item, based on its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public INamable GetPredefinedItem(string name)
        {
            INamable namable;

            PredefinedItems.TryGetValue(name, out namable);

            return namable;
        }

        /// <summary>
        ///     Indicates that the element is removed
        /// </summary>
        public bool IsRemoved
        {
            get { return false; }
        }

        /// <summary>
        ///     Initialises the declared elements
        /// </summary>
        public void InitDeclaredElements()
        {
            DeclaredElements = new Dictionary<string, List<INamable>>();

            Util.DontNotify(() =>
            {
                ISubDeclaratorUtils.AppendNamable(this, EmptyValue);
                foreach (Type type in PredefinedTypes.Values)
                {
                    ISubDeclaratorUtils.AppendNamable(this, type);
                }
                foreach (PredefinedFunction function in PredefinedFunctions.Values)
                {
                    ISubDeclaratorUtils.AppendNamable(this, function);
                }

                // Adds the namable from the default namespace as directly accessible
                foreach (Dictionary dictionary in Dictionaries)
                {
                    foreach (NameSpace nameSpace in dictionary.NameSpaces)
                    {
                        if ("Default".Equals(nameSpace.Name))
                        {
                            if (nameSpace.DeclaredElements == null)
                            {
                                nameSpace.InitDeclaredElements();
                            }
                            foreach (List<INamable> namables in nameSpace.DeclaredElements.Values)
                            {
                                foreach (INamable namable in namables)
                                {
                                    ISubDeclaratorUtils.AppendNamable(this, namable);
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        ///     Provides the list of declared elements in this System
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
        ///     Finds all namable which match the full name provided
        /// </summary>
        /// <param name="fullname">The full name used to search the namable</param>
        public INamable FindByFullName(string fullname)
        {
            INamable retVal = null;

            foreach (Dictionary dictionary in Dictionaries)
            {
                retVal = dictionary.FindByFullName(fullname);
                if (retVal != null)
                {
                    // TODO : only finds the first occurence of the namable in all opened dictionaries.
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type associated to the name
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type FindType(NameSpace nameSpace, string name)
        {
            Type retVal = null;

            if (name != null)
            {
                foreach (Dictionary dictionary in Dictionaries)
                {
                    retVal = dictionary.FindType(nameSpace, name);
                    if (retVal != null)
                    {
                        break;
                    }
                }

                if (retVal == null)
                {
                    PredefinedTypes.TryGetValue(name, out retVal);
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Provides the type associated to the name, if it exists.
        ///     If it does not exist, does not raise any errors.
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type FindType_silent(NameSpace nameSpace, string name)
        {
            Type retVal = null;

            ModelElement.DontRaiseError(() =>
            {
                retVal = FindType(nameSpace, name);
            });

            return retVal;
        }

        /// <summary>
        ///     Finds a rule according to its full name
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public Rule FindRule(string fullName)
        {
            Rule retVal = null;

            foreach (Dictionary dictionary in Dictionaries)
            {
                retVal = dictionary.FindRule(fullName);
                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }


        /// <summary>
        ///     Adds a model element in this model element
        /// </summary>
        /// <param name="element"></param>
        public void AddModelElement(IModelElement element)
        {
            {
                Dictionary item = element as Dictionary;
                if (item != null)
                {
                    Dictionaries.Add(item);
                }
            }
        }

        /// <summary>
        ///     Indicates whether this is a parent of the element.
        ///     It also returns true then parent==element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsParent(IModelElement element)
        {
            return true;
        }

        /// <summary>
        ///     The evaluator for this dictionary
        /// </summary>
        private Parser _parser;

        public Parser Parser
        {
            get
            {
                if (_parser == null)
                {
                    _parser = new Parser();
                }
                return _parser;
            }
        }

        /// <summary>
        ///     Parses the statement provided
        /// </summary>
        /// <param name="root">the root element for which this statement is created</param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Statement ParseStatement(ModelElement root, string expression)
        {
            return Parser.Statement(root, expression);
        }

        /// <summary>
        ///     The EFS System instance
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly EfsSystem _instance = new EfsSystem();

        /// <summary>
        /// The EFSSystem singleton
        /// </summary>
        public static EfsSystem Instance { get { return _instance; } }

        /// <summary>
        ///     Provides an RTF explanation of the system
        /// </summary>
        /// <returns></returns>
        public string GetExplain()
        {
            return "";
        }

        /// <summary>
        ///     The visitor who shall find all references
        /// </summary>
        private class ReferenceVisitor : Visitor
        {
            /// <summary>
            ///     The references found
            /// </summary>
            public List<Usage> Usages { get; private set; }

            /// <summary>
            ///     The element to be found
            /// </summary>
            private ModelElement Model { get; set; }

            /// <summary>
            ///     The filter to apply to the selection
            /// </summary>
            private BaseFilter Filter { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="model">The model element to be found</param>
            public ReferenceVisitor(ModelElement model)
            {
                Usages = new List<Usage>();
                Model = model;
                Filter = null;
            }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="filter">The filter to apply to the search</param>
            public ReferenceVisitor(BaseFilter filter)
            {
                Usages = new List<Usage>();
                Model = null;
                Filter = filter;
            }

            /// <summary>
            ///     Takes an interpreter tree into consideration
            /// </summary>
            /// <param name="tree"></param>
            private void ConsiderInterpreterTreeNode(InterpreterTreeNode tree)
            {
                if (tree != null && tree.StaticUsage != null)
                {
                    if (Model != null)
                    {
                        List<Usage> usages = tree.StaticUsage.Find(Model);
                        foreach (Usage usage in usages)
                        {
                            Usages.Add(usage);
                        }
                    }
                    else
                    {
                        foreach (Usage usage in tree.StaticUsage.AllUsages)
                        {
                            if (Filter.AcceptableChoice(usage.Referenced))
                            {
                                Usages.Add(usage);
                            }
                        }
                    }
                }
            }

            /// <summary>
            ///     Considers the type of the element provided as parameter
            /// </summary>
            /// <param name="element">The element which uses this type</param>
            private void ConsiderTypeOfElement(ITypedElement element)
            {
                ModelElement modelElement = element as ModelElement;
                if (modelElement != null)
                {
                    if (Model != null)
                    {
                        if ((element.Type == Model) && (element != Model))
                        {
                            Usages.Add(new Usage(Model, modelElement, Usage.ModeEnum.Type));
                        }
                    }
                    else
                    {
                        if (Filter.AcceptableChoice(element.Type))
                        {
                            Usages.Add(new Usage(element.Type, modelElement, Usage.ModeEnum.Type));
                        }
                    }
                }
            }

            /// <summary>
            ///     Walk through all elements
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="visitSubNodes"></param>
            public override void visit(BaseModelElement obj, bool visitSubNodes)
            {
                IExpressionable expressionable = obj as IExpressionable;
                if (expressionable != null)
                {
                    ConsiderInterpreterTreeNode(expressionable.Tree);
                }

                ITypedElement element = obj as ITypedElement;
                if (element != null)
                {
                    ConsiderTypeOfElement(element);
                }

                Function function = obj as Function;
                if (function != null)
                {
                    if (function.ReturnType == Model && function != Model)
                    {
                        Usages.Add(new Usage(element.Type, function, Usage.ModeEnum.Type));
                    }
                }

                /* searching for the implementation of interfaces */
                Structure structure = obj as Structure;
                Structure modelStructure = Model as Structure;
                if (structure != null && modelStructure != null && structure != modelStructure)
                {
                    if (modelStructure.IsAbstract && structure.ImplementedStructures.Contains(modelStructure))
                    {
                        Usages.Add(new Usage(Model, structure, Usage.ModeEnum.Interface));
                    }
                }

                /* searching for the redefinition of structure elements */
                StructureElement currentStructureElement = Model as StructureElement;
                StructureElement parameterStructureElement = obj as StructureElement;
                if (currentStructureElement != null && parameterStructureElement != null)
                {
                    Structure enclosingStructure = currentStructureElement.Enclosing as Structure;
                    Structure enclosingParameterStructure = parameterStructureElement.Enclosing as Structure;
                    if (enclosingStructure != null &&
                        enclosingParameterStructure != null &&
                        enclosingParameterStructure.IsAbstract &&
                        enclosingStructure.StructureElementIsInherited(currentStructureElement) &&
                        enclosingStructure.InterfaceIsInherited(enclosingParameterStructure))
                    {
                        if (currentStructureElement.Name == parameterStructureElement.Name &&
                            currentStructureElement.Type == parameterStructureElement.Type)
                        {
                            Usages.Add(new Usage(currentStructureElement, parameterStructureElement,
                                Usage.ModeEnum.Redefines));
                        }
                    }
                }

                base.visit(obj, visitSubNodes);
            }
        }

        /// <summary>
        ///     Provides the list of references of a given model element
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<Usage> FindReferences(ModelElement model)
        {
            List<Usage> retVal;

            if (model != null)
            {
                // Find references
                ReferenceVisitor visitor = new ReferenceVisitor(model);
                ModelElement.DontRaiseError(() =>
                {
                    foreach (Dictionary dictionary in Dictionaries)
                    {
                        visitor.visit(dictionary, true);
                    }
                    visitor.Usages.Sort();
                });

                retVal = visitor.Usages;
                foreach (Usage usage in retVal)
                {
                    // It has not been provent that it is something else than Read
                    // Let's consider it is read
                    if (usage.Mode == null)
                    {
                        usage.Mode = Usage.ModeEnum.Read;
                    }
                }
            }
            else
            {
                retVal = new List<Usage>();
            }

            return retVal;
        }


        /// <summary>
        ///     Provides the list of references for a given filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Usage> FindReferences(BaseFilter filter)
        {
            // Find references
            ReferenceVisitor visitor = new ReferenceVisitor(filter);
            ModelElement.DontRaiseError(() =>
            {
                foreach (Dictionary dictionary in Dictionaries)
                {
                    visitor.visit(dictionary, true);
                }
                visitor.Usages.Sort();
            });

            return visitor.Usages;
        }

        /// <summary>
        ///     Indicates whether enclosing messages should be displayed
        /// </summary>
        public bool DisplayEnclosingMessages { get; set; }

        /// <summary>
        ///     Indicates that requirements should be displayed as a list of element instead of the full requirement description
        /// </summary>
        public bool DisplayRequirementsAsList { get; set; }

        /// <summary>
        ///     When animating the model, verify the correctness of the 'parent' relation for each model element
        /// </summary>
        public bool CheckParentRelationship { get; set; }

        /// <summary>
        ///     When animating the model, cache the function results
        /// </summary>
        public bool CacheFunctions { get; set; }

        /// <summary>
        ///     Stops the system
        /// </summary>
        public void Stop()
        {
            Compiler.DoCompile = false;
            Context.Stop();
        }

        /// <summary>
        ///     Gets all paragraphs from EFS System
        /// </summary>
        /// <returns></returns>
        public void GetParagraphs(List<Paragraph> paragraphs)
        {
            foreach (Dictionary dictionary in Dictionaries)
            {
                dictionary.GetParagraphs(paragraphs);
            }
        }

        /// <summary>
        ///     Indicates if the element holds messages, or is part of a path to a message
        /// </summary>
        public MessageInfoEnum MessagePathInfo
        {
            get { return MessageInfoEnum.NoMessage; }
        }

        /// <summary>
        ///     Provides the list of requirement sets in the system
        /// </summary>
        public List<RequirementSet> RequirementSets
        {
            get
            {
                List<RequirementSet> retVal = new List<RequirementSet>();

                foreach (Dictionary dictionary in Dictionaries)
                {
                    foreach (RequirementSet requirementSet in dictionary.RequirementSets)
                    {
                        retVal.Add(requirementSet);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        ///     Marks the requirements for a specific requirement set
        /// </summary>
        private class RequirementSetMarker : Visitor
        {
            /// <summary>
            ///     The requirement set for which marking is done
            /// </summary>
            private RequirementSet RequirementSet { get; set; }

            /// <summary>
            ///     Indicates if the requirement must belong to the requirement set, or not
            /// </summary>
            private bool Belonging { get; set; }

            /// <summary>
            ///     Indicates if only the non implemented requirements should be marked
            /// </summary>
            private bool NotImplemented { get; set; }

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="requirementSet"></param>
            /// <param name="belonging">
            ///     Indicates whether the paragraph should belong to the requirement set
            /// </param>
            /// <param name="notImplemented">Indicates that the the elements that should be marked are the not implemented ones</param>
            /// or whether the requirement should not belong to that requirement set
            public RequirementSetMarker(RequirementSet requirementSet, bool belonging, bool notImplemented)
            {
                RequirementSet = requirementSet;
                Belonging = belonging;
                NotImplemented = notImplemented;
            }

            /// <summary>
            ///     Marks the paragraph
            /// </summary>
            /// <param name="paragraph"></param>
            /// <param name="recursively">Indicates that the paragraph should be marked recursively</param>
            /// <returns>true if marking recursively was applied</returns>
            private bool MarkBelongingParagraph(Paragraph paragraph, bool recursively)
            {
                if (!NotImplemented)
                {
                    paragraph.AddInfo("Requirement set " + RequirementSet.Name);
                }
                else if (paragraph.getImplementationStatus() != acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented &&
                         paragraph.getImplementationStatus() != acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NotImplementable)
                {
                    if (paragraph.getType() == acceptor.Paragraph_type.aREQUIREMENT)
                    {
                        paragraph.AddInfo("Belongs to Requirement set " + RequirementSet.Name +
                                          " but is not implemented");
                    }
                }

                if (recursively)
                {
                    foreach (Paragraph subParagraph in paragraph.SubParagraphs)
                    {
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        MarkBelongingParagraph(subParagraph, recursively);
                    }
                }

                return recursively;
            }

            public override void visit(Generated.Paragraph obj, bool visitSubNodes)
            {
                Paragraph paragraph = (Paragraph) obj;

                if (paragraph.BelongsToRequirementSet(RequirementSet))
                {
                    if (Belonging)
                    {
                        if (!MarkBelongingParagraph(paragraph, RequirementSet.getRecursiveSelection()))
                        {
                            base.visit(obj, visitSubNodes);
                        }
                    }
                    else
                    {
                        base.visit(obj, visitSubNodes);
                    }
                }
                else
                {
                    if (!Belonging)
                    {
                        if (paragraph.getType() == acceptor.Paragraph_type.aREQUIREMENT)
                        {
                            paragraph.AddInfo("Requirement does not belong to requirement set " + RequirementSet.Name);
                        }
                    }
                    base.visit(obj, visitSubNodes);
                }
            }
        }

        /// <summary>
        ///     Marks the requirements which relate to the corresponding requirement set
        /// </summary>
        /// <param name="requirementSet"></param>
        public void MarkRequirementsForRequirementSet(RequirementSet requirementSet)
        {
            MarkingHistory.PerformMark(() =>
            {
                RequirementSetMarker marker = new RequirementSetMarker(requirementSet, true, false);
                foreach (Dictionary dictionary in Dictionaries)
                {
                    marker.visit(dictionary);
                }
            });
        }

        /// <summary>
        ///     Marks the requirements which relate to the corresponding requirement set
        /// </summary>
        /// <param name="requirementSet"></param>
        public void MarkRequirementsWhichDoNotBelongToRequirementSet(RequirementSet requirementSet)
        {
            MarkingHistory.PerformMark(() =>
            {
                RequirementSetMarker marker = new RequirementSetMarker(requirementSet, false, false);
                foreach (Dictionary dictionary in Dictionaries)
                {
                    marker.visit(dictionary);
                }
            });
        }

        /// <summary>
        ///     Marks the requirements which relate to the corresponding requirement set
        /// </summary>
        /// <param name="requirementSet"></param>
        public void MarkNotImplementedRequirements(RequirementSet requirementSet)
        {
            MarkingHistory.PerformMark(() =>
            {
                RequirementSetMarker marker = new RequirementSetMarker(requirementSet, true, true);
                foreach (Dictionary dictionary in Dictionaries)
                {
                    marker.visit(dictionary);
                }
            });
        }

        /// <summary>
        ///     Provides the requirement set whose name corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RequirementSet FindRequirementSet(string name)
        {
            RequirementSet retVal = null;

            foreach (Dictionary dictionary in Dictionaries)
            {
                foreach (RequirementSet requirementSet in dictionary.RequirementSets)
                {
                    if (requirementSet.Name == name)
                    {
                        retVal = requirementSet;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        ///     Creates the status message
        /// </summary>
        /// <returns>the status string for the selected element</returns>
        public string CreateStatusMessage()
        {
            List<Paragraph> paragraphs = new List<Paragraph>();
            GetParagraphs(paragraphs);

            return Paragraph.CreateParagraphSetStatus(paragraphs);
        }
    }
}