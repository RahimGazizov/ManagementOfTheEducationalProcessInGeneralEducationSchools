using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class JournalController : Controller
    {
        private readonly AppDbContext _context;
        public JournalController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Create(string teacherId, string subjectId, string classId)
        {
            var journal = new Journal
            {
                TeacherId = teacherId,
                SubjectId = subjectId,
                ClassId = classId
            };

            var students = await _context.Students.Where(s => s.ClassId == classId).ToListAsync();
            foreach (var student in students)
            {
                var journalEntri = new JournalEntry
                {
                    StudentId = student.Id,
                    Date = DateTime.Now,
                    Grade = null,
                   
                };
                journal.Entries.Add(journalEntri);
            }
            _context.Journal.Add(journal);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = journal.Id });
        }
        public async Task<IActionResult> Edit(string id)
        {
            var journal = await _context.Journal
                .Include(c => c.Class)
                .Include(t => t.Teacher)
                .ThenInclude(t => t.User)
                .Include(s => s.Subject)
                .Include(j => j.Entries)
                .ThenInclude(j => j.Student)
                .ThenInclude(j => j.User)
                .FirstOrDefaultAsync(j => j.Id == id);

            return View(journal);
        }
        [HttpPost]
        public async Task<IActionResult> SaveJournal(Journal journal)
        {

            var exists = await _context.Journal.FirstOrDefaultAsync(j => j.Id == journal.Id);
            if (exists == null)
            {
                TempData["Error"] = "Журнал не найден";
                return View("Edit");
            }
            foreach (var entryModal in journal.Entries)
            {
                Console.WriteLine($"Оценки:{entryModal.Grade}");
                Console.WriteLine($"Присутсвие :{entryModal.IsPresent}");
                var entry = await _context.JournalEntri.FirstOrDefaultAsync(e => e.Id == entryModal.Id);
                if (entry != null)
                {
                    entry.Grade = entryModal.Grade;
                    entry.IsPresent = entryModal.IsPresent;
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = journal.Id });
        }
    }
}
