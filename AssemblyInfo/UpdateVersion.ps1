Write-Host "BUILD_BUILDNUMBER contents: $Env:BUILD_BUILDNUMBER"
(Get-Content AssemblyInfo\SharedAssemblyInfo.cs).replace('0.0.0.0', $Env:BUILD_BUILDNUMBER) | Set-Content AssemblyInfo\SharedAssemblyInfo.cs
Write-Host "Over and out."