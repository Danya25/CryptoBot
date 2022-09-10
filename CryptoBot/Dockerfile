FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src

COPY ["CryptoBot/CryptoBot.csproj", "CryptoBot/"]
COPY ["CryptoBot.DAL/CryptoBot.DAL.csproj", "CryptoBot.DAL/"]
COPY ["CryptoBot.Tests/CryptoBot.Tests.csproj", "CryptoBot.Tests/"]

RUN dotnet restore "CryptoBot/CryptoBot.csproj"

COPY . .

WORKDIR "/src/CryptoBot"
RUN dotnet build CryptoBot.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CryptoBot.csproj -c Release -o /app/publish

FROM build AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CryptoBot.dll"]

