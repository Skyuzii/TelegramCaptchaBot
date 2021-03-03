using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaptchaBot.Application.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CaptchaBot.Application.Services
{
    public class TelegramService
    {
        private readonly ICaptcha _captcha;

        public TelegramService(AppSettings settings, IEnumerable<ICaptcha> captchaWorkers)
        {
            _captcha = captchaWorkers.First(x => x.Name == settings.CaptchaType);
        }
        
        public async Task Handle(Update? update)
        {
            if (update == null) return;
            
            if (update.Message?.Type == MessageType.ChatMembersAdded)
            {
                await _captcha.ProcessNewChatMember(update);
            }
            else if (_captcha.IsCallback(update))
            {
                await _captcha.ProcessCallback(update);
            }
        }
    }
}