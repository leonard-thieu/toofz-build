[CmdletBinding()]
param(
    [String]$Project = $env:PROJECT,
    [String]$Configuration = $env:CONFIGURATION,
    [String]$SolutionDir = $env:APPVEYOR_BUILD_FOLDER,
    [String]$RepoName = $env:APPVEYOR_REPO_NAME
)

if ($Project -eq '') { throw 'The environment variable "PROJECT" or the parameter "Project" is not set. Tests have not been run.' }
if ($Configuration -eq '') { $Configuration = 'Debug' }
if ($SolutionDir -eq '') { $SolutionDir = Resolve-Path . }
if ($RepoName -eq '') { throw 'The environment variable "APPVEYOR_REPO_NAME" or the parameter "RepoName" is not set. Source linking has not run.' }

nuget install SourceLink -ExcludeVersion -SolutionDirectory $SolutionDir -Verbosity quiet
$sourceLink = Resolve-Path ".\packages\SourceLink\tools\SourceLink.exe"
Write-Debug "SourceLink path = $sourceLink"

& $sourceLink index `
    --proj ".\$Project\$Project.csproj" `
    --proj-prop Configuration $Configuration `
    --url "https://raw.githubusercontent.com/$RepoName/{0}/%var2%" `
    --repo $SolutionDir
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }