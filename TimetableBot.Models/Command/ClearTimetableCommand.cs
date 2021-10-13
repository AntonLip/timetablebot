using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TimetableBot.Models.DTOModels;
using TimetableBot.Models.Interface;

namespace TimetableBot.Models.Command
{
    public class ClearTimetableCommand : ICommand
    {
        private readonly ITimetableService _timetableService;
        public ClearTimetableCommand(ITimetableService timetableService)
        {
            _timetableService = timetableService;
        }
        public string Name => @"/clear";

        public bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var commandParams = message.Text.Split(' ');
            if (commandParams.Length == 1)
                await client.SendTextMessageAsync(chatId, "This command not for you", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            else if (commandParams.Length == 2)
            {
                if (commandParams[1] == "Liplianin")
                {
                    try
                    {
                        _timetableService.DeleteTimetable();
                        await client.SendTextMessageAsync(chatId, "Database is clear", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        return;
                    }
                    catch (Exception ex)
                    {
                        await client.SendTextMessageAsync(chatId, ex.Message, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                }
                await client.SendTextMessageAsync(chatId, "This command not for you", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }



        }

    }
}

