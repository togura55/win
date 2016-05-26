#pragma once

// CProgInfo コマンド ターゲット

class CProgInfo : public CObject
{
public:
	CProgInfo();
	virtual ~CProgInfo();

// Attribute
	CString		m_strTitle;		// 番組名
	CString		m_strDesc;		// 番組内容
	CString		m_strCategory;	// 番組カテゴリ
	CString		m_strStation;	// 放送局名
	//CString		m_strStartYear;	// 開始日時
	//CString		m_strStartMonth;	// 開始日時
	//CString		m_strStartDay;	// 開始日時
	//CString		m_strStartHour;	// 開始日時
	//CString		m_strStartMinute;	// 開始日時
	//CString		m_strStartSecond;	// 開始日時
	//CString		m_strEndYear;	// 終了日時
	//CString		m_strEndMonth;	// 終了日時
	//CString		m_strEndDay;	// 終了日時
	//CString		m_strEndHour;	// 終了日時
	//CString		m_strEndMinute;	// 終了日時
	//CString		m_strEndSecond;	// 終了日時

	CTime	m_tiStart;
	CTime	m_tiEnd;

	int			m_iStat;		// 録画要求状態
	int			m_iId;			// リクエストID
};


