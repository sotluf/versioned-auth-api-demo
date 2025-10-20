using Lab9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab9.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VersionedController(IVersionedService service) : ControllerBase
{
    private readonly IVersionedService _service = service;

    // Version 1.0 returns an integer
    [HttpGet]
    [ApiVersion("1.0", Deprecated = true)]
    public Task<int> GetV1()
    {
        return _service.GetIntegerAsync();
    }

    // Version 2.0 returns a string
    [HttpGet]
    [ApiVersion("2.0")]
    public Task<string> GetV2()
    {
        return _service.GetTextAsync();
    }

    // Version 3.0 returns an Excel file
    [HttpGet]
    [ApiVersion("3.0")]
    public async Task<IActionResult> GetV3()
    {
        return File(
            await _service.GenerateExcelAsync(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "GeneratedFile.xlsx"
        );
    }
}
