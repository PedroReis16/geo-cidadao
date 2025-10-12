FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.UserPostsAPI/GeoCidadao.UserPostsAPI.csproj", "GeoCidadao.UserPostsAPI/"]
COPY ["resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj", "GeoCidadao.AMQP/"]
COPY ["resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Database/GeoCidadao.Database.csproj", "GeoCidadao.Database/"]
COPY ["resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Caching/GeoCidadao.Caching.csproj", "GeoCidadao.Caching/"]
COPY ["resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.Model/GeoCidadao.Model.csproj", "GeoCidadao.Model/"]

RUN dotnet restore "resources/geo-cidadao-services/geocidadaodotnet/GeoCidadao.UserPostsAPI/GeoCidadao.UserPostsAPI.csproj"
COPY . .
WORKDIR "/src/GeoCidadao.UserPostsAPI"
RUN dotnet build "GeoCidadao.UserPostsAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeoCidadao.UserPostsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeoCidadao.UserPostsAPI.dll"]
