using VariableClass.Integration.GrocyProjects.Models;

namespace VariableClass.Integration.GrocyProjects.Services;

public interface IProjectManagementTool
{
  Task<IEnumerable<ChoreExecution>?> GetChoreExecutionsSinceAsync(DateTimeOffset lastRun);
  Task RegisterChoreAsync(Chore chore);
  Task RegisterChoresAsync(IEnumerable<Chore> chores);
}