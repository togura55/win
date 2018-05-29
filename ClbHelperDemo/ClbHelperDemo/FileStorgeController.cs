using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OneDrive.Sdk; // required Microsoft.OneDrive.Sdk v1.x, instead of 2.x
using System.Net.Http;
using Windows.UI.Core;
using Windows.UI.Xaml;
using System.Diagnostics;
using System.IO;


namespace ClbHelperDemo
{
    public class FileStorgeController
    {
        public class OneDrive
        {        
            // Client Apication Id.
            private string clientId = "3cb701ed-4d2c-49b2-b85c-b32774dea759"; // Application ID

            // Return url.
            private string returnUrl = "https://login.live.com/oauth20_desktop.srf";

            // Define the permission scopes.
            private static readonly string[] scopes = new string[] {
                "onedrive.readonly", "offline_access", "wl.signin"};
            //            "onedrive.readwrite", "offline_access", "wl.signin", "wl.basic" };

            // Client Secret
            private string clientSecret = "venxeBVONY32-^ktZB539+]";

            // Refresh Token
            private string refreshToken = string.Empty;

            public string dirId;

            // Create the OneDriveClient interface.
            public IOneDriveClient OneDriveClientAuth { get; set; }

            public async void LoginByWebAuthentication()
            {
                try
                {
                    // Setting up the client here, passing in our Client Id, Return Url, 
                    // Scopes that we want permission to, and building a Web Broker to 
                    // go do our authentication. 
                    OneDriveClientAuth = await OneDriveClient.GetAuthenticatedMicrosoftAccountClient(
                        clientId,
                        returnUrl,
                        scopes,
                        webAuthenticationUi: new WebAuthenticationBrokerWebAuthenticationUi());

                    if (!OneDriveClientAuth.IsAuthenticated)
                    {
                        AccountSession accountSession = await OneDriveClientAuth.AuthenticateAsync();

                        // Save accountSession.RefreshToken somewhere safe...
                        refreshToken = accountSession.RefreshToken;
                    }
                    else
                    {
                        refreshToken = OneDriveClientAuth.AuthenticationProvider.CurrentAccountSession.RefreshToken;
                    }
                }
                catch (OneDriveException exception)
                {
                    // Eating the authentication cancelled exceptions and resetting our client. 
                    if (!exception.IsMatch(OneDriveErrorCode.AuthenticationCancelled.ToString()))
                    {
                        if (exception.IsMatch(OneDriveErrorCode.AuthenticationFailure.ToString()))
                        {
//                            textBlock_Response.Text = "Authentication failed/cancelled, disposing of the client...";

                            ((OneDriveClient)OneDriveClientAuth).Dispose();
                            OneDriveClientAuth = null;
                        }
                        else
                        {
                            // Or we failed due to someother reason, let get that exception printed out.
//                            textBlock_Response.Text = exception.Error.ToString();
                        }
                    }
                    else
                    {
                        ((OneDriveClient)this.OneDriveClientAuth).Dispose();
                        this.OneDriveClientAuth = null;
                    }
                }
            }

