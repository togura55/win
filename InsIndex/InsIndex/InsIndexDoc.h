// InsIndexDoc.h : CInsIndexDoc クラスのインターフェイス
//


#pragma once


class CInsIndexDoc : public CDocument
{
protected: // シリアル化からのみ作成します。
	CInsIndexDoc();
	DECLARE_DYNCREATE(CInsIndexDoc)

// 属性
public:
	CString	m_strDoc;

// 操作
public:

// オーバーライド
public:
	virtual BOOL OnNewDocument();
	virtual void Serialize(CArchive& ar);

	virtual void DeleteContents();
	virtual BOOL OnOpenDocument(LPCTSTR lpszPathName);
// 実装
public:
	virtual ~CInsIndexDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// 生成された、メッセージ割り当て関数
protected:
	DECLARE_MESSAGE_MAP()
};


