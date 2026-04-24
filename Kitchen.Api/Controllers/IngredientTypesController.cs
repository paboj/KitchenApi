using System.Xml.Linq;
using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Application.Models.DTOs;
using Kitchen.Application.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IngredientTypesController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public IngredientTypesController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_catalogService.GetAll());

    [HttpPost]
    public IActionResult Create([FromBody] CreateIngredientTypeRequest request)
    {
        var command = new AddTypeCatalogCommand(
                request.Name,
                request.Unit
            );

        _catalogService.Add(command);

        return CreatedAtAction(nameof(GetAll), new { name = command.Name }, command);

    }

    [HttpPut("{name}")]
    public IActionResult Update(string name, [FromBody] UpdateIngredientTypeRequest request)
    {
        var command = new ModifyTypeCatalogCommand(
            name,
            request.Unit
        );

        _catalogService.Update(command);

        return NoContent();
    }

    [HttpDelete("{name}")]
    public IActionResult Delete(string name)
    {
        _catalogService.Delete(name);
        return NoContent();
    }
}