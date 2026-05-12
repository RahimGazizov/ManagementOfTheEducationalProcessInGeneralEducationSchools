using InformationSystemOfASchoolIducationalPortal.Service;
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
        private readonly JournalService _journalService;
        private readonly SystemStateService _systemStateService;
        public JournalController(AppDbContext context, JournalService journalService,
            SystemStateService systemStateService)
        {
            _context = context;
            _journalService = journalService;
            _systemStateService = systemStateService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(string teacherId, string subjectId, string classId)
        {
            try
            {
                var result = await _journalService.CreateJournal(teacherId, subjectId, classId);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Index", "TeacherPerAcc");
                }
                return RedirectToAction("Edit", new { id = result.Data });
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
                var journal = await _journalService.Edit(id);
                if (!journal.Success)
                    return RedirectToAction("Index", "TeacherPerAcc", new { error = journal.Message });
                return View(journal.Data);
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
                var result = await _journalService.SaveJournal(journal);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    return View("Edit");
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Edit", new { id = result.Data });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "TeacherPerAcc");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _journalService.Delete(id);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                }
                return RedirectToAction("Index", "TeacherPerAcc");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "TeacherPerAcc");
            }
        }

    }
}
