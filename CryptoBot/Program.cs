using CoinGecko.Clients;
using CoinGecko.Interfaces;
using CryptoBot.Crypto.Services;
using CryptoBot.DAL;
using CryptoBot.Handlers;
using CryptoBot.Models;
using CryptoBot.Services.PeriodValidator;
using CryptoBot.Settings;
using CryptoBot.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace CryptoBot
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var hostBuild = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostcontext, services) =>
                {
                    var configuration = hostcontext.Configuration;

                    Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));

                    services.Configure<BotOptions>(configuration.GetSection(nameof(BotOptions)));
                    services.Configure<TokenSendingLimit>(configuration.GetSection(nameof(TokenSendingLimit)));

                    services.AddDbContextFactory<ApplicationContext, ApplicationDbContextFactory>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

                    services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(configuration.GetSection($"{nameof(BotOptions)}:Token").Value));
                    services.AddSingleton<IUpdateHandler, UpdateHandler>();
                    services.AddSingleton<ICryptoCurrencyService, CryptoCurrencyService>();
                    services.AddSingleton<IPeriodValidator, PeriodValidator>();

                    services.Configure<HostOptions>(t => t.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore);

                    services.AddHostedService<TokenUpdateHostedService>();
                    services.AddHostedService<TelegramHostedService>();
                    services.AddHostedService<TokenSendHostedService>();

                    var http = new HttpClient();
                    var serializer = JsonSerializer.GetSerializerSettings();
                    services.AddSingleton<ICoinsClient>(new CoinsClient(http, serializer));
                    services.AddSingleton<IPingClient>(new PingClient(http, serializer));
                    services.AddSingleton<ISimpleClient>(new SimpleClient(http, serializer));

                    services.AddHttpClient();
                    services.AddMemoryCache();
                })
                .Build();
            Console.WriteLine(hostBuild.Services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection"));

            using (var context = hostBuild.Services.GetRequiredService<ApplicationContext>())
            {
                if ((await context.Database.GetPendingMigrationsAsync()).Any())
                    await context.Database.MigrateAsync();
            }

            await hostBuild.RunAsync();
        }
    }
}


