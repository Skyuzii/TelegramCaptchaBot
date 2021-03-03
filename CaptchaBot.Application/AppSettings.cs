using System;
using CaptchaBot.Application.Models;

namespace CaptchaBot.Application
{
    public class AppSettings
    {
        public string BotToken { get; set; }
        
        public string WebHookAddress { get; set; }

        public string CaptchaType { get; set; } = CaptchaTypes.NUMBER_SELECTION;

        public TimeSpan ProcessEventTimeout { get; set; } = TimeSpan.FromMinutes(1);
    }
}