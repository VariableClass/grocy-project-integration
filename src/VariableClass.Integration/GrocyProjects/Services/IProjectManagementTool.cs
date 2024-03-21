using VariableClass.Integration.GrocyProjects.Models;

namespace VariableClass.Integration.GrocyProjects.Services;

public interface IProjectManagementTool
{
  Task UpsertChoresAsync(IEnumerable<Chore> chores);
  Task<IEnumerable<ChoreExecution>?> GetChoreExecutionsSinceAsync(DateTimeOffset lastRun);
  Task ScheduleChoresAsync(IEnumerable<ChoreSchedule> chores);
}