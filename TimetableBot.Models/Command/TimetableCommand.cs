using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TimetableBot.Models.DTOModels;
using TimetableBot.Models.Interface;

namespace TimetableBot.Models.Command
{
    public class TimetableCommand : ICommand
    {
        private readonly ITimetableService _timetableService;
        public TimetableCommand(ITimetableService timetableService)
        {
            _timetableService = timetableService;
        }
        public string Name => @"/get";

        public bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var filterParameter = message.Text.Split(' ');

            if (filterParameter.Length < 3)
            {
                await client.SendTextMessageAsync(chatId, "Command format is /get + " +
                    "gr - for students + group number\n" +
                    "L - for lectural + lectural last name\n" +
                    "optional parameters if you want get lesson in some period\n" +
                    "datestart - start date for period\n" +
                    "you can write only datestart it's mean that you want get all lessons from datestart\n" +
                    "dateend - end of period\n", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                return;
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
        private string GetStringFromLessons(IEnumerable<IEnumerable<LessonDto>> lessonDtos)
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
    }
}

