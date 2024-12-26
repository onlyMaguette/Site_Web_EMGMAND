namespace EMGMAND.Models;

using System.ComponentModel.DataAnnotations;

public class CarBrand
{
    public int Id { get; set; } // Identifiant unique de la marque

    [Required(ErrorMessage = "Le nom de la marque est obligatoire.")]
    [StringLength(50, ErrorMessage = "Le nom de la marque ne peut pas dépasser 50 caractères.")]
    public string Name { get; set; } // Nom de la marque
}
