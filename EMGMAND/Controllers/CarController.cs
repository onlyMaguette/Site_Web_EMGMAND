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
                return View(new Car()); // Ajouter une nouvelle voiture
            else
                return View(_context.Cars.Find(id)); // Modifier une voiture existante
        }

        // Action pour sauvegarder les modifications
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCar(Car car)
        {
            if (ModelState.IsValid)
            {
                // Validation de l'année de la voiture (doit être supérieure ou égale à 2010)
                if (car.Year < 2010)
                {
                    ModelState.AddModelError("Year", "L'année de la voiture doit être supérieure ou égale à 2010.");
                }

                if (ModelState.IsValid)
                {
                    if (car.Id == 0)
                        _context.Cars.Add(car); // Ajouter une nouvelle voiture
                    else
                        _context.Cars.Update(car); // Modifier une voiture existante

                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index)); // Rediriger vers la liste des voitures
                }
            }

            ViewBag.CarBrands = _context.CarBrands.ToList();
            return View("AddOrEdit", car);
        }
    }
}
