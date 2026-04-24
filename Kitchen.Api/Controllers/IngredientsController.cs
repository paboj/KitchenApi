using Microsoft.AspNetCore.Mvc;
using Kitchen.Application.Services;
using Kitchen.Application.Models.DTOs;
using Kitchen.Application.Commands;

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

    [HttpGet("{name}")]
    public IActionResult Get(string name)
    {
        var ingredient = _inventoryService.GetByName(name);
        if (ingredient == null) return NotFound();

        return Ok(ingredient);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateIngredientRequest request)
    {
        var command = new AddToStockCommand(
            request.Name,
            request.Amount,
            request.Location
        );

        _inventoryService.Add(command);

        return CreatedAtAction(nameof(Get), new { name = request.Name }, request);
    
}

    [HttpPut("{name}")]
    public IActionResult Update(string name, [FromBody] UpdateIngredientRequest request)
    {
        var command = new ModifyInStockCommand(
            name,
            request.Amount,
            request.Location
        );

        _inventoryService.Update(command);
        return NoContent();
    }

    [HttpDelete("{name}")]
    public IActionResult Delete(string name)
    {
        _inventoryService.Delete(name);
        return NoContent();
    }
}