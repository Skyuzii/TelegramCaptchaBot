using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaptchaBot.Application;
using CaptchaBot.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace CaptchaBot.Api.Workers
{
    public class BanHostedService : IHostedService
    {
        private Timer _timer;
        private readonly AppSettings _settings;
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _telegramBot;
        private readonly ILogger<BanHostedService> _logger;

        public BanHostedService(
            AppSettings settings,
            IUserService userService,
            ILogger<BanHostedService> logger,
            ITelegramBotClient telegramBot)
        {
            _settings = settings;
            _userService = userService;
            _logger = logger;
            _telegramBot = telegramBot;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async x => await BanSlowUsers(), null, 0, 10000);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async Task BanSlowUsers()
        {
            foreach (var newUser in _userService
                .GetAll()
                .Where(x => DateTimeOffset.Now - x.JoinDateTime > _settings.ProcessEventTimeout))
            {
                await _telegramBot.KickChatMemberAsync(newUser.ChatId, (int) newUser.UserId, DateTime.Now.AddDays(1));
                await _telegramBot.DeleteMessageAsync(newUser.ChatId, newUser.InviteMessageId);
                await _telegramBot.DeleteMessageAsync(newUser.ChatId, newUser.JoinMessageId);
                _userService.Remove(newUser);

                _logger.LogInformation($"Пользователь {newUser.UserId} with name {newUser.PrettyUserName} был забанен после {_settings.ProcessEventTimeout.ToString()} ожидания.");
            }
        }
    }
}