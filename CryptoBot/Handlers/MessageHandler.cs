using CryptoBot.Constants;
using CryptoBot.DAL;
using CryptoBot.DAL.Extensions;
using CryptoBot.DAL.Models;
using CryptoBot.Settings;
using CryptoBot.Utils;
using CryptoBot.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBot.Handlers
{
    public partial class UpdateHandler
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContextFactory;
        private readonly ITelegramBotClient _botClient;
        private readonly IOptions<TokenSendingLimit> _options;
        public UpdateHandler(
            IDbContextFactory<ApplicationContext> dbContextFactory, 
            ITelegramBotClient botClient, 
            IOptions<TokenSendingLimit> options)
        {
            _botClient = botClient;
            _dbContextFactory = dbContextFactory;
            _options = options;
        }

        public async Task HandleMessage(Message? m)
        {
            var commands = m.Text.Split(' ');
            using var dbContext = _dbContextFactory.CreateDbContext();

            await (commands[0] switch
            {
                "/start" => HandleStartMessage(m, dbContext),
                "/time" => HandleTimeMessage(m, dbContext),
                "/currency" => HandleCurrencyMessage(m, dbContext),
                _ => DefaultTextHandler(m),
            });
        }

        private async Task HandleCurrencyMessage(Message m, ApplicationContext dbContext)
        {
            var text = m.Text.Split(" ");
            var userId = m.From.Id;

            if (text.Length != 2)
            {
                await _botClient.SendTextMessageAsync(userId, TextConstant.CommandWasNotRecognized);
                return;
            }

            var value = text[1];
            var isValidCurrency = CryptoListConstant.CurrencyList.Contains(value);
            if (!isValidCurrency)
            {
                await _botClient.SendTextMessageAsync(userId, "Currency isn't valid");
                return;
            }

            var info = await dbContext.UserPostsInfo.FirstAsync(t => t.UserId == userId);
            info.Currency = value;
            await dbContext.SaveChangesAsync();
        }

        private async Task HandleStartMessage(Message? m, ApplicationContext dbContext)
        {

            var userId = m.From.Id;
            var hasUser = await dbContext.Users.Where(t => t.TelegramId == userId).GetFirstOrDefaultASync();
            if (hasUser != null)
                return;

            var user = new DAL.Models.User
            {
                TelegramId = userId,
                Name = $"{m.From.FirstName} {m.From.LastName}"
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            await _botClient.SendTextMessageAsync(userId, "Welcome to the Crypto Bot.");
            await _botClient.SendTextMessageAsync(userId, "For set a certain time. Write /time 100 (in seconds max 864000 seconds)");
        }
        private async Task HandleTimeMessage(Message? m, ApplicationContext dbContext)
        {
            var text = m.Text.Split(" ");
            var userId = m.From.Id;

            if (text.Length != 2)
            {
                await _botClient.SendTextMessageAsync(userId, TextConstant.CommandWasNotRecognized);
                return;
            }

            var value = text[1];
            var isValidate = PeriodValidator.TryValidate(value, _options, out int period);
            if(!isValidate)
            {
                await _botClient.SendTextMessageAsync(userId, TextConstant.CommandWasNotRecognized);
            }

            var userPostInfo = dbContext.UserPostsInfo.Where(t => t.UserId == userId).FirstOrDefault();
            if (userPostInfo == null)
            {
                var userInfo = new UserPostInfo
                {
                    UserId = userId,
                    Timer = period,
                    CryptoSet = string.Join(';', CryptoListConstant.CryptoList),
                };
                await dbContext.UserPostsInfo.AddAsync(userInfo);
            }
            else
            {
                userPostInfo.Timer = period;
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task DefaultTextHandler(Message? m)
        {
           
        }
    }
}
