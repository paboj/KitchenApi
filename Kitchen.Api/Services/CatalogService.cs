using System.Xml.Linq;
using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;
using Kitchen.Api.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class CatalogService : ICatalogService
{
    private readonly List<IngredientType> _IngredientTypes = new();

    public IEnumerable<IngredientType> GetAll() => _IngredientTypes.AsReadOnly();

    public IngredientType? GetByName(string name) => _IngredientTypes.FirstOrDefault(i => i.Name == name);

    public void Add(AddTypeCatalogCommand command)
    {
        var definition = new IngredientType(
            command.Name,
            command.Unit
        );
        _IngredientTypes.Add(definition);
    }

    public bool Update(ModifyTypeCatalogCommand command)
    {
        var existing = GetByName(command.Name);
        if (existing == null) return false;

        existing.ChangeUnitType(command.Unit);
        return true;
    }

    public bool Delete(string name)
    {
        var ingredient = GetByName(name);
        return ingredient != null && _IngredientTypes.Remove(ingredient);
    }
}