using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace CryptoBot
{
    public class TelegramHostedService : IHostedService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUpdateHandler _handler;
        public TelegramHostedService(ITelegramBotClient botClient, IUpdateHandler handler)
        {
            _botClient = botClient;
            _handler = handler;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var bot = await _botClient.GetMeAsync();
                Console.WriteLine(bot.FirstName, "Bot ready to work!");

                _botClient.ReceiveAsync(_handler, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                Console.Write("Some Exception: ", ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
