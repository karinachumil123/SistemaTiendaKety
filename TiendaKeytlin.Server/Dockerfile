# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TiendaKeytlin.Server/TiendaKeytlin.Server.csproj", "TiendaKeytlin.Server/"]
RUN dotnet restore "TiendaKeytlin.Server/TiendaKeytlin.Server.csproj"
COPY . .
WORKDIR "/src/TiendaKeytlin.Server"
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "TiendaKeytlin.Server.dll"]
