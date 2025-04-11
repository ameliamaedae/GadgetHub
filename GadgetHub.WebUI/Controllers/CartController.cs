using GadgetHub.Domain.Data;
using GadgetHub.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GadgetHub.WebUI.Controllers;

public class CartController : Controller
{
    private readonly GadgetHubContext _context;

    public CartController(GadgetHubContext context)
    {
        _context = context;
    }

    // Show cart contents
    public IActionResult Index()
    {
        var cart = GetCartFromSession();
        return View(cart);
    }

    [HttpPost]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove("Cart");
        TempData["CartAlert"] = "Your cart has been cleared.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    // 1. Accept currentPage as a parameter
    public IActionResult Add(int productId, int currentPage)
    {
        var cart = GetCartFromSession() ?? new List<CartItem>();
        var existingItem = cart.FirstOrDefault(ci => ci.ProductId == productId);

        if (existingItem == null)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
                return NotFound(); // or handle differently

            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = 1
            });

            TempData["CartAlert"] = $"<strong>{product.Name}</strong> was added to your cart!";
        }
        else
        {
            existingItem.Quantity++;
            TempData["CartAlert"] =
                $"<strong>{existingItem.ProductName}</strong> quantity increased to {existingItem.Quantity}.";
        }

        SaveCartToSession(cart);

        // 2. Redirect back to Home/Index with the same page number
        return RedirectToAction("Index", "Home", new { page = currentPage });
    }

    [HttpPost]
    public IActionResult Update(int productId, int quantity)
    {
        var cart = GetCartFromSession() ?? new List<CartItem>();
        var item = cart.FirstOrDefault(ci => ci.ProductId == productId);

        if (item != null && quantity > 0)
        {
            item.Quantity = quantity;
            TempData["CartAlert"] = $"<strong>{item.ProductName}</strong> quantity updated to {quantity}.";
        }

        SaveCartToSession(cart);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Remove(int productId)
    {
        var cart = GetCartFromSession() ?? new List<CartItem>();
        var itemToRemove = cart.FirstOrDefault(ci => ci.ProductId == productId);
        if (itemToRemove != null)
        {
            cart.Remove(itemToRemove);
            TempData["CartAlert"] = $"<strong>{itemToRemove.ProductName}</strong> was removed from your cart.";
        }

        SaveCartToSession(cart);
        return RedirectToAction("Index");
    }

    private List<CartItem> GetCartFromSession()
    {
        var cartJson = HttpContext.Session.GetString("Cart");
        if (!string.IsNullOrEmpty(cartJson)) return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        return new List<CartItem>();
    }

    private void SaveCartToSession(List<CartItem> cart)
    {
        var cartJson = JsonConvert.SerializeObject(cart);
        HttpContext.Session.SetString("Cart", cartJson);
    }
}