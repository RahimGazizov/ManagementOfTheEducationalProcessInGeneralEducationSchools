using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
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
        private readonly LessonSlotService _lessonService;
        public LessonSlotController(AppDbContext context, LessonSlotService lessonSlotService)
        {
            _context = context;
            _lessonService = lessonSlotService;
        }

        public async Task<IActionResult> Index()
        {
            var lessonSlot = new LessonSlotViewModal
            {
                LessonSlots = await _context.LessonSlots.OrderBy(t => t.LessonNumber).ToListAsync()
            };
            return View(lessonSlot);
        }
        public async Task<IActionResult> AddLessonSlot(LessonSlotViewModal lessonSlotView)
        {
            try
            {
                var result = await _lessonService.AddLessonSlot(lessonSlotView.Form);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    lessonSlotView.LessonSlots = await _context.LessonSlots.ToListAsync();
                    lessonSlotView.IsOpenModalAdd = true;
                    return View("Index", lessonSlotView);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorIndex"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> EditLessonSlot(LessonSlotViewModal lessonSlotView)
        {
            try
            {
                var result = await _lessonService.EditLessonSlot(lessonSlotView.Form);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    lessonSlotView.LessonSlots = await _context.LessonSlots.ToListAsync();
                    lessonSlotView.IsOpenModalEdit = true;
                    return View("Index", lessonSlotView);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorIndex"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _lessonService.Delete(id);
                if (!result.Success)
                {
                    TempData["ErrorIndex"] = result.Message;
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorIndex"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}

