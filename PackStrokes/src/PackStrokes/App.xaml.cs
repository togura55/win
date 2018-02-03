using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PackStrokes
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        static readonly string license = "eyJhbGciOiJSUzUxMiJ9.eyJpc3MiOiJsbXMiLCJhdWQiOiJ3ZXMiLCJleHAiOjE1Nzc4MzY4MDAsImlhdCI6MTQ5MzkwMzM1NCwic3ViIjoiMCIsInJpZ2h0cyI6WyJDRExfQUNDRVNTIiwiQ0RMX0xJVkVfU1RSRUFNSU5HIl0sImdpdmVuX25hbWUiOiJXYWNvbSIsImZhbWlseV9uYW1lIjoiV2Fjb20iLCJlbWFpbCI6IndpbGxkZXZlbG9wZXJzQHdhY29tLmNvbSIsInR5cGUiOiJldmFsIiwibGljX25hbWUiOiJXaWxsRGV2aWNlc0RlbW9BcHAgbGljZW5zZSIsImxpY191aWQiOiI4ODFiM2UxYTNiOGZiNTBmN2Y3ODY3NTc1NmQ1MmU3MGNkNTdmNmFjY2UyMzIyMzFiYWY0MDg3MjQ1YTMzYzQ2IiwiYXBwc193aW5kb3dzIjpbImM4MDE0YWNjLTRlZDUtNDZkMS1hMjFkLWUyNmUzOWJjNDU1YSJdfQ.FYT2OdHHlUJAOHoUFqm65OYc4R8gbZ1gs5eTMZRDjyculv_xbkwjDg1zy_sv-GxKHZqzCHVgXZagTe5IVC1VC_f3k0MPGxbpUf4jltGq7JtQJC153GBf0H3aQ-e4bzWv8_2Uuuf4s1zEcdSzPVx1Moh5sy_zcJvfPkToa6H-ZpWcZ5AxNLoaDoo9n1Daq5rTEh2hLzsgMtBIddrRCMpgMkokKj9JI3DleUSPxacjLYa4O5CqrUYcSN9UA9KCz6iqA2j5_esbGWoGHWPzTLwKPTWzjqTlA2s97JqUaKGD1tIcywJsOQaMB6KBHG4kDRjtYp8qT_6RyOVvw_ihi8xtzQ";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Wacom.Licensing.LicenseManager.Instance.SetLicense(license);

            //this.InitializeComponent();
            //this.OnSuspending += OnSuspending;
        }
    }
}
