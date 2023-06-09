using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using ooadproject.Models;
using Microsoft.AspNetCore.Identity;

namespace ooadproject.Controllers
{
    public class StudentExamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Person> _userManager;

        public StudentExamController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: StudentExam
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentExam.Include(s => s.Course).Include(s => s.Exam);
            return View(await applicationDbContext.ToListAsync());
        }

        //Create function that returns every type of open exam and homework as a SelectListItem on the specific course by CourseID
        public async Task<IActionResult> TeacherInput(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var courses = await _context.Course.Where(c => c.TeacherID == user.Id).ToListAsync();
            var currentCourse = await _context.Course.FindAsync(id);
            var exams = await _context.Exam.Include(e => e.Course).Where(e => courses.Contains(e.Course)).ToListAsync();
            ViewData["Courses"] = courses;
            ViewData["Exams"] = exams;
            ViewData["CurrentCourse"] = currentCourse;
            return View();
        }

        // GET: StudentExam/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentExam == null)
            {
                return NotFound();
            }

            var studentExam = await _context.StudentExam
                .Include(s => s.Course)
                .Include(s => s.Exam)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (studentExam == null)
            {
                return NotFound();
            }

            return View(studentExam);
        }

        // GET: StudentExam/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.StudentCourse, "ID", "ID");
            ViewData["ExamID"] = new SelectList(_context.Exam, "ID", "ID");
            return View();
        }

        // POST: StudentExam/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CourseID,ExamID,PointsScored,IsPassed")] StudentExam studentExam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentExam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.StudentCourse, "ID", "ID", studentExam.CourseID);
            ViewData["ExamID"] = new SelectList(_context.Exam, "ID", "ID", studentExam.ExamID);
            return View(studentExam);
        }

        // GET: StudentExam/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudentExam == null)
            {
                return NotFound();
            }

            var studentExam = await _context.StudentExam.FindAsync(id);
            if (studentExam == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.StudentCourse, "ID", "ID", studentExam.CourseID);
            ViewData["ExamID"] = new SelectList(_context.Exam, "ID", "ID", studentExam.ExamID);
            return View(studentExam);
        }

        // POST: StudentExam/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CourseID,ExamID,PointsScored,IsPassed")] StudentExam studentExam)
        {
            if (id != studentExam.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentExam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExamExists(studentExam.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.StudentCourse, "ID", "ID", studentExam.CourseID);
            ViewData["ExamID"] = new SelectList(_context.Exam, "ID", "ID", studentExam.ExamID);
            return View(studentExam);
        }

        // GET: StudentExam/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentExam == null)
            {
                return NotFound();
            }

            var studentExam = await _context.StudentExam
                .Include(s => s.Course)
                .Include(s => s.Exam)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (studentExam == null)
            {
                return NotFound();
            }

            return View(studentExam);
        }

        // POST: StudentExam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentExam == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudentExam'  is null.");
            }
            var studentExam = await _context.StudentExam.FindAsync(id);
            if (studentExam != null)
            {
                _context.StudentExam.Remove(studentExam);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExamExists(int id)
        {
          return (_context.StudentExam?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
