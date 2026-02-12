using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class TeachingAssigmentLogic
    {
        private readonly AppDbContext _context;
        public TeachingAssigmentLogic(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<SelectListItem>> GetListTeachers()
        {
            var list = await _context.Teachers.Select(t => new SelectListItem
            {
                Value = t.Id,
                Text = t.User.UserName,
            }).ToListAsync();
            return list;
        }
        public async Task<List<SelectListItem>> GetListSubjects()
        {
            var list = await _context.Subjects.Select(s => new SelectListItem
            {
                Value = s.Id,
                Text = s.Name,
            }).ToListAsync();
            return list;
        }
    }
}
