namespace EMGMAND.Models;
using System.ComponentModel.DataAnnotations;

public class Car
{
    public int Id { get; set; }

    [Required]
    public string Brand { get; set; }

    [Required]
    public string Model { get; set; }

    [Required]
    [Range(2010, 9999)]
    public int Year { get; set; }

    public string Description { get; set; }
    public string PhotoPath { get; set; }
    public bool IsSold { get; set; }
}

