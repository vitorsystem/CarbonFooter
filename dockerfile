# Etapa de Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar os arquivos de projeto e restaurar as dependências
COPY ["CarbonFooter.Api/CarbonFooter.Api.csproj", "CarbonFooter.Api/"]
COPY ["CarbonFooter.Tests/CarbonFooter.Tests.csproj", "CarbonFooter.Tests/"]
RUN dotnet restore "CarbonFooter.Api/CarbonFooter.Api.csproj"

# Copiar todo o código-fonte
COPY . .
WORKDIR "/src/CarbonFooter.Api"
RUN dotnet publish "CarbonFooter.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CarbonFooter.Api.dll"]