namespace EMGMAND.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CarBrand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la marque est obligatoire.")]
        public required string Name { get; set; }

        // Constructeur pour initialiser les membres obligatoires
        public CarBrand(string name = "")
        {
            Name = name;
        }
    }
}
