using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimetableBot.Models.DBModels;
using TimetableBot.Models.DTOModels;
using TimetableBot.Models.Interface;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace TimetableBot.Services
{
    public class TimetableService : ITimetableService
    {
        private readonly ITimetableRepository _timetableRepository;
        private readonly IMapper _mapper;

        public TimetableService(ITimetableRepository timetableRepository, IMapper mapper)
        {
            _mapper = mapper;
            _timetableRepository = timetableRepository;

        }

        public async Task<LessonDto> AddLesson(AddLessonDto model, CancellationToken cancellationToken = default)
        {
            if (model is null)
            {
                throw new ArgumentNullException("", Resource.ModelIsEmpty);
            }
            model.LessonDate = model.LessonDate.AddDays(1);
            var lesson = _mapper.Map<Lesson>(model);
            await _timetableRepository.AddAsync(lesson, cancellationToken);
            return _mapper.Map<LessonDto>(lesson);
        }

        public async Task<LessonDto> DeleteLesson(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("", Resource.IdIsEmpty);
            }
            var returnModel = await _timetableRepository.GetByIdAsync(id);
            if (returnModel is null)
            {
                throw new ArgumentNullException("", Resource.FindByIdError);
            }

            await _timetableRepository.RemoveAsync(id, cancellationToken);
            var history = await _timetableRepository.GetByIdAsync(id);

            return !(history is null) ? _mapper.Map<LessonDto>(history) : throw new ArgumentNullException();
        }

        public async Task DeleteTimetable(CancellationToken cancellationToken = default)
        {
            await _timetableRepository.DeleteAllLessons(cancellationToken);
        }

        public async Task<IEnumerable<IEnumerable<LessonDto>>> GetFilteredTimetable(LessonFilter lessonFilter, CancellationToken cancellationToken = default)
        {
            if (lessonFilter is null)
            {
                throw new ArgumentNullException();
            }
            var lessons = await _timetableRepository.GetFilteredAsync(lessonFilter, cancellationToken);
            if (lessons is null)
            {
                throw new ArgumentNullException();
            }
            var lessonDto = _mapper.Map<List<LessonDto>>(lessons);
            return lessonDto.OrderBy(p => p.LessonDate).GroupBy(g => g.LessonDate).Select(g => g.ToList());
        }

        public async Task<LessonDto> GetLessonById(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("", Resource.IdIsEmpty);
            }
            var lesson = await _timetableRepository.GetByIdAsync(id, cancellationToken);
            return !(lesson is null) ? _mapper.Map<LessonDto>(lesson) : throw new ArgumentNullException();
        }

        public async Task<IEnumerable<LessonDto>> GetTimetable(CancellationToken cancellationToken = default)
        {
            var timetable = await _timetableRepository.GetAllAsync(cancellationToken);
            return timetable is null ? throw new ArgumentNullException() : _mapper.Map<List<LessonDto>>(timetable);
        }

        public async Task<int> CreateTimetableFromFile(IFormFile body, CancellationToken cancellationToken = default)
        {
            if (body == null || body.Length == 0)
                throw new ArgumentNullException("", Resource.ModelIsEmpty);

            string fileExtension = Path.GetExtension(body.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
                throw new ArgumentException("", Resource.ValidationError);

            int count = 0;


            string groupNumber = String.Empty;
            string disciplineName = String.Empty;
            string lecturalName = String.Empty;
            int numberOfWeek = 0;
            string dayOfWeek = String.Empty;
            int dayInWeekNumber = 0;
            DateTime lessonDate = DateTime.Now;
            int lessonInDayNumber = 0;
            string lessonType = String.Empty;
            int lessonNumber = 0;
            string auditoreNumber = String.Empty;
            await _timetableRepository.DeleteAllLessons(cancellationToken);

            using (var memoryStream = new MemoryStream())
            {
                List<Lesson> lessons = new List<Lesson>();
                await body.CopyToAsync(memoryStream);
                using (var document = SpreadsheetDocument.Open(memoryStream, true))
                {

                    //create the object for workbook part  
                    WorkbookPart wbPart = document.WorkbookPart;
                    //statement to get the count of the worksheet  
                    int worksheetcount = document.WorkbookPart.Workbook.Sheets.Count();
                    //statement to get the sheet object  
                    Sheet mysheet = (Sheet)document.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(0);
                    //statement to get the worksheet object by using the sheet id  
                    Worksheet Worksheet = ((WorksheetPart)wbPart.GetPartById(mysheet.Id)).Worksheet;
                    //statement to get the sheetdata which contains the rows and cell in table  
                    IEnumerable<Row> Rows = Worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                    //Loop through the Worksheet rows
                    foreach (var row in Rows)
                    {
                        if (row.RowIndex.Value != 0)
                        {
                            //var qq = Program.GetSharedStringItemById(wbPart ,0);

                            int idx = 1; int idy = 0;
                            foreach (Cell cell in row.Descendants<Cell>())
                            {

                                var val = TimetableService.GetValue(document, cell);
                                if (val == "Неделя")
                                    break;
                                if (idx < 8)
                                {
                                    if (idx == 1)
                                    {
                                        int x;
                                        Int32.TryParse(val, out x);
                                        numberOfWeek = x;
                                    }
                                    if (idx == 2)
                                    {
                                        dayOfWeek = val;
                                    }

                                    if (idx == 3)
                                    {
                                        int x;
                                        Int32.TryParse(val, out x);
                                        dayInWeekNumber = x;
                                    }
                                    if (idx == 4)
                                    {
                                        int day, month, year;
                                        string[] z = val.Split(' ');
                                        Int32.TryParse(z[0], out day);
                                        if (z[2].Length == 4)
                                            Int32.TryParse(z[2], out year);
                                        else
                                        {
                                            Int32.TryParse(z[2].Substring(0, 3), out year);
                                        }
                                        switch (z[1])
                                        {
                                            case "СЕНТЯБРЯ":
                                                month = 09; break;

                                            case "ОКТЯБРЯ":
                                                month = 10; break;

                                            case "НОЯБРЯ":
                                                month = 11; break;

                                            case "ДЕКАБРЯ":
                                                month = 12; break;

                                            case "ЯНВАРЯ":
                                                month = 1; break;

                                            case "ФЕВРАЛЯ":
                                                month = 2; break;

                                            case "МАРТА":
                                                month = 3; break;

                                            case "АПРЕЛЯ":
                                                month = 4; break;

                                            case "МАЯ":
                                                month = 5; break;

                                            case "ИЮНЯ":
                                                month = 6; break;

                                            case "ИЮЛЯ":
                                                month = 7; break;

                                            case "АВГУСТА":
                                                month = 8; break;
                                            default:
                                                month = 0; break;

                                        }
                                        DateTime dateTime = new DateTime(year, month, day);
                                        lessonDate = dateTime;
                                    }
                                    if (idx == 5)
                                    {
                                        int x;
                                        Int32.TryParse(val, out x);
                                        lessonInDayNumber = x;
                                    }
                                    if (idx == 7)
                                    {
                                        int x;
                                        Int32.TryParse(val, out x);
                                        if (x != 4)
                                            break;
                                    }
                                }
                                else
                                {

                                    idy++;
                                    if (idy == 1)
                                    {
                                        groupNumber = val;
                                    }
                                    if (idy == 2)
                                    {
                                        disciplineName = val;
                                    }

                                    if (idy == 3)
                                    {
                                        int x = 0;
                                        if (val != "" && val != "ОХРАНА" && val != "ПРАЗДНИК")
                                        {
                                            string[] z = val.Split(' ');
                                            lessonType = z[0];
                                            Int32.TryParse(z[z.Length - 1], out x);
                                            lessonNumber = x;
                                        }
                                    }
                                    if (idy == 4)
                                    {
                                        lecturalName = val;
                                    }
                                    if (idy == 5)
                                    {
                                        if (val != null)
                                            auditoreNumber = val;
                                        else
                                            auditoreNumber = "";
                                    }
                                    if (idy == 7)
                                    {
                                        idy = 0;
                                        //await sendLessonToAPIAsync(lesson);
                                        if (groupNumber != "431А" && groupNumber != "431Б" &&
                                            groupNumber != "441А" && groupNumber != "441Б" &&
                                            groupNumber != "451А" && groupNumber != "451Б")
                                        {

                                            if (lecturalName != "" && disciplineName != "")
                                                try
                                                {

                                                    lessons.Add(new Lesson()
                                                    {
                                                        AuditoreNumber = auditoreNumber,
                                                        DayInWeekNumber = dayInWeekNumber,
                                                        DayOfWeek = dayOfWeek,
                                                        DisciplineName = disciplineName,
                                                        GroupNumber = groupNumber,
                                                        IsDeleted = false,
                                                        LecturalName = lecturalName,
                                                        InfoForcadets = null,
                                                        InfoForEngeneers = null,
                                                        InfoForLectural = null,
                                                        LessonDate = lessonDate,
                                                        LessonInDayNumber = lessonInDayNumber,
                                                        LessonNumber = lessonNumber,
                                                        LessonType = lessonType,
                                                        NumberOfWeek = numberOfWeek
                                                    });


                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e.Message);
                                                }

                                        }
                                    }
                                }
                                idx++;
                            }
                            //count = idx;

                        }

                    }

                }
                if (lessons.Count != 0)
                {
                    await _timetableRepository.InsertManyLesson(lessons);
                    count = lessons.Count;
                }
            }

            return count == 0 ? throw new ArgumentNullException("", Resource.UpdateError) : count;
        }

        public async Task<LessonDto> UpdateLesson(Guid lessonId, LessonDto lessonDto, CancellationToken cancellationToken)
        {
            if (lessonId == Guid.Empty)
            {
                throw new ArgumentNullException("", Resource.IdIsEmpty);
            }
            if (lessonDto is null)
            {
                throw new ArgumentNullException("", Resource.ModelIsEmpty);
            }
            var lesson = await _timetableRepository.GetByIdAsync(lessonId);
            if (lesson.Id != lessonDto.Id)
            {
                throw new ArgumentException();
            }
            var newLesson = _mapper.Map<Lesson>(lessonDto);
            await _timetableRepository.UpdateAsync(lessonId, newLesson);
            return newLesson is null ? throw new ArgumentNullException("", Resource.UpdateError) : _mapper.Map<LessonDto>(newLesson);

        }

        public async Task<FileDto> GetTimetableInDocxAsync(LessonFilter lessonFilter, CancellationToken cancellationToken = default)
        {
            FileDto fileDto = new FileDto();
            var timetableList = await GetFilteredTimetable(lessonFilter, cancellationToken);
            if (timetableList.Count() == 0)
                throw new ArgumentNullException();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (WordprocessingDocument wordDocument =
                        WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                    mainPart.Document = new Document(new Body());

                    Table table = new Table();

                    TableRow tr1 = new TableRow();

                    TableCell tc11 = new TableCell();
                    tc11.Append(new Paragraph(new Run(new Text("Date of lesson"))));
                    tr1.Append(tc11);

                    TableCell tc12 = new TableCell();
                    tc12.Append(new Paragraph(new Run(new Text("Count of lesson"))));
                    tr1.Append(tc12);

                    TableCell tc13 = new TableCell();
                    tc13.Append(new Paragraph(new Run(new Text("Type of lesson"))));
                    tr1.Append(tc13);

                    TableCell tc14 = new TableCell();
                    tc14.Append(new Paragraph(new Run(new Text("Disciplines name"))));
                    tr1.Append(tc14);

                    TableCell tc15 = new TableCell();
                    tc15.Append(new Paragraph(new Run(new Text("Auditore"))));
                    tr1.Append(tc15);

                    table.Append(tr1);

                    foreach (var lesson in timetableList)

                    {
                        foreach (var l in lesson)
                        {
                            TableRow tr2 = new TableRow();


                            TableCell tc21 = new TableCell();
                            tc21.Append(new Paragraph(new Run(new Text(l.LessonDate.ToString()))));
                            tr2.Append(tc21);

                            TableCell tc22 = new TableCell();
                            ParagraphProperties pp22 = new ParagraphProperties();
                            tc22.Append(new Paragraph(new Run(new Text(l.LessonInDayNumber.ToString()))));
                            tr2.Append(tc22);


                            TableCell tc23 = new TableCell();
                            tc23.Append(new Paragraph(new Run(new Text(l.LessonType + " " + l.LessonNumber))));
                            tr2.Append(tc23);

                            TableCell tc24 = new TableCell();
                            tc24.Append(new Paragraph(new Run(new Text(l.DisciplineName))));
                            tr2.Append(tc24);

                            TableCell tc25 = new TableCell();
                            tc25.Append(new Paragraph(new Run(new Text(l.AuditoreNumber))));
                            tr2.Append(tc25);

                            table.Append(tr2);
                        }
                    }
                    mainPart.Document.Body.Append(table);

                    mainPart.Document.Save();

                }
                fileDto.FileData = memoryStream.ToArray();
                fileDto.FileName = "timetable.docx";
                fileDto.FileType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                return fileDto;
            }
        }

        public async Task<List<LessonDto>> InsertManyLessons(List<AddLessonDto> lessonDtos, CancellationToken cancellationToken = default)
        {
            if (lessonDtos is null)
                throw new ArgumentException("", Resource.ModelIsEmpty);
            var lessonsDB = _mapper.Map<List<Lesson>>(lessonDtos);
            await _timetableRepository.InsertManyLesson(lessonsDB, cancellationToken);
            return _mapper.Map<List<LessonDto>>(lessonsDB);
        }
        private static string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }
            return value;
        }

    }
}
