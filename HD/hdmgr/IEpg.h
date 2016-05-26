#pragma once

// CIEpg コマンド ターゲット

class CIEpg : public CObject
{
public:
	CIEpg();
	virtual ~CIEpg();

	// Attribute
	CString strContentType;
	CString strVersion;
	CString strStation;
	CString	strYear;
	CString strMonth;
	CString strDate;
	CString strStart;
	CString strEnd;
	CString strProgramTitle;
	CString strProgramSubtitle;
	CString strPerformer;

};


