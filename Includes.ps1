function Get-Project($projectName) {
    [Xml]$project = Get-Content ".\$projectName\$projectName.csproj"

    return $project
}

function Is-FrameworkVersion($version) {
    return $targetFramework.StartsWith('v')
}

function Get-TargetFramework($project) {
    return $project.Project.PropertyGroup.TargetFramework
}

function Get-PackageVersion($projectName, $packageName) {
    $project = Get-Project $projectName
    $targetFramework = Get-TargetFramework $project

    if (Is-FrameworkVersion $targetFramework) {
        [Xml]$packagesConfig = Get-Content ".\$projectName\packages.config"
        
        return ($packagesConfig.packages.package | ? { $_.id -eq $packageName }).version
    } else {
        return ($project.Project.ItemGroup.PackageReference | ? { $_.Include -eq $packageName }).Version
    }
}

function Get-PackagePath($projectName, $packageName) {
    $version = Get-PackageVersion $projectName $packageName

    if ($version -eq $null) { throw "$packageName is not installed in '$projectName'." }

    $project = Get-Project $projectName
    $targetFramework = Get-TargetFramework $project

    if (Is-FrameworkVersion $targetFramework) {
        $packageDirectory = "$packageName.$version"
    } else {
        $packageDirectory = $packageName.ToLower() + "\$version"
    }
    
    return Resolve-Path ".\packages\$packageDirectory"
}

function Get-OutputPath($projectName) {
    $project = Get-Project $projectName
    $targetFramework = Get-TargetFramework $project

    if (Is-FrameworkVersion $targetFramework) {
        return Resolve-Path ".\$projectName\bin\$configuration\$projectName.dll"
    } else {
        return Resolve-Path ".\$projectName\bin\$configuration\$targetFramework\$projectName.dll"
    }
}