using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Kitchen.Application.Services;

internal class InventoryService : IInventoryService
{
    private readonly IStockItemRepository _repository;
    private readonly IProductDefinitionRepository _typeRepository;
    private StockItem FindStockItem(Guid id)
    {
        var stockItem = GetById(id);
        if (stockItem == null) throw new StockItemNotFoundException();

        return stockItem;

    }

    public InventoryService(IStockItemRepository repository, IProductDefinitionRepository typeRepository)
    {
        _repository = repository;
        _typeRepository = typeRepository;
    }

    public IEnumerable<StockItem> GetAll() => _repository.GetAllWithDetails();

    public StockItem? GetById(Guid id) => _repository.GetByIdWithDetails(id);

    public IEnumerable<StockItem> GetByName(string name) => _repository.GetByNameWithDetails(name);

    public void Add(AddStockItemCommand command)
    {
        var productDefinition = _typeRepository.GetByName(command.Name);
        var stockItem = new StockItem(
            command.Name,
            command.Amount,
            command.Location,
            productDefinition
        );
        _repository.Add(stockItem);
    }

    public void Update(ModifyStockItemCommand command)
    {
        var stockItem = FindStockItem(command.Id);

        stockItem.SetName(command.Name);
        stockItem.AdjustAmount(command.Amount);
        stockItem.PlaceOrMove(command.Location);

        _repository.Update(stockItem);
    }

    public void Delete(Guid id)
    {
        var stockItem = FindStockItem(id);

        _repository.Delete(id);
    }
}