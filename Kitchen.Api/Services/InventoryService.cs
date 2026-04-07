using System.Xml.Linq;
using Kitchen.Api.Models.Entities;
using Kitchen.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly List<Ingredient> _ingredients = new();

    public IEnumerable<Ingredient> GetAll() => _ingredients.AsReadOnly();

    public Ingredient? GetById(Guid id) => _ingredients.FirstOrDefault(i => i.Id == id);

    public void Add(Ingredient ingredient)
    {
        ingredient.Id = Guid.NewGuid();
        _ingredients.Add(ingredient);
    }

    public bool Update(Guid id, Ingredient updatedIngredient)
    {
        var existing = GetById(id);
        if (existing == null) return false;

        existing.Name = updatedIngredient.Name;
        existing.Amount = updatedIngredient.Amount;
        existing.Unit = updatedIngredient.Unit;
        return true;
    }

    public bool Delete(Guid id)
    {
        var ingredient = GetById(id);
        return ingredient != null && _ingredients.Remove(ingredient);
    }
}