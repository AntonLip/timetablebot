using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TimetableBot.Models.Interface;

namespace TimetableBot.Models.Command
{
    public class StudentCommand : BaseCommand, ICommand
    {

        public StudentCommand(ITimetableService timetableService)
            : base(timetableService)
        {
        }
        public string Name => @"/students";


        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, CallbackQuery query, TelegramBotClient client)
        {
            long chatId = 0;
            string[] filterParameter = null;
            if (!(query is null))
            {
                await base.Handle(message, query, client);
            }
            else
            {
                chatId = message.Chat.Id;
                filterParameter = message.Text.Split(' ');
                InlineKeyboardButton gr442 = new InlineKeyboardButton();
                gr442.CallbackData = @"/students gr 442";
                gr442.Text = @"442";
                InlineKeyboardButton gr443 = new InlineKeyboardButton();
                gr443.CallbackData = @"/students gr 443";
                gr443.Text = @"443";
                InlineKeyboardButton gr444 = new InlineKeyboardButton();
                gr444.CallbackData = @"/students gr 444";
                gr444.Text = @"444";
                InlineKeyboardButton gr434 = new InlineKeyboardButton();
                gr434.CallbackData = @"/students gr 434";
                gr434.Text = @"434";
                InlineKeyboardButton gr435g = new InlineKeyboardButton();
                gr435g.CallbackData = @"/students gr 435Г";
                gr435g.Text = @"434g";
                List<List<InlineKeyboardButton>> inlineKeyboardButtons = new List<List<InlineKeyboardButton>>();
                var list = new List<InlineKeyboardButton>();
                list.Add(gr442);
                list.Add(gr443);
                list.Add(gr444);
                list.Add(gr435g);
                list.Add(gr434);
                inlineKeyboardButtons.Add(list);
                InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);
                await client.SendTextMessageAsync(chatId, "Выберите группу", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
            }
        }
    }
}

