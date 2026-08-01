using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Kitchen.Application.Services;

internal class InventoryService : IInventoryService
{
    private readonly IStockItemRepository _inventoryRepository;
    private readonly IProductDefinitionRepository _catalogRepository;
    private async Task<StockItem> FindStockItem(Guid id)
    {
        var stockItem = await GetById(id);
        if (stockItem == null) throw new StockItemNotFoundException();

        return stockItem;

    }

    public InventoryService(IStockItemRepository repository, IProductDefinitionRepository typeRepository)
    {
        _inventoryRepository = repository;
        _catalogRepository = typeRepository;
    }

    public async Task<IEnumerable<StockItem>> GetAll() => await _inventoryRepository.GetAllWithDetails();

    public async Task<StockItem?> GetById(Guid id) => await _inventoryRepository.GetByIdWithDetails(id);

    public async Task<IEnumerable<StockItem>> GetByName(string name) => await _inventoryRepository.GetByNameWithDetails(name);

    public async Task<StockItem> Add(AddStockItemCommand command)
    {
        var productDefinition = await _catalogRepository.GetByName(command.Name);
        var stockItem = new StockItem(
            command.Name,
            command.Amount,
            command.Location,
            productDefinition,
            command.ExpirationDate
        );
        await _inventoryRepository.Add(stockItem);

        return stockItem;
    }

    public async Task Update(ModifyStockItemCommand command)
    {
        var stockItem = await FindStockItem(command.Id);

        stockItem.SetName(command.Name);
        stockItem.AdjustAmount(command.Amount);
        stockItem.PlaceOrMove(command.Location);
        stockItem.SetExpirationDate(command.ExpirationDate);

        await _inventoryRepository.Update(stockItem);
    }

    public async Task Delete(Guid id)
    {
        var stockItem = await FindStockItem(id);

        await _inventoryRepository.Delete(id);
    }
}