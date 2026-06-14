using Kitchen.Application.Commands;
using Kitchen.Application.Models.Requests;
using Kitchen.Application.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductDefinitionsController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public ProductDefinitionsController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _catalogService.GetAll());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDefinitionRequest request)
    {
        var command = new AddProductDefinitionCommand(
                request.Name,
                request.Unit,
                request.Category
            );

        await _catalogService.Add(command);

        return CreatedAtAction(nameof(GetAll), new { name = command.Name }, command);

    }

    [HttpPut("{name}")]
    public async Task<IActionResult> Update(string name, [FromBody] UpdateProductDefinitionRequest request)
    {
        var command = new ModifyProductDefinitionCommand(
            name,
            request.Unit,
            request.Category
        );

        await _catalogService.Update(command);

        return NoContent();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        await _catalogService.Delete(name);
        return NoContent();
    }
}