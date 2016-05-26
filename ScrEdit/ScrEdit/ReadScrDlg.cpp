// ReadScrDlg.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ScrEdit.h"
#include "ReadScrDlg.h"
#include "ScrEditDlg.h"

// CReadScrDlg ダイアログ

IMPLEMENT_DYNAMIC(CReadScrDlg, CDialog)

CReadScrDlg::CReadScrDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CReadScrDlg::IDD, pParent)
	, m_xvEditReadScriptFile(_T(""))
	, m_xvCB_UseDef(FALSE)
{

	Create(CReadScrDlg::IDD, pParent);
}

CReadScrDlg::~CReadScrDlg()
{
}

void CReadScrDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT_READSCRIPT_FILE, m_xvEditReadScriptFile);
	DDX_Check(pDX, IDC_CB_READSCRIPT_DEFAULT, m_xvCB_UseDef);
}


BEGIN_MESSAGE_MAP(CReadScrDlg, CDialog)
	ON_EN_CHANGE(IDC_EDIT_READSCRIPT_FILE, &CReadScrDlg::OnEnChangeEdit1)
	ON_BN_CLICKED(IDC_PBTN_READSCRIPT_REFERENCE, &CReadScrDlg::OnBnClickedPbtnReadscriptReference)
	ON_BN_CLICKED(IDOK, &CReadScrDlg::OnBnClickedOk)
	ON_BN_CLICKED(IDC_CB_READSCRIPT_DEFAULT, &CReadScrDlg::OnBnClickedCbReadscriptDefault)
END_MESSAGE_MAP()


// CReadScrDlg メッセージ ハンドラ

void CReadScrDlg::OnEnChangeEdit1()
{
	// TODO:  これが RICHEDIT コントロールの場合、
	// まず、CDialog::OnInitDialog() 関数をオーバーライドして、OR 状態の ENM_CHANGE
	// フラグをマスクに入れて、CRichEditCtrl().SetEventMask() を呼び出さない限り、
	// コントロールは、この通知を送信しません。

	// TODO:  ここにコントロール通知ハンドラ コードを追加してください。
}

/////////////////////////////////////////////////
// Reference button hamdler
//
void CReadScrDlg::OnBnClickedPbtnReadscriptReference()
{
	// [ファイルを開く]ダイアログ
	CFileDialog dlg(TRUE, _T("txt"), NULL, OFN_HIDEREADONLY | OFN_CREATEPROMPT,
		_T("テキスト ファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*||"));

	CString		strPath, strTmp, str;

	if(dlg.DoModal() == IDOK)
	{
		strPath = dlg.GetPathName();
		m_xvEditReadScriptFile = strPath;	// エディットボックスにフルパスの表示
		UpdateData(FALSE);
	}
}

/////////////////////////////////////////////////
// OK button handler
//
void CReadScrDlg::OnBnClickedOk()
{
	// エディットボックスからファイル名を取得
	theApp.m_strScriptFile = m_xvEditReadScriptFile;

	// Script Fileの読み込み
	theApp.ReadScrFile(theApp.m_strScriptFile);

	OnOK();
}

/////////////////////////////////////////////////
// 
//
void CReadScrDlg::OnBnClickedCbReadscriptDefault()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
}

/////////////////////////////////////////////////
// 
//
BOOL CReadScrDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  ここに初期化を追加してください
	CString str;
	str.LoadStringW(IDS_READSCRIPT_TITLE);SetWindowText(str);
	str.LoadStringW(IDS_READSCRIPT_DESC);SetDlgItemText(IDC_TXT_READSCRIPT_DESC, str);
	str.LoadStringW(IDS_READSCRIPT_REFERENCE);SetDlgItemText(IDC_PBTN_READSCRIPT_REFERENCE, str);
	str.LoadStringW(IDS_READSCRIPT_DEFAULT);SetDlgItemText(IDC_CB_READSCRIPT_DEFAULT, str);

	m_xvCB_UseDef = theApp.m_bDefaultScriptFile;

	//SetActiveWindow();
	//SetFocus();
//	CReadScrDlg::GotoDlgCtrl(GetDlgItem(IDC_PBTN_READSCRIPT_REFERENCE)); // ListBoxが最初のフォーカスを持つ

	return FALSE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}

void CReadScrDlg::OnCancel()
{
	// TODO: ここに特定なコードを追加するか、もしくは基本クラスを呼び出してください。
//	CDialog::OnCancel();

	DestroyWindow();
}

void CReadScrDlg::OnOK()
{
	// TODO: ここに特定なコードを追加するか、もしくは基本クラスを呼び出してください。
//	CDialog::OnOK();
	
	UpdateData();	// DDX変数の値を更新

	m_xvCB_UseDef? theApp.m_bDefaultScriptFile=true : theApp.m_bDefaultScriptFile = false;

	theApp.WriteRegSettings();	// Update Registry

	DestroyWindow();
}

void CReadScrDlg::PostNcDestroy()
{
	// TODO: ここに特定なコードを追加するか、もしくは基本クラスを呼び出してください。
//	CDialog::PostNcDestroy();

	delete this;
}
