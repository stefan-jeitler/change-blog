function Create-LatestChanges
{
    param (
        [Parameter(Position = 0, Mandatory = $true)]
        [string]$Version,
        [Parameter(Position = 1, Mandatory = $true)]
        [string]$OutputDir
    )

    New-Item -ItemType Directory -Force -Path $OutputDir
    Write-Host "latest changes artifact created."

    $latestChanges = Get-Content -Raw -Path ./latest-changes.json | ConvertFrom-Json -AsHashtable
    $latestChanges.Add("version", $Version)

    $latestChanges `
    | ConvertTo-Json -Depth 100 `
    | Out-File (Join-Path $OutputDir "latest-changes.json")

    Write-Host "Latest changes created in $OutputDir"
}

function Publish-LatestChanges
{
    param (
        [Parameter(Mandatory = $true)]
        [string]$BaseUrl,
        [Parameter(Mandatory = $true)]
        [string]$ProductId,
        [Parameter(Mandatory = $true)]
        [string]$ApiKey
    )

    $latestChanges = Get-Content -Raw -Path ./latest-changes.json

    Write-Host "latest changes:"
    Write-Host $latestChanges

    $endpoint = "$($BaseUrl.TrimEnd("/") )/api/v1/products/$ProductId/versions"

    $parameters = @{
        Method = "POST"
        Uri = $endpoint
        Headers = @{ "X-API-KEY" = $ApiKey }
        ContentType = "application/json"
        Body = $latestChanges
    }

    try
    {
        Invoke-RestMethod @parameters
        Write-Host "LatestChanges successfully published."
    }
    catch
    {
        Write-Host "StatusCode:" $_.Exception.Response.StatusCode.value__

        EXIT -1
    }
}
