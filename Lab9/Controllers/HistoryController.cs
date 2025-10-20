using Lab9.Models;
using Lab9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab9.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HistoryController(IHistoryService historyService) : ControllerBase
{
    private readonly IHistoryService _historyService = historyService;

    // GET: api/History
    [HttpGet]
    public Task<IEnumerable<History>> Get()
    {
        return _historyService.GetAllAsync();
    }

    // GET: api/History/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<History>> GetById(Guid id)
    {
        return await _historyService.GetByIdAsync(id) is { } data ? data : NotFound();
    }

    // POST: api/History
    [HttpPost]
    public async Task<ActionResult<History>> Post(History history)
    {
        history = await _historyService.CreateAsync(history);
        return CreatedAtAction(nameof(GetById), new { id = history.Id }, history);
    }

    // PUT: api/History
    [HttpPut]
    public async Task<IActionResult> Update(History history)
    {
        return await _historyService.UpdateAsync(history) ? NoContent() : NotFound();
    }

    // DELETE: api/History/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await _historyService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
