namespace GadgetHub.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    // Foreign key for Category
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}