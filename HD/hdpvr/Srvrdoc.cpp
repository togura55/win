// srvrdoc.cpp : implementation of the CServerDoc class
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

#include "stdafx.h"
#include "chatsrvr.h"
#include "srvrdoc.h"
#include "srvrvw.h"
#include "msg.h"
#include "dialogs.h"
#include "Reclistview.h"

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CServerDoc

IMPLEMENT_DYNCREATE(CServerDoc, CDocument)

BEGIN_MESSAGE_MAP(CServerDoc, CDocument)
	//{{AFX_MSG_MAP(CServerDoc)
	//}}AFX_MSG_MAP
	ON_UPDATE_COMMAND_UI(ID_MESSAGES, OnUpdateMessages)
	ON_UPDATE_COMMAND_UI(ID_CONNECTIONS, OnUpdateConnections)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CServerDoc construction/destruction

CServerDoc::CServerDoc()
{
	m_pSocket = NULL;
	m_iReadStat = WAIT_COMMAND;
}

CServerDoc::~CServerDoc()
{
	delete m_pSocket;
}

BOOL CServerDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	CDiscussionDlg Dialog;

	if (Dialog.DoModal() == IDOK)
	{
		m_pSocket = new CListeningSocket(this);
		if (m_pSocket->Create(Dialog.m_nPort+700))
		{
			if (m_pSocket->Listen())
				return TRUE;
		}
	}
	return FALSE;
}

void CServerDoc::DeleteContents()
{
	delete m_pSocket;
	m_pSocket = NULL;

	CString temp;
	if (temp.LoadString(IDS_SERVERSHUTDOWN))
		m_msgList.AddTail(temp);

	while(!m_connectionList.IsEmpty())
	{
		CClientSocket* pSocket = (CClientSocket*)m_connectionList.RemoveHead();
		CMsg* pMsg = AssembleMsg(pSocket);
		pMsg->m_bClose = TRUE;

		SendMsg(pSocket, pMsg);

		if (!pSocket->IsAborted())
		{
			pSocket->ShutDown();

			BYTE Buffer[50];

			while (pSocket->Receive(Buffer,50) > 0);

			delete pSocket;
		}
	}

	m_msgList.RemoveAll();

	if (!m_viewList.IsEmpty())
		((CEditView*)m_viewList.GetHead())->SetWindowText(_T(""));

	CDocument::DeleteContents();
}

/////////////////////////////////////////////////////////////////////////////
// CServerDoc serialization

void CServerDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		// CEditView contains an edit control which handles all serialization
		((CEditView*)m_viewList.GetHead())->SerializeRaw(ar);
	}
}

/////////////////////////////////////////////////////////////////////////////
// CServerDoc diagnostics

#ifdef _DEBUG
void CServerDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CServerDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG

/////////////////////////////////////////////////////////////////////////////
// CServerDoc Operations

void CServerDoc::UpdateClients()
{
	for(POSITION pos = m_connectionList.GetHeadPosition(); pos != NULL;)
	{
		CClientSocket* pSocket = (CClientSocket*)m_connectionList.GetNext(pos);
		CMsg* pMsg = AssembleMsg(pSocket);

		if (pMsg != NULL)
			SendMsg(pSocket, pMsg);
	}
}

// OnAcceptコールバック内からコールされる
void CServerDoc::ProcessPendingAccept()
{
	CClientSocket* pSocket = new CClientSocket(this);

	if (m_pSocket->Accept(*pSocket))
	{
		pSocket->Init();
		m_connectionList.AddTail(pSocket);
	}
	else
		delete pSocket;
}

void CServerDoc::ProcessPendingRead(CClientSocket* pSocket)
{
	do
	{
		CMsg* pMsg = ReadMsg(pSocket);

		if (pMsg->m_bClose)
		{
			CloseSocket(pSocket);
			break;
		}
	}
	while (!pSocket->m_pArchiveIn->IsBufferEmpty());

	UpdateClients();
}

CMsg* CServerDoc::AssembleMsg(CClientSocket* pSocket)
{
	static CMsg msg;

	msg.Init();

	if (pSocket->m_nMsgCount >= m_msgList.GetCount())
		return NULL;

	for (POSITION pos1 = m_msgList.FindIndex(pSocket->m_nMsgCount); pos1 != NULL;)
	{
		CString temp = m_msgList.GetNext(pos1);
		msg.m_msgList.AddTail(temp);
	}
	pSocket->m_nMsgCount = m_msgList.GetCount();
	return &msg;
}

