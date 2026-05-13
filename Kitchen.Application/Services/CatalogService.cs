using System.Xml.Linq;
using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Kitchen.Application.Services;

internal class CatalogService : ICatalogService
{
    private readonly IProductDefinitionRepository _catalogRepository;
    private readonly IStockItemRepository _inventoryRepository;
    private ProductDefinition FindProductDefinition(string name)
    {
        var productDefinition = GetByName(name);
        if (productDefinition == null) throw new StockItemNotFoundException();

        return productDefinition;

    }

    public CatalogService(IProductDefinitionRepository repository, IStockItemRepository stockItemRepository)
    {
        _catalogRepository = repository;
        _inventoryRepository = stockItemRepository;
    }

    public IEnumerable<ProductDefinition> GetAll() => _catalogRepository.GetAll();

    public ProductDefinition? GetByName(string name) => _catalogRepository.GetByName(name);

    public void Add(AddTypeCatalogCommand command)
    {
        var existing = _catalogRepository.GetByName(command.Name);
        if (existing != null)
        {
            throw new ProductDefinitionAlreadyExistsException();
        }

        var definition = new ProductDefinition(
            command.Name,
            command.Unit,
            command.Category
        );
        _catalogRepository.Add(definition);
    }

    public void Update(ModifyTypeCatalogCommand command)
    {
        var productDefinition = FindProductDefinition(command.Name);
        productDefinition.ChangeUnitType(command.Unit);
    }

    public void Delete(string name)
    {
        var productDefinition = FindProductDefinition(name);
        _catalogRepository.Delete(name);
    }

    public void LinkMetadataToExistingStockItems(ProductDefinition productDefinition)
    {
        var orphanedStockItems = _inventoryRepository.GetAll(); //TODO: fetch with null type

        foreach (var stockItem in orphanedStockItems)
        {
            stockItem.AssignType(productDefinition);
        }
    }
}