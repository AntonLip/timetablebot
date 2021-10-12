using System.Collections.Generic;

namespace TimetableBot.Models.DTOModels
{
    public class ResultDto<TModel>
    {
        public TModel Value { get; set; }

        public List<string> Errors { get; set; }
    }
}
