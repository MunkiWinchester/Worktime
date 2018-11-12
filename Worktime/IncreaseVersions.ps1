function IncreaseVersion {
    param (
        [parameter(Mandatory = $true)]
        [string]
        $content,
        [parameter(Mandatory = $false)]
        [switch]
        $IsNuspec
    )
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

$nuspec = [System.Io.File]::ReadAllText('.\Worktime.nuspec');
$nuspec = IncreaseVersion $nuspec -IsNuspec;
[System.Io.File]::WriteAllText('.\Worktime.nuspec', $nuspec);

$assembly = [System.Io.File]::ReadAllText('.\Properties\AssemblyInfo.cs');
$assembly = IncreaseVersion $assembly;
[System.Io.File]::WriteAllText('.\Properties\AssemblyInfo.cs', $assembly);

Pop-Location;
