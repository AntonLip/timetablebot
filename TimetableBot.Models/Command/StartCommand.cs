using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TimetableBot.Models.Interface;

namespace TimetableBot.Models.Command
{
    public class StartCommand : BaseCommand, ICommand
    {
        public string Name => @"/start";

        public StartCommand( ITimetableService timetableService)
            :base(timetableService)
        {

        }
        public override bool Contains(Message message)
        {
            if (message is null)
                return false;
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, CallbackQuery query, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var list = new List<List<KeyboardButton>>();
            var buttons = new List<KeyboardButton>();
            buttons.Add(new KeyboardButton("/students"));
            buttons.Add(new KeyboardButton("/lectural"));
            list.Add(buttons);
            var markup = new ReplyKeyboardMarkup(list, true);
            await client.SendTextMessageAsync(chatId, "Для кого необходимо расписание", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,replyMarkup: markup);
        }       

        public override Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new System.NotImplementedException();
        }
    }
}
