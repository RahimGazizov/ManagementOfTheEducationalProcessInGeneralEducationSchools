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
        public async Task<IActionResult> Delete(string id)
        {
            var exists = await _context.AcademicYear.FindAsync(id);
            if (exists != null)
            {
                _context.AcademicYear.Remove(exists);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(AcademicYearViewModel viewModel)
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
    }
}
