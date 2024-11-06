using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UtilityBot.Services;

namespace UtilityBot.Controllers
{
    public class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;

        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            switch (message.Text)
            {
                case "/start":

                    // Объект, представляющий кнопки
                    var buttons = new List<InlineKeyboardButton[]>();
                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($" Количество символов" , $"CalculatingTheNumberOfCharacters"),
                        InlineKeyboardButton.WithCallbackData($" Сумма чисел" , $"CalculateSumOfNumbers")
                    });

                    // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>  Бот - мини калькулятор.</b> {Environment.NewLine}", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));

                    break;
                default:

                    string userTextProcessingMethod = _memoryStorage.GetSession(message.Chat.Id).TextProcessingMethod;

                    if (userTextProcessingMethod == "CalculatingTheNumberOfCharacters")
                    {
                        await _telegramClient.SendTextMessageAsync(message.From.Id, $"Количество символов: {message.Text.Length} знаков", cancellationToken: ct);
                    }
                    else if (userTextProcessingMethod == "CalculateSumOfNumbers")
                    {
                        try
                        {
                            int[] numbers = message.Text.Split(' ').Select(int.Parse).ToArray();                          
                            int sum = numbers.Sum();

                            await _telegramClient.SendTextMessageAsync(message.From.Id, $"Сумма чисел: {sum}", cancellationToken: ct);
                        }
                        catch (Exception ex)
                        {
                            await _telegramClient.SendTextMessageAsync(message.From.Id, $"ошибка: {ex.Message}", cancellationToken: ct);
                        }
                    }
                    else 
                    {
                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Выберите метод обработки текста", cancellationToken: ct);
                    }
              break;
            }
        }
    }
}
