using GadgetHub.Domain.Data;
using Microsoft.EntityFrameworkCore;
using GadgetHub.WebUI.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Set up the connection string – you can define it in appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=GadgetHub.db";

// Register the DbContext from the Domain project using SQLite:
builder.Services.AddDbContext<GadgetHubContext>(options =>
    options.UseSqlite(connectionString));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Configure routes using the dedicated class.
RouteConfigurator.ConfigureRoutes(app);


app.Run();