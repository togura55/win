// USBTpMonDlg.cpp : implementation file
//

#include "stdafx.h"
#include "USBTpMon.h"
#include "USBTpMonDlg.h"
#include "DeviceList.h"
#include "shlwapi.h"
#include "mapi.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif



// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

// Implementation
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CUSBTpMonDlg dialog




CUSBTpMonDlg::CUSBTpMonDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CUSBTpMonDlg::IDD, pParent)
	, m_xvAutoDetect(FALSE)
	, m_xvCalibTemp(0)
	, m_xvCalibHumid(0)
	, m_xvActValue(0)
	, m_xvActCondition(0)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CUSBTpMonDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_SENSOR, m_xcListDevice);
	DDX_Control(pDX, IDC_PBTN_START, m_xcStart);
	DDX_Control(pDX, IDC_PBTN_LED0, m_xcLed0);
	DDX_Control(pDX, IDC_PBTN_LED1, m_xcLed1);
	DDX_Control(pDX, IDC_PBTN_HEATER, m_xcHeater);
	DDX_Control(pDX, IDC_CB_AUTODETECT, m_xcAutoDetect);
	DDX_Check(pDX, IDC_CB_AUTODETECT, m_xvAutoDetect);
	DDX_Text(pDX, IDC_EDIT_CALIBTEMP, m_xvCalibTemp);
	DDX_Text(pDX, IDC_EDIT_CALIBHUMID, m_xvCalibHumid);
	DDX_Text(pDX, IDC_EDIT_ACTVALUE, m_xvActValue);
	DDX_Control(pDX, IDC_COMBO_ACTCONDITION, m_xcActCondition);
	DDX_CBIndex(pDX, IDC_COMBO_ACTCONDITION, m_xvActCondition);
	DDX_Control(pDX, IDC_LIST_ACTION, m_xcListAction);
}

BEGIN_MESSAGE_MAP(CUSBTpMonDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_PBTN_DETECTSENSOR, &CUSBTpMonDlg::OnBnClickedPbtnDetectsensor)
	ON_BN_CLICKED(IDC_PBTN_LED0, &CUSBTpMonDlg::OnBnClickedPbtnLed0)
	ON_BN_CLICKED(IDC_PBTN_LED1, &CUSBTpMonDlg::OnBnClickedPbtnLed1)
	ON_BN_CLICKED(IDC_PBTN_HEATER, &CUSBTpMonDlg::OnBnClickedPbtnHeater)
	ON_BN_CLICKED(IDC_PBTN_START, &CUSBTpMonDlg::OnBnClickedPbtnStart)
	ON_WM_TIMER()
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_CB_AUTODETECT, &CUSBTpMonDlg::OnBnClickedCbAutodetect)
	ON_CBN_SELCHANGE(IDC_COMBO_ACTCONDITION, &CUSBTpMonDlg::OnCbnSelchangeComboActcondition)
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_LIST_SENSOR, &CUSBTpMonDlg::OnLvnItemchangedListSensor)
	ON_NOTIFY(LVN_ENDLABELEDIT, IDC_LIST_SENSOR, &CUSBTpMonDlg::OnLvnEndlabeleditListSensor)
END_MESSAGE_MAP()



// CUSBTpMonDlg message handlers

