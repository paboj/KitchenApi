using System.Xml.Linq;
using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Application.Models.Requests;
using Kitchen.Core.Repositories;
using Kitchen.Application.Services;

internal class InventoryService : IInventoryService
{
    private readonly IStockItemRepository _repository;
    private readonly IProductDefinitionRepository _typeRepository;
    private StockItem FindStockItem(string name)
    {
        var stockItem = GetByName(name);
        if (stockItem == null) throw new StockItemNotFoundException();

        return stockItem;

    }

    public InventoryService(IStockItemRepository repository, IProductDefinitionRepository typeRepository)
    {
        _repository = repository;
        _typeRepository = typeRepository;
    }

    public IEnumerable<StockItem> GetAll() => _repository.GetAll();

    public StockItem? GetByName(string name) => _repository.GetByName(name);

    public void Add(AddToStockCommand command)
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

    public void Update(ModifyInStockCommand command)
    {
        var stockItem = FindStockItem(command.Name);

        stockItem.AdjustAmount(command.Amount);
        stockItem.PlaceOrMove(command.Location);

        _repository.Update(stockItem);
    }

    public void Delete(string name)
    {
        var stockItem = FindStockItem(name);

        _repository.Delete(name);
    }
}