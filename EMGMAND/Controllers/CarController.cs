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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var cars = _context.Cars.Include(c => c.Brand).ToList();
            return View(cars);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddOrEdit(int id = 0)
        {
            ViewBag.CarBrands = _context.CarBrands.ToList();

            if (id == 0)
            {
                var defaultBrand = _context.CarBrands.FirstOrDefault();
                var newCar = new Car
                {
                    Model = "",
                    Year = DateTime.Now.Year,
                    IsSold = false,
                    IsAvailable = true,
                    PhotoPath = "",
                    ManufactureDate = DateTime.Now,
                    Brand = defaultBrand ?? new CarBrand { Name = "Marque par défaut" },
                    BrandId = defaultBrand?.Id ?? 0
                };
                return View(newCar);
            }

            var car = _context.Cars.Include(c => c.Brand).FirstOrDefault(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveCar([Bind("Id,BrandId,Model,Year,Description,IsSold,IsAvailable,ManufactureDate,PhotoPath")] Car car, IFormFile? photoFile)
        {
            try
            {
                // Récupération et association de la marque
                var brand = await _context.CarBrands.FindAsync(car.BrandId);
                if (brand == null)
                {
                    ModelState.AddModelError("BrandId", "Marque invalide");
                    ViewBag.CarBrands = _context.CarBrands.ToList();
                    return View("AddOrEdit", car);
                }
                car.Brand = brand;

                // Gestion de l'upload de fichier
                if (photoFile != null && photoFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(photoFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("PhotoFile", "Seuls les fichiers JPG, JPEG et PNG sont autorisés.");
                        ViewBag.CarBrands = _context.CarBrands.ToList();
                        return View("AddOrEdit", car);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(stream);
                    }

                    car.PhotoPath = "/images/" + uniqueFileName;
                }
                else if (car.Id == 0 && string.IsNullOrEmpty(car.PhotoPath))
                {
                    car.PhotoPath = "/images/default-car.jpg";
                }

                if (car.Id == 0)
                {
                    await _context.Cars.AddAsync(car);
                }
                else
                {
                    var existingCar = await _context.Cars
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == car.Id);

                    if (existingCar == null)
                    {
                        return NotFound();
                    }

                    if (photoFile == null)
                    {
                        car.PhotoPath = existingCar.PhotoPath;
                    }

                    _context.Entry(car).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Une erreur s'est produite lors de la sauvegarde : " + ex.Message);
                ViewBag.CarBrands = _context.CarBrands.ToList();
                return View("AddOrEdit", car);
            }
        }
    }
}