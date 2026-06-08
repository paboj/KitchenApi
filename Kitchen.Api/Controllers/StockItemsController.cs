using Microsoft.AspNetCore.Mvc;
using Kitchen.Application.Services;
using Kitchen.Application.Models.Requests;
using Kitchen.Application.Commands;

[ApiController]
[Route("api/[controller]")]
public class StockItemsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public StockItemsController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_inventoryService.GetAll());

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var stockitem = _inventoryService.GetById(id);
        if (stockitem == null) return NotFound();

        return Ok(stockitem);
    }

    [HttpGet("{name}")]
    public IActionResult Get(string name)
    {
        var stockitems = _inventoryService.GetByName(name);
        if (!stockitems.Any()) return NotFound();

        return Ok(stockitems);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateStockItemRequest request)
    {
        var command = new AddStockItemCommand(
            request.Name,
            request.Amount,
            request.Location
        );

        _inventoryService.Add(command);

        return CreatedAtAction(nameof(Get), new { name = request.Name }, request);
    
}

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] UpdateStockItemRequest request)
    {
        var command = new ModifyStockItemCommand(
            id,
            request.Name,
            request.Amount,
            request.Location
        );

        _inventoryService.Update(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _inventoryService.Delete(id);
        return NoContent();
    }
}