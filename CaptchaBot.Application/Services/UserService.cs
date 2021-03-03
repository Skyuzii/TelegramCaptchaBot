using System.Collections.Concurrent;
using System.Collections.Generic;
using CaptchaBot.Application.Interfaces;
using CaptchaBot.Application.Models;

namespace CaptchaBot.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ConcurrentDictionary<(long chatId, long telegramId), TelegramUser> _users;

        public UserService()
        {
            _users = new ConcurrentDictionary<(long chatId, long telegramId), TelegramUser>();
        }
        
        public void Add(TelegramUser telegramUser)
        {
            _users.TryAdd((telegramUser.ChatId, telegramUser.UserId), telegramUser);
        }

        public TelegramUser? Get(long chatId, long telegramId)
        {
            return _users.TryGetValue((chatId, telegramId), out var user) 
                ? user 
                : null;
        }

        public IEnumerable<TelegramUser> GetAll() => _users.Values;

        public void Remove(TelegramUser telegramUser)
        {
            _users.TryRemove((telegramUser.ChatId, telegramUser.UserId), out _);
        }
    }
}