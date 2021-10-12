
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TimetableBot.Models;
using TimetableBot.Models.Interface;

namespace TimetableBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly TelegramBotClient _botClient;
        private readonly List<ICommand> _commands;
        public BotController(IBot bot)
        {
            _botClient = bot.GetBotClientAsync();
            _commands = bot.GetCommands();
        }

        [HttpPost]
        public async Task<OkResult> Post([FromBody] Update update)
        {
            if (update is null)
                return Ok();

            var message = update.Message;
            foreach (var command in _commands)
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, _botClient);
                    break;
                }
            }
            return Ok();
        }
    }
}