CMsg* CServerDoc::ReadMsg(CClientSocket* pSocket)
{
	static CMsg msg;
	static int dataSize;
	CString	str;
	int iIndex = 0;

	TRY
	{
		// メッセージの受信
		pSocket->ReceiveMsg(&msg);

		switch (m_iReadStat)
		{
		case WAIT_COMMAND:
			{
				char cb=0;
				cb = (char)atoi(msg.m_strText);

				switch(cb)
				{
				case CB_SEND_CLIENTSTATUS:	// Send Client Status
					break;
				case CB_SEND_DATASTREAM:	// Send Data Stream
					m_iReadStat = WAIT_DATA_HEADER;
					break;
				case CB_SEND_SETMODE:		// Set Mode
					break;
				
				case CB_GET_SERVERSTATUS:	// Get Server Status
					break;
				case CB_GET_GETLIST:		// Get List
					GetListProc(pSocket);
					//					
					m_iReadStat = WAIT_COMMAND;	// 状態の更新
					break;
				case CB_GET_DEVICEINFO:
					GetDeviceInfoProc(pSocket);
					m_iReadStat = WAIT_COMMAND;	// 状態の更新
					break;
				default:
					break;
				}
			}
			break;
		case WAIT_RESPONCE:
			break;
		case WAIT_DATA_HEADER:
			{
				int i;

				// ヘッダー部を読み込んで保存
				lstrcpy(chHdr, msg.m_strText);

				dataSize = 0;
				//for(i=0;i<18;i++)
				for(i=0;i<8;i++)
					dataSize += chHdr[i];
			}
			m_iReadStat = WAIT_DATA_PAYLOAD;	// 状態の更新
			break;

		case WAIT_DATA_PAYLOAD:
			{
				CProgInfo	*pPI;
				int iSubItem = 0, index= 0;
				CServerApp* pApp = (CServerApp*)AfxGetApp();

				// ペイロード部をデコードし、リストに追加			
				pPI = new CProgInfo();
				pApp->m_pProgramInfo->AddTail(pPI);
				pPI->m_strTitle = DecodePayload(&msg, chHdr, 0);
				pPI->m_strDesc = DecodePayload(&msg, chHdr, 1);
				pPI->m_strCategory = DecodePayload(&msg, chHdr, 2);
				pPI->m_strStation = DecodePayload(&msg, chHdr, 3);
				pPI->m_tiStart = CTime(atoi(DecodePayload(&msg, chHdr, 4)));
				pPI->m_tiEnd = CTime(atoi(DecodePayload(&msg, chHdr, 5)));
				pApp->m_iNumRecProg++;	// 総プログラム数の更新

				// リストビューに追加して表示
				CRecListView* pRecLV = new CRecListView();
				index = pApp->m_pProgramInfo->GetCount() -1;	
				pRecLV->ProgramContentsForDisplay(pPI, index);	// ListViewに表示するItemをセットする

				m_iReadStat = WAIT_COMMAND;
			}
			break;
		default:
			break;
		}
		Message(msg.m_strText);

		m_msgList.AddTail(msg.m_strText);
	}
	CATCH(CFileException, e)
	{
		CString strTemp;
		if (strTemp.LoadString(IDS_READERROR))
			Message(strTemp);

		msg.m_bClose = TRUE;
		pSocket->Abort();
	}
	END_CATCH

	return &msg;
}

////////////////////////////////////////////////////////////////////////////
// GetDeviceInfoProc
// called when "Get DeviceInfo" request is called
//
void CServerDoc::GetDeviceInfoProc(CClientSocket* pSocket)
{
	int i = 0;
	CString strText;
	CServerApp* pApp = (CServerApp*)AfxGetApp();
	CMsg* pMsg = new CMsg;

	// Headerの送信
	char ch[3];
	// Pack the program information data for the transmitting
	ch[i++] =  printf("%d",pApp->m_iDeviceID);			// DeviceIDの桁数
	ch[i++] = (char)pApp->m_strDeviceDesc.GetLength();	// DeviceDesc文字列の長さ
	ch[i++] = (char)(_T("\r\n"));						// 終端文字の付加
	pMsg->m_strText = (CString)ch;
	SendMsg(pSocket, pMsg);			// 送信

	// Payload部の送信
	CString strP;
	strText.Empty();

	strText.Format(_T("%d"),pApp->m_iDeviceID);strP.Append(strText);
	strP.Append(pApp->m_strDeviceDesc);

	pMsg->m_strText = (CString)strP;
	SendMsg(pSocket, pMsg);			// 送信
}

