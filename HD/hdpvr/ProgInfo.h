#pragma once

// CProgInfo �R�}���h �^�[�Q�b�g

class CProgInfo : public CObject
{
public:
	CProgInfo();
	virtual ~CProgInfo();

// Attribute
	CString		m_strTitle;		// �ԑg��
	CString		m_strDesc;		// �ԑg���e
	CString		m_strCategory;	// �ԑg�J�e�S��
	CString		m_strStation;	// �����ǖ�
	//CString		m_strStartYear;	// �J�n����
	//CString		m_strStartMonth;	// �J�n����
	//CString		m_strStartDay;	// �J�n����
	//CString		m_strStartHour;	// �J�n����
	//CString		m_strStartMinute;	// �J�n����
	//CString		m_strStartSecond;	// �J�n����
	//CString		m_strEndYear;	// �I������
	//CString		m_strEndMonth;	// �I������
	//CString		m_strEndDay;	// �I������
	//CString		m_strEndHour;	// �I������
	//CString		m_strEndMinute;	// �I������
	//CString		m_strEndSecond;	// �I������

	CTime	m_tiStart;
	CTime	m_tiEnd;

	int			m_iStat;		// �^��v�����
	int			m_iId;			// ���N�G�X�gID
};


