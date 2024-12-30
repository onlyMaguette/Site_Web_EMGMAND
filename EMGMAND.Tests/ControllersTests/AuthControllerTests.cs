using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Moq;
using EMGMAND.Data;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;

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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Utiliser un nom unique pour chaque test
                .Options;

            _context = new ApplicationDbContext(options);
            var store = new UserStore<IdentityUser>(_context);

            // Configuration du UserManager
            _userManager = new UserManager<IdentityUser>(
                store,
                new OptionsManager<IdentityOptions>(
                    new OptionsFactory<IdentityOptions>(
                        new List<IConfigureOptions<IdentityOptions>>(),
                        new List<IPostConfigureOptions<IdentityOptions>>()
                    )
                ),
                new PasswordHasher<IdentityUser>(),
                new List<IUserValidator<IdentityUser>>(),
                new List<IPasswordValidator<IdentityUser>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new Mock<IServiceProvider>().Object, // Remplacement de null par un mock
                new Mock<ILogger<UserManager<IdentityUser>>>().Object // Remplacement de null par un mock
            );

            // Mock du HttpContext
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            // Mock du UserClaimsPrincipalFactory
            var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            mockClaimsFactory
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync((IdentityUser user) => new ClaimsPrincipal());

            // Configuration du SignInManager avec l'ordre correct des paramètres
            _signInManager = new SignInManager<IdentityUser>(
    _userManager,                                // 1er argument : UserManager<IdentityUser>
    mockHttpContextAccessor.Object,              // 2e argument : IHttpContextAccessor
    mockClaimsFactory.Object,                    // 3e argument : IUserClaimsPrincipalFactory<IdentityUser>
    new OptionsManager<IdentityOptions>(         // 4e argument : IOptions<IdentityOptions>
        new OptionsFactory<IdentityOptions>(
            new List<IConfigureOptions<IdentityOptions>>(),
            new List<IPostConfigureOptions<IdentityOptions>>()
        )
    ),
    new Mock<ILogger<SignInManager<IdentityUser>>>().Object, // 5e argument : ILogger<SignInManager<IdentityUser>>
    new Mock<IAuthenticationSchemeProvider>().Object, // 6e argument : IAuthenticationSchemeProvider
    new Mock<IUserConfirmation<IdentityUser>>().Object // 7e argument : IUserConfirmation<IdentityUser>
);

        }

        [Fact]
        public async Task Login_ValidUser_ReturnsSignInResult()
        {
            // Arrange
            var user = new IdentityUser
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                EmailConfirmed = true // Important pour la validation
            };

            // Nettoyer le contexte avant de créer un nouvel utilisateur
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            // Créer l'utilisateur
            var createResult = await _userManager.CreateAsync(user, "Password123!");
            Assert.True(createResult.Succeeded, "User creation failed");

            // Act
            var signInResult = await _signInManager.PasswordSignInAsync(
                user.UserName,
                "Password123!",
                isPersistent: false,
                lockoutOnFailure: false);

            // Assert
            Assert.True(signInResult.Succeeded, "Sign in failed");
        }

        // Nettoyer les ressources
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
