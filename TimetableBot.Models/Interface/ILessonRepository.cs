using System;
using TimetableBot.Models.DBModels;

namespace TimetableBot.Models.Interface
{
    public interface ILessonRepository : IRepository<Lesson, Guid>
    {
    }
}
