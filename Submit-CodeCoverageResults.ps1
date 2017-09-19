[CmdletBinding()]
param(
    [String]$Project = $Env:PROJECT
)

if ($Project -eq '') { throw 'The environment variable "PROJECT" or the parameter "Project" is not set. Code coverage results have not been submitted.' }

Write-Verbose "`$Project = $Project"

[Xml]$packagesConfig = Get-Content "$Project.Tests\packages.config"
$version = ($packagesConfig.packages.package | ? { $_.id -eq 'Codecov' }).version
if ($version -eq $null) { throw "Codecov is not installed in '$Project.Tests'. Code coverage results have not been submitted." }

$codecov = Resolve-Path "packages\Codecov.$version\tools\codecov.exe"
Write-Verbose $codecov

& $codecov "--file=results.xml"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }