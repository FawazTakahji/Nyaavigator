#define MyAppName "Nyaavigator"
#define MyAppVersion "1.1.0"
#define MyAppPublisher "FawazT"
#define MyAppURL "https://github.com/FawazTakhji/Nyaavigator"
#define MyAppExeName "Nyaavigator.exe"

[Setup]
AppMutex={#MyAppPublisher}:{#MyAppName},Global\{#MyAppPublisher}:{#MyAppName}
SetupMutex={#MyAppPublisher}:{#MyAppName}:Setup,GLobal\{#MyAppPublisher}:{#MyAppName}:Setup
AppId={{EED8DF1A-5B6C-4160-B852-2331B671CA4E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=Output
OutputBaseFilename={#MyAppName}_v{#MyAppVersion}_Installer
SetupIconFile=..\Nyaavigator\Assets\Icon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";

[Files]
Source: "Files\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "Files\av_libglesv2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Files\libHarfBuzzSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Files\libSkiaSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Files\Nyaavigator.pdb"; DestDir: "{app}"; Flags: ignoreversion

[InstallDelete]
; Type: files; Name: "{app}\filename.ext"

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  CurrentAppDataFolder: string;
  CurrentSettingsFile: string;
  UsersFolder: string;
  AppDataFolder: string;
  SettingsFile: string;
  FindRec: TFindRec;
begin
  if CurUninstallStep = usPostUninstall then
  begin
    CurrentAppDataFolder := ExpandConstant('{userappdata}\.Nyaavigator')
    CurrentSettingsFile := CurrentAppDataFolder + '\Settings.json';

    if DirExists(CurrentAppDataFolder) then
      if MsgBox('Do you want to delete the remaining app files?', mbConfirmation, MB_YESNO) = IDYES then
        begin
          DeleteFile(CurrentSettingsFile);
          RemoveDir(CurrentAppDataFolder);
        end;

    if IsAdminInstallMode() then
    begin
      if MsgBox('Do you want to delete the remaining app files for other users?', mbConfirmation, MB_YESNO) = IDYES then
      begin
        UsersFolder := ExtractFileDir(ExtractFileDir(ExpandConstant('{userdesktop}'))) // Assuming {userdesktop} leads to C:\Users\CurrentUser\Desktop
        if FindFirst(UsersFolder + '\*', FindRec) then
        begin
          try
            repeat
              if FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY <> 0 then
              begin
                AppDataFolder := UsersFolder + '\' + FindRec.Name  + '\AppData\Roaming\.Nyaavigator';
                if DirExists(AppDataFolder) then
                begin
                  SettingsFile := AppDataFolder + '\Settings.json';
                  DeleteFile(SettingsFile);
                  RemoveDir(AppDataFolder);
                end;
              end;
            until not FindNext(FindRec);
          finally
            FindClose(FindRec)
          end;
        end;
      end;
    end;
  end;
end;