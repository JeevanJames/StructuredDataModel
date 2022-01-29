[CmdletBinding()]
Param ([string] $Version)

Function CheckExitCode()
{
    Param ([string] $ErrorMessage)
    if ($LastExitCode -ne "0")
    {
        Write-Host -ForegroundColor Red $ErrorMessage
        exit 1
    }
}

dotnet clean -c Release
CheckExitCode ".NET clean failed."

dotnet build -c Release
CheckExitCode ".NET clean failed."

Function Publish()
{
    param ([string] $Name, [string] $PackageId)

    Write-Host -ForegroundColor Magenta "Packing and publishing $Name package"

    dotnet pack ./src/$Name/$Name.csproj --include-symbols --include-source -c Release --no-build /p:Version=$Version
    CheckExitCode "Packing package $Name failed."

    dotnet nuget push ./src/$Name/bin/Release/$PackageId.$Version.nupkg -s https://api.nuget.org/v3/index.json
    CheckExitCode "Pushing package $Name failed"
}

Publish "Core" "NStructuredDataModel"
Publish "Json" "NStructuredDataModel.Json"
Publish "KeyValuePairs" "NStructuredDataModel.KeyValuePairs"
Publish "Xml" "NStructuredDataModel.Xml"
Publish "Yaml" "NStructuredDataModel.Yaml"
