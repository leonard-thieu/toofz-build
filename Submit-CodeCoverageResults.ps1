[CmdletBinding()]
param(
    [String]$Project = $env:PROJECT
)

if ($Project -eq '') { throw 'The environment variable "PROJECT" or the parameter "Project" is not set. Code coverage results have not been submitted.' }
Write-Debug "`$Project = $Project"
$testProject = "$Project.Tests"

nuget install Codecov -ExcludeVersion -SolutionDirectory . -Verbosity quiet
$codecovPath = Resolve-Path '.\packages\Codecov'
$codecov = Resolve-Path "$codecovPath\tools\codecov.exe"
Write-Debug "Codecov path = $codecov"

& $codecov "--file=results.xml"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }