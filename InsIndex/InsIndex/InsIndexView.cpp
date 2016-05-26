// InsIndexView.cpp : CInsIndexView �N���X�̎���
//

#include "stdafx.h"
#include "InsIndex.h"

#include "InsIndexDoc.h"
#include "InsIndexView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CInsIndexView

IMPLEMENT_DYNCREATE(CInsIndexView, CFormView)

BEGIN_MESSAGE_MAP(CInsIndexView, CFormView)
	// �W������R�}���h
	ON_COMMAND(ID_FILE_PRINT, &CFormView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CFormView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CFormView::OnFilePrintPreview)
END_MESSAGE_MAP()

// CInsIndexView �R���X�g���N�V����/�f�X�g���N�V����

CInsIndexView::CInsIndexView()
	: CFormView(CInsIndexView::IDD)
{
    TRACE("Entering CInsIndexView constructor\n");
	// TODO: �\�z�R�[�h�������ɒǉ����܂��B

}

CInsIndexView::~CInsIndexView()
{
}

BOOL CInsIndexView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: ���̈ʒu�� CREATESTRUCT cs ���C������ Window �N���X�܂��̓X�^�C����
	//  �C�����Ă��������B

	return CFormView::PreCreateWindow(cs);
}

// CInsIndexView �`��

void CInsIndexView::OnDraw(CDC* /*pDC*/)
{
	CInsIndexDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: ���̏ꏊ�Ƀl�C�e�B�u �f�[�^�p�̕`��R�[�h��ǉ����܂��B
}


// CInsIndexView ���

BOOL CInsIndexView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// ����̈������
	return DoPreparePrinting(pInfo);
}

void CInsIndexView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: ����O�̓��ʂȏ�����������ǉ����Ă��������B
}

void CInsIndexView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: �����̌㏈����ǉ����Ă��������B
}


// CInsIndexView �f�f

#ifdef _DEBUG
void CInsIndexView::AssertValid() const
{
	CFormView::AssertValid();
}

void CInsIndexView::Dump(CDumpContext& dc) const
{
	CFormView::Dump(dc);
}

CInsIndexDoc* CInsIndexView::GetDocument() const // �f�o�b�O�ȊO�̃o�[�W�����̓C�����C���ł��B
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CInsIndexDoc)));
	return (CInsIndexDoc*)m_pDocument;
}
#endif //_DEBUG


// CInsIndexView ���b�Z�[�W �n���h��
