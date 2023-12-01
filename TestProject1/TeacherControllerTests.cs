using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ooadproject.Controllers;
using ooadproject.Data;
using ooadproject.Models;
using System.Xml.Linq;

namespace ProjectTests
{
    [TestClass]
    public class TeacherControllerTests
    {
        private ApplicationDbContext _context;
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

            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockUserManager = new Mock<UserManager<Person>>(new Mock<IUserStore<Person>>().Object, null, null, null, null, null, null, null, null);
            _mockPasswordHasher = new Mock<IPasswordHasher<Person>>();

            _teacherController = new TeacherController(_context, null, null);
        }

        public static IEnumerable<object[]> ReadTeachersFromXml()
        {
            List<object[]> teachers = new List<object[]>();

            XDocument doc = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "teachers.xml"));

            foreach (XElement element in doc.Root.Elements("Teacher"))
            {
                object[] teacher = new object[]
                {
                    element.Element("Title").Value,
                    element.Element("FirstName").Value,
                    element.Element("LastName").Value,
                    element.Element("UserName").Value,
                    element.Element("Email").Value
                };

                teachers.Add(teacher);
            }

            return teachers;
        }

        
        static IEnumerable<object[]> TeachersXML
        {
            get
            {
                return ReadTeachersFromXml();
            }
        }

        public static IEnumerable<object[]> ReadTeachersFromJson()
        {
            try
            {
                // Read the JSON file content
                string jsonContent = File.ReadAllText("teachers.json");

                // Parse the JSON array into a list of Teacher objects
                List<object[]> teachers = JsonConvert.DeserializeObject<List<object[]>>(jsonContent);

                return teachers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        static IEnumerable<object[]> TeachersJSON
        {
            get
            {
                return ReadTeachersFromJson();
            }
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
        [DynamicData(nameof(TeachersXML))]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult(String Title, String FirstName, String LastName, String UserName, String Email)
        {
            // Arrange
            var controller = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var teacher = new Teacher { Title = Title, FirstName = FirstName, LastName = LastName, UserName = UserName, Email = Email };
            var name = teacher.GetFullName();

            // Act
            var result = await controller.Create(teacher) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Title + " " + FirstName + " " + LastName, name);
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

        [TestMethod]
        public async Task Index_ReturnsViewResult_WhenTeacherIsNotNull()
        {
            // Arrange
            // Act
            var result = await _teacherController.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Index_ReturnsProblem_WhenTeacherIsNull()
        {
            // Arrange
            _context.Teacher = null;
            // Act
            var result = await _teacherController.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _teacherController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenTeacherIsNull()
        {
            // Arrange
            int? id = 1;

            // Act
            var result = await _teacherController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResult_WhenTeacherIsNotNull()
        {
            // Arrange
            var id = 21;

            // Act
            var result = await _teacherController.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsCorrectTeacher_WhenTeacherIsNotNull()
        {
            // Arrange
            var id = 21;

            // Act
            var result = await _teacherController.Edit(id) as ViewResult;
            var resultTeacher = result.Model as Teacher;

            // Assert
            Assert.AreEqual(id, resultTeacher.Id);
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdDoesNotMatchTeacherId()
        {
            // Arrange
            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == 21);

            var newTeacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == 22); ;

            // Act
            var result = await _teacherController.Edit(teacher.Id, newTeacher);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_CallsUpdateAndSaveChangesAsync_WhenModelStateIsValid()
        {
            // Arrange
            var controller = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            var teachers = new List<Teacher>
            {
                new Teacher { Id = 1 }
            };
            var mockSet = new Mock<DbSet<Teacher>>();
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.Provider).Returns(teachers.AsQueryable().Provider);
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.Expression).Returns(teachers.AsQueryable().Expression);
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.ElementType).Returns(teachers.AsQueryable().ElementType);
            mockSet.As<IQueryable<Teacher>>().Setup(m => m.GetEnumerator()).Returns(teachers.AsQueryable().GetEnumerator());
            _mockContext.Setup(c => c.Set<Teacher>()).Returns(mockSet.Object);

            // Act
            var result = await controller.Edit(1, new Teacher { Id = 1 });

            // Assert
            _mockContext.Verify(c => c.Update(It.IsAny<Teacher>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenTeacherDoesNotExistsButConcurrencyExceptionOccurs()
        {
            // Arrange
            var teacher = new Teacher { Id = 1, Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            _mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());
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
            _mockContext.Setup(c => c.Set<Teacher>()).Returns(mockSet.Object);
            var controller = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);

            //Act
            var result = await controller.Edit(1, teacher);

            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ThrowsDbUpdateConcurrencyException_WhenTeacherDoesExistsButConcurrencyExceptionOccurs()
        {
            //Arrange
            var teacher = new Teacher { Id = 1, Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            _mockContext.Setup(c => c.Update(It.IsAny<Teacher>())).Throws(new DbUpdateConcurrencyException());
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
            var controller = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);

            //Act

            //Assert
            await Assert.ThrowsExceptionAsync<DbUpdateConcurrencyException>(async () => await controller.Edit(1, teacher));
        }

        [TestMethod]
        public async Task Edit_WhenModelStateIsNotValid_ReturnsView()
        {
            // Arrange
            var teacher = new Teacher { Id = 1, Title = "Mr.", FirstName = "John", LastName = "Doe", UserName = "johndoe", Email = "johndoe@example.com" };
            var _controller = new TeacherController(_mockContext.Object, _mockUserManager.Object, _mockPasswordHasher.Object);
            _controller.ModelState.AddModelError("Error", "ModelState is not valid");

            // Act
            var result = await _controller.Edit(1, teacher);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _teacherController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenTeacherIsNull()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _teacherController.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsViewResult_WhenTeacherIsNotNull()
        {
            // Arrange
            int? id = 21;

            var controller = new TeacherController(_context, _mockUserManager.Object, _mockPasswordHasher.Object );

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        [DynamicData(nameof(TeachersJSON))]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult_WhenTeacherExists(String Title, String FirstName, String LastName, String UserName, String Email)
        {
            // Arrange
            var teacher = new Teacher { Id = 999, Title = Title, FirstName = FirstName, LastName = LastName, UserName = UserName, Email = Email };
            _context.Teacher.Add(teacher);

            // Act
            var result = await _teacherController.DeleteConfirmed(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task DeleteConfirmed_ReturnsProblemResult_WhenTeacherIsNull()
        {
            // Arrange
            _context.Teacher = null;

            // Act
            var result = await _teacherController.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

       
    }
}
