using System.Xml.Linq;
using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class CatalogService : ICatalogService
{
    private readonly List<IngredientDefinition> _ingredientDefinitions = new();

    public IEnumerable<IngredientDefinition> GetAll() => _ingredientDefinitions.AsReadOnly();

    public IngredientDefinition? GetByName(string name) => _ingredientDefinitions.FirstOrDefault(i => i.Name == name);

    public void Add(AddToCatalogCommand command)
    {
        var definition = new IngredientDefinition(
            command.Name,
            command.Unit
        );
        _ingredientDefinitions.Add(definition);
    }

    public bool Update(ModifyInCatalogCommand command)
    {
        var existing = GetByName(command.Name);
        if (existing == null) return false;

        existing.ChangeUnitType(command.Unit);
        return true;
    }

    public bool Delete(string name)
    {
        var ingredient = GetByName(name);
        return ingredient != null && _ingredientDefinitions.Remove(ingredient);
    }
}