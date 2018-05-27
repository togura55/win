��OneDrive SDK for CSharp

[�I[�r���h�X�e�[�^�X]�ihttps://ci.appveyor.com/api/projects/status/fs9ddrmdev37v012/branch/master?svg=true�j]�ihttps://ci.appveyor.com/project/OneDrive/onedrive�j -sdk-csharp / branch / master�j

[OneDrive API]�ihttps://dev.onedrive.com/README.htm�j��C���ɓ�������v���W�F�N�g�I

OneDrive SDK��Portable Class Library�Ƃ��č\�z����Ă���A�ȉ��̃t���[�����[�N��ΏۂƂ��Ă��܂��F
* .NET 4.5.1
* .NET for Windows Store�A�v���P�[�V����
* Windows Phone 8.1�ȍ~

Azure Active Directory�F�؂͎��̖ړI�Ŏg�p�ł��܂��B
* Windows Forms�A�v���P�[�V����
* UWP�A�v��
* Windows 8.1�̃A�v���P�[�V����


## Nuget�o�R�̃C���X�g�[��

NuGet�o�R��OneDrive SDK���C���X�g�[������ɂ�
* NuGet���C�u������ `Microsoft.OneDriveSDK`���������邩�A�܂���
*�p�b�P�[�W�}�l�[�W���R���\�[���� `Install-Package Microsoft.OneDriveSDK`�Ɠ��͂��܂��B


## ����

### 1.�A�v���P�[�V������o�^����
[����]�ihttps://dev.onedrive.com/app-registration.htm�j�̎菇�ɏ]���āAOneDrive�̃A�v���P�[�V������o�^���܂��B

### 2.�A�v���P�[�V����ID�ƃX�R�[�v�̐ݒ�
���[�U�[��OneDrive�ɃA�N�Z�X����ɂ́A�A�v�����A�N�Z�X����v������K�v������܂��B������s���ɂ́A�A�v��ID�ƃX�R�[�v�A�܂��͌������x�����w�肵�܂��B
�ڍׂɂ��ẮA[�F�؃X�R�[�v]�ihttps://dev.onedrive.com/auth/msa_oauth.htm#authentication-scopes�j���Q�Ƃ��Ă��������B

### 3.�F�؂��ꂽOneDriveClient�I�u�W�F�N�g���擾����
** OneDriveClient **�I�u�W�F�N�g���擾���āA�A�v���P�[�V�������T�[�r�X�Ƀ��N�G�X�g���s���K�v������܂����A�܂�Microsoft.Graph.Core�� `IAuthenticationProvider`����������I�u�W�F�N�g�̃C���X�^���X���K�v�ł��B
���̂悤�Ȏ����̗�́A[MSA Auth Adapter repository]�ihttps://github.com/OneDrive/onedrive-sdk-dotnet-msa-auth-adapter�j�ɂ���܂��B `IAuthenticationProvider`���쐬���A�F�؂��܂�
`AuthenticateUserAsync�i�j`���g�p���āA���ɔF�؃v���o�C�_���R���X�g���N�^�����Ƃ��Ďg�p���� `OneDriveClient`���쐬���܂��B�܂��A�A�v����ClientId�A�A�v���Ɏw�肵�����^�[��URL�AAPI�̃x�[�XURL�ȉ��́AOneDrive�T�[�r�X�ŔF�؂��邽�߂̃p�^�[���̃T���v���ł��B

`` `csharp
var msaAuthProvider = new myAuthProvider�i
    myClientId,
    "https://login.live.com/oauth20_desktop.srf",
    {"onedrive.readonly", "wl.signin"}�j;
await msaAuthProvider.AuthenticateUserAsync�i�j;
var oneDriveClient = new OneDriveClient�i "https://api.onedrive.com/v1.0", msaAuthProvider�j;
`` ``

���̌�A `oneDriveClient`�I�u�W�F�N�g���g���ăT�[�r�X���Ăяo�����Ƃ��ł��܂��B�ڍׂɂ��ẮA[OneDrive��C���A�v���̔F��]�idocs / auth.md�j���Q�Ƃ��Ă��������B


### 4.�T�[�r�X�ւ̃��N�G�X�g

OneDriveClient���F�؂����ƁA���̃T�[�r�X�ɑ΂��ČĂяo�����J�n�ł��܂��B�T�[�r�X�ɑ΂��郊�N�G�X�g�́AOneDrive��[REST API]�ihttps://dev.onedrive.com/README.htm�j�̂悤�Ɍ����܂��B

���[�U�[�̃h���C�u���擾����ɂ́F

`` `csharp
    var drive = await oneDriveClient
                          .Drive
                          .Request�i�j
                          .GetAsync�i�j;
`` ``

`GetAsync`�͐��������` Drive`�I�u�W�F�N�g��Ԃ��A�G���[�̏ꍇ�� `Microsoft.Graph.ServiceException`���X���[���܂��B

�h���C�u�̌��݂̃��[�U�[�̃��[�g�t�H���_���擾����ɂ́F

`` `csharp
    var rootItem = await oneDriveClient
                             .Drive
                             .Root
                             .Request�i�j
                             .GetAsync�i�j;
`` ``

`GetAsync`�͐��������` Item`�I�u�W�F�N�g��Ԃ��A�G���[�̏ꍇ�� `Microsoft.Graph.ServiceException`���X���[���܂��B

SDK�̐݌v���@�̊T�v�ɂ��ẮA[�T�v]�idocs / overview.md�j���Q�Ƃ��Ă��������B

���̃T���v���A�v���P�[�V���������p�ł��܂��B
* [OneDrive API�u���E�U]�ihttps://github.com/OneDrive/onedrive-ample-apibrowser-dotnet�j -  Windows�t�H�[���A�v���P�[�V����
* [OneDrive�t�H�g�u���E�U]�ihttps://github.com/OneDrive/onedrive-ample-photobrowser-uwp�j -  Windows Universal�A�v��
* [OneDrive Webhooks]�ihttps://github.com/OneDrive/onedrive-webhooks-aspnet�j -  ASP.NET MVC�A�v��

OneDrivePhotoBrowser�T���v���A�v���P�[�V���������s����ɂ́A[UWP app development]�ihttps://msdn.microsoft.com/en-us/library/windows/apps/dn609832.aspx�j�p�Ƀ}�V�����\�����A�v���W�F�N�g��Windows�X�g�A�Ɋ֘A�t����K�v������܂��B

##�h�L�������g�ƃ��\�[�X

* [�T�v]�idocs / overview.md�j
* [Auth]�idocs / auth.md�j
* [Items]�idocs / items.md�j
* [�`�����N�A�b�v���[�h]�idocs / chunked-uploads.md�j
* [�R���N�V����]�idocs / collections.md�j
* [�G���[]�idocs / errors.md�j
* [OneDrive API]�ihttp://dev.onedrive.com�j

##���

����\���܂��͋L�^����ɂ́A[issues]�ihttps://github.com/OneDrive/onedrive-sdk-csharp/issues�j���Q�Ƃ��Ă��������B

##���̑��̃��\�[�X

* NuGet�p�b�P�[�W�F[https://www.nuget.org/packages/Microsoft.OneDriveSDK](https://www.nuget.org/packages/Microsoft.OneDriveSDK�j


##���C�Z���X

[���C�Z���X]�iLICENSE.txt�j

���̃v���W�F�N�g�́A[Microsoft Open Source Code of Conduct]�ihttps://opensource.microsoft.com/codeofconduct/�j���̗p���܂����B�ڍׂɂ��ẮA[Code of Conduct FAQ]�ihttps://opensource.microsoft.com/codeofconduct/faq/�j�܂���[opencode@microsoft.com]�imailto�Fopencode@microsoft.com�j�܂ŃR�����g�܂��͂��₢���킹���������B