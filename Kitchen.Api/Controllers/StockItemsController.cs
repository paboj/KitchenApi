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
    public async Task<IActionResult> GetAll() => Ok(await _inventoryService.GetAll());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var stockitem = await _inventoryService.GetById(id);
        if (stockitem == null) return NotFound();

        return Ok(stockitem);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name)
    {
        var stockitems = await _inventoryService.GetByName(name);
        if (!stockitems.Any()) return NotFound();

        return Ok(stockitems);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStockItemRequest request)
    {
        var command = new AddStockItemCommand(
            request.Name,
            request.Amount,
            request.Location
        );

        await _inventoryService.Add(command);

        return CreatedAtAction(nameof(Get), new { name = request.Name }, request);
    
}

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStockItemRequest request)
    {
        var command = new ModifyStockItemCommand(
            id,
            request.Name,
            request.Amount,
            request.Location
        );

        await _inventoryService.Update(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _inventoryService.Delete(id);
        return NoContent();
    }
}