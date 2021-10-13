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
       // private  TelegramBotClient botClient;
        private  List<ICommand> commandsList;
        private readonly ITimetableService _timetableService;
        //private readonly string _botKey;
        //private readonly string _hookUrl;
        //private readonly string _botName;
        //IOptions<BotSettings> options, 
        public Bot(ITimetableService timetableService)
        {
            _timetableService = timetableService;
            //_botName = options.Value.BotName;
            //_botKey = options.Value.Token;
            //botClient = new TelegramBotClient(_botKey);
            commandsList = new List<ICommand>();
            commandsList.Add(new StartCommand());
            commandsList.Add(new TimetableCommand(_timetableService));
            commandsList.Add(new ClearTimetableCommand(_timetableService));
            //string hook = options.Value.Url + "api/bot";
            //SetHookToBot();

        }
        //private async Task  SetHookToBot()
        //{
        //     await botClient.SetWebhookAsync(_hookUrl);
        //}

        //public  TelegramBotClient GetBotClientAsync()
        //{         
        //    return botClient;
        //}

        public List<ICommand> GetCommands()
        {
            if (commandsList is null)
                throw new System.Exception();
            return commandsList;
        }
    }
}
