[CmdletBinding()]
param
(
    [string[]] $TaskList = @('Build-Windows'),
    [hashtable] $Properties = @{}
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$rootDirectory = Split-Path -Parent $PSCommandPath
Set-Location -Path $rootDirectory

function Ensure-Psake
{
    if (-not (Get-Module -ListAvailable -Name psake))
    {
        Write-Host 'psake not found. Installing to current user scope...' -ForegroundColor Yellow
        Install-Module -Name psake -Scope CurrentUser -Force -AllowClobber -ErrorAction Stop | Out-Null
    }

    Import-Module -Name psake -MinimumVersion 4.9.0 -ErrorAction Stop
}

Ensure-Psake

Invoke-psake -buildFile (Join-Path -Path $rootDirectory -ChildPath 'psakefile.ps1') `
    -taskList $TaskList `
    -properties $Properties `
    -nologo

exit $LASTEXITCODE
