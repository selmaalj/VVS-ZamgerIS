using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ooadproject.Controllers;
using ooadproject.Data;
using ooadproject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTests
{
    [TestClass]
    public class StudentCourseControllerTests
    {
        private ApplicationDbContext _context;
        private StudentCourseController _studentCourseController;
        private StudentController _studentController;
        private CourseController _courseController;
        private TeacherController _teacherController;
        private Mock<ApplicationDbContext> _mockContext;
        private Mock<UserManager<Person>> _mockUserManager;
        private Mock<IPasswordHasher<Person>> _mockPasswordHasher;

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

            _courseController = new CourseController(_context, null);
            _teacherController = new TeacherController(_context, null, null);
            _studentCourseController = new StudentCourseController(_context, null);
            _studentController = new StudentController(_context, null, null);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _studentCourseController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenStudentCourseIsNull()
        {
            // Arrange
            int? id = 9999;

            // Act
            var result = await _studentCourseController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        [TestMethod]
        public async Task Create_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _studentCourseController.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }



        [TestMethod]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange

            var student = new Student { FirstName = "Selma", LastName = "Selma", UserName = "selma", Email = "selmal@example.com", Index = 12345, Department = "RI", Year = 3 };
            var teacher = new Teacher { Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            var course = new Course { Name = "Vjerovatnoća i statistika", Teacher = teacher, TeacherID = teacher.Id, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            var studentCourse = new StudentCourse { ID = 99, CourseID = course.ID, StudentID = student.Id, Student = student, Course = course, Points = 75, Grade = 8 };

            var studentController = new StudentController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var teacherController = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var courseController = new CourseController(_mockContext.Object, _mockUserManager.Object);
            var studentCourseController = new StudentCourseController(_mockContext.Object, _mockUserManager.Object);
            // Act
            var resultTeacher = await teacherController.Create(teacher) as RedirectToActionResult;
            var resultCourse = await courseController.Create(course) as RedirectToActionResult;
            var resultStudent = await studentController.Create(student) as RedirectToActionResult;
            var resultStudentCourse = await studentCourseController.Create(studentCourse) as RedirectToActionResult;

            // Assert

            Assert.IsNotNull(resultStudentCourse);
            Assert.AreEqual("Index", resultStudentCourse.ActionName);
        }

        [TestMethod]
        public async Task Create_InvalidModel_ReturnsViewResultWithModel()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course{ ID = 1, Name = "Test", TeacherID = 21, AcademicYear = "2022", ECTS = 6, Semester = 1 },
                new Course{ ID = 2, Name = "Test2", TeacherID = 21, AcademicYear = "2022", ECTS = 6, Semester = 1 },
                new Course{ ID = 3, Name = "Test3", TeacherID = 21, AcademicYear = "2022", ECTS = 6, Semester = 1 },
                new Course{ ID = 4, Name = "Test4", TeacherID = 21, AcademicYear = "2022", ECTS = 6, Semester = 1 },
                new Course{ ID = 5, Name = "Test5", TeacherID = 21, AcademicYear = "2022", ECTS = 6, Semester = 1 },
            };
            var mockSet = new Mock<DbSet<Course>>();
            mockSet.As<IQueryable<Course>>().Setup(m => m.Provider).Returns(courses.AsQueryable().Provider);
            mockSet.As<IQueryable<Course>>().Setup(m => m.Expression).Returns(courses.AsQueryable().Expression);
            mockSet.As<IQueryable<Course>>().Setup(m => m.ElementType).Returns(courses.AsQueryable().ElementType);
            mockSet.As<IQueryable<Course>>().Setup(m => m.GetEnumerator()).Returns(courses.AsQueryable().GetEnumerator());
            _mockContext.Setup(c => c.Course).Returns(mockSet.Object);
            var students = new List<Student>
            {
                new Student { Id = 1,  FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" },
                new Student { Id = 2,  FirstName = "Jane", LastName = "Smith", UserName = "janesmith", Email = "janesmith@example.com" },
                new Student { Id = 3,  FirstName = "David", LastName = "Johnson", UserName = "davidjohnson", Email = "davidjohnson@example.com" },
                new Student { Id = 4, FirstName = "Emily", LastName = "Brown", UserName = "emilybrown", Email = "emilybrown@example.com" },
                new Student { Id = 5, FirstName = "Michael", LastName = "Davis", UserName = "michaeldavis", Email = "michaeldavis@example.com" }
            };
            var mockSet1 = new Mock<DbSet<Student>>();
            mockSet1.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.AsQueryable().Provider);
            mockSet1.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.AsQueryable().Expression);
            mockSet1.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.AsQueryable().ElementType);
            mockSet1.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.AsQueryable().GetEnumerator());
            _mockContext.Setup(c => c.Student).Returns(mockSet1.Object);
            var controller = new StudentCourseController(_mockContext.Object, _mockUserManager.Object);
            var studentCourse = new StudentCourse { ID = 99, CourseID = 1, StudentID = 1, Points = 75, Grade = 8 };
            controller.ModelState.AddModelError("error", "some error");

            //Act
            var result = await controller.Create(studentCourse) as ViewResult;


            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(studentCourse, result.Model);
        }

       

        [TestMethod]
        public async Task Index_ReturnsViewResult_WhenStudentCourseInNotNull()
        {
            // Arrange
            // Act
            var result = await _studentCourseController.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenStudentCourseIsNotNull()
        {
            // Arrange
            int? id = 2;
            // Act
            var result = await _studentCourseController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Delete_WrongId()
        {
            // Arrange
            var id = 99;

            // Act
            var result = await _studentCourseController.Delete(id) as RedirectToActionResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Delete_ExistingId()
        {
            // Arrange
            var studentCourse = new StudentCourse { ID = 99, CourseID = 1, StudentID = 1, Points = 75, Grade = 8 };
            _context.StudentCourse.Add(studentCourse);

            // Act
            var result = await _studentCourseController.Delete(studentCourse.ID) as RedirectToActionResult;


            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Delete_ExistingId_WhenStudentCourseIsNull_ReturnsProblem()
        {
            // Arrange
            _context.StudentCourse = null;
            // Act
            var result = await _studentCourseController.Delete(1) as ObjectResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task StudentCourseStatus()
        {
            // Arrange
            var student = new Student { FirstName = "Selma", LastName = "Selma", UserName = "selma", Email = "selmal@example.com", Index = 12345, Department = "RI", Year = 3 };
            var teacher = new Teacher { Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            var course = new Course { Name = "Vjerovatnoća i statistika", Teacher = teacher, TeacherID = teacher.Id, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            var studentCourse = new StudentCourse { ID = 99, CourseID = course.ID, StudentID = student.Id, Student = student, Course = course, Points = 75, Grade = 8 };
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(student);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());

            var mockController = new StudentCourseController(_context, _mockUserManager.Object);
            // Act
            var result = await mockController.StudentCourseStatus(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public async Task StudentCourseStatus_StudentCourseNotNull()
        {
            // Arrange
            var student = new Student { FirstName = "Selma", LastName = "Selma", UserName = "selmatest", Email = "selmal@example.com", Index = 12345, Department = "RI", Year = 3 };
            _context.Student.Add(student);
            _context.SaveChanges();
            var foundStudent = await _context.Student.FirstOrDefaultAsync(m => m.UserName == "selmatest");

            var teacher = new Teacher { Title = "Mr.", FirstName = "test", LastName = "test", UserName = "testtest328", Email = "johndoe@example.com" };
            _context.Teacher.Add(teacher);
            _context.SaveChanges();
            var foundTeacher = await _context.Teacher.FirstOrDefaultAsync(m => m.UserName == "testtest328");

            var course = new Course { Name = "Ne znam ni ja vise", Teacher = foundTeacher, TeacherID = foundTeacher.Id, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            _context.Course.Add(course);
            _context.SaveChanges();
            var foundCourse = await _context.Course.FirstOrDefaultAsync(m => (m.Name == "Ne znam ni ja vise" && m.Teacher.UserName == "testtest328"));

            var studentCourse = new StudentCourse { CourseID = foundCourse.ID, StudentID = foundStudent.Id, Student = foundStudent, Course = foundCourse, Points = 75, Grade = 8 };
            _context.StudentCourse.Add(studentCourse);
            _context.SaveChanges();
            var foundStudentCourse = await _context.StudentCourse.FirstOrDefaultAsync(m => (m.CourseID == foundCourse.ID && m.StudentID == foundStudent.Id));

            var exam = new Exam { CourseID = foundCourse.ID, Time = DateTime.Now, Type = ExamType.Midterm, TotalPoints = 53, MinimumPoints = 15 };
            _context.Exam.Add(exam);
            _context.SaveChanges();
            var foundExam = await _context.Exam.FirstOrDefaultAsync(m => (m.CourseID == exam.CourseID && m.TotalPoints == 53));

            var studentExam = new StudentExam { CourseID = foundStudentCourse.ID, Course = foundStudentCourse, ExamID = foundExam.ID, PointsScored = 16, IsPassed = true };
            _context.StudentExam.Add(studentExam);
            _context.SaveChanges();
            var foundStudentExam = await _context.StudentExam.FirstOrDefaultAsync(m => (m.CourseID == studentExam.CourseID && m.ExamID == studentExam.ExamID));

            var homework = new Homework { CourseID = foundCourse.ID, Deadline = DateTime.Now, TotalPoints = 10, Description = "Boze pomagaj" };
            _context.Homework.Add(homework);
            _context.SaveChanges();
            var foundHomework = await _context.Homework.FirstOrDefaultAsync(m => (m.CourseID == homework.CourseID && m.Description == "Boze pomagaj"));

            var studentHomework = new StudentHomework { CourseID = foundStudentCourse.ID, HomeworkID = foundHomework.ID, PointsScored = 7, Comment = "sda" };
            _context.StudentHomework.Add(studentHomework);
            _context.SaveChanges();
            var foundStudentHomework = await _context.StudentHomework.FirstOrDefaultAsync(m => m.CourseID == studentHomework.CourseID);


            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(student);


            var mockController = new StudentCourseController(_context, _mockUserManager.Object);
            // Act
            var result = await mockController.StudentCourseStatus(foundStudentCourse.ID);


            // Assert
            _context.StudentExam.Remove(foundStudentExam);
            _context.Exam.Remove(foundExam);
            _context.StudentHomework.Remove(foundStudentHomework);
            _context.Homework.Remove(homework);
            _context.StudentCourse.Remove(foundStudentCourse);
            _context.Course.Remove(foundCourse);
            _context.Student.Remove(foundStudent);
            _context.Teacher.Remove(foundTeacher);



            _context.SaveChanges();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task StudentOverallStatus()
        {
            // Arrange
            var student = new Student { FirstName = "Selma", LastName = "Selma", UserName = "selma", Email = "selmal@example.com", Index = 12345, Department = "RI", Year = 3 };
            var teacher = new Teacher { Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            var course = new Course { Name = "Vjerovatnoća i statistika", Teacher = teacher, TeacherID = teacher.Id, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            var studentCourse = new StudentCourse { ID = 99, CourseID = course.ID, StudentID = student.Id, Student = student, Course = course, Points = 75, Grade = 8 };
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(student);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());

            var mockController = new StudentCourseController(_context, _mockUserManager.Object);
            // Act
            var result = await mockController.StudentOverallStatus(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task StudentOverallStatus_NotNull()
        {
            // Arrange
            var student = new Student { FirstName = "Selma", LastName = "Selma", UserName = "selmatest", Email = "selmal@example.com", Index = 12345, Department = "RI", Year = 3 };
            _context.Student.Add(student);
            _context.SaveChanges();
            var foundStudent = await _context.Student.FirstOrDefaultAsync(m => m.UserName == "selmatest");

            var teacher = new Teacher { Title = "Mr.", FirstName = "test", LastName = "test", UserName = "testtest328", Email = "johndoe@example.com" };
            _context.Teacher.Add(teacher);
            _context.SaveChanges();
            var foundTeacher = await _context.Teacher.FirstOrDefaultAsync(m => m.UserName == "testtest328");

            var course = new Course { Name = "Ne znam ni ja vise", Teacher = foundTeacher, TeacherID = foundTeacher.Id, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            _context.Course.Add(course);
            _context.SaveChanges();
            var foundCourse = await _context.Course.FirstOrDefaultAsync(m => (m.Name == "Ne znam ni ja vise" && m.Teacher.UserName == "testtest328"));

            var studentCourse = new StudentCourse { CourseID = foundCourse.ID, StudentID = foundStudent.Id, Student = foundStudent, Course = foundCourse, Points = 75, Grade = 8 };
            _context.StudentCourse.Add(studentCourse);
            _context.SaveChanges();
            var foundStudentCourse = await _context.StudentCourse.FirstOrDefaultAsync(m => (m.CourseID == foundCourse.ID && m.StudentID == foundStudent.Id));

            var exam = new Exam { CourseID = foundCourse.ID, Time = DateTime.Now, Type = ExamType.Midterm, TotalPoints = 53, MinimumPoints = 15 };
            _context.Exam.Add(exam);
            _context.SaveChanges();
            var foundExam = await _context.Exam.FirstOrDefaultAsync(m => (m.CourseID == exam.CourseID && m.TotalPoints == 53));

            var studentExam = new StudentExam { CourseID = foundStudentCourse.ID, Course = foundStudentCourse, ExamID = foundExam.ID, PointsScored = 16, IsPassed = true };
            _context.StudentExam.Add(studentExam);
            _context.SaveChanges();
            var foundStudentExam = await _context.StudentExam.FirstOrDefaultAsync(m => (m.CourseID == studentExam.CourseID && m.ExamID == studentExam.ExamID));

            var homework = new Homework { CourseID = foundCourse.ID, Deadline = DateTime.Now, TotalPoints = 10, Description = "Boze pomagaj" };
            _context.Homework.Add(homework);
            _context.SaveChanges();
            var foundHomework = await _context.Homework.FirstOrDefaultAsync(m => (m.CourseID == homework.CourseID && m.Description == "Boze pomagaj"));

            var studentHomework = new StudentHomework { CourseID = foundStudentCourse.ID, HomeworkID = foundHomework.ID, PointsScored = 7, Comment = "sda" };
            _context.StudentHomework.Add(studentHomework);
            _context.SaveChanges();
            var foundStudentHomework = await _context.StudentHomework.FirstOrDefaultAsync(m => m.CourseID == studentHomework.CourseID);


            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(student);


            var mockController = new StudentCourseController(_context, _mockUserManager.Object);
            // Act
            var result = await mockController.StudentOverallStatus(foundStudentCourse.ID);


            // Assert
            _context.StudentExam.Remove(foundStudentExam);
            _context.Exam.Remove(foundExam);
            _context.StudentHomework.Remove(foundStudentHomework);
            _context.Homework.Remove(homework);
            _context.StudentCourse.Remove(foundStudentCourse);
            _context.Course.Remove(foundCourse);
            _context.Student.Remove(foundStudent);
            _context.Teacher.Remove(foundTeacher);


            _context.SaveChanges();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}
