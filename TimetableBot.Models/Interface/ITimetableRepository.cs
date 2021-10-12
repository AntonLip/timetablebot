using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimetableBot.Models.DBModels;
using TimetableBot.Models.DTOModels;

namespace TimetableBot.Models.Interface
{
    public interface ITimetableRepository : IRepository<Lesson, Guid>
    {
        Task<IEnumerable<Lesson>> GetFilteredAsync(LessonFilter LessonFilter, CancellationToken cancellationToken = default);
        Task InsertManyLesson(List<Lesson> lessons, CancellationToken cancellationToken = default);
        Task DeleteAllLessons(CancellationToken cancellationToken);
    }
}
