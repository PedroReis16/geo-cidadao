param(
    [string]$PROJECT_NAME,
    [string]$API_NAME,
    [int]$HTTP_PORT,
    [int]$SSL_PORT
)

# Cria novo projeto de Web API e adiciona na solução
dotnet new webapi -n $PROJECT_NAME -f net8.0
dotnet sln add $PROJECT_NAME


# Adiciona packages do NuGet (sempre revisar as versões)
dotnet add $PROJECT_NAME package Microsoft.EntityFrameworkCore --version 8.0.4
dotnet add $PROJECT_NAME package Microsoft.EntityFrameworkCore.Design --version 8.0.4
dotnet add $PROJECT_NAME package Microsoft.VisualStudio.Azure.Containers.Tools.Targets --version 1.19.5
dotnet add $PROJECT_NAME package NetDevPack.Security.JwtExtensions --version 8.0.0
dotnet add $PROJECT_NAME package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.2
dotnet add $PROJECT_NAME package RabbitMQ.Client --version 6.8.1
dotnet add $PROJECT_NAME package Swashbuckle.AspNetCore --version 6.5.0
dotnet add $PROJECT_NAME package Swashbuckle.AspNetCore.Annotations --version 6.5.0
dotnet add $PROJECT_NAME package Polly --version 8.2.0
dotnet add $PROJECT_NAME package Polly.Extensions --version 8.4.1
dotnet add $PROJECT_NAME package Polly.Extensions.Http --version 3.0.0
dotnet add $PROJECT_NAME package Microsoft.Extensions.Http.Polly --version 8.2.0

# Adiciona referências dos projetos
dotnet add $PROJECT_NAME reference GeoCidadao.Models
dotnet add $PROJECT_NAME reference GeoCidadao.Caching
dotnet add $PROJECT_NAME reference GeoCidadao.Database
dotnet add $PROJECT_NAME reference GeoCidadao.AMQP
dotnet add $PROJECT_NAME reference GeoCidadao.Jobs

# Cria estrutura de diretórios
New-Item -Path "$PROJECT_NAME\Controllers" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Contracts" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Services" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Model" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Config" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Middlewares" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Database" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Database\Contracts" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Database\EFDao" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Database\CacheContracts" -ItemType Directory
New-Item -Path "$PROJECT_NAME\Database\Cache" -ItemType Directory

# Cria classes padrão
Copy-Item -Path "Templates\AppSettingsProperties.cs" -Destination "$PROJECT_NAME\Config\AppSettingsProperties.cs"
(Get-Content "$PROJECT_NAME\Config\AppSettingsProperties.cs") -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content "$PROJECT_NAME\Config\AppSettingsProperties.cs"

Copy-Item -Path "Templates\SlugifyParameterTransformer.cs" -Destination "$PROJECT_NAME\Config\SlugifyParameterTransformer.cs"
(Get-Content "$PROJECT_NAME\Config\SlugifyParameterTransformer.cs") -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content "$PROJECT_NAME\Config\SlugifyParameterTransformer.cs"

Copy-Item -Path ".\Templates\TagDescriptionsDocumentFilter.cs" -Destination "$PROJECT_NAME\Config\TagDescriptionsDocumentFilter.cs"
(Get-Content "$PROJECT_NAME\Config\TagDescriptionsDocumentFilter.cs") -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content "$PROJECT_NAME\Config\TagDescriptionsDocumentFilter.cs"

Copy-Item -Path "Templates\Program.cs" -Destination "$PROJECT_NAME\Program.cs"
(Get-Content "$PROJECT_NAME\Program.cs") -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content "$PROJECT_NAME\Program.cs"

Copy-Item -Path "Templates\HealthCheckController.cs" -Destination "$PROJECT_NAME\Controllers\HealthCheckController.cs"
(Get-Content "$PROJECT_NAME\Controllers\HealthCheckController.cs") -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content "$PROJECT_NAME\Controllers\HealthCheckController.cs"

