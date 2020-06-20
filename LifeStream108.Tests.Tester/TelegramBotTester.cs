using LifeStream108.Modules.SettingsManagement;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace LifeStream108.Tests.Tester
{
    internal class TelegramBotTester
    {
        private static TelegramBotClient _botClient = null;

        public TelegramBotTester()
        {
            _botClient = new TelegramBotClient(SettingsManager.GetSettingEntryByCode(SettingCode.TelegramBotToken).Value);
            var botUser = _botClient.GetMeAsync().Result;
            Console.WriteLine($"Hello, World! I am user {botUser.Id} and my name is {botUser.FirstName}.");
            _botClient.StartReceiving();
            //_botClient.OnMessage += BotClient_OnMessage;
            _botClient.OnUpdate += _botClient_OnUpdate;
        }

        private async void _botClient_OnUpdate(object sender, UpdateEventArgs e)
        {
            var keyboard = CreateTelegramKeyboard();
            await _botClient.SendTextMessageAsync("302115880", "Text", ParseMode.Default, false, false, 0, keyboard);
        }

        private static InlineKeyboardMarkup CreateTelegramKeyboard()
        {
            InlineKeyboardButton[] row1 = new InlineKeyboardButton[2];
            row1[0] = InlineKeyboardButton.WithCallbackData("Item 1", "i1");
            row1[1] = InlineKeyboardButton.WithCallbackData("Item 2", "i2");

            InlineKeyboardButton[] row2 = new InlineKeyboardButton[1];
            row2[0] = InlineKeyboardButton.WithCallbackData("Item 3", "i3");

            InlineKeyboardButton[][] rows = new InlineKeyboardButton[2][];
            rows[0] = row1;
            rows[1] = row2;
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(rows);
            return keyboard;
        }

        private static void BotClient_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("OnMessage");
            try
            {
                string userLogin = !string.IsNullOrEmpty(e.Message.From.Username) ? $"({e.Message.From.Username})" : "";
                string userName = $"{e.Message.From.FirstName} {e.Message.From.LastName}{userLogin}";
                Console.WriteLine($"[{e.Message.MessageId}] Received message '{e.Message.Text}' from {userName} [{e.Message.From.Id}]");

                string messageToSend;
                if (e.Message.Type == MessageType.Text)
                {
                    messageToSend = "Вы сказали: " + e.Message.Text;
                }
                else
                {
                    Console.WriteLine($"[{e.Message.MessageId}] Message with unsupported type '{e.Message.Type}' received");
                    messageToSend = "Извините, я общаюсь только используя слова";
                }

                Console.WriteLine($"[{e.Message.MessageId}] Sending answer: " + messageToSend);
                SendMessage(e.Message.From.Id.ToString(), messageToSend);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error to read telegram message: " + ex);
            }
        }

        private static async void SendMessage(string recipient, string message)
        {
            await _botClient.SendTextMessageAsync(recipient, message, ParseMode.Html);
        }
    }
}
