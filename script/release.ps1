param(
    [string]$rootPath,
    [string]$version
)

Write-Host "Running build" -ForegroundColor Blue

dotnet msbuild $rootPath\src\echo\Echo.csproj '/consoleLoggerParameters:Summary;Verbosity=minimal' /m /t:Rebuild /nologo /p:TreatWarningsAsErrors=true /p:Configuration=Release

Write-Host "Publishing to Nuget" -ForegroundColor Blue

$apiKey = Get-Content "$rootPath\..\vault\keys\nuget\echo.apikey"
nuget push "$rootPath\src\echo\bin\Release\Echo.$version.nupkg" $apiKey -source https://api.nuget.org/v3/index.json
