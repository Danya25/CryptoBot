﻿using CoinGecko.Clients;
using CoinGecko.Interfaces;
using CryptoBot.Crypto.Services;
using CryptoBot.DAL;
using CryptoBot.Handlers;
using CryptoBot.Models;
using CryptoBot.Settings;
using CryptoBot.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace CryptoBot.Tests
{
    public class TestHost
    {
        public IHost CreateTestHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostcontext, services) =>
                {
                    //TODO: Rework configuration
                    var botOptions = new BotOptions();
                    var sendingLimits = new TokenSendingLimit();
                    hostcontext.Configuration.GetSection(nameof(BotOptions)).Bind(botOptions);
                    hostcontext.Configuration.GetSection(nameof(TokenSendingLimit)).Bind(sendingLimits);
                    services.Configure<BotOptions>(t =>
                    {
                        t.Token = botOptions.Token;
                    });
                    services.Configure<TokenSendingLimit>(t =>
                    {
                        t.Min = sendingLimits.Min;
                        t.Max = sendingLimits.Max;
                    });

                    services.AddDbContextFactory<ApplicationContext, ApplicationDbContextFactory>(options => options.UseSqlServer(hostcontext.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botOptions.Token));
                    services.AddSingleton<IUpdateHandler, UpdateHandler>();
                    services.AddSingleton<ICryptoCurrencyService, CryptoCurrencyService>();

                    services.AddHostedService<TelegramHostedService>();

                    var http = new HttpClient();
                    var serializer = JsonSerializer.GetSerializerSettings();
                    services.AddSingleton<ICoinsClient>(new CoinsClient(http, serializer));
                    services.AddSingleton<IPingClient>(new PingClient(http, serializer));
                    services.AddSingleton<ISimpleClient>(new SimpleClient(http, serializer));
                    services.AddSingleton<TokenUpdater>();
                    services.AddSingleton<TokenSender>();

                    services.AddHttpClient();
                    services.AddMemoryCache();
                })
                .Build();
        }
    }
}
