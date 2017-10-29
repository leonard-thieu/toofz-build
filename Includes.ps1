function Get-Project($projectName) {
    [Xml]$project = Get-Content ".\$projectName\$projectName.csproj"

    return $project
}

function Get-TargetFramework($project) {
    $targetFramework = $project.Project.PropertyGroup.TargetFramework

    # .NET Framework projects
    if ($targetFramework.StartsWith('v')) {
        return '.NET Framework'
    }
    # .NET Standard/Core projects
    else {
        return '.NET Core'
    }
}

function Get-PackageVersion($projectName, $packageName) {
    $project = Get-Project $projectName
    $targetFramework = Get-TargetFramework $project

    switch ($targetFramework) {
        '.NET Framework' {
            [Xml]$packagesConfig = Get-Content ".\$projectName\packages.config"
        
            return ($packagesConfig.packages.package | ? { $_.id -eq $packageName }).version   
        }
        '.NET Core' {
            return ($project.Project.ItemGroup.PackageReference | ? { $_.Include -eq $packageName }).Version
        }
    }
}

function Get-PackagePath($projectName, $packageName) {
    $version = Get-PackageVersion $projectName $packageName

    if ($version -eq $null) { throw "$packageName is not installed in '$projectName'." }

    $project = Get-Project $projectName
    $targetFramework = Get-TargetFramework $project

    switch ($targetFramework) {
        '.NET Framework' { $packageDirectory = "$packageName.$version" }
        '.NET Core' { $packageDirectory = $packageName.ToLower() + "\$version" }
    }
    
    return Resolve-Path ".\packages\$packageDirectory"
}

function Get-OutputPath($projectName) {
    $project = Get-Project $projectName
    $targetFramework = Get-TargetFramework $project

    switch ($targetFramework) {
        '.NET Framework' { return Resolve-Path ".\$projectName\bin\$configuration\$projectName.dll" }
        '.NET Core' { return Resolve-Path ".\$projectName\bin\$configuration\$targetFramework\$projectName.dll" }
    }
}