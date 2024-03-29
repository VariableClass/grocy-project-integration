namespace VariableClass.Integration.GrocyProjects.Models;

public class ChoreExecution(string choreId, string assignee, DateTimeOffset completed)
{
    public string ChoreId { get; set; } = choreId;
    public string Assignee { get; set; } = assignee;
    public DateTimeOffset Completed { get; set; } = completed;
}
