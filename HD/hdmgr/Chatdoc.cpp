// chatdoc.cpp : implementation of the CChatDoc class
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
#include "chatter.h"

#include "chatsock.h"
#include "chatdoc.h"
#include "chatvw.h"
#include "setupdlg.h"
#include "msg.h"
#include "ProgInfo.h"
#include "RecListView.h"
#include "Deviceinfo.h"


#ifdef _WIN32
#ifndef _UNICODE
//#include <strstrea.h>
#include <strstream>
using namespace std;

#endif
#endif

#ifdef _DEBUG
#undef THIS_FILE
static char BASED_CODE THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CChatDoc

IMPLEMENT_DYNCREATE(CChatDoc, CDocument)

BEGIN_MESSAGE_MAP(CChatDoc, CDocument)
	//{{AFX_MSG_MAP(CChatDoc)
	//}}AFX_MSG_MAP
	//ON_MESSAGE(WM_USER_CONFIGDLG, OnConfigDlg)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CChatDoc construction/destruction

CChatDoc::CChatDoc()
{
	m_bAutoChat = FALSE;
	m_pSocket = NULL;
	m_pFile = NULL;
	m_pArchiveIn = NULL;
	m_pArchiveOut = NULL;

	// 本アプリケーションのポインタを取得し、派生して
	// 作られたCChatterAppのメンバ変数にChatDocオブジェクト
	// のポインタを格納する。外部クラスから使用可能にする
	CChatterApp* pApp = (CChatterApp*)AfxGetApp();
	pApp->m_pChatDoc = this;

	m_iReadStat = 0;

}

CChatDoc::~CChatDoc()
{
}

BOOL CChatDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

#ifdef _WIN32
#ifndef _UNICODE
	if (AfxGetApp()->m_lpCmdLine[0] != '\0')
	{
		TCHAR strHandle[128];
		TCHAR strServer[128];
		int nChannel;

		istrstream(AfxGetApp()->m_lpCmdLine) >> strHandle >> strServer >> nChannel;
		return ConnectSocket(strHandle, strServer, nChannel);
	}
	//else
#endif
#endif
	//{
	//	CSetupDlg Dialog;

	//	Dialog.m_strHandle=m_strHandle;
	//	Dialog.m_strServer=_T("");
	//	Dialog.m_nChannel=0;

	//	while(TRUE)
	//	{
	//		if (IDOK != Dialog.DoModal())
	//			return FALSE;

	//		if (ConnectSocket(Dialog.m_strHandle, Dialog.m_strServer, Dialog.m_nChannel))
	//			return TRUE;

	//		if (AfxMessageBox(IDS_CHANGEADDRESS,MB_YESNO) == IDNO)
	//			return FALSE;
	//	}
	//}

	return TRUE;
}

void CChatDoc::DeleteContents()
{
	CChatterApp* pApp = (CChatterApp*)AfxGetApp();
	m_bAutoChat = FALSE;

	if ((m_pSocket != NULL) && (m_pFile != NULL) && (m_pArchiveOut != NULL))
	{
		CMsg msg;
		CString strTemp;

		if (strTemp.LoadString(IDS_DISCONNECT))
		{
			msg.m_bClose = TRUE;
			msg.m_strText = pApp->m_strHandle + strTemp;
			msg.Serialize(*m_pArchiveOut);
			m_pArchiveOut->Flush();
		}
	}

	delete m_pArchiveOut;
	m_pArchiveOut = NULL;
	delete m_pArchiveIn;
	m_pArchiveIn = NULL;
	delete m_pFile;
	m_pFile = NULL;

	if (m_pSocket != NULL)
	{
		BYTE Buffer[50];
		m_pSocket->ShutDown();

		while(m_pSocket->Receive(Buffer,50) > 0);
	}

	delete m_pSocket;
	m_pSocket = NULL;

	for(POSITION pos=GetFirstViewPosition();pos!=NULL;)
	{
		CView* pView = GetNextView(pos);

		if (pView->IsKindOf(RUNTIME_CLASS(CChatView)))
		{
			CChatView* pChatView = (CChatView*)pView;
			pChatView->GetEditCtrl().SetWindowText(_T(""));
		}
	}
	CDocument::DeleteContents();
}

