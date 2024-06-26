# Microsoft Developer Studio Generated NMAKE File, Based on chatter.dsp
!IF "$(CFG)" == ""
CFG=Chatter - Win32 Release
!MESSAGE No configuration specified. Defaulting to Chatter - Win32 Release.
!ENDIF 

!IF "$(CFG)" != "Chatter - Win32 Release" && "$(CFG)" != "Chatter - Win32 Debug" && "$(CFG)" != "Chatter - Win32 Unicode Release" && "$(CFG)" != "Chatter - Win32 Unicode Debug"
!MESSAGE Invalid configuration "$(CFG)" specified.
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "chatter.mak" CFG="Chatter - Win32 Release"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "Chatter - Win32 Release" (based on "Win32 (x86) Application")
!MESSAGE "Chatter - Win32 Debug" (based on "Win32 (x86) Application")
!MESSAGE "Chatter - Win32 Unicode Release" (based on "Win32 (x86) Application")
!MESSAGE "Chatter - Win32 Unicode Debug" (based on "Win32 (x86) Application")
!MESSAGE 
!ERROR An invalid configuration is specified.
!ENDIF 

!IF "$(OS)" == "Windows_NT"
NULL=
!ELSE 
NULL=nul
!ENDIF 

CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "Chatter - Win32 Release"

OUTDIR=.\Release
INTDIR=.\Release
# Begin Custom Macros
OutDir=.\.\Release
# End Custom Macros

ALL : "$(OUTDIR)\chatter.exe"


CLEAN :
	-@erase "$(INTDIR)\Chatdoc.obj"
	-@erase "$(INTDIR)\Chatsock.obj"
	-@erase "$(INTDIR)\Chatter.obj"
	-@erase "$(INTDIR)\chatter.pch"
	-@erase "$(INTDIR)\Chatter.res"
	-@erase "$(INTDIR)\Chatvw.obj"
	-@erase "$(INTDIR)\Mainfrm.obj"
	-@erase "$(INTDIR)\Msg.obj"
	-@erase "$(INTDIR)\Sendvw.obj"
	-@erase "$(INTDIR)\Setupdlg.obj"
	-@erase "$(INTDIR)\Stdafx.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(OUTDIR)\chatter.exe"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

CPP_PROJ=/nologo /MD /W3 /GX /O2 /D "NDEBUG" /D "WIN32" /D "_WINDOWS" /D "_AFXDLL" /D "_MBCS" /Fp"$(INTDIR)\chatter.pch" /Yu"Stdafx.h" /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
MTL_PROJ=/nologo /D "NDEBUG" /mktyplib203 /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\Chatter.res" /d "NDEBUG" /d "_AFXDLL" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\chatter.bsc" 
BSC32_SBRS= \
	
LINK32=link.exe
LINK32_FLAGS=/nologo /subsystem:windows /incremental:no /pdb:"$(OUTDIR)\chatter.pdb" /machine:I386 /out:"$(OUTDIR)\chatter.exe" 
LINK32_OBJS= \
	"$(INTDIR)\Chatdoc.obj" \
	"$(INTDIR)\Chatsock.obj" \
	"$(INTDIR)\Chatter.obj" \
	"$(INTDIR)\Chatvw.obj" \
	"$(INTDIR)\Mainfrm.obj" \
	"$(INTDIR)\Msg.obj" \
	"$(INTDIR)\Sendvw.obj" \
	"$(INTDIR)\Setupdlg.obj" \
	"$(INTDIR)\Stdafx.obj" \
	"$(INTDIR)\Chatter.res"

"$(OUTDIR)\chatter.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "Chatter - Win32 Debug"

OUTDIR=.\Debug
INTDIR=.\Debug
# Begin Custom Macros
OutDir=.\.\Debug
# End Custom Macros

ALL : "$(OUTDIR)\chatter.exe"


