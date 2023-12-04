using Google.Apis.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ooadproject.Controllers;
using ooadproject.Data;
using ooadproject.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectTests
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private Mock<ILogger<HomeController>> _loggerMock;
        public DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private Mock<UserManager<Person>> _userManagerMock;

        private List<StudentCourse> studentCourses;
        private List<Course> courses;
        private List<Exam> exams;
        private List<ExamRegistration> examRegistrations;
        private Teacher teacher;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase").EnableSensitiveDataLogging().Options;

            _loggerMock = new Mock<ILogger<HomeController>>();
            //_contextMock = new Mock<ApplicationDbContext>(_dbContextOptions);
            _userManagerMock = new Mock<UserManager<Person>>(Mock.Of<IUserStore<Person>>(), null, null, null, null, null, null, null, null);

            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                // Add sample data to the in-memory database
                var student = new Student { UserName = "teststudent", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Index = 12345, Department = "Computer Science", Year = 2 };
                teacher = new Teacher { UserName = "testteacher", FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", Title = "dr." };
                var studentService = new StudentService();
                courses = new List<Course>
                {
                    new Course { Name = "English", AcademicYear = "2023-2024", ECTS = 6, Semester = 3, TeacherID = teacher.Id, Teacher = teacher },
                    new Course { Name = "Math", AcademicYear = "2023-2024", ECTS = 6, Semester = 3, TeacherID = teacher.Id, Teacher = teacher },
                    new Course { Name = "History", AcademicYear = "2023-2024", ECTS = 6, Semester = 3, TeacherID = teacher.Id, Teacher = teacher }
                };
                studentCourses = new List<StudentCourse>
                {
                    new StudentCourse { CourseID = courses[0].ID, Course = courses[0], StudentID = student.Id, Student = student, Grade = 8, Points = 73.5 },
                    new StudentCourse { CourseID = courses[1].ID, Course = courses[1], StudentID = student.Id, Student = student },
                    new StudentCourse { CourseID = courses[2].ID, Course = courses[2], StudentID = student.Id, Student = student }
                };
                exams = new List<Exam>
                {
                    new Exam { CourseID = courses[0].ID, Time = DateTime.Now.AddDays(1), Course = courses[0], MinimumPoints = 10, TotalPoints = 20, Type = ExamType.Final },
                    new Exam { CourseID = courses[1].ID, Time = DateTime.Now.AddDays(5), Course = courses[1], MinimumPoints = 10, TotalPoints = 20, Type = ExamType.Midterm },
                    new Exam { CourseID = courses[2].ID, Time = DateTime.Now.AddDays(7), Course = courses[2], MinimumPoints = 10, TotalPoints = 20, Type = ExamType.Test }
                };
                examRegistrations = new List<ExamRegistration>
                {
                    new ExamRegistration { StudentID = 1, Student = student, ExamID = exams[0].ID, Exam = exams[0], RegistrationTime = DateTime.Now },
                    new ExamRegistration { StudentID = 1, Student = student, ExamID = exams[1].ID, Exam = exams[1], RegistrationTime = DateTime.Now },
                    new ExamRegistration { StudentID = 1, Student = student, ExamID = exams[1].ID, Exam = exams[1], RegistrationTime = DateTime.Now }
                };
                context.AddRange(student, teacher,
                    courses[0], courses[1], courses[2],
                    studentCourses[0], studentCourses[1], studentCourses[2],
                    exams[0], exams[1], exams[2],
                    examRegistrations[0], examRegistrations[1], examRegistrations[2]);
                context.SaveChanges();

                // Mock a user with a non-null Id
                _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                    .ReturnsAsync(student);
            }

            _controller = new HomeController(_loggerMock.Object, new ApplicationDbContext(_dbContextOptions), _userManagerMock.Object);
        }

        [TestMethod]
        public async Task GetUserRoleAsync_ReturnsUserRole()
        {
            // Arrange
            var user = new Student();

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Student" });

            // Act
            var result = await _controller.GetUserRoleAsync();

            // Assert
            Assert.AreEqual("Student", result);
        }

        [TestMethod]
        public void GetExamRegistrations_ReturnsCorrectCount()
        {
            var count0 = _controller.GetExamRegistrations(exams[0].ID); //1
            var count1 = _controller.GetExamRegistrations(exams[1].ID); //2
            var count2 = _controller.GetExamRegistrations(exams[2].ID); //0

            Assert.AreEqual(1, count0);
            Assert.AreEqual(2, count1);
            Assert.AreEqual(0, count2);
        }

        [TestMethod]
        public async Task Index_RedirectsToStudentHome_WhenUserRoleIsStudent()
        {
            // Arrange
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Student());
            _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<Student>())).ReturnsAsync(new List<string> { "Student" });

            // Act
            var result = await _controller.Index() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("StudentHome", result.ActionName);
        }

        [TestMethod]
        public async Task Index_RedirectsToStudentServiceHome_WhenUserRoleIsStudentService()
        {
            // Arrange
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new StudentService());
            _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<StudentService>())).ReturnsAsync(new List<string> { "StudentService" });

            // Act
            var result = await _controller.Index() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("StudentServiceHome", result.ActionName);
        }

        [TestMethod]
        public async Task Index_RedirectsToTeacherHome_WhenUserRoleIsTeacher()
        {
            // Arrange
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Teacher());
            _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<Teacher>())).ReturnsAsync(new List<string> { "Teacher" });

            // Act
            var result = await _controller.Index() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TeacherHome", result.ActionName);
        }

        [TestMethod]
        public void Privacy_ReturnsView()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Error_ReturnsViewResultWithExpectedViewName()
        {
            var httpContext = new Mock<HttpContext>();
            var activity = new Activity("TestActivity");
            httpContext.Setup(c => c.TraceIdentifier).Returns("some_trace_identifier");
            Activity.Current = activity;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext.Object };

            // Act
            var result = _controller.Error();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as ErrorViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(_controller.ViewData.Model, typeof(ErrorViewModel));
            Assert.IsNotNull(model.RequestId);
        }

        [TestMethod]
        public void Error_ReturnsViewResultWhenActivityIsNotNull()
        {
            using (Activity unitTestActivity = new Activity("UnitTest").Start())
            {
                // Act
                var result = _controller.Error();
                var viewResult = result as ViewResult;
                var model = viewResult.Model as ErrorViewModel;

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(_controller.ViewData.Model, typeof(ErrorViewModel));
                Assert.IsNotNull(model.RequestId);
            }
        }

        [TestMethod]
        public async Task StudentHome_ReturnsViewResultWithCourses()
        {
            var result = await _controller.StudentHome();
            var viewResult = result as ViewResult;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsNotNull(viewResult.ViewData);
            Assert.IsTrue(viewResult.ViewData.ContainsKey("Courses"));
            Assert.IsInstanceOfType(viewResult.ViewData["Courses"], typeof(List<StudentCourse>));
            Assert.AreEqual(studentCourses.Count, ((List<StudentCourse>)viewResult.ViewData["Courses"]).Count);
        }

        [TestMethod]
        public async Task TeacherHome_ReturnsViewResultWithCoursesAndExams()
        {
            // Arrange
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(teacher);

            // Act
            var result = await _controller.TeacherHome();
            var viewResult = result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            // Check that ViewData keys exist
            Assert.IsTrue(viewResult.ViewData.ContainsKey("Courses"));
            Assert.IsTrue(viewResult.ViewData.ContainsKey("Exams"));
            Assert.IsTrue(viewResult.ViewData.ContainsKey("RegisteredForExam"));
            // Assert that ViewData["Courses"] and ViewData["Exams"] are set correctly
            Assert.AreEqual(courses.Count, ((List<Course>)viewResult.ViewData["Courses"]).Count);
            Assert.AreEqual(exams.Count, ((List<Exam>)viewResult.ViewData["Exams"]).Count);
            //Check ViewData types
            Assert.IsInstanceOfType(viewResult.ViewData["Courses"], typeof(List<Course>));
            Assert.IsInstanceOfType(viewResult.ViewData["Exams"], typeof(List<Exam>));
            Assert.IsInstanceOfType(viewResult.ViewData["RegisteredForExam"], typeof(Dictionary<int, int>));
        }

        [TestMethod]
        public async Task StudentServiceHome_ReturnsViewResultWithCoursesAndExams()
        {
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(teacher);

            // Act
            var result = await _controller.StudentServiceHome();
            var viewResult = result as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            // Check that ViewData keys exist
            Assert.IsTrue(viewResult.ViewData.ContainsKey("Courses"));
            Assert.IsTrue(viewResult.ViewData.ContainsKey("Exams"));
            // Assert that ViewData["Courses"] and ViewData["Exams"] are set correctly
            Assert.AreEqual(courses.Count, ((List<Course>)viewResult.ViewData["Courses"]).Count);
            // Assert that ViewData["Exams"] includes the exams with a non-null Course
            Assert.AreEqual(exams.Count, ((List<Exam>)viewResult.ViewData["Exams"]).Count);
            // Check ViewData types
            Assert.IsInstanceOfType(viewResult.ViewData["Courses"], typeof(List<Course>));
            Assert.IsInstanceOfType(viewResult.ViewData["Exams"], typeof(List<Exam>));
        }
    }
}
