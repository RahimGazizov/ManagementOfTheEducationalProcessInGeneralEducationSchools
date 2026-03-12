using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Админ")]
    public class ScheduleLessonController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ScheduleLogic _scheduleLogic;
        public ScheduleLessonController(AppDbContext context, ScheduleLogic scheduleLogic)
        {
            _context = context;
            _scheduleLogic = scheduleLogic;
        }
        public async Task<IActionResult> Index()
        {
            var schedule = await _scheduleLogic.ForIndex();

            ViewBag.DaysOfWeek = _scheduleLogic.GetListDaysOfWeek();
            ViewBag.Assignment = await _scheduleLogic.ListAssigment();
            ViewBag.LessonSlot = await _scheduleLogic.LessonSlotList();
            return View(schedule);
        }
        [HttpPost]
        public async Task<IActionResult> AddSchedule(ScheduleLessonViewModal schedule)
        {
            var fm = schedule.FormSchedule;
            var result = await _scheduleLogic.AddScheduleClass(fm);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                schedule.ScheduleLessons = await _scheduleLogic.ScheduleLessons();
                schedule.IsOpenModalAdd = true;
                ViewBag.DaysOfWeek = _scheduleLogic.GetListDaysOfWeek();
                ViewBag.Assignment = await _scheduleLogic.ListAssigment();
                ViewBag.LessonSlot = await _scheduleLogic.LessonSlotList();
                return View("Index", schedule);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditSchedule(ScheduleLessonViewModal schedule)
        {
            var fm = schedule.FormSchedule;
            var result = await _scheduleLogic.EditScheduleClass(fm);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                schedule.ScheduleLessons = await _scheduleLogic.ScheduleLessons();
                schedule.IsOpenModalAdd = true;
                ViewBag.DaysOfWeek = _scheduleLogic.GetListDaysOfWeek();
                ViewBag.Assignment = await _scheduleLogic.ListAssigment();
                ViewBag.LessonSlot = await _scheduleLogic.LessonSlotList();
                return View("Index", schedule);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string id)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
