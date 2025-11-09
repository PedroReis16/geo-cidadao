FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY resources/geo-cidadao-services/geocidadaodotnet/<PROJECT_NAME>/<PROJECT_NAME>.csproj <PROJECT_NAME>/
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj GeoCidadao.AMQP/
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Database/GeoCidadao.Database.csproj GeoCidadao.Database/
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Caching/GeoCidadao.Caching.csproj GeoCidadao.Caching/
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Models/GeoCidadao.Models.csproj GeoCidadao.Models/

RUN dotnet restore <PROJECT_NAME>/<PROJECT_NAME>.csproj

COPY resources/geo-cidadao-services/geocidadaodotnet/ ./

WORKDIR "/src/<PROJECT_NAME>"
RUN dotnet build "<PROJECT_NAME>.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "<PROJECT_NAME>.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "<PROJECT_NAME>.dll"]