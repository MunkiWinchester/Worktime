Set-StrictMode -version 2.0;
$ErrorActionPreference = "Stop";

Push-Location;
Set-Location $PSScriptRoot;

.\IncreaseVersions.ps1;

Invoke-Command -ScriptBlock { & "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe" ..\Worktime.sln /t:Build /p:Configuration=Release };

$nuget    = Get-ChildItem -Path ..\packages\ -Recurse -Filter NuGet.exe    | Select-Object -ExpandProperty FullName;
$squirrel = Get-ChildItem -Path ..\packages\ -Recurse -Filter Squirrel.exe | Select-Object -ExpandProperty FullName;

$OutDir = Join-Path $PSScriptRoot "Publish";
$Version = ([XML] (Get-Content .\Worktime.nuspec)).Package.Metadata.Version;

Invoke-Command -ScriptBlock { & $nuget pack .\WorkTime.nuspec -Version $Version -Properties Configuration=Release -OutputDirectory $OutDir };
Invoke-Command -ScriptBlock { & $squirrel --releasify "${OutDir}\WorkTime.${Version}.nupkg" };

Pop-Location;
