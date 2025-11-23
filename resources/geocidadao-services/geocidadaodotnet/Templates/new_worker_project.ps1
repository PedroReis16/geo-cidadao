param(
    [string]$PROJECT_NAME,
    [string]$WORKER_NAME
)

dotnet new worker -n $PROJECT_NAME -f net8.0 -o "./WorkerServices/$PROJECT_NAME"
dotnet sln add "./WorkerServices/$PROJECT_NAME/$PROJECT_NAME.csproj"

# Cria novo projeto de testes unitários e adiciona na solução
dotnet new xunit -f net8.0 -n "$PROJECT_NAME.UnitTests" -o "./Tests/Unit/WorkerServices/$PROJECT_NAME.UnitTests"
dotnet sln add "./Tests/Unit/WorkerServices/$PROJECT_NAME.UnitTests/$PROJECT_NAME.UnitTests.csproj"
dotnet add "./Tests/Unit/WorkerServices/$PROJECT_NAME.UnitTests/$PROJECT_NAME.UnitTests.csproj" reference "./WorkerServices/$PROJECT_NAME/$PROJECT_NAME.csproj"

# Adiciona referências e pacotes necessários para os testes unitários
dotnet add "./Tests/Unit/WorkerServices/$PROJECT_NAME.UnitTests/$PROJECT_NAME.UnitTests.csproj" reference "./Tests/GeoCidadao.TestShared/GeoCidadao.TestShared.csproj"
dotnet add "./Tests/Unit/WorkerServices/$PROJECT_NAME.UnitTests/$PROJECT_NAME.UnitTests.csproj" package FluentAssertions --version 8.3.0
New-Item -Path "./Tests/Unit/WorkerServices/$PROJECT_NAME.UnitTests/Services" -ItemType Directory

# Cria novo projeto de testes de integração e adiciona na solução
dotnet new xunit -f net8.0 -n "$PROJECT_NAME.IntegrationTests" -o "./Tests/Integration/WorkerServices/$PROJECT_NAME.IntegrationTests"
dotnet sln add "./Tests/Integration/WorkerServices/$PROJECT_NAME.IntegrationTests/$PROJECT_NAME.IntegrationTests.csproj"
dotnet add "./Tests/Integration/WorkerServices/$PROJECT_NAME.IntegrationTests/$PROJECT_NAME.IntegrationTests.csproj" reference "./WorkerServices/$PROJECT_NAME/$PROJECT_NAME.csproj"

# Adiciona referências e pacotes necessários para os testes de integração
dotnet add "./Tests/Integration/WorkerServices/$PROJECT_NAME.IntegrationTests/$PROJECT_NAME.IntegrationTests.csproj" reference "./Tests/GeoCidadao.TestShared/GeoCidadao.TestShared.csproj"
dotnet add "./Tests/Integration/WorkerServices/$PROJECT_NAME.IntegrationTests/$PROJECT_NAME.IntegrationTests.csproj" package FluentAssertions --version 8.3.0

New-Item -Path "./Tests/Integration/WorkerServices/$PROJECT_NAME.IntegrationTests/Services" -ItemType Directory


dotnet add ".\WorkerServices\$PROJECT_NAME/$PROJECT_NAME.csproj" reference "./Libraries/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj"
dotnet add ".\WorkerServices\$PROJECT_NAME/$PROJECT_NAME.csproj" reference "./Libraries/GeoCidadao.Models/GeoCidadao.Models.csproj"


New-Item -Path ".\WorkerServices\$PROJECT_NAME\Services" -ItemType Directory
New-Item -Path ".\WorkerServices\$PROJECT_NAME\Model" -ItemType Directory
New-Item -Path ".\WorkerServices\$PROJECT_NAME\Config" -ItemType Directory

