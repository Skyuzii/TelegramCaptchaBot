using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaptchaBot.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace CaptchaBot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            var settings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(settings);

            services.AddApplication(settings);
        }

        public void Configure(IApplicationBuilder app, ITelegramBotClient telegramBot, AppSettings settings, IWebHostEnvironment env)
        {
            telegramBot.SetWebhookAsync($"{settings.WebHookAddress}/api/bot");
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
