using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Kitchen.Application.Services;

internal class InventoryService : IInventoryService
{
    private readonly IStockItemRepository _repository;
    private readonly IProductDefinitionRepository _typeRepository;
    private async Task<StockItem> FindStockItem(Guid id)
    {
        var stockItem = await GetById(id);
        if (stockItem == null) throw new StockItemNotFoundException();

        return stockItem;

    }

    public InventoryService(IStockItemRepository repository, IProductDefinitionRepository typeRepository)
    {
        _repository = repository;
        _typeRepository = typeRepository;
    }

    public async Task<IEnumerable<StockItem>> GetAll() => await _repository.GetAllWithDetails();

    public async Task<StockItem?> GetById(Guid id) => await _repository.GetByIdWithDetails(id);

    public async Task<IEnumerable<StockItem>> GetByName(string name) => await _repository.GetByNameWithDetails(name);

    public async Task Add(AddStockItemCommand command)
    {
        var productDefinition = await _typeRepository.GetByName(command.Name);
        var stockItem = new StockItem(
            command.Name,
            command.Amount,
            command.Location,
            productDefinition
        );
        await _repository.Add(stockItem);
    }

    public async Task Update(ModifyStockItemCommand command)
    {
        var stockItem = await FindStockItem(command.Id);

        stockItem.SetName(command.Name);
        stockItem.AdjustAmount(command.Amount);
        stockItem.PlaceOrMove(command.Location);

        await _repository.Update(stockItem);
    }

    public async Task Delete(Guid id)
    {
        var stockItem = await FindStockItem(id);

        await _repository.Delete(id);
    }
}