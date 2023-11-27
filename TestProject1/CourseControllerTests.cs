using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NuGet.DependencyResolver;
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
    public class CourseControllerTests
    {
        private ApplicationDbContext _context;
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
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_WhenTeacherIsNotNull()
        {
            // Arrange
            // Act
            var result = await _courseController.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _courseController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenCourseIsNull()
        {
            // Arrange
            int? id = 200;

            // Act
            var result = await _courseController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenCourseIsNotNull()
        {
            // Arrange
            int? id = 1;
            // Act
            var result = await _courseController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [TestMethod]
        public async Task Create_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _courseController.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var teacher = new Teacher { Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            var course = new Course { Name = "Vjerovatnoća i statistika", Teacher = teacher, TeacherID = teacher.Id, AcademicYear = "2022", ECTS = 6, Semester = 2 };

            var teacherController = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var courseController = new CourseController(_mockContext.Object, _mockUserManager.Object);

            // Act
            var resultTeacher = await teacherController.Create(teacher) as RedirectToActionResult;
            var result = await courseController.Create(course) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }


        [TestMethod]
        public async Task Create_InvalidModel_ReturnsViewResultWithModel()
        {
            // Arrange
            var teachers = new List<Teacher>
            {
                new Teacher { Id = 1, Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" },
                new Teacher { Id = 2, Title = "Ms.", FirstName = "Jane", LastName = "Smith", UserName = "janesmith", Email = "janesmith@example.com" },
                new Teacher { Id = 3, Title = "Dr.", FirstName = "David", LastName = "Johnson", UserName = "davidjohnson", Email = "davidjohnson@example.com" },
                new Teacher { Id = 4, Title = "Mrs.", FirstName = "Emily", LastName = "Brown", UserName = "emilybrown", Email = "emilybrown@example.com" },
                new Teacher { Id = 5, Title = "Mr.", FirstName = "Michael", LastName = "Davis", UserName = "michaeldavis", Email = "michaeldavis@example.com" }
            };
            var mockSet = new Mock<DbSet<Teacher>>();
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.Provider).Returns(teachers.AsQueryable().Provider);
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.Expression).Returns(teachers.AsQueryable().Expression);
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.ElementType).Returns(teachers.AsQueryable().ElementType);
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.GetEnumerator()).Returns(teachers.AsQueryable().GetEnumerator());
            _mockContext.Setup(c => c.Teacher).Returns(mockSet.Object);
            var controller = new CourseController(_mockContext.Object, _mockUserManager.Object);
            var course = new Course { Name = "Vjerovatnoća i statistika", TeacherID = 21, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            controller.ModelState.AddModelError("error", "some error");

            //Act
            var result = await controller.Create(course) as ViewResult;


            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(course, result.Model);
        }


        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _courseController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenCourseIsNull()
        {
            // Arrange
            int? id = 200;

            // Act
            var result = await _courseController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResult_WhenCourseIsNotNull()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _courseController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_DifferentIds()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _courseController.Edit(id, new Course
            { ID = 10, Name = "Test", TeacherID = 10, AcademicYear = "2022", ECTS = 6, Semester = 1 });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }



        [TestMethod]
        public async Task Edit_SameIds()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _courseController.Edit(id, new Course
            { ID = 1, Name = "Test", TeacherID = 10, AcademicYear = "2022", ECTS = 6, Semester = 1 });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }
        [TestMethod]
        public async Task Edit_SameIdsThatDontExist()
        {
            // Arrange
            var id = 99;

            // Act
            var result = await _courseController.Edit(id, new Course
            { ID = 99, Name = "Test", TeacherID = 10, AcademicYear = "2022", ECTS = 6, Semester = 1 });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public async Task Edit_SameIdsExeption()
        {
            // Arrange
            var course = new Course{ ID = 1, Name = "Test1", TeacherID = 21, AcademicYear = "2022", ECTS = 6, Semester = 1 };
            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());
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
            var controller = new CourseController(_mockContext.Object, _mockUserManager.Object);

            //Act
            var result = await controller.Edit(1, course);

            //Assert
         

        }

        [TestMethod]
        public async Task Edit_InvalidModel()
        {
        
            // Arrange
            var id = 1;
            var course = new Course
            { ID = 1, Name = "Test", TeacherID = 10, AcademicYear = "2022", ECTS = 6, Semester = 1 };
            var teachers = new List<Teacher>
            {
                new Teacher { Id = 1, Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" },
                new Teacher { Id = 2, Title = "Ms.", FirstName = "Jane", LastName = "Smith", UserName = "janesmith", Email = "janesmith@example.com" },
                new Teacher { Id = 3, Title = "Dr.", FirstName = "David", LastName = "Johnson", UserName = "davidjohnson", Email = "davidjohnson@example.com" },
                new Teacher { Id = 4, Title = "Mrs.", FirstName = "Emily", LastName = "Brown", UserName = "emilybrown", Email = "emilybrown@example.com" },
                new Teacher { Id = 5, Title = "Mr.", FirstName = "Michael", LastName = "Davis", UserName = "michaeldavis", Email = "michaeldavis@example.com" }
            };
            var mockSet = new Mock<DbSet<Teacher>>();
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.Provider).Returns(teachers.AsQueryable().Provider);
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.Expression).Returns(teachers.AsQueryable().Expression);
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.ElementType).Returns(teachers.AsQueryable().ElementType);
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.GetEnumerator()).Returns(teachers.AsQueryable().GetEnumerator());
            _mockContext.Setup(c => c.Teacher).Returns(mockSet.Object);
            var controller = new CourseController(_mockContext.Object, _mockUserManager.Object);
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.Edit(id, course) as ViewResult; ;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(course, result.Model);
        }

        [TestMethod]
        public async Task Delete_IsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _courseController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_DoesntExist()
        {
            // Arrange
            int? id = 99;

            // Act
            var result = await _courseController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_Exists()
        {
            // Arrange
            var course = new Course { Name = "Vjerovatnoća", TeacherID = 10, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            var courseResult = await _courseController.Create(course);

            // Act
            var result = await _courseController.Delete(course.ID) as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenTeacherExists()
        {
            // Arrange
            var course = new Course { ID = 999, Name = "Vjerovatnoća", TeacherID = 10, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            _context.Course.Add(course);
            var id = 999;

            // Act
            var result = await _courseController.DeleteConfirmed(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsProblemResult_WhenTeacherIsNull()
        {
            // Arrange
            _context.Course = null;

            // Act
            var result = await _courseController.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task CourseStatus()
        {
            // Arrange
            var course = new Course { Name = "Vjerovatnoća i statistika", TeacherID = 21, AcademicYear = "2022", ECTS = 6, Semester = 2 };
            var teacher = new Teacher { Id = 1, Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(teacher);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());

            var mockController = new CourseController(_context, _mockUserManager.Object);
            // Act
            var result = await mockController.CourseStatus(5);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

    }
}
