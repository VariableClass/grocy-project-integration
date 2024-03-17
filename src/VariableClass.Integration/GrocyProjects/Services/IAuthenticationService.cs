namespace VariableClass.Integration.GrocyProjects.Services;

public interface IAuthenticationService
{
    Task<string> GetAccessTokenAsync();
}