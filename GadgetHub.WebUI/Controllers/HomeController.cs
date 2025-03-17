using System.Diagnostics;
using GadgetHub.Domain.Data;
using GadgetHub.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly GadgetHubContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(GadgetHubContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.Include(p => p.Category).ToListAsync();

        var productViewModels = products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            PriceFormatted = p.Price.ToString("C"),
            CategoryName = p.Category?.Name ?? "Uncategorized"
        }).ToList();

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