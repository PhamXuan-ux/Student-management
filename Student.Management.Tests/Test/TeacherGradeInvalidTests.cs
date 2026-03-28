using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Student.Management.Areas.Teacher.Controllers;
using Student.Management.DataAccess.DataAccess;
using Student.Management.Domain.Entities;
using System.Security.Claims;
using System.Text.Json;
using Xunit;

public class TeacherGradeInvalidTests
{
    private StudentManagementDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<StudentManagementDbContext>()
            .UseInMemoryDatabase(databaseName: "TeacherTestDB_" + Guid.NewGuid())
            .Options;

        return new StudentManagementDbContext(options);
    }

    private HomeController GetController(StudentManagementDbContext context)
    {
        var mockUser = new ApplicationUser { Id = "T1", UserName = "teacher1" };

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

        userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(mockUser);

        var controller = new HomeController(context, userManagerMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "T1")
                }, "mock"))
            }
        };

        return controller;
    }

    [Fact]
    public async void EnterGrade_ScoreMoreThan10_ReturnsError()
    {
        // Arrange
        var context = GetDbContext();

        context.Classes.Add(new Class
        {
            Id = 1,
            TeacherId = "T1",
            ClassName = "Test Class",
            Room = "A1", // bắt buộc
            IsActive = true
        });

        context.StudentEntity.Add(new StudentEntity { Id = 1 });

        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result = await controller.SaveGrade(1, 1, "quiz", 11); // score > 10
        var json = Assert.IsType<JsonResult>(result);

        // Convert to JsonElement
        var jsonString = JsonSerializer.Serialize(json.Value);
        var doc = JsonDocument.Parse(jsonString);
        var root = doc.RootElement;

        // Assert
        Assert.False(root.GetProperty("success").GetBoolean());
        Assert.Equal("Score must be between 0 and 10.", root.GetProperty("error").GetString());
    }
}
