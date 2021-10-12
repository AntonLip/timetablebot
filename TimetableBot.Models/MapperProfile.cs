using AutoMapper;
using TimetableBot.Models.DBModels;
using TimetableBot.Models.DTOModels;

namespace TimetableBot.Models
{
    public  class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AddLessonDto, Lesson>();
            CreateMap<Lesson, LessonDto>().ReverseMap();

        }
    }
}
