// ScrEditDlg.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ScrEdit.h"
#include "ScrEditDlg.h"
#include "ReadScrDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// �A�v���P�[�V�����̃o�[�W�������Ɏg���� CAboutDlg �_�C�A���O

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

// ����
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CScrEditDlg �_�C�A���O




CDlgScrEdit::CDlgScrEdit(CWnd* pParent /*=NULL*/)
//	: CPropertySheet(CDlgScrEdit::IDD, pParent)
	: CPropertySheet(_T(""), pParent)	// Edited for Propertysheet
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);

	m_pReadSrcDlg = NULL;

	static CString strMain, strConfig;
	CString str;


	// Invisible Help button
	this->m_psh.dwFlags &= ~PSH_HASHELP;
	m_propMain.m_psp.dwFlags &= ~PSP_HASHELP;
	m_propConfig.m_psp.dwFlags &= ~PSP_HASHELP;


	m_propMain.m_psp.dwFlags |= PSP_USETITLE;
	strMain.LoadStringW(IDS_MAIN_PROPMAIN);m_propMain.m_psp.pszTitle = strMain;		// �萔������Ƃ��Ďw��

	m_propConfig.m_psp.dwFlags |= PSP_USETITLE;
	strConfig.LoadStringW(IDS_CONFIG_PROPCONFIG);m_propConfig.m_psp.pszTitle = strConfig;	// �萔������Ƃ��Ďw��

	AddPage(&m_propMain);
    AddPage(&m_propConfig);

	str.LoadStringW(IDS_PROGRAMNAME);SetTitle(str, 0);

}

void CDlgScrEdit::DoDataExchange(CDataExchange* pDX)
{
	CPropertySheet::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CDlgScrEdit, CPropertySheet)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


// CScrEditDlg ���b�Z�[�W �n���h��

BOOL CDlgScrEdit::OnInitDialog()
{
	CPropertySheet::OnInitDialog();

	// "�o�[�W�������..." ���j���[���V�X�e�� ���j���[�ɒǉ����܂��B

	// IDM_ABOUTBOX �́A�V�X�e�� �R�}���h�͈͓̔��ɂȂ���΂Ȃ�܂���B
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// ���̃_�C�A���O�̃A�C�R����ݒ肵�܂��B�A�v���P�[�V�����̃��C�� �E�B���h�E���_�C�A���O�łȂ��ꍇ�A
	//  Framework �́A���̐ݒ�������I�ɍs���܂��B
	SetIcon(m_hIcon, TRUE);			// �傫���A�C�R���̐ݒ�
	SetIcon(m_hIcon, FALSE);		// �������A�C�R���̐ݒ�

	// TODO: �������������ɒǉ����܂��B
	//{
	//	CDialogPropSheet   cPropSheet(_T(""), this);
	//	bool            main;
	//	unsigned int    config;
	//	CString         str;
	//    
	//	if (cPropSheet.DoModal() == IDOK)
	//	{
	//		main = (AfxGetApp()->GetProfileInt(_T("Option"), _T("Main"), 0)) ? true : false;
	//		config = AfxGetApp()->GetProfileInt(_T("Option"), _T("Config"), 0);

	//		//m_xvEditRes = (kind == 0) ? _T("�e�L�X�g") : 
	//		//    (kind == 1) ? _T("�摜") : _T("�摜�ƃe�L�X�g");
	//		//m_xvEditRes = m_xvEditRes +_T("���\��") 
	//		//    +(disp ? _T("����܂��B") : _T("����܂���B"));
	//		UpdateData(FALSE);
	//	}
	//}


	SendMessage (DM_SETDEFID, IDC_LIST_AS);	// Default focus set

	// ReadScr �q�_�C�A���O�E�B���h�E�̕\��
	if (!theApp.m_bDefaultScriptFile)
	{
		m_pReadSrcDlg = new CReadScrDlg(this);

		//if (m_pReadSrcDlg->DoModal() == IDOK){
		//}

		m_pReadSrcDlg->ShowWindow(true);
	}
	else
	{
		// Script File�̓ǂݍ���
		theApp.ReadScrFile(theApp.m_strScriptFile);
	}

	return FALSE;  // �t�H�[�J�X���R���g���[���ɐݒ肵���ꍇ�������ATRUE ��Ԃ��܂��B
}

void CDlgScrEdit::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CPropertySheet::OnSysCommand(nID, lParam);
	}
}

// �_�C�A���O�ɍŏ����{�^����ǉ�����ꍇ�A�A�C�R����`�悷�邽�߂�
//  ���̃R�[�h���K�v�ł��B�h�L�������g/�r���[ ���f�����g�� MFC �A�v���P�[�V�����̏ꍇ�A
//  ����́AFramework �ɂ���Ď����I�ɐݒ肳��܂��B

void CDlgScrEdit::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // �`��̃f�o�C�X �R���e�L�X�g

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// �N���C�A���g�̎l�p�`�̈���̒���
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// �A�C�R���̕`��
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CPropertySheet::OnPaint();
	}
}

// ���[�U�[���ŏ��������E�B���h�E���h���b�O���Ă���Ƃ��ɕ\������J�[�\�����擾���邽�߂ɁA
//  �V�X�e�������̊֐����Ăяo���܂��B
HCURSOR CDlgScrEdit::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



