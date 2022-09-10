using CryptoBot.Crypto.Services;
using CryptoBot.DAL;
using CryptoBot.Models;
using CryptoBot.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Text;
using CryptoBot.Constants;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CryptoBot
{
    public class TokenUpdateHostedService : BackgroundService
    {
        private ICryptoCurrencyService _cryptoCurrencyService;
        private IDbContextFactory<ApplicationContext> _factoryContext;
        private ITelegramBotClient _botClient;

        private IMemoryCache _memoryCache;
        public TokenUpdateHostedService(ICryptoCurrencyService cryptoCurrencyService, ITelegramBotClient botClient, IDbContextFactory<ApplicationContext> factoryContext, IMemoryCache memoryCache)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
            _botClient = botClient;
            _factoryContext = factoryContext;
            _memoryCache = memoryCache;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var dbContext = _factoryContext.CreateDbContext();
                var result = await _cryptoCurrencyService.GetTokenPrice(DefaultCryptoList.CryptoList, DefaultCryptoList.CurrencyList);
                var sb = new StringBuilder();

                foreach (var token in result)
                {
                    if(!_memoryCache.TryGetValue(token.Name, out CryptoToken cacheToken))
                    {
                        cacheToken = new CryptoToken
                        {
                            Name = token.Name,
                            UsdPrice = token.UsdPrice
                        };
                    }
                    cacheToken.UsdPrice = token.UsdPrice;
                    _memoryCache.Set(cacheToken.Name, cacheToken);
                }

                await Task.Delay(10_000);
            }
        }
    }
}
