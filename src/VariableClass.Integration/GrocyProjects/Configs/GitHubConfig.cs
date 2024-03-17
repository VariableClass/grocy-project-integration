namespace VariableClass.Integration.GrocyProjects.Configs;

public sealed class GitHubConfig
{
  public const string Key = "GitHub";
  public required string InstanceUrl { get; set; }
  public required string ClientName { get; set; }
  public required string Credential { get; set; }
  public required string RepositoryOwner { get; set; }
  public required string Repository { get; set; }
  public string LabelGrocy { get; set; } = "grocy";
  public string LabelChoreIdPrefix { get; set; } = "chore-id";
  public char LabelChoreIdSeparator { get; set; } = ':';
}