using System.Xml.Linq;
using Kitchen.Api.Models.Entities;
using Kitchen.Api.Services;

public class IngredientCatalogService : IIngredientCatalogService
{
    private readonly List<IngredientDefinition> _ingredientDefinitions = new();

    public IEnumerable<IngredientDefinition> GetAll() => _ingredientDefinitions.AsReadOnly();

    public IngredientDefinition? GetByName(string name) => _ingredientDefinitions.FirstOrDefault(i => i.Name == name);

    public void Add(IngredientDefinition ingredientDefinition)
    {
        _ingredientDefinitions.Add(ingredientDefinition);
    }

    public bool Update(IngredientDefinition ingredient)
    {
        var existing = GetByName(ingredient.Name);
        if (existing == null) return false;

        existing.ChangeUnitType(ingredient.Unit);
        return true;
    }

    public bool Delete(string name)
    {
        var ingredient = GetByName(name);
        return ingredient != null && _ingredientDefinitions.Remove(ingredient);
    }
}