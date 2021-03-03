using CaptchaBot.Application.CaptchaWorkers;
using CaptchaBot.Application.Interfaces;
using CaptchaBot.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace CaptchaBot.Application
{
    public static class Configure
    {
        public static void AddApplication(this IServiceCollection services, AppSettings settings)
        {
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(settings.BotToken));
            services.AddServices();
            services.AddCaptcha();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<TelegramService>();
            services.AddSingleton<IUserService, UserService>();
        }

        private static void AddCaptcha(this IServiceCollection services)
        {
            services.AddTransient<ICaptcha, NumberSelectionCaptcha>();
        }
    }
}