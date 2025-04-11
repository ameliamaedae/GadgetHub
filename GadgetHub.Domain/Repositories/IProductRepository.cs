using GadgetHub.Domain.Entities;

namespace GadgetHub.Domain.Repositories;

public interface IProductRepository
{
    IQueryable<Product> Products { get; }
    IQueryable<Category> Categories { get; }
    void SaveProduct(Product product);
    Product? DeleteProduct(int productId);
}