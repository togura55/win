// chatdoc.h : interface of the CChatDoc class
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

#include "chatsock.h"
#include "msg.h"
#include "Deviceinfo.h"

class CChatDoc : public CDocument
{
//protected: // create from serialization only
public: // create from serialization only
	CChatDoc();
	DECLARE_DYNCREATE(CChatDoc)

// Attributes
public:
	BOOL m_bAutoChat;
	CChatSocket* m_pSocket;
	CSocketFile* m_pFile;
	CArchive* m_pArchiveIn;
	CArchive* m_pArchiveOut;

	int		m_iReadStat;
	char	m_chHdr[8];	// ÉwÉbÉ_Å[ïî å≈íËí∑

// Operations
public:
	BOOL ConnectSocket(LPCTSTR lpszHandle, LPCTSTR lpszAddress, UINT nPort);
	void ProcessPendingRead();
	void SendMsg(CString& strText);
	void ReceiveMsg();
	void DisplayMsg(LPCTSTR lpszText);
	void GetListHeaderDecodeProc(CDeviceInfo *pDI, CMsg *pMsg);
	void GetListPayloadDecodeProc(CDeviceInfo *pDI, CMsg *pMsg);
	CString DecodePayload(CString strMsg, char *m_chHdr, int iIndex);

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CChatDoc)
	public:
	virtual BOOL OnNewDocument();
	virtual void DeleteContents();
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CChatDoc();
	virtual void Serialize(CArchive& ar);   // overridden for document i/o
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

	//afx_msg void OnConfigDlg();

protected:

// Generated message map functions
protected:
	//{{AFX_MSG(CChatDoc)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////
