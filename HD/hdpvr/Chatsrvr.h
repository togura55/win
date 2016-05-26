// chatsrvr.h : main header file for the CHATSRVR application
//
// This is a part of the Microsoft Foundation Classes C++ library.
// Copyright (C) 1992-1998 Microsoft Corporation
// All rights reserved.
//
// This source code is only intended as a supplement to the
// Microsoft Foundation Classes Reference and related
// electronic documentation provided with the library.
// See these sources for detailed information regarding the
// Microsoft Foundation Classes product.

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"       // main symbols
#include "afxcoll.h"
#include "Proginfo.h"
#include "RecListView.h"

// コマンドバイト
#define CB_SEND_CLIENTSTATUS	0x01	// Send Client Status
#define CB_SEND_DATASTREAM		0x02	// Send Data Stream
#define	CB_SEND_SETMODE			0x03	// Set Mode
//#define	unknown				0x04	//
//#define	unknown				0x05	//
#define	CB_GET_SERVERSTATUS		0x10	// Get Server Status
#define	CB_GET_GETLIST			0x11	// Get List
#define	CB_GET_DEVICEINFO		0x12	// Get DeviceInfo

class CServerDoc;
class CRecListView;

/////////////////////////////////////////////////////////////////////////////
// CServerApp:
// See chatsrvr.cpp for the implementation of this class
//

class CServerApp : public CWinApp
{
public:
	CServerApp();

// Attribute
public:
	CObList*	m_pProgramInfo;	// グローバル変数で利用するための番組情報
	int			m_iNumRecProg;
	CListCtrl*	m_pRecListCtrl;	// 録画予約情報表示用RecListViewオブジェクト

	int			m_iDeviceID;		// デバイスID
	CString		m_strDeviceDesc;	// デバイスの説明

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CServerApp)
	public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CServerApp)
	afx_msg void OnAppAbout();
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
	bool ReadRegSettings(void);
	bool WriteRegSettings(void);
};


/////////////////////////////////////////////////////////////////////////////
