using System.Diagnostics;
using GadgetHub.Domain.Data;
using GadgetHub.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.WebUI.Controllers;

public class HomeController : Controller
{
    private const int PageSize = 6; // Number of products per page
    private readonly GadgetHubContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(GadgetHubContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Accepts an optional page parameter (defaults to 1)
    public async Task<IActionResult> Index(int page = 1)
    {
        // Load all products + categories in one go
        var allProducts = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        var totalProducts = allProducts.Count;
        var paged = allProducts
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        // Map to view models, including the ImageUrl if available
        var productViewModels = paged.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            PriceFormatted = p.Price.ToString("C"),
            CategoryName = p.Category?.Name ?? "Uncategorized",

            // Build URL to your GetImage action (or leave null for placeholders)
            ImageUrl = p.ImageData != null
                ? Url.Action("GetImage", "Product", new { productId = p.Id })
                : null
        }).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

        return View(productViewModels);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}