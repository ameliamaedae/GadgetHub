namespace GadgetHub.WebUI.Configuration;

public static class RouteConfigurator
{
    public static void ConfigureRoutes(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");

        // Additional route configurations can go here.
    }
}