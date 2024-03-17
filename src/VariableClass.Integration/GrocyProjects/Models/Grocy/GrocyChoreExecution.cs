using System.Text.Json.Serialization;

namespace VariableClass.Integration.GrocyProjects.Models.Grocy;

public class GrocyChoreExecution(string choreId, int doneBy, DateTimeOffset trackedTime)
{
  [JsonPropertyName("id")]
  public string ChoreId { get; set; } = choreId;
  
  [JsonPropertyName("done_by")]
  public int DoneBy { get; set; } = doneBy;

  [JsonPropertyName("tracked_time")]
  public DateTimeOffset TrackedTime { get; set; } = trackedTime;
}
