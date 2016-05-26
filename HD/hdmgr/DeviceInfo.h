#pragma once

#include "ProgInfo.h"

// CDeviceInfo コマンド ターゲット

#pragma once

class CDeviceInfo : public CObject
{
public:
	CDeviceInfo();
	virtual ~CDeviceInfo();

	// Attribute
	int		m_iDeviceID;		// デバイスID
	CString	m_strDeviceDesc;	// デバイスの説明
	CObList	*m_pProgram;		// プログラムのリスト
	int		m_iNumProg;			// プログラム数
	char	*m_pchHdr;			// Headerパケット

};


