$allTests = Get-ChildItem -Recurse -Filter "*Tests.dll"
foreach ($assembly in $allTests) {
    Remove-Item $assembly.FullName
}
