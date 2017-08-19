$appveyor_repo_tag = $env:APPVEYOR_REPO_TAG

if ($appveyor_repo_tag -ne 'true') {
    Write-Warning ('The environment variable "APPVEYOR_REPO_TAG" is not set to "true". ' +
                   'NuGet package has not been deployed.')
} else {
    $configuration = $env:CONFIGURATION
    $platform = $env:PLATFORM
    $appveyor_build_folder = $env:APPVEYOR_BUILD_FOLDER
    $project = $env:PROJECT

    if ($configuration -eq $null) { $configuration = 'Debug' }

    switch ($platform) {
        'x86' { break }
        'x64' { break }
        default { $platform = 'AnyCPU' }
    }

    nuget pack "$appveyor_build_folder\$project\$project.csproj" `
        -Properties "Configuration=$configuration;Platform=$platform" `
        -Verbosity quiet `
        -NonInteractive
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    [xml]$nuspec = Get-Content -Path "$appveyor_build_folder\$project\$project.nuspec"
    $id = $nuspec.package.metadata.id
    $version = $nuspec.package.metadata.version
    
    Push-AppveyorArtifact "$appveyor_build_folder\$id.$version.nupkg"
}