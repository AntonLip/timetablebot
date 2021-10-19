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
    public class ClearTimetableCommand : BaseCommand, ICommand
    {
        public ClearTimetableCommand(ITimetableService timetableService)
            :base(timetableService)
        {
        }
        public string Name => @"/clear";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, CallbackQuery query, TelegramBotClient client)
        {
            await Handle(message, query, client);
        }

        public override async Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
           await _timetableService.DeleteTimetable();
        }

       

    }
}

