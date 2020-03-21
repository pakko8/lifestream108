using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Common.Grammar;
using LifeStream108.Libs.Entities.MessageEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Libs.Entities.TicketEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.TempDataManagement.Managers;
using LifeStream108.Modules.UserManagement.Managers;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using OurUser = LifeStream108.Libs.Entities.UserEntities.User;
using TelegramUser = Telegram.Bot.Types.User;

namespace LifeStream108.Modules.TelegramBotManager
{
    public class MainTelegramChatClient : BaseTelegramBotClient
    {
        private bool _botPaused = false;

        public override void Start()
        {
            base.Start();
            _botClient.OnMessage += BotClient_OnMessage;
            _botClient.OnCallbackQuery += BotClient_OnCallbackQuery;
            //_botClient.OnUpdate += BotClient_OnUpdate;
            _botClient.OnInlineQuery += BotClient_OnInlineQuery;
            _botClient.OnInlineResultChosen += BotClient_OnInlineResultChosen;
            _botClient.OnReceiveError += BotClient_OnReceiveError;
            _botClient.OnReceiveGeneralError += BotClient_OnReceiveGeneralError;
        }

        #region Delete messages
        private async Task<string> DeleteMessages(int userId)
        {
            // Step 1. Get messages for this user from database
            TelegramMessageEntry[] telegramMessages;
            try
            {
                telegramMessages = TelegramMessageEntryManager.GetEntriesForUser(userId);
            }
            catch (Exception ex)
            {
                Logger.Error("Error loading telegram messages: " + ex);
                return "Не удалось загрузить историю сообщений";
            }

            if (telegramMessages.Length == 0) return "Не найдены сообщения для удаления";
            // Step 2. Delete messages in Telegram
            int countErrors = 0;
            foreach (TelegramMessageEntry message in telegramMessages)
            {
                try
                {
                    await _botClient.DeleteMessageAsync(message.ChatId, message.MessageId);
                }
                catch (Exception ex)
                {
                    if (countErrors < 5) Logger.Error("Error to delete message in Telegram: " + ex);
                    countErrors++;
                }
            }
            int countDeletedMessages = telegramMessages.Length - countErrors;
            Logger.Info($"In Telegram were deleted {countDeletedMessages} of {telegramMessages.Length} messages");

            string errorMessage = $"Удалено {countDeletedMessages} " +
                $"{Declanations.DeclineByNumeral(countDeletedMessages, "сообщение", "сообщения", "сообщений")}";

            // Step 3. Delete messages from database
            try
            {
                TelegramMessageEntryManager.DeleteEntries(telegramMessages);
            }
            catch (Exception ex)
            {
                Logger.Error("Error to delete messages from database: " + ex);
            }

            return errorMessage;
        }
        #endregion

        #region Save Telegram message entry
        private static void SaveMessageEntry(int telegramUserId, long chatId, int messageId)
        {
            if (telegramUserId == 0 || chatId == 0 || messageId == 0)
            {
                Logger.Error($"Error to save Telegram message <{messageId}> in chat <{chatId}> from user <{telegramUserId}> couse some value is zero");
                return;
            }

            try
            {
                TelegramMessageEntry entry = new TelegramMessageEntry
                {
                    TelegramUserId = telegramUserId,
                    ChatId = chatId,
                    MessageId = messageId
                };
                TelegramMessageEntryManager.AddEntry(entry);
            }
            catch (Exception ex)
            {
                Logger.Error("Error to save Telegram message: " + ex);
            }
        }
        #endregion

        private void BotClient_OnMessage(object sender, MessageEventArgs e)
        {
            SaveMessageEntry(e.Message.From.Id, e.Message.Chat.Id, e.Message.MessageId);
            ProcessRequest(
                e.Message.Text,
                e.Message.MessageId,
                e.Message.Type,
                UpdateType.Message,
                e.Message.From,
                e.Message.Chat.Id);
        }

        private void BotClient_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            SaveMessageEntry(e.CallbackQuery.From.Id, 0, e.CallbackQuery.Message.MessageId);
            ProcessRequest(
                e.CallbackQuery.Data,
                e.CallbackQuery.Message.MessageId,
                e.CallbackQuery.Message.Type,
                UpdateType.CallbackQuery,
                e.CallbackQuery.From,
                e.CallbackQuery.Message.Chat.Id);
        }