BOOL CUSBTpMonDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon


	// Set GUI Strings
	{
		CString str, str2, str3;
		str.LoadStringA(IDS_PBTN_DETECTSENSOR);SetDlgItemText(IDC_PBTN_DETECTSENSOR, str);
		str.LoadStringA(IDS_TXT_DETECTSENSOR_NO);SetDlgItemText(IDC_TXT_DETECTSENSOR, str);
		str.LoadStringA(IDS_PBTN_LED0);SetDlgItemText(IDC_PBTN_LED0, str);
		str.LoadStringA(IDS_PBTN_LED1);SetDlgItemText(IDC_PBTN_LED1, str);
		str2.LoadStringA(IDS_TXT_HEATERSTAT);str3.LoadStringA(IDS_TXT_OFF);str.Format(str2, str3);SetDlgItemText(IDC_TXT_HEATERSTAT, str);
		str.LoadStringA(IDS_PBTN_HEATER);SetDlgItemText(IDC_PBTN_HEATER, str);
		str.LoadStringA(IDS_TXT_HEATERDESC);SetDlgItemText(IDC_TXT_HEATERDESC, str);
		str.LoadStringA(IDS_TXT_TEMPDEF);SetDlgItemText(IDC_TXT_TEMP, str);
		str.LoadStringA(IDS_TXT_HUMIDDEF);SetDlgItemText(IDC_TXT_HUMID, str);
		str.LoadStringA(IDS_CB_AUTODETECT);SetDlgItemText(IDC_CB_AUTODETECT, str);

		str.LoadStringA(IDS_PBTN_START);SetDlgItemText(IDC_PBTN_START, str);

		str.LoadStringA(IDS_TXT_CALIBTEMP);SetDlgItemText(IDC_TXT_CALIBTEMP, str);
		str.LoadStringA(IDS_TXT_CALIBHUMID);SetDlgItemText(IDC_TXT_CALIBHUMID, str);
		str.LoadStringA(IDS_TXT_ACTTEMP);SetDlgItemText(IDC_TXT_ACTTEMP, str);
		str.LoadStringA(IDS_TXT_ACTION);SetDlgItemText(IDC_TXT_ACTION, str);

	}
    {
        int        err = 0;

        if (!err) err = ListActionInit();        // リストコントロール初期化
        if (!err) err = ListActionInsertItem();  // リストアイテム挿入

        if (!err) err = ListDeviceInit();        // リストコントロール初期化
		
    }

	// Initialize UI controls
	UpdateControls(0);

	// Polling for detecting devices
	if (theApp.bEnablePol)
		theApp.nTimerID = SetTimer(-1, theApp.iPolDuration, NULL);

	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CUSBTpMonDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CUSBTpMonDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CUSBTpMonDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


// **************************************************************************
//
// AddSensor proc called from DetectSensor
//
void CUSBTpMonDlg::AddSensor(BSTR ret)
{
	CString str = (CString)(char*)ret, strDest;
	
	DeviceList	*devListNew = new DeviceList();

	devListNew->m_bStart = false;	// Detected but not started yet
	devListNew->devName = ret;		// Device name gotten from device

	int iStart = str.ReverseFind(_T('{')) + 1;		// Starting next to a char "{"
	strDest = str.Mid(iStart, str.GetLength()-iStart-1);	// End char might be "}"
	devListNew->m_strDesc = strDest;
	TRACE ("m_strDesc=%s\n", strDest);

	static bool bStat[1];
	for (int i=0; i<2; i++)
	{
		bStat[i] = false;
		devListNew->devPortList.AddTail(bStat[i]);
	}
	theApp.deviceList.AddTail(devListNew);
}

// **************************************************************************
//
// Detect sensor devices called from polling proc or PBTN events
//
bool CUSBTpMonDlg::DetectSensor(void)
{
	CString str, str2;
	BSTR	ret;
	int		iIndex = 0;
	bool	bSensor = false;

	while(1)
	{
		ret = theApp.pFindUSB(&iIndex);
		str = (CString)(char*)ret;

		if (str.IsEmpty())
			break;			// No devices anymore

		// A device was detected!
		else
		{
			if (theApp.deviceList.IsEmpty())
			{
				AddSensor(ret);
				bSensor = true;
			}
			else
			{
				POSITION pos = theApp.deviceList.GetHeadPosition();
				DeviceList *devList = (DeviceList *)theApp.deviceList.GetAt(pos);
				while(pos)
				{
					str2 = (CString)(char*)devList->devName;
					if(str.Compare(str2) != 0)
					{
						AddSensor(ret);
						bSensor = true;
					}
					devList = (DeviceList *)theApp.deviceList.GetNext(pos);
				}
			}
			TRACE("DetectSensor: ret=%s\n",  (CString)(char*)ret);
		}
	}

	// Devices are detected
	if (bSensor)
	{
		int iSelect = 0;	// Select Top of the List
		str.LoadStringA(IDS_TXT_DETECTSENSOR_YES);SetDlgItemText(IDC_TXT_DETECTSENSOR, str);
		UpdateListDevice(iSelect);
		UpdateControls (iSelect);	// Update all controls
	}

	return false;
}

// **************************************************************************
//
// DetectSensor Pbtn handler
//
void CUSBTpMonDlg::OnBnClickedPbtnDetectsensor()
{
	// For debug
//	SendMail();

	DetectSensor();
}

