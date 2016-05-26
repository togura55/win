// USBTpMonDlg.h : header file
//

#pragma once
#include "afxwin.h"
#include "afxcmn.h"


// CUSBTpMonDlg dialog
class CUSBTpMonDlg : public CDialog
{
// Construction
public:
	CUSBTpMonDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_USBTPMON_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedButton1();
	afx_msg void OnBnClickedPbtnDetectsensor();
	afx_msg void OnBnClickedPbtnLed0();
	afx_msg void OnBnClickedPbtnLed1();
	afx_msg void OnBnClickedPbtnHeater();
	afx_msg void OnBnClickedPbtnStart();
	CListCtrl m_xcListDevice;
	bool UpdateListDevice(int iSelect);
	bool UpdateControls(int iSelect);
	CButton m_xcStart;
	CButton m_xcLed0;
	CButton m_xcLed1;
	CButton m_xcHeater;
	CButton m_xcAutoDetect;
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg void OnClose();
	bool DetectSensor(void);
	BOOL m_xvAutoDetect;
	afx_msg void OnBnClickedCbAutodetect();
	void AddSensor(BSTR);
	void ToggleLED(int);
	double m_xvCalibTemp;
	double m_xvCalibHumid;
	double m_xvActValue;
	CComboBox m_xcActCondition;
	int m_xvActCondition;
	CListCtrl m_xcListAction;
	afx_msg void OnCbnSelchangeComboActcondition();
	int ListActionInit(void);
	int ListActionInsertItem(void);
	int ListDeviceInit(void);
	int ListDeviceInsertItem(int, CString);
	afx_msg void OnLvnItemchangedListSensor(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnEndlabeleditListSensor(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnLvnItemActivateListSensor(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnHdnItemStateIconClickListSensor(NMHDR *pNMHDR, LRESULT *pResult);
	int SendMail(void);
};
