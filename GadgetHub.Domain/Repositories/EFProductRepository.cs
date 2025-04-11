using GadgetHub.Domain.Data;
using GadgetHub.Domain.Entities;

namespace GadgetHub.Domain.Repositories;

public class EFProductRepository : IProductRepository
{
    private readonly GadgetHubContext _ctx;

    public EFProductRepository(GadgetHubContext ctx)
    {
        _ctx = ctx;
    }

    public IQueryable<Product> Products => _ctx.Products;
    public IQueryable<Category> Categories => _ctx.Categories;

    public void SaveProduct(Product product)
    {
        if (product.Id == 0)
            _ctx.Products.Add(product);
        else
            _ctx.Products.Update(product);
        _ctx.SaveChanges();
    }

    public Product? DeleteProduct(int productId)
    {
        var prod = _ctx.Products.FirstOrDefault(p => p.Id == productId);
        if (prod == null) return null;
        _ctx.Products.Remove(prod);
        _ctx.SaveChanges();
        return prod;
    }
}