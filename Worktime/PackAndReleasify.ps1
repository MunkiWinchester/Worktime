Param(
        [parameter(Mandatory = $false)]
        [switch]
        $IncreaseVersion
)

function IncreaseVersion(
        [parameter(Mandatory = $true)]
        [string]
        $content,
        [parameter(Mandatory = $false)]
        [switch]
        $IsNuspec
    )
{
    if($IsNuspec -eq $false)
    {
        $contentMatches = [Regex]::Matches($content, '\[assembly: Assembly.*Version\(\"\d+\.\d+\.\d+\"\)\]');
    }
    else
    {
        $contentMatches = [Regex]::Matches($content, '<version>\d+\.\d+\.\d<\/version>');
    }

    foreach($version in $contentMatches)
    {
        $versionString = $version.Value;
        $numbers = [regex]::Matches($versionString, '\d');
        $newVersionNumber = "";
        for ($i = 0; $i -lt $numbers.Count; $i++)
        {
            $number = $numbers[$i].Value;
            if($i -lt $numbers.Count - 1)
            {
                $newVersionNumber += $number + '.';
            }
            else
            {
                $number = [convert]::ToInt32($numbers[$numbers.Count - 1].Value) + 1;
                $newVersionNumber += $number;
            }
        }
        $newVersionString = [Regex]::Replace($versionString, "\d+\.\d+\.\d+", $newVersionNumber);
        $content = $content.Replace($versionString, $newVersionString);
    }

    return $content;
}


Set-StrictMode -version 2.0;
$ErrorActionPreference = "Stop";

Push-Location;
Set-Location $PSScriptRoot;

if($IncreaseVersion -eq $true)
{
    Write-Output 'Increasing version of "Worktime.nuspec"';
    $nuspec = [System.Io.File]::ReadAllText((Resolve-Path .\Worktime.nuspec));
    $nuspec = IncreaseVersion $nuspec -IsNuspec;
    [System.Io.File]::WriteAllText((Resolve-Path .\Worktime.nuspec), $nuspec);

    Write-Output 'Increasing version of "Properties\AssemblyInfo.cs"';
    $assembly = [System.Io.File]::ReadAllText((Resolve-Path .\Properties\AssemblyInfo.cs));
    $assembly = IncreaseVersion $assembly;
    [System.Io.File]::WriteAllText((Resolve-Path .\Properties\AssemblyInfo.cs), $assembly);
}

Invoke-Command -ScriptBlock { & "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe" ..\Worktime.sln /t:Build /p:Configuration=Release };

$nuget    = Get-ChildItem -Path ..\packages\ -Recurse -Filter NuGet.exe    | Select-Object -ExpandProperty FullName;
$squirrel = Get-ChildItem -Path ..\packages\ -Recurse -Filter Squirrel.exe | Select-Object -ExpandProperty FullName;

$OutDir = Join-Path $PSScriptRoot "Publish";
$Version = ([XML] (Get-Content .\Worktime.nuspec)).Package.Metadata.Version;

Invoke-Command -ScriptBlock { & $nuget pack .\WorkTime.nuspec -Version $Version -Properties Configuration=Release -OutputDirectory $OutDir };
Invoke-Command -ScriptBlock { & $squirrel --releasify "${OutDir}\WorkTime.${Version}.nupkg" };

Pop-Location;
