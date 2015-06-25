.phony:all remove clean
.default:all

DIR_EXEC=../bin
DIR_EXEC_EXTRA=$(DIR_EXEC)/res

DEPENDANCIES=$(DIR_EXEC)/gnuplot 
DEPENDANCIES+=$(DIR_EXEC)/PdfSharp.dll 
DEPENDANCIES+=$(DIR_EXEC)/PdfSharp.Charting.dll 
DEPENDANCIES+=$(DIR_EXEC)/MigraDoc.Rendering.dll 
DEPENDANCIES+=$(DIR_EXEC)/MigraDoc.DocumentObjectModel.dll 
DEPENDANCIES+=$(DIR_EXEC)/log4net.dll
DEPENDANCIES+=$(DIR_EXEC)/Microsoft.Office.Interop.Excel.dll
DEPENDANCIES+=$(DIR_EXEC)/LibGit2Sharp.dll
DEPENDANCIES+=$(DIR_EXEC)/ICSharpCode.SharpZipLib.dll
DEPENDANCIES+=$(DIR_EXEC)/ObjectListView.dll
DEPENDANCIES+=InnoInstaller5

all: $(DIR_EXEC) $(DEPENDANCIES)

remove:
	@-rm -rf $(DEPENDANCIES) log4net-1.2.11

clean:

$(DIR_EXEC):
	@-mkdir $(DIR_EXEC)

$(DIR_EXEC)/gnuplot: gnuplot.zip
	@echo "---- Unzipping $< "
	@unzip -u -o -q $<
	@mv gnuplot $(DIR_EXEC)
	@chmod -R 777 $@
	@touch --reference=$<  $@

$(DIR_EXEC)/PdfSharp.dll: PdfSharp/1_3_1_binaries_patched/PdfSharp.dll
	@echo "---- Copying $< "
	@cp $< ../bin

$(DIR_EXEC)/PdfSharp.Charting.dll: PdfSharp/1_3_1_binaries_patched/PdfSharp.Charting.dll
	@echo "---- Copying $< "
	@cp $< ../bin

$(DIR_EXEC)/MigraDoc.Rendering.dll: PdfSharp/1_3_1_binaries_patched/MigraDoc.Rendering.dll
	@echo "---- Copying $< "
	@cp $< ../bin

$(DIR_EXEC)/MigraDoc.DocumentObjectModel.dll: PdfSharp/1_3_1_binaries_patched/MigraDoc.DocumentObjectModel.dll
	@echo "---- Copying $< "
	@cp $< ../bin

$(DIR_EXEC)/log4net.dll: log4net-1.2.11-bin-newkey.zip
	@echo "---- Unzipping $< "
	@unzip -u -o -q $<
	@echo "---- Copying log4net.dll "
	@cp log4net-1.2.11/bin/net/4.0/release/log4net.dll ../bin
	
$(DIR_EXEC)/Microsoft.Office.Interop.Excel.dll: Microsoft.Office.Interop.Excel.dll
	@echo "---- Copying $< "
	@cp $< ../bin

$(DIR_EXEC)/LibGit2Sharp.dll: LibGit2Sharp/LibGit2Sharp.dll
	@echo "---- Copying $< "
	@cp $< ../bin
	@-mkdir ../bin/NativeBinaries
	@cp -r LibGit2Sharp/NativeBinaries/* ../bin/NativeBinaries

$(DIR_EXEC)/ICSharpCode.SharpZipLib.dll: ICSharpCode.SharpZipLib.dll
	@echo "---- Copying $< "
	@cp $< ../bin

$(DIR_EXEC)/ObjectListView.dll: ObjectListView.dll
	@cp ObjectListView.dll ../bin

InnoInstaller5: InnoInstaller5.zip
	@echo "---- Unzipping $< "
	@unzip -u -o -q $<
	@chmod -R 777 $@

