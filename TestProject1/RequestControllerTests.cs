using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
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
            var result = await _requestController.Details(9);
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
        public async Task Edit_ReturnsNotFound_WhenItDoesntExit()
        {
            var result = await _requestController.Edit(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewResultl()
        {
            var result = await _requestController.Edit(2);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}   
