namespace VariableClass.Integration.GrocyProjects.Exceptions;

public class ChoreIdMissingException(string issueId) : Exception
{
    public string IssueId { get; } = issueId;
}