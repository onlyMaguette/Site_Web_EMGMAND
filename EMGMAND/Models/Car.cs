namespace EMGMAND.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La marque est obligatoire.")]
        public int BrandId { get; set; } // Clé étrangère vers la table des marques
        public required CarBrand Brand { get; set; } // Propriété de navigation vers la marque

        [Required(ErrorMessage = "Le modèle est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le modèle ne peut pas dépasser 50 caractères.")]
        public required string Model { get; set; }

        [Required(ErrorMessage = "L'année est obligatoire.")]
        [Range(2010, int.MaxValue, ErrorMessage = "L'année doit être comprise entre 2010 et l'année actuelle.")]
        public required int Year { get; set; }

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Veuillez fournir un chemin d'image valide.")]
        public required string PhotoPath { get; set; }

        [Required(ErrorMessage = "Veuillez spécifier si la voiture est vendue.")]
        public required bool IsSold { get; set; }

        [Required(ErrorMessage = "Veuillez spécifier si la voiture est disponible.")]
        public required bool IsAvailable { get; set; }

        [Required(ErrorMessage = "La date de fabrication est obligatoire.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de fabrication")]
        public required DateTime ManufactureDate { get; set; }

        // Constructeur sans paramètres pour initialisation par défaut
        public Car()
        {
            BrandId = 0;  // Valeur par défaut
            Model = "";
            Year = 2010;
            IsSold = false;
            IsAvailable = true;
            PhotoPath = "";
            ManufactureDate = DateTime.Now;
        }

        // Constructeur pour initialiser les propriétés non-nullables
        public Car(int brandId = 0, string model = "", int year = 2010, bool isSold = false, bool isAvailable = true, DateTime manufactureDate = default, string description = "", string photoPath = "")
        {
            BrandId = brandId;
            Model = model;
            Year = year;
            IsSold = isSold;
            IsAvailable = isAvailable;
            ManufactureDate = manufactureDate == default ? DateTime.Now : manufactureDate; // Assurez-vous que ManufactureDate est défini
            Description = description ?? "";
            PhotoPath = photoPath ?? "";
        }


    }
}
