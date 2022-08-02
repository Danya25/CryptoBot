using CryptoBot;
using CryptoBot.Crypto.Services;
using CryptoBot.DAL;
using CryptoBot.Handlers;
using CryptoBot.Models;
using CryptoBot.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

await Host.CreateDefaultBuilder(args)
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

        services.AddHostedService<TokenUpdateHostedService>();
        services.AddHostedService<TelegramHostedService>();
        services.AddHostedService<TokenSendHostedService>();

        services.AddHttpClient();
        services.AddMemoryCache();
    })
    .Build()
    .RunAsync();