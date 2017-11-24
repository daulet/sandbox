param(
    [string]$rootPath
)

$args = ""
$uniqueTests = New-Object System.Collections.Generic.HashSet[string]
$allTests = Get-ChildItem $rootPath -Recurse -Filter "*Tests.dll"
foreach ($assembly in $allTests) {
    if ($uniqueTests.Add($assembly.Name)) {
        $args += " $($assembly.FullName)"
    }
}
$args += " -parallel all"

$startInfo = New-Object System.Diagnostics.ProcessStartInfo
$startInfo.FileName = "$rootPath\packages\xunit.runner.console\2.3.1\tools\xunit.console.exe"
$startInfo.Arguments = $args
$startInfo.UseShellExecute = $false
$startInfo.WorkingDirectory = Get-Location
$startInfo.CreateNoWindow = $false

$process = New-Object System.Diagnostics.Process
$process.StartInfo = $startInfo
$process.Start() | Out-Null

$finished = $false
try {
    while (-not $process.WaitForExit(100)) { 
        # Non-blocking loop done to allow ctr-c interrupts
    }

    $finished = $true
    if ($process.ExitCode -ne 0) { 
        throw "Command failed to execute: $($startInfo.FileName) $($startInfo.Arguments)" 
    }
}
finally {
    # If we didn't finish then an error occured or the user hit ctrl-c.
    # Either way kill the process.
    if (-not $finished) {
        $process.Kill()
    }
}
