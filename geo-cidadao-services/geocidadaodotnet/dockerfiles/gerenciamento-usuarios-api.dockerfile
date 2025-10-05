FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GeoCidadao.GerenciamentoUsuariosAPI/GeoCidadao.GerenciamentoUsuariosAPI.csproj", "GeoCidadao.GerenciamentoUsuariosAPI/"]
COPY ["GeoCidadao.AMQP/GeoCidadao.AMQP.csproj", "GeoCidadao.AMQP/"]
COPY ["GeoCidadao.Database/GeoCidadao.Database.csproj", "GeoCidadao.Database/"]
COPY ["GeoCidadao.Caching/GeoCidadao.Caching.csproj", "GeoCidadao.Caching/"]
COPY ["GeoCidadao.Model/GeoCidadao.Model.csproj", "GeoCidadao.Model/"]

RUN dotnet restore "GeoCidadao.GerenciamentoUsuariosAPI/GeoCidadao.GerenciamentoUsuariosAPI.csproj"
COPY . .
WORKDIR "/src/GeoCidadao.GerenciamentoUsuariosAPI"
RUN dotnet build "GeoCidadao.GerenciamentoUsuariosAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeoCidadao.GerenciamentoUsuariosAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeoCidadao.GerenciamentoUsuariosAPI.dll"]
