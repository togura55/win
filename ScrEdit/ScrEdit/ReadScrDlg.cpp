// ReadScrDlg.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ScrEdit.h"
#include "ReadScrDlg.h"
#include "ScrEditDlg.h"

// CReadScrDlg �_�C�A���O

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


// CReadScrDlg ���b�Z�[�W �n���h��

void CReadScrDlg::OnEnChangeEdit1()
{
	// TODO:  ���ꂪ RICHEDIT �R���g���[���̏ꍇ�A
	// �܂��ACDialog::OnInitDialog() �֐����I�[�o�[���C�h���āAOR ��Ԃ� ENM_CHANGE
	// �t���O���}�X�N�ɓ���āACRichEditCtrl().SetEventMask() ���Ăяo���Ȃ�����A
	// �R���g���[���́A���̒ʒm�𑗐M���܂���B

	// TODO:  �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����Ă��������B
}

/////////////////////////////////////////////////
// Reference button hamdler
//
void CReadScrDlg::OnBnClickedPbtnReadscriptReference()
{
	// [�t�@�C�����J��]�_�C�A���O
	CFileDialog dlg(TRUE, _T("txt"), NULL, OFN_HIDEREADONLY | OFN_CREATEPROMPT,
		_T("�e�L�X�g �t�@�C�� (*.txt)|*.txt|���ׂẴt�@�C�� (*.*)|*.*||"));

	CString		strPath, strTmp, str;

	if(dlg.DoModal() == IDOK)
	{
		strPath = dlg.GetPathName();
		m_xvEditReadScriptFile = strPath;	// �G�f�B�b�g�{�b�N�X�Ƀt���p�X�̕\��
		UpdateData(FALSE);
	}
}

/////////////////////////////////////////////////
// OK button handler
//
void CReadScrDlg::OnBnClickedOk()
{
	// �G�f�B�b�g�{�b�N�X����t�@�C�������擾
	theApp.m_strScriptFile = m_xvEditReadScriptFile;

	// Script File�̓ǂݍ���
	theApp.ReadScrFile(theApp.m_strScriptFile);

	OnOK();
}

/////////////////////////////////////////////////
// 
//
void CReadScrDlg::OnBnClickedCbReadscriptDefault()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
}

/////////////////////////////////////////////////
// 
//
BOOL CReadScrDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  �����ɏ�������ǉ����Ă�������
	CString str;
	str.LoadStringW(IDS_READSCRIPT_TITLE);SetWindowText(str);
	str.LoadStringW(IDS_READSCRIPT_DESC);SetDlgItemText(IDC_TXT_READSCRIPT_DESC, str);
	str.LoadStringW(IDS_READSCRIPT_REFERENCE);SetDlgItemText(IDC_PBTN_READSCRIPT_REFERENCE, str);
	str.LoadStringW(IDS_READSCRIPT_DEFAULT);SetDlgItemText(IDC_CB_READSCRIPT_DEFAULT, str);

	m_xvCB_UseDef = theApp.m_bDefaultScriptFile;

	//SetActiveWindow();
	//SetFocus();
//	CReadScrDlg::GotoDlgCtrl(GetDlgItem(IDC_PBTN_READSCRIPT_REFERENCE)); // ListBox���ŏ��̃t�H�[�J�X������

	return FALSE;  // return TRUE unless you set the focus to a control
	// ��O : OCX �v���p�e�B �y�[�W�͕K�� FALSE ��Ԃ��܂��B
}

void CReadScrDlg::OnCancel()
{
	// TODO: �����ɓ���ȃR�[�h��ǉ����邩�A�������͊�{�N���X���Ăяo���Ă��������B
//	CDialog::OnCancel();

	DestroyWindow();
}

void CReadScrDlg::OnOK()
{
	// TODO: �����ɓ���ȃR�[�h��ǉ����邩�A�������͊�{�N���X���Ăяo���Ă��������B
//	CDialog::OnOK();
	
	UpdateData();	// DDX�ϐ��̒l���X�V

	m_xvCB_UseDef? theApp.m_bDefaultScriptFile=true : theApp.m_bDefaultScriptFile = false;

	theApp.WriteRegSettings();	// Update Registry

	DestroyWindow();
}

void CReadScrDlg::PostNcDestroy()
{
	// TODO: �����ɓ���ȃR�[�h��ǉ����邩�A�������͊�{�N���X���Ăяo���Ă��������B
//	CDialog::PostNcDestroy();

	delete this;
}
