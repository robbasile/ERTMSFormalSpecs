.PHONY: all release test gitclone nightbuild fullbuild java clean

all:
	make -C depends all
	make -C src all

release: clean
	make -C src release
	make distrib

distrib:
	-cp bin/DataDictionary.dll 	../../efs_dlls/released
	-cp bin/EFSServiceClient.dll 	../../efs_dlls/released
	-cp bin/ErtmsSolutions.dll 	../../efs_dlls/released
	-cp bin/GUIUtils.dll 		../../efs_dlls/released
	-cp bin/HistoricalData.dll 	../../efs_dlls/released
	-cp bin/Importer.dll 		../../efs_dlls/released
	-cp bin/Reports.dll 		../../efs_dlls/released
	-cp bin/Utils.dll  		../../efs_dlls/released
	-cp bin/XmlBooster.dll		../../efs_dlls/released

gitclone:
	-rm -rf ERTMSFormalSpecs
	../tools/Git/bin/git.exe clone https://github.com/stanpinte/ERTMSFormalSpecs.git
	cp -r ERTMSFormalSpecs/ErtmsFormalSpecs/* .

java:
#	cs2j-2013.0.1/CS2JTranslator/bin/cs2j.exe -config=src/ErtmsSolutions/CSharpToJava.ini
	cs2j-2013.0.1/CS2JTranslator/bin/cs2j.exe -net-templates-dir=cs2j-2013.0.1/NetFramework/ -out-java-dir=java/interpreter/translated -cs-dir=src/ErtmsSolutions
	cs2j-2013.0.1/CS2JTranslator/bin/cs2j.exe -net-templates-dir=cs2j-2013.0.1/NetFramework/ -out-java-dir=java/interpreter/translated -cs-dir=src/XmlBooster/src
	cs2j-2013.0.1/CS2JTranslator/bin/cs2j.exe -net-templates-dir=cs2j-2013.0.1/NetFramework/ -out-java-dir=java/interpreter/translated -cs-dir=src/Utils/src
	cs2j-2013.0.1/CS2JTranslator/bin/cs2j.exe -net-templates-dir=cs2j-2013.0.1/NetFramework/ -out-java-dir=java/interpreter/translated -cs-dir=src/DataDictionary/src
	cs2j-2013.0.1/CS2JTranslator/bin/cs2j.exe -net-templates-dir=cs2j-2013.0.1/NetFramework/ -out-java-dir=java/interpreter/translated -cs-dir=src/DataDictionary/generated

test:
	bin/EFSTester.exe "doc/specs/subset-026.efs" 
	bin/EFSTester.exe "doc/specs/subset-026.efs" "doc/specs/braking curves verification.efs"

unit_test:
	../../tools/NUnit-2.6.3/bin/nunit-console.exe src/DataDictionary.test/bin/Release/DataDictionary.test.dll /xml=nunit-result.xml

nightbuild: release test unit_test
	echo "done."

clean:
	make -C depends clean
	make -C src clean
	rm -rf distrib distrib.zip
	-mkdir -p bin

fullbuild: clean
	make -C src fullbuild