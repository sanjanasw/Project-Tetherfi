using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Tetherfi.Data;
using Tetherfi.Data.Models;
using AutoMapper;
using Tetherfi.Helpers.Exeptions;
using Tetherfi.Common.Models;
using Tetherfi.Common.Enums;
using Tetherfi.Service.Interfaces;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tetherfi.Data.Enums;
using Tetherfi.Services.Mappings;

namespace Tetherfi.Services.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            _mockAuthService = new Mock<IAuthService>();
            _mockMapper = new Mock<IMapper>();
            var configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperProfile>();
            });
            var mapper = new Mapper(configuration);
            _mockMapper.Setup(x => x.Map<User>(It.IsAny<ApplicationUserData>()))
                .Returns((ApplicationUserData data) => mapper.Map<User>(data));
            _mockLogger = new Mock<ILogger<UserService>>();

            _userService = new UserService(
                _mockContext.Object,
                _mockUserManager.Object,
                _mockRoleManager.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockAuthService.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidModel_ReturnsUser()
        {
            // Arrange
            var registerModel = new Register
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john.doe@example.com",
                Role = UserRole.User,
                Password = "Password123",
                Dob = new DateTime(1990, 1, 1)
            };

            var user = new ApplicationUser
            {
                FirstName = "John",
                LastName = "Doe",
                Dob = DateTime.Now,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                ProfilePicture = "profile.jpg",
            };

            _mockMapper.Setup(x => x.Map<ApplicationUser>(registerModel)).Returns(user);
            _mockUserManager.Setup(x => x.CreateAsync(user, registerModel.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockRoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockAuthService.Setup(x => x.GetCurrentLoggedInUsername()).Returns("admin");

            // Act
            var result = await _userService.CreateAsync(registerModel);

            // Assert
            Assert.NotNull(result);
        }


        [Fact]
        public async Task UpdateUserAsync_ValidModel_ReturnsUpdatedUser()
        {
            // Arrange
            var userId = "testUserId";
            var model = new UserUpdate
            {
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                UserName = "UpdatedUserName",
                Email = "updated.email@example.com",
                Dob = new DateTime(1990, 1, 1),
                ProfilePicture = "updated_profile.jpg"
            };
            var user = new ApplicationUser
            {
                FirstName = "John",
                LastName = "Doe",
                Dob = DateTime.Now,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                ProfilePicture = "profile.jpg",
            };
            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            _mockAuthService.Setup(x => x.GetCurrentLoggedInUsername()).Returns("admin");

            // Act
            var result = await _userService.UpdateUserAsync(userId, model);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateUserAsync_UserNotFound_ThrowsHumanErrorException()
        {
            // Arrange
            var userId = "testUserId";
            var model = new UserUpdate
            {
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                UserName = "UpdatedUserName",
                Email = "updated.email@example.com",
                Dob = new DateTime(1990, 1, 1),
                ProfilePicture = "updated_profile.jpg"
            };
            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<HumanErrorException>(() => _userService.UpdateUserAsync(userId, model));
        }

    }
}
