using CryptoBot.Constants;
using CryptoBot.Crypto.Services;
using CryptoBot.DAL;
using CryptoBot.DAL.Extensions;
using CryptoBot.DAL.Models;
using CryptoBot.Services.PeriodValidator;
using CryptoBot.Utils;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBot.Handlers
{
    public partial class UpdateHandler
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContextFactory;
        private readonly ITelegramBotClient _botClient;
        private readonly ICryptoCurrencyService _cryptoCurrencyService;
        private readonly IPeriodValidator _periodValidator;
        public UpdateHandler(
            IDbContextFactory<ApplicationContext> dbContextFactory,
            ITelegramBotClient botClient,
            ICryptoCurrencyService cryptoCurrencyService,
            IPeriodValidator periodValidator)
        {
            _botClient = botClient;
            _dbContextFactory = dbContextFactory;
            _cryptoCurrencyService = cryptoCurrencyService;
            _periodValidator = periodValidator;
        }

        private async Task HandleMessage(Message? m)
        {
            var commands = m.Text.Split(' ');
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var result = await (commands[0] switch
            {
                "/start" => HandleStartMessage(m, dbContext),
                "/time" => HandleTimeMessage(m, dbContext),
                "/currency" => HandleCurrencyMessage(m, dbContext),
                "/add" => HandleFindTokenMessage(m, dbContext),
                "/remove" => HandleRemoveToken(m, dbContext),
                _ => DefaultTextHandler(m),
            });


            if (string.IsNullOrEmpty(result.Value))
                return;

            await _botClient.SendTextMessageAsync(m.From.Id, result.Value);

        }

        private async Task<Result<string>> HandleRemoveToken(Message m, ApplicationContext dbContext)
        {
            var texts = m.Text.Split(' ');
            var userId = m.From.Id;
            
            if (texts.Length != 2)
            {
                return TextConstant.CommandWasNotRecognized.ToErrorMethodResult();
            }

            var user = dbContext.Users.Include(t => t.PostInfo).FirstOrDefault(t => t.TelegramId == userId);
            if (user is null)
                return "User doesn't exist".ToErrorMethodResult();

            var cryptoAsset = texts[1];
            var isRemoved = user.PostInfo.RemoveCryptoAsset(cryptoAsset);
            await dbContext.SaveChangesAsync();

            return "Token was removed".ToSuccessMethodResult(); ;
        }

        private async Task<Result<string>> HandleFindTokenMessage(Message m, ApplicationContext dbContext)
        {
            var texts = m.Text.Split(' ');
            var userId = m.From.Id;

            if (texts.Length != 2)
            {
                return TextConstant.CommandWasNotRecognized.ToErrorMethodResult();
            }
            var cryptoAsset = texts[1];
            var fullInfo = await _cryptoCurrencyService.GetTokenInfo(cryptoAsset);
            if (fullInfo is null)
            {
                return "This token doesn't exist.".ToErrorMethodResult();
            }

            var user = dbContext.Users.Include(t => t.PostInfo).FirstOrDefault(t => t.TelegramId == userId);
            if (user is null)
                return "User doesn't exist".ToErrorMethodResult();

            user.PostInfo.AddCryptoAsset(cryptoAsset);

            await dbContext.SaveChangesAsync();

            return "Crypto active was added.".ToSuccessMethodResult();

        }

        private async Task<Result<string>> HandleCurrencyMessage(Message m, ApplicationContext dbContext)
        {
            var text = m.Text.Split(" ");
            var userId = m.From.Id;

            if (text.Length != 2)
            {
                return TextConstant.CommandWasNotRecognized.ToErrorMethodResult();
            }

            var value = text[1];
            var isValidCurrency = DefaultCryptoList.CurrencyList.Contains(value);
            if (!isValidCurrency)
            {
                return "Currency isn't valid".ToErrorMethodResult();
            }

            var info = await dbContext.UserPostsInfo.FirstAsync(t => t.UserId == userId);
            info.Currency = value;
            await dbContext.SaveChangesAsync();

            return string.Empty.ToSuccessMethodResult();
        }
        private async Task<Result<string>> HandleStartMessage(Message? m, ApplicationContext dbContext)
        {

            var userId = m.From.Id;
            var hasUser = await dbContext.Users.Where(t => t.TelegramId == userId).GetFirstOrDefaultASync();
            if (hasUser != null)
                return "You are already in the system.".ToErrorMethodResult();

            var user = new DAL.Models.User
            {
                TelegramId = userId,
                Name = $"{m.From.FirstName} {m.From.LastName}"
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            await _botClient.SendTextMessageAsync(userId, "Welcome to the Crypto Bot.");
            await _botClient.SendTextMessageAsync(userId, "For set a certain time. Write /time 100 (in seconds max 864000 seconds)");

            return string.Empty.ToSuccessMethodResult();
        }
        private async Task<Result<string>> HandleTimeMessage(Message? m, ApplicationContext dbContext)
        {
            var text = m.Text.Split(" ");
            var userId = m.From.Id;

            if (text.Length != 2)
            {
                return TextConstant.CommandWasNotRecognized.ToErrorMethodResult();
            }

            var value = text[1];
            var isValidate = _periodValidator.TryValidate(value, out int period);
            if (!isValidate)
            {
                return "Period isn't valid".ToErrorMethodResult();
            }

            var userPostInfo = dbContext.UserPostsInfo.Where(t => t.UserId == userId).FirstOrDefault();
            if (userPostInfo == null)
            {
                var userInfo = new UserPostInfo
                {
                    UserId = userId,
                    Timer = period,
                    CryptoSet = string.Join(';', DefaultCryptoList.CryptoList),
                };
                await dbContext.UserPostsInfo.AddAsync(userInfo);
            }
            else
            {
                userPostInfo.Timer = period;
            }

            await dbContext.SaveChangesAsync();

            return "Time setted".ToSuccessMethodResult();
        }

        private async Task<Result<string>> DefaultTextHandler(Message? m)
        {
            return null;
        }
    }
}
