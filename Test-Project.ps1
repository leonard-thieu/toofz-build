[CmdletBinding()]
param(
    [String]$Project = $Env:PROJECT,
    [String]$Filter,
    [Switch]$AsLocalSystem
)

if ($Project -eq '') { throw 'The environment variable "PROJECT" or the parameter "Project" is not set. Tests have not been run.' }

Write-Verbose "`$Project = $Project"

$configuration = $Env:CONFIGURATION
if ($configuration -eq $null) { $configuration = 'Debug' }

[String]$targetArgs = Resolve-Path ".\$Project.Tests\bin\$configuration\$Project.Tests.dll"
if (Test-Path Env:\APPVEYOR) { $targetArgs += ' /logger:AppVeyor' }

$filter = "+[$Project*]* -[$Project.Tests*]*";
if ($Filter -ne $null) { $filter += " $Filter" }

[Xml]$packagesConfig = Get-Content "$Project.Tests\packages.config"
$version = ($packagesConfig.packages.package | ? { $_.id -eq 'OpenCover' }).version
if ($version -eq $null) { throw "OpenCover is not installed in '$Project.Tests'. Tests have not been run." }

$openCover = Resolve-Path "packages\OpenCover.$version\tools\OpenCover.Console.exe"
Write-Verbose $openCover

Get-ChildItem Env: | % { [Environment]::SetEnvironmentVariable($_.Name, $_.Value, 'Machine') }
$cd = Get-Location
$openCoverProfile_x86 = Resolve-Path "packages\OpenCover.$version\tools\x86\OpenCover.Profiler.dll"
psexec -accepteula -nobanner -s -w $cd regsvr32 /s $openCoverProfile_x86 2>&1 | % { "$_" }
$openCoverProfile_x64 = Resolve-Path "packages\OpenCover.$version\tools\x64\OpenCover.Profiler.dll"
psexec -accepteula -nobanner -s -w $cd regsvr32 /s $openCoverProfile_x64 2>&1 | % { "$_" }

if ($AsLocalSystem -eq $true) {
    psexec -accepteula -nobanner -s -w $cd `
        $openCover `
            -target:vstest.console.exe `
            "-targetargs:$targetArgs" `
            -returntargetcode `
            "-filter:$filter" `
            "-excludebyattribute:*.ExcludeFromCodeCoverage*;*.GeneratedCodeAttribute*" `
            2>&1 | % { "$_" }
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} else {
    & $openCover `
        -target:vstest.console.exe `
        "-targetargs:$targetArgs" `
        -returntargetcode `
        "-filter:$filter" `
        "-excludebyattribute:*.ExcludeFromCodeCoverage*;*.GeneratedCodeAttribute*"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}