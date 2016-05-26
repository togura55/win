// chatter.h : main header file for the CHATTER application
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

// �R�}���h�o�C�g
#define CB_SEND_CLIENTSTATUS	0x01	// Send Client Status
#define CB_SEND_DATASTREAM		0x02	// Send Data Stream
#define	CB_SEND_SETMODE			0x03	// Set Mode
//#define	unknown				0x04	//
//#define	unknown				0x05	//
#define	CB_GET_SERVERSTATUS		0x10	// Get Server Status
#define	CB_GET_GETLIST			0x11	// Get List
#define	CB_GET_DEVICEINFO		0x12	// Get DeviceInfo

#define WAIT_COMMAND		0
#define WAIT_RESPONCE		1
#define WAIT_DATA_HEADER	2
#define WAIT_DATA_PAYLOAD	3
#define WAIT_GETLIST_DATA_LISTSIZE		4
#define WAIT_GETLIST_DATA_HEADER		5
#define WAIT_GETLIST_DATA_PAYLOAD		6
#define WAIT_GETDEVICEINFO_DATA_HEADER	7
#define WAIT_GETDEVICEINFO_DATA_PAYLOAD	8

class CChatDoc;

/////////////////////////////////////////////////////////////////////////////
// CChatterApp:
// See chatter.cpp for the implementation of this class
//

class CChatterApp : public CWinApp
{
public:
	CChatterApp();

// Attribute
public:
	CChatDoc*	m_pChatDoc;		// �O���[�o���ϐ��ŗ��p���邽�߂�CChatDoc�I�u�W�F�N�g

	CObList*	m_pProgramInfo;	// �O���[�o���ϐ��ŗ��p���邽�߂̔ԑg���
	int			m_nChannel;		// ���[�U�[���͂̃`�����l���ԍ�
	CString		m_strHandle;	// ���[�U�[���͂̃N���C�A���g��
	CString		m_strServer;	// ���[�U�[���͂̃T�[�o�[IP�A�h���X

//	int			m_iNumProg;		// PVR����擾�����ԑg�� ** ToDo�@�f�o�C�X���ɊǗ�����悤�ɕύX���ׂ�
	CListCtrl*	m_pRecListCtrl;	// �^��\����\���pRecListView�I�u�W�F�N�g

	CObList*	m_pDeviceInfo;	// DeviceInfo�̃��X�g�I�u�W�F�N�g

// Implementation

	//{{AFX_MSG(CChatterApp)
	afx_msg void OnAppAbout();
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG

	afx_msg void OnConfigDlg();

	DECLARE_MESSAGE_MAP()
	
	BOOL InitInstance();
	void SetProgramInfo(void);
	CTime ToCTime(int, int, int, int, int, int);

	int GetCurrentIPAddr(CString *m_strText);
};


/////////////////////////////////////////////////////////////////////////////
