# build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY resources/geocidadao-services/geocidadaodotnet/WorkerServices/GeoCidadao.RelevanceWorker/GeoCidadao.RelevanceWorker.csproj GeoCidadao.RelevanceWorker/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj GeoCidadao.AMQP/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.Models/GeoCidadao.Models.csproj GeoCidadao.Models/
RUN dotnet restore GeoCidadao.RelevanceWorker/GeoCidadao.RelevanceWorker.csproj

COPY resources/geocidadao-services/geocidadaodotnet/WorkerServices/GeoCidadao.RelevanceWorker ./WorkerServices/GeoCidadao.RelevanceWorker
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.AMQP ./Libraries/GeoCidadao.AMQP
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.Models ./Libraries/GeoCidadao.Models

WORKDIR "/src/WorkerServices/GeoCidadao.RelevanceWorker"
RUN dotnet publish "GeoCidadao.RelevanceWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# runtime
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GeoCidadao.RelevanceWorker.dll"]