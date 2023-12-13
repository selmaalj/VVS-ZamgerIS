using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NuGet.DependencyResolver;
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
    public class ExamControllerTests
    {
        private ApplicationDbContext _context;
        private ExamController _examController;
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

            var mockUserStore = new Mock<IUserStore<Person>>();
            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<UserManager<Person>>(new Mock<IUserStore<Person>>().Object, null, null, null, null, null, null, null, null);
            _mockPasswordHasher = new Mock<IPasswordHasher<Person>>();

            _examController = new ExamController(_context, null);
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_WhenTeacherIsNotNull()
        {
            // Arrange
            var teacher = await _context.Teacher.FirstOrDefaultAsync(m => m.Id == 21);
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(teacher);
            var controller = new ExamController(_context, _mockUserManager.Object);

            // Act
            var result = (await controller.Index()) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _examController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        /*
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenExamisNull()
        {
            // Arrange
            var teacher = await _context.Teacher.FirstOrDefaultAsync(m => m.Id == 21);
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(teacher);
            var mockExams = new List<Exam>(); // Empty collection or null when the exam doesn't exist
            var mockSet = new Mock<DbSet<Exam>>();
            mockSet.As<IQueryable<Exam>>().Setup(m => m.Provider).Returns(mockExams.AsQueryable().Provider);
            mockSet.As<IQueryable<Exam>>().Setup(m => m.Expression).Returns(mockExams.AsQueryable().Expression);
            mockSet.As<IQueryable<Exam>>().Setup(m => m.ElementType).Returns(mockExams.AsQueryable().ElementType);
            mockSet.As<IQueryable<Exam>>().Setup(m => m.GetEnumerator()).Returns(mockExams.AsQueryable().GetEnumerator());

            _mockContext.Setup(m => m.Exam).Returns(mockSet.Object);
            var controller = new ExamController(_mockContext.Object, _mockUserManager.Object);

            int? id = 3;

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

        }
        */
        [TestMethod]
        public async Task Create_InvalidModel_ReturnsView()
        {
            //Arrange
            var controller = new ExamRegistrationController(_mockContext.Object, _mockUserManager.Object);
            controller.ModelState.AddModelError("ExamID", "Incorrect input of ExamID");

            //Act
            var result = await controller.Create(2, new ExamRegistration { }) as ViewResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        /*
        [TestMethod]
        public async Task Delete_ReturnsProblemResult_WhenExamRegistrationIsNull()
        {
            //Arrange
            var controller = new ExamRegistrationController(_mockContext.Object, null);

            // Act
            var result = await controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        
        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenCourseIsNotNull()
        {
            // Arrange
            var teacher = await _context.Teacher.FirstOrDefaultAsync(m => m.Id == 21);
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(teacher);
            var controller = new ExamController(_context, _mockUserManager.Object);

            int? id =11;
            // Act
            var result = await _examController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        */

        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenExamIsNotNull()
        {
            int? id = 1;
            // Arrange
            var teacher = new Teacher { Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(teacher);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());

            var mockController = new ExamController(_context, _mockUserManager.Object);
            // Act
            var result = await mockController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _examController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenExamIsNull()
        {
            // Arrange
            int? id = 101;

            // Act
            var result = await _examController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_ReturnsViewResult()
        {
            // Arrange
            var teacher = await _context.Teacher.FirstOrDefaultAsync(m => m.Id == 21);
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(teacher);
            var controller = new ExamController(_context, _mockUserManager.Object);
            // Act
            var result = await controller.Create();


            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var exam = new Exam { CourseID = 1, Type = ExamType.Midterm, MinimumPoints = 50, TotalPoints = 100, Time = DateTime.Now };

            var controller = new ExamController(_mockContext.Object, _mockUserManager.Object);

            // Act

            var result = await controller.Create(exam) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }


        [TestMethod]
        public void GetExamTypesList_ReturnsListOfSelectListItems()
        {

            // Act
            var result = _examController.GetExamTypesList(); // Call the method to test

            // Assert
            Assert.IsNotNull(result); // Check if the result is not null
            Assert.IsInstanceOfType(result, typeof(List<SelectListItem>));
        }
        [TestMethod]
        public void GetExamTypesList_ReturnsCorrectNumberOfItems()
        {
            // Arrange

            var enumValuesCount = Enum.GetValues(typeof(ExamType)).Length; // Get count of ExamType enum values

            // Act
            var result = _examController.GetExamTypesList(); // Call the method to test

            // Assert
            Assert.AreEqual(enumValuesCount, result.Count); // Check if the count matches the number of enum values
        }

        [TestMethod]
        public async Task GetCourseNamesList_ShouldReturnListOfCourseNames()
        {
            var teacher = await _context.Teacher.FirstOrDefaultAsync(m => m.Id == 21);
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(teacher);

            var controller = new ExamController(_context, _mockUserManager.Object);

            // Act
            var result = await controller.GetCourseNamesList();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<SelectListItem>));

            // Check the count and content of the returned list of SelectListItem
            Assert.AreEqual(3, result.Count); // Assuming only one course for the specific user in the test
        }

        [TestMethod]
        public async Task GetExamRegistrations_ReturnsCorrectCount()
        {

            var student = await _context.Student.FindAsync(11);
            _mockUserManager.Setup(m => m.GetUserAsync(default)).ReturnsAsync(student);
            var controller = new ExamController(_context, _mockUserManager.Object);



            // Act
            var result = controller.GetExamRegistrations(5);

            // Assert
            Assert.AreEqual(0, result); // u bazi ne postoji nijedan examregistration
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _examController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenExamIsNull()
        {
            // Arrange
            int? id = 101;

            // Act
            var result = await _examController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_DifferentIds()
        {
            // Arrange
            var id = 10;

            // Act
            var result = await _examController.Edit(id, new Exam
            {
                ID = 100,
                CourseID = 1,
                Time = DateTime.Now.AddDays(5),
                Type = ExamType.Midterm,
                TotalPoints = 100,
                MinimumPoints = 50
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_SameIds()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _examController.Edit(id, new Exam
            {
                ID = 1,
                CourseID = 1,
                Time = DateTime.Now.AddDays(5),
                Type = ExamType.Midterm,
                TotalPoints = 100,
                MinimumPoints = 50
            });

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Edit_SameIdsThatDontExist()
        {
            // Arrange
            var id = 99;

            // Act
            var result = await _examController.Edit(id, new Exam
            {
                ID = 99,
                CourseID = 1,
                Time = DateTime.Now.AddDays(5),
                Type = ExamType.Midterm,
                TotalPoints = 100,
                MinimumPoints = 50
            });
            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        /*
        [TestMethod]
        public void GetExamRegistrations_WhenNoRegistrations_ReturnsZero()
        {
            // Arrange
            var examRegistrations = new List<ExamRegistration>(); // No registrations


            // Act
            var result = _examController.GetExamRegistrations(1);

            // Assert
            Assert.AreEqual(0, result); // Expecting zero registrations for ExamID 1
        }
        [TestMethod]
        public async Task Index_ReturnsViewWithCorrectData()
        {
            var teacher = new Teacher { Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };

            var courses = new List<Course>
    {
         new Course { Name = "Vjerovatnoća i statistika", Teacher = teacher, TeacherID = teacher.Id, AcademicYear = "2022", ECTS = 6, Semester = 2 },
              ,
        // Add more courses as needed for the test
    };

     
    List<Exam> exams = new List<Exam>
{
    new Exam
    {
        ID = 1,
        CourseID = 1, // Associate with Course 1
        Course = courses.FirstOrDefault(c => c.ID == 1),
        Time = DateTime.Now.AddDays(5), // Set exam time to 5 days from now
        Type = ExamType.Midterm, // Set the exam type (assuming you have defined ExamType enum)
        TotalPoints = 100, // Set total points for the exam
        MinimumPoints = 50 // Set minimum required points for the exam
    },
    new Exam
    {
        ID = 2,
        CourseID = 2, // Associate with Course 2
        Course = courses.FirstOrDefault(c => c.ID == 2),
        Time = DateTime.Now.AddDays(10), // Set exam time to 10 days from now
        Type = ExamType.Midterm, // Set the exam type
        TotalPoints = 150, // Set total points for the exam
        MinimumPoints = 75 // Set minimum required points for the exam
    }
    };

            var mockRegistered = new Dictionary<int, int>
    {
        { 1, 2 }, // Assuming exam with ID 1 has 2 registrations
        // Add more registered exams as needed for the test
    };

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Course).ReturnsDbSet(courses);
            mockContext.Setup(c => c.Exam).ReturnsDbSet(exams);

            var mockController = new ExamController(mockContext.Object, mockUserManager.Object);
            mockController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = mockUser }
            };

            var expectedViewData = new ViewDataDictionary();
            expectedViewData["Courses"] = mockCourseList;
            expectedViewData["RegisteredForExam"] = mockRegistered;

            // Act
            var result = await mockController.Index();

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(result);
            Assert.AreEqual("Index", viewResult.ViewName); // Assuming your view is named "Index"
            var model = Assert.IsAssignableFrom<List<Exam>>(viewResult.Model);
            Assert.AreEqual(mockExamList.Count, model.Count); // Check if the correct number of exams is returned

            var actualViewData = viewResult.ViewData;
            Assert.IsNotNull(actualViewData);
            Assert.AreEqual(expectedViewData.Count, actualViewData.Count);
            Assert.AreEqual(expectedViewData["Courses"], actualViewData["Courses"]);
            Assert.AreEqual(expectedViewData["RegisteredForExam"], actualViewData["RegisteredForExam"]);

            // Verify that GetUserAsync method was called once
            mockUserManager.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }
        */
        [TestMethod]
        public async Task Edit_ModelStateInvalid_ReturnsViewResult()
        {
            //Arrange
            var exam = new Exam {
                ID = 99,
                CourseID = 1,
                Time = DateTime.Now.AddDays(5),
                Type = ExamType.Midterm,
                TotalPoints = 100,
                MinimumPoints = 50
            };
            var controller = new ExamController(_context, _mockUserManager.Object);
            controller.ModelState.AddModelError("error", "Neki error");

            //Act
            var result = await controller.Edit(exam.ID, exam) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(exam, result.Model);
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResult_WhenStudentServiceIsNotNull()
        {
            //Arrange
            int? id = 2;
            var user = await _context.Teacher.FindAsync(18);
            _mockUserManager.Setup(m => m.GetUserAsync(default)).ReturnsAsync(user);
            var controller = new ExamController(_context, _mockUserManager.Object);

            //Act
            var result = await controller.Edit(id);

            //Arrange
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}