using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using ooadproject.Models;

namespace ooadproject.Controllers
{
    public class ExamRegistrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Person> _userManager;

        public ExamRegistrationController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ExamRegistration
        public async Task<IActionResult> Index()
        {
            
            var user = await _userManager.GetUserAsync(User);
            var studentCourses = await _context.StudentCourse.Include(sc => sc.Course).Where(c => c.StudentID == user.Id).ToListAsync();
            var courses = studentCourses.Select(c => c.Course).ToList();
            var exams = await _context.Exam.Include(e => e.Course).Where(e => courses.Contains(e.Course) && e.Time > DateTime.Now).ToListAsync();

            var registrations = await _context.ExamRegistration.Include(er => er.Exam).Where(er => exams.Contains(er.Exam)).ToListAsync();
            var registeredExams = registrations.Select(r => r.Exam);

            var openedExams = registrations.Where(r => exams.Contains(r.Exam) && !registeredExams.Contains(r.Exam)).ToList();

            ViewData["RegisteredExams"] = registeredExams;
            ViewData["OpenedExams"] = openedExams;

            return View(openedExams);
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

  


        // POST: ExamRegistration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,StudentID,ExamID,RegistrationTime")] ExamRegistration examRegistration)
        {
            // pass ExamID via different controller
            int examID = (int)TempData["ExamID"];
            var user = await _userManager.GetUserAsync(User);
            examRegistration.StudentID = user.Id;
            examRegistration.ExamID = examID;
            examRegistration.RegistrationTime = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(examRegistration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExamID"] = new SelectList(_context.Exam, "ID", "ID", examRegistration.ExamID);
            ViewData["StudentID"] = new SelectList(_context.Student, "Id", "Id", examRegistration.StudentID);
            return View(examRegistration);
        }

  
        // POST: ExamRegistration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.ExamRegistration == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ExamRegistration'  is null.");
            }
            var examRegistration = await _context.ExamRegistration.FindAsync(id);
            if (examRegistration != null)
            {
                _context.ExamRegistration.Remove(examRegistration);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamRegistrationExists(int id)
        {
          return (_context.ExamRegistration?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
