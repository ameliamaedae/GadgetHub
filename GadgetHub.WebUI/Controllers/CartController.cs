using GadgetHub.Domain.Data;
using GadgetHub.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GadgetHub.WebUI.Controllers
{
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
        public IActionResult Add(int productId)
        {
            var cart = GetCartFromSession() ?? new List<CartItem>();

            // Check if this product is already in the cart
            var existingItem = cart.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingItem == null)
            {
                // Fetch the product from DB so we have name, price, etc.
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
            }
            else
            {
                existingItem.Quantity++;
            }

            SaveCartToSession(cart);

            // Redirect wherever you want: back to home or cart index
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Update(int productId, int quantity)
        {
            var cart = GetCartFromSession() ?? new List<CartItem>();
            var item = cart.FirstOrDefault(ci => ci.ProductId == productId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
            }

            SaveCartToSession(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            var cart = GetCartFromSession() ?? new List<CartItem>();
            cart.RemoveAll(ci => ci.ProductId == productId);

            SaveCartToSession(cart);
            return RedirectToAction("Index");
        }

        private List<CartItem> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(cartJson))
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            }
            return new List<CartItem>();
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }
    }
}
