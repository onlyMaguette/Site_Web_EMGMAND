using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using EMGMAND;

namespace EMGMAND.Tests.ControllersTests
{
	public class AuthenticationTests
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public AuthenticationTests()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase("TestDatabase")
				.Options;

			var context = new ApplicationDbContext(options);
			var store = new UserStore<IdentityUser>(context);
			_userManager = new UserManager<IdentityUser>(store, null, null, null, null, null, null, null, null);
			_signInManager = new SignInManager<IdentityUser>(_userManager, null, null, null, null, null, null);
		}

		[Fact]
		public async Task Login_ValidUser_ReturnsSignInResult()
		{
			var user = new IdentityUser { UserName = "testuser", Email = "testuser@example.com" };
			await _userManager.CreateAsync(user, "Password123!");

			var result = await _signInManager.PasswordSignInAsync(user.UserName, "Password123!", false, false);

			Assert.True(result.Succeeded);
		}
	}
}
