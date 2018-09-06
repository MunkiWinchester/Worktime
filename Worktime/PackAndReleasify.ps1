Set-StrictMode -version 2.0;
$ErrorActionPreference = "Stop";

Push-Location;
Set-Location $PSScriptRoot;

$nuget    = Get-ChildItem -Path ..\packages\ -Recurse -Filter NuGet.exe    | Select-Object -ExpandProperty FullName;
$squirrel = Get-ChildItem -Path ..\packages\ -Recurse -Filter Squirrel.exe | Select-Object -ExpandProperty FullName;

$OutDir = Join-Path $PSScriptRoot "Publish";
$Version = ([XML] (Get-Content .\Worktime.nuspec)).Package.Metadata.Version;

Invoke-Command -ScriptBlock { & $nuget pack .\WorkTime.nuspec -Version $Version -Properties Configuration=Release -OutputDirectory $OutDir };
Invoke-Command -ScriptBlock { & $squirrel --releasify "${OutDir}\WorkTime.${Version}.nupkg" };

Pop-Location;
