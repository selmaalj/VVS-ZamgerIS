using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ooadproject.Controllers;
using ooadproject.Data;
using ooadproject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTests
{
    [TestClass]
    public class StudentExamControllerTests
    {
        private ApplicationDbContext _context;
        private Mock<ApplicationDbContext> _mockContext;
        private Mock<UserManager<Person>> _mockUserManager;
        private Mock<IPasswordHasher<Person>> _mockPasswordHasher;
        private StudentExamController _studentexamController;

        [TestInitialize]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options;
            _context = new ApplicationDbContext(options);
            //_userManager = new Microsoft.AspNetCore.Identity.UserManager<Person>(Person, null, null, null, null, null, null, null, null);

            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<UserManager<Person>>(new Mock<IUserStore<Person>>().Object, null, null, null, null, null, null, null, null);
            _mockPasswordHasher = new Mock<IPasswordHasher<Person>>();

            _studentexamController = new StudentExamController(_context, null);
        }
        [TestMethod]
        public async Task Index_ReturnsViewResult_WhenStudentCourseInNotNull()
        {
            // Arrange
            // Act
            var result = await _studentexamController.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _studentexamController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenStudentCourseIsNull()
        {
            // Arrange
            int? id = 9999;

            // Act
            var result = await _studentexamController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        /*
        [TestMethod]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange

            var student = new Student { FirstName = "Selma", LastName = "Selma", UserName = "selma", Email = "selmal@example.com", Index = 12345, Department = "RI", Year = 3 };
            var teacher = new Teacher { Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            var course = new Course { Name = "Vjerovatnoća i statistika", Teacher = teacher, TeacherID = teacher.Id, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            var studentCourse = new StudentCourse { ID = 99, CourseID = course.ID, StudentID = student.Id, Student = student, Course = course, Points = 75, Grade = 8 };
            var exam = new Exam { CourseID = 1, Type = ExamType.Midterm, MinimumPoints = 50, TotalPoints = 100, Time = DateTime.Now };
            var studentExam = new StudentExam { Exam=exam, ExamID=exam.ID, Course = studentCourse, CourseID= studentCourse.ID , PointsScored= 90, IsPassed = true};

            var studentController = new StudentController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var teacherController = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var courseController = new CourseController(_mockContext.Object, _mockUserManager.Object);
            var studentCourseController = new StudentCourseController(_mockContext.Object, _mockUserManager.Object);
            var examController = new ExamController(_mockContext.Object, _mockUserManager.Object);
            var studentExamController = new StudentExamController(_mockContext.Object, _mockUserManager.Object);
            // Act
            var resultTeacher = await teacherController.Create(teacher) as RedirectToActionResult;
            var resultCourse = await courseController.Create(course) as RedirectToActionResult;
            var resultStudent = await studentController.Create(student) as RedirectToActionResult;
            var resultStudentCourse = await studentCourseController.Create(studentCourse) as RedirectToActionResult;
            var result = await studentExamController.Create(2,studentExam) as RedirectToActionResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
       */


        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenCourseIsNull()
        {
            // Arrange
            int? id = 99;

            // Act
            var result = await _studentexamController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResult_WhenCourseIsNotNull()
        {
            // Arrange
            var id = 189;

            // Act
            var result = await _studentexamController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_DifferentIds()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _studentexamController.Edit(id, new StudentExam
            { ID = 10, CourseID = 1, ExamID = 1, PointsScored = 90, IsPassed = true });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_SameIds()
        {
            // Arrange
            var id = 189;

            // Act
            var result = await _studentexamController.Edit(id, new StudentExam
            { ID = 189, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Edit_SameIdsThatDontExist()
        {
            // Arrange
            var id = 99;

            // Act
            var result = await _studentexamController.Edit(id, new StudentExam
            { ID = 990, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        
        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public async Task Edit_SameIdsExeption()
        {
            // Arrange
            var studentExam = new StudentExam { ID = 189, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true };
            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());
            var studentExams = new List<StudentExam>
            {
                new StudentExam { ID = 189, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true },
                new StudentExam { ID = 111, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true },
                new StudentExam { ID = 114, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true },
                new StudentExam { ID = 124, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true }
        };
            var mockSet = new Mock<DbSet<StudentExam>>();
            mockSet.As<IQueryable<StudentExam>>().Setup(m => m.Provider).Returns(studentExams.AsQueryable().Provider);
            mockSet.As<IQueryable<StudentExam>>().Setup(m => m.Expression).Returns(studentExams.AsQueryable().Expression);
            mockSet.As<IQueryable<StudentExam>>().Setup(m => m.ElementType).Returns(studentExams.AsQueryable().ElementType);
            mockSet.As<IQueryable<StudentExam>>().Setup(m => m.GetEnumerator()).Returns(studentExams.AsQueryable().GetEnumerator());
            _mockContext.Setup(c => c.StudentExam).Returns(mockSet.Object);
            var controller = new StudentExamController(_mockContext.Object, _mockUserManager.Object);

            //Act
            var result = await controller.Edit(189, studentExam);
        }

        [TestMethod]
        public async Task Edit_SameIdsDoesntExits()
        {
            // Arrange
            var studentExam = new StudentExam { ID = 11, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true };
            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());
            var studentExams = new List<StudentExam>
            {
                new StudentExam { ID = 189, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true },
                new StudentExam { ID = 111, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true },
                new StudentExam { ID = 114, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true },
                new StudentExam { ID = 124, CourseID = 2, ExamID = 1, PointsScored = 90, IsPassed = true }
        };
            var mockSet = new Mock<DbSet<StudentExam>>();
            mockSet.As<IQueryable<StudentExam>>().Setup(m => m.Provider).Returns(studentExams.AsQueryable().Provider);
            mockSet.As<IQueryable<StudentExam>>().Setup(m => m.Expression).Returns(studentExams.AsQueryable().Expression);
            mockSet.As<IQueryable<StudentExam>>().Setup(m => m.ElementType).Returns(studentExams.AsQueryable().ElementType);
            mockSet.As<IQueryable<StudentExam>>().Setup(m => m.GetEnumerator()).Returns(studentExams.AsQueryable().GetEnumerator());
            _mockContext.Setup(c => c.StudentExam).Returns(mockSet.Object);
            var controller = new StudentExamController(_mockContext.Object, _mockUserManager.Object);

            //Act
            var result = await controller.Edit(11, studentExam);

            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        [TestMethod]
        public async Task Delete_IsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _studentexamController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_DoesntExist()
        {
            // Arrange
            int? id = 99;

            // Act
            var result = await _studentexamController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_Exist()
        {
            // Arrange
            int? id = 189;

            // Act
            var result = await _studentexamController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

    }
}