# Cria classes padrão
Copy-Item -Path "Templates\Worker.AppSettingsProperties.cs" -Destination ".\WorkerServices\$PROJECT_NAME\Config\AppSettingsProperties.cs"
(Get-Content ".\WorkerServices\$PROJECT_NAME\Config\AppSettingsProperties.cs") -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content ".\WorkerServices\$PROJECT_NAME\Config\AppSettingsProperties.cs"

Copy-Item -Path "Templates\AssemblyInfo.cs" -Destination ".\WorkerServices\$PROJECT_NAME\Properties\AssemblyInfo.cs"
(Get-Content ".\WorkerServices\$PROJECT_NAME\Properties\AssemblyInfo.cs") -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content ".\WorkerServices\$PROJECT_NAME\Properties\AssemblyInfo.cs"

# Cria arquivos de configurações
Copy-Item -Path "Templates\Worker.appsettings.json" -Destination ".\WorkerServices\$PROJECT_NAME\appsettings.json"
(Get-Content ".\WorkerServices\$PROJECT_NAME\appsettings.json") -replace '<WORKER_NAME>', $WORKER_NAME -replace '<LOG_NAME>', ( -join ($WORKER_NAME -split '[-_\s]+' | ForEach-Object { if ($_.Length -gt 0) { $_.Substring(0, 1).ToUpper() + $_.Substring(1).ToLower() } })) | Set-Content ".\WorkerServices\$PROJECT_NAME\appsettings.json"

Copy-Item -Path "Templates\Worker.appsettings.Development.json" -Destination ".\WorkerServices\$PROJECT_NAME\appsettings.Development.json"
(Get-Content ".\WorkerServices\$PROJECT_NAME\appsettings.Development.json") -replace '<WORKER_NAME>', $WORKER_NAME -replace '<LOG_NAME>', ( -join ($WORKER_NAME -split '[-_\s]+' | ForEach-Object { if ($_.Length -gt 0) { $_.Substring(0, 1).ToUpper() + $_.Substring(1).ToLower() } })) | Set-Content ".\WorkerServices\$PROJECT_NAME\appsettings.Development.json"

Copy-Item -Path "Templates\Worker.Template.env" -Destination "../../../docker-compose/prod/$WORKER_NAME-worker.env"
(Get-Content "../../../docker-compose/prod/$WORKER_NAME-worker.env") -replace '<LOG_NAME>', ( -join ($WORKER_NAME -split '[-_\s]+' | ForEach-Object { if ($_.Length -gt 0) { $_.Substring(0, 1).ToUpper() + $_.Substring(1).ToLower() } })) | Set-Content "../../../docker-compose/prod/$WORKER_NAME-worker.env" 

Copy-Item -Path "Templates\Worker.Template.env" -Destination "../../../docker-compose/local/$WORKER_NAME-worker.env"
(Get-Content "../../../docker-compose/local/$WORKER_NAME-worker.env") -replace '<LOG_NAME>', ( -join ($WORKER_NAME -split '[-_\s]+' | ForEach-Object { if ($_.Length -gt 0) { $_.Substring(0, 1).ToUpper() + $_.Substring(1).ToLower() } })) | Set-Content "../../../docker-compose/local/$WORKER_NAME-worker.env"
# Cria arquivos para deploy
Copy-Item -Path "Templates\Template.dockerfile" -Destination "../../../docker-compose/dockerfiles/$WORKER_NAME-worker.dockerfile"
(Get-Content "../../../docker-compose/dockerfiles/$WORKER_NAME-worker.dockerfile") -replace '<PROJECT_NAME>', $PROJECT_NAME | Set-Content "../../../docker-compose/dockerfiles/$WORKER_NAME-worker.dockerfile"


# Modifica o arquivo .csproj para adicionar as configurações de documentação
$csprojPath = ".\WorkerServices\$PROJECT_NAME\$PROJECT_NAME.csproj"
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
dotnet build ".\WorkerServices\$PROJECT_NAME\$PROJECT_NAME.csproj"