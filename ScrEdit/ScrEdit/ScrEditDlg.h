// ScrEditDlg.h : ヘッダー ファイル
//

#pragma once
#include "afxcmn.h"
#include "PropMain.h"
#include "PropConfig.h"


// CScrEditDlg ダイアログ
//class CDlgScrEdit : public CDialog
class CDlgScrEdit : public CPropertySheet
{
// コンストラクション
public:
	CDlgScrEdit(CWnd* pParent = NULL);	// 標準コンストラクタ

// ダイアログ データ
	enum { IDD = IDD_SCREDIT_DIALOG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV サポート


// 実装
protected:
	HICON m_hIcon;

	// 生成された、メッセージ割り当て関数
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()


public:
	afx_msg void OnTcnSelchangeTabScredit(NMHDR *pNMHDR, LRESULT *pResult);
	
	CDialog	*m_pReadSrcDlg;
	CPropMain	m_propMain;
    CPropConfig	m_propConfig;

};
