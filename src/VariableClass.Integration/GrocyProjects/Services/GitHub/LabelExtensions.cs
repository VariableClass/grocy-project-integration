using Octokit;

namespace VariableClass.Integration.GrocyProjects.Services.GitHub;

public static class IssueExtensions
{
  public static string GetChoreId(this Issue issue, string choreIdPrefix, char choreIdSeparator)
  {
    return issue.Labels.SingleOrDefault(
          label => label.Name.StartsWith(choreIdPrefix)).Name
            .Split(choreIdSeparator)
            .Last();
  }
}