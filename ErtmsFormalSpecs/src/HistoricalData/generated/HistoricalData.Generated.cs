
using XmlBooster;
using System.IO;
using System.Collections;
using System;

/// <remarks>XMLBooster-generated code (Version 2.22.4.0)
/// This code is generated automatically. It is not meant
/// to be maintained or even read. As it is generated, 
/// it does not follow any coding standard. Please refrain
/// from performing any change directly on this generated 
/// code, as it might be overwritten anytime.
/// This documentation is provided for information purposes
/// only, in order to make the generated API somewhat more
/// understandable. It is meant to be a maintenance guide,
/// as this code is not meant to be maintained at all.</remarks>
namespace HistoricalData.Generated{
public partial class History
: HistoricalData.HistoricalDataElement
{
public  override  bool find(Object search){
if (search is String ) {
}
return false;
}

public  override  void NotifyControllers(Lock aLock){
	base.NotifyControllers(aLock);
	ControllersManager.HistoryController.alertChange(aLock, this);
}
private System.Collections.ArrayList aCommits;

/// <summary>Part of the list interface for Commits</summary>
/// <returns>a collection of all the elements in Commits</returns>
public System.Collections.ArrayList allCommits()
  {
if (aCommits == null){
    setAllCommits( new System.Collections.ArrayList() );
} // If
    return aCommits;
  }

/// <summary>Part of the list interface for Commits</summary>
/// <returns>a collection of all the elements in Commits</returns>
private System.Collections.ArrayList getCommits()
  {
    return allCommits();
  }

/// <summary>Part of the list interface for Commits</summary>
/// <param name="coll">a collection of elements which replaces 
///        Commits's current content.</param>
public void setAllCommits(System.Collections.ArrayList coll)
  {
  __setDirty(true);
    aCommits = coll;
    NotifyControllers(null);
  }
public void setAllCommits(Lock aLock,System.Collections.ArrayList coll)
  {
  __setDirty(true);
    aCommits = coll;
NotifyControllers(aLock);
  }

/// <summary>Part of the list interface for Commits</summary>
/// <param name="el">a Commit to add to the collection in 
///           Commits</param>
/// <seealso cref="appendCommits(System.Collections.IList)"/>
public void appendCommits(Commit el)
  {
  __setDirty(true);
  el.__setDirty(true);
  allCommits().Add(el);
  acceptor.connectSon (this, el);
NotifyControllers(null);
  }

public void appendCommits(Lock aLock,Commit el)
  {
  __setDirty(true);
  el.__setDirty(true);
  allCommits().Add(el);
  acceptor.connectSon (this, el);
NotifyControllers(aLock);
  }
/// <summary>Part of the list interface for Commits</summary>
/// <param name="coll">a collection ofCommits to add to the collection in 
///           Commits</param>
/// <seealso cref="appendCommits(Commit)"/>
public void appendCommits(System.Collections.IList coll)
  {
  __setDirty(true);
  allCommits().AddRange(coll);
  acceptor.connectSons (this, coll);
NotifyControllers(null);
  }

public void appendCommits(System.Collections.IList coll,Lock aLock)
  {
  __setDirty(true);
  allCommits().AddRange(coll);
  acceptor.connectSons (this, coll);
NotifyControllers(aLock);
  }

/// <summary>Part of the list interface for Commits
/// This insertion function inserts a new element in the
/// collection in Commits</summary>
/// <param name="idx">the index where the insertion must take place</param>
/// <param name="el">the element to insert</param>
public void insertCommits(int idx, Commit el)
  {
  __setDirty(true);
  allCommits().Insert (idx, el);
NotifyControllers(null);
  }

public void insertCommits(int idx, Commit el,Lock aLock)
  {
  __setDirty(true);
  allCommits().Insert (idx, el);
NotifyControllers(aLock);
  }

/// <summary>Part of the list interface for Commits
/// This function returns the index of an element in
/// the collection.</summary>
/// <param name="el">the object to look for</param>
/// <returns>the index where it is found, or -1 if it is not.</returns>
public int indexOfCommits(IXmlBBase el)
  {
  return ((System.Collections.IList) allCommits()).IndexOf (el);
  }

/// <summary>Part of the list interface for Commits
/// This deletion function removes an element from the
/// collection in Commits</summary>
/// <param name="idx">the index of the element to remove</param>
public void deleteCommits(int idx)
  {
  __setDirty(true);
  allCommits().RemoveAt(idx);
NotifyControllers(null);
  }

public void deleteCommits(int idx,Lock aLock)
  {
  __setDirty(true);
  allCommits().RemoveAt(idx);
NotifyControllers(aLock);
  }

/// <summary>Part of the list interface for Commits
/// This deletion function removes an element from the
/// collection in Commits
/// If the object given in parameter is not found in the
/// the collection, this function does nothing.</summary>
/// <param name="obj">the object to remove</param>
public void removeCommits(IXmlBBase obj)
  {
  int idx = indexOfCommits(obj);
  if (idx >= 0) { deleteCommits(idx);
NotifyControllers(null);
   }
  }

public void removeCommits(IXmlBBase obj,Lock aLock)
  {
  int idx = indexOfCommits(obj);
  if (idx >= 0) { deleteCommits(idx);
NotifyControllers(aLock);
  }}

/// <summary>Part of the list interface for Commits</summary>
/// <returns>the number of elements in Commits</returns>
public int countCommits()
  {
  return allCommits().Count;
  }

/// <summary>Part of the list interface for Commits
/// This function returns an element from the
/// collection in Commits based on an index.</summary>
/// <param name="idx">the index of the element to extract</param>
/// <returns>the extracted element</returns>
public Commit getCommits(int idx)
{
  return (Commit) ( allCommits()[idx]);
}

public History()
{
History obj = this;
aCommits=(null);
}

public void copyTo(History other)
{
other.aCommits = aCommits;
}

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override void parseBody(XmlBContext ctxt)

{
#pragma warning disable 0168, 0219
int indicator=0;
char quoteChar;
 string  tempStr;
#pragma warning restore 0168, 0219
Commit fl102;

ctxt.skipWhiteSpace();
// Repeat
ctxt.skipWhiteSpace();
fl102 = null;
while(ctxt.lookAheadOpeningTag ("<Commit")) {
fl102 = acceptor.lAccept_Commit(ctxt, null);
appendCommits(fl102);
ctxt.skipWhiteSpace();
} // -- monomorphic Loop
// EndRepeat
ctxt.skipWhiteSpace();
}


/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override  void parse(XmlBContext ctxt,  string  endingTag)

{
#pragma warning disable 0168, 0219
int indicator = 0;
char quoteChar;
 string  tempStr = null;
bool fl113;
#pragma warning restore 0168, 0219

ctxt.skipWhiteSpace();
fl113 = true ; 
while (fl113) { // BeginLoop 
ctxt.skipWhiteSpace();
if (ctxt.isAlNum()){
ctxt.skipTill ('=');
ctxt.advance();
ctxt.skipWhiteSpace();
quoteChar = ctxt.acceptQuote();
ctxt.skipTill (quoteChar);
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
} else {
fl113 = false ; 
} // If
} // While
ctxt.skipWhiteSpace();
if (ctxt.current() == '/'){
ctxt.advance();
ctxt.accept('>');
} else {
ctxt.accept('>');
parseBody(ctxt);
ctxt.acceptString ("</History>");
// If formula empty
} // If
}

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
 override  public  void unParse(TextWriter pw,
                    bool typeId,
                     string  headingTag,
                     string  endingTag)
{
#pragma warning disable 0168, 0219
int i;
#pragma warning restore 0168, 0219

pw.Write("<History");
if (typeId){
pw.Write(" xsi:type=\"History\"");
} // If
pw.Write('>');
pw.Write('\n');
unParseBody(pw);
pw.Write("</History>");
pw.Write('\n');
}

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
 override public void unParseBody(TextWriter pw)
{
#pragma warning disable 0168, 0219
int i;
#pragma warning restore 0168, 0219

// Unparsing Repeat
// Unparsing repetition
unParse(pw, this.getCommits(), false, null, null);
}
public  override  void dispatch(XmlBBaseVisitor v)
{
  ((Visitor)v).visit(this);
}

public  override  void dispatch(XmlBBaseVisitor v, bool visitSubNodes)
{
  ((Visitor)v).visit(this, visitSubNodes);
}
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override void subElements(ArrayList l)
{
for (int i = 0; i < countCommits(); i++) {
  l.Add(getCommits(i));
}
}

}
public partial class Commit
: HistoricalData.HistoricalDataElement
{
public  override  bool find(Object search){
if (search is String ) {
if(getMessage().CompareTo((String) search) == 0)return true;
if(getDate().CompareTo((String) search) == 0)return true;
if(getHash().CompareTo((String) search) == 0)return true;
if(getCommitter().CompareTo((String) search) == 0)return true;
}
return false;
}

public  override  void NotifyControllers(Lock aLock){
	base.NotifyControllers(aLock);
	ControllersManager.CommitController.alertChange(aLock, this);
}
private   string  aMessage;

public   string  getMessage() { return aMessage;}

public  void setMessage( string  v) {
  aMessage = v;
  __setDirty(true);
  NotifyControllers(null);
}


private   string  aDate;

public   string  getDate() { return aDate;}

public  void setDate( string  v) {
  aDate = v;
  __setDirty(true);
  NotifyControllers(null);
}


private   string  aHash;

public   string  getHash() { return aHash;}

public  void setHash( string  v) {
  aHash = v;
  __setDirty(true);
  NotifyControllers(null);
}


private   string  aCommitter;

public   string  getCommitter() { return aCommitter;}

public  void setCommitter( string  v) {
  aCommitter = v;
  __setDirty(true);
  NotifyControllers(null);
}


private System.Collections.ArrayList aChanges;

/// <summary>Part of the list interface for Changes</summary>
/// <returns>a collection of all the elements in Changes</returns>
public System.Collections.ArrayList allChanges()
  {
if (aChanges == null){
    setAllChanges( new System.Collections.ArrayList() );
} // If
    return aChanges;
  }

/// <summary>Part of the list interface for Changes</summary>
/// <returns>a collection of all the elements in Changes</returns>
private System.Collections.ArrayList getChanges()
  {
    return allChanges();
  }

/// <summary>Part of the list interface for Changes</summary>
/// <param name="coll">a collection of elements which replaces 
///        Changes's current content.</param>
public void setAllChanges(System.Collections.ArrayList coll)
  {
  __setDirty(true);
    aChanges = coll;
    NotifyControllers(null);
  }
public void setAllChanges(Lock aLock,System.Collections.ArrayList coll)
  {
  __setDirty(true);
    aChanges = coll;
NotifyControllers(aLock);
  }

/// <summary>Part of the list interface for Changes</summary>
/// <param name="el">a Change to add to the collection in 
///           Changes</param>
/// <seealso cref="appendChanges(System.Collections.IList)"/>
public void appendChanges(Change el)
  {
  __setDirty(true);
  el.__setDirty(true);
  allChanges().Add(el);
  acceptor.connectSon (this, el);
NotifyControllers(null);
  }

public void appendChanges(Lock aLock,Change el)
  {
  __setDirty(true);
  el.__setDirty(true);
  allChanges().Add(el);
  acceptor.connectSon (this, el);
NotifyControllers(aLock);
  }
/// <summary>Part of the list interface for Changes</summary>
/// <param name="coll">a collection ofChanges to add to the collection in 
///           Changes</param>
/// <seealso cref="appendChanges(Change)"/>
public void appendChanges(System.Collections.IList coll)
  {
  __setDirty(true);
  allChanges().AddRange(coll);
  acceptor.connectSons (this, coll);
NotifyControllers(null);
  }

public void appendChanges(System.Collections.IList coll,Lock aLock)
  {
  __setDirty(true);
  allChanges().AddRange(coll);
  acceptor.connectSons (this, coll);
NotifyControllers(aLock);
  }

/// <summary>Part of the list interface for Changes
/// This insertion function inserts a new element in the
/// collection in Changes</summary>
/// <param name="idx">the index where the insertion must take place</param>
/// <param name="el">the element to insert</param>
public void insertChanges(int idx, Change el)
  {
  __setDirty(true);
  allChanges().Insert (idx, el);
NotifyControllers(null);
  }

public void insertChanges(int idx, Change el,Lock aLock)
  {
  __setDirty(true);
  allChanges().Insert (idx, el);
NotifyControllers(aLock);
  }

/// <summary>Part of the list interface for Changes
/// This function returns the index of an element in
/// the collection.</summary>
/// <param name="el">the object to look for</param>
/// <returns>the index where it is found, or -1 if it is not.</returns>
public int indexOfChanges(IXmlBBase el)
  {
  return ((System.Collections.IList) allChanges()).IndexOf (el);
  }

/// <summary>Part of the list interface for Changes
/// This deletion function removes an element from the
/// collection in Changes</summary>
/// <param name="idx">the index of the element to remove</param>
public void deleteChanges(int idx)
  {
  __setDirty(true);
  allChanges().RemoveAt(idx);
NotifyControllers(null);
  }

public void deleteChanges(int idx,Lock aLock)
  {
  __setDirty(true);
  allChanges().RemoveAt(idx);
NotifyControllers(aLock);
  }

/// <summary>Part of the list interface for Changes
/// This deletion function removes an element from the
/// collection in Changes
/// If the object given in parameter is not found in the
/// the collection, this function does nothing.</summary>
/// <param name="obj">the object to remove</param>
public void removeChanges(IXmlBBase obj)
  {
  int idx = indexOfChanges(obj);
  if (idx >= 0) { deleteChanges(idx);
NotifyControllers(null);
   }
  }

public void removeChanges(IXmlBBase obj,Lock aLock)
  {
  int idx = indexOfChanges(obj);
  if (idx >= 0) { deleteChanges(idx);
NotifyControllers(aLock);
  }}

/// <summary>Part of the list interface for Changes</summary>
/// <returns>the number of elements in Changes</returns>
public int countChanges()
  {
  return allChanges().Count;
  }

/// <summary>Part of the list interface for Changes
/// This function returns an element from the
/// collection in Changes based on an index.</summary>
/// <param name="idx">the index of the element to extract</param>
/// <returns>the extracted element</returns>
public Change getChanges(int idx)
{
  return (Change) ( allChanges()[idx]);
}

public Commit()
{
Commit obj = this;
aMessage=(null);
aDate=(null);
aHash=(null);
aCommitter=(null);
aChanges=(null);
}

public void copyTo(Commit other)
{
other.aMessage = aMessage;
other.aDate = aDate;
other.aHash = aHash;
other.aCommitter = aCommitter;
other.aChanges = aChanges;
}

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override void parseBody(XmlBContext ctxt)

{
#pragma warning disable 0168, 0219
int indicator=0;
char quoteChar;
 string  tempStr;
#pragma warning restore 0168, 0219
bool fl114;
Change fl116;

ctxt.skipWhiteSpace();
ctxt.skipWhiteSpace();
// Optional Enclosed
if (ctxt.lookAheadOpeningTag("<Message")){
ctxt.skipWhiteSpace();
fl114 = true ; 
while (fl114) { // BeginLoop 
ctxt.skipWhiteSpace();
if (ctxt.isAlNum()){
ctxt.skipTill ('=');
ctxt.advance();
ctxt.skipWhiteSpace();
quoteChar = ctxt.acceptQuote();
ctxt.skipTill (quoteChar);
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
} else {
fl114 = false ; 
} // If
} // While
ctxt.accept('>');
// Indicator
// Parse PC data
this.setMessage(acceptor.lAcceptPcData(ctxt, -1, '<',XmlBContext.WS_PRESERVE));
// Regexp
ctxt.skipWhiteSpace();
ctxt.acceptString ("</Message>");
} // If
// End enclosed
// Repeat
ctxt.skipWhiteSpace();
fl116 = null;
while(ctxt.lookAheadOpeningTag ("<Change")) {
fl116 = acceptor.lAccept_Change(ctxt, null);
appendChanges(fl116);
ctxt.skipWhiteSpace();
} // -- monomorphic Loop
// EndRepeat
ctxt.skipWhiteSpace();
}


/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override  void parse(XmlBContext ctxt,  string  endingTag)

{
#pragma warning disable 0168, 0219
int indicator = 0;
char quoteChar;
 string  tempStr = null;
bool fl127;
bool fl128;
bool fl129;
bool fl130;
#pragma warning restore 0168, 0219

ctxt.skipWhiteSpace();
{
// Accept Attributes
fl127 = false ; 
fl128 = false ; 
fl129 = false ; 
fl130 = true ; 
while (fl130) { // BeginLoop 
switch (ctxt.current()) {
case 'H':
{
ctxt.advance();
if (ctxt.lookAheadString("ash=")){
indicator = 127;
} else {
indicator = 131;
} // If
break;
} // Case
case 'D':
{
ctxt.advance();
if (ctxt.lookAheadString("ate=")){
indicator = 129;
} else {
indicator = 131;
} // If
break;
} // Case
case 'C':
{
ctxt.advance();
if (ctxt.lookAheadString("ommitter=")){
indicator = 128;
} else {
indicator = 131;
} // If
break;
} // Case
default:
indicator = 131;
break;
} // Switch
switch (indicator) {
case 127: {
// Handling attribute Hash
// Also handles alien attributes with prefix Hash
if (fl127){
ctxt.fail ("Duplicate attribute: Hash");
} // If
fl127 = true ; 
quoteChar = ctxt.acceptQuote();
this.setHash((acceptor.lAcceptPcData(ctxt,-1, quoteChar, XmlBContext.WS_PRESERVE)));
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
break;
} // End of dispatch label
case 128: {
// Handling attribute Committer
// Also handles alien attributes with prefix Committer
if (fl128){
ctxt.fail ("Duplicate attribute: Committer");
} // If
fl128 = true ; 
quoteChar = ctxt.acceptQuote();
this.setCommitter((acceptor.lAcceptPcData(ctxt,-1, quoteChar, XmlBContext.WS_PRESERVE)));
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
break;
} // End of dispatch label
case 129: {
// Handling attribute Date
// Also handles alien attributes with prefix Date
if (fl129){
ctxt.fail ("Duplicate attribute: Date");
} // If
fl129 = true ; 
quoteChar = ctxt.acceptQuote();
this.setDate((acceptor.lAcceptPcData(ctxt,-1, quoteChar, XmlBContext.WS_PRESERVE)));
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
break;
} // End of dispatch label
// Final default label
case 131: {
// Taking ignorable attributes into account
if (ctxt.isAlNum()){
ctxt.skipTill ('=');
ctxt.advance();
ctxt.skipWhiteSpace();
quoteChar = ctxt.acceptQuote();
ctxt.skipTill (quoteChar);
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
} else {
if (!fl127){
ctxt.fail ("Mandatory attribute missing: Hash in Commit");
} // If
if (!fl128){
ctxt.fail ("Mandatory attribute missing: Committer in Commit");
} // If
if (!fl129){
ctxt.fail ("Mandatory attribute missing: Date in Commit");
} // If
fl130 = false ; 
} // If
break;
} // End of dispatch label
} // Dispatch
} // While
}
ctxt.skipWhiteSpace();
if (ctxt.current() == '/'){
ctxt.advance();
ctxt.accept('>');
} else {
ctxt.accept('>');
parseBody(ctxt);
ctxt.acceptString ("</Commit>");
// If formula empty
} // If
}

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
 override  public  void unParse(TextWriter pw,
                    bool typeId,
                     string  headingTag,
                     string  endingTag)
{
#pragma warning disable 0168, 0219
int i;
#pragma warning restore 0168, 0219

pw.Write("<Commit");
if (typeId){
pw.Write(" xsi:type=\"Commit\"");
} // If
pw.Write('\n');
pw.Write(" Hash=\"");
acceptor.unParsePcData(pw, this.getHash());
pw.Write('"');
pw.Write('\n');
pw.Write(" Committer=\"");
acceptor.unParsePcData(pw, this.getCommitter());
pw.Write('"');
pw.Write('\n');
pw.Write(" Date=\"");
acceptor.unParsePcData(pw, this.getDate());
pw.Write('"');
pw.Write('\n');
pw.Write('>');
pw.Write('\n');
unParseBody(pw);
pw.Write("</Commit>");
pw.Write('\n');
}

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
 override public void unParseBody(TextWriter pw)
{
#pragma warning disable 0168, 0219
int i;
#pragma warning restore 0168, 0219

// Unparsing Enclosed
// Testing for empty content: Message
if (this.getMessage() != null){
pw.Write("<Message>");
// Unparsing PcData
acceptor.unParsePcData(pw, this.getMessage());
pw.Write("</Message>");
// Father is not a mixed
pw.Write('\n');
} // If
// After Testing for empty content: Message
// Unparsing Repeat
// Unparsing repetition
unParse(pw, this.getChanges(), false, null, null);
}
public  override  void dispatch(XmlBBaseVisitor v)
{
  ((Visitor)v).visit(this);
}

public  override  void dispatch(XmlBBaseVisitor v, bool visitSubNodes)
{
  ((Visitor)v).visit(this, visitSubNodes);
}
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override void subElements(ArrayList l)
{
for (int i = 0; i < countChanges(); i++) {
  l.Add(getChanges(i));
}
}

}
public partial class Change
: HistoricalData.HistoricalDataElement
{
public  override  bool find(Object search){
if (search is String ) {
if(getGuid().CompareTo((String) search) == 0)return true;
if(getBefore().CompareTo((String) search) == 0)return true;
if(getAfter().CompareTo((String) search) == 0)return true;
if(getField().CompareTo((String) search) == 0)return true;
}
return false;
}

public  override  void NotifyControllers(Lock aLock){
	base.NotifyControllers(aLock);
	ControllersManager.ChangeController.alertChange(aLock, this);
}
private   string  aGuid;

public   string  getGuid() { return aGuid;}

public  void setGuid( string  v) {
  aGuid = v;
  __setDirty(true);
  NotifyControllers(null);
}


private   string  aBefore;

public   string  getBefore() { return aBefore;}

public  void setBefore( string  v) {
  aBefore = v;
  __setDirty(true);
  NotifyControllers(null);
}


private   string  aAfter;

public   string  getAfter() { return aAfter;}

public  void setAfter( string  v) {
  aAfter = v;
  __setDirty(true);
  NotifyControllers(null);
}


private  acceptor.ChangeOperationEnum aOperation;

public  acceptor.ChangeOperationEnum getOperation() { return aOperation;}

public  void setOperation(acceptor.ChangeOperationEnum v) {
  aOperation = v;
  __setDirty(true);
  NotifyControllers(null);
}


public  string   getOperation_AsString()
{
  return acceptor.Enum_ChangeOperationEnum_ToString (aOperation);
}

public  bool setOperation_AsString( string  v)
{
 acceptor.ChangeOperationEnum  temp = acceptor.StringTo_Enum_ChangeOperationEnum(v);
if (temp >= 0){
  aOperation = temp;
  __setDirty(true);
  NotifyControllers(null);
  return true;
} // If
return false;
}

private   string  aField;

public   string  getField() { return aField;}

public  void setField( string  v) {
  aField = v;
  __setDirty(true);
  NotifyControllers(null);
}


public Change()
{
Change obj = this;
aGuid=(null);
aBefore=(null);
aAfter=(null);
aOperation=(0);
aField=(null);
}

public void copyTo(Change other)
{
other.aGuid = aGuid;
other.aBefore = aBefore;
other.aAfter = aAfter;
other.aOperation = aOperation;
other.aField = aField;
}

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override void parseBody(XmlBContext ctxt)

{
#pragma warning disable 0168, 0219
int indicator=0;
char quoteChar;
 string  tempStr;
#pragma warning restore 0168, 0219
bool fl136;
bool fl137;

ctxt.skipWhiteSpace();
ctxt.skipWhiteSpace();
// Enclosed
ctxt.acceptString ("<Before");
if (ctxt.isAlNum()){
ctxt.fail ("White space expected after TAG");
} // If
ctxt.skipWhiteSpace();
fl136 = true ; 
while (fl136) { // BeginLoop 
ctxt.skipWhiteSpace();
if (ctxt.isAlNum()){
ctxt.skipTill ('=');
ctxt.advance();
ctxt.skipWhiteSpace();
quoteChar = ctxt.acceptQuote();
ctxt.skipTill (quoteChar);
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
} else {
fl136 = false ; 
} // If
} // While
ctxt.accept('>');
// Indicator
// Parse PC data
this.setBefore(acceptor.lAcceptPcData(ctxt, -1, '<',XmlBContext.WS_PRESERVE));
// Regexp
ctxt.skipWhiteSpace();
ctxt.acceptString ("</Before>");
// End enclosed
ctxt.skipWhiteSpace();
// Enclosed
ctxt.acceptString ("<After");
if (ctxt.isAlNum()){
ctxt.fail ("White space expected after TAG");
} // If
ctxt.skipWhiteSpace();
fl137 = true ; 
while (fl137) { // BeginLoop 
ctxt.skipWhiteSpace();
if (ctxt.isAlNum()){
ctxt.skipTill ('=');
ctxt.advance();
ctxt.skipWhiteSpace();
quoteChar = ctxt.acceptQuote();
ctxt.skipTill (quoteChar);
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
} else {
fl137 = false ; 
} // If
} // While
ctxt.accept('>');
// Indicator
// Parse PC data
this.setAfter(acceptor.lAcceptPcData(ctxt, -1, '<',XmlBContext.WS_PRESERVE));
// Regexp
ctxt.skipWhiteSpace();
ctxt.acceptString ("</After>");
// End enclosed
ctxt.skipWhiteSpace();
}


/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override  void parse(XmlBContext ctxt,  string  endingTag)

{
#pragma warning disable 0168, 0219
int indicator = 0;
char quoteChar;
 string  tempStr = null;
bool fl138;
bool fl139;
bool fl140;
bool fl141;
#pragma warning restore 0168, 0219

ctxt.skipWhiteSpace();
{
// Accept Attributes
fl138 = false ; 
fl139 = false ; 
fl140 = false ; 
fl141 = true ; 
while (fl141) { // BeginLoop 
switch (ctxt.current()) {
case 'O':
{
ctxt.advance();
if (ctxt.lookAheadString("peration=")){
indicator = 140;
} else {
indicator = 142;
} // If
break;
} // Case
case 'G':
{
ctxt.advance();
if (ctxt.lookAheadString("uid=")){
indicator = 138;
} else {
indicator = 142;
} // If
break;
} // Case
case 'F':
{
ctxt.advance();
if (ctxt.lookAheadString("ield=")){
indicator = 139;
} else {
indicator = 142;
} // If
break;
} // Case
default:
indicator = 142;
break;
} // Switch
switch (indicator) {
case 138: {
// Handling attribute Guid
// Also handles alien attributes with prefix Guid
if (fl138){
ctxt.fail ("Duplicate attribute: Guid");
} // If
fl138 = true ; 
quoteChar = ctxt.acceptQuote();
this.setGuid((acceptor.lAcceptPcData(ctxt,-1, quoteChar, XmlBContext.WS_PRESERVE)));
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
break;
} // End of dispatch label
case 139: {
// Handling attribute Field
// Also handles alien attributes with prefix Field
if (fl139){
ctxt.fail ("Duplicate attribute: Field");
} // If
fl139 = true ; 
quoteChar = ctxt.acceptQuote();
this.setField((acceptor.lAcceptPcData(ctxt,-1, quoteChar, XmlBContext.WS_PRESERVE)));
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
break;
} // End of dispatch label
case 140: {
// Handling attribute Operation
// Also handles alien attributes with prefix Operation
if (fl140){
ctxt.fail ("Duplicate attribute: Operation");
} // If
fl140 = true ; 
quoteChar = ctxt.acceptQuote();
this.setOperation(acceptor.lAcceptEnum_ChangeOperationEnum(ctxt));
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
break;
} // End of dispatch label
// Final default label
case 142: {
// Taking ignorable attributes into account
if (ctxt.isAlNum()){
ctxt.skipTill ('=');
ctxt.advance();
ctxt.skipWhiteSpace();
quoteChar = ctxt.acceptQuote();
ctxt.skipTill (quoteChar);
ctxt.accept(quoteChar);
ctxt.skipWhiteSpace();
} else {
if (!fl138){
ctxt.fail ("Mandatory attribute missing: Guid in Change");
} // If
if (!fl139){
ctxt.fail ("Mandatory attribute missing: Field in Change");
} // If
if (!fl140){
ctxt.fail ("Mandatory attribute missing: Operation in Change");
} // If
fl141 = false ; 
} // If
break;
} // End of dispatch label
} // Dispatch
} // While
}
ctxt.skipWhiteSpace();
ctxt.accept('>');
parseBody(ctxt);
ctxt.acceptString ("</Change>");
}

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
 override  public  void unParse(TextWriter pw,
                    bool typeId,
                     string  headingTag,
                     string  endingTag)
{
#pragma warning disable 0168, 0219
int i;
#pragma warning restore 0168, 0219

pw.Write("<Change");
if (typeId){
pw.Write(" xsi:type=\"Change\"");
} // If
pw.Write('\n');
pw.Write(" Guid=\"");
acceptor.unParsePcData(pw, this.getGuid());
pw.Write('"');
pw.Write('\n');
pw.Write(" Field=\"");
acceptor.unParsePcData(pw, this.getField());
pw.Write('"');
pw.Write('\n');
pw.Write(" Operation=\"");
acceptor.unParsePcData(pw,
  acceptor.Enum_ChangeOperationEnum_ToString(this.getOperation()));
pw.Write('"');
pw.Write('\n');
pw.Write('>');
pw.Write('\n');
unParseBody(pw);
pw.Write("</Change>");
pw.Write('\n');
}

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
 override public void unParseBody(TextWriter pw)
{
#pragma warning disable 0168, 0219
int i;
#pragma warning restore 0168, 0219

// Unparsing Enclosed
pw.Write("<Before>");
// Unparsing PcData
acceptor.unParsePcData(pw, this.getBefore());
pw.Write("</Before>");
// Father is not a mixed
pw.Write('\n');
// Unparsing Enclosed
pw.Write("<After>");
// Unparsing PcData
acceptor.unParsePcData(pw, this.getAfter());
pw.Write("</After>");
// Father is not a mixed
pw.Write('\n');
}
public  override  void dispatch(XmlBBaseVisitor v)
{
  ((Visitor)v).visit(this);
}

public  override  void dispatch(XmlBBaseVisitor v, bool visitSubNodes)
{
  ((Visitor)v).visit(this, visitSubNodes);
}
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override void subElements(ArrayList l)
{
}

}
public partial class ControllersManager
{
//History  History
public static Controller<History, IListener<History>> HistoryController = new Controller<History, IListener<History>>();
//Commit  Commit
public static Controller<Commit, IListener<Commit>> CommitController = new Controller<Commit, IListener<Commit>>();
//Change  Change
public static Controller<Change, IListener<Change>> ChangeController = new Controller<Change, IListener<Change>>();
public static void ActivateAllNotifications(){
HistoryController.ActivateNotification();
CommitController.ActivateNotification();
ChangeController.ActivateNotification();
}
public static void DesactivateAllNotifications(){
HistoryController.DesactivateNotification();
CommitController.DesactivateNotification();
ChangeController.DesactivateNotification();
}
}
public partial class acceptor
: XmlBBaseAcceptor
{

public enum ChangeOperationEnum {
     defaultChangeOperationEnum,
     aChange,
     aAdd,
     aRemove
};

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public static ChangeOperationEnum lAcceptEnum_ChangeOperationEnum (XmlBContext ctxt)

{
#pragma warning disable 0168, 0219
  int indicator=0;
#pragma warning restore 0168, 0219
  ChangeOperationEnum res = ChangeOperationEnum.defaultChangeOperationEnum;
switch (ctxt.current()) {
case 'R':
{
ctxt.advance();
if (ctxt.lookAheadString("emove")){
res = ChangeOperationEnum.aRemove;
} else {
ctxt.moveBack(1);
res = 0;
} // If
break;
} // Case
case 'C':
{
ctxt.advance();
if (ctxt.lookAheadString("hange")){
res = ChangeOperationEnum.aChange;
} else {
ctxt.moveBack(1);
res = 0;
} // If
break;
} // Case
case 'A':
{
ctxt.advance();
if (ctxt.lookAhead2('d','d')){
res = ChangeOperationEnum.aAdd;
} else {
ctxt.moveBack(1);
res = 0;
} // If
break;
} // Case
default:
res = 0;
break;
} // Switch
return res;
}

public static  string  Enum_ChangeOperationEnum_ToString (ChangeOperationEnum v)
{
switch (v) {
 case ChangeOperationEnum.aChange: return "Change";
 case ChangeOperationEnum.aAdd: return "Add";
 case ChangeOperationEnum.aRemove: return "Remove";
} return "";
}

public static ChangeOperationEnum StringTo_Enum_ChangeOperationEnum( string  str)
{
if (str.Equals("Change")){
  return ChangeOperationEnum.aChange;
} // If
if (str.Equals("Add")){
  return ChangeOperationEnum.aAdd;
} // If
if (str.Equals("Remove")){
  return ChangeOperationEnum.aRemove;
} // If
return ChangeOperationEnum.defaultChangeOperationEnum;
}


/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public static bool lAcceptBoolean (XmlBContext ctxt)

{
#pragma warning disable 0168, 0219
  int indicator = 0;
#pragma warning restore 0168, 0219
  bool res = false;
switch (ctxt.current()) {
case 'y':
{
ctxt.advance();
switch (ctxt.current()) {
case 'e':
{
ctxt.advance();
if (ctxt.lookAhead1('s')){
res = true;
} else {
res = true;
} // If
break;
} // Case
default:
res = true;
break;
} // Switch
break;
} // Case
case 't':
{
ctxt.advance();
ctxt.accept3('r','u','e');
res = true;
break;
} // Case
case 'o':
{
ctxt.advance();
switch (ctxt.current()) {
case 'n':
{
ctxt.advance();
res = true;
break;
} // Case
case 'f':
{
ctxt.advance();
ctxt.accept('f');
res = false;
break;
} // Case
default:
ctxt.recoverableFail ("Other character expected (156)");
break;
} // Switch
break;
} // Case
case 'n':
{
ctxt.advance();
switch (ctxt.current()) {
case 'o':
{
ctxt.advance();
res = false;
break;
} // Case
default:
res = false;
break;
} // Switch
break;
} // Case
case 'f':
{
ctxt.advance();
ctxt.acceptString ("alse");
res = false;
break;
} // Case
case 'Y':
{
ctxt.advance();
switch (ctxt.current()) {
case 'E':
{
ctxt.advance();
if (ctxt.lookAhead1('S')){
res = true;
} else {
res = true;
} // If
break;
} // Case
default:
res = true;
break;
} // Switch
break;
} // Case
case 'T':
{
ctxt.advance();
switch (ctxt.current()) {
case 'r':
{
ctxt.advance();
ctxt.accept2('u','e');
res = true;
break;
} // Case
case 'R':
{
ctxt.advance();
ctxt.accept2('U','E');
res = true;
break;
} // Case
default:
ctxt.recoverableFail ("Other character expected (165)");
break;
} // Switch
break;
} // Case
case 'O':
{
ctxt.advance();
switch (ctxt.current()) {
case 'n':
{
ctxt.advance();
res = true;
break;
} // Case
case 'f':
{
ctxt.advance();
ctxt.accept('f');
res = false;
break;
} // Case
case 'N':
{
ctxt.advance();
res = true;
break;
} // Case
case 'F':
{
ctxt.advance();
ctxt.accept('F');
res = false;
break;
} // Case
default:
ctxt.recoverableFail ("Other character expected (171)");
break;
} // Switch
break;
} // Case
case 'N':
{
ctxt.advance();
switch (ctxt.current()) {
case 'O':
{
ctxt.advance();
res = false;
break;
} // Case
default:
res = false;
break;
} // Switch
break;
} // Case
case 'F':
{
ctxt.advance();
switch (ctxt.current()) {
case 'a':
{
ctxt.advance();
ctxt.accept3('l','s','e');
res = false;
break;
} // Case
case 'A':
{
ctxt.advance();
ctxt.accept3('L','S','E');
res = false;
break;
} // Case
default:
ctxt.recoverableFail ("Other character expected (177)");
break;
} // Switch
break;
} // Case
case '1':
{
ctxt.advance();
res = true;
break;
} // Case
case '0':
{
ctxt.advance();
res = false;
break;
} // Case
default:
ctxt.recoverableFail ("Other character expected (180)");
break;
} // Switch
return res;
}
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public static History lAccept_History (XmlBContext ctxt, 
                          string  endingTag)

  {
  History res = aFactory.createHistory();
  res.parse(ctxt, endingTag);
  return res;
  }

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public static Commit lAccept_Commit (XmlBContext ctxt, 
                          string  endingTag)

  {
  Commit res = aFactory.createCommit();
  res.parse(ctxt, endingTag);
  return res;
  }

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public static Change lAccept_Change (XmlBContext ctxt, 
                          string  endingTag)

  {
  Change res = aFactory.createChange();
  res.parse(ctxt, endingTag);
  return res;
  }

/// <summary>Utility function which parse an entity character 
/// as defined in the XMLBooster configuration.</summary>
/// <param name="ctxt">the context from which the data must be parsed</param>
static char lAcceptPcDataChr(XmlBContext ctxt)

{
    char c = (char)0;
    int indicator=0;
switch (ctxt.current()) {
case 'q':
{
ctxt.advance();
ctxt.acceptString ("uot;");
indicator = 185;
break;
} // Case
case 'n':
{
ctxt.advance();
ctxt.acceptString ("bsp;");
indicator = 184;
break;
} // Case
case 'l':
{
ctxt.advance();
ctxt.accept2('t',';');
indicator = 182;
break;
} // Case
case 'g':
{
ctxt.advance();
ctxt.accept2('t',';');
indicator = 183;
break;
} // Case
case 'a':
{
ctxt.advance();
switch (ctxt.current()) {
case 'p':
{
ctxt.advance();
ctxt.accept3('o','s',';');
indicator = 186;
break;
} // Case
case 'm':
{
ctxt.advance();
ctxt.accept2('p',';');
indicator = 181;
break;
} // Case
default:
ctxt.recoverableFail ("Other character expected (194)");
break;
} // Switch
break;
} // Case
case '#':
{
ctxt.advance();
ctxt.accept('x');
indicator = 187;
break;
} // Case
default:
ctxt.recoverableFail ("Other character expected (196)");
break;
} // Switch
switch (indicator) {
case 181: {
c = XMLB_AMPERSAND;
break;
} // End of dispatch label
case 182: {
c = XMLB_LESS;
break;
} // End of dispatch label
case 183: {
c = XMLB_GREATER;
break;
} // End of dispatch label
case 184: {
c = XMLB_NBSP;
break;
} // End of dispatch label
case 185: {
c = XMLB_QUOT;
break;
} // End of dispatch label
case 186: {
c = XMLB_APOS;
break;
} // End of dispatch label
case 187: {
c = (char) ctxt.acceptHexa();
ctxt.accept(';');
break;
} // End of dispatch label
} // Dispatch
return c;
}
/// <summary>Utility function which parse a PCDATA component 
/// from a context. It takes the entities defined in the
/// in the configuration into account.</summary>
/// <param name="ctxt">the context from which the data must be 
///        parsed</param>
/// <param name="maxLen">the maximal number of characters that 
///        can be read.</param>
/// <param name="closingCh">a character on which parsing must stop
///        in addition to the standard &lt; character.</param>
/// <param name="wsMode">indicates PRESERVE (default), REPLACE or COLLAPSE.</param>
public static  string  lAcceptPcData(XmlBContext ctxt, 
                                   int maxLen,
                                   char closingCh,
                                   int wsMode)

 {
    char ch;
    char lastch = '.';
    System.Text.StringBuilder buff;
     string  res;

  buff = new System.Text.StringBuilder();
  bool go_on = true;
  while (go_on) 
{
  go_on = false;
  while ((ctxt.current() != '<') && (ctxt.current() != closingCh)) // while 1 
{
    ch = ctxt.current();
ctxt.advance();
if (ch == '&'){
ch = lAcceptPcDataChr(ctxt);
} else {
if (wsMode >= WS_REPLACE){
if (ch == '\t' || ch == '\n' || ch == '\r'){
ch = ' ';
} // If
if (wsMode == WS_COLLAPSE){
if ((ch == ' ') && ((lastch == ' ') || (buff.Length == 0))){
ch = (char)0;
} else {
lastch = ch;
} // If
} else {
lastch = ch;
} // If
} // If
} // If
if (ch != '\0'){
buff.Append (ch);
} // If
}
// end while
if (ctxt.current() == '<'){
if (ctxt.lookAheadString("<![CDATA[")){
     string  cdata = ctxt.acceptUntil("]]>", true);
    buff.Append (cdata);
    go_on = true;
} else {
if (ctxt.lookAhead2('<','?')){
ctxt.skipTill ('?');
ctxt.accept2('?','>');
go_on = true;
} else {
} // If
} // If
} // If
}
if (wsMode == WS_COLLAPSE && lastch == ' ' && buff. Length > 0){
res = buff.ToString (0, buff.Length -1);
} else {
res = buff.ToString();
} // If
if ((maxLen > 0) && (res.Length > maxLen)){
ctxt.recoverableFail ("Maximum length exceeded");
} // If
return res;
}
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
  private static bool requiresEscape (char a)
  {
    switch (a)
    {
      case XMLB_AMPERSAND:
      case XMLB_LESS:
      case XMLB_GREATER:
      case XMLB_QUOT:
      case XMLB_APOS:
        return true;
      default: break;
    }
    return false;
  }
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
  private static bool requiresEscape ( string  a)
  {
    for (int i=0; i < a.Length; i++)
    {
      if (requiresEscape(a[i]))
        return true;
    }
    return false	;
  }
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
  public static void unParsePcData (TextWriter pw,  string  a)
    {
      bool escaped = false;
      
      if (a == null)
      {
          return;
      }
      escaped = requiresEscape (a);
      if (! escaped)
        pw.Write (a);
      else
      {
        char c;
        for (int i = 0; i < a.Length; i++)
        {
          c = a[i];
          switch (c)
            {
              case XMLB_AMPERSAND:
                  pw.Write("&amp;"); 
                  break;
              case XMLB_LESS:
                  pw.Write("&lt;"); 
                  break;
              case XMLB_GREATER:
                  pw.Write("&gt;"); 
                  break;
              case XMLB_QUOT:
                  pw.Write("&quot;"); 
                  break;
              case XMLB_APOS:
                  pw.Write("&apos;"); 
                  break;
               default: 
                   pw.Write(c);
                   break;
            }
        }
      }
    }
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
  public static void unParsePcData (TextWriter pw, bool flag)
    {
      if (flag)
// TrueString is: TRUE
// FalseString is: FALSE
        pw.Write ("TRUE");
       else
        pw.Write("FALSE");
    }

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
  public static void unParsePcData (TextWriter pw, object obj)
    {
      if (obj != null)
        unParsePcData (pw, obj.ToString());
    }

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
  public static void unParsePcData (TextWriter pw, int val)
    {
      pw.Write (val);
    }

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
  public static void unParsePcData (TextWriter pw, long val)
    {
      pw.Write (val);
    }

/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
  public static void unParsePcData (TextWriter pw, double val)
    {
      XmlBConverter conv;
      conv = getConverter();
      if(conv != null)
        pw.Write(conv.doubleToString(val));
       else
        pw.Write (val);
    }



private static Factory aFactory;
/// <summary>Sets the factory to introduce an indirection level
/// so that the user can externally define derived classes
/// to be used in place of the XMLBooster-generated 
/// classes.</summary>
public static void setFactory (Factory f) { aFactory = f; }

/// <returns>the currently active factory.</returns>
public static Factory getFactory () { return aFactory; }
static private acceptor theOne = null;
static public acceptor getUnique()
{
  if (theOne == null) { theOne = new acceptor(); }
  return theOne;
}

static public void setUnique(acceptor acc)
{
  theOne = acc;
}


/// <summary>Top level function to parse an History from 
/// a context. This kind of function is only made
/// available for elements marked as MAIN in the 
/// metadefinition</summary>
/// <seealso cref="accept"/>
public static History acceptHistory(XmlBContext ctxt)

  {
History res;
ctxt.skipWhiteSpace();
try {
ctxt.acceptString ("<History");
if (ctxt.isAlNum()){
ctxt.fail ("White space expected after TAG");
} // If
  res = lAccept_History(ctxt, "</History>");
 } catch (XmlBRecoveryException e) {
  throw new XmlBException("Unexpected recovery exception: " +
     e.ToString());
}
  ctxt.close();
if (ctxt.errCount() > 0){
res = null;
throw new XmlBException (ctxt.errorMessage());
} // If
  return res;
  }

public static History accept(XmlBContext ctxt)

{
  return acceptHistory(ctxt);
}


/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override bool genericUnParse(TextWriter pw, IXmlBBase obj)
{
  ((XmlBBase ) obj).unParse(pw, false);
  return true;
}
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public static IXmlBBase[] subElements(IXmlBBase obj)
{
  return ((XmlBBase ) obj).subElements();
}
/// <remarks>This method is used by XMLBooster-generated code
/// internally. Please refrain from using it, as it
/// might produce unexpected results, and might change
/// or even disappear in the future.</remarks>
public  override  IXmlBBase[] genericSubElements(IXmlBBase obj)
{
  return ((XmlBBase ) obj).subElements();
}
public  override IXmlBBase genericAccept (XmlBContext ctxt)

  {
    return accept(ctxt);
  }
}
public abstract partial class Factory
{
public abstract History createHistory();
public abstract Commit createCommit();
public abstract Change createChange();
}

public partial class TestParser
{
public static void main( string [] args)
  {
   XmlBTester tester = new  XmlBTester();
   tester.performTest (acceptor.getUnique(), args);
  }
}

public partial class Visitor
: XmlBBaseVisitor
{
public virtual void visit(History obj)
{
  visit(obj, true);
}

public virtual void visit(History obj, bool visitSubNodes)
{
visit ((IXmlBBase)obj, false);
if (visitSubNodes){
IXmlBBase[] Subs  = acceptor.subElements((IXmlBBase)obj);
if (Subs != null){
for (int i=0; i<Subs.Length; i++) {
  dispatch(Subs[i], true);
} // If
} // If
}
}

public virtual void visit(Commit obj)
{
  visit(obj, true);
}

public virtual void visit(Commit obj, bool visitSubNodes)
{
visit ((IXmlBBase)obj, false);
if (visitSubNodes){
IXmlBBase[] Subs  = acceptor.subElements((IXmlBBase)obj);
if (Subs != null){
for (int i=0; i<Subs.Length; i++) {
  dispatch(Subs[i], true);
} // If
} // If
}
}

public virtual void visit(Change obj)
{
  visit(obj, true);
}

public virtual void visit(Change obj, bool visitSubNodes)
{
visit ((IXmlBBase)obj, false);
if (visitSubNodes){
IXmlBBase[] Subs  = acceptor.subElements((IXmlBBase)obj);
if (Subs != null){
for (int i=0; i<Subs.Length; i++) {
  dispatch(Subs[i], true);
} // If
} // If
}
}

public  override  void dispatch(IXmlBBase obj)
{
  dispatch (obj, true);
}

public  override  void dispatch(IXmlBBase obj, bool visitSubNodes)
{
if (obj == null){
return;
} // If
((XmlBBase)obj).dispatch(this, visitSubNodes);
} // End of dispatch methods

}
}
