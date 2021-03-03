using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaptchaBot.Application.Interfaces;
using CaptchaBot.Application.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CaptchaBot.Application.CaptchaWorkers
{
    public class NumberSelectionCaptcha : ICaptcha
    {
        private const int ButtonsCount = 8;
        private readonly AppSettings _settings;
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _telegramBot;
        private readonly ILogger<NumberSelectionCaptcha> _logger;
        
        public NumberSelectionCaptcha(
            IUserService userService,
            AppSettings settings,
            ILogger<NumberSelectionCaptcha> logger, ITelegramBotClient telegramBot)
        {
            _userService = userService;
            _settings = settings;
            _logger = logger;
            _telegramBot = telegramBot;
        }

        public string Name => CaptchaTypes.NUMBER_SELECTION;

        public bool IsCallback(Update update) => update.Type == UpdateType.CallbackQuery;

        public async Task ProcessNewChatMember(Update update)
        {
            var message = update.Message;
            var freshness = DateTime.UtcNow - message.Date.ToUniversalTime();
            if (freshness > _settings.ProcessEventTimeout)
            {
                _logger.LogInformation($"Сообщение о {message.NewChatMembers.Length} получено {freshness} назад и проигнорировано");
                return;
            }

            foreach (var unauthorizedUser in message.NewChatMembers)
            { 
                await _telegramBot.RestrictChatMemberAsync(
                    message.Chat.Id,
                    unauthorizedUser.Id,
                    new ChatPermissions
                    {
                        CanAddWebPagePreviews = false,
                        CanChangeInfo = false,
                        CanInviteUsers = false,
                        CanPinMessages = false,
                        CanSendMediaMessages = false,
                        CanSendMessages = false,
                        CanSendOtherMessages = false,
                        CanSendPolls = false
                    },
                    DateTime.Now.AddDays(1));

                var answer = new Random().Next(1, ButtonsCount + 1);
                var prettyUserName = GetPrettyName(unauthorizedUser);

                var sendTextMessage = await _telegramBot.SendTextMessageAsync(
                        message.Chat.Id,
                        $"Привет, {prettyUserName}, нажми кнопку {answer}, чтобы тебя не забанили!",
                        replyToMessageId: message.MessageId,
                        replyMarkup: new InlineKeyboardMarkup(GetKeyboardButtons()));

                var telegramUser = new TelegramUser(
                    message.Chat.Id,
                    message.From.Id,
                    sendTextMessage.MessageId,
                    message.MessageId,
                    GetPrettyName(message.From),
                    answer);

                _userService.Add(telegramUser);
                _logger.LogInformation(
                    $"Новый пользователь {unauthorizedUser.Id} с именем {prettyUserName} Обнаружен и испытан. " +
                    $"У него есть {_settings.ProcessEventTimeout.ToString()}, чтобы ответить.");
            }
        }

        public async Task ProcessCallback(Update update)
        {
            var query = update.CallbackQuery;
            var chatId = query.Message.Chat.Id;
            var userId = query.From.Id;
            
            var unauthorizedUser = _userService.Get(chatId, userId);
            if (unauthorizedUser == null)
            {
                _logger.LogInformation($"Пользователь с id {userId} не найден");
                return;
            }

            var unauthorizedUserAnswer = int.Parse(query.Data);
            if (unauthorizedUserAnswer != (int)unauthorizedUser.Answer)
            {
                await _telegramBot.KickChatMemberAsync(
                    chatId,
                    query.From.Id,
                    DateTime.Now.AddDays(1));

                _logger.LogInformation(
                    $"Пользователь {userId} с ником {unauthorizedUser.PrettyUserName} был забанен после ввода неправильного ответа {unauthorizedUserAnswer}, " +
                    $"в то время как правильный это - {unauthorizedUser.Answer}.");
            }
            else
            {
                var defaultPermissions = (await _telegramBot.GetChatAsync(chatId)).Permissions;
                var postBanPermissions = new ChatPermissions
                {
                    CanAddWebPagePreviews = defaultPermissions?.CanAddWebPagePreviews ?? true,
                    CanChangeInfo = defaultPermissions?.CanChangeInfo ?? true,
                    CanInviteUsers = defaultPermissions?.CanInviteUsers ?? true,
                    CanPinMessages = defaultPermissions?.CanPinMessages ?? true,
                    CanSendMediaMessages = defaultPermissions?.CanSendMediaMessages ?? true,
                    CanSendMessages = defaultPermissions?.CanSendMessages ?? true,
                    CanSendOtherMessages = defaultPermissions?.CanSendOtherMessages ?? true,
                    CanSendPolls = defaultPermissions?.CanSendPolls ?? true
                };

                await _telegramBot.RestrictChatMemberAsync(
                    chatId,
                    query.From.Id,
                    postBanPermissions);

                _logger.LogInformation($"Пользователь {unauthorizedUser.UserId} с именем {unauthorizedUser.PrettyUserName} был авторизован с ответом {unauthorizedUser.Answer}.");
            }

            await _telegramBot.DeleteMessageAsync(unauthorizedUser.ChatId, unauthorizedUser.InviteMessageId);
            await _telegramBot.DeleteMessageAsync(unauthorizedUser.ChatId, unauthorizedUser.JoinMessageId);
            _userService.Remove(unauthorizedUser);
        }

        #region Helpers

        private static string GetPrettyName(User messageNewChatMember)
        {
            var names = new[] 
            {   messageNewChatMember.FirstName ,
                messageNewChatMember.LastName,
                $"(@{messageNewChatMember.Username})" 
            };
            
            return string.Join(" ", names.Where(x => !string.IsNullOrWhiteSpace(x)));
        }
        
        private static IEnumerable<InlineKeyboardButton> GetKeyboardButtons()
        {
            return Enumerable
                .Range(1, ButtonsCount)
                .Select(x => InlineKeyboardButton.WithCallbackData(x.ToString(), x.ToString()));
        }

        #endregion
    }
}