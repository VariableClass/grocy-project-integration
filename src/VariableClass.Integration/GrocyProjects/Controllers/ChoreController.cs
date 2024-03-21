using Microsoft.AspNetCore.Mvc;
using VariableClass.Integration.GrocyProjects.Services;

namespace VariableClass.Integration.GrocyProjects.Controllers;

[ApiController]
[Route("[controller]")]
public class ChoresController(ILogger<ChoresController> logger, ChoreExportService choreExportService) : ControllerBase
{
    private readonly ILogger<ChoresController> _logger = logger;
    private readonly ChoreExportService _choreExportService = choreExportService;

    [HttpGet]
    [Route("sync")]
    public async Task SyncChoresAsync()
    {        
        // Mark any chores completed since last check as complete
        await _choreExportService.CompleteChoresAsync();

        // Add any open chores to the project as tasks
        await _choreExportService.ScheduleChoresAsync();
    }

    [HttpGet]
    [Route("fetch")]
    public async Task FetchChoreUpdatesAsync()
    {
        // Update project management with chore master data updates
        await _choreExportService.FetchChoreUpdatesAsync();
    }
}
