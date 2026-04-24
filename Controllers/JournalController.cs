using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Учитель")]
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
            try
            {
                var dayToday = DateTime.Today;
                var currentYear = await GetAcademicYear(dayToday);
                var currentTerm = await GetTerm(dayToday, currentYear.Id);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Teacher-id - {teacherId}");
                Console.WriteLine($"Subject-id - {subjectId}");
                Console.WriteLine($"CLass-id - {classId}");
                Console.WriteLine($"CurrentYear-id - {currentYear.Id}");
                Console.WriteLine($"Current-id - {currentTerm.Id}");
                Console.ForegroundColor = ConsoleColor.White;
                
                if (currentYear == null)
                {
                    TempData["Error"] = "Не найден текущий учебный год";
                    return RedirectToAction("Index", "TeacherPerAcc");
                }

                if (currentTerm == null)
                {
                    TempData["Error"] = ("Не найдена текущая четверть");
                    return RedirectToAction("Index", "TeacherPerAcc");
                }
                var journal = new Journal
                {
                    Date = dayToday,
                    TeacherId = teacherId,
                    SubjectId = subjectId,
                    ClassId = classId,
                    AcademicYearId = currentYear.Id,
                    TermId = currentTerm.Id,
                    HomeWork = null,
                    LessonTopic = null,
                    IsLocked = true
                };

                var students = await _context.Students.Where(s => s.ClassId == classId).ToListAsync();
                foreach (var student in students)
                {
                    var journalEntri = new JournalEntry
                    {
                        StudentId = student.Id,
                        Grade = null
                    };
                    journal.Entries.Add(journalEntri);
                }
                _context.Journal.Add(journal);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", new { id = journal.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "TeacherPerAcc");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                Console.WriteLine($"ID-{id}");
                var journal = await _context.Journal
                    .Include(c => c.Class)
                    .Include(t => t.Teacher)
                    .ThenInclude(t => t.User)
                    .Include(s => s.Subject)
                    .Include(j => j.Entries)
                    .ThenInclude(j => j.Student)
                    .ThenInclude(j => j.User)
                    .Include(j => j.AcademicYear)
                    .Include(j => j.Term)
                    .FirstOrDefaultAsync(j => j.Id == id);
                DateTime now = DateTime.Now;
                if (journal.Date != default)
                {
                    journal.IsLocked = (DateTime.Now - journal.Date).TotalDays <= 7;
                }
                else
                {
                    journal.IsLocked = true; // ещё нет записей, редактировать можно
                }
                return View(journal);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "TeacherPerAcc");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveJournal(Journal journal)
        {
            try
            {
                var exists = await _context.Journal.FirstOrDefaultAsync(j => j.Id == journal.Id);
                if (exists == null)
                {
                    TempData["Error"] = "Журнал не найден";
                    return View("Edit");
                }
                exists.LessonTopic = journal.LessonTopic;
                exists.HomeWork = journal.HomeWork;
                foreach (var entryModal in journal.Entries)
                {
                    var entry = await _context.JournalEntry.FirstOrDefaultAsync(e => e.Id == entryModal.Id);
                    if (entry != null)
                    {
                        if(entryModal.Grade != null && !entryModal.IsPresent)
                            entry.IsPresent = true;
                        
                        else
                            entry.IsPresent = entryModal.IsPresent;
                        
                        entry.Grade = entryModal.Grade;

                    }

                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Журнал успешно сохранен";
                return RedirectToAction("Edit", new { id = journal.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "TeacherPerAcc");
            }
        }
        public async Task<IActionResult> Delete(string id)
        {
            try
            {

                var journal = await _context.Journal.Include(j => j.Entries)
                    .Where(j => j.Id == id)
                    .FirstOrDefaultAsync();
                if (journal != null) _context.Remove(journal);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "TeacherPerAcc");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "TeacherPerAcc");
            }
        }
        private async Task<AcademicYear?> GetAcademicYear(DateTime dayToday) => await _context.AcademicYear
                .Where(d => d.StartDateYear <= dayToday && dayToday <= d.EndDateYear)
                .FirstOrDefaultAsync();
        private async Task<Term?> GetTerm(DateTime dayToday, string currentId) => await _context.Term
           .Where(d => d.AcademicYearId == currentId && d.DateStartTerm <= dayToday && dayToday <= d.DateEndTerm)
           .FirstOrDefaultAsync();
    }
}