/////////////////////////////////////////////////////////////////////////////
// CChatDoc Operations
//
BOOL CChatDoc::ConnectSocket(LPCTSTR lpszHandle, LPCTSTR lpszAddress, UINT nPort)
{
	m_pSocket = new CChatSocket(this);

	if (!m_pSocket->Create())
	{
		delete m_pSocket;
		m_pSocket = NULL;
		AfxMessageBox(IDS_CREATEFAILED);
		return FALSE;
	}

	while (!m_pSocket->Connect(lpszAddress, nPort + 700))
	{
		if (AfxMessageBox(IDS_RETRYCONNECT,MB_YESNO) == IDNO)
		{
			delete m_pSocket;
			m_pSocket = NULL;
			return FALSE;
		}
	}

	m_pFile = new CSocketFile(m_pSocket);
	m_pArchiveIn = new CArchive(m_pFile,CArchive::load);
	m_pArchiveOut = new CArchive(m_pFile,CArchive::store);

	//CString strTemp;
	//if (strTemp.LoadString(IDS_CONNECT))
	//	SendMsg(strTemp);

	return TRUE;
}

void CChatDoc::ProcessPendingRead()
{
	do
	{
		ReceiveMsg();
		if (m_pSocket == NULL)
			return;
	}
	while(!m_pArchiveIn->IsBufferEmpty());
}

void CChatDoc::SendMsg(CString& strText)
{
	if (m_pArchiveOut != NULL)
	{
		CMsg msg;

		//msg.m_strText = m_strHandle + _T(": ") + strText;
		msg.m_strText = strText;

		TRY
		{
			msg.Serialize(*m_pArchiveOut);
			m_pArchiveOut->Flush();
		}
		CATCH(CFileException, e)
		{
			m_bAutoChat = FALSE;
			m_pArchiveOut->Abort();
			delete m_pArchiveOut;
			m_pArchiveOut = NULL;

			CString strTemp;
			if (strTemp.LoadString(IDS_SERVERRESET))
				DisplayMsg(strTemp);
		}
		END_CATCH
	}
}

/////////////////////////////////////////////////////////////////////////////
// CChatDoc::ReceiveMsg
//	Socket経由でのメッセージ受信処理
//
void CChatDoc::ReceiveMsg()
{
	CMsg msg;
	CChatterApp* pApp = (CChatterApp*)AfxGetApp();

	TRY
	{
		// メッセージの受信
		msg.Serialize(*m_pArchiveIn);

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

					//					GetListDecodeProc();
					m_iReadStat = WAIT_GETLIST_DATA_HEADER;	// 状態の更新
					break;
				
				default:
					break;
				}
			}
			break;
		case WAIT_RESPONCE:
			break;

		case WAIT_GETLIST_DATA_LISTSIZE:
			{
				int i;
				char	ch[2];
				CDeviceInfo *pDI = NULL;
				CProgInfo	*pPI = NULL;
	
				// ToDo: 選択されているデバイスに相当するリストの位置に移動する
				pDI = (CDeviceInfo *)pApp->m_pDeviceInfo->GetHead();

				// 1 byte dataをコピー、Program List総数情報のDecode
				lstrcpy(ch, msg.m_strText);	
				pDI->m_iNumProg = (int)ch[0];

				// このデバイスのためのProgramリストオブジェクトを作成する
				pDI->m_pProgram = new CObList();
				for (i=0; i<pDI->m_iNumProg; i++)
				{
					pPI = new CProgInfo();
					pDI->m_pProgram->AddTail(pPI);
				}

				m_iReadStat = WAIT_GETLIST_DATA_HEADER;	// 状態の更新
			}
			break;
		case WAIT_GETLIST_DATA_HEADER:
			{
				// ToDo: 選択されているデバイスに相当するリストの位置に移動しなければならない
				CDeviceInfo *pDI;	
				pDI = (CDeviceInfo *)pApp->m_pDeviceInfo->GetHead();

				GetListHeaderDecodeProc(pDI, &msg);
				m_iReadStat = WAIT_GETLIST_DATA_PAYLOAD;	// 状態の更新
			}
			break;
		case WAIT_GETLIST_DATA_PAYLOAD:
			{
				// ToDo: 選択されているデバイスに相当するリストの位置に移動する
				CDeviceInfo *pDI;	
				pDI = (CDeviceInfo *)pApp->m_pDeviceInfo->GetHead();

				GetListPayloadDecodeProc(pDI, &msg);
				m_iReadStat = WAIT_COMMAND;					// 状態の更新
			}
			break;

		case WAIT_DATA_HEADER:
			{
				int i, dataSize = 0;

				// ヘッダー部を読み込んで保存
				lstrcpy(m_chHdr, msg.m_strText);

				//for(i=0;i<18;i++)
				for(i=0;i<8;i++)
					dataSize += m_chHdr[i];
			}
			m_iReadStat = WAIT_DATA_PAYLOAD;	// 状態の更新
			break;

		case WAIT_DATA_PAYLOAD:
			{
				CProgInfo	*pPI;
				int iSubItem = 0;

				// ペイロード部をデコードし、リストに追加			
				pPI = new CProgInfo();
				pApp->m_pProgramInfo->AddTail(pPI);
				pPI->m_strTitle = DecodePayload(msg.m_strText, m_chHdr, 0);
				pPI->m_strDesc = DecodePayload(msg.m_strText, m_chHdr, 1);
				pPI->m_strCategory = DecodePayload(msg.m_strText, m_chHdr, 2);
				pPI->m_strStation = DecodePayload(msg.m_strText, m_chHdr, 3);
				pPI->m_tiStart = CTime(atoi(DecodePayload(msg.m_strText, m_chHdr, 4)));
				pPI->m_tiEnd = CTime(atoi(DecodePayload(msg.m_strText, m_chHdr, 5)));

				m_iReadStat = WAIT_COMMAND;
			}
			break;
		case WAIT_GETDEVICEINFO_DATA_HEADER:
			// ヘッダー部を読み込んで保存
			lstrcpy(m_chHdr, msg.m_strText);
			m_iReadStat = WAIT_GETDEVICEINFO_DATA_PAYLOAD;	// 状態の更新
			break;
		case WAIT_GETDEVICEINFO_DATA_PAYLOAD:
			// ペイロード部をデコードし、リストに追加			
			CDeviceInfo	*pDI;
			pDI = new CDeviceInfo();
			pApp->m_pDeviceInfo->AddTail(pDI);
			pDI->m_iDeviceID = atoi(DecodePayload(msg.m_strText, m_chHdr, 0));
			pDI->m_strDeviceDesc = DecodePayload(msg.m_strText, m_chHdr, 1);
			m_iReadStat = WAIT_COMMAND;	// 状態の更新
			break;

		default:
			break;
		}
				// 表示
		while(!msg.m_msgList.IsEmpty())
		{
			CString temp = msg.m_msgList.RemoveHead();
			DisplayMsg(temp);
		}

	}
	CATCH(CFileException, e)
	{
		m_bAutoChat = FALSE;
		msg.m_bClose = TRUE;
		m_pArchiveOut->Abort();

		CString strTemp;
		if (strTemp.LoadString(IDS_SERVERRESET))
			DisplayMsg(strTemp);
		if (strTemp.LoadString(IDS_CONNECTIONCLOSED))
			DisplayMsg(strTemp);
	}
	END_CATCH

	if (msg.m_bClose)
	{
		delete m_pArchiveIn;
		m_pArchiveIn = NULL;
		delete m_pArchiveOut;
		m_pArchiveOut = NULL;
		delete m_pFile;
		m_pFile = NULL;
		delete m_pSocket;
		m_pSocket = NULL;
	}
}


/////////////////////////////////////////////////////////////////////////////
// 
//	Get Listのヘッダーから、各プログラムに対応するヘッダーに分割し、pPIに保存
//
void CChatDoc::GetListHeaderDecodeProc(CDeviceInfo *pDI, CMsg *pMsg)
{
	int i = 0, iStart = 0, iEnd = 0;
	CProgInfo	*pPI;
	CString	strTmp;

	POSITION pos = pDI->m_pProgram->GetHeadPosition();
	for (i=0; i<pDI->m_iNumProg; i++)
	{
		iEnd = iStart + 8;
		pPI = (CProgInfo *)pDI->m_pProgram->GetAt(pos);

		strTmp = pMsg->m_strText.Mid(iStart, iEnd);
		lstrcpy(pPI->m_chHdr, strTmp);
		
		pDI->m_pProgram->GetNext(pos);
		iStart += 8;
	}
}

