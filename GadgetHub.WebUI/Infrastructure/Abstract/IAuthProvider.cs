namespace GadgetHub.WebUI.Infrastructure.Abstract;

public interface IAuthProvider
{
    Task<bool> AuthenticateAsync(string userName, string password);
    Task SignOutAsync();
}