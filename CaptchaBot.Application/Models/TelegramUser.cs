using System;
using Telegram.Bot.Types;

namespace CaptchaBot.Application.Models
{
    public class TelegramUser
    {
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public string PrettyUserName { get; set; }
        public int InviteMessageId { get; set; }
        public int JoinMessageId { get; set; }
        public object Answer { get; set; }
        public DateTimeOffset JoinDateTime { get; set; } = DateTimeOffset.Now;

        public TelegramUser(
            long chatId,
            long userId,
            int inviteMessageId,
            int joinMessageId,
            string prettyUserName,
            object answer)
        {
            ChatId = chatId;
            UserId = userId;
            InviteMessageId = inviteMessageId;
            JoinMessageId = joinMessageId;
            PrettyUserName = prettyUserName;
            Answer = answer;
        }
    }
}