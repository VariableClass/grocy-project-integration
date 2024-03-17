using Microsoft.AspNetCore.Mvc;
using VariableClass.Integration.GrocyProjects.Services;

namespace VariableClass.Integration.GrocyProjects.Controllers;

[ApiController]
[Route("[controller]")]
public class ChoresController(ILogger<ChoresController> logger, ChoreExportService choreExportService) : ControllerBase
{
    private readonly ILogger<ChoresController> _logger = logger;
    private readonly ChoreExportService _choreExportService = choreExportService;

    [HttpGet(Name = "ExportChores")]
    public async Task ExportChores()
        => await _choreExportService.ExportChoresAsync();
}
