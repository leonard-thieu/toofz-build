[CmdletBinding()]
param(
    [String]$Project = $env:PROJECT,
    [String]$Configuration = $env:CONFIGURATION,
    [String]$Filter
)

if ($Project -eq '') { throw 'The environment variable "PROJECT" or the parameter "Project" is not set. Tests have not been run.' }
Write-Debug "`$Project = $Project"
$testProject = "$Project.Tests"

if ($Configuration -eq '') { $Configuration = 'Debug' }

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
    $targetArgs += "$($testProjectObj.GetOutPath($Configuration)) "
} else {
    $target = Resolve-Path "$env:ProgramFiles\dotnet\dotnet.exe"
    $targetArgs += "test $testProjectPath "
}
$targetArgs = $targetArgs.Trim()

$testOutDir = Split-Path $testProjectObj.GetOutPath($Configuration)

$filterArg = "+[$Project*]* -[$testProject*]*"
if ($Filter -ne $null) { $filterArg += " $Filter" }

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