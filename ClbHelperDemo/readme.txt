----  Memo ---------------------
required Microsoft.OneDrive.Sdk v1.x, instead of 2.x

操作：最初の1回はOneDriveにWeb UIで手動ログインが必要。

---- ToDo ---------------------
- ２回目以降の自動ログイン
- InkDocumentをきれいに
- MainPageコードをきれいに


-  GetSilentlyAuthenticatedMicrosoftAccountClient って何に使うの？
- 

https://stackoverflow.com/questions/39035054/onedrive-auto-login-after-initial-authorisation/39068176

https://msdn.microsoft.com/ja-jp/magazine/mt632271.aspx

public static Task<IOneDriveClient> GetAuthenticatedMicrosoftAccountClient(
	string appId, 
	string returnUrl, 
	string[] scopes, 
	IWebAuthenticationUi webAuthenticationUi, 
	CredentialCache credentialCache = null, 
	IHttpProvider httpProvider = null);

IWebAuthenticationUiの代わりに、IAuthenticationProvider authenticationProvider,を使えないか？

https://github.com/ginach/Simple-IAuthenticationProvider-sample-for-OneDrive-SDK

-------------------------
アプリの登録
https://apps.dev.microsoft.com/?mkt=ja-jp&referrer=https%3a%2f%2faccount.live.com#/appList

Login ID
wacomtestj@gmail.com

名前
OneDriveTestUwp

アプリケーション ID
3cb701ed-4d2c-49b2-b85c-b32774dea759

ネイティブアプリケーション
カスタム リダイレクト URI ：msal3cb701ed-4d2c-49b2-b85c-b32774dea759://auth

アプリケーション　シークレット
パスワード：venxeBVONY32-^ktZB539+]
