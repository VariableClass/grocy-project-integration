using System.Text.Json.Serialization;

namespace VariableClass.Integration.GrocyProjects.Models.Grocy;

public class GrocyChore
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string ChoreName { get; set; }

    [JsonPropertyName("next_execution_assigned_to_user_id")]
    public required long NextExecutionAssignedToUserId { get; set; }

    [JsonPropertyName("userfields")]
    public required ChoreUserFields UserFields { get; set; }
}

public class ChoreUserFields
{
    [JsonPropertyName("consequences")]
    public required string Consequences { get; set; }

    [JsonPropertyName("effort")]
    public required string Effort { get; set; }

    [JsonPropertyName("room")]
    public required string Room { get; set; }
}
