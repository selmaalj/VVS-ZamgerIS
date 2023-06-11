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
        public async Task<IActionResult> Index()
        {

            var applicationDbContext = _context.StudentCourse.Include(s => s.Course).Include(s => s.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StudentCourse/Details/5
        //studentcoursetstatus
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

        public async Task<IActionResult> StudentCourseStatus(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var courses = await _context.StudentCourse.Include(sc => sc.Course).Where(sc => sc.StudentID == user.Id).ToListAsync();
            var StudentCourse = await _context.StudentCourse.FindAsync(id);
            StudentCourse.Course.Teacher = await _context.Teacher.FindAsync(StudentCourse.Course.TeacherID);
            //Set Teacher for every StudentCourse.Course
            foreach (var item in courses)
            {
                item.Course.Teacher = await _context.Teacher.FindAsync(item.Course.TeacherID);
            }
   
            var StudentExams = await _context.StudentExam.Where(se => se.CourseID == id).ToListAsync();
            var StudentHomeworks = await _context.StudentHomework.Where(sh => sh.CourseID == id).ToListAsync();

            var Activities = new List<IActivity>();

            double Total = 0, Scored = 0, MaxPossible = 0 ;

            foreach (var item in StudentExams)
            {
                Activities.Add(item);
                Scored += item.GetPointsScored();
                Total += item.GetTotalPoints();
                MaxPossible += item.Exam.TotalPoints;
            }
            foreach (var item in StudentHomeworks)
            {
                Activities.Add(item);
                Scored += item.GetPointsScored();
                MaxPossible += item.Homework.TotalPoints;
                Total += item.GetTotalPoints();
            }

            // ovdje sve sto treba za ovaj view
            ViewData["Courses"] = courses;
            ViewData["StudentCourse"] = StudentCourse;
            ViewData["Activities"] = Activities;
            ViewData["PointsScored"] = Scored;
            ViewData["TotalPoints"] = Scored;
            ViewData["MaxPossible"] = MaxPossible;
            return View();
        }
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
            ViewData["GradedCourses"] = CoursesWithGrade.OrderByDescending(c => c.Course.Semester).ThenBy(c => c.Course.Name).ToList();
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
