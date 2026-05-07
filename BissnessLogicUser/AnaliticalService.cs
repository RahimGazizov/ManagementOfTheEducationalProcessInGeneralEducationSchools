using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class AnaliticalService
    {
        private readonly AppDbContext _context;
        public AnaliticalService(AppDbContext context)
        {
            _context = context;
        }
        public class OperationResult<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
            public static OperationResult<T> Ok(T data) => new OperationResult<T> { Success = true, Data = data };
            public static OperationResult<T> Fail(string message) => new OperationResult<T> { Success = false, Message = message };
        }
        public async Task<OperationResult<AnaliticalReportDto>> AnaliticalData(string academicId,
       string termId,
       string classId,
       string subjectId,
       string? studentId)
        {
            var query = _context.JournalEntry
                .Where(j =>
                    j.Journal.AcademicYearId == academicId &&
                    j.Journal.TermId == termId &&
                    j.Journal.ClassId == classId &&
                    j.Journal.SubjectId == subjectId);

            var listData = await query
                .GroupBy(s => new
                {
                    s.StudentId,
                    s.Student.User.FullName
                })
                .Select(s => new
                {
                    studentId = s.Key.StudentId,
                    studentName = s.Key.FullName,
                    averageScore = Math.Round(s.Where(g => g.Grade != null)
                        .Average(x => (double?)x.Grade) ?? 0, 1),
                    subjectCount = s.Count(),
                    presentCount = s.Count(x => x.IsPresent)
                })
                .ToListAsync();

            var result = listData.Select(s =>
            {
                double attendance = s.subjectCount == 0
                    ? 0
                    : (double)s.presentCount / s.subjectCount * 100;

                double index = 0.8 * s.averageScore + 0.2 * (attendance / 100 * 10);

                return new DataStudents
                {
                    StudentId = s.studentId,
                    StudentFullName = s.studentName,
                    AverageGrade = s.averageScore,
                    PercentOfPresence = Math.Round(attendance, 1),
                    AcademicPerformanceIndex = Math.Round(index, 1)
                };
            }).OrderByDescending(s => s.AcademicPerformanceIndex)
            .ThenByDescending(s => s.StudentFullName)
                .ToList();

            // 🔥 если выбран студент — фильтр здесь
            if (!string.IsNullOrWhiteSpace(studentId))
            {
                result = result
                    .Where(x => listData.Any(l => l.studentId == studentId && l.studentName == x.StudentFullName))
                    .ToList();
            }

            var model = new AnaliticalReportDto
            {
                AcademicId = academicId,
                TermId = termId,
                ClassId = classId,
                SubjectId = subjectId,
                ClassName = await _context.Classes
                    .Where(x => x.Id == classId)
                    .Select(x => x.NumClass + "-" + x.LetterClass)
                    .FirstOrDefaultAsync(),

                SubjectName = await _context.Subjects
                    .Where(x => x.Id == subjectId)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(),

                DataStudents = result
            };

            return OperationResult<AnaliticalReportDto>.Ok(model);
        }
    }
}
