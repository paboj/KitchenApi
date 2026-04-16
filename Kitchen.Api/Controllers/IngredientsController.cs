using System.Xml.Linq;
using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Models.DTOs;
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
        var command = new ModifyIngredientCommand(
            name,
            request.Amount,
            request.Location
        );

        var success = _inventoryService.Update(command);

        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{name}")]
    public IActionResult Delete(string name)
    {
        var success = _inventoryService.Delete(name);
        return success ? NoContent() : NotFound();
    }
}