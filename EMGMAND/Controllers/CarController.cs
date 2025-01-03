using EMGMAND.Models;
using EMGMAND.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EMGMAND.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cars = _context.Cars.ToList();
            return View(cars);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddOrEdit(int id = 0)
        {
            ViewBag.CarBrands = _context.CarBrands.ToList();

            if (id == 0)
            {
                var newCar = new Car
                {
                    Model = "",
                    Year = DateTime.Now.Year,
                    IsSold = false,
                    IsAvailable = true,
                    PhotoPath = "",
                    ManufactureDate = DateTime.Now,
                    Brand = new CarBrand { Name = "Marque par défaut" }
                };
                return View(newCar);
            }

            var car = _context.Cars.Find(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveCar(Car car)
        {
            if (ModelState.IsValid)
            {
                if (car.Id == 0)
                {
                    _context.Cars.Add(car);
                }
                else
                {
                    _context.Cars.Update(car);
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CarBrands = _context.CarBrands.ToList();
            return View("AddOrEdit", car);
        }
    }
}