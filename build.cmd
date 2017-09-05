powershell .\script\clean.ps1 %cd%
powershell .\script\restore.ps1
powershell .\script\build.ps1
powershell .\script\test.ps1 %cd%
