using GadgetHub.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

public class ProductController : Controller
{
    private readonly IProductRepository _repo;

    public ProductController(IProductRepository repo)
    {
        _repo = repo;
    }

    public FileContentResult GetImage(int productId)
    {
        var prod = _repo.Products.FirstOrDefault(p => p.Id == productId);
        if (prod?.ImageData != null)
            return File(prod.ImageData, prod.ImageMimeType);
        return null; // or return a placeholder
    }
}