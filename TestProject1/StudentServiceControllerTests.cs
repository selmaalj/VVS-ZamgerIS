using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ooadproject.Controllers;
using ooadproject.Data;
using ooadproject.Models;

namespace ProjectTests
{
    [TestClass]
    public class StudentServiceControllerTests
    {
        private ApplicationDbContext _context;
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
        }

        public DbSet<StudentService> SetupMockDB()
        {
            var studentServices = new List<StudentService>
            {
                new StudentService { Id = 1,  FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" },
                new StudentService { Id = 2, FirstName = "Jane", LastName = "Smith", UserName = "janesmith", Email = "janesmith@example.com" },
                new StudentService { Id = 3, FirstName = "David", LastName = "Johnson", UserName = "davidjohnson", Email = "davidjohnson@example.com" },
                new StudentService { Id = 4, FirstName = "Emily", LastName = "Brown", UserName = "emilybrown", Email = "emilybrown@example.com" },
                new StudentService { Id = 5, FirstName = "Michael", LastName = "Davis", UserName = "michaeldavis", Email = "michaeldavis@example.com" }
            };
            var mockSet = new Mock<DbSet<StudentService>>();
            mockSet.As<IQueryable<StudentService>>().Setup(m => m.Provider).Returns(studentServices.AsQueryable().Provider);
            mockSet.As<IQueryable<StudentService>>().Setup(m => m.Expression).Returns(studentServices.AsQueryable().Expression);
            mockSet.As<IQueryable<StudentService>>().Setup(m => m.ElementType).Returns(studentServices.AsQueryable().ElementType);
            mockSet.As<IQueryable<StudentService>>().Setup(m => m.GetEnumerator()).Returns(studentServices.AsQueryable().GetEnumerator());

            return mockSet.Object;
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenStudentServiceIsNull()
        {
            // Arrange
            int? id = 1;
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenStudentServiceExists()
        {
            // Arrange
            int? id = 2;
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenStudentServiceDoesNotExists()
        {
            // Arrange
            int? id = 1;
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_ReturnsViewResult()
        {
            // Arrange
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_InvalidModel_ReturnsView()
        {
            // Arrange
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var studentService = new StudentService()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "johndoe@example.com"
            };
            controller.ModelState.AddModelError("FirstName", "The FirstName field is required.");

            // Act
            var result = await controller.Create(studentService) as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(studentService, result.Model);
        }

        [TestMethod]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var studentService = new StudentService()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "johndoe@example.com"
            };

            // Act
            var result = await controller.Create(studentService);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenStudentServiceIsNull()
        {
            // Arrange
            int? id = 1;
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResult_WhenStudentServiceIsNotNull()
        {
            // Arrange
            int? id = 2;
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResult_WhenStudentServiceIsNull()
        {
            // Arrange
            int? id = 1;
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ModelStateInvalid_ReturnsViewResult()
        {
            // Arrange
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);
            var studentService = new StudentService { Id = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            controller.ModelState.AddModelError("FirstName", "The FirstName field is required.");

            // Act
            var result = await controller.Edit(studentService.Id, studentService) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(studentService, result.Model);
        }

        [TestMethod]
        public async Task Edit_DbUpdateConcurrencyException_ReturnsNotFoundResult()
        {
            // Arrange

            var studentService = new StudentService { Id = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            _mockContext.Setup(c => c.Update(studentService)).Throws(new DbUpdateConcurrencyException());
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Edit(studentService.Id, studentService) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Edit_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var studentService = new StudentService { Id = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };

            // Act
            var result = await controller.Edit(2, studentService) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Edit_CallsUpdateAndSaveChangesAsync_WhenModelStateIsValid()
        {
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            _mockContext.Setup(c => c.Set<StudentService>()).Returns(SetupMockDB());

            // Act
            var result = await controller.Edit(1, new StudentService { Id = 1 });

            // Assert
            _mockContext.Verify(c => c.Update(It.IsAny<StudentService>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Edit_ThrowsDbUpdateConcurrencyException_WhenStudentServiceDoesExistsButConcurrencyExceptionOccurs()
        {
            //Arrange
            var studentService = new StudentService { Id = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            _mockContext.Setup(c => c.Update(It.IsAny<StudentService>())).Throws(new DbUpdateConcurrencyException());
            _mockContext.Setup(c => c.StudentService).Returns(SetupMockDB());
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            //Act

            //Assert
            await Assert.ThrowsExceptionAsync<DbUpdateConcurrencyException>(async () => await controller.Edit(1, studentService));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenStudentServiceIsNull()
        {
            // Arrange
            int? id = 1;
            var controller = new StudentServiceController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsViewResult_WhenStudentServiceExists()
        {
            // Arrange
            int? id = 2;
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenStudentServiceExists()
        {
            // Arrange
            int? id = 1;
            var controller = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenStudentServiceExists()
        {
            // Arrange
            var studentService = new StudentService()
            {
               FirstName = "Test", LastName = "Test", Email = "Test@test.com", UserName = "Test" 
            };
            _context.StudentService.Add(studentService);
            _context.SaveChanges();
            var studentServiceControler = new StudentServiceController(_context, _mockUserManager.Object, _mockPasswordHasher.Object);

            // Act
            var foundId = _context.StudentService.FirstOrDefault(m => m.FirstName == studentService.FirstName);
            var result = await studentServiceControler.DeleteConfirmed(foundId.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsProblemResult_WhenStudentServiceIsNull()
        {
            // Arrange
            _context.StudentService = null;
            var _studentServiceController = new StudentServiceController(_context, null, null);

            // Act
            var result = await _studentServiceController.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

    }
}
