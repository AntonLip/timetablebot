using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TimetableBot.Filters;
using TimetableBot.Models.DTOModels;
using TimetableBot.Models.Interface;

namespace TimetableBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ExceptionFilter]
    [HttpModelResultFilter]
    public class TimetableController : ControllerBase
    {

        private readonly ITimetableService _timetableService;
        private readonly ILogger<TimetableController> _logger;
        public TimetableController(ITimetableService timetableService,
                                    ILogger<TimetableController> logger)
        {
            _timetableService = timetableService;
            _logger = logger;
        }

       

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Get full timetable", Type = typeof(ResultDto<List<LessonDto>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ResultDto<IEnumerable<LessonDto>>>> GetTimetable()
        {
            return Ok(await _timetableService.GetTimetable());
        }
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Create Timetable From File", Type = typeof(ResultDto<int>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ResultDto<IEnumerable<LessonDto>>>> CreateTimetableFromFile([FromForm]  IFormFile body)
        {
            return Ok(await _timetableService.CreateTimetableFromFile(body));
        }

        [HttpDelete]
        [Route("lesson/{lessonId:Guid}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Delete lesson", Type = typeof(ResultDto<List<LessonDto>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ResultDto<IEnumerable<LessonDto>>>> RemoveLessonBuId([FromRoute] Guid lessonId)
        {
            return Ok(await _timetableService.DeleteLesson(lessonId));
        }

        [HttpPost]
        [Route("lesson/filtered")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Get filtered lessons", Type = typeof(ResultDto<List<LessonDto>>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<ResultDto<IEnumerable<LessonDto>>>> GetFilteredLessons([FromBody] LessonFilter lessonFilter)
        {
            return Ok(await _timetableService.GetFilteredTimetable(lessonFilter));
        }
        [HttpPost]
        [Route("lesson/filteredDoc")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Get filtered lessons in docx", Type = typeof(File))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFilteredLessonsInDocx([FromBody] LessonFilter lessonFilter)
        {
            var fileDto = await _timetableService.GetTimetableInDocxAsync(lessonFilter);
            if (fileDto is null)
                throw new ArgumentException();

            return File(fileDto.FileData, fileDto.FileType,fileDto.FileName);               
        }
        [HttpPost]
        [Route("lesson/addlesson")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Add lesson", Type = typeof(ResultDto<LessonDto>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<LessonDto>> AddLesson([FromBody] AddLessonDto lesson)
        {
            var result = await _timetableService.AddLesson(lesson);
            return Ok(result);
        }

        [HttpGet]
        [Route("lesson/{lessonId:Guid}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Get lesson by id", Type = typeof(ResultDto<LessonDto>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<LessonDto>> GetLessonById([FromRoute] Guid lessonId)
        {
            var result = await _timetableService.GetLessonById(lessonId);
            return Ok(result);
        }
        [HttpPut]
        [Route("lesson/{lessonId:Guid}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Get lesson by id", Type = typeof(ResultDto<LessonDto>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<LessonDto>> UpdateLesson([FromRoute] Guid lessonId, [FromBody] LessonDto lessonDto)
        {
            var result = await _timetableService.UpdateLesson(lessonId, lessonDto );
            return Ok(result);
        }
    }
}
