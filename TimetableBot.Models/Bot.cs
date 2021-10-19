using System.Collections.Generic;
using TimetableBot.Models.Command;
using TimetableBot.Models.Interface;

namespace TimetableBot.Models
{
    public class Bot :IBot
    {
        private  List<ICommand> commandsList;
        public Bot(ITimetableService timetableService)
        {
            commandsList = new List<ICommand>();
            commandsList.Add(new StartCommand(timetableService));
            commandsList.Add(new ClearTimetableCommand(timetableService));
            commandsList.Add(new StudentCommand(timetableService));
            commandsList.Add(new LecturalCommand(timetableService));
        }      

        public List<ICommand> GetCommands()
        {
            if (commandsList is null)
                throw new System.Exception();
            return commandsList;
        }
    }
}
