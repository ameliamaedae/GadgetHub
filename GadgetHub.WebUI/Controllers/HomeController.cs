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
        // Fetch all products including their Category data.
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        // Total count for pagination
        var totalProducts = products.Count;

        // Apply pagination: skip products for previous pages, take current page's products
        var paginatedProducts = products
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        // Map products to view models
        var productViewModels = paginatedProducts.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            PriceFormatted = p.Price.ToString("C"),
            CategoryName = p.Category?.Name ?? "Uncategorized"
        }).ToList();

        // Pass current page and total pages info to the view via ViewBag
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