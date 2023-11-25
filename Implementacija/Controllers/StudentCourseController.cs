using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using ooadproject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ooadproject.Controllers
{
    public class StudentCourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Person> _userManager;

        public StudentCourseController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: StudentCourse
        [Authorize(Roles = "StudentService")]
        public async Task<IActionResult> Index()
        {

            var applicationDbContext = _context.StudentCourse.Include(s => s.Course).Include(s => s.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StudentCourse/Details/5
        //studentcoursetstatus
        [Authorize(Roles = "StudentService")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentCourse == null)
            {
                return NotFound();
            }

            var studentCourse = await _context.StudentCourse
                .Include(s => s.Course)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (studentCourse == null)
            {
                return NotFound();
            }

            return View(studentCourse);
        }

        // GET: StudentCourse/Create
        [Authorize(Roles = "StudentService")]
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Course, "ID", "Name");


            List<SelectListItem> Students = new List<SelectListItem>();

            foreach (var item in _context.Student)
            {
                Students.Add(new SelectListItem() { Text = $"{item.FirstName} {item.LastName}", Value = item.Id.ToString() });
            }

            ViewData["StudentID"] = Students;
            return View();
        }

        // POST: StudentCourse/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "StudentService")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CourseID,StudentID")] StudentCourse studentCourse)
        {
            studentCourse.Points = 0;
            studentCourse.Grade = 5;
            if (ModelState.IsValid)
            {
                _context.Add(studentCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Course, "ID", "ID", studentCourse.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Student, "Id", "Id", studentCourse.StudentID);
            return View(studentCourse);
        }

        [Authorize(Roles = "StudentService")]
        // POST: StudentCourse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.StudentCourse == null)
            {
                return Problem("Entity set 'ApplicationDbContext.StudentCourse'  is null.");
            }
            var studentCourse = await _context.StudentCourse.FindAsync(id);
            if (studentCourse != null)
            {
                _context.StudentCourse.Remove(studentCourse);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentCourseStatus(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var courses = await _context.StudentCourse.Include(sc => sc.Course).Where(sc => sc.StudentID == user.Id).ToListAsync();
            var StudentCourse = await _context.StudentCourse.FindAsync(id);
            if (StudentCourse != null && StudentCourse.Course != null) {
                StudentCourse.Course.Teacher = await _context.Teacher.FindAsync(StudentCourse.Course.TeacherID);
            }
            //Set Teacher for every StudentCourse.Course
            foreach (var item in courses)
            {
                if (item.Course != null)
                {
                    item.Course.Teacher = await _context.Teacher.FindAsync(item.Course.TeacherID);
                }
            }

            var StudentExams = await _context.StudentExam.Where(se => se.CourseID == id).ToListAsync();
            var StudentHomeworks = await _context.StudentHomework.Where(sh => sh.CourseID == id).ToListAsync();

            var Activities = new List<IActivity>();

            var studentExams = await _context.StudentExam
            .Include(se => se.Exam)
            .Where(se => se.CourseID == id)
            .Select(se => new { se.PointsScored, se.Exam!.TotalPoints })
            .ToListAsync();

            var studentHomeworks = await _context.StudentHomework
                .Include(sh => sh.Homework)
                .Where(sh => sh.CourseID == id)
                .Select(sh =>  new { sh.PointsScored, sh.Homework!.TotalPoints })
                .ToListAsync();

            double scored = studentExams.Sum(se => se.PointsScored) + studentHomeworks.Sum(sh => sh.PointsScored);
            double maxPossible = studentExams.Sum(se => se.TotalPoints) + studentHomeworks.Sum(sh => sh.TotalPoints);

            double Total = 0;

            foreach (StudentExam item in StudentExams)
            {
                Activities.Add(item);
                item.Exam = await _context.Exam.FindAsync(item.ExamID);
                Total += item.GetTotalPoints();
            }
            foreach (StudentHomework item in StudentHomeworks)
            {
                Activities.Add(item);
                item.Homework = await _context.Homework.FindAsync(item.HomeworkID);
                Total += item.GetTotalPoints();
            }

            // ovdje sve sto treba za ovaj view
            ViewData["Courses"] = courses;
            ViewData["StudentCourse"] = StudentCourse;
            ViewData["Activities"] = Activities;
            ViewData["PointsScored"] = scored;
            ViewData["TotalPoints"] = scored;
            ViewData["MaxPossible"] = maxPossible;
            return View();
        }
        [Authorize(Roles = "Student")]
        //View that shows the status of all courses for a student based on the year of study
        public async Task<IActionResult> StudentOverallStatus(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var courses = await _context.StudentCourse.Include(sc => sc.Course).Where(sc => sc.StudentID == user.Id).ToListAsync();

            //Create a set of courses where the grade is bigger than 5
            var CoursesWithGrade = new List<StudentCourse>();
            foreach (var item in courses)
            {
                if (item.Grade > 5)
                {
                    CoursesWithGrade.Add(item);
                }
            }

            ViewData["GradedCourses"] = CoursesWithGrade.OrderByDescending(c =>
            {
                return c.Course!.Semester;
            }).ThenBy(c =>
            {
                return c.Course?.Name;
            })
            .ToList();

            ViewData["Courses"] = courses;
            //Calculate the average grade for all courses
            double AverageGrade = 0;
            foreach (var item in CoursesWithGrade)
            {
                AverageGrade += item.Grade;
            }
            AverageGrade /= CoursesWithGrade.Count;
            ViewData["AverageGrade"] = AverageGrade;

            return View();
        }

        private bool StudentCourseExists(int id)
        {
          return (_context.StudentCourse?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
