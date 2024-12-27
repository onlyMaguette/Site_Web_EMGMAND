using Xunit;
using EMGMAND.Controllers;
using EMGMAND.Models;
using EMGMAND.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMGMAND.Tests.ControllersTests
{
	public class CarControllerTests
	{
		private readonly CarController _controller;
		private readonly ApplicationDbContext _context;

		public CarControllerTests()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase("TestDatabase")
				.Options;

			_context = new ApplicationDbContext(options);
			_controller = new CarController(_context);
		}

		[Fact]
		public void AddCar_ValidCar_ReturnsRedirectToActionResult()
		{
			var car = new Car
			{
				Model = "Test Model",
				Description = "Test Description",
				Year = 2020,
				IsSold = false,
				BrandId = 1 // On s'assure ici que la marque existe
			};

			var result = _controller.SaveCar(car);

			Assert.IsType<RedirectToActionResult>(result);
			var redirectToActionResult = result as RedirectToActionResult;
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}

		[Fact]
		public void EditCar_ValidData_UpdatesCar()
		{
			var car = new Car
			{
				Id = 1,
				Model = "Updated Model",
				Description = "Updated Description",
				Year = 2021,
				IsSold = true,
				BrandId = 2
			};

			_context.Cars.Add(car);
			_context.SaveChanges();

			car.Description = "New Description";

			_controller.SaveCar(car);
			_context.SaveChanges();

			var updatedCar = _context.Cars.Find(1);
			Assert.Equal("New Description", updatedCar.Description);
		}

		[Fact]
		public void SaveCar_InvalidCar_ReturnsViewWithError()
		{
			var car = new Car
			{
				Model = "",  // Invalid model
				Year = 2020,
				IsSold = false,
				BrandId = 1
			};

			var result = _controller.SaveCar(car);

			Assert.IsType<ViewResult>(result);
			var viewResult = result as ViewResult;
			Assert.False(viewResult.ViewData.ModelState.IsValid);
		}
	}
}
