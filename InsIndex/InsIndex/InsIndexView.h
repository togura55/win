// InsIndexView.h : CInsIndexView �N���X�̃C���^�[�t�F�C�X
//


#pragma once


class CInsIndexView : public CFormView
{
public:
	//{{AFX_DATA(CInsIndexView)
	enum { IDD = IDD_DLG_MAIN };
	//}}AFX_DATA

protected: // �V���A��������̂ݍ쐬���܂��B
	CInsIndexView();
	DECLARE_DYNCREATE(CInsIndexView)

// ����
public:
	CInsIndexDoc* GetDocument() const;

// ����
public:

// �I�[�o�[���C�h
public:
	virtual void OnDraw(CDC* pDC);  // ���̃r���[��`�悷�邽�߂ɃI�[�o�[���C�h����܂��B
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);

protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// ����
public:
	virtual ~CInsIndexView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// �������ꂽ�A���b�Z�[�W���蓖�Ċ֐�
protected:
	DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG  // InsIndexView.cpp �̃f�o�b�O �o�[�W����
inline CInsIndexDoc* CInsIndexView::GetDocument() const
   { return reinterpret_cast<CInsIndexDoc*>(m_pDocument); }
#endif

