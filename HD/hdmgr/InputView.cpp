// InputView.cpp : 実装ファイル
//

#include "stdafx.h"
#include "resource.h"
#include "chatter.h"
#include "InputView.h"
#include "Chatdoc.h"
#include "Proginfo.h"
#include "iEpg.h"

// CInputView

IMPLEMENT_DYNCREATE(CInputView, CFormView)

CInputView::CInputView()
	: CFormView(CInputView::IDD)
{
	//{{AFX_DATA_INIT(CInputView)
	//}}AFX_DATA_INIT
}

CInputView::~CInputView()
{
}

void CInputView::DoDataExchange(CDataExchange* pDX)
{
	CChatterApp* pApp = (CChatterApp*)AfxGetApp();

	CFormView::DoDataExchange(pDX);

	//{{AFX_DATA_MAP(CInputView)
	DDX_Text(pDX, IDC_EDIT_CHANNEL, pApp->m_nChannel);
	DDV_MinMaxInt(pDX, pApp->m_nChannel, 0, 99);
	DDX_Text(pDX, IDC_EDIT_HANDLE, pApp->m_strHandle);
	DDV_MaxChars(pDX, pApp->m_strHandle, 50);
	DDX_Text(pDX, IDC_EDIT_SERVER, pApp->m_strServer);
	DDV_MaxChars(pDX, pApp->m_strServer, 255);

	//DDX_CBString(pDX, IDC_COMBO_RECREQUEST, m_strRecRequest);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_COMBO_RECREQUEST, m_xcRecRequest);
}

BEGIN_MESSAGE_MAP(CInputView, CFormView)
	ON_BN_CLICKED(IDC_PBTN_RECREQUEST, &CInputView::OnBnClickedPbtnRecrequest)
	ON_BN_CLICKED(IDC_PBTN_SETCONFIG, &CInputView::OnBnClickedPbtnSetconfig)
	ON_BN_CLICKED(IDC_PBTN_GETLIST, &CInputView::OnBnClickedPbtnGetlist)
	ON_CBN_DROPDOWN(IDC_COMBO_RECREQUEST, &CInputView::OnCbnDropdownComboRecrequest)
	ON_BN_CLICKED(IDC_PBTN_READFILE, &CInputView::OnBnClickedPbtnReadfile)
END_MESSAGE_MAP()


// CInputView 診断

#ifdef _DEBUG
void CInputView::AssertValid() const
{
	CFormView::AssertValid();
}

#ifndef _WIN32_WCE
void CInputView::Dump(CDumpContext& dc) const
{
	CFormView::Dump(dc);
}
#endif
#endif //_DEBUG


// CInputView メッセージ ハンドラ

/////////////////////////////////////////////////////////////////////////////
// OnBnClickedPbtnRecrequest
//  Event handler called when "Rec" button is pressed
//
void CInputView::OnBnClickedPbtnRecrequest()
{
	int sel=0;
	CString strText;
	CProgInfo*	pPI = NULL;
	CChatterApp* pApp = (CChatterApp*)AfxGetApp();
	CObList* pProgList = pApp->m_pProgramInfo;
	CChatDoc* pDoc = (CChatDoc*)pApp->m_pChatDoc;

	// Send a command byte to the server
	strText.Format(_T("%d"), CB_SEND_DATASTREAM);
	pDoc->SendMsg(strText);		// 送信

	// Send header part
	UpdateData(TRUE);
	sel = m_xcRecRequest.GetCurSel(); // a number of the selected cell

	pPI = (CProgInfo *)pProgList->GetAt(pProgList->FindIndex(sel)); // Get program information

	// Pack the program information data for the transmitting
	int header = 0, i = 0;	
	char ch[9];

	ch[i++] = (char)pPI->m_strTitle.GetLength();		// タイトル文字列の長さ
	ch[i++] = (char)pPI->m_strDesc.GetLength();			// 説明文字列の長さ
	ch[i++] = (char)pPI->m_strCategory.GetLength();		// カテゴリ文字列の長さ
	ch[i++] = (char)pPI->m_strStation.GetLength();		// 局名文字列の長さ
	ch[i++] = (char)printf("%d",pPI->m_tiStart.GetTime()); // 開始時間を表すtime64_t値の桁数
	ch[i++] = (char)printf("%d",pPI->m_tiEnd.GetTime()); // 終了時間を表すtime64_t値の桁数
	ch[i++] = (char)printf("%d",pPI->m_iStat);			// 録画要求状態の桁数
	ch[i++] = (char)printf("%d",pPI->m_iId);			// リクエストIDの桁数
	ch[i++] = (char)(_T("\r\n"));						// 終端文字の付加

	pDoc->SendMsg((CString)ch);		// 送信

	// Payload part
	CString strP;
	strText.Empty();

	strP.Append(pPI->m_strTitle);
	strP.Append(pPI->m_strDesc);
	strP.Append(pPI->m_strCategory);
	strP.Append(pPI->m_strStation);

	strText.Format(_T("%d"),pPI->m_tiStart.GetTime());strP.Append(strText);
	strText.Format(_T("%d"),pPI->m_tiEnd.GetTime());strP.Append(strText);

	strText.Format(_T("%d"),pPI->m_iStat);strP.Append(strText);
	strText.Format(_T("%d"),pPI->m_iId);strP.Append(strText);

	pDoc->SendMsg(strP);		// 送信

}

/////////////////////////////////////////////////////////////////////////////
// OnBnClickedPbtnSetconfig
//  Event handler called when "Set Config" button is pressed
//
void CInputView::OnBnClickedPbtnSetconfig()
{
	CChatterApp* pApp = (CChatterApp*)AfxGetApp();
	CChatDoc* pDoc = pApp->m_pChatDoc;
	CString strText;

	UpdateData(TRUE);
	if (pDoc->ConnectSocket(pApp->m_strHandle, pApp->m_strServer, pApp->m_nChannel))
	{
		// 通信経路が確立されたので、コントロールをEnable
		(CButton *)GetDlgItem(IDC_PBTN_RECREQUEST)->EnableWindow(TRUE);
		(CButton *)GetDlgItem(IDC_PBTN_GETLIST)->EnableWindow(TRUE);

		// DeviceInfoオブジェクトの作成
		// ToDo


		// デバイスから基本情報を取得する
		// Send a command byte to the server
		pDoc->m_iReadStat = WAIT_GETDEVICEINFO_DATA_HEADER;		//受信モードのセット
		strText.Format(_T("%d"), CB_GET_DEVICEINFO);
		pDoc->SendMsg(strText);		// 送信

	}
}

/////////////////////////////////////////////////////////////////////////////
// OnBnClickedPbtnGetlist
//  Event handler called when "Get List" button is pressed
//
void CInputView::OnBnClickedPbtnGetlist()
{
	CString strText;
	CChatterApp* pApp = (CChatterApp*)AfxGetApp();
	CChatDoc* pDoc = (CChatDoc*)pApp->m_pChatDoc;

	// Set mode
	pDoc->m_iReadStat = WAIT_GETLIST_DATA_LISTSIZE;	// 受信モードの更新

	// Send a command byte to the server
	strText.Format(_T("%d"), CB_GET_GETLIST);
	pDoc->SendMsg(strText);


}

void CInputView::OnCbnDropdownComboRecrequest()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
}

