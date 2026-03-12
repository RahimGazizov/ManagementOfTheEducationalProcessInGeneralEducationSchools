using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class TeacherPerAccLogic
    {
        private readonly AppDbContext _context;
        public TeacherPerAccLogic(AppDbContext context)
        {
            _context = context;
        }
        public async Task<string> GetTeacherId(string userId)
        {
            var teacherId = await _context.Teachers
               .Where(t => t.UserId == userId)
               .Select(t => t.Id)
               .FirstOrDefaultAsync();
            return teacherId;
        }
        public async Task<List<Subjects>> GetListSubjects(string userId)
        {
            var teacherId = await GetTeacherId(userId);
            return await _context.TeacherAssigments
            .Where(a => a.TeacherId == teacherId)
            .Include(a => a.Subject)
            .Select(a => a.Subject)
            .Distinct()
            .ToListAsync();
        }
        public async Task<List<Class>> GetListClass(string userId)
        {
            var teacherId = await GetTeacherId(userId);
            return await _context.TeacherAssigments
                .Where(t => t.TeacherId == teacherId)
                .Include(t => t.Class)
                .Select(t => t.Class)
                .ToListAsync();
        }
        public async Task<List<Journal>> GetListJournals(string userId, string subjectId, string classId, DateTime? dateFrom, DateTime? dateTo)
        {
            var teacherId = await GetTeacherId(userId);
            var journals = await _context.Journal
               .Where(j => j.TeacherId == teacherId && j.SubjectId == subjectId && j.ClassId == classId)
               .ToListAsync();
            if (dateFrom.HasValue || dateTo.HasValue)
            {
                var fromDate = dateFrom?.Date;
                var toDate = dateTo?.Date.AddDays(1);

                journals = journals.Where(j => _context.JournalEntry.Any(it => it.JournalId == j.Id &&
                (!fromDate.HasValue || it.Date >= fromDate) &&
                (!toDate.HasValue || it.Date < toDate))).ToList();
            }
         
            return journals;
        }
    }
}
