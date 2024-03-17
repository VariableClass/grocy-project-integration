namespace VariableClass.Integration.GrocyProjects.Configs;

public sealed class GrocyConfig
{
  public const string Key = "Grocy";
  public required string InstanceUrl { get; set; }
  public string ChoresEndpoint { get; set; } = "/api/objects/chores";
  public string ExecuteChoreEndpoint { get; set; } = "/api/chores/{0}/execute"; 
}