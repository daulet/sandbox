param(
    [string]$rootPath
)

. $PsScriptRoot\clean.ps1 $rootPath

if ($LastExitCode -ne 0) {
    return $LastExitCode
}

. $PsScriptRoot\restore.ps1

if ($LastExitCode -ne 0) {
    return $LastExitCode
}

msbuild.exe .\Echo.sln '/consoleLoggerParameters:Summary;Verbosity=minimal' /m /nodeReuse:false /nologo /p:TreatWarningsAsErrors=true

if ($LastExitCode -ne 0) {
    return $LastExitCode
}

. $PsScriptRoot\test.ps1 $rootPath
