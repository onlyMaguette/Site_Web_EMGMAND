using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Ajouté pour UserStore<>
using Microsoft.AspNetCore.Http; // Ajouté pour IHttpContextAccessor et HttpContext
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using EMGMAND.Data;
using Microsoft.Extensions.Logging; // Ajouté pour ILogger<>
using Microsoft.AspNetCore.Authentication; // Ajouté pour IAuthenticationSchemeProvider
using System;
using System.Linq;
using System.Security.Claims; // Ajouté pour ClaimsPrincipal
using System.Threading.Tasks;

namespace EMGMAND.Tests.ControllersTests
{
    public class AuthenticationTests : IDisposable
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AuthenticationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            var store = new UserStore<IdentityUser>(_context);

            _userManager = new UserManager<IdentityUser>(store,
                new OptionsManager<IdentityOptions>(new OptionsFactory<IdentityOptions>(
                    new List<IConfigureOptions<IdentityOptions>>(),
                    new List<IPostConfigureOptions<IdentityOptions>>()
                )),
                new PasswordHasher<IdentityUser>(),
                new List<IUserValidator<IdentityUser>>(),
                new List<IPasswordValidator<IdentityUser>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<IdentityUser>>>().Object
            );

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

            mockClaimsFactory.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync((IdentityUser user) => new ClaimsPrincipal(
                    new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, user.UserName ?? "") })
                ));

            mockAuthService.Setup(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            mockServiceProvider.Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(mockAuthService.Object);

            mockHttpContext.Setup(x => x.RequestServices).Returns(mockServiceProvider.Object);
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            _signInManager = new SignInManager<IdentityUser>(
                _userManager,
                mockHttpContextAccessor.Object,
                mockClaimsFactory.Object,
                new OptionsManager<IdentityOptions>(new OptionsFactory<IdentityOptions>(
                    new List<IConfigureOptions<IdentityOptions>>(),
                    new List<IPostConfigureOptions<IdentityOptions>>()
                )),
                new Mock<ILogger<SignInManager<IdentityUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<IdentityUser>>().Object
            );
        }

        [Fact]
        public async Task Login_ValidUser_ReturnsSignInResult()
        {
            var user = new IdentityUser
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                EmailConfirmed = true
            };

            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            var createResult = await _userManager.CreateAsync(user, "Password123!");
            Assert.True(createResult.Succeeded, "User creation failed");

            // Assurez-vous que l'utilisateur existe avant de tenter la connexion
            var userToSignIn = await _userManager.FindByNameAsync(user.UserName);
            Assert.NotNull(userToSignIn);

            var signInResult = await _signInManager.PasswordSignInAsync(
                userToSignIn,
                "Password123!",
                isPersistent: false,
                lockoutOnFailure: false
            );

            Assert.True(signInResult.Succeeded, "Sign in failed");
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
