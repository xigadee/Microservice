Write-Host "BUILD_BUILDNUMBER contents: $Env:BUILD_BUILDNUMBER"
(Get-Content AssemblyInfo\SharedAssemblyInfo.cs).replace('0.0.0.0', $Env:BUILD_BUILDNUMBER) | Set-Content AssemblyInfo\SharedAssemblyInfo.cs
Get-ChildItem "**\*.nuspec" -Recurse | ForEach-Object -Process {
    (Get-Content $_) -Replace '{ps1.replace}', $Env:BUILD_BUILDNUMBER | Set-Content $_
}
Write-Host "Over and out."