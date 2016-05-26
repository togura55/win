// DeviceList.cpp : 実装ファイル
//

#include "stdafx.h"
#include "USBTpMon.h"
#include "DeviceList.h"


// DeviceList

DeviceList::DeviceList()
{
	// Initialization
//	devName = _T("");		// ToDo BSTRの初期化
	devFWVer = _T("");
	devTemp = 0;
	devHumid = 0;
	devSetHeater = 0;
//	devPortList = NULL;

	m_strDesc = _T("");
	m_iDuration = 5000;		// msec
	m_nTimerID = 0;
	
	m_bStart = false;
	m_bWriteFile = true;
	m_bWriteDB = false;
	m_strWriteFilePath = _T("");
	m_dCalibTemp = 0;
	m_dCalibHumid = 0;
}

DeviceList::~DeviceList()
{
}


// DeviceList メンバ関数
