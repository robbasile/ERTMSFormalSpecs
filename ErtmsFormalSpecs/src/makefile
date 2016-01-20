.phony:all clean increase_release_number release fullbuild
.default:all

include etc/makefile.shared

XMLBOOSTER_ROOT=$(ERTMS_TOOLS_ROOT)/xmlb_2_22_11
XMLB_CALL=$(XMLBOOSTER_ROOT)/bin/xmlbgui.exe -CFG=../../../tools/xmlb_2_22_11/xmlbcfg.xml

all: 
	@make -C Utils all 
	@make -C DataDictionary all 
	@make -C Reports all 
	@make -C Importers all 
	@make -C GUIUtils all 
	@make -C HistoricalData all
	@make -C Reports all
	@make -C GUI all 
	@make -C EFSTester all
	@make -C EFSServiceClient all

clean:
	@make -C Utils clean
	@make -C DataDictionary clean 
	@make -C Reports clean 
	@make -C Importers clean 
	@make -C GUIUtils clean 
	@make -C HistoricalData clean
	@make -C Reports clean
	@make -C GUI clean 
	@make -C EFSTester clean 
	@make -C EFSServiceClient clean

increase_release_number:
	@echo === Increase build number
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs Utils/Properties/AssemblyInfo.cs > /dev/null
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs DataDictionary/Properties/AssemblyInfo.cs > /dev/null
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs Reports/Properties/AssemblyInfo.cs > /dev/null
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs Importers/Properties/AssemblyInfo.cs > /dev/null
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs HistoricalData/Properties/AssemblyInfo.cs > /dev/null
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs Reports/Properties/AssemblyInfo.cs > /dev/null
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs GUIUtils/Properties/AssemblyInfo.cs > /dev/null
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs GUI/Properties/AssemblyInfo.cs > /dev/null
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs EFSTester/Properties/AssemblyInfo.cs > /dev/null
	@-$(XMLB_CALL) -SCRIPT update_release_number.rcs EFSServiceClient/Properties/AssemblyInfo.cs > /dev/null

patch_configuration_file:
	@echo === Patch configuration file
	@-$(XMLB_CALL) -SCRIPT patch_configuration_file.rcs ../bin/GUI.exe.config

release: clean increase_release_number patch_configuration_file Installer/EFSW_Installer_Script.iss	fullbuild
	@echo === Building Installer package	
	@../depends/InnoInstaller5/ISCC.exe Installer/EFSW_Installer_Script.iss /q

fullbuild: clean
	@echo === Rebuilding solution in Release mode
	$(BUILD) /t:Rebuild /p:Configuration=Release