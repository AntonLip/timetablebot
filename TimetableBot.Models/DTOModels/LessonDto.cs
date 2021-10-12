﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableBot.Models.DTOModels
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        //public int NumberOfWeek { get; set; }
        public string DayOfWeek { get; set; }
        //public int DayInWeekNumber { get; set; }
        public int LessonNumber { get; set; }
        public string GroupNumber { get; set; }
        public int LessonInDayNumber { get; set; }
        public string DisciplineName { get; set; }
        public string LessonType { get; set; }
        public string LecturalName { get; set; }
        public DateTime LessonDate { get; set; }

        public string AuditoreNumber { get; set; }


        //public string InfoForLectural { get; set; }
        //public string InfoForEngeneers { get; set; }
        //public string InfoForcadets { get; set; }
    }
}
