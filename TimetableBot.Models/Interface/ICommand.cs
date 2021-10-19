using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TimetableBot.Models.Interface
{
    public interface ICommand
    {
        public string Name { get; }
        Task Handle(Message message, CallbackQuery query, TelegramBotClient client);
        Task Execute(Message message, CallbackQuery query, TelegramBotClient client);
        bool Contains(Message message);
    }
}
