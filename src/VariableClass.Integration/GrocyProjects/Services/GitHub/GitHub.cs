using Microsoft.Extensions.Options;
using Octokit;
using VariableClass.Integration.GrocyProjects.Configs;
using VariableClass.Integration.GrocyProjects.Exceptions;
using VariableClass.Integration.GrocyProjects.Models;

namespace VariableClass.Integration.GrocyProjects.Services.GitHub;

public class GitHub(IOptions<GitHubConfig> config, GitHubClient github) : IProjectManagementTool
{
  private const int WaitTimeMs = 1000;

  private readonly GitHubConfig _config = config.Value;
  private readonly GitHubClient _github = github;

  public async Task UpsertChoresAsync(IEnumerable<Chore> chores)
  {
    var issueRequest = new IssueRequest();
    issueRequest.Labels.Add(_config.LabelGrocy);

    var choreIssues = await _github.Issue.GetAllForOrganization(_config.RepositoryOwner, issueRequest);

    var issuesToDelete = choreIssues.Where(issue =>
      !chores.Select(chore => chore.Id).Contains(
        issue.GetChoreId(_config.LabelChoreIdPrefix, _config.LabelChoreIdSeparator)));
    
    foreach (var issueToDelete in issuesToDelete)
    {
      await DeleteChoreAsync(issueToDelete);
    }

    foreach (var chore in chores)
    {
      var issue = choreIssues.SingleOrDefault(choreIssue =>
        choreIssue.GetChoreId(_config.LabelChoreIdPrefix, _config.LabelChoreIdSeparator).Equals(chore.Id));

      if (issue == null)
      {
        await CreateChoreAsync(chore);
        return;
      }
      
      await UpdateChoreAsync(issue, chore);
    }
  }

  private async Task CreateChoreAsync(Chore chore)
  {
    // Set issue title
    var issue = new NewIssue($"{chore.Title} ({chore.Room})");
    
    // Set issue labels
    issue.Labels.Add(_config.LabelGrocy);
    issue.Labels.Add($"{_config.LabelChoreIdPrefix}{_config.LabelChoreIdSeparator}{chore.Id}");
    
    // Set issue assignee
    issue.Assignees.Add(chore.Assignee);

    // TODO: (GraphQL) Set priority

    // TODO: (GraphQL) Set effort

    // Perforem create
    await _github.Issue.Create(_config.RepositoryOwner, _config.Repository, issue);
    Thread.Sleep(WaitTimeMs);
  }

  private async Task UpdateChoreAsync(Issue issue, Chore chore)
  {
    var issueUpdate = issue.ToUpdate();

    // Update title
    var newTitle = $"{chore.Title} ({chore.Room})";
    var titleUpdated = issue.Title != newTitle;
    if (titleUpdated)
    {
      issueUpdate.Title = newTitle;
    }

    // Reassign
    var assigneeUpdated = issue.Assignee.Login != chore.Assignee;
    if (assigneeUpdated)
    {
      issueUpdate.ClearAssignees();
      issueUpdate.AddAssignee(chore.Assignee);
    }

    // TODO: (GraphQL) Update priority
    var priorityUpdated = false;

    // TODO: (GraphQL) Update effort
    var effortUpdated = false;

    // Return if no changes made
    if (!titleUpdated && !assigneeUpdated && !priorityUpdated && !effortUpdated)
    {
      return;
    }

    // Perform the update
    await _github.Issue.Update(
      _config.RepositoryOwner,
      _config.Repository,
      issue.Number,
      issueUpdate);
    Thread.Sleep(WaitTimeMs);
  }

  private async Task OpenChoreAsync(Issue issue, ChoreSchedule choreSchedule)
  {
    var issueUpdate = issue.ToUpdate();

    // Set state to open
    issueUpdate.State = ItemState.Open;
    
    // Update assignee
    issueUpdate.ClearAssignees();
    issueUpdate.AddAssignee(choreSchedule.Assignee);

    // TODO: (GraphQL) Remove assigned due date

    // TODO: (GraphQL) Remove assigned week

    await _github.Issue.Update(_config.RepositoryOwner, _config.Repository, issue.Number, issueUpdate);
    Thread.Sleep(WaitTimeMs);
  }

  private async Task DeleteChoreAsync(Issue issue)
  {
    // TODO: (GraphQL) Implement with GraphQL API

    Thread.Sleep(WaitTimeMs);
  }

  public async Task ScheduleChoresAsync(IEnumerable<ChoreSchedule> choresSchedule)
  {
    var issueRequest = new IssueRequest()
    {
      State = ItemStateFilter.Open
    };
    issueRequest.Labels.Add(_config.LabelGrocy);

    var openIssues = await _github.Issue.GetAllForOrganization(_config.RepositoryOwner, issueRequest);

    // Determine chores to register by excluding those with an ID matching the chore ID label of any open issues
    var choresToSchedule = choresSchedule.ExceptBy(
      openIssues
        .Select(issue => issue.GetChoreId(_config.LabelChoreIdPrefix, _config.LabelChoreIdSeparator)),
      x => x.ChoreId);

    foreach (var choreSchedule in choresToSchedule)
    {
      var issue = openIssues.Single(
        x => x.GetChoreId(_config.LabelChoreIdPrefix, _config.LabelChoreIdSeparator).Equals(choreSchedule.ChoreId));
      await OpenChoreAsync(issue, choreSchedule);
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

    // TODO: (GraphQL) Get only issues for a given project

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