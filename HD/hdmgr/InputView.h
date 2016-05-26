#pragma once
#include "afxwin.h"



// CInputView フォーム ビュー

class CInputView : public CFormView
{
	DECLARE_DYNCREATE(CInputView)

protected:
	CInputView();           // 動的生成で使用される protected コンストラクタ
	virtual ~CInputView();

public:
	//{{AFX_DATA(CInputView)
	enum { IDD = IDD_INPUTFORM };
	//}}AFX_DATA

	CComboBox m_xcRecRequest;

#ifdef _DEBUG
	virtual void AssertValid() const;
#ifndef _WIN32_WCE
	virtual void Dump(CDumpContext& dc) const;
#endif
#endif

	virtual void OnInitialUpdate();

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedPbtnRecrequest();
	afx_msg void OnBnClickedPbtnSetconfig();
	afx_msg void OnBnClickedPbtnGetlist();
	afx_msg void OnCbnDropdownComboRecrequest();

	afx_msg void OnBnClickedPbtnGetlist2();
	afx_msg void OnBnClickedPbtnReadfile();
};


