
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimetableBot.Models.DTOModels;

namespace TimetableBot.Models.Interface
{
    public interface ITimetableService
    {
        Task DeleteTimetable(CancellationToken cancellationToken = default);
        Task<IEnumerable<IEnumerable<LessonDto>>> GetFilteredTimetable(LessonFilter lessonFilter, CancellationToken cancellationToken = default);
    }
}
