using EMGMAND.Models;
using EMGMAND.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EMGMAND.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action pour afficher le formulaire d'ajout ou de modification
        public IActionResult AddOrEdit(int id = 0)
        {
            ViewBag.CarBrands = _context.CarBrands.ToList();
            if (id == 0)
            {
                var newCar = new Car
                {
                    Model = "",
                    Year = 2010,
                    IsSold = false,
                    IsAvailable = true,
                    PhotoPath = "",
                    ManufactureDate = DateTime.Now,
                    Brand = new CarBrand
                    {
                        Name = "Marque par défaut" // Initialisation du membre obligatoire
                    }
                };
                return View(newCar); // Ajouter une nouvelle voiture
            }
            else
            {
                var car = _context.Cars.Find(id);
                if (car == null)
                {
                    return NotFound();
                }
                return View(car); // Modifier une voiture existante
            }
        }

        // Action pour sauvegarder les modifications
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCar(Car car)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(car.Model))
                {
                    ModelState.AddModelError("Model", "Le modèle est obligatoire.");
                }

                if (car.Year < 2010)
                {
                    ModelState.AddModelError("Year", "L'année de la voiture doit être supérieure ou égale à 2010.");
                }

                if (car.Brand == null || string.IsNullOrWhiteSpace(car.Brand.Name))
                {
                    ModelState.AddModelError("Brand.Name", "Le nom de la marque est obligatoire.");
                }

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
            }

            ViewBag.CarBrands = _context.CarBrands.ToList();
            return View("CarForm", car);
        }
    }
}