CLEAN :
	-@erase "$(INTDIR)\Chatdoc.obj"
	-@erase "$(INTDIR)\Chatsock.obj"
	-@erase "$(INTDIR)\Chatter.obj"
	-@erase "$(INTDIR)\chatter.pch"
	-@erase "$(INTDIR)\Chatter.res"
	-@erase "$(INTDIR)\Chatvw.obj"
	-@erase "$(INTDIR)\Mainfrm.obj"
	-@erase "$(INTDIR)\Msg.obj"
	-@erase "$(INTDIR)\Sendvw.obj"
	-@erase "$(INTDIR)\Setupdlg.obj"
	-@erase "$(INTDIR)\Stdafx.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\vc60.pdb"
	-@erase "$(OUTDIR)\chatter.exe"
	-@erase "$(OUTDIR)\chatter.ilk"
	-@erase "$(OUTDIR)\chatter.pdb"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

CPP_PROJ=/nologo /MDd /W3 /Gm /GX /ZI /Od /D "_DEBUG" /D "WIN32" /D "_WINDOWS" /D "_AFXDLL" /D "_MBCS" /Fp"$(INTDIR)\chatter.pch" /Yu"Stdafx.h" /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
MTL_PROJ=/nologo /D "_DEBUG" /mktyplib203 /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\Chatter.res" /d "_DEBUG" /d "_AFXDLL" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\chatter.bsc" 
BSC32_SBRS= \
	
LINK32=link.exe
LINK32_FLAGS=/nologo /subsystem:windows /incremental:yes /pdb:"$(OUTDIR)\chatter.pdb" /debug /machine:I386 /out:"$(OUTDIR)\chatter.exe" 
LINK32_OBJS= \
	"$(INTDIR)\Chatdoc.obj" \
	"$(INTDIR)\Chatsock.obj" \
	"$(INTDIR)\Chatter.obj" \
	"$(INTDIR)\Chatvw.obj" \
	"$(INTDIR)\Mainfrm.obj" \
	"$(INTDIR)\Msg.obj" \
	"$(INTDIR)\Sendvw.obj" \
	"$(INTDIR)\Setupdlg.obj" \
	"$(INTDIR)\Stdafx.obj" \
	"$(INTDIR)\Chatter.res"

"$(OUTDIR)\chatter.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "Chatter - Win32 Unicode Release"

OUTDIR=.\UniRelease
INTDIR=.\UniRelease
# Begin Custom Macros
OutDir=.\.\UniRelease
# End Custom Macros

ALL : "$(OUTDIR)\chatter.exe"


CLEAN :
	-@erase "$(INTDIR)\Chatdoc.obj"
	-@erase "$(INTDIR)\Chatsock.obj"
	-@erase "$(INTDIR)\Chatter.obj"
	-@erase "$(INTDIR)\chatter.pch"
	-@erase "$(INTDIR)\Chatter.res"
	-@erase "$(INTDIR)\Chatvw.obj"
	-@erase "$(INTDIR)\Mainfrm.obj"
	-@erase "$(INTDIR)\Msg.obj"
	-@erase "$(INTDIR)\Sendvw.obj"
	-@erase "$(INTDIR)\Setupdlg.obj"
	-@erase "$(INTDIR)\Stdafx.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(OUTDIR)\chatter.exe"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

CPP_PROJ=/nologo /MD /W3 /GX /O2 /D "NDEBUG" /D "WIN32" /D "_WINDOWS" /D "_UNICODE" /D "_AFXDLL" /D "_MBCS" /Fp"$(INTDIR)\chatter.pch" /Yu"Stdafx.h" /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
MTL_PROJ=/nologo /D "NDEBUG" /mktyplib203 /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\Chatter.res" /d "NDEBUG" /d "_AFXDLL" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\chatter.bsc" 
BSC32_SBRS= \
	
LINK32=link.exe
LINK32_FLAGS=/nologo /entry:"wWinMainCRTStartup" /subsystem:windows /incremental:no /pdb:"$(OUTDIR)\chatter.pdb" /machine:I386 /out:"$(OUTDIR)\chatter.exe" 
LINK32_OBJS= \
	"$(INTDIR)\Chatdoc.obj" \
	"$(INTDIR)\Chatsock.obj" \
	"$(INTDIR)\Chatter.obj" \
	"$(INTDIR)\Chatvw.obj" \
	"$(INTDIR)\Mainfrm.obj" \
	"$(INTDIR)\Msg.obj" \
	"$(INTDIR)\Sendvw.obj" \
	"$(INTDIR)\Setupdlg.obj" \
	"$(INTDIR)\Stdafx.obj" \
	"$(INTDIR)\Chatter.res"

