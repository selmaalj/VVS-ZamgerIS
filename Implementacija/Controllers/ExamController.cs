using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using ooadproject.Models;

namespace ooadproject.Controllers
{
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Person> _userManager;

        public ExamController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Exam
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Exam.Include(e => e.Course);
            var user = await _userManager.GetUserAsync(User);
            var courses = await _context.Course.Where(c => c.TeacherID == user.Id).ToListAsync();
            var exams = await _context.Exam.Where(e => courses.Contains(e.Course)).ToListAsync();
            return View(exams);
        }

        // GET: Exam/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Exam == null)
            {
                return NotFound();
            }

            var exam = await _context.Exam
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // GET: Exam/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Course, "ID", "ID");
            return View();
        }

        // POST: Exam/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CourseID,Time,Type,TotalPoints,MinimumPoints")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Course, "ID", "ID", exam.CourseID);
            return RedirectToAction(nameof(Index)); 
        }

        // POST: Exam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Exam == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Exam'  is null.");
            }
            var exam = await _context.Exam.FindAsync(id);
            if (exam != null)
            {
                _context.Exam.Remove(exam);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamExists(int id)
        {
          return (_context.Exam?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
