FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY resources/geocidadao-services/geocidadaodotnet/GeoCidadao.EngagementServiceAPI/GeoCidadao.EngagementServiceAPI.csproj GeoCidadao.EngagementServiceAPI/
COPY resources/geocidadao-services/geocidadaodotnet/Labraries/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj GeoCidadao.AMQP/
COPY resources/geocidadao-services/geocidadaodotnet/Labraries/GeoCidadao.Database/GeoCidadao.Database.csproj GeoCidadao.Database/
COPY resources/geocidadao-services/geocidadaodotnet/Labraries/GeoCidadao.Caching/GeoCidadao.Caching.csproj GeoCidadao.Caching/
COPY resources/geocidadao-services/geocidadaodotnet/Labraries/GeoCidadao.Models/GeoCidadao.Models.csproj GeoCidadao.Models/

RUN dotnet restore GeoCidadao.EngagementServiceAPI/GeoCidadao.EngagementServiceAPI.csproj

COPY resources/geocidadao-services/geocidadaodotnet/ ./

WORKDIR "/src/GeoCidadao.EngagementServiceAPI"
RUN dotnet build "GeoCidadao.EngagementServiceAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeoCidadao.EngagementServiceAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeoCidadao.EngagementServiceAPI.dll"]
