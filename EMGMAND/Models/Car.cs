namespace EMGMAND.Models;

using System.ComponentModel.DataAnnotations;

public class Car
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La marque est obligatoire.")]
    public int BrandId { get; set; } // Clé étrangère vers la table des marques
    public CarBrand Brand { get; set; } // Propriété de navigation vers la marque

    [Required(ErrorMessage = "Le modèle est obligatoire.")]
    [StringLength(50, ErrorMessage = "Le modèle ne peut pas dépasser 50 caractères.")]
    public string Model { get; set; }

    [Required(ErrorMessage = "L'année est obligatoire.")]
    [Range(2010, int.MaxValue, ErrorMessage = "L'année doit être comprise entre 2010 et l'année actuelle.")]
    public int Year { get; set; }

    [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
    public string Description { get; set; }

    [Url(ErrorMessage = "Veuillez fournir un chemin d'image valide.")]
    public string PhotoPath { get; set; }

    [Required(ErrorMessage = "Veuillez spécifier si la voiture est vendue.")]
    public bool IsSold { get; set; } // Indique si la voiture est vendue.

    [Required(ErrorMessage = "Veuillez spécifier si la voiture est disponible.")]
    public bool IsAvailable { get; set; } // Indique si la voiture est disponible à la vente.

    [Required(ErrorMessage = "La date doit être sélectionnée correctement.")]
    public DateTime DateAdded { get; set; } // Date d'ajout de la voiture

}
