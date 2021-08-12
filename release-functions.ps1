
function Create-LatestChanges {
  param (  
    [Parameter(Position = 0, Mandatory = $true)]
    [string]$Version
  )
  
  $latestChanges = Get-Content -Raw -Path ./latest-changes.json | ConvertFrom-Json


  if ($latestChanges.Version -ne $Version) {
    Write-Error "Version mismatch: latest-changes.json version $($latestChanges.version), release version $Version"
    Exit -1
  }

  Write-Host "Latest changes version: $($latestChanges.Version)"
  Write-Host "Release version: $Version"
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
