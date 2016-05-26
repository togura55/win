static private string sSMTPServer = "smtp.gmail.com";
static private int iSMTPServerPort = 587;
static private bool bSMTPAuth = true;
static private bool bSMTPSSL = true;
static private string sFrom = "test.sdljapan@gmail.com";
static private string sTo = "togura@sdl.com";       // colon separate for multiple sending
static private string sCc = "";
static private string sBcc = "";
static private string sPwd = "sdllab.2";
static private string sSubject = "";
static private string sBody = "This is a test mail";
static private string sSign = "";
static private string sFromDisplayName = "QARepPars Notifier";
static private bool bSimpleDesc = true;   // Show by simple description

static string sConfFile = "";       // 
static string sConfFilePath = "";   // Full path

static private string[] slistConf={ "SMTPServer=",
                                      "SMTPServerPort=",
                                      "SMTPAuth=",
                                      "SMTPSSL=",
                                      "From=",
                                      "FromDisplayName=",
                                      "To=",
                                      "Cc=",
                                      "Bcc=",
                                      "Pwd=",
                                      "Subject=",
                                      "Body=",
                                      "Sign="};

static private int SetConfig(string path)
{
    // Get PC name
    IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
    sPCName = hostInfo.HostName;

    try
    {
        // Config�t�@�C�����J���ăp�����[�^���Z�b�g����
        using (StreamReader sr = new StreamReader(
            path, Encoding.GetEncoding("Shift_JIS")))   // Closed in IDisposable
        {
            sConfFile = sr.ReadToEnd();
            string s;
            for (int i = 0; i < slistConf.Length; i++)
            {
                int startIndex = 0, endIndex = 0, iLineEnd = 0;
                if (FindKeyword(sConfFile, slistConf[i], ref startIndex, ref endIndex) == 0)
                {
                    iLineEnd = sConfFile.IndexOf(sCRLF, startIndex) - startIndex;
                    s = (sConfFile.Substring(startIndex, iLineEnd)).Substring(slistConf[i].Length, sConfFile.Substring(startIndex, iLineEnd).Length - slistConf[i].Length);

                    switch (i)
                    {
                        case 0:
                            sSMTPServer = s;
                            break;
                        case 1:
                            if (s.Length != 0)
                                iSMTPServerPort = int.Parse(s);
                            break;
                        case 2:
                            bSMTPAuth = (s == "yes" ? true : false);
                            break;
                        case 3:
                            bSMTPSSL = (s == "yes" ? true : false);
                            break;
                        case 4:
                            sFrom = s;
                            break;
                        case 5:
                            sFromDisplayName = s;
                            break;
                        case 6:
                            sTo = s;
                            break;
                        case 7:
                            sCc = s;
                            break;
                        case 8:
                            sBcc = s;
                            break;
                        case 9:
                            sPwd = s;
                            break;
                        case 10:
                            sSubject = s;
                            break;
                        case 11:
                            sBody = s;
                            break;
                        case 12:
                            sSign = s;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }
    catch (Exception e)
    {
        // �G���[����
        // �ڂ����́u��O�����v�Ő������܂��B
        // �t�@�C�������݂��Ȃ�������A�A�N�Z�X�������Ȃ��ꍇ�ɂ��������s�����B
        Console.Write(e.Message + sCRLF);
        return -1;
    }

    return 0;
}