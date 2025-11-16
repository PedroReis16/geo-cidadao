FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["GeoCidadao.AnalyticsServiceAPI/GeoCidadao.AnalyticsServiceAPI.csproj", "GeoCidadao.AnalyticsServiceAPI/"]
COPY ["Libraries/GeoCidadao.Models/GeoCidadao.Models.csproj", "Libraries/GeoCidadao.Models/"]
COPY ["Libraries/GeoCidadao.Database/GeoCidadao.Database.csproj", "Libraries/GeoCidadao.Database/"]
COPY ["Libraries/GeoCidadao.AMQP/GeoCidadao.AMQP.csproj", "Libraries/GeoCidadao.AMQP/"]
COPY ["Libraries/GeoCidadao.Caching/GeoCidadao.Caching.csproj", "Libraries/GeoCidadao.Caching/"]
COPY ["Libraries/GeoCidadao.OAuth/GeoCidadao.OAuth.csproj", "Libraries/GeoCidadao.OAuth/"]

# Restore dependencies
RUN dotnet restore "GeoCidadao.AnalyticsServiceAPI/GeoCidadao.AnalyticsServiceAPI.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/GeoCidadao.AnalyticsServiceAPI"
RUN dotnet build "GeoCidadao.AnalyticsServiceAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeoCidadao.AnalyticsServiceAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeoCidadao.AnalyticsServiceAPI.dll"]
