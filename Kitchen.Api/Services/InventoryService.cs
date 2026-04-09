using System.Xml.Linq;
using Kitchen.Api.Models.Entities;
using Kitchen.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly List<Ingredient> _ingredients = new();

    public IEnumerable<Ingredient> GetAll() => _ingredients.AsReadOnly();

    public Ingredient? GetByName(string name) => _ingredients.FirstOrDefault(i => i.Name == name);

    public void Add(Ingredient ingredient)
    {
        _ingredients.Add(ingredient);
    }

    public bool Update(Ingredient ingredient)
    {
        var existing = GetByName(ingredient.Name);
        if (existing == null) return false;

        existing.AdjustAmount(ingredient.Amount);
        existing.PlaceOrMove(ingredient.Location);
        return true;
    }

    public bool Delete(string name)
    {
        var ingredient = GetByName(name);
        return ingredient != null && _ingredients.Remove(ingredient);
    }
}