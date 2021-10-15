using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimetableBot.Models;
using TimetableBot.Models.DTOModels;
using TimetableBot.Models.Interface;

namespace timetablebot.Service
{
    public class TimetableService: ITimetableService
    {
        private readonly HttpClient timetableClient = new HttpClient();
        public TimetableService(IOptions<HttpClientSettings> options)
        {
            timetableClient.BaseAddress = new Uri(options.Value.Url);
        }

        public Task DeleteTimetable(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IEnumerable<LessonDto>>> GetFilteredTimetable(LessonFilter lessonFilter, CancellationToken cancellationToken = default)
        {
            
            var json = JsonConvert.SerializeObject(lessonFilter);
            var result = await timetableClient.PostAsync("/api/Timetable/lesson/filtered",
                            new StringContent(json, Encoding.UTF8, "application/json"));
            string resultContent = await result.Content.ReadAsStringAsync();
            try

            {
                var lessons = JsonConvert.DeserializeObject<List<List<LessonDto>>>(resultContent);
                return lessons;
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
            timetableClient.Dispose();
            return null;
        }
    }
}
