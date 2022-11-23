FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

WORKDIR /src
COPY ["CryptoBot/CryptoBot.csproj", "CryptoBot/"]
COPY ["CryptoBot.DAL/CryptoBot.DAL.csproj", "CryptoBot.DAL/"]
COPY ["CryptoBot.MigrationApp/CryptoBot.MigrationApp.csproj", "CryptoBot.MigrationApp/"]

RUN dotnet restore "CryptoBot/CryptoBot.csproj"
RUN dotnet restore "CryptoBot.DAL/CryptoBot.DAL.csproj"
RUN dotnet restore "CryptoBot.MigrationApp/CryptoBot.MigrationApp.csproj"

COPY . .


FROM build AS build-bot
WORKDIR "/src/CryptoBot"
RUN dotnet build CryptoBot.csproj -c Release -o /app


FROM build AS build-migrationapp
WORKDIR "/src/CryptoBot.MigrationApp"
RUN dotnet build CryptoBot.MigrationApp.csproj -c Release -o /migration


FROM build-bot AS publish-bot
RUN dotnet publish CryptoBot.csproj -c Release -o /app/publish


FROM build-migrationapp AS publish-migrationapp
RUN dotnet publish CryptoBot.MigrationApp.csproj -c Release -o /migration/publish


FROM build AS final
WORKDIR /app
COPY --from=publish-bot /app/publish .
WORKDIR /migration
COPY --from=build "/src/CryptoBot.DAL" .


WORKDIR /app
ENTRYPOINT ["dotnet", "CryptoBot.dll"]