using System.Text.Json.Serialization;

namespace VariableClass.Integration.GrocyProjects.Models.Grocy;

public class GrocyChore
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }
    
    [JsonPropertyName("chore_name")]
    public required string ChoreName { get; set; }

    [JsonPropertyName("next_execution_assigned_user")]
    public required GrocyAssignee Assignee { get; set; }

    [JsonPropertyName("room")]
    public required string Room { get; set; }
}
