[CmdletBinding()]
param(
    [String]$Project = $Env:PROJECT
)

if ($Project -eq '') { throw 'The environment variable "PROJECT" or the parameter "Project" is not set. Code coverage results have not been submitted.' }
Write-Debug "`$Project = $Project"
$testProject = "$Project.Tests"

Import-Module .\toofz.Build.dll

$project = Get-Project ".\$testProject\$testProject.csproj"

$codecovPath = $project.GetPackageDirectory('Codecov')
$codecov = Join-Path $codecovPath '.\tools\codecov.exe'
Write-Debug "Codecov path = $codecov"

& $codecov "--file=results.xml"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }