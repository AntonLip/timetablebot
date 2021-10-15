using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TimetableBot.Models.DTOModels;
using TimetableBot.Models.Interface;

namespace TimetableBot.Models.Command
{
    public class TimetableCommand : BaseCommand, ICommand
    {
        public TimetableCommand(ITimetableService timetableService)
            :base(timetableService)
        {
        }
        public string Name => @"/get";        

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new NotImplementedException();
        }
    }
}

