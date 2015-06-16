.phony: all import clean 
.default: all

BUILD=$(ERTMS_TOOLS_ROOT)/NET_FW_4.0/MSBuild.exe
PROJECT=CodecNTImporter
TARGET=bin/Debug/$(PROJECT).exe

all: $(TARGET) import

$(TARGET):  
	@banner $(PROJECT)
	@make clean
	@-mkdir bin
	@$(BUILD)

import:
	$(TARGET) ../../ERTMSFormalSpecs/ErtmsFormalSpecs/doc/specs/subset-026.efs data/codecNT-subset-026-v330.xml data/codecNT-subset-026-v330.xml Messages       trace
	$(TARGET) ../../ERTMSFormalSpecs/ErtmsFormalSpecs/doc/specs/subset-026.efs data/codecNT-subset-026-v330.xml data/codecNT-subset-027-v300.xml Messages.JRU notrace

clean:
	@echo Cleaning $(PROJECT)
	@rm -rf $(TARGET)	

	