            public async void LoginByAuthProvider()
            {
                try
                {
                    MicrosoftAccountServiceInfo serviceInfo = new MicrosoftAccountServiceInfo { };

                    serviceInfo.AppId = clientId; //something like: 00000000ABCDEFGH
                    serviceInfo.ClientSecret = clientSecret; //something like: 3vx[...]1sJ
                    serviceInfo.ReturnUrl = returnUrl; //something like: https://localhost/return
                    serviceInfo.Scopes = scopes; //something like new string[] { "wl.signin", "wl.offline_access", "onedrive.readonly" }

                    MicrosoftAccountAuthenticationProvider authenticationProvider = new MicrosoftAccountAuthenticationProvider(serviceInfo);

                    //                OneDriveClient OneDriveClient = await OneDriveClient.GetAuthenticatedMicrosoftAccountClient(
                    OneDriveClientAuth = await OneDriveClient.GetAuthenticatedMicrosoftAccountClient(
                        clientId,
                        returnUrl,
                        scopes,
                        authenticationProvider);

                    //more code

                    await OneDriveClientAuth.SignOutAsync();
                }
                catch (OneDriveException odex)
                {
                    throw odex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public class CustomAuthenticationProvider : IAuthenticationProvider
            {
                AccountSession IAuthenticationProvider.CurrentAccountSession { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

                Task IAuthenticationProvider.AppendAuthHeaderAsync(HttpRequestMessage request)
                {
                    throw new NotImplementedException();
                }

                Task<AccountSession> IAuthenticationProvider.AuthenticateAsync()
                {
                    throw new NotImplementedException();
                }

                Task IAuthenticationProvider.SignOutAsync()
                {
                    throw new NotImplementedException();
                }
            }

            public async void LoginBySilentlyAuth()
            {
                try
                {
                    AppConfig appConfig = new AppConfig
                    {
                        MicrosoftAccountAppId = clientId, //something like 00000000123456AB
                        MicrosoftAccountClientSecret = clientSecret, //something like 3vx[...]1sJ
                        MicrosoftAccountReturnUrl = returnUrl,
                        MicrosoftAccountScopes = scopes
                    };

                    // Create my own IAuthenticationProvider
                    var client = new OneDriveClient(appConfig,
                        serviceInfoProvider: new ServiceInfoProvider(new CustomAuthenticationProvider()));

                    // Use one of the various default authentication implementations. 
                    // Take a look at the SDK authentication documentation for the available options 
                    // and examples. 

                    //               var refreshtoken = (((MsaAuthenticationProvider)oneDriveClient.AuthenticationProvider).CurrentAccountSession).RefreshToken;
                    //                var refreshToken = ((client.AuthenticationProvider).CurrentAccountSession).RefreshToken;

                    // If you have a refresh token and only want to do the silent authentication flow 
                    // you can use OneDriveClient.GetSilentlyAuthenticatedMicrosoftAccountClient.
                    var client2 = await OneDriveClient.GetSilentlyAuthenticatedMicrosoftAccountClient(
                        clientId,
                        returnUrl,
                        scopes,
                        refreshToken);
                }
                catch (OneDriveException odex)
                {
                    throw odex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public async Task<string> GetFolderId(string dirName)
            {
                string id = null;
                try
                {
                    var dirRequest = id == null ? OneDriveClientAuth.Drive.Root :
                                     OneDriveClientAuth.Drive.Items[id];
                    var objs = await dirRequest.Children.Request().GetAsync();
                    foreach (var obj in objs)
                    {
                        if (obj.Folder != null)
                        {
                            if (dirName == obj.Name.ToString())
                            {
                                // bingo!
                                id = obj.Id;
                                break;
                            }
                        }
                        else
                        {
                        }
                    }

                }
                catch (OneDriveException odex)
                {
                    Debug.WriteLine(odex.Message);
                    throw odex;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw ex;
                }

                return id;
            }

            public async Task GetFileList(StoredInkFile storedInkFile, string directoryId = null)
            {
                try
                {
                    if (OneDriveClientAuth != null && OneDriveClientAuth.IsAuthenticated == true)
                    {
                        var drive = await OneDriveClientAuth.Drive.Request().GetAsync();

                        var dirRequest = directoryId == null ? OneDriveClientAuth.Drive.Root :
                                                         OneDriveClientAuth.Drive.Items[directoryId];
                        var objs = await dirRequest.Children.Request().GetAsync();
                        foreach (var obj in objs)
                        {
                            if (obj.Folder != null)
                            {
                                //                        ListBox_LogMessages.Items.Add("Folder: " + obj.Name.ToString());
                            }
                            else
                            {
                                if (!storedInkFile.IsExisted(obj))
                                {
                                    storedInkFile.Add(obj);

                                    // Do actions for new files in here
                                    //
                                }
                            }
                        }

                    }

                }
                catch (OneDriveException odex)
                {
                    Debug.WriteLine(odex.Message);
                    throw odex;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw ex;
                }
            }

            public string GetRefreshToken()
            {
                return refreshToken;
            }


            public async Task<bool> IsFileExisted(string FileName, string DirId)
            {
                var requestPath = DirId == null ? OneDriveClientAuth.Drive.Root :
                                                                OneDriveClientAuth.Drive.Items[DirId];

                var children = await requestPath.Children
                                                .Request()
                                                .GetAsync();
                return children.Any(item => item.Name == FileName);
            }


            public async Task<Stream> GetDownloadStreamAsync(string FileName, string DirId)
            {
                if (!(await IsFileExisted(FileName, DirId)))
                {
                    // 本来ならここで例外を投げたほうがいい
                    return new MemoryStream();
                }

                var requestPath = DirId == null ? OneDriveClientAuth.Drive.Root :
                                                         OneDriveClientAuth.Drive.Items[DirId];

                return await requestPath.ItemWithPath(FileName)
                                                        .Content
                                                        .Request()
                                                        .GetAsync();
            }
        }
    }
}
