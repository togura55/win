// USBTpMon.h : main header file for the PROJECT_NAME application
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CUSBTpMonApp:
// See USBTpMon.cpp for the implementation of this class
//

typedef BSTR (__stdcall **GETVERS)(BSTR); 
typedef BSTR (__stdcall *FINDUSB)(int *); 
typedef int (__stdcall *CONTROLIO)(BSTR, int, int); 
typedef int (__stdcall *SETHEATER)(BSTR, int); 
typedef int (__stdcall *GETTEMPHUMIDTRUE)(BSTR, double *, double *); 

class CUSBTpMonApp : public CWinApp
{
public:
	CUSBTpMonApp();

	CObList	deviceList;

	// DLL I/F Functions
	GETVERS		pGetVers;
	FINDUSB		pFindUSB;
	CONTROLIO	pControlIO;
	SETHEATER	pSetHeater;
	GETTEMPHUMIDTRUE pGetTempHumidTrue;

	bool		bEnablePol;
	int			iPolDuration;
	UINT_PTR	nTimerID;

// Overrides
	public:
	virtual BOOL InitInstance();

// Implementation

	DECLARE_MESSAGE_MAP()
};

extern CUSBTpMonApp theApp;