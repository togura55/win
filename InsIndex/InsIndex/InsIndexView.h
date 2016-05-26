// InsIndexView.h : CInsIndexView クラスのインターフェイス
//


#pragma once


class CInsIndexView : public CFormView
{
public:
	//{{AFX_DATA(CInsIndexView)
	enum { IDD = IDD_DLG_MAIN };
	//}}AFX_DATA

protected: // シリアル化からのみ作成します。
	CInsIndexView();
	DECLARE_DYNCREATE(CInsIndexView)

// 属性
public:
	CInsIndexDoc* GetDocument() const;

// 操作
public:

// オーバーライド
public:
	virtual void OnDraw(CDC* pDC);  // このビューを描画するためにオーバーライドされます。
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);

protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// 実装
public:
	virtual ~CInsIndexView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// 生成された、メッセージ割り当て関数
protected:
	DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG  // InsIndexView.cpp のデバッグ バージョン
inline CInsIndexDoc* CInsIndexView::GetDocument() const
   { return reinterpret_cast<CInsIndexDoc*>(m_pDocument); }
#endif

