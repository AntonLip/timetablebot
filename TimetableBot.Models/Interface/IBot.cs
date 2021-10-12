
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TimetableBot.Models.Interface
{
    public interface IBot
    {
        Task<TelegramBotClient> GetBotClientAsync();
        List<ICommand> GetCommands();
    }
}
