using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class TermController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TermLogic _term;
        public TermController(AppDbContext context, TermLogic term)
        {
            _context = context;
            _term = term;
        }
        public async Task<IActionResult> Index()
        {
            var term = await _context.Term
                .Include(t => t.AcademicYear)
                .ToListAsync();
            var termView = new TermViewModel
            {
                Terms = term
            };
            ViewBag.AcademicList = _term.ListAcademicYear();
            return View(termView);
        }
        public async Task<IActionResult> AddTerm(TermViewModel termView)
        {
            var fm = termView.Form;
            var result = await _term.AddTerm(fm);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                termView.Terms = await _context.Term
                .Include(t => t.AcademicYear)
                .ToListAsync();
                termView.IsAdd = true;
                ViewBag.AcademicList = _term.ListAcademicYear();
                return View("Index", termView);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditTerm(TermViewModel termView)
        {
            var fm = termView.Form;
            var result = await _term.EditTerm(fm);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                termView.Terms = await _context.Term
                .Include(t => t.AcademicYear)
                .ToListAsync();
                termView.IsEdit = true;
                ViewBag.AcademicList = _term.ListAcademicYear();
                return View("Index", termView);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _term.Delete(id);
            if (!result.Success)
            {
                TempData["ErrorIndex"] = result.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
