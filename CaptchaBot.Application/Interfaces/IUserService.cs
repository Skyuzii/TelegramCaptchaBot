using System.Collections.Generic;
using CaptchaBot.Application.Models;

namespace CaptchaBot.Application.Interfaces
{
    public interface IUserService
    {
        void Add(TelegramUser telegramUser);

        TelegramUser? Get(long chatId, long telegramId);
        
        IEnumerable<TelegramUser> GetAll();

        void Remove(TelegramUser telegramUser);
    }
}