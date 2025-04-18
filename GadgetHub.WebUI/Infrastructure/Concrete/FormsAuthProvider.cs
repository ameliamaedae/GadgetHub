using System.Security.Claims;
using System.Threading.Tasks;
using GadgetHub.WebUI.Infrastructure.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GadgetHub.WebUI.Infrastructure.Concrete
{
    public class FormsAuthProvider : IAuthProvider
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _ctx;

        public FormsAuthProvider(IConfiguration config, IHttpContextAccessor ctx)
        {
            _config = config;
            _ctx     = ctx;
        }

        public async Task<bool> AuthenticateAsync(string userName, string password)
        {
            var adminUser = _config["AdminCredentials:UserName"];
            var adminPass = _config["AdminCredentials:Password"];

            if (userName == adminUser && password == adminPass)
            {
                var claims   = new[] { new Claim(ClaimTypes.Name, userName) };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await _ctx.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal
                );
                return true;
            }
            return false;
        }

        public Task SignOutAsync()
            => _ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}