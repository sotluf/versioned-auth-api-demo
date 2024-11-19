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


    [HttpGet]
    [ApiVersion("1.0", Deprecated = true)]
    public Task<int> GetV1()
    {
        return _service.GetIntegerAsync();
    }


    [HttpGet]
    [ApiVersion("2.0")]
    public Task<string> GetV2()
    {
        return _service.GetTextAsync();
    }


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
