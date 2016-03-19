#define AppName "ShareX.Wpf"
#define AppFilename "ShareX.exe"
#define AppParentDirectory "ShareX\bin\Debug"
#define AppPath AppParentDirectory + "\" + AppFilename
#dim Version[4]
#expr ParseVersion(AppPath, Version[0], Version[1], Version[2], Version[3])
#define AppVersion Str(Version[0]) + "." + Str(Version[1]) + "." + Str(Version[2])
#define AppPublisher "ShareX Team"
#define AppId "C4149C1B-9E99-4CFB-BB07-2FE30F0BEADD"

[Setup]
AppCopyright=Copyright (c) {#AppPublisher}
AppId={#AppId}
AppMutex={#AppId}
AppName={#AppName}
AppPublisher={#AppPublisher}
AppPublisherURL=https://github.com/McoreD
AppSupportURL=https://github.com/McoreD/ShareX.Wpf/issues
AppUpdatesURL=https://github.com/McoreD/ShareX.Wpf/releases
AppVerName={#AppName} {#AppVersion}
AppVersion={#AppVersion}
ArchitecturesAllowed=x86 x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64
DefaultDirName={pf}\{#AppName}
DefaultGroupName={#AppName}
DirExistsWarning=no
DisableProgramGroupPage=yes
;LicenseFile=..\LICENSE.txt
MinVersion=0,5.01.2600
OutputBaseFilename={#AppName}-{#AppVersion}-setup
OutputDir=Output\
PrivilegesRequired=none
ShowLanguageDialog=no
UninstallDisplayIcon={app}\{#AppFilename}
UninstallDisplayName={#AppName}
VersionInfoCompany={#AppPublisher}
VersionInfoTextVersion={#AppVersion}
;VersionInfoVersion={#AppVersion}
WizardImageBackColor=clWhite
;WizardImageFile=WizardImageFile.bmp
;WizardImageStretch=no
;WizardSmallImageFile=WizardSmallImageFile.bmp


[Tasks]
Name: "CreateDesktopIcon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"
Name: "CreateQuickLaunchIcon"; Description: "Create a quick launch shortcut"; GroupDescription: "Additional shortcuts:"; OnlyBelowVersion: 0,6.1

[Files]
Source: "{#AppParentDirectory}\{#AppFilename}"; DestDir: {app}; Flags: ignoreversion
Source: "{#AppParentDirectory}\*.dll"; DestDir: {app}; Flags: ignoreversion
Source: "{#AppParentDirectory}\Plugins\ShareX.UploadersLib.*.dll"; DestDir: "{userdocs}\{#AppName}\Uploaders"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppFilename}"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"; WorkingDir: "{app}"
Name: "{userdesktop}\{#AppName}"; Filename: "{app}\{#AppFilename}"; WorkingDir: "{app}"; Tasks: CreateDesktopIcon; Check: not DesktopIconExists
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#AppName}"; Filename: "{app}\{#AppFilename}"; WorkingDir: "{app}"; Tasks: CreateQuickLaunchIcon

[Run]
Filename: "{app}\{#AppFilename}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall

[Code]
function DesktopIconExists(): Boolean;
begin
  Result := FileExists(ExpandConstant('{userdesktop}\{#AppName}.lnk'));
end;