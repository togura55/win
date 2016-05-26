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
	CTime		m_tiStart;		// 開始日時
	CTime		m_tiEnd;		// 終了日時
	int			m_iStat;		// 録画要求状態
	int			m_iId;			// リクエストID

	char		m_chHdr[8];	// ヘッダー部 固定長

};


