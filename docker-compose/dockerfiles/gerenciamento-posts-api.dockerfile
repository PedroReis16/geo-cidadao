FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.GerenciamentoPostsAPI/GeoCidadao.GerenciamentoPostsAPI.csproj GeoCidadao.GerenciamentoPostsAPI/
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj GeoCidadao.AMQP/
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Database/GeoCidadao.Database.csproj GeoCidadao.Database/
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Caching/GeoCidadao.Caching.csproj GeoCidadao.Caching/
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Models/GeoCidadao.Models.csproj GeoCidadao.Models/
COPY resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Cloud/GeoCidadao.Cloud.csproj GeoCidadao.Cloud/

RUN dotnet restore GeoCidadao.GerenciamentoPostsAPI/GeoCidadao.GerenciamentoPostsAPI.csproj

COPY resources/geo-cidadao-services/geocidadaodotnet/ ./

WORKDIR "/src/GeoCidadao.GerenciamentoPostsAPI"
RUN dotnet build "GeoCidadao.GerenciamentoPostsAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeoCidadao.GerenciamentoPostsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeoCidadao.GerenciamentoPostsAPI.dll"]
