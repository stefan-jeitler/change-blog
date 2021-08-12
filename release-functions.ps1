
function Create-ReleaseConfig {
  param (  
    [Parameter(Position = 0, Mandatory = $true)]
    [string]$Version,
    [Parameter(Position = 1, Mandatory = $true)]
    [string]$OutputDir,
    [Parameter(Position = 2, Mandatory = $true)]
    [string]$DbUpdaterDir
  )
  
  New-Item -ItemType Directory -Force -Path $OutputDir
  Copy-Item ./release-functions.ps1 $OutputDir
  Write-Host "'release-config' artifact created."

  $latestChanges = Get-Content -Raw -Path ./latest-changes.json | ConvertFrom-Json -AsHashtable
  $latestChanges.Add("version", $Version)

  $latestChanges | ConvertTo-Json -depth 100 | Out-File (Join-Path $OutputDir "latest-changes.json")
  Write-Host "Latest changes created in $OutputDir"


}

function Publish-LatestChanges {
  param (  
    [Parameter(Mandatory = $true)]
    [string]$BaseUrl,  
    [Parameter(Mandatory = $true)]
    [string]$ProductId,
    [Parameter(Mandatory = $true)]
    [string]$ApiKey
  )

  $latestChanges = Get-Content -Raw -Path ./latest-changes.json

  $endpoint = "$($BaseUrl.TrimEnd("/"))/api/v1/products/$ProductId/versions"

  $parameters = @{
    Method      = "POST"
    Uri         = $endpoint
    Headers     = @{"X-API-KEY" = $ApiKey } 
    ContentType = "application/json"
    Body        = $latestChanges
  }

  try {
    Invoke-RestMethod @parameters
    Write-Host "LatestChanges successfully published."
  }
  catch {
    Write-Host "StatusCode:" $_.Exception.Response.StatusCode.value__

    EXIT -1
  }
}
