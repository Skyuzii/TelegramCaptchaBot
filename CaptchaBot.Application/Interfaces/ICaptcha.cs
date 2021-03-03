using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CaptchaBot.Application.Interfaces
{
    public interface ICaptcha
    {
        string Name { get; }

        bool IsCallback(Update update);
        
        Task ProcessNewChatMember(Update update);

        Task ProcessCallback(Update update);
    }
}