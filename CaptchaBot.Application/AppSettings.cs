using System;
using CaptchaBot.Application.Models;

namespace CaptchaBot.Application
{
    public class AppSettings
    {
        /// <summary>
        /// Токен бота
        /// </summary>
        public string BotToken { get; set; }
        
        /// <summary>
        /// Адрес вебхука
        /// </summary>
        public string WebHookAddress { get; set; }

        /// <summary>
        /// Тип капчи
        /// </summary>
        public string CaptchaType { get; set; } = CaptchaTypes.NUMBER_SELECTION;

        /// <summary>
        /// Время на разгадывание капчи
        /// </summary>
        public TimeSpan ProcessEventTimeout { get; set; } = TimeSpan.FromMinutes(1);
        
        /// <summary>
        /// Время бана
        /// </summary>
        public TimeSpan BanTime { get; set; } = TimeSpan.FromDays(1);
    }
}