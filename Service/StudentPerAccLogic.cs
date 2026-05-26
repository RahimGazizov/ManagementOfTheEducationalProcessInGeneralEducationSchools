using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace InformationSystemOfASchoolIducationalPortal.Service
{
    public class StudentPerAccLogic
    {
        private readonly AppDbContext _context;
        public StudentPerAccLogic(AppDbContext context)
        {
            _context = context;
        }
        public class ResultRating
        {
            public RatingCurrentClass RatingCurrentClass { get; set; }
            public RatingParallelClass? RatingParallelClass { get; set; }
            public string? Message { get; set; }
        }
        public class RatingCurrentClass
        {
            public List<RatingData> Top3 { get; set; }
            public RatingData RatingStudent { get; set; }
            public int TotalStudent { get; set; }
        }
        public class RatingParallelClass
        {
            public List<RatingData> Top3Parallel { get; set; }
            public RatingData RatingStudent { get; set; }
            public int TotalStudent { get; set; }
        }
        public class RatingData
        {
            public int Place { get; set; }
            public string StudentId { get; set; }
            public string StudentName { get; set; }
            public double Average { get; set; }
            public double Percent { get; set; }
            public double Score { get; set; }
            public int? ClassNum { get; set; }
            public string? ClassLetter { get; set; }
            public int? LessonCount { get; set; }
            public int? PresentLesson { get; set; }
        }
        public class SubjectInfoDto
        {
            public double Average { get; set; }
            public string Name { get; set; }
            public double Percent { get; set; }
        }
        public class OperationResult
        {
            public string Message { get; set; }
            public bool Success { get; set; }
            public static OperationResult Ok() => new OperationResult { Success = true };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }
        public class OperationResult<T>
        {
            public string Message { get; set; }
            public bool Success { get; set; }
            public T Data { get; set; }
            public static OperationResult<T> Ok(T data) => new OperationResult<T> { Success = true, Data = data };
            public static OperationResult<T> Fail(string? message) => new OperationResult<T> { Success = false, Message = message };
        }
        public async Task<AcademicYear> GetCurrentAcademicYear()
        {
            return await _context.AcademicYear
               .Where(d => d.StartDateYear <= DateTime.Now && d.EndDateYear >= DateTime.Now)
               .FirstOrDefaultAsync() ?? new();
        }
        public async Task<Term> GetCurrentTerm()
        {
            return await _context.Term
               .Where(d => d.DateStartTerm <= DateTime.Now && d.DateEndTerm >= DateTime.Now)
               .FirstOrDefaultAsync() ?? new();
        }
        public async Task<List<Subjects>> ListSubjects(string classId)
        {
            return await _context.TeacherAssigments
                    .Where(t => t.ClassId == classId)
                    .Include(s => s.Subject)
                    .Select(s => s.Subject)
                    .ToListAsync();
        }
        public async Task<double> AverageGrade(Students student)
        {
            var avr = await _context.JournalEntry
                   .Where(t => t.StudentId == student.Id && t.Grade != null)
                   .AverageAsync(e => e.Grade);
            return avr != null ? Convert.ToDouble(avr) : 0;
        }
        public async Task<OperationResult<SubjectInfoDto>> AvgGradeForTheSubject(string classId, string subjectId, string academicId, string termId, string studentId)
        {
            if (string.IsNullOrWhiteSpace(classId) || string.IsNullOrWhiteSpace(subjectId) || string.IsNullOrWhiteSpace(academicId)
                || string.IsNullOrWhiteSpace(termId) || string.IsNullOrWhiteSpace(studentId))
                return OperationResult<SubjectInfoDto>.Fail("Некоторые данные пусты");
            var subjectName = await _context.Subjects.Where(s => s.Id == subjectId).Select(s => s.Name).FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(subjectName))
                return OperationResult<SubjectInfoDto>.Fail("Предмет не найден");

            var entries = await _context.Journal
                .Where(j => j.ClassId == classId && j.SubjectId == subjectId
                && j.AcademicYearId == academicId && j.TermId == termId)
                .SelectMany(j => j.Entries)
                .Where(j => j.StudentId == studentId)
                .ToListAsync();
            var averageScore = entries.Any() ? entries.Average(j => j.Grade) : 0;

            var count = entries.Count(s => s.IsPresent);
            var percentageOfAttendance = entries.Any() ? (double)count / entries.Count * 100 : 0;
            var subInfo = new SubjectInfoDto
            {
                Average = averageScore ?? 0,
                Name = subjectName,
                Percent = percentageOfAttendance
            };
            return OperationResult<SubjectInfoDto>.Ok(subInfo);
        }
        public async Task<OperationResult<ResultRating>> RatingStudent(string studentId, string classId, string academicId, string termId)
        {
            if (string.IsNullOrWhiteSpace(classId) || string.IsNullOrWhiteSpace(academicId)
           || string.IsNullOrWhiteSpace(termId) || string.IsNullOrWhiteSpace(studentId))
                return OperationResult<ResultRating>.Fail("Некоторые данные пусты");
            var data = await _context.JournalEntry
                .Where(s => s.Journal.ClassId == classId
                    && s.Journal.AcademicYearId == academicId
                    && s.Journal.TermId == termId)
                .GroupBy(s => new
                {
                    s.StudentId,
                    s.Student.User.FullName
                })
                .Select(g => new RatingData
                {
                    StudentId = g.Key.StudentId,
                    StudentName = g.Key.FullName,
                    Average = Math.Round(g.Where(s => s.Grade != null)
                                .Average(s => (double?)s.Grade) ?? 0, 1),
                    LessonCount = g.Count(),
                    PresentLesson = g.Count(s => s.IsPresent)
                })
                .ToListAsync();
            if (!data.Any())
                return OperationResult<ResultRating>.Fail("Данные для рейтинга пусты");

            var result = BuildRating(data);
            var ratingParallel = await GetParallelRating(studentId, classId, academicId, termId);
            var currentStudent = result.FirstOrDefault(x => x.StudentId == studentId);
            if (currentStudent == null)
                return OperationResult<ResultRating>.Fail("Нет ваших данных для рейтинга");
            var classRating = new RatingCurrentClass
            {
                Top3 = result.Take(3).ToList(),
                RatingStudent = currentStudent,
                TotalStudent = result.Count()
            };
            var totalResult = new ResultRating
            {
                RatingCurrentClass = classRating
            };
            
            if (ratingParallel.Success)
                totalResult.RatingParallelClass = ratingParallel.Data;
            else
            {
                totalResult.RatingParallelClass = null;
                totalResult.Message = "Рейтинг параллели пока недоступен";
            }
            return OperationResult<ResultRating>.Ok(totalResult);
        }
        public async Task<OperationResult<RatingParallelClass>> GetParallelRating(string studentid, string classId, string academicId, string termId)
        {
            try
            {
                var numClass = await _context.Classes
                    .Where(s => s.Id == classId).Select(s => s.NumClass).FirstOrDefaultAsync();
                var parrallelCLass = await _context.Classes
                    .Where(s => s.NumClass == numClass).ToListAsync();
                if (parrallelCLass.Count < 2)
                    return OperationResult<RatingParallelClass>.Fail(null);
                var data = await _context.JournalEntry
                    .Where(s => s.Journal.Class.NumClass == numClass
                    && s.Journal.AcademicYearId == academicId
                    && s.Journal.TermId == termId)
                    .GroupBy(s => new
                    {
                        s.StudentId,
                        s.Student.User.FullName,
                    })
                    .Select(s => new RatingData
                    {
                        StudentId = s.Key.StudentId,
                        StudentName = s.Key.FullName,
                        ClassNum = s.Select(g => g.Journal.Class.NumClass).FirstOrDefault(),
                        ClassLetter = s.Select(g => g.Journal.Class.LetterClass).FirstOrDefault(),
                        Average = Math.Round(s.Where(g => g.Grade != null)
                        .Average(g => g.Grade) ?? 0, 1),
                        LessonCount = s.Count(),
                        PresentLesson = s.Count(g => g.IsPresent)
                    }).ToListAsync();
                if (!data.Any())
                    return OperationResult<RatingParallelClass>.Fail(null);
                var result = BuildRating(data);
                var currentUser = result.FirstOrDefault(s => s.StudentId == studentid);
                if (currentUser == null)
                    return OperationResult<RatingParallelClass>.Fail(null);
                var parallel = new RatingParallelClass
                {
                    Top3Parallel = result.Take(3).ToList(),
                    RatingStudent = currentUser,
                    TotalStudent = result.Count(),
                };
                return OperationResult<RatingParallelClass>.Ok(parallel);
            }
            catch(Exception)
            {
                return OperationResult<RatingParallelClass>.Fail(null);
            }
        }
        private List<RatingData> BuildRating(List<RatingData> data)
        {
            var rating = data
                .Select(s =>
                {
                    int lessonCount = s.LessonCount ?? 0;
                    int presentLesson = s.PresentLesson ?? 0;

                    double attendancePercent = lessonCount == 0
                        ? 0
                        : (double)presentLesson / lessonCount * 100;

                    double score = 0.8 * s.Average + 0.2 * (attendancePercent / 20.0);

                    return new RatingData
                    {
                        StudentId = s.StudentId,
                        StudentName = s.StudentName,

                        Average = s.Average,
                        Percent = Math.Round(attendancePercent, 1),
                        Score = Math.Round(score, 1),

                        ClassNum = s.ClassNum,
                        ClassLetter = s.ClassLetter,

                        LessonCount = s.LessonCount,
                        PresentLesson = s.PresentLesson
                    };
                })
                .OrderByDescending(s => s.Score)
                .ThenByDescending(s => s.Average)
                .ToList();

            for (int i = 0; i < rating.Count; i++)
            {
                rating[i].Place = i + 1;
            }

            return rating;
        }
        public async Task<List<ScheduleLesson>> ScheduleLessons(Students student)
        {
            var dayToday = DateTime.Today.ToString("dddd", new CultureInfo("ru-RU")).ToLower().Trim();
            var schedule = await _context.Schedules
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Subject)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Class)
                .Include(t => t.LessonSlot)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Teacher)
                .ThenInclude(t => t.User)
                .Where(t => t.Assigment != null && t.Assigment.ClassId == student.ClassId && t.DayOfWeek.ToLower().Trim() == dayToday)
                .ToListAsync();
            return schedule;
        }
        public List<SelectListItem> GetAcademicList()
        {
            return _context.AcademicYear.Select(a => new SelectListItem
            {
                Value = a.Id,
                Text = a.Name
            }).ToList();
        }
        public List<SelectListItem> GetTermList()
        {
            return _context.Term.Select(a => new SelectListItem
            {
                Value = a.Id,
                Text = a.Name
            }).ToList();
        }
        public async Task<List<SelectListItem>> GetClasses(string studentId)
        {
            return await _context.StudentsHistory
                .Where(s => s.StudentId == studentId)
                .Select(s => new SelectListItem
                {
                    Value = s.ClassId,
                    Text = s.Class.NumClass + s.Class.LetterClass
                }).ToListAsync();
        }
        public async Task<double> AvgScoreDimamics(Students student)
        {
            var today = DateTime.Today;

            var dayOfWeek = (int)today.DayOfWeek;
            var offest = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            var startOfWeek = today.AddDays(-offest).Date;
            var currentTerm = await GetCurrentTerm();
            var currentYear = await GetCurrentAcademicYear();

            var grades = await _context.JournalEntry
                 .Where(s => s.StudentId == student.Id && s.Journal.ClassId == student.ClassId
                 && s.Journal.AcademicYearId == currentYear.Id && s.Journal.TermId == currentTerm.Id
                 && s.Grade != null)
                 .Select(s => new
                 {
                     Grade = (double)s.Grade,
                     LessonDate = s.Journal.Date,
                 }).ToListAsync();
            if (!grades.Any())
                return 0;
            var currentAvg = grades.Average(s => s.Grade);

            var oldAvgGrade = grades
                .Where(s => s.LessonDate < startOfWeek)
                .ToList();
            if (!oldAvgGrade.Any())
                return 0;

            var previousAverage = oldAvgGrade.Average(s => s.Grade);
            var delta = Math.Round(currentAvg - previousAverage, 1);

            return delta;

        }
    }
}
