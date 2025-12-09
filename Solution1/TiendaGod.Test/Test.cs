using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using TiendaGod.Identity.Controllers;
using TiendaGod.Shared.Events;

namespace TiendaGod.Test
{
    public class Tests
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IPublishEndpoint> _mockPublishEndpoint;
        private Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private AuthController _authController;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
                _mockUserManager.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                null, null, null, null);

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["Jwt:Key"]).Returns("EstaEsUnaClaveMuySegura12345");
            _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("TiendaGodIssuer");
            _mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("TiendaGodAudience");
            _mockConfiguration.Setup(config => config["Jwt:ExpireMinutes"]).Returns("60");

            _mockPublishEndpoint = new Mock<IPublishEndpoint>();

            _authController = new AuthController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockConfiguration.Object,
                _mockPublishEndpoint.Object
            );
        }

        [Test]

        public async Task Register_WithValidCredentials_ShouldReturnTrue()
        {
            var email = "test@correo.com";
            var password ="TestPassword123!";
            var user = new IdentityUser
            {
                UserName = "testuser",
                Email = email
            };

            var userModel = new RegisterModel("test", email, password);

            _mockUserManager.Setup(c => c.CreateAsync(It.IsAny<IdentityUser>(), password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(c => c.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _mockPublishEndpoint.Setup(c => c.Publish(It.IsAny<object>(), default))
                .Returns(Task.CompletedTask);

            var result = await _authController.Register(userModel);

         
            Assert.That(result, Is.True);
            _mockUserManager.Verify(c => c.CreateAsync(It.Is<IdentityUser>(c => 
                c.Email == email && c.UserName == "test"), password), Times.Once);
            _mockUserManager.Verify(c => c.FindByEmailAsync(email), Times.Once);
            _mockPublishEndpoint.Verify(c => c.Publish(It.Is<UserCreatedEvent>(c =>
                c.userId == user.Id && c.email == email), default), Times.Once);
        }
    }
}