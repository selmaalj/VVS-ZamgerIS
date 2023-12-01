using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
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
    public class RequestControllerTests
    {

        private ApplicationDbContext _context;
        private RequestController _requestController;
        private StudentController _studentController;
        private Mock<Microsoft.AspNetCore.Identity.UserManager<Person>> _mockUserManager;
        private Mock<ApplicationDbContext> _mockDbContext;
        private Mock<IPasswordHasher<Person>> _mockPasswordHasher;

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
            _requestController = new RequestController(_context, null!);
            _studentController = new StudentController(_context, null!, null!);
        }

        [TestMethod]
        public void GetRequestTypesList_ReturnsList()
        {
            List<SelectListItem> result = _requestController.GetRequestTypesList();
            Assert.IsNotNull(result);
            Assert.AreEqual(Enum.GetValues(typeof(RequestType)).Length, result.Count);
            Assert.AreEqual("StudyTestimony", result[0].Text);
            Assert.AreEqual("PassedExamsTestimony", result[1].Text);
        }

        [TestMethod]
        public void GetRequestStatusList_ReturnsList()
        {
            List<SelectListItem> result = _requestController.GetRequestStatusList();
            Assert.IsNotNull(result);
            Assert.AreEqual(Enum.GetValues(typeof(RequestStatus)).Length, result.Count);
            Assert.AreEqual("Pending", result[0].Text);
            Assert.AreEqual("Accepted", result[1].Text);
            Assert.AreEqual("Rejected", result[2].Text);
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult()
        {
            var result = await _requestController.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Create_ReturnsViewResult()
        {
            var result = _requestController.Create();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Create_ValidModel_ReturnsRedirectToActionResult()
        {
            Student? student = await _context.Student.FirstOrDefaultAsync(m => m.Id.Equals(15));
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(student);
            var controller = new RequestController(_context, _mockUserManager.Object);
            var request = new Request { Type = RequestType.StudyTestimony, Requester=student, RequesterID=student.Id};
            var result = await controller.Create(request) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("StudentRequests",result.ActionName);
            _context.Request.Remove(request);
            await _context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task Create_InvalidModel_ReturnsViewResultWithModel()
        {
            Student? student = await _context.Student.FirstOrDefaultAsync(m => m.Id.Equals(15));
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(student);
            var controller = new RequestController(_context, _mockUserManager.Object);
            var request = new Request { Type = RequestType.StudyTestimony, Requester = student, RequesterID = student.Id };
            controller.ModelState.AddModelError("error", "some error");
            var result = await controller.Create(request) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(request, result.Model);
        }

        [TestMethod]
        public async Task Delete_ReturnsProblem()
        {
            _context.Request = null;
            var result = await _requestController.Delete(9);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task Delete_RedirectsToAction()
        {
            var request = new Request { ID = 99, Type = RequestType.StudyTestimony};
            _context.Request.Add(request);
            var result = await _requestController.Delete(99);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("StudentRequests", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            int? id = null;
            var result = await _requestController.Details(id);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenRequestIsNull()
        {
            _context.Request = null;
            var result = await _requestController.Details(9);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenItDoesntExit()
        {
            var result = await _requestController.Details(99);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewResultl()
        {
            var result = await _requestController.Details(2);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [TestMethod]
        public async Task StudentRequests_ReturnsView()
        {
            Student? student = await _context.Student.FirstOrDefaultAsync(m => m.Id.Equals(13));
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(student);
            var controller = new RequestController(_context, _mockUserManager.Object);
            var result = await controller.StudentRequests();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            int? id = null;
            var result = await _requestController.Edit(id);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenRequestIsNull()
        {
            _context.Request = null;
            var result = await _requestController.Edit(9);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenDoesntExit()
        {
            var result = await _requestController.Edit(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResult()
        {
            var result = await _requestController.Edit(2);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenDifferentIds()
        {
            StudentService? studentService = await _context.StudentService.FirstOrDefaultAsync(m => m.Id.Equals(2));
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(studentService);
            var controller = new RequestController(_context, _mockUserManager.Object);
            var request = new Request { Type = RequestType.StudyTestimony, Processor = studentService, ProcessorID = studentService.Id };
            var result = await controller.Edit(1123, request);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_InvalidModel_ReturnsViewResultWithModel()
        {
            StudentService? studentService = await _context.StudentService.FirstOrDefaultAsync(m => m.Id.Equals(2));
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(studentService);
            var controller = new RequestController(_context, _mockUserManager.Object);
            var request = new Request { Type = RequestType.StudyTestimony, Processor = studentService, ProcessorID = studentService.Id};
            controller.ModelState.AddModelError("error", "some error");
            var result = await controller.Edit(request.ID, request) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(request, result.Model);
        }

        [TestMethod]
        public async Task Edit_ValidModel_DbUpdateConcurrencyException()
        {
            StudentService? studentService = await _context.StudentService.FirstOrDefaultAsync(m => m.Id.Equals(2));
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(studentService);
            var request = new Request { ID=1, Type = RequestType.StudyTestimony, Processor = studentService, ProcessorID = studentService.Id };
            _mockDbContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());
            var requests = new List<Request>
            {
                new Request { ID=1, Type = RequestType.StudyTestimony,  Processor=studentService, ProcessorID=studentService.Id},
                new Request { ID=2, Type = RequestType.StudyTestimony,  Processor=studentService, ProcessorID=studentService.Id },
                new Request { ID=3, Type = RequestType.StudyTestimony,  Processor=studentService, ProcessorID=studentService.Id },
                new Request { ID=4, Type = RequestType.StudyTestimony,  Processor=studentService, ProcessorID=studentService.Id}
            };
            var mockSet = new Mock<DbSet<Request>>();
            mockSet.As<IQueryable<Request>>().Setup(m => m.Provider).Returns(requests.AsQueryable().Provider);
            mockSet.As<IQueryable<Request>>().Setup(m => m.Expression).Returns(requests.AsQueryable().Expression);
            mockSet.As<IQueryable<Request>>().Setup(m => m.ElementType).Returns(requests.AsQueryable().ElementType);
            mockSet.As<IQueryable<Request>>().Setup(m => m.GetEnumerator()).Returns(requests.AsQueryable().GetEnumerator());
            _mockDbContext.Setup(c => c.Request).Returns(mockSet.Object);
            var controller = new RequestController(_mockDbContext.Object, _mockUserManager.Object);
            await Assert.ThrowsExceptionAsync<DbUpdateConcurrencyException>(async () => await controller.Edit(1, request));
        }

        [TestMethod]
        public async Task Edit_ValidModel_DbUpdateConcurrencyException_DoesntExist()
        {
            StudentService? studentService = await _context.StudentService.FirstOrDefaultAsync(m => m.Id.Equals(2));
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(studentService);
            var request = new Request { ID = 999, Type = RequestType.StudyTestimony, Processor = studentService, ProcessorID = studentService.Id };
            _mockDbContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateConcurrencyException());
            int id = 999;
            var requests = new List<Request>
            {
                new Request { ID=1, Type = RequestType.StudyTestimony,  Processor=studentService, ProcessorID=studentService.Id},
                new Request { ID=2, Type = RequestType.StudyTestimony,  Processor=studentService, ProcessorID=studentService.Id },
                new Request { ID=3, Type = RequestType.StudyTestimony,  Processor=studentService, ProcessorID=studentService.Id },
                new Request { ID=4, Type = RequestType.StudyTestimony,  Processor=studentService, ProcessorID=studentService.Id}
            };
            var mockSet = new Mock<DbSet<Request>>();
            mockSet.As<IQueryable<Request>>().Setup(m => m.Provider).Returns(requests.AsQueryable().Provider);
            mockSet.As<IQueryable<Request>>().Setup(m => m.Expression).Returns(requests.AsQueryable().Expression);
            mockSet.As<IQueryable<Request>>().Setup(m => m.ElementType).Returns(requests.AsQueryable().ElementType);
            mockSet.As<IQueryable<Request>>().Setup(m => m.GetEnumerator()).Returns(requests.AsQueryable().GetEnumerator());
            _mockDbContext.Setup(c => c.Request).Returns(mockSet.Object);
            var controller = new RequestController(_mockDbContext.Object, _mockUserManager.Object);
            var result = await controller.Edit(id, request);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ValidModel_UpdatesAndRedirectsToAction()
        {
            StudentService? studentService = await _context.StudentService.FirstOrDefaultAsync(m => m.Id.Equals(2)); 
            _mockUserManager.Setup(c => c.GetUserAsync(default)).ReturnsAsync(studentService);
            Student? student = await _context.Student.FirstOrDefaultAsync(m => m.Id.Equals(11));
            var controller = new RequestController(_context, _mockUserManager.Object);
            var request = new Request { ID=1, Type = RequestType.StudyTestimony, Processor = studentService, ProcessorID = studentService.Id, Requester = student, RequesterID = student.Id, Status=RequestStatus.Accepted};
            var result = await controller.Edit(1, request);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }
    }
}   
