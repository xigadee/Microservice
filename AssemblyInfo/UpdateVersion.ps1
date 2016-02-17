param(
	[string]$version = $null, 
	[string]$subversion = $null
)
Write-Host "BUILD_BUILDNUMBER contents: $Env:BUILD_BUILDNUMBER"

IF([string]::IsNullOrWhitespace($version)){$version=$Env:BUILD_BUILDNUMBER}
Write-Host "Assembly Version:" $version;

IF(![string]::IsNullOrWhitespace($subversion)){$nugetversion=$version+"-"+$subversion}
Write-Host "Nuget Version:" $nugetversion;

(Get-Content AssemblyInfo\SharedAssemblyInfo.cs).replace('0.0.0.0', $version) | Set-Content AssemblyInfo\SharedAssemblyInfo.cs

Get-ChildItem -Path "..\*\*.nuspec" -Recurse | ForEach-Object -Process {Write-Host $_.name -foregroundcolor cyan}

Get-ChildItem -Path "..\*\*.nuspec" -Recurse | ForEach-Object -Process {
    (Get-Content $_) -Replace '$nugetversion$', $nugetversion | Set-Content $_
}
Write-Host "Over and out."