// InsIndexDoc.h : CInsIndexDoc �N���X�̃C���^�[�t�F�C�X
//


#pragma once


class CInsIndexDoc : public CDocument
{
protected: // �V���A��������̂ݍ쐬���܂��B
	CInsIndexDoc();
	DECLARE_DYNCREATE(CInsIndexDoc)

// ����
public:
	CString	m_strDoc;

// ����
public:

// �I�[�o�[���C�h
public:
	virtual BOOL OnNewDocument();
	virtual void Serialize(CArchive& ar);

	virtual void DeleteContents();
	virtual BOOL OnOpenDocument(LPCTSTR lpszPathName);
// ����
public:
	virtual ~CInsIndexDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// �������ꂽ�A���b�Z�[�W���蓖�Ċ֐�
protected:
	DECLARE_MESSAGE_MAP()
};