/////////////////////////////////////////////////////////////////////////////
// CInputView::OnInitialUpdate()
// 　コンボボックスを初期化して表示データをリストにセット
//
void CInputView::OnInitialUpdate()
{
	CFormView::OnInitialUpdate();

	CComboBox *pComboBox;
	CChatterApp* pApp = (CChatterApp*)AfxGetApp();
	CProgInfo*	pPI;
	POSITION pos  = pApp->m_pProgramInfo->GetHeadPosition();

	// 通信経路が確立されるまでは、コントロールをDisable
	(CButton *)GetDlgItem(IDC_PBTN_RECREQUEST)->EnableWindow(FALSE);
	(CButton *)GetDlgItem(IDC_PBTN_GETLIST)->EnableWindow(FALSE);

	// リストボックスに初期値を追加
	pComboBox = (CComboBox *)GetDlgItem(IDC_COMBO_RECREQUEST);
	pPI = (CProgInfo*)pApp->m_pProgramInfo->GetAt(pos);
	m_xcRecRequest.InsertString(-1, pPI->m_strTitle);
	pApp->m_pProgramInfo->GetNext(pos);

	pPI = (CProgInfo*)pApp->m_pProgramInfo->GetAt(pos);
	m_xcRecRequest.InsertString(-1, pPI->m_strTitle);
	pApp->m_pProgramInfo->GetNext(pos);

	pPI = (CProgInfo*)pApp->m_pProgramInfo->GetAt(pos);
	m_xcRecRequest.InsertString(-1, pPI->m_strTitle);
}


/////////////////////////////////////////////////////////////////////////////
// CInputView::OnBnClickedPbtnReadfile()
// 　Read Fileボタンの処理
//
void CInputView::OnBnClickedPbtnReadfile()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	// [ファイルを開く]ダイアログ
	CFileDialog dlg(TRUE, _T("tvpi"), NULL, OFN_HIDEREADONLY | OFN_CREATEPROMPT,
		_T("テキスト ファイル (*.tvpi)|*.tvpi|すべてのファイル (*.*)|*.*||"));

	CString		strPath, strTmp, str;
	CStdioFile	stdFile;


	if(dlg.DoModal() == IDOK)
	{
		//CString strMsg;
		//strMsg.Format("%s", dlg.GetPathName());
		//MessageBox(strMsg);
		//CString	iEPGString;
		
		CStringList	 striEPG;

		strPath = dlg.GetPathName();
		if(stdFile.Open(strPath,CFile::modeRead)) 	//ファイルを正常に開けた場合
		{
			// iEPGファイルの全行を読み込む
			while(stdFile.ReadString(strTmp))
			{
				striEPG.AddTail(strTmp);
			}
			// iEPGのparse
			CIEpg iEPG;
			POSITION pos;

			pos = striEPG.GetHeadPosition();

			while(1)
			{
				iEPG.strContentType = striEPG.GetAt(pos);
				if (iEPG.strContentType.IsEmpty())
					break;

				iEPG.strVersion = striEPG.GetNext(pos);
				if (iEPG.strVersion.IsEmpty())
					break;

				iEPG.strStation = striEPG.GetNext(pos);
				if (iEPG.strStation.IsEmpty())
					break;

				iEPG.strYear = striEPG.GetNext(pos);
				if (iEPG.strYear.IsEmpty())
					break;

				iEPG.strMonth = striEPG.GetNext(pos);
				if (iEPG.strMonth.IsEmpty())
					break;	

				iEPG.strDate = striEPG.GetNext(pos);
				if (iEPG.strDate.IsEmpty())
					break;	

				iEPG.strStart = striEPG.GetNext(pos);
				if (iEPG.strStart.IsEmpty())
					break;	

				iEPG.strEnd = striEPG.GetNext(pos);
				if (iEPG.strEnd.IsEmpty())
					break;	
				
				iEPG.strProgramTitle = striEPG.GetNext(pos);
				if (iEPG.strProgramTitle.IsEmpty())
					break;	
				
				iEPG.strProgramSubtitle = striEPG.GetNext(pos);
				if (iEPG.strProgramSubtitle.IsEmpty())
					break;	
				
				iEPG.strPerformer = striEPG.GetNext(pos);
				if (iEPG.strPerformer.IsEmpty())
					break;	

			}

			stdFile.Close();
			//AfxMessageBox(str);
		}
		else{
			AfxMessageBox("Could not open file");
		}

	}

}
