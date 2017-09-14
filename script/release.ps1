param(
    [string]$rootPath,
    [string]$version
)

Write-Host "Running build" -ForegroundColor Blue

msbuild.exe $rootPath\src\echo\Echo.csproj '/consoleLoggerParameters:Summary;Verbosity=minimal' /m /nodeReuse:false /nologo /p:TreatWarningsAsErrors=true /p:Configuration=Release

Write-Host "Publishing to Nuget" -ForegroundColor Blue

$apiKey = Get-Content "$rootPath\..\vault\keys\nuget\echo.apikey"
nuget push "$rootPath\src\echo\bin\Release\Echo.$version.nupkg" $apiKey -source https://api.nuget.org/v3/index.json
