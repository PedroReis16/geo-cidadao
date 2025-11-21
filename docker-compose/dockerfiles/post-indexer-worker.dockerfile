# build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY resources/geocidadao-services/geocidadaodotnet/WorkerServices/GeoCidadao.PostIndexerWorker/GeoCidadao.PostIndexerWorker.csproj GeoCidadao.PostIndexerWorker/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj GeoCidadao.AMQP/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.Models/GeoCidadao.Models.csproj GeoCidadao.Models/
RUN dotnet restore GeoCidadao.PostIndexerWorker/GeoCidadao.PostIndexerWorker.csproj

COPY resources/geocidadao-services/geocidadaodotnet/WorkerServices ./

WORKDIR "/src/GeoCidadao.PostIndexerWorker"
RUN dotnet publish "GeoCidadao.PostIndexerWorker.csproj" -c Release -o /app/publish 

FROM build AS publish
RUN dotnet publish "GeoCidadao.PostIndexerWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeoCidadao.PostIndexerWorker.dll"]