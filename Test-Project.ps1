[CmdletBinding()]
param(
    [String]$Project = $env:PROJECT,
    [String]$Filter,
    [Switch]$AsLocalSystem
)

if ($Project -eq '') { throw 'The environment variable "PROJECT" or the parameter "Project" is not set. Tests have not been run.' }
Write-Debug "`$Project = $Project"
$testProject = "$Project.Tests"

$configuration = $env:CONFIGURATION
if ($configuration -eq $null) { $configuration = 'Debug' }

Import-Module "$PSScriptRoot\toofz.Build.dll"

$testProjectPath = Resolve-Path ".\$testProject\$testProject.csproj"
$projectObj = Get-Project $testProjectPath

$openCoverPath = $projectObj.GetPackageDirectory('OpenCover')
$openCover = Join-Path $openCoverPath '.\tools\OpenCover.Console.exe'
Write-Debug "OpenCover path = $openCover"

$cd = Get-Location

# Register profilers
$openCoverProfile_x86 = Join-Path $openCoverPath '.\tools\x86\OpenCover.Profiler.dll'
psexec -accepteula -nobanner -s -w $cd regsvr32 /s $openCoverProfile_x86 2>&1 | % { "$_" }
$openCoverProfile_x64 = Join-Path $openCoverPath '.\tools\x64\OpenCover.Profiler.dll'
psexec -accepteula -nobanner -s -w $cd regsvr32 /s $openCoverProfile_x64 2>&1 | % { "$_" }

if ($projectObj.IsNetFramework) {
    $vstest = 'vstest.console.exe'
} else {
    $vstest = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe'
}

$targetArgs = $projectObj.GetOutPath($configuration)
if ((Test-Path Env:\APPVEYOR) -and $projectObj.IsNetFramework) { $targetArgs += ' /logger:AppVeyor' }

$filter = "+[$Project*]* -[$testProject*]*";
if ($Filter -ne $null) { $filter += " $Filter" }

if ($AsLocalSystem.IsPresent) {
    # Copy environment variables to machine level so tools have access to them when running under LocalSystem
    Get-ChildItem Env: | % { [Environment]::SetEnvironmentVariable($_.Name, $_.Value, 'Machine') }

    psexec -accepteula -nobanner -s -w $cd `
        $openCover `
            "-target:$vstest" `
            "-targetargs:$targetArgs" `
            -returntargetcode `
            "-filter:$filter" `
            "-excludebyattribute:*.ExcludeFromCodeCoverage*" `
            2>&1 | % { "$_" }
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} else {
    & $openCover `
        "-target:$vstest" `
        "-targetargs:$targetArgs" `
        -returntargetcode `
        "-filter:$filter" `
        "-excludebyattribute:*.ExcludeFromCodeCoverage*"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}