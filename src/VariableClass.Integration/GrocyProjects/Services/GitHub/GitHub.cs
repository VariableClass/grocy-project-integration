using Microsoft.Extensions.Options;
using Octokit;
using VariableClass.Integration.GrocyProjects.Configs;
using VariableClass.Integration.GrocyProjects.Exceptions;
using VariableClass.Integration.GrocyProjects.Models;

namespace VariableClass.Integration.GrocyProjects.Services.GitHub;

public class GitHub(IOptions<GitHubConfig> config, GitHubClient github) : IProjectManagementTool
{
  private readonly GitHubConfig _config = config.Value;
  private readonly GitHubClient _github = github;

  public async Task RegisterChoreAsync(Chore chore)
  {
    var issue = new NewIssue($"{chore.Title} ({chore.Room})");
    issue.Labels.Add(_config.LabelGrocy);
    issue.Labels.Add($"{_config.LabelChoreIdPrefix}{_config.LabelChoreIdSeparator}{chore.Id}");
    issue.Assignees.Add(chore.Assignee);

    await _github.Issue.Create(_config.RepositoryOwner, _config.Repository, issue);

    Thread.Sleep(1000);

    // TODO: Append project metadata
  }

  public async Task RegisterChoresAsync(IEnumerable<Chore> chores)
  {
    var issueRequest = new IssueRequest()
    {
      State = ItemStateFilter.Open
    };
    issueRequest.Labels.Add(_config.LabelGrocy);

    var openIssues = await _github.Issue.GetAllForOrganization(_config.RepositoryOwner, issueRequest);

    // Determine chores to register by excluding those with an ID matching the chore ID label of any open issues
    var choresToRegister = chores.ExceptBy(
      openIssues.Select(
        issue => issue.Labels.Single(
          label => label.Name.StartsWith(_config.LabelChoreIdPrefix)).Name
            .Split(_config.LabelChoreIdSeparator)[1]),
      x => x.Id);

    foreach (var chore in choresToRegister)
    {
      await RegisterChoreAsync(chore);
    }
  }

  public async Task<IEnumerable<ChoreExecution>?> GetChoreExecutionsSinceAsync(DateTimeOffset lastRun)
  {

    var issueRequest = new IssueRequest()
    {
      State = ItemStateFilter.Closed
    };
    issueRequest.Labels.Add(_config.LabelGrocy);

    var closedIssues = await _github.Issue.GetAllForOrganization(_config.RepositoryOwner, issueRequest);

    // TODO: Get only issues for a given project

    ICollection<ChoreExecution>? choreExecutions = null;

    foreach (var issue in closedIssues)
    {
      var issueId = issue.Id.ToString();

      if (!issue.ClosedAt.HasValue)
      {
        throw new UnclosedIssueException(issueId);
      }

      var choreIdLabel = issue.Labels.SingleOrDefault(x => x.Name.StartsWith(_config.LabelChoreIdPrefix))
        ?? throw new ChoreIdMissingException(issueId);

      var choreId = choreIdLabel.Name.Split(_config.LabelChoreIdSeparator).Last()
        ?? throw new ChoreIdMissingException(issueId);

      var assignee = issue.Assignee.Login
        ?? throw new NoChoreAssigneeException(issueId);

      var choreExecution = new ChoreExecution(choreId, assignee, issue.ClosedAt.Value);

      choreExecutions ??= [];
      choreExecutions.Add(choreExecution);
    }

    return choreExecutions;
  }
}