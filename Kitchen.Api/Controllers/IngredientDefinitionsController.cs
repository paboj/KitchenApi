using System.Xml.Linq;
using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Models.DTOs;
using Kitchen.Api.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IngredientDefinitionsController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public IngredientDefinitionsController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_catalogService.GetAll());

    [HttpPost]
    public IActionResult Create([FromBody] CreateIngredientDefinitionRequest request)
    {
        var command = new AddToCatalogCommand(
                request.Name,
                request.Unit
            );

        _catalogService.Add(command);

        return CreatedAtAction(nameof(GetAll), new { name = command.Name }, command);

    }

    [HttpPut("{name}")]
    public IActionResult Update(string name, [FromBody] UpdateIngredientDefinitionRequest request)
    {
        var command = new ModifyInCatalogCommand(
            name,
            request.Unit
        );

        var success = _catalogService.Update(command);

        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{name}")]
    public IActionResult Delete(string name)
    {
        var success = _catalogService.Delete(name);
        return success ? NoContent() : NotFound();
    }
}