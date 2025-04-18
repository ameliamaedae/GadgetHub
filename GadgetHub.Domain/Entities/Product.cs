using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadgetHub.Domain.Entities;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] [StringLength(60)] public string Name { get; set; } = string.Empty;

    [StringLength(2000)] public string? Description { get; set; }

    [Range(0.01, 99999.99)] public decimal Price { get; set; }

    //[Display(Name = "Category")] public int CategoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please choose a category")]
    public int CategoryId { get; set; }

    public Category? Category { get; set; }
    
    public byte[]? ImageData     { get; set; }
    public string? ImageMimeType { get; set; }
}