// **************************************************************************
//
// Start Pbtn handler
//
void CUSBTpMonDlg::OnBnClickedPbtnStart()
{


	int	iIndex = -1;
	bool bStart = false;
	DeviceList *devList = NULL;
	CString str;
	int nID = 0;

	iIndex = m_xcListDevice.GetNextItem(iIndex, LVNI_SELECTED);	// Get the current highlighted line

//	if (iIndex == -1){goto End;}


	POSITION pos = theApp.deviceList.FindIndex(iIndex);	// Set POS
	devList = (DeviceList*)theApp.deviceList.GetAt(pos);

	if (devList->m_bStart)
	{
		// Go stop
		// Uninstall Timer and stop sensing process
		if (KillTimer(devList->m_nTimerID))
		{
			TRACE ("OnBnClickedPbtnStart: KillTimer success: %d\n", devList->m_nTimerID);
		}
		else
		{
			TRACE ("OnBnClickedPbtnStart: KillTimer fail: %d\n", devList->m_nTimerID);
		}
		nID = IDS_PBTN_START;
	}
	else
	{
		// Go start
		if (devList->m_bWriteFile)
		{
			if (devList->m_strWriteFilePath.IsEmpty())
			{
				char szCurrentDir[_MAX_PATH];
				::GetCurrentDirectory(_MAX_PATH, szCurrentDir);
				str.LoadStringA(IDS_GEN_DATASUBDIRNAME);
				devList->m_strWriteFilePath.Format("%s\\%s",szCurrentDir,str);
			}

			if (!::PathIsDirectory(devList->m_strWriteFilePath))
			{
				if (::CreateDirectory(devList->m_strWriteFilePath,NULL))
				{
					TRACE("OnBnClickedPbtnStart: Create %s\n", 
						devList->m_strWriteFilePath);
				}
				else
				{
					devList->m_bWriteFile = false;
					TRACE("OnBnClickedPbtnStart: Cannot create %s\n", 
						devList->m_strWriteFilePath);
				}
			}
		}

		// Update Calibration value
		UpdateData();
		devList->m_dCalibTemp = m_xvCalibTemp;
		devList->m_dCalibHumid = m_xvCalibHumid;


		// Install Timer and start sensing process
		SetTimer(iIndex, devList->m_iDuration, NULL);
		devList->m_nTimerID = iIndex;
		TRACE ("OnBnClickedPbtnStart: SetTimer: %d\n", devList->m_nTimerID);

		nID = IDS_PBTN_STOP;
	}
	devList->m_bStart = !devList->m_bStart;
	str.LoadStringA(nID);SetDlgItemText(IDC_PBTN_START, str);	// UI Caption

	UpdateControls(iIndex);

}


// **************************************************************************
//
// Toggle LED state
//
void CUSBTpMonDlg::ToggleLED(int iPort)
{
	int iIndex = m_xcListDevice.GetItemCount();	// Get the current highlighted line
	//int	iIndex = m_xcListDevice.GetCurSel();	// Get the current highlighted line
	int iStat = 0, iRet = 0;
	DeviceList *deviceList = NULL;

	POSITION pos = theApp.deviceList.FindIndex(iIndex);	// Set POS
	deviceList = (DeviceList*)theApp.deviceList.GetAt(pos);

	pos = deviceList->devPortList.FindIndex(iPort);
	bool bStat = (bool)(deviceList->devPortList.GetAt(pos));
	bStat = !bStat;
	bStat ? iStat = 1 : iStat = 0;		// 1 = ON, 0 = OFF
	iRet = theApp.pControlIO(deviceList->devName, iPort, iStat);
	(bool)(deviceList->devPortList.GetAt(pos)) = bStat;
}

// **************************************************************************
//
// LED 0 Pbtn event handler
//
void CUSBTpMonDlg::OnBnClickedPbtnLed0()
{
	ToggleLED(0);
}

// **************************************************************************
//
// LED 1 Pbtn event handler
//
void CUSBTpMonDlg::OnBnClickedPbtnLed1()
{
	ToggleLED(1);
}

