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
    public class HomeworkControllerTests
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

            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<UserManager<Person>>(new Mock<IUserStore<Person>>().Object, null, null, null, null, null, null, null, null);
            _mockPasswordHasher = new Mock<IPasswordHasher<Person>>();
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeworkController(_context);

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
            var controller = new HomeworkController(_mockContext.Object);

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenHomeworkIsNull()
        {
            // Arrange
            int? id = 1;
            var controller = new HomeworkController(_mockContext.Object);

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenHomeworkExists()
        {
            // Arrange
            int? id = 56;
            var controller = new HomeworkController(_context);

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenHomeworkObjectIsNull()
        {
            // Arrange
            int? id = 1;
            var controller = new HomeworkController(_context);

            // Act
            var result = await controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_ReturnsViewResult()
        {
            // Arrange
            var _controller = new HomeworkController(_context);

            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult()
        {
            // Arrange
            var controller = new HomeworkController(_mockContext.Object);
            var homework = new Homework()
            {
                ID = 1,
                CourseID = 1,
                Deadline = DateTime.Now.AddDays(7),
                TotalPoints = 100,
                Description = "Sample homework"
            };

            // Act
            var result = await controller.Create(homework) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task Create_InvalidModel_ReturnsViewResult()
        {
            //Arrange
            var controller = new HomeworkController(_context);
            controller.ModelState.AddModelError("error", "some error");
            var homework = new Homework()
            {
                ID = 1,
                CourseID = 1,
                Deadline = DateTime.Now.AddDays(7),
                TotalPoints = 100,
                Description = "Sample homework"
            };

            // Act
            var result = await controller.Create(homework);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;
            var _controller = new HomeworkController(_mockContext.Object);

            // Act
            var result = await _controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenHomeworkIsNull()
        {
            // Arrange
            int? id = 1;
            var _controller = new HomeworkController(_context);

            // Act
            var result = await _controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResult_WhenHomeworkExists()
        {
            // Arrange
            int? id = 56;
            var _controller = new HomeworkController(_context);

            // Act
            var result = await _controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_InvalidId_ReturnsNotFoundResult()
        {
            //Arrange 
            var controller = new HomeworkController(_mockContext.Object);
            var homework = new Homework()
            {
                ID = 1,
                CourseID = 1,
                Deadline = DateTime.Now.AddDays(7),
                TotalPoints = 100,
                Description = "Sample homework"
            };

            //Act
            var result = await controller.Edit(2, homework) as NotFoundResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_CallsUpdateAndSaveChangesAsync_WhenModelStateIsValid()
        {
            //Arrange 
            var controller = new HomeworkController(_mockContext.Object);
            var homework = new Homework()
            {
                ID = 1,
                CourseID = 1,
                Deadline = DateTime.Now.AddDays(7),
                TotalPoints = 100,
                Description = "Sample homework"
            };

            //Act
            var result = await controller.Edit(homework.ID, homework);


            //Assert
            _mockContext.Verify(c => c.Update(It.IsAny<Homework>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Edit_ThrowsDbUpdateConcurrencyException_WhenHomeworkExistsButConcurrencyExceptionOccurs()
        {
            //Arrange
            var homework = new Homework()
            {
                ID = 2,
                CourseID = 1,
                Deadline = DateTime.Now.AddDays(7),
                TotalPoints = 100,
                Description = "Sample homework"
            };
            var homeworks = new List<Homework>
            {
                new Homework
                {
                    ID = 1,
                    CourseID = 1,
                    Deadline = DateTime.Now.AddDays(7),
                    TotalPoints = 100,
                    Description = "Sample homework 1"
                },
                new Homework
                {
                    ID = 2,
                    CourseID = 2,
                    Deadline = DateTime.Now.AddDays(14),
                    TotalPoints = 80,
                    Description = "Sample homework 2"
                }
            };
            var mockSet = new Mock<DbSet<Homework>>();
            mockSet.As<IQueryable<Homework>>().Setup(m => m.Provider).Returns(homeworks.AsQueryable().Provider);
            mockSet.As<IQueryable<Homework>>().Setup(m => m.Expression).Returns(homeworks.AsQueryable().Expression);
            mockSet.As<IQueryable<Homework>>().Setup(m => m.ElementType).Returns(homeworks.AsQueryable().ElementType);
            mockSet.As<IQueryable<Homework>>().Setup(m => m.GetEnumerator()).Returns(homeworks.AsQueryable().GetEnumerator());

            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());
            _mockContext.Setup(c => c.Homework).Returns(mockSet.Object);
            var controller = new HomeworkController(_mockContext.Object);

            //Act

            //Assert
            await Assert.ThrowsExceptionAsync<DbUpdateConcurrencyException>(async () => await controller.Edit(homework.ID, homework));
        }

        [TestMethod]
        public async Task Edit_DbUpdateConcurrencyException_ReturnsNotFoundResult()
        {
            //Arrange
            var homework = new Homework()
            {
                ID = 23,
                CourseID = 1,
                Deadline = DateTime.Now.AddDays(7),
                TotalPoints = 100,
                Description = "Sample homework"
            };
            var homeworks = new List<Homework>
            {
                new Homework
                {
                    ID = 1,
                    CourseID = 1,
                    Deadline = DateTime.Now.AddDays(7),
                    TotalPoints = 100,
                    Description = "Sample homework 1"
                },
                new Homework
                {
                    ID = 2,
                    CourseID = 2,
                    Deadline = DateTime.Now.AddDays(14),
                    TotalPoints = 80,
                    Description = "Sample homework 2"
                }
            };
            var mockSet = new Mock<DbSet<Homework>>();
            mockSet.As<IQueryable<Homework>>().Setup(m => m.Provider).Returns(homeworks.AsQueryable().Provider);
            mockSet.As<IQueryable<Homework>>().Setup(m => m.Expression).Returns(homeworks.AsQueryable().Expression);
            mockSet.As<IQueryable<Homework>>().Setup(m => m.ElementType).Returns(homeworks.AsQueryable().ElementType);
            mockSet.As<IQueryable<Homework>>().Setup(m => m.GetEnumerator()).Returns(homeworks.AsQueryable().GetEnumerator());

            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());
            _mockContext.Setup(c => c.Homework).Returns(mockSet.Object);
            var controller = new HomeworkController(_mockContext.Object);

            //Act
            var result = await controller.Edit(homework.ID, homework) as NotFoundResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ModelStateInvalid_ReturnsViewResult()
        {
            //Arrange
            var homework = new Homework()
            {
                ID = 23,
                CourseID = 1,
                Deadline = DateTime.Now.AddDays(7),
                TotalPoints = 100,
                Description = "Sample homework"
            };
            var controller = new HomeworkController(_mockContext.Object);
            controller.ModelState.AddModelError("error", "neki error");

            //Act
            var result = await controller.Edit(homework.ID, homework);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;
            var controller = new HomeworkController(_mockContext.Object);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenHomeworkIsNull()
        {
            // Arrange
            int? id = 1;
            var controller = new HomeworkController(_mockContext.Object);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsViewResult_WhenHomeworkExists()
        {
            //Arrange
            int? id = 56;
            var controller = new HomeworkController(_context);

            //Act
            var result = await controller.Delete(id);

            //Arrange
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenHomeworkDoesntExist()
        {
            //Arrange
            int? id = 23;
            var controller = new HomeworkController(_context);

            //Act
            var result = await controller.Delete(id);

            //Arrange
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsProblemResult_WhenHomeworkIsNull()
        {
            //Arrange
            int id = 1;
            var controller = new HomeworkController(_mockContext.Object);

            //Act
            var result = await controller.DeleteConfirmed(id);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenHomeworkExists()
        {
            //Arrange
            var homework = new Homework
            {
                CourseID = 1,
                Course = await _context.Course.FindAsync(10),
                Deadline = DateTime.Now.AddDays(7),
                TotalPoints = 100,
                Description = "Sample homework"
            };
            _context.Homework.Add(homework);
            await _context.SaveChangesAsync();
            var foundHomework = await _context.Homework.FirstOrDefaultAsync(m => m.Description.Equals(homework.Description));
            var controller = new HomeworkController(_context);

            //Act
            var result = await controller.DeleteConfirmed(foundHomework.ID);

            //Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }
    }
}
