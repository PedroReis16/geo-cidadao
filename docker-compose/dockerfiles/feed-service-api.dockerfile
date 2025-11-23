FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY resources/geocidadao-services/geocidadaodotnet/GeoCidadao.FeedServiceAPI/GeoCidadao.FeedServiceAPI.csproj GeoCidadao.FeedServiceAPI/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj GeoCidadao.AMQP/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.Database/GeoCidadao.Database.csproj GeoCidadao.Database/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.Caching/GeoCidadao.Caching.csproj GeoCidadao.Caching/
COPY resources/geocidadao-services/geocidadaodotnet/Libraries/GeoCidadao.Models/GeoCidadao.Models.csproj GeoCidadao.Models/

RUN dotnet restore GeoCidadao.FeedServiceAPI/GeoCidadao.FeedServiceAPI.csproj

COPY resources/geocidadao-services/geocidadaodotnet/ ./

WORKDIR "/src/GeoCidadao.FeedServiceAPI"
RUN dotnet build "GeoCidadao.FeedServiceAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeoCidadao.FeedServiceAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeoCidadao.FeedServiceAPI.dll"]
