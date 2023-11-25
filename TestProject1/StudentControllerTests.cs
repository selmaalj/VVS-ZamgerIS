using ooadproject.Controllers;
using ooadproject.Data;
using ooadproject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace ProjectTests
{
    [TestClass]
    public class StudentControllerTests
    {
        private ApplicationDbContext _context;
        private StudentController _studentController;
        private Mock<Microsoft.AspNetCore.Identity.UserManager<Person>> _mockUserManager;
        private Mock<Microsoft.AspNetCore.Identity.IPasswordHasher<Person>> _mockPasswordHasher;
        private Mock<ApplicationDbContext> _mockDbContext;

        [TestInitialize]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
         .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
         .Options;
            _mockDbContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<Microsoft.AspNetCore.Identity.UserManager<Person>>(new Mock<Microsoft.AspNetCore.Identity.IUserStore<Person>>().Object, null, null, null, null, null, null, null, null);
            _mockPasswordHasher = new Mock<IPasswordHasher<Person>>();

            _context = new ApplicationDbContext(options);
            _studentController = new StudentController(_context, null!, null!);
        }


        [TestMethod]
        public async Task Index_ReturnsViewResult()
        {
            var result = await _studentController.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Index_ReturnsProblem_WhenStudentIsNull()
        {
            _context.Student = null;
            var result = await _studentController.Index();
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void BubbleSort_ReturnsSortedList()
        {
            var students = new List<Student>
            {
                new Student { Index = 2, FirstName = "John" },
                new Student { Index = 3, FirstName = "Zelda"},
                new Student { Index = 1, FirstName = "Ana" },
            };
            students = _studentController.BubbleSort(students);
            Assert.AreEqual(3, students.Count);
            Assert.AreEqual(1, students[0].Index);
            Assert.AreEqual(2, students[1].Index);
            Assert.AreEqual(3, students[2].Index);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            int? id = null;
            var result = await _studentController.Details(id);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenStudentIsNull()
        {
            Student s = new();
            var result = await _studentController.Details(s.Id);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WhenStudentIsNotNull()
        {
            int? id = 11;
            var result = await _studentController.Details(id);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Create_ReturnsViewResult()
        {
            var result = _studentController.Create();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult()
        {
            var controller = new StudentController(_mockDbContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var student = new Student { Index = 1, Department = "RI", Year = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            var result = await controller.Create(student) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
        [TestMethod]
        public async Task Create_InvalidModel_ReturnsViewResultWithModel()
        {
            var controller = new StudentController(_mockDbContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var student = new Student { Index = 1, Department = "RI", Year = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            controller.ModelState.AddModelError("error", "some error");
            var result = await controller.Create(student) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(student, result.Model);
        }


        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            int? id = null;
            var result = await _studentController.Edit(id);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenStudentIsNull()
        {
            Student s = new();
            var result = await _studentController.Edit(s.Id);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResult_WhenStudentIsNotNull()
        {
            var id = 11;
            var result = await _studentController.Edit(id);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsCorrectStudent_WhenStudentIsNotNull()
        {
            var id = 11;  
            var result = await _studentController.Edit(id) as ViewResult;
            var resultStudent = result!.Model as Student;
            Assert.AreEqual(id, resultStudent!.Id);
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdDoesNotMatchStudentId()
        {
            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == 14);  //

            var newStudent = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == 15); ; //

            var result = await _studentController.Edit(student!.Id, newStudent!);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_CallsUpdateAndSaveChangesAsync_WhenModelStateIsValid()
        {
            var controller = new StudentController(_mockDbContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var students = new List<Student>
            {
                new Student { Id = 1 }
            };
            var mockSet = new Mock<DbSet<Teacher>>();
            mockSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.AsQueryable().Provider);
            mockSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.AsQueryable().Expression);
            mockSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.AsQueryable().ElementType);
            mockSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.AsQueryable().GetEnumerator());
            _mockDbContext.Setup(c => c.Set<Teacher>()).Returns(mockSet.Object);
            var result = await controller.Edit(1, new Student { Id = 1 });
            _mockDbContext.Verify(c => c.Update(It.IsAny<Student>()), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenStudentExistsButConcurrencyExceptionOccurs()
        {
            var student = new Student { Id = 1, Index = 1, Department = "RI", Year = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            _mockDbContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());
            var students = new List<Student>
            {
                new Student { Id=1, Index = 1, Department = "RI", Year = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" },
                new Student { Id=2, Index = 1, Department = "RI", Year = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" },
                new Student { Id=3, Index = 1, Department = "RI", Year = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" },
                new Student { Id=4, Index = 1, Department = "RI", Year = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" },
                new Student { Id=5, Index = 1, Department = "RI", Year = 1, FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" }
            };
            var mockSet = new Mock<DbSet<Student>>();
            mockSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.AsQueryable().Provider);
            mockSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.AsQueryable().Expression);
            mockSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.AsQueryable().ElementType);
            mockSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.AsQueryable().GetEnumerator());
            _mockDbContext.Setup(c => c.Set<Student>()).Returns(mockSet.Object);
            _mockDbContext.Object.Student.Add(student);
            var controller = new StudentController(_mockDbContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var result = await controller.Edit(1, student);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _studentController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenStudentIsNull()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _studentController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsViewResult_WhenStudentIsNotNull()
        {
            // Arrange
            var student = new Student { Index = 1, Department = "RI", Year = 1, FirstName = "Michael", LastName = "Davis", UserName = "michaeldavis", Email = "michaeldavis@example.com" };
            _context.Student.Add(student);
            _context.SaveChanges();

            // Act
            var foundStudentId = (_context.Student.FirstOrDefault(m => m.FirstName == student.FirstName))!.Id;
            var result = await _studentController.Delete(foundStudentId);
            _context.Student.Remove(student);
            _context.SaveChanges();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenStudentExists()
        {
            // Arrange
            var student = new Student { Id = 999, Index = 1, Department = "RI", Year = 1, FirstName = "Michael", LastName = "Davis", UserName = "michaeldavis", Email = "michaeldavis@example.com" };
            _context.Student.Add(student);
            var id = 999;

            // Act
            var result = await _studentController.DeleteConfirmed(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsNotFoundResult_WhenStudentDoesNotExist()
        {
            // Arrange
            var student = new Student { Index = 1, Department = "RI", Year = 1, FirstName = "Michael", LastName = "Davis", UserName = "michaeldavis", Email = "michaeldavis@example.com" };
            _context.Student.Add(student);
            _context.SaveChanges();

            // Act
            var foundStudent = _context.Student.FirstOrDefault(m => m.FirstName == student.FirstName);
            var result = await _studentController.DeleteConfirmed(2);
            _context.Student.Remove(foundStudent!);
            _context.SaveChanges();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsProblemResult_WhenStudentIsNull()
        {
            // Arrange
            _context.Student = null!;

            // Act
            var result = await _studentController.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }
    }
}
