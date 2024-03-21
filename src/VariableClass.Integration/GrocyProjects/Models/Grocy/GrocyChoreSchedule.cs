using System.Text.Json.Serialization;

namespace VariableClass.Integration.GrocyProjects.Models.Grocy;

public class GrocyChoreSchedule(
    string choreId,
    long nextExecutionAssignedToUserId,
    DateTimeOffset nextEstimatedExecutionTime)
{
    [JsonPropertyName("chore_id")]
    public string ChoreId { get; set; } = choreId;

    [JsonPropertyName("next_execution_assigned_to_user_id")]
    public long NextExecutionAssignedToUserId { get; set; } = nextExecutionAssignedToUserId;

    [JsonPropertyName("next_estimated_execution_time")]
    public DateTimeOffset NextEstimatedExecutionTime { get; set; } = nextEstimatedExecutionTime;
}
