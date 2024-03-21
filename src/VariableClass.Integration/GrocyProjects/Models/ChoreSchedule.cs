namespace VariableClass.Integration.GrocyProjects.Models;

public class ChoreSchedule(string choreId, string assignee, DateTimeOffset due)
{
    public string ChoreId { get; set; } = choreId;
    public string Assignee { get; set; } = assignee;
    public DateTimeOffset Due { get; set; } = due;
}
