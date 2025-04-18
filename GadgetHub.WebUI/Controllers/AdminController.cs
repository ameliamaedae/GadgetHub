using System.IO;
using System.Linq;
using GadgetHub.Domain.Entities;
using GadgetHub.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IProductRepository _repo;

        public AdminController(IProductRepository repo)
        {
            _repo = repo;
        }

        // Helper – populates the Categories dropdown for Create and Edit views.
        private void PopulateCategoriesDropDownList(int? selectedId = null)
        {
            ViewBag.Categories = new SelectList(
                _repo.Categories.OrderBy(c => c.Name),
                "Id", "Name", selectedId);
        }

        // GET: /Admin
        [HttpGet]
        public IActionResult Index()
        {
            var products = _repo.Products
                .Include(p => p.Category)
                .ToList();
            return View(products);
        }

        // GET: /Admin/Create
        [HttpGet]
        public IActionResult Create()
        {
            PopulateCategoriesDropDownList();
            return View("Edit", new Product());
        }

        // GET: /Admin/Edit/5
        [HttpGet]
        public IActionResult Edit(int productId)
        {
            var product = _repo.Products
                .FirstOrDefault(p => p.Id == productId);
            PopulateCategoriesDropDownList(product?.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product model, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategoriesDropDownList(model.CategoryId);
                return View(model);
            }

            if (model.Id == 0)
            {
                // — New product —
                if (image != null && image.Length > 0)
                {
                    using var ms = new MemoryStream();
                    image.CopyTo(ms);
                    model.ImageData     = ms.ToArray();
                    model.ImageMimeType = image.ContentType;
                }

                _repo.SaveProduct(model);
            }
            else
            {
                // — Existing product: load the tracked entity
                var prod = _repo.Products.FirstOrDefault(p => p.Id == model.Id);
                if (prod == null)
                    return NotFound();

                // Copy over the scalar properties
                prod.Name        = model.Name;
                prod.Description = model.Description;
                prod.Price       = model.Price;
                prod.CategoryId  = model.CategoryId;

                // If a new image was uploaded, overwrite; otherwise leave the old one
                if (image != null && image.Length > 0)
                {
                    using var ms = new MemoryStream();
                    image.CopyTo(ms);
                    prod.ImageData     = ms.ToArray();
                    prod.ImageMimeType = image.ContentType;
                }

                _repo.SaveProduct(prod);
            }

            TempData["message"] = $"✅ '{model.Name}' has been saved.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Delete/5
        [HttpGet]
        public IActionResult Delete(int productId)
        {
            var deleted = _repo.DeleteProduct(productId);
            if (deleted != null)
                TempData["message"] = $"🗑️ '{deleted.Name}' was deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
