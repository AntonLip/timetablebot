using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TimetableBot.Models.Interface;

namespace TimetableBot.Models.Command
{
    public class LecturalCommand : BaseCommand, ICommand
    {

        public LecturalCommand(ITimetableService timetableService)
            : base(timetableService)
        {
        }
        public new string Name => @"/lectural";


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
                InlineKeyboardButton perhovaButton = new InlineKeyboardButton();
                perhovaButton.CallbackData = @"/lectural L Перхова";
                perhovaButton.Text = @"Перхова";
                InlineKeyboardButton paltsevButton = new InlineKeyboardButton();
                paltsevButton.CallbackData = @"/lectural L Пальцев";
                paltsevButton.Text = @"Пальцев";
                InlineKeyboardButton liplButton = new InlineKeyboardButton();
                liplButton.CallbackData = @"/lectural L Липлянин";
                liplButton.Text = @"Липлянин";
                List<List<InlineKeyboardButton>> inlineKeyboardButtons = new List<List<InlineKeyboardButton>>();
                var list = new List<InlineKeyboardButton>();
                list.Add(perhovaButton);
                list.Add(paltsevButton);
                list.Add(liplButton);
                inlineKeyboardButtons.Add(list);
                InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);
                await client.SendTextMessageAsync(chatId, "Выберите фамилию", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
            }
        }
    }
}

