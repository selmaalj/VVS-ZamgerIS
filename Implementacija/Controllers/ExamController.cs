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

        public List<SelectListItem> GetExamTypesList()
        {
            List<SelectListItem> Types = new List<SelectListItem>();
            var EnumValues = Enum.GetValues(typeof(ExamType));

            foreach (var value in EnumValues)
            {
                Types.Add(new SelectListItem
                {
                    Text = value.ToString(),
                    Value = ((int)value).ToString()
                });
            }

            return Types;

        }
        public async Task<List<SelectListItem>> GetCourseNamesList()
        {
            var user = await _userManager.GetUserAsync(User);

            var Courses = new List<SelectListItem>();
            
            var UserCourses = await _context.Course.Where(c => c.Teacher.Id == user.Id).ToListAsync();
            foreach (var item in UserCourses)
            {
                Courses.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString() });
            }

            return Courses;

        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Exam.Include(e => e.Course);
            var user = await _userManager.GetUserAsync(User);
            var courses = await _context.Course.Where(c => c.TeacherID == user.Id).ToListAsync();
            var exams = await _context.Exam.Where(e => courses.Contains(e.Course)).ToListAsync();
            return View(exams);
        }

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


        public async Task<IActionResult> Create()
        {   

            ViewData["CourseID"] = await GetCourseNamesList();
            ViewData["ExamTypes"] = GetExamTypesList();
          //  ViewData["CourseNames"] = ;
            return View();
        }

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
