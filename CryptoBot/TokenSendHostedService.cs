using CryptoBot.DAL;
using CryptoBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CryptoBot
{
    internal class TokenSendHostedService : BackgroundService
    {
        private ITelegramBotClient _botClient;
        private IDbContextFactory<ApplicationContext> _factoryContext;
        private IMemoryCache _memoryCache;
        public TokenSendHostedService(ITelegramBotClient botClient, IDbContextFactory<ApplicationContext> factoryContext, IMemoryCache memoryCache)
        {
            _botClient = botClient;
            _factoryContext = factoryContext;
            _memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var myId = "310018521";
            var sb = new StringBuilder();

            while (!stoppingToken.IsCancellationRequested)
            {
                using var dbContext = _factoryContext.CreateDbContext();
                var users = dbContext.Users
                .Include(t => t.PostInfo)
                .Where(t => t.PostInfo.LastPostTime.AddSeconds(t.PostInfo.Timer) <= DateTime.UtcNow)
                .ToList();

                foreach (var user in users)
                {
                    var userTokens = user.PostInfo.CryptoSetCollection;
                    foreach (var userToken in userTokens)
                    {
                        var cachedToken = _memoryCache.Get<CryptoToken>(userToken.ToUpper());
                        if (cachedToken is null)
                            continue;

                        var text = $"<b>{cachedToken.Name}</b>: {cachedToken.UsdPrice}$ \n";
                        var separator = string.Join("", text.Select(t => "-").ToArray()) + "\n";
                        sb.Append(text);
                        sb.Append(separator);
                    }

                    user.PostInfo.LastPostTime = DateTime.UtcNow;

                    await _botClient.SendTextMessageAsync(user.TelegramId, sb.ToString(), ParseMode.Html);
                    sb.Clear();
                }
                if(users.Count > 0)
                {
                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(10_000);
            }
        }
    }
}
