if (-not(Test-Path Env:\PROJECT)) { throw 'The environment variable "PROJECT" is not set. Tests will not be run.' }

$project = $env:PROJECT
$configuration = $env:CONFIGURATION
if ($configuration -eq $null) { $configuration = 'Debug' }

$targetArgs = ".\$project.Tests\bin\$configuration\$project.Tests.dll"
if (Test-Path Env:\APPVEYOR) { $targetArgs += ' /logger:AppVeyor' }

OpenCover.Console.exe `
    "-register:user" `
    "-target:vstest.console.exe" `
    "-targetargs:$targetArgs" `
    "-returntargetcode" `
    "-filter:+[$project*]* -[$project.Tests*]*" `
    "-excludebyattribute:*.ExcludeFromCodeCoverage*;*.GeneratedCodeAttribute*"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }