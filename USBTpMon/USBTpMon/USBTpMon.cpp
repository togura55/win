// USBTpMon.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "USBTpMon.h"
#include "USBTpMonDlg.h"
#include "DeviceList.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CUSBTpMonApp

BEGIN_MESSAGE_MAP(CUSBTpMonApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// CUSBTpMonApp construction

CUSBTpMonApp::CUSBTpMonApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
	pGetVers = NULL;
	pFindUSB = NULL;
	pControlIO = NULL;
	pSetHeater = NULL;
	pGetTempHumidTrue = NULL;

	bEnablePol = false;
	iPolDuration = 1000;
	nTimerID = 0;
}


// The one and only CUSBTpMonApp object

CUSBTpMonApp theApp;


// CUSBTpMonApp initialization

BOOL CUSBTpMonApp::InitInstance()
{
	// InitCommonControlsEx() is required on Windows XP if an application
	// manifest specifies use of ComCtl32.dll version 6 or later to enable
	// visual styles.  Otherwise, any window creation will fail.
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// Set this to include all the common control classes you want to use
	// in your application.
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();

	AfxEnableControlContainer();

	// Standard initialization
	// If you are not using these features and wish to reduce the size
	// of your final executable, you should remove from the following
	// the specific initialization routines you do not need
	// Change the registry key under which our settings are stored
	// TODO: You should modify this string to be something appropriate
	// such as the name of your company or organization
	SetRegistryKey(_T("Local AppWizard-Generated Applications"));


	// Load device Interface DLL
	CString str;

	HMODULE hModule = NULL;
	str.LoadStringA(IDS_GEN_DLLNAME);
	hModule = LoadLibraryA(str);
	if (hModule != NULL)
	{		
		theApp.pGetVers = (GETVERS)GetProcAddress( hModule, "_GetVers@4" );
		theApp.pFindUSB = (FINDUSB)GetProcAddress( hModule, "_FindUSB@4" );
		theApp.pControlIO = (CONTROLIO)GetProcAddress( hModule, "_ControlIO@12" );
		theApp.pSetHeater = (SETHEATER)GetProcAddress( hModule, "_SetHeater@8" );
		theApp.pGetTempHumidTrue = (GETTEMPHUMIDTRUE)GetProcAddress( hModule, "_GetTempHumidTrue@12" );

		if (!theApp.pGetVers || !theApp.pFindUSB || !theApp.pControlIO || !theApp.pSetHeater || !theApp.pGetTempHumidTrue )
		{
			CString msg, strForm;
			strForm.LoadStringA(IDS_GEN_DLLFUNCERR);
			msg.Format(strForm, str);
			AfxMessageBox (msg);
		}
		else
		{
		}

	}
	else
	{
		CString msg, strForm;
		strForm.LoadStringA(IDS_GEN_DLLLOADERR);
		msg.Format(strForm, str);
		AfxMessageBox (msg);
	}
	


	CUSBTpMonDlg dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: Place code here to handle when the dialog is
		//  dismissed with OK
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO: Place code here to handle when the dialog is
		//  dismissed with Cancel
	}

	// Since the dialog has been closed, return FALSE so that we exit the
	//  application, rather than start the application's message pump.
	return FALSE;
}
