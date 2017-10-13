[CmdletBinding()]
param(
    [String]$Project = $env:PROJECT,
    [String]$Filter,
    [Switch]$AsLocalSystem
)

if ($Project -eq '') { throw 'The environment variable "PROJECT" or the parameter "Project" is not set. Tests have not been run.' }

Write-Debug "`$Project = $Project"

$configuration = $env:CONFIGURATION
if ($configuration -eq $null) { $configuration = 'Debug' }

[String]$targetArgs = Resolve-Path ".\$Project.Tests\bin\$configuration\$Project.Tests.dll"
if (Test-Path Env:\APPVEYOR) { $targetArgs += ' /logger:AppVeyor' }

$filter = "+[$Project*]* -[$Project.Tests*]*";
if ($Filter -ne $null) { $filter += " $Filter" }

[Xml]$packagesConfig = Get-Content "$Project.Tests\packages.config"
$version = ($packagesConfig.packages.package | ? { $_.id -eq 'OpenCover' }).version
if ($version -eq $null) { throw "OpenCover is not installed in '$Project.Tests'. Tests have not been run." }

$openCover = Resolve-Path "packages\OpenCover.$version\tools\OpenCover.Console.exe"
Write-Debug "OpenCover path = $openCover"

$cd = Get-Location

# Register profilers
$openCoverProfile_x86 = Resolve-Path "packages\OpenCover.$version\tools\x86\OpenCover.Profiler.dll"
psexec -accepteula -nobanner -s -w $cd regsvr32 /s $openCoverProfile_x86 2>&1 | % { "$_" }
$openCoverProfile_x64 = Resolve-Path "packages\OpenCover.$version\tools\x64\OpenCover.Profiler.dll"
psexec -accepteula -nobanner -s -w $cd regsvr32 /s $openCoverProfile_x64 2>&1 | % { "$_" }

if ($AsLocalSystem.IsPresent) {
    # Copy environment variables to machine level so tools have access to them when running under LocalSystem
    Get-ChildItem Env: | % { [Environment]::SetEnvironmentVariable($_.Name, $_.Value, 'Machine') }

    psexec -accepteula -nobanner -s -w $cd `
        $openCover `
            -target:vstest.console.exe `
            "-targetargs:$targetArgs" `
            -returntargetcode `
            "-filter:$filter" `
            "-excludebyattribute:*.ExcludeFromCodeCoverage*" `
            2>&1 | % { "$_" }
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} else {
    & $openCover `
        -target:vstest.console.exe `
        "-targetargs:$targetArgs" `
        -returntargetcode `
        "-filter:$filter" `
        "-excludebyattribute:*.ExcludeFromCodeCoverage*"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}