using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ooadproject.Controllers;
using ooadproject.Data;
using ooadproject.Models;



namespace ProjectTests
{
    [TestClass]
    public class TeacherControllerTests
    {
        private ApplicationDbContext _context;
        private TeacherController _teacherController;
        private Mock<ApplicationDbContext> _mockContext;
        private Mock<Microsoft.AspNetCore.Identity.UserManager<Person>> _mockUserManager;
        private Mock<Microsoft.AspNetCore.Identity.IPasswordHasher<Person>> _mockPasswordHasher;


        [TestInitialize]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options;

            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<Microsoft.AspNetCore.Identity.UserManager<Person>>(new Mock<Microsoft.AspNetCore.Identity.IUserStore<Person>>().Object, null, null, null, null, null, null, null, null);
            _mockPasswordHasher = new Mock<IPasswordHasher<Person>>();

            _context = new ApplicationDbContext(options);
            _teacherController = new TeacherController(_context, null, null);
        }


        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _teacherController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenTeacherIsNull()
        {
            // Arrange
            int? id = 1;

            // Act
            var result = await _teacherController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenTeacherIsNotNull()
        {
            // Arrange
            int? id = 21;
            // Act
            var result = await _teacherController.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _teacherController.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var controller = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var teacher = new Teacher { Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };

            // Act
            var result = await controller.Create(teacher) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
        [TestMethod]
        public async Task Create_InvalidModel_ReturnsViewResultWithModel()
        {
            // Arrange
            var controller = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var teacher = new Teacher { Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.Create(teacher) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(teacher, result.Model);
        }

    }
}