        #region Not implemented events
        private void BotClient_OnUpdate(object sender, UpdateEventArgs e)
        {
            Logger.Warn("OnUpdate request received");
        }

        private void BotClient_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            Logger.Warn("OnInlineQuery request received");
        }

        private void BotClient_OnInlineResultChosen(object sender, ChosenInlineResultEventArgs e)
        {
            Logger.Warn("OnInlineResultChosen request received");
        }

        private void BotClient_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            Logger.Warn("OnReceiveError request received");
        }

        private void BotClient_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            Logger.Warn("OnReceiveGeneralError request received");
        }
        #endregion

        private async void ProcessRequest(
            string userRequestText,
            int userRequestId,
            MessageType userRequestType,
            UpdateType telegramRequestType,
            TelegramUser tlgrmUser,
            long chatId)
        {
            if (userRequestType != MessageType.Text)
            {
                Logger.Warn($"[{tlgrmUser.Id}][{userRequestId}] Message with unsupported type '{userRequestType}' received");
                SendTelegramMessage("Это сообщение не поддерживается", tlgrmUser.Id, chatId, userRequestId);
                return;
            }

            string userRequestTextTemp = userRequestText.ToUpper();
            if (userRequestTextTemp == "PAUSE 123")
            {
                _botPaused = true;
                Logger.Info($"Bot paused with command <{userRequestText}>");
                return;
            }
            else if (userRequestTextTemp == "RESUME 123")
            {
                _botPaused = false;
                Logger.Info($"Bot resumed with command <{userRequestText}>");
            }

            if (_botPaused)
            {
                Logger.Info($"Request <{userRequestText}> will not processed because bot is paused");
                return;
            }

            string telegramUserInfo;
            try
            {
                string telegramUserLogin = !string.IsNullOrEmpty(tlgrmUser.Username) ? $"({tlgrmUser.Username})" : "NO_LOGIN";
                string telegramUserName = $"{tlgrmUser.FirstName} {tlgrmUser.LastName}".Trim();
                telegramUserInfo = $"name={telegramUserName}, login={telegramUserLogin}";
                Logger.Info($"[{tlgrmUser.Id}][{userRequestId}] Received message <{userRequestText}> of type <{userRequestType}> from user <{telegramUserInfo}>");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{tlgrmUser.Id}][{userRequestId}] Error to read request info: {ex}");
                SendTelegramMessage("Ошибка обработки вашего запроса", tlgrmUser.Id, chatId, userRequestId);
                return;
            }

            OurUser currentDbUser = null;
            try
            {
                Tuple<bool, string, OurUser> authResult = UserManager.AuthorizeUser(tlgrmUser.Id);
                if (authResult.Item1)
                {
                    currentDbUser = authResult.Item3;

                    Session currentSession = SessionManager.GetSessionForUser(currentDbUser.Id);
                    if (currentSession != null)
                    {
                        currentSession.LastActivityTime = DateTime.Now;
                        SessionManager.UpdateSession(currentSession);
                    }
                    else
                    {
                        currentSession = new Session();
                        currentSession.UserId = currentDbUser.Id;
                        currentSession.StartTime = DateTime.Now;
                        currentSession.LastActivityTime = DateTime.Now;
                        currentSession.LastRequestText = userRequestText;
                        SessionManager.AddSession(currentSession);
                        Logger.Info($"[{tlgrmUser.Id}][{userRequestId}] Created session <{currentSession.Id}>");
                    }

                    string messageToSend;
                    ChoiceItem[] choiceItems = null;
                    int choiceItemsColumnsCount = 0;

                    CommandExecutor commandExecututor = new CommandExecutor();
                    ExecuteCommandResult execCmdResult = commandExecututor.Run(userRequestText, currentSession);

                    Logger.Info("Command result:\r\n" + execCmdResult);

                    currentSession.Data = execCmdResult.Success ? execCmdResult.SessionData : currentSession.Data;
                    if (execCmdResult.NeedUpdateCommandId) currentSession.LastCommandId = execCmdResult.CommandId;
                    if (execCmdResult.NeedUpdateLifeGroupId) currentSession.LastLifeGroupId = execCmdResult.LifeGroupId;
                    if (execCmdResult.NeedUpdateLifeActivityId) currentSession.LastLifeActivityId = execCmdResult.LifeActivityId;
                    currentSession.LastActivityTime = DateTime.Now;
                    SessionManager.UpdateSession(currentSession);

                    if (execCmdResult.Success)
                    {
                        switch (execCmdResult.SpecialCommandForTelegramBot)
                        {
                            case SpecialCommandForTelegramBot.None:
                                messageToSend = execCmdResult.ResponseMessage;
                                choiceItems = execCmdResult.ChoiceItemList;
                                choiceItemsColumnsCount = execCmdResult.ChoiceListShowColumnsCount;
                                break;
                            case SpecialCommandForTelegramBot.ClearChat:
                                messageToSend = await DeleteMessages(tlgrmUser.Id);
                                break;
                            default:
                                throw new LifeStream108Exception(ErrorType.SpecialTelegramCommandNotImplemented,
                                    $"Special command <{execCmdResult.SpecialCommandForTelegramBot}> not implementd",
                                    "Ошибка реализации команды", userRequestText);
                        }
                    }
                    else
                    {
                        messageToSend = execCmdResult.ErrorText;
                    }

                    SendTelegramMessage(messageToSend, tlgrmUser.Id, chatId, userRequestId, choiceItems, choiceItemsColumnsCount);
                }
                else
                {
                    Logger.Warn($"[{tlgrmUser.Id}][{userRequestId}] Auth failed: {authResult.Item2}");
                    SendTelegramMessage(authResult.Item2, tlgrmUser.Id, chatId, userRequestId);
                }
            }
            catch (LifeStream108Exception ls108Ex)
            {
                ProcessLifeStream108Exception(ls108Ex, tlgrmUser.Id, currentDbUser);
            }
            catch (DateNotInCorrectFormat dateEx)
            {
                Logger.Warn("Date parse error: " + dateEx.Message);
                SendTelegramMessage(dateEx.Message, tlgrmUser.Id, chatId, userRequestId);
            }
            catch (Exception ex)
            {
                Logger.Error($"[{tlgrmUser.Id}][{userRequestId}] Error to process user <{telegramUserInfo}> request <{userRequestText}>: {ex}");
                SendTelegramMessage("Ошибка обработки", tlgrmUser.Id, chatId, userRequestId);
            }
        }

        private async void SendTelegramMessage(string messageToSend, int telegramUserId, long chatId, int userRequestId,
            ChoiceItem[] choiceItems = null, int choiceItemsColumnsCount = 0)
        {
            try
            {
                IReplyMarkup keyboard = null;
                if (choiceItems != null)
                {
                    keyboard = CreateKeyboard(choiceItems, choiceItemsColumnsCount);
                }
                Logger.Info("Message length: " + messageToSend.Length);
                Logger.Info($"[{telegramUserId}][{userRequestId}] Sending message: " + messageToSend);
                Message sentMessageObject = await _botClient.SendTextMessageAsync(
                    telegramUserId.ToString(), messageToSend, ParseMode.Html, false, false, 0, keyboard);
                Logger.Info("Message sent with id: " + sentMessageObject.MessageId);
                SaveMessageEntry(telegramUserId, chatId, sentMessageObject.MessageId);
            }
            catch (Exception ex)
            {
                Logger.Error("Error sending message: " + ex);
            }
        }

        #region ProcessLifeStream108Exception
        private async void ProcessLifeStream108Exception(LifeStream108Exception ls108ex, int telegramUserId, OurUser currentDbUser)
        {
            if (currentDbUser == null)
            {
                Logger.Error(ls108ex);
                return;
            }
            BugTicket bugTicket;
            try
            {
                bugTicket = new BugTicket
                {
                    ErrorType = ls108ex.ErrorType.ToString(),
                    UserId = currentDbUser.Id,
                    RequestDetails = ls108ex.UserRequestDetails,
                    ErrorMessage = ls108ex.UserMessage
                };
                BugTicketManager.AddBugTicket(bugTicket);
                Logger.Error($"Bug ticket #{bugTicket.Id} registered: {ls108ex}");
            }
            catch (Exception regTicketEx)
            {
                Logger.Error(regTicketEx);
                bugTicket = null;
            }

            if (bugTicket != null)
            {
                string errorMessageToSend = $"{ls108ex.UserMessage} Ошибка зарегистрирована с номером #{bugTicket.Id}. " +
                    "Ожидайте, пожалуйста, исправления. После исправления Вам должно прийти уведомление.";
                await _botClient.SendTextMessageAsync(telegramUserId.ToString(), errorMessageToSend, ParseMode.Html);
            }
            else
            {
                Logger.Error(ls108ex);
                await _botClient.SendTextMessageAsync(telegramUserId.ToString(), ls108ex.UserMessage, ParseMode.Html);
            }
        }
        #endregion

        #region Create keyboard
        private static IReplyMarkup CreateKeyboard(ChoiceItem[] choiceItemList, int columnsCount)
        {
            #region Test list
            /*
                ChoiceItem[] choiceItemList = new[]
                {
                    new ChoiceItem { Text = "1: 1234567890123456789012345678901234567890", Command = "Cmd1" },
                    new ChoiceItem { Text = "2: 1234567890", Command = "Cmd2" },
                    new ChoiceItem { Text = "3: 1234567890", Command = "Cmd3" },
                    new ChoiceItem { Text = "4: 12345678901234567890", Command = "Cmd4" },
                    new ChoiceItem { Text = "5: 12345678901234567890", Command = "Cmd5" },
                    new ChoiceItem { Text = "6: 12345678901234567890", Command = "Cmd6" },
                    new ChoiceItem { Text = "7: 12345", Command = "Cmd7" },
                    new ChoiceItem { Text = "8: 12345", Command = "Cmd8" },
                    new ChoiceItem { Text = "9: 12345", Command = "Cmd9" }
                };
            */
            #endregion

            if (columnsCount == 0) // Divide items by rows in which amount of columns depends on sum text length of each column
            {
                int maxSymbolsCountPerOneRow = 40;
                List<int> itemsPerRowList = new List<int>();

                int itemsCount = 0;
                int symbolsCount = 0;
                foreach (ChoiceItem dataItem in choiceItemList)
                {
                    itemsCount++;
                    symbolsCount += dataItem.Text.Length;
                    if (symbolsCount > maxSymbolsCountPerOneRow)
                    {
                        if (itemsCount == 1) // Значит, в этой строке поместится только один элемент
                        {
                            itemsPerRowList.Add(1);
                            itemsCount = 0;
                            symbolsCount = 0;
                        }
                        else // Здесь будет строка с одним или более элементами
                        {
                            // Последний же оставшийся элемент откинем и оставим для следущего прохода цикла
                            itemsPerRowList.Add(itemsCount - 1);
                            itemsCount = 1;
                            symbolsCount = dataItem.Text.Length;
                        }
                    }
                }
                if (itemsCount > 0) itemsPerRowList.Add(itemsCount); // Учтём оставшиеся элементы в последней строке

                InlineKeyboardButton[][] rows = new InlineKeyboardButton[itemsPerRowList.Count][];
                for (int rowIndex = 0, itemIndex = 0; rowIndex < itemsPerRowList.Count; rowIndex++)
                {
                    int itemsPerRow = itemsPerRowList[rowIndex];
                    InlineKeyboardButton[] row = new InlineKeyboardButton[itemsPerRow];
                    for (int i = 0; i < itemsPerRow; i++, itemIndex++)
                    {
                        row[i] = InlineKeyboardButton.WithCallbackData(choiceItemList[itemIndex].Text, choiceItemList[itemIndex].Command);
                    }
                    rows[rowIndex] = row;
                }
                return new InlineKeyboardMarkup(rows);
            }
            else // Divide items by rows with equal amoun of columns
            {
                int rowsCount = choiceItemList.Length / columnsCount;
                if (choiceItemList.Length % columnsCount != 0) rowsCount++;

                int elementIndex = 0;
                InlineKeyboardButton[][] rows = new InlineKeyboardButton[rowsCount][];
                for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                {
                    int diff = choiceItemList.Length - elementIndex;
                    int thisRowColumnsCount = diff >= columnsCount ? columnsCount : diff;

                    InlineKeyboardButton[] row = new InlineKeyboardButton[thisRowColumnsCount];
                    for (int i = 0; i < thisRowColumnsCount; i++)
                    {
                        row[i] = InlineKeyboardButton.WithCallbackData(choiceItemList[elementIndex].Text, choiceItemList[elementIndex].Command);
                        elementIndex++;
                    }
                    rows[rowIndex] = row;
                }
                return new InlineKeyboardMarkup(rows);
            }
        }
        #endregion
    }
}
