param(
    [string]$rootPath
)

Write-Host "Running clean" -ForegroundColor Blue

. $PsScriptRoot\clean.ps1 $rootPath

if ($LastExitCode -ne 0) {
    return $LastExitCode
}

Write-Host "Running restore" -ForegroundColor Blue

. $PsScriptRoot\restore.ps1

if ($LastExitCode -ne 0) {
    return $LastExitCode
}

Write-Host "Running build" -ForegroundColor Blue

msbuild.exe .\Echo.sln '/consoleLoggerParameters:Summary;Verbosity=minimal' /m /nodeReuse:false /nologo /p:TreatWarningsAsErrors=true

if ($LastExitCode -ne 0) {
    return $LastExitCode
}

Write-Host "Running test" -ForegroundColor Blue

. $PsScriptRoot\test.ps1 $rootPath
