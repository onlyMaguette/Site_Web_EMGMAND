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

            // Ajouter une marque par défaut pour éviter les erreurs de clé étrangère
            _context.CarBrands.Add(new CarBrand { Id = 1, Name = "Default Brand" });
            _context.SaveChanges();
            _controller = new CarController(_context);  // Correction de la syntaxe
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
                BrandId = 1,
                Brand = new CarBrand { Id = 1, Name = "Default Brand" },
                IsAvailable = true,
                ManufactureDate = DateTime.Now,
                PhotoPath = "/images/default.jpg"  // Ajout de PhotoPath
            };

            var result = _controller.SaveCar(car);
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = (RedirectToActionResult)result;  // Cast sûr au lieu de 'as'
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void EditCar_ValidData_UpdatesCar()
        {
            var car = new Car
            {
                Id = 1,
                Model = "Initial Model",
                Description = "Initial Description",
                Year = 2021,
                IsSold = true,
                BrandId = 1,
                Brand = new CarBrand { Id = 1, Name = "Default Brand" },
                IsAvailable = false,
                ManufactureDate = DateTime.Now,
                PhotoPath = "/images/default.jpg"  // Ajout de PhotoPath
            };

            _context.Cars.Add(car);
            _context.SaveChanges();

            car.Description = "Updated Description";
            _controller.SaveCar(car);
            _context.SaveChanges();

            var updatedCar = _context.Cars.Find(1);
            Assert.NotNull(updatedCar);
            Assert.Equal("Updated Description", updatedCar.Description);
        }

        [Fact]
        public void SaveCar_InvalidCar_ReturnsViewWithError()
        {
            var car = new Car
            {
                Model = "", // Modèle invalide
                Year = 2020,
                IsSold = false,
                BrandId = 1,
                Brand = new CarBrand { Id = 1, Name = "Default Brand" },
                IsAvailable = true,
                ManufactureDate = DateTime.Now,
                PhotoPath = "/images/default.jpg"  // Ajout de PhotoPath
            };

            var result = _controller.SaveCar(car);
            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;  // Cast sûr au lieu de 'as'
            Assert.NotNull(viewResult);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }
    }
}