namespace VariableClass.Integration.GrocyProjects.Exceptions;

public class NoChoreAssigneeException(string issueId) : Exception
{
    public string IssueId { get; } = issueId;
}