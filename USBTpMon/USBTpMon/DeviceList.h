#pragma once

// DeviceList コマンド ターゲット


class DeviceList : public CObject
{
public:
	DeviceList();
	virtual ~DeviceList();

	// Implementation
	//  DLL I/F parameters
	BSTR		devName;
	CString		devFWVer;
	double		devTemp;
	double		devHumid;
	long		devSetHeater;
	CList<bool, bool&> 		devPortList;

	CString		m_strDesc;
	int			m_iDuration;
	UINT_PTR	m_nTimerID;

	bool	m_bStart;
	bool	m_bWriteFile;
	bool	m_bWriteDB;
	CString m_strWriteFilePath;
	double	m_dCalibTemp;
	double	m_dCalibHumid;
};


