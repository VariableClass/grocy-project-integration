namespace VariableClass.Integration.GrocyProjects.Services;

public class ChoreExportService(IChoreProvider choreProvider, IProjectManagementTool projectManagementTool)
{
  private const int DefaultRunIntervalMins = 30;
  private const int IterationLengthDays = 7;
  private readonly IChoreProvider _choreProvider = choreProvider;
  private readonly IProjectManagementTool _projectManagementTool = projectManagementTool;

  public async Task ExportChoresAsync()
  {
    // TODO: Get last run time (once a week)
    var lastRun = DateTimeOffset.UtcNow.AddMinutes(-DefaultRunIntervalMins);

    var closedChores = await _projectManagementTool.GetChoreExecutionsSinceAsync(lastRun);

    if (closedChores != null)
    {
      await _choreProvider.CompleteChoresAsync(closedChores);
    }

    // TODO: Get iteration dates 
    var today = DateTime.Today;
    var daysUntilMonday = ((int) DayOfWeek.Monday - (int) today.DayOfWeek + 7) % 7;
    DateTimeOffset comingMonday = today.AddDays(daysUntilMonday);

    var upcomingChores = await _choreProvider.GetChoresDueBetweenAsync(
      comingMonday,
      comingMonday.AddDays(IterationLengthDays));
    
    if (upcomingChores != null)
    {
      await _projectManagementTool.RegisterChoresAsync(upcomingChores);
    }
  }
}