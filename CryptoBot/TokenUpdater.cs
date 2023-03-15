using CryptoBot.Crypto.Services;
using CryptoBot.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CryptoBot.Constants;
using CryptoBot.DAL.Models;
using CryptoBot.DAL.Extensions;

namespace CryptoBot
{
    public class TokenUpdater
    {
        private ICryptoCurrencyService _cryptoCurrencyService;
        private IDbContextFactory<ApplicationContext> _factoryContext;

        private IMemoryCache _memoryCache;
        public TokenUpdater(ICryptoCurrencyService cryptoCurrencyService, IDbContextFactory<ApplicationContext> factoryContext, IMemoryCache memoryCache)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
            _factoryContext = factoryContext;
            _memoryCache = memoryCache;
        }
        public async Task ExececuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var dbContext = _factoryContext.CreateDbContext();
                var result = await _cryptoCurrencyService.GetTokenPrice(DefaultCryptoList.CryptoList, DefaultCryptoList.CurrencyList);
                var tokens = result.Select(t => new Token { Date = DateTime.UtcNow, Name = t.Name, PriceUsd = t.UsdPrice.ToString() });

                foreach(var token in tokens)
                {
                    dbContext.AddOrUpdate(token);
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex) when (stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Task was stopped \r\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

        }
    }
}
