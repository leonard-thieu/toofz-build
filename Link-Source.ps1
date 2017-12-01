$Project = $Env:PROJECT

nuget install SourceLink -ExcludeVersion -SolutionDirectory . -Verbosity quiet
$sourceLink = Resolve-Path ".\packages\SourceLink\tools\SourceLink.exe"
Write-Debug "SourceLink path = $sourceLink"

& $sourceLink index `
    --proj ".\$Project\$Project.csproj" `
    --proj-prop Configuration Release `
    --url "https://raw.githubusercontent.com/$env:APPVEYOR_REPO_NAME/{0}/%var2%"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }