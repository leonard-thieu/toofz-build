[CmdletBinding()]
param(
    [String]$Project = $env:PROJECT,
    [String]$Filter
)

if ($Project -eq '') { throw 'The environment variable "PROJECT" or the parameter "Project" is not set. Tests have not been run.' }

Write-Verbose "`$Project = $Project"

$configuration = $env:CONFIGURATION
if ($configuration -eq $null) { $configuration = 'Debug' }

$targetArgs = ".\$Project.Tests\bin\$configuration\$Project.Tests.dll"
if (Test-Path Env:\APPVEYOR) { $targetArgs += ' /logger:AppVeyor' }

$filter = "-filter:+[$Project*]* -[$Project.Tests*]*";
if ($Filter -ne $null) { $filter += " $Filter" }

[Xml]$packagesConfig = Get-Content "$Project.Tests\packages.config"
$version = ($packagesConfig.packages.package | ? { $_.id -eq 'OpenCover' }).version
if ($version -eq $null) { throw "OpenCover is not installed in '$Project.Tests'. Tests have not been run." }

Write-Verbose "packages\OpenCover.$version\tools\OpenCover.Console.exe"

& "packages\OpenCover.$version\tools\OpenCover.Console.exe" `
    "-register:user" `
    "-target:vstest.console.exe" `
    "-targetargs:$targetArgs" `
    "-returntargetcode" `
    "$filter" `
    "-excludebyattribute:*.ExcludeFromCodeCoverage*;*.GeneratedCodeAttribute*"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }