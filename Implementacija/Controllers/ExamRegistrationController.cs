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
            var courses = studentCourses.Select(c => c.CourseID).ToList();
            var exams = await _context.Exam.Include(e => e.Course).Where(e => courses.Contains(e.CourseID) && e.Time > DateTime.Now).ToListAsync();
            var examsID = exams.Select(exams => exams.ID).ToList();

            //Get all exams that a student has registered for
            var registrations = await _context.ExamRegistration.Include(er => er.Exam).Where(er => er.StudentID == user.Id).ToListAsync();
            //Get all open exams that a student has not registered for and where the date is not passed
            var openedExams = exams.Where(e => !registrations.Select(r => r.ExamID).Contains(e.ID)).ToList();            
            

            ViewData["RegisteredExams"] = registrations;
            ViewData["OpenedExams"] = openedExams;
            ViewData["Courses"] = studentCourses;

            return View();
        }
        // POST: ExamRegistration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int examID, [Bind("StudentID,ExamID,RegistrationTime")] ExamRegistration examRegistration)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                examRegistration.StudentID = user.Id;
                examRegistration.ExamID = examID;
                examRegistration.RegistrationTime = DateTime.Now;

                await _context.ExamRegistration.AddAsync(examRegistration);
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
