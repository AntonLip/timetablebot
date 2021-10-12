using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using TimetableBot.Models.Command;
using TimetableBot.Models.Interface;

namespace TimetableBot.Models
{
    public class Bot :IBot
    {
        private readonly TelegramBotClient botClient;
        private  List<ICommand> commandsList;
        private readonly string _botKey;
        private readonly string _botUrl;
        private readonly string _botName;
        public Bot(IOptions<BotSettings> options)
        {
            _botName = options.Value.BotName;
            _botKey = options.Value.Token;
            _botUrl = options.Value.Url;
            botClient = new TelegramBotClient(_botKey);
            commandsList = new List<ICommand>();
            commandsList.Add(new StartCommand());
        }

        public  async Task<TelegramBotClient> GetBotClientAsync()
        {
            
            string hook = _botUrl + "api/bot";
            await botClient.SetWebhookAsync(hook);
            return botClient;
        }

        public List<ICommand> GetCommands()
        {
            if (commandsList is null)
                throw new System.Exception();
            return commandsList;
        }
    }
}
