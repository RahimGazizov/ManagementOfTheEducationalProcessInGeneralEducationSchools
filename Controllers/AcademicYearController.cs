using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class AcademicYearController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AcademicYearLogic _yearLogic;
        public AcademicYearController(AppDbContext context, AcademicYearLogic yearLogic)
        {
            _context = context;
            _yearLogic = yearLogic;
        }
        public async Task<IActionResult> Index()
        {
            var academic = await _context.AcademicYear
                .Include(t => t.Terms)
                .ToListAsync();
            var viewAcademic = new AcademicYearViewModel
            {
                AcademicYears = academic,
            };
            return View(viewAcademic);
        }
        public async Task<IActionResult> AddAcademicYear(AcademicYearViewModel academicYear)
        {
            try
            {
                var fm = academicYear.From;
                var result = await _yearLogic.AddAcademicYear(fm);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    academicYear.AcademicYears = await _context.AcademicYear
                    .Include(t => t.Terms)
                    .ToListAsync();
                    academicYear.IsAdd = true;
                    return View("Index", academicYear);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                academicYear.AcademicYears = await _context.AcademicYear
                .Include(t => t.Terms)
                .ToListAsync();
                academicYear.IsAdd = true;
                return View("Index", academicYear);
            }
        }
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
               var result = await _yearLogic.Delete(id);
                if (!result.Success)
                {
                    TempData["ErrorIndex"] = result.Message;
                }
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["ErrorIndex"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Edit(AcademicYearViewModel viewModel)
        {
            try
            {
                var fm = viewModel.From;
                var result = await _yearLogic.EditAcademicYear(fm);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    viewModel.AcademicYears = await _context.AcademicYear
                    .Include(t => t.Terms)
                    .ToListAsync();
                    viewModel.IsEdit = true;
                    return View("Index", viewModel);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                viewModel.AcademicYears = await _context.AcademicYear
                .Include(t => t.Terms)
                .ToListAsync();
                viewModel.IsEdit = true;
                return View("Index", viewModel);
            }
        }
    }
}
