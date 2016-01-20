.PHONY: all release test gitclone nightbuild fullbuild java clean

all:
	make -C depends all
	make -C src all

release: clean
	make -C src release

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

nightbuild: release test
	echo "done."

clean:
	make -C depends clean
	make -C src clean
	rm -rf distrib distrib.zip
	-mkdir -p bin

fullbuild: clean
	make -C src fullbuild