/////////////////////////////////////////////////////////////////////////////
// 
//	
//
void CChatDoc::GetListPayloadDecodeProc(CDeviceInfo *pDI, CMsg *pMsg)
{
	int i, j, index, iCnt=0, iStart=0;
	CProgInfo	*pPI;
	char ch, *pt;
	CString strMsg;
	CChatterApp* pApp = (CChatterApp*)AfxGetApp();

	// Payloadのデコード
	POSITION pos = pDI->m_pProgram->GetHeadPosition();
	for (i=0; i<pDI->m_iNumProg; i++)
	{	
		// ペイロード部をデコードし、リストに追加			
		pPI = (CProgInfo *)pDI->m_pProgram->GetAt(pos);
		pt = pPI->m_chHdr;
		for (j=0; j<8; j++)
		{
			ch = (char)*pt;
			iCnt += ch;
			pt++;
		}
		strMsg = pMsg->m_strText.Mid(iStart, iCnt);

		pPI->m_strTitle = DecodePayload(strMsg, pPI->m_chHdr, 0);
		pPI->m_strDesc = DecodePayload(strMsg, pPI->m_chHdr, 1);
		pPI->m_strCategory = DecodePayload(strMsg, pPI->m_chHdr, 2);
		pPI->m_strStation = DecodePayload(strMsg, pPI->m_chHdr, 3);
		pPI->m_tiStart = CTime(atoi(DecodePayload(strMsg, pPI->m_chHdr, 4)));

		pPI->m_tiEnd = CTime(atoi(DecodePayload(strMsg, pPI->m_chHdr, 5)));
		pPI->m_iStat = (int)atoi(DecodePayload(strMsg, pPI->m_chHdr, 6));
		pPI->m_iId = (int)atoi(DecodePayload(strMsg, pPI->m_chHdr, 7));

		iStart = iCnt;

		pDI->m_pProgram->GetNext(pos);
	}

	// リストビューに表示
	CRecListView* pRecLV = new CRecListView();
	pApp->m_pRecListCtrl->DeleteAllItems();		// 全表示アイテムの削除
	//index = pApp->m_pProgramInfo->GetCount() -1;
	index = 0;
	pos = pDI->m_pProgram->GetHeadPosition();
	for (i=0; i<pDI->m_iNumProg; i++)
	{	
		pPI = (CProgInfo *)pDI->m_pProgram->GetAt(pos);
		pRecLV->ProgramContentsForDisplay(pPI, index);	// ListViewに表示するItemをセットする
		pDI->m_pProgram->GetNext(pos);
	}
	delete pRecLV;
}


/////////////////////////////////////////////////////////////////////////////
// 
//	
//
CString CChatDoc::DecodePayload(CString strMsg, char *chHdr, int iIndex)
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

	return  strMsg.Mid(iStart, iEnd);
}


/////////////////////////////////////////////////////////////////////////////
// 
//	
//
void CChatDoc::DisplayMsg(LPCTSTR lpszText)
{

	for(POSITION pos=GetFirstViewPosition();pos!=NULL;)
	{
		CView* pView = GetNextView(pos);
		CChatView* pChatView = DYNAMIC_DOWNCAST(CChatView, pView);

		if (pChatView != NULL)
			pChatView->Message(lpszText);
	}
}

/////////////////////////////////////////////////////////////////////////////
// CChatDoc serialization
//
void CChatDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		for(POSITION pos=GetFirstViewPosition();pos!=NULL;)
		{
			CView* pView = GetNextView(pos);
			CChatView* pChatView = DYNAMIC_DOWNCAST(CChatView, pView);

			if (pChatView != NULL)
				pChatView->SerializeRaw(ar);
		}
	}
}

/////////////////////////////////////////////////////////////////////////////
// CChatDoc diagnostics

#ifdef _DEBUG
void CChatDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void CChatDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


/////////////////////////////////////////////////////////////////////////////
// CChatDoc commands
