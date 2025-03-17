namespace GadgetHub.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Navigation property: a category can have many products
    public ICollection<Product> Products { get; set; } = new List<Product>();
}