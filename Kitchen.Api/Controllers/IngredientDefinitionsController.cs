using Kitchen.Api.Models.DTOs;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IngredientDefinitionsController : ControllerBase
{
    private readonly IIngredientCatalogService _ingredientCatalogService;

    public IngredientDefinitionsController(IIngredientCatalogService ingredientCatalogService)
    {
        _ingredientCatalogService = ingredientCatalogService;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_ingredientCatalogService.GetAll());

    [HttpPost]
    public IActionResult Create([FromBody] IngredientDefinitionDto request)
    {
        var ingredientDefinition = new IngredientDefinition(request.Name, request.Unit);

        _ingredientCatalogService.Add(ingredientDefinition);
        return CreatedAtAction(nameof(GetAll), new { name = ingredientDefinition.Name }, ingredientDefinition);
    }

    [HttpPut("{name}")]
    public IActionResult Update(string name, [FromBody] IngredientDefinitionDto request)
    {
        var ingredient = new IngredientDefinition(name, request.Unit);

        var success = _ingredientCatalogService.Update(ingredient);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{name}")]
    public IActionResult Delete(string name)
    {
        var success = _ingredientCatalogService.Delete(name);
        return success ? NoContent() : NotFound();
    }
}