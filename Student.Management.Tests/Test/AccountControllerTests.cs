using Management.Project.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Student.Management.Controllers;
using Student.Management.DataAccess.DataAccess;
using Student.Management.Domain.Entities;
using Student.Management.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.AspNetCore.Identity.SignInResult;


namespace Student.Management.Tests
{
    public class AccountControllerTests
    {
        private readonly AccountController _controller;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManager;

        public AccountControllerTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var principalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManager = new Mock<SignInManager<ApplicationUser>>(_userManager.Object, contextAccessor.Object, principalFactory.Object, null, null, null, null);

            _controller = new AccountController(_userManager.Object, _signInManager.Object, null);
        }

        private static Dictionary<string, object> ToDict(JsonResult result) =>
            result.Value.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(result.Value));

        [Fact]
        public async Task Login_Success_Admin()
        {
            var model = new LoginModel { Email = "admin@btecfpt.edu.vn", Password = "Admin@123" };
            var user = new ApplicationUser { Email = model.Email };

            _userManager.Setup(u => u.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _signInManager.Setup(s => s.PasswordSignInAsync(model.Email, model.Password, true, false)).ReturnsAsync(Success);
            _userManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

            var dict = ToDict(await _controller.Login(model) as JsonResult);

            Assert.True((bool)dict["success"]);
            Assert.Equal("/Admin/Admin", dict["redirect"]);
        }

        [Fact]
        public async Task Login_Failed_WrongPassword()
        {
            var model = new LoginModel { Email = "admin@btecfpt.edu.vn", Password = "Wrong123" };
            var user = new ApplicationUser { Email = model.Email };

            _userManager.Setup(u => u.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _signInManager.Setup(s => s.PasswordSignInAsync(model.Email, model.Password, true, false)).ReturnsAsync(Failed);

            var dict = ToDict(await _controller.Login(model) as JsonResult);

            Assert.False((bool)dict["success"]);
            Assert.Equal("Invalid credentials", dict["message"]);
        }

        [Fact]
        public async Task Logout_RedirectsToLogin()
        {
            var result = await _controller.Logout() as RedirectToActionResult;
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }

        [Fact]
        public async Task Register_NewStudent_Success()
        {
            var model = new RegisterModel { Email = "student@test.com", Password = "Test@123", Role = "Student" };

            _userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), model.Password)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), model.Role)).ReturnsAsync(IdentityResult.Success);

            var dict = ToDict(await _controller.Register(model) as JsonResult);

            Assert.True((bool)dict["success"]);
            Assert.Contains("Registration successful", (string)dict["message"]);
        }

       
        
    }
}
