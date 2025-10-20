using Lab9.Models;
using Lab9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab9.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController(IProductService historyService) : ControllerBase
{
    private readonly IProductService _historyService = historyService;

    // GET: api/Product
    [HttpGet]
    public Task<IEnumerable<Product>> Get()
    {
        return _historyService.GetAllAsync();
    }

    // GET: api/Product/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(Guid id)
    {
        return await _historyService.GetByIdAsync(id) is { } data ? data : NotFound();
    }

    // POST: api/Product
    [HttpPost]
    public async Task<ActionResult<Product>> Post(Product product)
    {
        product = await _historyService.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
    // PUT: api/Product
    [HttpPut]
    public async Task<IActionResult> Update(Product product)
    {
        return await _historyService.UpdateAsync(product) ? NoContent() : NotFound();
    }

    // DELETE: api/Product/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await _historyService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
