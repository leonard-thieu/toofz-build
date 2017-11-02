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

nuget install OpenCover -ExcludeVersion -SolutionDirectory . -Verbosity quiet
$openCoverPath = Resolve-Path '.\packages\OpenCover'
$openCover = Resolve-Path "$openCoverPath\tools\OpenCover.Console.exe"
Write-Debug "OpenCover path = $openCover"

Import-Module "$PSScriptRoot\toofz.Build.dll"

$testProjectPath = Resolve-Path ".\$testProject\$testProject.csproj"
$testProjectObj = Get-Project $testProjectPath

$targetArgs = ''
if ($testProjectObj -is [toofz.Build.FrameworkProject]) {
    $target = 'vstest.console.exe'
    $targetArgs += "$($testProjectObj.GetOutPath($configuration)) "
    if (Test-Path Env:\APPVEYOR) { $targetArgs += '/logger:AppVeyor ' }
} else {
    $target = Resolve-Path "$env:ProgramFiles\dotnet\dotnet.exe"
    $targetArgs += "test $testProjectPath "
}
$targetArgs = $targetArgs.Trim()

$filterArg = "+[$Project*]* -[$testProject*]*"
if ($Filter -ne $null) { $filterArg += " $Filter" }

if ($AsLocalSystem.IsPresent) {
    # Copy environment variables to machine level so tools have access to them when running under LocalSystem
    Get-ChildItem Env: | % { [Environment]::SetEnvironmentVariable($_.Name, $_.Value, 'Machine') }

    $cd = Get-Location
    psexec -accepteula -nobanner -s -w $cd `
        $openCover `
            "-register:Path64" `
            "-target:$target" `
            "-targetargs:$targetArgs" `
            "-targetdir:$testOutDir" `
            "-returntargetcode" `
            "-filter:$filterArg" `
            "-excludebyattribute:*.ExcludeFromCodeCoverage*" `
            2>&1 | % { "$_" }
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} else {
    $testOutDir = Split-Path $testProjectObj.GetOutPath($configuration)
    & $openCover `
        "-register:user" `
        "-target:$target" `
        "-targetargs:$targetArgs" `
        "-targetdir:$testOutDir" `
        "-returntargetcode" `
        "-filter:$filterArg" `
        "-excludebyattribute:*.ExcludeFromCodeCoverage*" `
        "-oldstyle"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}