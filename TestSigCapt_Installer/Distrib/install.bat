@echo off

@echo Installing Signature SDK...
call msiexec /q /l*v log_sdk.txt /i Wacom-Signature-SDK-x64-4.5.1.msi

@echo Installing application program...
call msiexec /q /l*v log_app.txt /i setup.msi

@echo Completed