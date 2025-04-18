using System.Threading.Tasks;
using GadgetHub.WebUI.Infrastructure.Abstract;
using GadgetHub.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHub.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthProvider _auth;

        public AccountController(IAuthProvider auth) => _auth = auth;

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(vm);

            if (await _auth.AuthenticateAsync(vm.UserName, vm.Password))
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(vm);
        }

        public async Task<IActionResult> Logout()
        {
            await _auth.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}