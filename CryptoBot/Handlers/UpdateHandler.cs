using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CryptoBot.Handlers
{
    public partial class UpdateHandler : IUpdateHandler
    {
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await (update.Type switch
                {
                    UpdateType.Message => HandleMessageByAi(update.Message),
                    _ => Task.CompletedTask
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToString());
            return Task.CompletedTask;
        }

        public UpdateType[] AllowedUpdates => new[] { UpdateType.Message };
    }
}
