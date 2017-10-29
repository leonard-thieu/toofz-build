function Get-Project($projectName) {
    [Xml]$project = Get-Content ".\$projectName\$projectName.csproj"

    return $project
}

function Get-TargetFramework($project) {
    return $project.Project.PropertyGroup.TargetFramework
}

function Get-PackageVersion($projectName, $packageName) {
    $project = Get-Project $projectName
    $targetFramework = Get-TargetFramework $project

    # .NET Framework projects
    if ($targetFramework.StartsWith('v')) { 
        [Xml]$packagesConfig = Get-Content ".\$projectName\packages.config"
        
        return ($packagesConfig.packages.package | ? { $_.id -eq $packageName }).version   
    } 
    # .NET Standard/Core projects
    else {
        return ($project.Project.ItemGroup.PackageReference | ? { $_.Include -eq $packageName }).Version
    }
}

function Get-PackagePath($projectName, $packageName) {
    $version = Get-PackageVersion $projectName $packageName

    if ($version -eq $null) { throw "$packageName is not installed in '$projectName'." }

    return Resolve-Path ".\packages\$packageName.$version"
}

function Get-OutputPath($projectName) {
    $project = Get-Project $projectName
    $targetFramework = Get-TargetFramework $project

    # .NET Framework projects
    if ($targetFramework.StartsWith('v')) {
        return Resolve-Path ".\$projectName\bin\$configuration\$projectName.dll"
    } 
    # .NET Standard/Core projects
    else {
        return Resolve-Path ".\$projectName\bin\$configuration\$targetFramework\$projectName.dll"
    }
}