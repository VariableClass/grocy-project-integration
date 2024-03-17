using VariableClass.Integration.GrocyProjects.Models;

namespace VariableClass.Integration.GrocyProjects.Services;

public interface IChoreProvider
{
  Task<IEnumerable<Chore>?> GetChoresDueBetweenAsync(DateTimeOffset start, DateTimeOffset end);
  Task CompleteChoresAsync(IEnumerable<ChoreExecution> choreExecutions);
}