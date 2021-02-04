# Packs the SDK locally, updates the Sample to use this package, and then builds the sample.

$rootPath = Split-Path -Parent $PSScriptRoot
$localPath = "C:\Users\ankikuma\Desktop\local-nuget"

`dotnet pack $rootPath\sdk\sdk\Sdk.csproj -o $localPath
`dotnet remove $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Sdk
`dotnet add $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Sdk -s $localPath --prerelease

`dotnet pack $rootPath\extensions\Worker.Extensions.Abstractions\Worker.Extensions.Abstractions.csproj -o $localPath
`dotnet remove $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Extensions.Abstractions
`dotnet add $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Extensions.Abstractions -s $localPath --prerelease

`dotnet pack $rootPath\extensions\Worker.Extensions.Http\Worker.Extensions.Http.csproj -o $localPath
`dotnet remove $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Extensions.Http
`dotnet add $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Extensions.Http -s $localPath --prerelease

`dotnet pack $rootPath\extensions\Worker.Extensions.Storage\Worker.Extensions.Storage.csproj -o $localPath
`dotnet remove $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Extensions.Storage
`dotnet add $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Extensions.Storage -s $localPath --prerelease

`dotnet pack $rootPath\extensions\Worker.Extensions.Abstractions\Worker.Extensions.Abstractions.csproj -o $localPath
`dotnet remove $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Extensions.Abstractions
`dotnet add $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj package Microsoft.Azure.Functions.Worker.Extensions.Abstractions -s $localPath --prerelease

`dotnet build $rootPath\samples\NewFunctionApp\NewFunctionApp.csproj