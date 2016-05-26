// srvrdoc.h : interface of the CServerDoc class
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

#include "lstnsock.h"
#include "clntsock.h"

#define WAIT_COMMAND		0
#define WAIT_RESPONCE		1
#define WAIT_DATA_HEADER	2
#define WAIT_DATA_PAYLOAD	3


class CMsg;

class CServerDoc : public CDocument
{
protected: // create from serialization only
	CServerDoc();
	DECLARE_DYNCREATE(CServerDoc)

// Attributes
public:
	CListeningSocket* m_pSocket;
	CStringList m_msgList;
	CPtrList m_connectionList;
	int		m_iReadStat;
	char	chHdr[18];	// �w�b�_�[�� �Œ蒷

	CString		m_strTitle;		// �ԑg��
	CString		m_strDesc;		// �ԑg���e
	CString		m_strCategory;	// �ԑg�J�e�S��
	CString		m_strStation;	// �����ǖ�
	CTime		m_tiStart;		// �J�n����
	CTime		m_tiEnd;		// �I������
	int			m_iStat;		// �^��v�����
	int			m_iId;			// ���N�G�X�gID
// Operations
public:
	void UpdateClients();
	void ProcessPendingAccept();
	void ProcessPendingRead(CClientSocket* pSocket);
	CMsg* AssembleMsg(CClientSocket* pSocket);
	CMsg* ReadMsg(CClientSocket* pSocket);
	void SendMsg(CClientSocket* pSocket, CMsg* pMsg);
	void CloseSocket(CClientSocket* pSocket);
	void Message(LPCTSTR lpszMessage);

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CServerDoc)
	public:
	virtual BOOL OnNewDocument();
	virtual void DeleteContents();
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CServerDoc();
	virtual void Serialize(CArchive& ar);   // overridden for document i/o
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	//{{AFX_MSG(CServerDoc)
	//}}AFX_MSG
	afx_msg void OnUpdateMessages(CCmdUI* pCmdUI);
	afx_msg void OnUpdateConnections(CCmdUI* pCmdUI);
	DECLARE_MESSAGE_MAP()
public:
	CString DecodePayload(CMsg* pMsg, char *chHdr, int iIndex);
	void	GetListProc(CClientSocket* pSocket);
	void	GetDeviceInfoProc(CClientSocket* pSocket);


};

/////////////////////////////////////////////////////////////////////////////