"$(OUTDIR)\chatter.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "Chatter - Win32 Unicode Debug"

OUTDIR=.\UniDebug
INTDIR=.\UniDebug
# Begin Custom Macros
OutDir=.\.\UniDebug
# End Custom Macros

ALL : "$(OUTDIR)\chatter.exe"


CLEAN :
	-@erase "$(INTDIR)\Chatdoc.obj"
	-@erase "$(INTDIR)\Chatsock.obj"
	-@erase "$(INTDIR)\Chatter.obj"
	-@erase "$(INTDIR)\chatter.pch"
	-@erase "$(INTDIR)\Chatter.res"
	-@erase "$(INTDIR)\Chatvw.obj"
	-@erase "$(INTDIR)\Mainfrm.obj"
	-@erase "$(INTDIR)\Msg.obj"
	-@erase "$(INTDIR)\Sendvw.obj"
	-@erase "$(INTDIR)\Setupdlg.obj"
	-@erase "$(INTDIR)\Stdafx.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\vc60.pdb"
	-@erase "$(OUTDIR)\chatter.exe"
	-@erase "$(OUTDIR)\chatter.ilk"
	-@erase "$(OUTDIR)\chatter.pdb"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

CPP_PROJ=/nologo /MDd /W3 /Gm /GX /ZI /Od /D "_DEBUG" /D "WIN32" /D "_WINDOWS" /D "_UNICODE" /D "_AFXDLL" /D "_MBCS" /Fp"$(INTDIR)\chatter.pch" /Yu"Stdafx.h" /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
MTL_PROJ=/nologo /D "_DEBUG" /mktyplib203 /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\Chatter.res" /d "_DEBUG" /d "_AFXDLL" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\chatter.bsc" 
BSC32_SBRS= \
	
LINK32=link.exe
LINK32_FLAGS=/nologo /entry:"wWinMainCRTStartup" /subsystem:windows /incremental:yes /pdb:"$(OUTDIR)\chatter.pdb" /debug /machine:I386 /out:"$(OUTDIR)\chatter.exe" 
LINK32_OBJS= \
	"$(INTDIR)\Chatdoc.obj" \
	"$(INTDIR)\Chatsock.obj" \
	"$(INTDIR)\Chatter.obj" \
	"$(INTDIR)\Chatvw.obj" \
	"$(INTDIR)\Mainfrm.obj" \
	"$(INTDIR)\Msg.obj" \
	"$(INTDIR)\Sendvw.obj" \
	"$(INTDIR)\Setupdlg.obj" \
	"$(INTDIR)\Stdafx.obj" \
	"$(INTDIR)\Chatter.res"

"$(OUTDIR)\chatter.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ENDIF 

.c{$(INTDIR)}.obj::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cpp{$(INTDIR)}.obj::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cxx{$(INTDIR)}.obj::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.c{$(INTDIR)}.sbr::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cpp{$(INTDIR)}.sbr::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cxx{$(INTDIR)}.sbr::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<


!IF "$(NO_EXTERNAL_DEPS)" != "1"
!IF EXISTS("chatter.dep")
!INCLUDE "chatter.dep"
!ELSE 
!MESSAGE Warning: cannot find "chatter.dep"
!ENDIF 
!ENDIF 


!IF "$(CFG)" == "Chatter - Win32 Release" || "$(CFG)" == "Chatter - Win32 Debug" || "$(CFG)" == "Chatter - Win32 Unicode Release" || "$(CFG)" == "Chatter - Win32 Unicode Debug"
SOURCE=.\Chatdoc.cpp

"$(INTDIR)\Chatdoc.obj" : $(SOURCE) "$(INTDIR)" "$(INTDIR)\chatter.pch"


