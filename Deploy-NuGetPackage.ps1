[CmdletBinding()]
param(
    [String]$RepoTag = $env:APPVEYOR_REPO_TAG,
    [String]$Configuration = $env:CONFIGURATION,
    [String]$Platform = $env:PLATFORM,
    [String]$Project = $env:PROJECT,
    [String]$MyGetApiKey = $env:MYGET_API_KEY
)

if ($RepoTag -ne 'true') {
    Write-Warning ('The environment variable "APPVEYOR_REPO_TAG" or the parameter "-RepoTag" is not set to "true". ' +
                   'NuGet package has not been deployed.')
} else {
    if ($Configuration -eq $null) { $Configuration = 'Debug' }

    switch ($Platform) {
        'x86' { break }
        'x64' { break }
        default { $Platform = 'AnyCPU' }
    }

    nuget pack "$Project\$Project.csproj" `
        -Properties "Configuration=$Configuration;Platform=$Platform" `
        -Symbols `
        -Verbosity quiet
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    [Xml]$nuspec = Get-Content -Path "$Project\$Project.nuspec"
    $id = $nuspec.package.metadata.id
    $version = $nuspec.package.metadata.version
    
    nuget push "$id.$version.nupkg" `
        -Source https://www.myget.org/F/toofz/api/v2/package -ApiKey $MyGetApiKey `
        -SymbolSource https://www.myget.org/F/toofz/symbols/api/v2/package -SymbolApiKey $MyGetApiKey `
        -Verbosity quiet
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Push-AppveyorArtifact "$id.$version.nupkg"
    Push-AppveyorArtifact "$id.$version.symbols.nupkg"
}