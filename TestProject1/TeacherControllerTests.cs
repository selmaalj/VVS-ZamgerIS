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

        [TestInitialize]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .Options;

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
    }
}