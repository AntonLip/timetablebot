
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
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
        private TelegramBotClient _botClient;
        private readonly List<ICommand> _commands;
        public BotController(IOptions<BotSettings> options, IBot bot)
        {
            _botClient = new TelegramBotClient(options.Value.Token);
            _commands = bot.GetCommands();
        }

        [HttpPost]
        public async Task<OkResult> Post([FromBody] Update update)
        {

            if (update is null)
                return Ok();
           
            Message message;

            if (update.Message is null)
            {
                var callback = update.CallbackQuery;
                if (callback is null)
                    return Ok();
                message = callback.Message;
                //message.Type = Telegram.Bot.Types.Enums.MessageType.Text;
                message.Text = callback.Data;
            }
            else
                message = update.Message;
            
            foreach (var command in _commands)
            {
                if (command.Contains(message))
                {
                    try
                    {
                        await command.Execute(message, update.CallbackQuery, _botClient);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return Ok();
        }
    }
}
