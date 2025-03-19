using GadgetHub.Domain.Data;
using GadgetHub.WebUI.Configuration;
using Microsoft.EntityFrameworkCore;
using GadgetHub.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------
// 1. Configure Services
// ---------------------------------------------

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

var app = builder.Build();

// ---------------------------------------------
// 2. Configure Middleware Pipeline
// ---------------------------------------------

// Use exception handling / HSTS in production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Enforce HTTPS
app.UseHttpsRedirection();

// Serve static files (e.g., CSS, JS, images in wwwroot)
app.UseStaticFiles();

// Enable routing
app.UseRouting();

// (Optional) Enable session
app.UseSession();

// If you have any authentication/authorization in the future:
app.UseAuthorization();

// Map static assets from a custom extension method (if needed)
app.MapStaticAssets();

// Configure routes via the RouteConfigurator
RouteConfigurator.ConfigureRoutes(app);

// ---------------------------------------------
// 3. Run the Application
// ---------------------------------------------
app.Run();