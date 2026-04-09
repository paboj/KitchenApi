using Kitchen.Api.Models.DTOs;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public IngredientsController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_inventoryService.GetAll());

    [HttpPost]
    public IActionResult Create([FromBody] IngredientDto request)
    {
        var ingredient = new Ingredient(request.Name, request.Amount, request.Location);

        _inventoryService.Add(ingredient);
        return CreatedAtAction(nameof(GetAll), new { id = ingredient.Id }, ingredient);
    }

    [HttpPut("{name}")]
    public IActionResult Update(string name, [FromBody] IngredientDto request)
    {
        var ingredient = new Ingredient(name, request.Amount, request.Location);

        var success = _inventoryService.Update(ingredient);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{name}")]
    public IActionResult Delete(string name)
    {
        var success = _inventoryService.Delete(name);
        return success ? NoContent() : NotFound();
    }
}