# Cria arquivos de configurações
Copy-Item -Path "Templates\launchSettings.json" -Destination "$PROJECT_NAME\Properties\launchSettings.json"
(Get-Content "$PROJECT_NAME\Properties\launchSettings.json") -replace '<HTTP_PORT>', $HTTP_PORT -replace '<SSL_PORT>', $SSL_PORT -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content "$PROJECT_NAME\Properties\launchSettings.json"

Copy-Item -Path "Templates\appsettings.json" -Destination "$PROJECT_NAME\appsettings.json"
(Get-Content "$PROJECT_NAME\appsettings.json") -replace '<API_NAME>', $API_NAME -replace '<LOG_NAME>', ( -join ($API_NAME -split '[-_\s]+' | ForEach-Object { if ($_.Length -gt 0) { $_.Substring(0, 1).ToUpper() + $_.Substring(1).ToLower() } })) | Set-Content "$PROJECT_NAME\appsettings.json"

Copy-Item -Path "Templates\appsettings.Development.json" -Destination "$PROJECT_NAME\appsettings.Development.json"
(Get-Content "$PROJECT_NAME\appsettings.Development.json") -replace '<API_NAME>', $API_NAME -replace '<LOG_NAME>', ( -join ($API_NAME -split '[-_\s]+' | ForEach-Object { if ($_.Length -gt 0) { $_.Substring(0, 1).ToUpper() + $_.Substring(1).ToLower() } })) | Set-Content "$PROJECT_NAME\appsettings.Development.json"

Copy-Item -Path "Templates\Template.env" -Destination "../../../docker-compose/$API_NAME.env"
(Get-Content "../../../docker-compose/$API_NAME.env") -replace '<API_NAME>', $API_NAME | Set-Content "../../../docker-compose/$API_NAME.env" 

# Cria arquivos para deploy
Copy-Item -Path "Templates\Template.dockerfile" -Destination "../../../docker-compose/dockerfiles/$API_NAME-api.dockerfile"
(Get-Content "../../../docker-compose/dockerfiles/$API_NAME-api.dockerfile") -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content "../../../docker-compose/dockerfiles/$API_NAME-api.dockerfile"

# Exclui arquivos sem uso
Remove-Item "$PROJECT_NAME\$PROJECT_NAME.http"

# Modifica o arquivo .csproj para adicionar as configurações de documentação
$csprojPath = "$PROJECT_NAME/$PROJECT_NAME.csproj"
[xml]$csprojXml = Get-Content $csprojPath

# Verifica se já existe a tag PropertyGroup
$propertyGroup = $csprojXml.Project.PropertyGroup[0]
if ($null -eq $propertyGroup) {
    $propertyGroup = $csprojXml.CreateElement("PropertyGroup")
    $csprojXml.Project.AppendChild($propertyGroup)
}

# Adiciona as tags <GenerateDocumentationFile> e <NoWarn> se não existirem
$generateDocNode = $propertyGroup.SelectSingleNode("GenerateDocumentationFile")
if ($null -eq $generateDocNode) {
    $generateDocNode = $csprojXml.CreateElement("GenerateDocumentationFile")
    $generateDocNode.InnerText = "true"
    $propertyGroup.AppendChild($generateDocNode)
}

$noWarnNode = $propertyGroup.SelectSingleNode("NoWarn")
if ($null -eq $noWarnNode) {
    $noWarnNode = $csprojXml.CreateElement("NoWarn")
    $noWarnNode.InnerText = "`$(NoWarn);1591"
    $propertyGroup.AppendChild($noWarnNode)
}
else {
    # Se a tag NoWarn já existe, adiciona 1591 caso ainda não esteja presente
    if ($noWarnNode.InnerText -notlike "*1591*") {
        $noWarnNode.InnerText += ";1591"
    }
}

# Salva as alterações no arquivo .csproj
$csprojXml.Save($csprojPath)

# Realiza o build do projeto
dotnet build $PROJECT_NAME