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

        public async Task<List<Subjects>> GetListSubjects(string teacherId)
        {
            return await _context.TeacherAssigments
            .Where(a => a.TeacherId == teacherId)
            .Include(a => a.Subject)
            .Select(a => a.Subject)
            .Distinct()
            .ToListAsync();
        }
        public async Task<List<Class>> GetListClass(string teacherId)
        {
            return await _context.TeacherAssigments
                .Where(t => t.TeacherId == teacherId)
                .Include(t => t.Class)
                .Select(t => t.Class)
                .ToListAsync();
        }
    }
}
