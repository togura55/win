----  Memo ---------------------
required Microsoft.OneDrive.Sdk v1.x, instead of 2.x

����F�ŏ���1���OneDrive��Web UI�Ŏ蓮���O�C�����K�v�B

---- ToDo ---------------------
- �Q��ڈȍ~�̎������O�C��
- InkDocument�����ꂢ��
- MainPage�R�[�h�����ꂢ��


-  GetSilentlyAuthenticatedMicrosoftAccountClient ���ĉ��Ɏg���́H
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

IWebAuthenticationUi�̑���ɁAIAuthenticationProvider authenticationProvider,���g���Ȃ����H

https://github.com/ginach/Simple-IAuthenticationProvider-sample-for-OneDrive-SDK

-------------------------
�A�v���̓o�^
https://apps.dev.microsoft.com/?mkt=ja-jp&referrer=https%3a%2f%2faccount.live.com#/appList

Login ID
wacomtestj@gmail.com

���O
OneDriveTestUwp

�A�v���P�[�V���� ID
3cb701ed-4d2c-49b2-b85c-b32774dea759

�l�C�e�B�u�A�v���P�[�V����
�J�X�^�� ���_�C���N�g URI �Fmsal3cb701ed-4d2c-49b2-b85c-b32774dea759://auth

�A�v���P�[�V�����@�V�[�N���b�g
�p�X���[�h�FvenxeBVONY32-^ktZB539+]
