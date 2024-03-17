using System.Text.Json.Serialization;

namespace VariableClass.Integration.GrocyProjects.Models.Grocy;

public class GrocyAssignee
{    
    [JsonPropertyName("username")]
    public required string Username { get; set; }
}
