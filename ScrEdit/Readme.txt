


---- ToDo ------
- Open ScriptFile by Notepad
- Error detection at the script file parse phase
- Localize WiX installer UI
- Resource localization by using Passolo

----- Registry Entries Specification --------
SDLKK\ScrEdit
 UseDefaultScriptFile
 Type: dword
 Parameters: 1 = Use Default ScriptFile; 0 = Otherwise

SDLKK\ScrEdit
 ScriptFile
 Type: strings
 Parameters: strings of Script File full path

----- Build Environment --------------------
- Visual Studio 2008 (9.0.21022.8 RTM)
- WIX 3.0 (3.0.5419.0)
- Windows SDK v6.0A