////////////////////////////////////////////////////////////////////////////
// GetListProc
// called when "Get List" request is called
//
void CServerDoc::GetListProc(CClientSocket* pSocket)
{
	int i = 0;
	CString strTmp, strP;
	CProgInfo*	pPI = NULL;
	CServerApp* pApp = (CServerApp*)AfxGetApp();
	CObList* pProgList = pApp->m_pProgramInfo;
	CMsg* pMsg = new CMsg;

	// Listの総数を送信
	// *** ToDo パケットのID番号を振ること
	char ch1[2];	// < 0-255
	ch1[i++] = (char)pApp->m_iNumRecProg;	// 
	ch1[i++] = (char)(_T("\r\n"));		// 終端文字の付加
	pMsg->m_strText = (CString)ch1;
	SendMsg(pSocket, pMsg);			// 送信

	// Headerの送信
	// 全リスト分を積んで送信
	// *** ToDo パケットのID番号をつけること
	pPI = (CProgInfo *)pApp->m_pProgramInfo->GetHead();
	pMsg->m_strText.Empty();

//	char ch[9];
	char *ch;
	ch = (char *)malloc (sizeof (char) * 8 * pApp->m_iNumRecProg + 1);
	POSITION pos;
		i = 0;
	for(pos = pApp->m_pProgramInfo->GetHeadPosition(); pos != NULL;)
	{
		pPI = (CProgInfo *)pApp->m_pProgramInfo->GetAt(pos);
		// Header part
		ch[i++] = (char)pPI->m_strTitle.GetLength();
		ch[i++] = (char)pPI->m_strDesc.GetLength();
		ch[i++] = (char)pPI->m_strCategory.GetLength();
		ch[i++] = (char)pPI->m_strStation.GetLength();

		ch[i++] = (char)printf("%d",pPI->m_tiStart.GetTime()); // 開始時間を表すtime64_t値の桁数
		ch[i++] = (char)printf("%d",pPI->m_tiEnd.GetTime()); // 終了時間を表すtime64_t値の桁数
		ch[i++] = (char)printf("%d",pPI->m_iStat);			// 録画要求状態の桁数
		ch[i++] = (char)printf("%d",pPI->m_iId);			// リクエストIDの桁数

		// Payload part
		strTmp.Empty();
		strP.Append(pPI->m_strTitle);
		strP.Append(pPI->m_strDesc);
		strP.Append(pPI->m_strCategory);
		strP.Append(pPI->m_strStation);
		strTmp.Format(_T("%d"),pPI->m_tiStart.GetTime());strP.Append(strTmp);
		strTmp.Format(_T("%d"),pPI->m_tiEnd.GetTime());strP.Append(strTmp);
		strTmp.Format(_T("%d"),pPI->m_iStat);strP.Append(strTmp);
		strTmp.Format(_T("%d"),pPI->m_iId);strP.Append(strTmp);

		pApp->m_pProgramInfo->GetNext(pos);	// 次のリストへ
	}
	ch[i++] =	(char)(_T("\r\n"));
	pMsg->m_strText = (CString)ch;

	SendMsg(pSocket, pMsg);			// Header送信

	pMsg->m_strText = (CString)strP;
	SendMsg(pSocket, pMsg);			// Payload送信

}

////////////////////////////////////////////////////////////////////////////
// DecodePayload
// 
//
CString CServerDoc::DecodePayload(CMsg* pMsg, char *chHdr, int iIndex)
{
	int i, iStart=0, iEnd=0;

	if (iIndex == 0)
	{
		iStart=0;
		iEnd = chHdr[0];
	}
	else
	{
		for (i=0; i<iIndex; i++)
		{
			iStart += chHdr[i];
		}
		iEnd = chHdr[iIndex];
	}

	return  pMsg->m_strText.Mid(iStart, iEnd);
}


void CServerDoc::SendMsg(CClientSocket* pSocket, CMsg* pMsg)
{
	TRY
	{
		pSocket->SendMsg(pMsg);
	}
	CATCH(CFileException, e)
	{
		pSocket->Abort();

		CString strTemp;
		if (strTemp.LoadString(IDS_SENDERROR))
			Message(strTemp);
	}
	END_CATCH
}

void CServerDoc::CloseSocket(CClientSocket* pSocket)
{
	pSocket->Close();

	POSITION pos,temp;
	for(pos = m_connectionList.GetHeadPosition(); pos != NULL;)
	{
		temp = pos;
		CClientSocket* pSock = (CClientSocket*)m_connectionList.GetNext(pos);
		if (pSock == pSocket)
		{
			m_connectionList.RemoveAt(temp);
			break;
		}
	}

	delete pSocket;
}

void CServerDoc::Message(LPCTSTR lpszMessage)
{
	((CServerView*)m_viewList.GetHead())->Message(lpszMessage);
}

/////////////////////////////////////////////////////////////////////////////
// CServerDoc Handlers

void CServerDoc::OnUpdateMessages(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);

	CString strFmt;
	if (strFmt.LoadString(IDS_MESSAGESFMT))
	{
		CString strTemp;
		wsprintf(strTemp.GetBuffer(50),strFmt,m_msgList.GetCount());
		strTemp.ReleaseBuffer();
		pCmdUI->SetText(strTemp);
	}
}

void CServerDoc::OnUpdateConnections(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);

	CString strFmt;
	if (strFmt.LoadString(IDS_CONNECTIONSFMT))
	{
		CString strTemp;
		wsprintf(strTemp.GetBuffer(50),strFmt,m_connectionList.GetCount());
		strTemp.ReleaseBuffer();
		pCmdUI->SetText(strTemp);
	}
}
