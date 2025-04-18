using GadgetHub.Domain.Data;
using GadgetHub.Domain.Repositories;
using GadgetHub.WebUI.Configuration;
using GadgetHub.WebUI.Infrastructure.Abstract;
using GadgetHub.WebUI.Infrastructure.Concrete;
using GadgetHub.WebUI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------
// 1. Configure Services
// ---------------------------------------------

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddScoped<IAuthProvider, FormsAuthProvider>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// Register MVC controllers with views
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

// Register DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=GadgetHub.db";
builder.Services.AddDbContext<GadgetHubContext>(options =>
    options.UseSqlite(connectionString));

// (Optional) Add Session support
builder.Services.AddSession(options =>
{
    // options.IdleTimeout = TimeSpan.FromMinutes(20);
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProductRepository, EFProductRepository>();

var app = builder.Build();

// ---------------------------------------------
// 2. Configure Middleware Pipeline
// ---------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Configure routes via the RouteConfigurator
RouteConfigurator.ConfigureRoutes(app);

// ---------------------------------------------
// 3. Run the Application
// ---------------------------------------------
app.Run();