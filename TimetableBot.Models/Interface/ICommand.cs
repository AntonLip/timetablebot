using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TimetableBot.Models.Interface
{
    public interface ICommand
    {
        public string Name { get; }

        public abstract Task Execute(Message message, TelegramBotClient client);

        public abstract bool Contains(Message message);
    }
}
