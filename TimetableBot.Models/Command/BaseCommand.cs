using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using TimetableBot.Models.Interface;

namespace TimetableBot.Models
{
    public abstract class BaseCommand : ICommand
    {
        public string Name { get; }

        public abstract Task Execute(Message message, TelegramBotClient client);

        public abstract bool Contains(Message message);

    }
}
