    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [string]$Dockerfile,
        [Parameter(Mandatory = $true)]
        [string]$ImageName,
        [string]$Version = "0.0.0",
        [string]$Tag = "latest",
        [switch]$PushToRegistry,
        [string]$RegistryUrl,
        [string]$RegistryUsername,
        [string]$RegistryPwd
    )

    $semVersionRegExPattern = '^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$'
    if ($Version -NotMatch $semVersionRegExPattern)
    {
        Write-Error "$Version is not a valid version."
        Exit -1
    }

    $SourceDir = (Get-Item $Dockerfile).Directory.Parent.FullName
    Write-Host "SourceDir: $SourceDir"

    $LocalImageName = "$( $ImageName ):$Tag"

    docker build -t $LocalImageName -f $Dockerfile --build-arg "Version=$version" $SourceDir

    if ($LastExitCode -ne 0)
    {
        Exit $LastExitCode
    }

    if (-Not$PushToRegistry)
    {
        Exit 0
    }

    Write-Host "Registry login: $RegistryUrl"
    $RegistryPwd | docker login $RegistryUrl --username $RegistryUsername --password-stdin

    if ($LastExitCode -ne 0)
    {
        Write-Error "Registry login failed"
        Exit $LastExitCode
    }

    $ImageName = "$RegistryUrl/$LocalImageName"

    docker tag $LocalImageName $ImageName
    docker push $ImageName

    Exit $LASTEXITCODE
