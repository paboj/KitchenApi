using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Kitchen.Application.Services;

internal class CatalogService : ICatalogService
{
    private readonly IProductDefinitionRepository _catalogRepository;
    private readonly IStockItemRepository _inventoryRepository;
    private async Task<ProductDefinition> FindProductDefinition(string name)
    {
        var productDefinition = await GetByName(name);
        if (productDefinition == null) throw new ProductDefinitionNotFoundException();

        return productDefinition;

    }

    public CatalogService(IProductDefinitionRepository repository, IStockItemRepository stockItemRepository)
    {
        _catalogRepository = repository;
        _inventoryRepository = stockItemRepository;
    }

    public async Task<IEnumerable<ProductDefinition>> GetAll() =>  await _catalogRepository.GetAll();

    public async Task<ProductDefinition?> GetByName(string name) => await _catalogRepository.GetByName(name);

    public async Task Add(AddProductDefinitionCommand command)
    {
        var existing = await _catalogRepository.GetByName(command.Name);
        if (existing != null) throw new ProductDefinitionAlreadyExistsException();

        var definition = new ProductDefinition(
            command.Name,
            command.Unit,
            command.Category
        );
        await _catalogRepository.Add(definition);

        await LinkToExistingStockItems(definition);
    }

    public async Task Update(ModifyProductDefinitionCommand command)
    {
        var productDefinition = await FindProductDefinition(command.Name);
        productDefinition.ChangeUnitType(command.Unit);
        productDefinition.SetCategory(command.Category);

        await _catalogRepository.Update(productDefinition);
    }

    public async Task Delete(string name)
    {
        var productDefinition = await FindProductDefinition(name);
        await _catalogRepository.Delete(name);
    }

    public async Task LinkToExistingStockItems(ProductDefinition productDefinition)
    {
        var stockItems = await _inventoryRepository.GetAll();

        foreach (var stockItem in stockItems)
        {
            if (productDefinition.Name == stockItem.Name && stockItem.Type == null)
            {
                stockItem.AssignType(productDefinition);
                await _inventoryRepository.Update(stockItem);
            }
        }
    }
}