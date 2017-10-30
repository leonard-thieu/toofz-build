[CmdletBinding()]
param(
    [String]$Configuration = $Env:CONFIGURATION,
    [String]$Platform = $Env:PLATFORM,
    [String]$Project = $Env:PROJECT,
    [String]$MyGetApiKey = $Env:MYGET_API_KEY
)

if ($Env:APPVEYOR_REPO_TAG -ne 'true') {
    Write-Warning ('The environment variable "APPVEYOR_REPO_TAG" is not set to "true". ' +
                   'NuGet package has not been deployed.')
} else {
    if ($Configuration -eq $null) { $Configuration = 'Debug' }

    switch ($Platform) {
        'x86' { break }
        'x64' { break }
        default { $Platform = 'AnyCPU' }
    }

    Import-Module "$PSScriptRoot\toofz.Build.dll"

    $projectDir = Resolve-Path ".\$Project"
    $projectPath = Join-Path $projectDir "$Project.csproj" | Resolve-Path
    $projectObj = Get-Project $projectPath

    if ($projectObj.IsNetFramework) {
        nuget pack $projectPath `
            -Properties "Configuration=$Configuration;Platform=$Platform" `
            -Symbols `
            -Verbosity quiet
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

        [Xml]$nuspec = Get-Content ".\$Project\$Project.nuspec"
        $id = $nuspec.package.metadata.id
        $version = $nuspec.package.metadata.version

        $package = Resolve-Path ".\$id.$version.nupkg"
        $symbols = Resolve-Path ".\$id.$version.symbols.nupkg"
    } else {
        msbuild /target:Pack "/property:Configuration=$Configuration;Platform=$Platform;IncludeSymbols=true;IncludeSource=true" /verbosity:quiet $projectPath

        $id = $Project
        $version = $projectObj.Version

        $outDir = [IO.Path]::Combine($projectDir, 'bin', $Configuration) | Resolve-Path
        $package = Join-Path $outDir ".\$id.$version.nupkg" | Resolve-Path
        $symbols = Join-Path $outDir ".\$id.$version.symbols.nupkg" | Resolve-Path
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