using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimetableBot.Models;
using TimetableBot.Models.DBModels;
using TimetableBot.Models.DTOModels;
using TimetableBot.Models.Interface;

namespace TimetableBot.DataAccess
{
    public class TimetableRepository : BaseRepository<Lesson>, ITimetableRepository
    {
        public TimetableRepository(IOptions<MongoDBSettings> mongoDbSettings) : base(mongoDbSettings)
        {

        }
        public async Task<IEnumerable<Lesson>> GetFilteredAsync(LessonFilter LessonFilter, CancellationToken cancellationToken = default)
        {
            var DefaultDate = new DateTime(1991, 10, 11);
            FilterDefinition<Lesson> filter =
                 Builders<Lesson>.Filter.Eq(new ExpressionFieldDefinition<Lesson, bool>(x => x.IsDeleted), false);
            if (LessonFilter?.FilterBy?.Lectural != null)
            {
                filter = filter & Builders<Lesson>.Filter.Eq(new ExpressionFieldDefinition<Lesson, string>(x => x.LecturalName),
                    LessonFilter?.FilterBy?.Lectural.ToUpper());
            }

            if (!String.IsNullOrEmpty(LessonFilter?.FilterBy?.Group))
            {
                filter = filter & Builders<Lesson>.Filter.Eq(new ExpressionFieldDefinition<Lesson, string>(x => x.GroupNumber),
                    LessonFilter?.FilterBy?.Group.ToUpper());
            }

            if (!String.IsNullOrEmpty(LessonFilter?.FilterBy?.Discipline))
            {
                filter = filter & Builders<Lesson>.Filter.Eq(new ExpressionFieldDefinition<Lesson, string>(x => x.DisciplineName),
                    LessonFilter?.FilterBy?.Discipline.ToUpper());
            }
           
            if (!String.IsNullOrEmpty(LessonFilter?.FilterBy?.AuditoreNumber))
            {
                filter = filter & Builders<Lesson>.Filter.Eq(new ExpressionFieldDefinition<Lesson, string>(x => x.AuditoreNumber),
                    (string)LessonFilter?.FilterBy?.AuditoreNumber.ToUpper());
            }
            if (LessonFilter?.FilterBy?.DateStart != DefaultDate)
            {
                filter = filter & Builders<Lesson>.Filter.Gte(new ExpressionFieldDefinition<Lesson, DateTime>(x => x.LessonDate),
                    (DateTime)LessonFilter?.FilterBy?.DateStart);
            }
            if (LessonFilter?.FilterBy?.DateEnd != DefaultDate)
            {
                filter = filter & Builders<Lesson>.Filter.Lte(new ExpressionFieldDefinition<Lesson, DateTime>(x => x.LessonDate),
                      (DateTime)LessonFilter?.FilterBy?.DateEnd.AddDays(1));
            }
            
            return await GetCollection().Find(filter).ToListAsync(cancellationToken);
        }

        public async Task InsertManyLesson(List<Lesson> lessons, CancellationToken cancellationToken = default)
        {
            var options = new InsertManyOptions { IsOrdered = false , BypassDocumentValidation = false};

            try
            {
                await GetCollection().InsertManyAsync(lessons);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task DeleteAllLessons(CancellationToken cancellationToken)
        {
            await GetCollection().DeleteManyAsync(FilterDefinition<Lesson>.Empty, cancellationToken);
        }
    }
}
