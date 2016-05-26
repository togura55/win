
// DLLLodDlg.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "DLLLod.h"
#include "DLLLodDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CDLLLodDlg �_�C�A���O




CDLLLodDlg::CDLLLodDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CDLLLodDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CDLLLodDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CDLLLodDlg, CDialog)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_BUTTON1, &CDLLLodDlg::OnBnClickedButton1)
END_MESSAGE_MAP()


// CDLLLodDlg ���b�Z�[�W �n���h��

BOOL CDLLLodDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// ���̃_�C�A���O�̃A�C�R����ݒ肵�܂��B�A�v���P�[�V�����̃��C�� �E�B���h�E���_�C�A���O�łȂ��ꍇ�A
	//  Framework �́A���̐ݒ�������I�ɍs���܂��B
	SetIcon(m_hIcon, TRUE);			// �傫���A�C�R���̐ݒ�
	SetIcon(m_hIcon, FALSE);		// �������A�C�R���̐ݒ�

	// TODO: �������������ɒǉ����܂��B

	return TRUE;  // �t�H�[�J�X���R���g���[���ɐݒ肵���ꍇ�������ATRUE ��Ԃ��܂��B
}

// �_�C�A���O�ɍŏ����{�^����ǉ�����ꍇ�A�A�C�R����`�悷�邽�߂�
//  ���̃R�[�h���K�v�ł��B�h�L�������g/�r���[ ���f�����g�� MFC �A�v���P�[�V�����̏ꍇ�A
//  ����́AFramework �ɂ���Ď����I�ɐݒ肳��܂��B

void CDLLLodDlg::OnPaint()
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
		CDialog::OnPaint();
	}
}

// ���[�U�[���ŏ��������E�B���h�E���h���b�O���Ă���Ƃ��ɕ\������J�[�\�����擾���邽�߂ɁA
//  �V�X�e�������̊֐����Ăяo���܂��B
HCURSOR CDLLLodDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



typedef BSTR (__stdcall GETVERS)( BSTR *);
typedef BSTR (__stdcall FINDUSB)( int *);

void CDLLLodDlg::OnBnClickedButton1()
{
	HINSTANCE	hInst;
	GETVERS		*pfnDllFuncGetVers;
	FINDUSB		*pfnDllFuncFindUSB;

	hInst = ::LoadLibrary(_T("USBMeter") );	//DLL��ǂݍ��݂܂��B
	if( hInst == NULL )
	{
		MessageBox(_T("�c�k�k�����[�h�ł��܂���ł����B") );
		return;
	}

	pfnDllFuncGetVers = (GETVERS *)::GetProcAddress( hInst, "_GetVers@4" );	//�֐��̃A�h���X���擾���܂��B
	pfnDllFuncFindUSB = (FINDUSB *)::GetProcAddress( hInst, "_FindUSB@4" );	//�֐��̃A�h���X���擾���܂��B
	if( pfnDllFuncGetVers == NULL)
	{
		MessageBox(_T("�֐����擾�ł��܂���ł����B") );
		::FreeLibrary( hInst );       //DLL��������܂��B
		return;
	}

	BSTR str;			// Srring type compatibility w/ VB
	BSTR strParam;
	int index = 0;

//	static TCHAR s[MAX_PATH +1];
//strParam = s;

//	static TCHAR strParam[MAX_PATH +1];

	str = pfnDllFuncGetVers( &strParam );	//�Ăяo���I
	str = pfnDllFuncFindUSB( &index );	//�Ăяo���I

	::FreeLibrary( hInst ); 
}
