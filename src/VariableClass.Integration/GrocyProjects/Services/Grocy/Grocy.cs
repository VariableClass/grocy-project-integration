using System.Net.Http.Headers;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Options;
using VariableClass.Integration.GrocyProjects.Configs;
using VariableClass.Integration.GrocyProjects.Models;
using VariableClass.Integration.GrocyProjects.Models.Grocy;

namespace VariableClass.Integration.GrocyProjects.Services.Grocy;

public class Grocy(IOptions<GrocyConfig> config, HttpClient apiClient, IAuthenticationService authenticationService)
  : IChoreProvider
{
  private readonly GrocyConfig _configuration = config.Value;
  private readonly HttpClient _apiClient = apiClient;
  private readonly IAuthenticationService _authenticationService = authenticationService;

  public async Task<IEnumerable<Chore>?> GetChoresDueBetweenAsync(
  DateTimeOffset dueFrom,
  DateTimeOffset dueTo)
  {
    // TODO: Get chore master data and execution schedule in separate calls

    var accessToken = await _authenticationService.GetAccessTokenAsync();
    _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var requestUri = $"{_configuration.InstanceUrl}{_configuration.ChoresEndpoint}?";
    
    // Add Grocy query parameters
    var grocyQueryFilters = new string[] { "active=1" };
    foreach (var grocyQueryFilter in grocyQueryFilters)
    {
      requestUri += $"query[]={grocyQueryFilter}&";
    }

    var choresRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);

    var choresResponse = await _apiClient.SendAsync(choresRequest);
    choresResponse.EnsureSuccessStatusCode();

    var grocyChores = (await choresResponse.Content.ReadFromJsonAsync<IEnumerable<GrocyChore>>())
      ?? throw new Exception();

    ICollection<Chore>? chores = null;
    foreach (var grocyChore in grocyChores)
    {
      chores ??= [];

      var chore = new Chore()
      {
        Id = grocyChore.Id.ToString(),
        Title = grocyChore.ChoreName.Split('(')[0],
        Assignee = grocyChore.Assignee.Username,
        Room = grocyChore.Room
      };

      chores.Add(chore);
    }

    return chores;
  }

  public async Task CompleteChoresAsync(IEnumerable<ChoreExecution> choreExecutions)
  {
    var accessToken = await _authenticationService.GetAccessTokenAsync();
    _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    
    foreach (var choreExecution in choreExecutions)
    {
      // TODO: Get users to retrieve ID

      var grocyChoreExecution = new GrocyChoreExecution(
        choreExecution.ChoreId,
        choreExecution.Assignee == "VariableClass" ? 1 : 4,
        choreExecution.CompletedAt);

      var endpoint = string.Format($"{_configuration.InstanceUrl}{_configuration.ExecuteChoreEndpoint}", choreExecution.ChoreId);

      var choreExecutedRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
      {
        Content = JsonContent.Create(grocyChoreExecution, mediaType: MediaTypeHeaderValue.Parse("application/json"))
      };

      var completedResponse = await _apiClient.SendAsync(choreExecutedRequest);
      completedResponse.EnsureSuccessStatusCode();
    }
  }
}