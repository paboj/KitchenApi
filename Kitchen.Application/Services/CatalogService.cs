using System.Xml.Linq;
using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Exceptions;
using Kitchen.Core.Repositories;
using Kitchen.Application.Services;

internal class CatalogService : ICatalogService
{
    private readonly IIngredientTypeRepository _repository;
    private IngredientType FindIngredientType(string name)
    {
        var ingredientType = GetByName(name);
        if (ingredientType == null) throw new IngredientNotFoundException();

        return ingredientType;

    }

    public CatalogService(IIngredientTypeRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<IngredientType> GetAll() => _repository.GetAll();

    public IngredientType? GetByName(string name) => _repository.GetByName(name);

    public void Add(AddTypeCatalogCommand command)
    {
        var existing = _repository.GetByName(command.Name);
        if (existing != null)
        {
            throw new IngredientTypeAlreadyExistsException();
        }

        var definition = new IngredientType(
            command.Name,
            command.Unit
        );
        _repository.Add(definition);
    }

    public void Update(ModifyTypeCatalogCommand command)
    {
        var ingredientType = FindIngredientType(command.Name);
        ingredientType.ChangeUnitType(command.Unit);
    }

    public void Delete(string name)
    {
        var ingredientType = FindIngredientType(name);
        _repository.Delete(name);
    }
}