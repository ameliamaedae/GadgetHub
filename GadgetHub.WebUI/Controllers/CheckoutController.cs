using GadgetHub.WebUI.Models;
using GadgetHub.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace GadgetHub.WebUI.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IEmailService _emailService;

        public CheckoutController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // GET: /Checkout/Index
        public IActionResult Index()
        {
            // Retrieve the cart from session
            var cartJson = HttpContext.Session.GetString("Cart");
            var cartItems = !string.IsNullOrEmpty(cartJson)
                ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson)
                : new List<CartItem>();

            var viewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                Total = cartItems.Sum(i => i.UnitPrice * i.Quantity)
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(CheckoutViewModel model)
        {
            // Reload cart items in case validation fails
            var cartJson = HttpContext.Session.GetString("Cart");
            var cartItems = !string.IsNullOrEmpty(cartJson)
                ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson)
                : new List<CartItem>();

            model.CartItems = cartItems;
            model.Total = cartItems.Sum(i => i.UnitPrice * i.Quantity);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Use the email service to send confirmation with shipping details
                _emailService.SendOrderConfirmationEmail(model);

                // Clear the cart after a successful checkout
                HttpContext.Session.Remove("Cart");
                TempData["CheckoutAlert"] = "Order placed successfully! A confirmation email has been sent.";
                return RedirectToAction("Confirmation");
            }
            catch
            {
                ModelState.AddModelError("", "There was an error processing your order. Please try again.");
                return View(model);
            }
        }

        // GET: /Checkout/Confirmation
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
