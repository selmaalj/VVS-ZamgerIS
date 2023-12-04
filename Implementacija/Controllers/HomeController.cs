using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ooadproject.Data;
using ooadproject.Models;
using System.Diagnostics;

namespace ooadproject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Person> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext, UserManager<Person> userManager)
        {
            _logger = logger;
            _context = applicationDbContext;
            _userManager = userManager;
        }

        public async Task<string> GetUserRoleAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var role = await _userManager.GetRolesAsync(user);
            return role[0]; // always a single role
        }
        public int GetExamRegistrations(int id)
        {
            return _context.ExamRegistration.Where(er => er.ExamID == id).CountAsync().Result;

        }

        //redirecting to Home view according to user role

        public async Task<IActionResult> Index() { 
            var role =  await GetUserRoleAsync();
            if(role == "Student")
            {
                return RedirectToAction(nameof(StudentHome));
            }
            else if(role == "StudentService")
            {
                return RedirectToAction(nameof(StudentServiceHome));
            }
            else
            {
                return RedirectToAction(nameof(TeacherHome));
            }     
        }


        public async Task<IActionResult> StudentHome()
        {
            var student = await _userManager.GetUserAsync(User);

            var StudentCourses = await _context.StudentCourse.Include(sc => sc.Course).Where(sc => sc.StudentID == student.Id).ToListAsync();
            ViewData["Courses"] = StudentCourses;

            return View();
        }


        public async Task<IActionResult> TeacherHome()
        {
            var teacher = await _userManager.GetUserAsync(User);

            var Courses = await _context.Course.Where(c => c.TeacherID == teacher.Id).ToListAsync();
            var Exams = new List<Exam>();
            var exams = await _context.Exam.Include(e => e.Course).ToListAsync();
            foreach (var exam in exams)
            {
                if (exam.Course.TeacherID == teacher.Id && exam.Time > DateTime.Now)
                {
                    Exams.Add(exam);
                }
            }
            var registered = new Dictionary<int, int>();
            foreach (var item in Exams)
            {
                registered.Add(item.ID, GetExamRegistrations(item.ID));
            }

            ViewData["Courses"] = Courses;
            ViewData["Exams"] = Exams;
            ViewData["RegisteredForExam"] = registered;

            return View();
        }


        public async Task<IActionResult> StudentServiceHome()
        {
            var teacher = await _userManager.GetUserAsync(User);

            var Courses = await _context.Course.Where(c => c.TeacherID == teacher.Id).ToListAsync();
            var Exams = new List<Exam>();
            var exams = await _context.Exam.Include(e => e.Course).ToListAsync();
            foreach (var exam in exams)
            {
                if (exam.Course.TeacherID == teacher.Id && exam.Time > DateTime.Now)
                {
                    Exams.Add(exam);
                }
            }
            ViewData["Courses"] = Courses;
            ViewData["Exams"] = Exams;

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}