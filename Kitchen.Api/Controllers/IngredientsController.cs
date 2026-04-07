using Kitchen.Api.Models.Entities;
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
    public IActionResult Create(Ingredient ingredient)
    {
        _inventoryService.Add(ingredient);
        return CreatedAtAction(nameof(GetAll), new { id = ingredient.Id }, ingredient);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, Ingredient ingredient)
    {
        var success = _inventoryService.Update(id, ingredient);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var success = _inventoryService.Delete(id);
        return success ? NoContent() : NotFound();
    }
}