// **************************************************************************
//
// Set Heater Pbtn event handler
//
void CUSBTpMonDlg::OnBnClickedPbtnHeater()
{
	int iIndex = m_xcListDevice.GetItemCount();	// Get the current highlighted line
//	int	iIndex = m_xcListDevice.GetCurSel();	// Get the current highlighted line
	int iStat = 0, iRet = 0;
	DeviceList *deviceList = NULL;

	POSITION pos = theApp.deviceList.FindIndex(iIndex);	// Set POS
	deviceList = (DeviceList*)theApp.deviceList.GetAt(pos);

	// 0 = OFF, 1 = ON
	if(deviceList->devSetHeater == 0)
		deviceList->devSetHeater = 1;
	else
		deviceList->devSetHeater = 0;
	iRet = theApp.pSetHeater(deviceList->devName, deviceList->devSetHeater);
	TRACE("OnBnClickedPbtnHeater: pSetHeater iRet=%d\n", iRet);

	UpdateControls(iIndex);
}

// **************************************************************************
//
// Auto Detect Checkbox event handler
//
void CUSBTpMonDlg::OnBnClickedCbAutodetect()
{
    UpdateData();
    if (m_xvAutoDetect)
		theApp.nTimerID = SetTimer(-1, theApp.iPolDuration, NULL);
	else
		KillTimer(theApp.nTimerID);

	theApp.bEnablePol = !theApp.bEnablePol;
 }

// **************************************************************************
//
// Update ListBox of DeviceList view
//
bool CUSBTpMonDlg::UpdateListDevice(int iSelect)
{
	POSITION pos = theApp.deviceList.GetHeadPosition();
	CString	str;
	DeviceList	*devList = NULL;

	int        err = 0, lbErr = 0;

	// 既に表示されているリストをクリアする
	m_xcListDevice.DeleteAllItems();

	while(pos)
	{
		if (!err)
		{
			devList = (DeviceList *)theApp.deviceList.GetAt(pos);
			err = ListDeviceInsertItem(theApp.deviceList.GetCount(), devList->m_strDesc);
		}
		theApp.deviceList.GetNext(pos);	// if end of list, pos = NULL
	}

	if (!err)
	{
		m_xcListDevice.SetItemState(iSelect, LVIS_FOCUSED | LVIS_SELECTED,
			LVIS_FOCUSED | LVIS_SELECTED); // 指定行をハイライト表示
	}

	return TRUE;
}

// **************************************************************************
//
// Update GUI control state
//
bool CUSBTpMonDlg::UpdateControls(int iSelect)
{
	bool bEnable = true;

	if (theApp.deviceList.IsEmpty())
	{
		bEnable = false;
		m_xcStart.EnableWindow(bEnable);
	}
	else
	{
		DeviceList	*devList = NULL;
		POSITION pos = theApp.deviceList.FindIndex(iSelect);	// Set POS
		devList = (DeviceList*)theApp.deviceList.GetAt(pos);
		CString str,str2, str3;
		int	nID = 0;
		bEnable = true;

		m_xcStart.EnableWindow(bEnable);

		devList->m_bStart ? bEnable = true : bEnable = false;

		if(devList->devSetHeater == 0)		// Heater ON/OFF message
			nID = IDS_TXT_OFF;
		else
			nID = IDS_TXT_ON;
		str2.LoadStringA(IDS_TXT_HEATERSTAT);str3.LoadStringA(nID);str.Format(str2, str3);
		SetDlgItemText(IDC_TXT_HEATERSTAT, str);

		// Calibration Editbox
		m_xvCalibTemp = devList->m_dCalibTemp;
		m_xvCalibHumid = devList->m_dCalibHumid;
	}

	m_xcLed0.EnableWindow(bEnable);
	m_xcLed1.EnableWindow(bEnable);
	m_xcHeater.EnableWindow(bEnable);

	m_xvAutoDetect = theApp.bEnablePol;		// Enable Auto Detect checkbox

	// ActCondition pulldown
	{
        int        err = 0, cbErr = 0;
		CString str;

        if (!err)
        {	
			str.LoadStringA(IDS_COMBO_NONE);
            cbErr = m_xcActCondition.InsertString(-1, str);
            if (cbErr == CB_ERR || cbErr == CB_ERRSPACE) err = 1;
        }
        if (!err)
        {
			str.LoadStringA(IDS_COMBO_SMALL);
            cbErr = m_xcActCondition.InsertString(-1, str);
            if (cbErr == CB_ERR || cbErr == CB_ERRSPACE) err = 1;
        }
        if (!err)
        {
			str.LoadStringA(IDS_COMBO_LARGE);
            cbErr = m_xcActCondition.InsertString(-1, str);
            if (cbErr == CB_ERR || cbErr == CB_ERRSPACE) err = 1;
        }
        if (!err)
        {
            m_xvActCondition = 0;		// Def
        }
    }

	UpdateData(FALSE);

	return true;
}

