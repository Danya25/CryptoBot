FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src

COPY ["CryptoBot/CryptoBot.csproj", "CryptoBot/"]
COPY ["CryptoBot.DAL/CryptoBot.DAL.csproj", "CryptoBot.DAL/"]
COPY ["CryptoBot.Tests/CryptoBot.Tests.csproj", "CryptoBot.Tests/"]
COPY ["CryptoBot.MigrationApp/CryptoBot.MigrationApp.csproj", "CryptoBot.MigrationApp/"]

RUN dotnet restore "CryptoBot/CryptoBot.csproj"
RUN dotnet restore "CryptoBot.DAL/CryptoBot.DAL.csproj"
RUN dotnet restore "CryptoBot.Tests/CryptoBot.Tests.csproj"
RUN dotnet restore "CryptoBot.MigrationApp/CryptoBot.MigrationApp.csproj"

COPY . .

WORKDIR "/src/CryptoBot.MigrationApp"
RUN dotnet build "CryptoBot.MigrationApp.csproj" -c Release -o /app/migration

WORKDIR "/src/CryptoBot"
RUN dotnet build CryptoBot.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CryptoBot.csproj -c Release -o /app/publish

FROM build AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CryptoBot.dll"]