SOURCE=.\Chatsock.cpp

"$(INTDIR)\Chatsock.obj" : $(SOURCE) "$(INTDIR)" "$(INTDIR)\chatter.pch"


SOURCE=.\Chatter.cpp

"$(INTDIR)\Chatter.obj" : $(SOURCE) "$(INTDIR)" "$(INTDIR)\chatter.pch"


SOURCE=.\Chatter.rc

"$(INTDIR)\Chatter.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) $(RSC_PROJ) $(SOURCE)


SOURCE=.\Chatvw.cpp

"$(INTDIR)\Chatvw.obj" : $(SOURCE) "$(INTDIR)" "$(INTDIR)\chatter.pch"


SOURCE=.\Mainfrm.cpp

"$(INTDIR)\Mainfrm.obj" : $(SOURCE) "$(INTDIR)" "$(INTDIR)\chatter.pch"


SOURCE=.\Msg.cpp

"$(INTDIR)\Msg.obj" : $(SOURCE) "$(INTDIR)" "$(INTDIR)\chatter.pch"


SOURCE=.\Sendvw.cpp

"$(INTDIR)\Sendvw.obj" : $(SOURCE) "$(INTDIR)" "$(INTDIR)\chatter.pch"


SOURCE=.\Setupdlg.cpp

"$(INTDIR)\Setupdlg.obj" : $(SOURCE) "$(INTDIR)" "$(INTDIR)\chatter.pch"


SOURCE=.\Stdafx.cpp

!IF  "$(CFG)" == "Chatter - Win32 Release"

CPP_SWITCHES=/nologo /MD /W3 /GX /O2 /D "NDEBUG" /D "WIN32" /D "_WINDOWS" /D "_AFXDLL" /D "_MBCS" /Fp"$(INTDIR)\chatter.pch" /Yc /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 

"$(INTDIR)\Stdafx.obj"	"$(INTDIR)\chatter.pch" : $(SOURCE) "$(INTDIR)"
	$(CPP) @<<
  $(CPP_SWITCHES) $(SOURCE)
<<


!ELSEIF  "$(CFG)" == "Chatter - Win32 Debug"

CPP_SWITCHES=/nologo /MDd /W3 /Gm /GX /ZI /Od /D "_DEBUG" /D "WIN32" /D "_WINDOWS" /D "_AFXDLL" /D "_MBCS" /Fp"$(INTDIR)\chatter.pch" /Yc"Stdafx.h" /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 

"$(INTDIR)\Stdafx.obj"	"$(INTDIR)\chatter.pch" : $(SOURCE) "$(INTDIR)"
	$(CPP) @<<
  $(CPP_SWITCHES) $(SOURCE)
<<


!ELSEIF  "$(CFG)" == "Chatter - Win32 Unicode Release"

CPP_SWITCHES=/nologo /MD /W3 /GX /O2 /D "NDEBUG" /D "WIN32" /D "_WINDOWS" /D "_UNICODE" /D "_AFXDLL" /D "_MBCS" /Fp"$(INTDIR)\chatter.pch" /Yc /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 

"$(INTDIR)\Stdafx.obj"	"$(INTDIR)\chatter.pch" : $(SOURCE) "$(INTDIR)"
	$(CPP) @<<
  $(CPP_SWITCHES) $(SOURCE)
<<


!ELSEIF  "$(CFG)" == "Chatter - Win32 Unicode Debug"

CPP_SWITCHES=/nologo /MDd /W3 /Gm /GX /ZI /Od /D "_DEBUG" /D "WIN32" /D "_WINDOWS" /D "_UNICODE" /D "_AFXDLL" /D "_MBCS" /Fp"$(INTDIR)\chatter.pch" /Yc /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 

"$(INTDIR)\Stdafx.obj"	"$(INTDIR)\chatter.pch" : $(SOURCE) "$(INTDIR)"
	$(CPP) @<<
  $(CPP_SWITCHES) $(SOURCE)
<<


!ENDIF 


!ENDIF 