// **************************************************************************
//
// Timer callback procedure
//
void CUSBTpMonDlg::OnTimer(UINT_PTR nIDEvent)
{

	if (nIDEvent == -1)
	{
		// Timer for polling devices
		DetectSensor();
	}
	else
	{
		// Timer for communication to each device
		// 温度と湿度の取得
		double temp = 0, humid = 0;
		int re = 0;
		DeviceList *devList = NULL;

		POSITION pos = theApp.deviceList.FindIndex(nIDEvent);
		devList = (DeviceList *)theApp.deviceList.GetAt(pos);

		re =  theApp.pGetTempHumidTrue(devList->devName, &temp, &humid);

		// re==0の時正常終了, re!=0だとエラー
		if(!re)
		{
			CString str, str2;
			
			// ダイアログに温度表示
			temp += devList->m_dCalibTemp;	// Calibration
			str2.LoadStringA(IDS_TXT_TEMP);
			str.Format(str2, temp);SetDlgItemText(IDC_TXT_TEMP, str);

			// ダイアログに湿度表示
			humid += devList->m_dCalibHumid;	// Calibration
			str2.LoadStringA(IDS_TXT_HUMID);
			str.Format(str2, humid);SetDlgItemText(IDC_TXT_HUMID, str);

			if (devList->m_bWriteFile)
			{
				CStdioFile    stdFile;
				CString       wstr, rstr, str, str2;
				LPTSTR        rstrBuf = NULL;
				int           err = 0;

				str.Format("%s\\%s", devList->m_strWriteFilePath, devList->m_strDesc);
				// (1)読み書き用にオープン
				if (!err)
				{
					if (!stdFile.Open(str, 
						CFile::modeReadWrite | 
						CFile::modeNoTruncate |
						CFile::shareExclusive | 
						CFile::modeCreate)) err = 1;
				}
				// (2)書き込み
				if (!err)
				{
					CTime t = CTime::GetCurrentTime();
					str.Format("%d;%d;%d;%d;%d;%d",
						t.GetYear(),t.GetMonth(),t.GetDay(),
						t.GetHour(),t.GetMinute(),t.GetSecond()); 

					wstr.Format("%s;%.2f;%.1f\n", str, temp, humid);
					TRACE("%s;%.2f;%.1f\n",str, temp, humid);

					stdFile.SeekToEnd();				// Add to the end
					TRY {stdFile.WriteString(wstr);}
					CATCH (CFileException, eP) {err = 1;}
					END_CATCH

					stdFile.Close();
				}
			}
		}
	}
	CDialog::OnTimer(nIDEvent);
}

// **************************************************************************
//
// WM_CLOSE message handler
//
void CUSBTpMonDlg::OnClose()
{
	if(theApp.nTimerID > 0)
		KillTimer(theApp.nTimerID);

	CDialog::OnClose();
}


void CUSBTpMonDlg::OnCbnSelchangeComboActcondition()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
}

int CUSBTpMonDlg::ListActionInit(void)
{
    int         err = 0;
    LVCOLUMN    lvc;
    int         i = 0;

    lvc.mask = LVCF_WIDTH | LVCF_SUBITEM;    // 有効フラグ
    lvc.iSubItem    = i;            // サブアイテム番号
    lvc.cx          = 100;          // 横幅
    if (m_xcListAction.InsertColumn(i, &lvc) == -1) {err = 1;}

    return err;
}

int CUSBTpMonDlg::ListActionInsertItem(void)
{
    int          err = 0;
    const int    itemNum = 3;
    LVITEM       lvi;
    CString      str;
    int          i, index = 0;
    //
    for (i = 0; i < itemNum; i++)
    {
        lvi.mask = LVIF_TEXT;
        if (!err)
        {
			str.LoadStringA(IDS_LIST_ACTEMAIL+i);
            lvi.iItem = i;
            lvi.iSubItem = 0;
            lvi.pszText =   (LPTSTR)(LPCTSTR)str;
            if ((index = m_xcListAction.InsertItem(&lvi)) == -1) err = 1;
        }

        if (err) break;
    }
    
    return err;
}

