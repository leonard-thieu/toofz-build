[CmdletBinding()]
param(
    [String]$Configuration = $env:CONFIGURATION,
    [String]$Platform = $env:PLATFORM,
    [String]$Project = $env:PROJECT,
    [String]$MyGetApiKey = $env:MYGET_API_KEY
)

if ($env:APPVEYOR_REPO_TAG -ne 'true') {
    Write-Warning ('The environment variable "APPVEYOR_REPO_TAG" is not set to "true". ' +
                   'NuGet package has not been deployed.')
} else {
    if ($Configuration -eq '') { $Configuration = 'Debug' }

    switch ($Platform) {
        'x86' { break }
        'x64' { break }
        default { $Platform = 'AnyCPU' }
    }

    Import-Module "$PSScriptRoot\toofz.Build.dll"

    $projectDir = Resolve-Path ".\$Project"
    $projectPath = Resolve-Path "$projectDir\$Project.csproj"
    $projectObj = Get-Project $projectPath

    if (Test-Path ".\$Project\$Project.nuspec") {
        nuget pack $projectPath `
            -Properties "Configuration=$Configuration;Platform=$Platform" `
            -Symbols `
            -Verbosity quiet
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        [Xml]$nuspec = Resolve-Path ".\$Project\$Project.nuspec" | Get-Content
        $id = $nuspec.package.metadata.id
        $version = $nuspec.package.metadata.version

        $package = Resolve-Path ".\$id.$version.nupkg"
        $symbols = Resolve-Path ".\$id.$version.symbols.nupkg"
    } else {
        msbuild /target:Pack "/property:Configuration=$Configuration;Platform=$Platform;IncludeSymbols=true;IncludeSource=true" /verbosity:quiet $projectPath

        $id = $projectObj.PackageId
        $version = $projectObj.PackageVersion

        $outDir = Resolve-Path "$projectDir\bin\$Configuration"
        $package = Resolve-Path "$outDir\$id.$version.nupkg"
        $symbols = Resolve-Path "$outDir\$id.$version.symbols.nupkg"
    }
    
    nuget push $package `
        -Source https://www.myget.org/F/toofz/api/v2/package -ApiKey $MyGetApiKey `
        -SymbolSource https://www.myget.org/F/toofz/symbols/api/v2/package -SymbolApiKey $MyGetApiKey `
        -Verbosity quiet
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    if (Test-Path Env:\APPVEYOR) { 
        Push-AppveyorArtifact $package
        Push-AppveyorArtifact $symbols
    }
}
