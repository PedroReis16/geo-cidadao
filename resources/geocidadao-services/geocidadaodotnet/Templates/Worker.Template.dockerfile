# build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY resources/geocidadao-services/geocidadaodotnet/WorkerServices/<PROJECT_NAME>/<PROJECT_NAME>.csproj <PROJECT_NAME>/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj GeoCidadao.AMQP/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.Models/GeoCidadao.Models.csproj GeoCidadao.Models/
RUN dotnet restore <PROJECT_NAME>/<PROJECT_NAME>.csproj

COPY resources/geocidadao-services/geocidadaodotnet/WorkerServices/<PROJECT_NAME> ./WorkerServices/<PROJECT_NAME>
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.AMQP ./Libraries/GeoCidadao.AMQP
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.Models ./Libraries/GeoCidadao.Models

WORKDIR "/src/WorkerServices/<PROJECT_NAME>"
RUN dotnet publish "<PROJECT_NAME>.csproj" -c Release -o /app/publish /p:UseAppHost=false

# runtime
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "<PROJECT_NAME>.dll"]