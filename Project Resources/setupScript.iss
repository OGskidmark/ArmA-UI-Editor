; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "ArmA-UI-Editor"
#define MyAppVersion "0.4.6229.32099"
#define MyAppPublisher "X39"
#define MyAppURL "http://x39.io/projects?project=ArmA-UI-Editor"
#define MyAppExeName "ArmA UI Editor.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{336E80CF-8AA9-4276-8347-6FF2D9D668BD}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=setup
SetupIconFile=Logo_V3.ico
Compression=lzma
SolidCompression=yes
WizardSmallImageFile=Logo_V3.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]   
Source: "..\ArmA UI Editor\bin\Release\ArmA UI Editor.exe"; DestDir: "{app}"; Flags: ignoreversion    
Source: "..\ArmA UI Editor\bin\Release\ArmA UI Editor.exe.config"; DestDir: "{app}"; Flags: ignoreversion    
Source: "..\ArmA UI Editor\bin\Release\ArmAClassParser.dll"; DestDir: "{app}"; Flags: ignoreversion    
Source: "..\ArmA UI Editor\bin\Release\NLog.dll"; DestDir: "{app}"; Flags: ignoreversion    
Source: "..\ArmA UI Editor\bin\Release\Settings.xml"; DestDir: "{userappdata}\ArmA-UI-Editor\"; Flags: ignoreversion    
Source: "..\ArmA UI Editor\bin\Release\AddIns\*"; DestDir: "{app}\AddIns\"; Flags: ignoreversion recursesubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[UninstallDelete]
Type: filesandordirs; Name: "{userappdata}\ArmA-UI-Editor\"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