int CUSBTpMonDlg::ListDeviceInit(void)
{
    int         err = 0;
    LVCOLUMN    lvc;
    int         i = 0;

    lvc.mask = LVCF_WIDTH | LVCF_SUBITEM;    // 有効フラグ
    lvc.iSubItem    = i;            // サブアイテム番号
    lvc.cx          = 300;          // 横幅
    if (m_xcListDevice.InsertColumn(i, &lvc) == -1) {err = 1;}

    return err;
}

int CUSBTpMonDlg::ListDeviceInsertItem(int iItem, CString str)
{
    int          err = 0, index = 0;
    LVITEM       lvi;
    //
    lvi.mask = LVIF_TEXT;
    lvi.iItem = iItem;
    lvi.iSubItem = 0;
    lvi.pszText =   (LPTSTR)(LPCTSTR)str;
    if ((index = m_xcListDevice.InsertItem(&lvi)) == -1) err = 1;
    
    return err;
}

void CUSBTpMonDlg::OnLvnItemchangedListSensor(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	*pResult = 0;
}

// **************************************************************************
//
// Event handler called when editing ListDevice item was finished
//
void CUSBTpMonDlg::OnLvnEndlabeleditListSensor(NMHDR *pNMHDR, LRESULT *pResult)
{
	CString str;
	NMLVDISPINFO *pDispInfo = reinterpret_cast<NMLVDISPINFO*>(pNMHDR);

	int iSelect = pDispInfo->item.iItem;	// row position
	str.Format("%s", pDispInfo->item.pszText);
	if (!str.IsEmpty())
	{
		DeviceList	*devList = NULL;
		POSITION pos = theApp.deviceList.FindIndex(iSelect);	// Set POS
		devList = (DeviceList*)theApp.deviceList.GetAt(pos);
		devList->m_strDesc.Format("%s", pDispInfo->item.pszText);

		m_xcListDevice.SetItemText(iSelect, 0, pDispInfo->item.pszText);
	}
	else
	{
		str.LoadStringA(IDS_GEN_STREMPTYERR);
		AfxMessageBox(str, MB_ICONEXCLAMATION, 0);
	}
	*pResult = 0;
}



int CUSBTpMonDlg::SendMail(void)
{
  // 宛先設定
  MapiRecipDesc mrd;
  memset(&mrd, NULL, sizeof(mrd));
  mrd.ulRecipClass = MAPI_TO;
  mrd.lpszAddress = "togura@sdl.com";    

  // 添付ファイル情報設定
  MapiFileDesc mfd;
  memset(&mfd, NULL, sizeof(mfd));
  mfd.lpszPathName = "";               // 添付ファイルがあればファイル名を指定

  // メール情報設定
  MapiMessage mms;
  memset(&mms, NULL, sizeof(mms));
  mms.lpszSubject = "タイトル";        // メールのタイトル
  mms.lpszNoteText = "本文";           // メールの本文
  mms.nRecipCount = 1;                 // MapiRecipDesc構造体を設定した数
  mms.lpRecips = &mrd;                 // MapiRecipDesc構造体のポインタ
  mms.nFileCount = 0;          // MapiFileDesc構造体を設定した数
  mms.lpFiles = &mfd;               // MapiFileDesc構造体のポインタ

  // MAPIDLLロード
  HINSTANCE hDll;
  hDll = LoadLibrary("mapi32.dll");

  // DLL内の関数呼び出し
  if (hDll)
  {
    ULONG (_stdcall *SendMail)(LHANDLE, ULONG, lpMapiMessage, FLAGS, ULONG);
      (FARPROC&)SendMail = GetProcAddress(hDll, "MAPISendMail");

      // 実際のメール送信
      int ret = SendMail(0, 0, &mms, MAPI_LOGON_UI, 0);

    // 異常時のエラー処理
    if (ret != SUCCESS_SUCCESS)
    {
		CString str;
		str.Format("SendMail Error: %d", ret);
		MessageBox(str);
    }
  }

	return 0;
}
