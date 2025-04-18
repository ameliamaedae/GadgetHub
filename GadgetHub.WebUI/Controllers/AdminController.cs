using GadgetHub.Domain.Entities;
using GadgetHub.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace GadgetHub.WebUI.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly IProductRepository _repo;

    public AdminController(IProductRepository repo)
    {
        _repo = repo;
    }

    // Central helper – populates the Categories dropdown for Create and Edit views.
    private void PopulateCategoriesDropDownList(int? selectedId = null)
    {
        ViewBag.Categories = new SelectList(
            _repo.Categories.OrderBy(c => c.Name), // Sorted alphabetically
            "Id",
            "Name",
            selectedId);
    }

    // GET: /Admin
    public IActionResult Index()
    {
        // Load all products, including their Category details
        var products = _repo.Products.Include(p => p.Category).ToList();
        return View(products);
    }

    // GET: /Admin/Create
    public IActionResult Create()
    {
        PopulateCategoriesDropDownList();
        // Reuse the Edit view to create a new product
        return View("Edit", new Product());
    }

    // GET: /Admin/Edit/5
    public IActionResult Edit(int productId)
    {
        var product = _repo.Products.FirstOrDefault(p => p.Id == productId);
        PopulateCategoriesDropDownList(product?.CategoryId);
        return View(product);
    }

    // POST: /Admin/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Product product)
    {
        if (!ModelState.IsValid)
        {
            // Need to repopulate the category list if the model state is invalid
            PopulateCategoriesDropDownList(product.CategoryId);
            return View(product);
        }

        _repo.SaveProduct(product);
        TempData["message"] = $"✅ '{product.Name}' has been saved.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Delete/5
    public IActionResult Delete(int productId)
    {
        var deleted = _repo.DeleteProduct(productId);
        if (deleted != null)
            TempData["message"] = $"🗑️ '{deleted.Name}' was deleted.";
        return RedirectToAction(nameof(Index));
    }
    
    // POST: /Admin/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Product product, IFormFile image)
    {
        if (!ModelState.IsValid)
        {
            PopulateCategoriesDropDownList(product.CategoryId);
            return View(product);
        }

        // If editing, preserve existing image when none is uploaded:
        if (product.Id != 0)
        {
            var existing = _repo.Products.FirstOrDefault(p => p.Id == product.Id);
            product.ImageData     = existing?.ImageData;
            product.ImageMimeType = existing?.ImageMimeType;
        }

        if (image != null && image.Length > 0)
        {
            using var ms = new MemoryStream();
            image.CopyTo(ms);
            product.ImageData     = ms.ToArray();
            product.ImageMimeType = image.ContentType;
        }

        _repo.SaveProduct(product);
        TempData["message"] = $"✅ '{product.Name}' has been saved.";
        return RedirectToAction(nameof(Index));
    }
    
    
}