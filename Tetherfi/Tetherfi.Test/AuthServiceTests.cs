using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Tetherfi.Service.Interfaces;
using System.Threading.Tasks;
using System.Net;
using Tetherfi.Helpers.Exeptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Tetherfi.Data.Models;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Tetherfi.Common.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tetherfi.Data;
using Tetherfi.Helpers.JWT;
using Tetherfi.Services;
using AutoMapper;
using Tetherfi.Services.Mappings;
using Tetherfi.Data.Enums;

namespace Tetherfi.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly Mock<IOptions<JWTConfigurations>> _mockJwtConfigurations;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);
            _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockMapper = new Mock<IMapper>();
            var configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperProfile>();
            });
            var mapper = new Mapper(configuration);
            _mockMapper.Setup(x => x.Map<User>(It.IsAny<ApplicationUserData>()))
                .Returns((ApplicationUserData data) => mapper.Map<User>(data));
            _mockLogger = new Mock<ILogger<AuthService>>();
            _mockJwtConfigurations = new Mock<IOptions<JWTConfigurations>>();
            _mockJwtConfigurations.Setup(x => x.Value).Returns(new JWTConfigurations
                {
                    Key = "testkeytestkeytestkeytestkeytestkey",
                    Expires = 60,
                    Audience = "https://localhost:4200",
                    Issuer = "https://localhost:5001"
            });
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _authService = new AuthService(
                _mockContext.Object,
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockJwtConfigurations.Object,
                _mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task SignInAsync_ValidCredentials_ReturnsJWTResult()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser"};
            var roles = new List<string> { "UserRole" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, "password", false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            _mockJwtConfigurations.Setup(x => x.Value).Returns(new JWTConfigurations { Key = "testkey", Expires = 60 });
            _mockHttpContextAccessor.Setup(x => x.HttpContext.User.Claims)
                .Returns(new List<Claim> { new Claim(ClaimTypes.GivenName, user.UserName) });

            // Act
            var result = await _authService.SignInAsync("testuser", "password");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.Equal(user.UserName, result.User.Username);
            Assert.NotNull(result.Expiration);
        }

        [Fact]
        public async Task SignInAsync_UserNotFound_ThrowsHumanErrorException()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<HumanErrorException>(() => _authService.SignInAsync("testuser", "password"));
        }

        [Fact]
        public async Task SignInAsync_IncorrectPassword_ThrowsHumanErrorException()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, "password", false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act & Assert
            await Assert.ThrowsAsync<HumanErrorException>(() => _authService.SignInAsync("testuser", "password"));
        }

        [Fact]
        public async Task SignInAsync_DeletedUser_ThrowsHumanErrorException()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser", Status = RecordStatus.Deleted };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<HumanErrorException>(() => _authService.SignInAsync("testuser", "password"));
        }

    }
}
