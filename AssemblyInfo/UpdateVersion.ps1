Write-Host "BUILD_BUILDNUMBER contents: $Env:BUILD_BUILDNUMBER"
(Get-Content SharedAssemblyInfo.cs).replace('0.0.0.0', $Env:BUILD_BUILDNUMBER) | Set-Content SharedAssemblyInfo.cs
Write-Host "Over and out."