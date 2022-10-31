#!/usr/bin/env pwsh
param(
	[string][Parameter(Mandatory = $true)] $DayName
)

if (test-path $DayName) {
	Write-Error "Project ${DayName} already exists"
	exit
}

New-Item -type directory -path $DayName | out-null
Copy-Item -Path _template/project/* -Destination $DayName -Recurse
Rename-Item -Path $(join-path $DayName "template.csproj") -NewName $($DayName + ".csproj")
dotnet sln add $DayName
