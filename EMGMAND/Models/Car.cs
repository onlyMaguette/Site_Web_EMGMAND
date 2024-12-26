namespace EMGMAND.Models;

using System.ComponentModel.DataAnnotations;

public class Car
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La marque est obligatoire.")]
    [StringLength(50, ErrorMessage = "La marque ne peut pas dépasser 50 caractères.")]
    public string Brand { get; set; }

    [Required(ErrorMessage = "Le modèle est obligatoire.")]
    [StringLength(50, ErrorMessage = "Le modèle ne peut pas dépasser 50 caractères.")]
    public string Model { get; set; }

    [Required(ErrorMessage = "L'année est obligatoire.")]
    [Range(2010, 9999, ErrorMessage = "L'année doit être comprise entre 2010 et l'année actuelle.")]
    public int Year { get; set; }

    [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
    public string Description { get; set; }

    [Url(ErrorMessage = "Veuillez fournir un chemin d'image valide.")]
    public string PhotoPath { get; set; }

    [Required]
    public bool IsSold { get; set; } // Indique si la voiture est vendue.

    [Required]
    public bool IsAvailable { get; set; } // Indique si la voiture est disponible à la vente.
}
