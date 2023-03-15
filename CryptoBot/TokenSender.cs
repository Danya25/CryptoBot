using CryptoBot.DAL;
using CryptoBot.DAL.Models;
using CryptoBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CryptoBot
{
    internal class TokenSender
    {
        private ITelegramBotClient _botClient;
        private IDbContextFactory<ApplicationContext> _factoryContext;
        private IMemoryCache _memoryCache;
        public TokenSender(ITelegramBotClient botClient, IDbContextFactory<ApplicationContext> factoryContext, IMemoryCache memoryCache)
        {
            _botClient = botClient;
            _factoryContext = factoryContext;
            _memoryCache = memoryCache;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var myId = "310018521";
            var sb = new StringBuilder();

            try
            {
                using var dbContext = _factoryContext.CreateDbContext();
                var users = dbContext.Users
                .Include(t => t.PostInfo)
                .Where(t => t.PostInfo.LastPostTime.AddSeconds(t.PostInfo.Timer) <= DateTime.UtcNow)
                .ToList();

                // TODO: HashSet/Dictionary
                var tokens = await dbContext.Tokens.ToListAsync();

                foreach (var user in users)
                {
                    var userTokens = user.PostInfo.CryptoSetCollection;
                    foreach (var userToken in userTokens)
                    {
                        var token = tokens.FirstOrDefault(t=> t.Name.Equals(userToken));
                        if (token is null)
                            continue;

                        var text = $"<b>✨{token.Name}</b>: {token.PriceUsd}$ \n";
                        var separator = string.Join("", text.Select(t => "-").ToArray()) + "\n";
                        sb.Append(text);
                        sb.Append(separator);
                    }

                    user.PostInfo.LastPostTime = DateTime.UtcNow;

                    if (sb.Length == 0)
                        continue;

                    await _botClient.SendTextMessageAsync(user.TelegramId, sb.ToString(), ParseMode.Html);
                    sb.Clear();
                }
                if (users.Count > 0)
                {
                    await dbContext.SaveChangesAsync();
                }
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
