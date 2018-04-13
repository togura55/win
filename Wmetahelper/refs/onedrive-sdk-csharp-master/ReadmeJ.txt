＃OneDrive SDK for CSharp

[！[ビルドステータス]（https://ci.appveyor.com/api/projects/status/fs9ddrmdev37v012/branch/master?svg=true）]（https://ci.appveyor.com/project/OneDrive/onedrive） -sdk-csharp / branch / master）

[OneDrive API]（https://dev.onedrive.com/README.htm）をC＃に統合するプロジェクト！

OneDrive SDKはPortable Class Libraryとして構築されており、以下のフレームワークを対象としています：
* .NET 4.5.1
* .NET for Windows Storeアプリケーション
* Windows Phone 8.1以降

Azure Active Directory認証は次の目的で使用できます。
* Windows Formsアプリケーション
* UWPアプリ
* Windows 8.1のアプリケーション


## Nuget経由のインストール

NuGet経由でOneDrive SDKをインストールするには
* NuGetライブラリで `Microsoft.OneDriveSDK`を検索するか、または
*パッケージマネージャコンソールに `Install-Package Microsoft.OneDriveSDK`と入力します。


## 入門

### 1.アプリケーションを登録する
[次の]（https://dev.onedrive.com/app-registration.htm）の手順に従って、OneDriveのアプリケーションを登録します。

### 2.アプリケーションIDとスコープの設定
ユーザーのOneDriveにアクセスするには、アプリがアクセス権を要求する必要があります。これを行うには、アプリIDとスコープ、または権限レベルを指定します。
詳細については、[認証スコープ]（https://dev.onedrive.com/auth/msa_oauth.htm#authentication-scopes）を参照してください。

### 3.認証されたOneDriveClientオブジェクトを取得する
** OneDriveClient **オブジェクトを取得して、アプリケーションがサービスにリクエストを行う必要がありますが、まずMicrosoft.Graph.Coreに `IAuthenticationProvider`を実装するオブジェクトのインスタンスが必要です。
そのような実装の例は、[MSA Auth Adapter repository]（https://github.com/OneDrive/onedrive-sdk-dotnet-msa-auth-adapter）にあります。 `IAuthenticationProvider`を作成し、認証します
`AuthenticateUserAsync（）`を使用して、次に認証プロバイダをコンストラクタ引数として使用して `OneDriveClient`を作成します。また、アプリのClientId、アプリに指定したリターンURL、APIのベースURL以下は、OneDriveサービスで認証するためのパターンのサンプルです。

`` `csharp
var msaAuthProvider = new myAuthProvider（
    myClientId,
    "https://login.live.com/oauth20_desktop.srf",
    {"onedrive.readonly", "wl.signin"}）;
await msaAuthProvider.AuthenticateUserAsync（）;
var oneDriveClient = new OneDriveClient（ "https://api.onedrive.com/v1.0", msaAuthProvider）;
`` ``

その後、 `oneDriveClient`オブジェクトを使ってサービスを呼び出すことができます。詳細については、[OneDriveのC＃アプリの認証]（docs / auth.md）を参照してください。


### 4.サービスへのリクエスト

OneDriveClientが認証されると、そのサービスに対して呼び出しを開始できます。サービスに対するリクエストは、OneDriveの[REST API]（https://dev.onedrive.com/README.htm）のように見えます。

ユーザーのドライブを取得するには：

`` `csharp
    var drive = await oneDriveClient
                          .Drive
                          .Request（）
                          .GetAsync（）;
`` ``

`GetAsync`は成功すれば` Drive`オブジェクトを返し、エラーの場合は `Microsoft.Graph.ServiceException`をスローします。

ドライブの現在のユーザーのルートフォルダを取得するには：

`` `csharp
    var rootItem = await oneDriveClient
                             .Drive
                             .Root
                             .Request（）
                             .GetAsync（）;
`` ``

`GetAsync`は成功すると` Item`オブジェクトを返し、エラーの場合は `Microsoft.Graph.ServiceException`をスローします。

SDKの設計方法の概要については、[概要]（docs / overview.md）を参照してください。

次のサンプルアプリケーションも利用できます。
* [OneDrive APIブラウザ]（https://github.com/OneDrive/onedrive-ample-apibrowser-dotnet） -  Windowsフォームアプリケーション
* [OneDriveフォトブラウザ]（https://github.com/OneDrive/onedrive-ample-photobrowser-uwp） -  Windows Universalアプリ
* [OneDrive Webhooks]（https://github.com/OneDrive/onedrive-webhooks-aspnet） -  ASP.NET MVCアプリ

OneDrivePhotoBrowserサンプルアプリケーションを実行するには、[UWP app development]（https://msdn.microsoft.com/en-us/library/windows/apps/dn609832.aspx）用にマシンを構成し、プロジェクトをWindowsストアに関連付ける必要があります。

##ドキュメントとリソース

* [概要]（docs / overview.md）
* [Auth]（docs / auth.md）
* [Items]（docs / items.md）
* [チャンクアップロード]（docs / chunked-uploads.md）
* [コレクション]（docs / collections.md）
* [エラー]（docs / errors.md）
* [OneDrive API]（http://dev.onedrive.com）

##問題

問題を表示または記録するには、[issues]（https://github.com/OneDrive/onedrive-sdk-csharp/issues）を参照してください。

##その他のリソース

* NuGetパッケージ：[https://www.nuget.org/packages/Microsoft.OneDriveSDK](https://www.nuget.org/packages/Microsoft.OneDriveSDK）


##ライセンス

[ライセンス]（LICENSE.txt）

このプロジェクトは、[Microsoft Open Source Code of Conduct]（https://opensource.microsoft.com/codeofconduct/）を採用しました。詳細については、[Code of Conduct FAQ]（https://opensource.microsoft.com/codeofconduct/faq/）または[opencode@microsoft.com]（mailto：opencode@microsoft.com）までコメントまたはお問い合わせください。