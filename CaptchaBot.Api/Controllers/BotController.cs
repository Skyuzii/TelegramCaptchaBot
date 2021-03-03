using System.Threading.Tasks;
using CaptchaBot.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace CaptchaBot.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BotController : ControllerBase
    {
        private readonly TelegramService _telegramService;

        public BotController(TelegramService telegramService)
        {
            _telegramService = telegramService;
        }
        
        [HttpGet]
        public string Check() => "OK";

        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] Update update)
        {
            await _telegramService.Handle(update);
            return Ok();
        }        
    }
}