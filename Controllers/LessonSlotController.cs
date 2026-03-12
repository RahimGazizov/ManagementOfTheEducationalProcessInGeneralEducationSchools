using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Админ")]
    public class LessonSlotController : Controller
    {
        private readonly AppDbContext _context;
        public LessonSlotController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lessonSlot = new LessonSlotViewModal
            {
                LessonSlots = await _context.LessonSlots.ToListAsync()
            };
            return View(lessonSlot);
        }
        public async Task<IActionResult> AddLessonSlot(LessonSlotViewModal lessonSlotView)
        {
            var fm = lessonSlotView.Form;
            var exists = await _context.LessonSlots.AnyAsync(l => l.LessonNumber == fm.LessonNumber &&
            l.StartTime == fm.StartTime && l.EndTime == fm.EndTime);
            if (exists)
            {
                TempData["Error"] = "Такая запись уже существует!";
                lessonSlotView.LessonSlots = await _context.LessonSlots.ToListAsync();
                lessonSlotView.IsOpenModalAdd = true;
                return View("Index", lessonSlotView);
            }
            if (fm.StartTime > fm.EndTime)
            {
                TempData["Error"] = "Не верный формат ввода времени";
                lessonSlotView.LessonSlots = await _context.LessonSlots.ToListAsync();
                lessonSlotView.IsOpenModalAdd = true;
                return View("Index", lessonSlotView);
            }
            if ((fm.EndTime - fm.StartTime).TotalMinutes != 45)
            {
                TempData["Error"] = "Урок должен длиться 45 минут";
                lessonSlotView.LessonSlots = await _context.LessonSlots.ToListAsync();
                lessonSlotView.IsOpenModalAdd = true;
                return View("Index", lessonSlotView);
            }
            _context.LessonSlots.Add(fm);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditLessonSlot(LessonSlotViewModal lessonSlotView)
        {
            var fm = lessonSlotView.Form;
            var exists = await _context.LessonSlots.AnyAsync(l => l.LessonNumber == fm.LessonNumber &&
            l.StartTime == fm.StartTime && l.EndTime == fm.EndTime && l.Id != fm.Id);
            if (exists)
            {
                TempData["Error"] = "Такая запись уже существует!";
                lessonSlotView.LessonSlots = await _context.LessonSlots.ToListAsync();
                lessonSlotView.IsOpenModalEdit = true;
                return View("Index", lessonSlotView);
            }
            if (fm.StartTime > fm.EndTime)
            {
                TempData["Error"] = "Время начало урока должна быть меньше конца урока";
                lessonSlotView.LessonSlots = await _context.LessonSlots.ToListAsync();
                lessonSlotView.IsOpenModalEdit = true;
                return View("Index", lessonSlotView);
            }
            if ((fm.EndTime - fm.StartTime).TotalMinutes != 45)
            {
                TempData["Error"] = "Урок должен длиться 45 минут";
                lessonSlotView.LessonSlots = await _context.LessonSlots.ToListAsync();
                lessonSlotView.IsOpenModalEdit = true;
                return View("Index", lessonSlotView);
            }
            _context.LessonSlots.Add(fm);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var exists = await _context.LessonSlots.FirstOrDefaultAsync(x => x.Id == id);
                if (exists != null)
                {
                    _context.LessonSlots.Remove(exists);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                TempData["Error"] = "Не удалось удалить";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
