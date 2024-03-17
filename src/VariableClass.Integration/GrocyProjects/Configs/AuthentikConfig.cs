namespace VariableClass.Integration.GrocyProjects.Configs;

public sealed class AuthentikConfig
{
  public const string Key = "Authentik";
  public required string InstanceUrl { get; set; }
  public required string TokenEndpoint { get; set; } = "/application/o/token/";
  public required string ClientId { get; set; }
  public required string Username { get; set; }
  public required string Password{ get; set; }
}