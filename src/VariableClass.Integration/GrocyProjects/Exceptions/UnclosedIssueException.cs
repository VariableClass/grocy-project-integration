namespace VariableClass.Integration.GrocyProjects.Exceptions;

public class UnclosedIssueException(string issueId) : Exception
{
    public string IssueId { get; } = issueId;
}