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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Nettoyer la base de donn�es
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Ajouter la marque par d�faut
            _context.CarBrands.Add(new CarBrand {Name = "Default Brand" });
            _context.SaveChanges();

            // Get the actual generated Id
            var defaultBrand = _context.CarBrands.First();
            _context.ChangeTracker.Clear();

            _controller = new CarController(_context);
        }

        [Fact]
        public void AddCar_ValidCar_ReturnsRedirectToActionResult()
        {
            // R�cup�rer la marque d�j� existante
            var defaultBrand = _context.CarBrands.First();

            // Cr�er un nouveau v�hicule en r�utilisant la marque existante
            var car = new Car
            {
                Model = "Test Model",
                Description = "Test Description",
                Year = 2020,
                IsSold = false,
                BrandId = defaultBrand.Id, // R�utiliser l'Id de la marque existante
                Brand = defaultBrand, // R�utiliser l'instance de la marque existante
                IsAvailable = true,
                ManufactureDate = DateTime.Now,
                PhotoPath = "/images/default.jpg"  // Ajout de PhotoPath
            };

            // Sauvegarder le v�hicule dans la base de donn�es
            var result = _controller.SaveCar(car);

            // V�rifier que l'action retourne un Redirection
            Assert.IsType<RedirectToActionResult>(result);
            var redirectToActionResult = (RedirectToActionResult)result;  // Cast s�r
            Assert.Equal("Index", redirectToActionResult.ActionName);  // V�rifier la redirection vers Index
        }


        [Fact]
        public void EditCar_ValidData_UpdatesCar()
        {
            // R�cup�rer la marque d�j� existante
            var defaultBrand = _context.CarBrands.First();

            // Cr�er un nouveau v�hicule en r�utilisant la marque existante
            var car = new Car
            {
                Model = "Initial Model",
                Description = "Initial Description",
                Year = 2021,
                IsSold = true,
                BrandId = defaultBrand.Id, // R�utiliser l'Id de la marque existante
                Brand = defaultBrand, // R�utiliser l'instance de la marque existante
                IsAvailable = false,
                ManufactureDate = DateTime.Now,
                PhotoPath = "/images/default.jpg"  // Ajout de PhotoPath
            };

            // Ajouter le v�hicule dans la base de donn�es
            _context.Cars.Add(car);
            _context.SaveChanges();

            // Modifier les donn�es du v�hicule
            car.Description = "Updated Description";

            // Sauvegarder les modifications
            _controller.SaveCar(car);
            _context.SaveChanges();

            // V�rifier que les modifications ont bien �t� prises en compte
            var updatedCar = _context.Cars.Find(car.Id);
            Assert.NotNull(updatedCar);
            Assert.Equal("Updated Description", updatedCar.Description);
        }


        [Fact]
        public void SaveCar_InvalidCar_ReturnsViewWithError()
        {
            var defaultBrand = _context.CarBrands.First();
            var car = new Car
            {
                Model = "", // Mod�le invalide
                Year = 2020,
                IsSold = false,
                BrandId = defaultBrand.Id,
                Brand = new CarBrand { Id = defaultBrand.Id, Name = "Default Brand" },
                IsAvailable = true,
                ManufactureDate = DateTime.Now,
                PhotoPath = "/images/default.jpg"  // Ajout de PhotoPath
            };

            var result = _controller.SaveCar(car);
            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;  // Cast s�r au lieu de 'as'
            Assert.NotNull(viewResult);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }
    }
}