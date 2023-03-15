using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;

namespace CryptoBot.Handlers
{
    public partial class UpdateHandler
    {


        private async Task HandleMessageByAi(Message? m)
        {
            var completionResult = await _openAI.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                MaxTokens = 2048,
                Prompt = m.Text,
                Model = "gpt-3.5-turbo",
                User = m.From.Username,
            });

            if (completionResult.Successful)
            {
                await _botClient.SendTextMessageAsync(m.From.Id, completionResult.Choices.Single().Text) ;
            }
            else
            {
                if (completionResult.Error == null)
                {
                    await _botClient.SendTextMessageAsync(m.From.Id, "Unknown Error");
                }
                await _botClient.SendTextMessageAsync(m.From.Id, $"{completionResult.Error.Code}: {completionResult.Error.Message}");
                Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
            }
        }
    }
}
