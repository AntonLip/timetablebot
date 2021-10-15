using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using TimetableBot.Models.Interface;
using System.Collections.Generic;
using TimetableBot.Models.DTOModels;
using System;
using System.Linq;

namespace TimetableBot.Models
{
    public abstract class BaseCommand : ICommand
    {
        private readonly ITimetableService _timetableService;
        public string Name { get; }
        public BaseCommand(ITimetableService timetableService)
        {
            _timetableService = timetableService;
        }

        public abstract bool Contains(Message message);         

        public virtual async Task Execute(Message message, CallbackQuery query, TelegramBotClient client)
        {
            throw new Exception();
        }
        protected string GetStringFromLessons(IEnumerable<IEnumerable<LessonDto>> lessonDtos)
        {
            var returnString = string.Empty;
            foreach (var lessons in lessonDtos)
            {
                returnString += lessons.First(p => true).LessonDate.ToShortDateString().ToString() + "\n";
                returnString += lessons.First(p => true).LessonDate.DayOfWeek + "\n";
                foreach (var lesson in lessons)
                {
                    returnString += lesson.LessonInDayNumber + "ая пара " + lesson.DisciplineName + " " +
                                   lesson.LessonType + "№" + lesson.LessonNumber;
                    if (!(lesson.AuditoreNumber is null))
                    {
                        returnString += " в " + lesson.AuditoreNumber;
                    }
                    returnString += "\n";
                }
            }
            return returnString;
        }

        public virtual async Task Handle(Message message, CallbackQuery query, TelegramBotClient client)
        {            
            long chatId = 0;
            string[] filterParameter = null;
            if (message is null)
            {
                chatId = query.Message.Chat.Id;
                filterParameter = query.Data.Split(' ');
            }
            else
            {
                chatId = message.Chat.Id;
                filterParameter = message.Text.Split(' ');
            }

            if (filterParameter.Length < 3)
            {
              
            }
            FilterFields lessonFilter = new FilterFields();
            lessonFilter.DateStart = DateTime.Today;
            lessonFilter.DateEnd = DateTime.Today;
            if (filterParameter[1] == "gr")
                lessonFilter.Group = filterParameter[2];
            else
                lessonFilter.Lectural = filterParameter[2];
            if (filterParameter.Length == 4)
            {
                lessonFilter.DateStart = DateTime.Parse(filterParameter[3]);
                lessonFilter.DateEnd = new DateTime(1991, 10, 11);
            }
            else if (filterParameter.Length == 5)
            {
                lessonFilter.DateStart = DateTime.Parse(filterParameter[3]);
                lessonFilter.DateEnd = DateTime.Parse(filterParameter[4]);
            }
            var lessons = await _timetableService.GetFilteredTimetable(new LessonFilter { FilterBy = lessonFilter });
            if (lessons is null || lessons.Count() == 0)
                await client.SendTextMessageAsync(chatId, "Something go wrong or you haven't got lessons", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            else
                await client.SendTextMessageAsync(chatId, GetStringFromLessons(lessons), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }

    }
}
